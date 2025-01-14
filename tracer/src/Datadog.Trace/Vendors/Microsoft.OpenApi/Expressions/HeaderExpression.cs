//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendoredCode tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0023, SYSLIB0032
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Datadog.Trace.Vendors.Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Header expression, The token identifier in header is case-insensitive.
    /// </summary>
    internal class HeaderExpression : SourceExpression
    {
        /// <summary>
        /// header. string
        /// </summary>
        public const string Header = "header.";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderExpression"/> class.
        /// </summary>
        /// <param name="token">The token string, it's case-insensitive.</param>
        public HeaderExpression(string token)
            : base(token)
        {
            Utils.CheckArgumentNullOrEmpty(token);
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression { get => Header + Value; }

        /// <summary>
        /// Gets the token string.
        /// </summary>
        public string Token { get => Value; }
    }
}
