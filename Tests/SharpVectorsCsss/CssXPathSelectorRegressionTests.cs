using System.Xml;
using NUnit.Framework;
using SharpVectors.Dom.Css;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssXPathSelectorRegressionTests
    {
        [TestCase(".hyperlinks:not")]
        [TestCase(".hyperlinks:not (:hover)")]
        [TestCase(".hyperlinks:not()")]
        [TestCase(".hyperlinks:not(:not)")]
        public void MalformedNot_DoesNotMatch(string text)
        {
            var document = new XmlDocument();
            document.LoadXml("<rect class='hyperlinks'/>");
            var selector = new CssXPathSelector(text);

            Assert.That(selector.Matches(document.DocumentElement.CreateNavigator()), Is.False);
            Assert.That(selector.Specificity, Is.EqualTo(0));
            Assert.That(selector.XPath, Is.Null);
        }

        [TestCase(".hyperlinks:not")]
        [TestCase(".hyperlinks:not (:hover)")]
        [TestCase(".hyperlinks:not()")]
        public void MalformedNot_IsReportedByValidator(string text)
        {
            var analysis = CssSelectorValidator.Analyze(text);

            Assert.That(analysis.IsValid, Is.False);
            Assert.That(analysis.Issues, Does.Contain("Selector has invalid syntax"));
        }

        [Test]
        public void MalformedNot_IsReportedInStylesheetParsingContext()
        {
            var stylesheet = CssHelper.CreateStyleSheet();
            var context = new CssParsingContext();
            stylesheet.ParsingContext = context;

            stylesheet.InsertRule(".hyperlinks:not (:hover) { fill: red; }", 0U);

            Assert.That(context.Warnings, Has.Some.Matches<CssParsingWarning>(warning =>
                warning.Message.Contains("Selector has invalid syntax") && warning.Severity == CssWarningLevel.High));
        }

        [Test]
        public void ValidNot_MatchesOnlyElementsOutsideItsArgument()
        {
            var document = new XmlDocument();
            document.LoadXml("<root><rect class='hyperlinks'/><rect class='hyperlinks disabled'/></root>");
            var selector = new CssXPathSelector(".hyperlinks:not(.disabled)");

            Assert.That(selector.Matches(document.DocumentElement.FirstChild.CreateNavigator()), Is.True);
            Assert.That(selector.Matches(document.DocumentElement.LastChild.CreateNavigator()), Is.False);
            Assert.That(selector.Specificity, Is.EqualTo(20));
        }

        [Test]
        public void PseudoClassBeginningWithNot_IsNotParsedAsNegation()
        {
            var selector = new CssXPathSelector("rect:notable");

            Assert.That(selector.XPath, Is.EqualTo(new CssXPathSelector("rect").XPath));
        }
    }
}
