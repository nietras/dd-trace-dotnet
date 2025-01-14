//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendoredCode tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0023, SYSLIB0032
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;

namespace Datadog.Trace.Vendors.Microsoft.OpenApi
{
    /// <summary>
    /// JSON pointer.
    /// </summary>
    internal class JsonPointer
    {
        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="pointer">Pointer as string.</param>
        public JsonPointer(string pointer)
        {
            Tokens = string.IsNullOrEmpty(pointer) || pointer == "/"
                ? new string[0]
                : pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="tokens">Pointer as tokenized string.</param>
        private JsonPointer(string[] tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Tokens.
        /// </summary>
        public string[] Tokens { get; }

        /// <summary>
        /// Gets the parent pointer.
        /// </summary>
        public JsonPointer ParentPointer
        {
            get
            {
                if (Tokens.Length == 0)
                {
                    return null;
                }

                return new(Tokens.Take(Tokens.Length - 1).ToArray());
            }
        }

        /// <summary>
        /// Decode the string.
        /// </summary>
        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        /// <summary>
        /// Gets the string representation of this JSON pointer.
        /// </summary>
        public override string ToString()
        {
            return "/" + string.Join("/", Tokens);
        }
    }
}
