// <copyright file="IRegistryService.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable

namespace Datadog.Trace.Tools.Runner.Checks
{
    internal interface IRegistryService
    {
        string[] GetLocalMachineValueNames(string key);

        string? GetLocalMachineValue(string key);

        public string[] GetLocalMachineKeyNames(string key);

        public string? GetLocalMachineKeyNameValue(string key, string subKeyName, string name);
    }
}
