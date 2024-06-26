﻿// <copyright file="AgentTelemetryTransport.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable
using System;
using Datadog.Trace.Agent;
using Datadog.Trace.Telemetry.Metrics;

namespace Datadog.Trace.Telemetry.Transports;

internal class AgentTelemetryTransport : JsonTelemetryTransport
{
    public AgentTelemetryTransport(IApiRequestFactory requestFactory, bool debugEnabled)
        : base(requestFactory, debugEnabled)
    {
    }

    public override string GetTransportInfo() => nameof(AgentTelemetryTransport) + " to " + GetEndpointInfo();

    protected override MetricTags.TelemetryEndpoint GetEndpointMetricTag() => MetricTags.TelemetryEndpoint.Agent;
}
