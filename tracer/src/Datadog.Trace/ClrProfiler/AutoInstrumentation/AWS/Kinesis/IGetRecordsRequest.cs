// <copyright file="IGetRecordsRequest.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable

using System.Collections;

namespace Datadog.Trace.ClrProfiler.AutoInstrumentation.AWS.Kinesis
{
    /// <summary>
    /// GetRecordsRequest interface for duck typing.
    /// </summary>
    internal interface IGetRecordsRequest
    {
        string StreamARN { get; }
    }
}
