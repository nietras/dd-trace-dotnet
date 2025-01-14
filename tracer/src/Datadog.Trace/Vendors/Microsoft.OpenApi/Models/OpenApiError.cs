//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendoredCode tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0023, SYSLIB0032
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Datadog.Trace.Vendors.Microsoft.OpenApi.Exceptions;

namespace Datadog.Trace.Vendors.Microsoft.OpenApi.Models
{
    /// <summary>
    /// Error related to the Open API Document.
    /// </summary>
    internal class OpenApiError
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class using the message and pointer from the given exception.
        /// </summary>
        public OpenApiError(OpenApiException exception) : this(exception.Pointer, exception.Message)
        {
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiError(string pointer, string message)
        {
            Pointer = pointer;
            Message = message;
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiError"/> object
        /// </summary>
        public OpenApiError(OpenApiError error)
        {
            Pointer = error.Pointer;
            Message = error.Message;
        }

        /// <summary>
        /// Message explaining the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Pointer to the location of the error.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// Gets the string representation of <see cref="OpenApiError"/>.
        /// </summary>
        public override string ToString()
        {
            return Message + (!string.IsNullOrEmpty(Pointer) ? " [" + Pointer + "]" : "");
        }
    }
}
