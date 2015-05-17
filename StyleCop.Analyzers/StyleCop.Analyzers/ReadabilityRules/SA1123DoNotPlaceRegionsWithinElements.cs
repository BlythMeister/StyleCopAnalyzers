﻿namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains a region within the body of a code element.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a region is placed within the body of a code element. In many
    /// editors, including Visual Studio, the region will appear collapsed by default, hiding the code within the
    /// region. It is generally a bad practice to hide code within the body of an element, as this can lead to bad
    /// decisions as the code is maintained over time.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1123DoNotPlaceRegionsWithinElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1123DoNotPlaceRegionsWithinElements"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1123";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1123Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1123MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1123Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1123.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleRegionDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia);
        }

        private void HandleRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            RegionDirectiveTriviaSyntax regionSyntax = context.Node as RegionDirectiveTriviaSyntax;

            if (regionSyntax != null && IsCompletelyContainedInBody(regionSyntax))
            {
                // Region must not be located within a code element.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, regionSyntax.GetLocation()));
            }
        }

        /// <summary>
        /// Checks if a region is completely part of a body. That means that the <c>#region</c> and <c>#endregion</c>
        /// tags both have to have a common <see cref="BlockSyntax"/> as one of their ancestors.
        /// </summary>
        /// <param name="regionSyntax">The <see cref="RegionDirectiveTriviaSyntax"/> that should be analyzed.</param>
        /// <returns><see langword="true"/>, if both tags have a common <see cref="BlockSyntax"/> as one of their
        /// ancestors; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="regionSyntax"/> is <see langword="null"/>.
        /// </exception>
        internal static bool IsCompletelyContainedInBody(RegionDirectiveTriviaSyntax regionSyntax)
        {
            if (regionSyntax == null)
            {
                throw new ArgumentNullException(nameof(regionSyntax));
            }

            BlockSyntax syntax = null;
            foreach (var directive in regionSyntax.GetRelatedDirectives())
            {
                BlockSyntax blockSyntax = directive.AncestorsAndSelf().OfType<BlockSyntax>().LastOrDefault();
                if (blockSyntax == null)
                {
                    return false;
                }
                else if (syntax == null)
                {
                    syntax = blockSyntax;
                }
                else if (blockSyntax != syntax)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
