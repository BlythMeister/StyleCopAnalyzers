﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1646IncludedDocumentationXPathDoesNotExist"/>.
    /// </summary>
    public class SA1646UnitTests
    {
        [Fact]
        public void TestDisabledByDefaultAndNotConfigurable()
        {
            var analyzer = new SA1646IncludedDocumentationXPathDoesNotExist();
            Assert.Single(analyzer.SupportedDiagnostics);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
            Assert.Contains(WellKnownDiagnosticTags.NotConfigurable, analyzer.SupportedDiagnostics[0].CustomTags);
        }
    }
}
