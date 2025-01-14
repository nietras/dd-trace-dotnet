//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendoredCode tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0023, SYSLIB0032
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Datadog.Trace.Vendors.Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Datetime
    /// </summary>
    internal class OpenApiDateTime : OpenApiPrimitive<DateTimeOffset>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiDateTime"/> class.
        /// </summary>
        public OpenApiDateTime(DateTimeOffset value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType => PrimitiveType.DateTime;
    }
}
