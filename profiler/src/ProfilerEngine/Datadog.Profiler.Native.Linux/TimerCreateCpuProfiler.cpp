// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2022 Datadog, Inc.

#include "TimerCreateCpuProfiler.h"

#include "CpuTimeProvider.h"
#include "IManagedThreadList.h"
#include "Log.h"
#include "OpSysTools.h"
#include "ProfilerSignalManager.h"

#include <libunwind.h>
#include <sys/syscall.h> /* Definition of SYS_* constants */
#include <sys/types.h>
#include <ucontext.h>
#include <unistd.h>

TimerCreateCpuProfiler* TimerCreateCpuProfiler::Instance = nullptr;

TimerCreateCpuProfiler::TimerCreateCpuProfiler(
    IConfiguration* pConfiguration,
    ProfilerSignalManager* pSignalManager,
    IManagedThreadList* pManagedThreadsList,
    CpuTimeProvider* pProvider,
    CallstackProvider callstackProvider) noexcept
    :
    _pSignalManager{pSignalManager}, // put it as parameter for better testing
    _pManagedThreadsList{pManagedThreadsList},
    _pProvider{pProvider},
    _callstackProvider{std::move(callstackProvider)},
    _samplingInterval{pConfiguration->GetCpuProfilingInterval()}
{
    Log::Info("Cpu profiling interval: ", _samplingInterval.count(), "ms");
    Log::Info("timer_create Cpu profiler is enabled");
}

TimerCreateCpuProfiler::~TimerCreateCpuProfiler()
{
    Stop();
}

void TimerCreateCpuProfiler::RegisterThread(std::shared_ptr<ManagedThreadInfo> threadInfo)
{
    std::shared_lock lock(_registerLock);

    if (GetState() != ServiceBase::State::Started)
    {
        return;
    }

    RegisterThreadImpl(threadInfo.get());
}

void TimerCreateCpuProfiler::UnregisterThread(std::shared_ptr<ManagedThreadInfo> threadInfo)
{
    std::shared_lock lock(_registerLock);
    UnregisterThreadImpl(threadInfo.get());
}

const char* TimerCreateCpuProfiler::GetName()
{
    return "timer_create-based Cpu Profiler";
}

bool TimerCreateCpuProfiler::StartImpl()
{
    if (_pSignalManager == nullptr)
    {
        Log::Info("Profiler Signal manager was not correctly initialized (see previous messages).",
                  "timer_create-based CPU profiler is disabled.");
        return false;
    }

    // If the signal is hijacked, what to do?
    auto registered = _pSignalManager->RegisterHandler(TimerCreateCpuProfiler::CollectStackSampleSignalHandler);

    if (registered)
    {
        std::unique_lock lock(_registerLock);
        Instance = this;

        // Create and start timer for all threads.
        _pManagedThreadsList->ForEach([this](ManagedThreadInfo* thread) { RegisterThreadImpl(thread); });
    }

    return registered;
}

bool TimerCreateCpuProfiler::StopImpl()
{
    {
        std::unique_lock lock(_registerLock);

        // We have to remove all timers before unregistering the handler for SIGPROF.
        // Otherwise, the process will end with exit code 155 (128 + 27 => 27 being SIGPROF value)
        _pManagedThreadsList->ForEach([this](ManagedThreadInfo* thread) { UnregisterThreadImpl(thread); });
    }

    Instance = nullptr;
    _pSignalManager->UnRegisterHandler();

    return true;
}

bool TimerCreateCpuProfiler::CollectStackSampleSignalHandler(int sig, siginfo_t* info, void* ucontext)
{
    auto instance = Instance;
    if (instance == nullptr)
    {
        return false;
    }

    return instance->Collect(ucontext);
}

// This symbol is defined in the Datadog.Linux.ApiWrapper. It allows us to check if the thread to be profiled
// contains a frame of a function that might cause a deadlock.
extern "C" unsigned long long dd_inside_wrapped_functions() __attribute__((weak));

bool TimerCreateCpuProfiler::CanCollect(void* ctx)
{
    // TODO (in another PR): add metrics about reasons we could not collect
    if (dd_inside_wrapped_functions != nullptr && dd_inside_wrapped_functions() != 0)
    {
        return false;
    }

    auto* context = reinterpret_cast<ucontext_t*>(ctx);
    // If SIGSEGV is part of the sigmask set, it means that the thread was executing
    // the SIGSEGV signal handler (or someone blocks SIGSEGV signal for this thread,
    // but that less likely)
    if (sigismember(&(context->uc_sigmask), SIGSEGV) == 1)
    {
        return false;
    }

    return true;
}

struct ErrnoSaveAndRestore
{
public:
    ErrnoSaveAndRestore() :
        _oldErrno{errno}
    {
    }
    ~ErrnoSaveAndRestore()
    {
        errno = _oldErrno;
    }

private:
    int _oldErrno;
};

struct StackWalkLock
{
public:
    StackWalkLock(std::shared_ptr<ManagedThreadInfo> threadInfo) :
        _threadInfo{std::move(threadInfo)}
    {
        // Do not call lock while being in the signal handler otherwise
        // we might end in a deadlock situation (lock inversion...)
        _lockTaken = _threadInfo->TryAcquireLock();
    }

    ~StackWalkLock()
    {
        if (_lockTaken)
        {
            _threadInfo->ReleaseLock();
        }
    }

    bool IsLockAcquired() const
    {
        return _lockTaken;
    }

private:
    std::shared_ptr<ManagedThreadInfo> _threadInfo;
    bool _lockTaken;
};

bool TimerCreateCpuProfiler::Collect(void* ctx)
{
    auto threadInfo = ManagedThreadInfo::CurrentThreadInfo;
    if (threadInfo == nullptr)
    {
        // Ooops should never happen
        return false;
    }

    StackWalkLock l(threadInfo);
    if (!l.IsLockAcquired())
    {
        return false;
    }

    if (!CanCollect(ctx))
    {
        return false;
    }

    // Libunwind can overwrite the value of errno - save it beforehand and restore it at the end
    ErrnoSaveAndRestore errnoScope;

    auto callstack = _callstackProvider.Get();

    if (callstack.Capacity() <= 0)
    {
        return false;
    }

    auto buffer = callstack.Data();
    auto* context = reinterpret_cast<unw_context_t*>(ctx);
    auto count = unw_backtrace2((void**)buffer.data(), buffer.size(), context, UNW_INIT_SIGNAL_FRAME);
    callstack.SetCount(count);

    if (count == 0)
    {
        // TODO a metric on event without callstack ?
        return false;
    }

    RawCpuSample rawCpuSample;

    std::tie(rawCpuSample.LocalRootSpanId, rawCpuSample.SpanId) = threadInfo->GetTracingContext();

    rawCpuSample.Timestamp = OpSysTools::GetTimestampSafe();
    rawCpuSample.AppDomainId = threadInfo->GetAppDomainId();
    rawCpuSample.Stack = std::move(callstack);
    rawCpuSample.ThreadInfo = std::move(threadInfo);
    rawCpuSample.Duration = _samplingInterval.count();
    _pProvider->Add(std::move(rawCpuSample));

    return true;
}

void TimerCreateCpuProfiler::RegisterThreadImpl(ManagedThreadInfo* threadInfo)
{
    auto timerId = threadInfo->GetTimerId();
    auto tid = threadInfo->GetOsThreadId();

    if (timerId != -1)
    {
        // already register (lost the race)
        Log::Debug("Timer was already created for thread ", tid);
        return;
    }

    Log::Debug("Creating timer for thread ", tid);

    struct sigevent sev;
    sev.sigev_value.sival_ptr = nullptr;
    sev.sigev_signo = _pSignalManager->GetSignal();
    sev.sigev_notify = SIGEV_THREAD_ID;
    ((int*)&sev.sigev_notify)[1] = tid;

    // Use raw syscalls, since libc wrapper allows only predefined clocks
    clockid_t clock = ((~tid) << 3) | 6; // CPUCLOCK_SCHED | CPUCLOCK_PERTHREAD_MASK thread_cpu_clock(tid);
    if (syscall(__NR_timer_create, clock, &sev, &timerId) < 0)
    {
        Log::Error("Call to timer_create failed for thread ", tid);
        return;
    }

    threadInfo->SetTimerId(timerId);

    std::int32_t _interval = std::chrono::duration_cast<std::chrono::nanoseconds>(_samplingInterval).count();
    struct itimerspec ts;
    ts.it_interval.tv_sec = (time_t)(_interval / 1000000000);
    ts.it_interval.tv_nsec = _interval % 1000000000;
    ts.it_value = ts.it_interval;
    syscall(__NR_timer_settime, timerId, 0, &ts, nullptr);
}

void TimerCreateCpuProfiler::UnregisterThreadImpl(ManagedThreadInfo* threadInfo)
{
    auto timerId = threadInfo->SetTimerId(-1);

    if (timerId != -1)
    {
        Log::Debug("Unregister timer for thread ", threadInfo->GetOsThreadId());
        syscall(__NR_timer_delete, timerId);
    }
}