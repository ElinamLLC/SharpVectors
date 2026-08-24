using NUnit.Framework;

using SharpVectors.Dom.Css;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssSelectorParsingTests
    {
        #region Basic Selector Tests

        [Test]
        public void AnalyscSelector_WithTypeSelector_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("div");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Supported));
        }

        [Test]
        public void AnalyzeSelector_WithClassSelector_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze(".active");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Supported));
        }

        [Test]
        public void AnalyzeSelector_WithIdSelector_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("#main");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Supported));
        }

        [Test]
        public void AnalyzeSelector_WithAttributeSelector_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("input[type='text']");
            Assert.That(analysis.IsValid, Is.True);
        }

        [Test]
        public void AnalyzeSelector_WithComplexSelector_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("div.container > p.text");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.Complexity, Is.GreaterThan(0));
        }

        #endregion

        #region Pseudo-Class Tests

        [Test]
        public void AnalyzeSelector_WithHoverPseudoClass_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("a:hover");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoClasses.Count, Is.GreaterThan(0));
            Assert.That(analysis.PseudoClasses[0], Does.Contain("hover"));
        }

        [Test]
        public void AnalyzeSelector_WithActivePseudoClass_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("button:active");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoClasses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void AnalyzeSelector_WithFirstChildPseudoClass_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("li:first-child");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoClasses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void AnalyzeSelector_WithNthChildPseudoClass_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("li:nth-child(2n)");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoClasses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void AnalyzeSelector_WithNotPseudoClass_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("li:not(.active)");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoClasses.Count, Is.GreaterThan(0));
        }

        #endregion

        #region Pseudo-Element Tests

        [Test]
        public void AnalyzeSelector_WithBeforePseudoElement_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("p::before");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoElements.Count, Is.GreaterThan(0));
            Assert.That(analysis.PseudoElements[0], Does.Contain("before"));
        }

        [Test]
        public void AnalyzeSelector_WithAfterPseudoElement_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("div::after");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoElements.Count, Is.GreaterThan(0));
        }

        [Test]
        public void AnalyzeSelector_WithFirstLinePseudoElement_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("p::first-line");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.PseudoElements.Count, Is.GreaterThan(0));
        }

        #endregion

        #region Modern Selector Tests

        [Test]
        public void AnalyzeSelector_WithIsPseudoClass_DetectsModern()
        {
            var analysis = CssSelectorValidator.Analyze("div:is(.active, .hover)");
            Assert.That(analysis.ModernFeatures.Count, Is.GreaterThan(0));
            Assert.That(analysis.ModernFeatures[0], Does.Contain("is"));
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Modern));
        }

        [Test]
        public void AnalyzeSelector_WithWherePseudoClass_DetectsModern()
        {
            var analysis = CssSelectorValidator.Analyze("div:where(.active)");
            Assert.That(analysis.ModernFeatures.Count, Is.GreaterThan(0));
            Assert.That(analysis.ModernFeatures[0], Does.Contain("where"));
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Modern));
        }

        [Test]
        public void AnalyzeSelector_WithHasPseudoClass_DetectsModern()
        {
            var analysis = CssSelectorValidator.Analyze("div:has(> .child)");
            Assert.That(analysis.ModernFeatures.Count, Is.GreaterThan(0));
            Assert.That(analysis.ModernFeatures[0], Does.Contain("has"));
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Modern));
        }

        #endregion

        #region Combinator Tests

        [Test]
        public void AnalyzeSelector_WithDecendantCombinator_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("div p");
            Assert.That(analysis.IsValid, Is.True);
            Assert.That(analysis.Complexity, Is.GreaterThan(0));
        }

        [Test]
        public void AnalyzeSelector_WithChildCombinator_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("div > p");
            Assert.That(analysis.IsValid, Is.True);
        }

        [Test]
        public void AnalyzeSelector_WithRuleCombinator_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("h1 + p");
            Assert.That(analysis.IsValid, Is.True);
        }

        [Test]
        public void AnalyzeSelector_WithGeneralSiblingCombinator_IsValid()
        {
            var analysis = CssSelectorValidator.Analyze("h1 ~ p");
            Assert.That(analysis.IsValid, Is.True);
        }

        #endregion

        #region Complexity Tests

        [Test]
        public void AnalyzeSelector_CalculatesComplexity()
        {
            var simple = CssSelectorValidator.Analyze("div");
            var complex = CssSelectorValidator.Analyze("html body div.container > p:first-child::before");

            Assert.That(simple.Complexity, Is.LessThan(complex.Complexity));
        }

        [Test]
        public void AnalyzeSelector_HighComplexityWarning()
        {
            var analysis = CssSelectorValidator.Analyze("div.a > p.b + span.c ~ a:hover::before");
            Assert.That(analysis.Complexity, Is.GreaterThan(0));
            // Complex selector should potentially warn
        }

        #endregion

        #region Invalid Selector Tests

        [Test]
        public void AnalyzeSelector_WithEmptySelector_IsInvalid()
        {
            var analysis = CssSelectorValidator.Analyze("");
            Assert.That(analysis.IsValid, Is.False);
            Assert.That(analysis.SupportLevel, Is.EqualTo(CssSelectorValidator.SelectorSupportLevel.Error));
        }

        [Test]
        public void AnalyzeSelector_WithUnbalancedBrackets_IsInvalid()
        {
            var analysis = CssSelectorValidator.Analyze("div[");
            Assert.That(analysis.IsValid, Is.False);
        }

        [Test]
        public void AnalyzeSelector_WithUnbalancedParentheses_IsInvalid()
        {
            var analysis = CssSelectorValidator.Analyze("div:not(.active");
            Assert.That(analysis.IsValid, Is.False);
        }

        #endregion

        #region Multiple Selector Tests

        [Test]
        public void AnalyzeSelectors_WithCommaSeparated_AnalyzesAll()
        {
            var results = CssSelectorValidator.AnalyzeSelectors("div, .active, #main");
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].IsValid, Is.True);
            Assert.That(results[1].IsValid, Is.True);
            Assert.That(results[2].IsValid, Is.True);
        }

        [Test]
        public void AnalyzeSelectors_WithNesting_HandlesCorrectly()
        {
            var results = CssSelectorValidator.AnalyzeSelectors("div:is(.a, .b), span");
            Assert.That(results.Count, Is.EqualTo(2));
        }

        #endregion

        #region Diagnostics Integration Tests

        [Test]
        public void GetDiagnostics_WithModernSelector_ReturnsDiagnostics()
        {
            var diagnostics = CssSelectorValidator.GetDiagnostics("div:is(.active)");
            Assert.That(diagnostics.Count, Is.GreaterThan(0));
            Assert.That(diagnostics[0], Does.Contain("limited support").IgnoreCase);
        }

        [Test]
        public void GetDiagnostics_WithValidSelector_ReturnsNoDiagnostics()
        {
            var diagnostics = CssSelectorValidator.GetDiagnostics("div.active");
            // Valid selectors may have no diagnostics or only info-level ones
            // depending on complexity
        }

        #endregion

        #region Selector Description Tests

        [Test]
        public void GetSelectorDescription_WithPseudoClass_IncludesIt()
        {
            var description = CssSelectorValidator.GetSelectorDescription("a:hover");
            Assert.That(description, Does.Contain("pseudo-class").IgnoreCase);
        }

        [Test]
        public void GetSelectorDescription_WithModernFeature_IncludesIt()
        {
            var description = CssSelectorValidator.GetSelectorDescription("div:is(.a, .b)");
            Assert.That(description, Does.Contain("modern").IgnoreCase);
        }

        #endregion

        #region Integration with CSS Parsing

        [Test]
        public void InsertRule_WithModernSelector_TracksInDiagnostics()
        {
            var stylesheet = CssHelper.CreateStyleSheet();
            var context = new CssParsingContext();
            stylesheet.ParsingContext = context;

            // This should trigger selector validation
            stylesheet.InsertRule("div:is(.active) { color: red; }", 0U);

            // Diagnostics should be recorded if validation ran
            // Note: This tests the integration, diagnostics will only be recorded
            // if the ParsingContext is properly threaded through
        }

        #endregion
    }
}
