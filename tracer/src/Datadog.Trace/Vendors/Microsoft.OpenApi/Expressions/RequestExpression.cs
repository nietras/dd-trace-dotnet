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
    /// $request. expression.
    /// </summary>
    internal sealed class RequestExpression : RuntimeExpression
    {
        /// <summary>
        /// $request. string
        /// </summary>
        public const string Request = "$request.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExpression"/> class.
        /// </summary>
        /// <param name="source">The source of the request.</param>
        public RequestExpression(SourceExpression source)
        {
            Source = Utils.CheckArgumentNull(source);
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression => Request + Source.Expression;

        /// <summary>
        /// The <see cref="SourceExpression"/> expression.
        /// </summary>
        public SourceExpression Source { get; }
    }
}
