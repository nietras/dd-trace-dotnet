// <copyright file="HttpHeaderNames.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using System;

namespace Datadog.Trace
{
    /// <summary>
    /// Names of HTTP headers that can be used tracing inbound or outbound HTTP requests.
    /// </summary>
    public static class HttpHeaderNames
    {
        /// <summary>
        /// ID of a distributed trace.
        /// </summary>
        public const string TraceId = "x-datadog-trace-id";

        /// <summary>
        /// ID of the parent span in a distributed trace.
        /// </summary>
        public const string ParentId = "x-datadog-parent-id";

        /// <summary>
        /// Setting used to determine whether a trace should be sampled or not.
        /// </summary>
        public const string SamplingPriority = "x-datadog-sampling-priority";

        /// <summary>
        /// If header is set to "false", tracing is disabled for that http request.
        /// Tracing is enabled by default.
        /// </summary>
        public const string TracingEnabled = "x-datadog-tracing-enabled";

        /// <summary>
        /// Origin of the distributed trace.
        /// </summary>
        public const string Origin = "x-datadog-origin";

        /// <summary>
        /// The user agent that originated an http request.
        /// </summary>
        public const string UserAgent = "User-Agent";

        /// <summary>
        /// Deprecated.
        /// </summary>
        [Obsolete]
        public const string DatadogTags = PropagatedTags;

        /// <summary>
        /// Internal Datadog tags.
        /// A collection of internal Datadog tags. Only trace-level tags with
        /// the "_dd.p.*" prefix will be propagated using this header.
        /// </summary>
        internal const string PropagatedTags = "x-datadog-tags";

        /// <summary>
        /// ID of a span.
        /// Used in a serverless context only.
        /// </summary>
        internal const string SpanId = "x-datadog-span-id";

        /// <summary>
        /// If header is set to "true", the extension will know that the current invocation has failed
        /// Used in a serverless context only.
        /// </summary>
        internal const string InvocationError = "x-datadog-invocation-error";

        /// <summary>
        /// Contains the error.msg span tag.
        /// Used in a serverless context only.
        /// </summary>
        internal const string InvocationErrorMsg = "x-datadog-invocation-error-msg";

        /// <summary>
        /// Contains the error.type span tag.
        /// Used in a serverless context only.
        /// </summary>
        internal const string InvocationErrorType = "x-datadog-invocation-error-type";

        /// <summary>
        /// Contains the error.stack span tag.
        /// Used in a serverless context only.
        /// </summary>
        internal const string InvocationErrorStack = "x-datadog-invocation-error-stack";

        /// <summary>
        /// An internal, temporary, header for tracking the start time of a span
        /// </summary>
        internal const string InternalStartTime = "x-datadog-span-start-timestamp";
    }
}
