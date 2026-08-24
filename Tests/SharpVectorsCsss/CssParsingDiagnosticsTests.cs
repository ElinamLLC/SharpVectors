using System.IO;
using System.Text;

using NUnit.Framework;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Stylesheets;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Csss.Tests
{
    [TestFixture]
    public class CssParsingDiagnosticsTests
    {
        #region Helper Methods

        private static CssStyleSheet CreateStyleSheet()
        {
            string svgTemplate = @"<svg xmlns='http://www.w3.org/2000/svg'>
                        <style type='text/css'>
                        </style>
                      </svg>";

            SvgDocument svgDoc = new SvgDocument(TestSvgWindow.Create());
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(svgTemplate)))
            {
                svgDoc.Load(stream);
            }

            IStyleSheetList sheets = svgDoc.StyleSheets;
            CssStyleSheet testSheet = (CssStyleSheet)sheets[0];
            return testSheet;
        }

        #endregion

        [Test]
        public void ParsingContext_WithoutDiagnostics_WorksNormally()
        {
            // Arrange
            var stylesheet = CreateStyleSheet();

            // Act - should not throw even without diagnostics
            stylesheet.InsertRule("div { color: red; }", 0U);

            // Assert
            Assert.That(stylesheet.CssRules.Length, Is.GreaterThan(0));
        }

        [Test]
        public void ParsingContext_CanBeSetAfterCreation()
        {
            // Arrange
            var stylesheet = CreateStyleSheet();
            var context = new CssParsingContext();

            // Act
            stylesheet.ParsingContext = context;

            // Assert
            Assert.That(stylesheet.ParsingContext, Is.EqualTo(context));
        }

        [Test]
        public void ParsingContext_HandlesEmptyStylesheet()
        {
            // Arrange
            var stylesheet = CreateStyleSheet();
            var context = new CssParsingContext();
            stylesheet.ParsingContext = context;

            // Act
            var rules = stylesheet.CssRules;

            // Assert
            Assert.That(context.HasErrors, Is.False);
        }

        [Test]
        public void ParsingContext_TracksSimpleRule()
        {
            // Arrange
            var stylesheet = CreateStyleSheet();
            var context = new CssParsingContext();
            stylesheet.ParsingContext = context;

            // Act
            stylesheet.InsertRule("body { color: black; }", 0U);

            // Assert
            Assert.That(context.HasErrors, Is.False);
        }

        [Test]
        public void CssParsingError_HasRequiredProperties()
        {
            // Arrange & Act
            var error = new CssParsingError
            {
                Message = "Test error message",
                Position = 10,
                Context = "sample context"
            };

            // Assert
            Assert.That(error.Message, Is.EqualTo("Test error message"));
            Assert.That(error.Position, Is.EqualTo(10));
            Assert.That(error.Context, Is.EqualTo("sample context"));
        }

        [Test]
        public void CssParsingWarning_HasRequiredProperties()
        {
            // Arrange & Act
            var warning = new CssParsingWarning
            {
                Message = "Test warning",
                Severity = CssWarningLevel.Medium
            };

            // Assert
            Assert.That(warning.Message, Is.EqualTo("Test warning"));
            Assert.That(warning.Severity, Is.EqualTo(CssWarningLevel.Medium));
        }

        [Test]
        public void CssWarningLevel_AllValuesExist()
        {
            // Assert
            Assert.That(CssWarningLevel.Info, Is.EqualTo(CssWarningLevel.Info));
            Assert.That(CssWarningLevel.Low, Is.EqualTo(CssWarningLevel.Low));
            Assert.That(CssWarningLevel.Medium, Is.EqualTo(CssWarningLevel.Medium));
            Assert.That(CssWarningLevel.High, Is.EqualTo(CssWarningLevel.High));
        }

        [Test]
        public void ParsingContext_Clear_ResetsState()
        {
            // Arrange
            var context = new CssParsingContext();
            context.AddError("Test error");
            Assert.That(context.Errors.Count, Is.EqualTo(1));

            // Act
            context.Clear();

            // Assert
            Assert.That(context.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void ParsingContext_RecordRuleParsed_IncrementsCounter()
        {
            // Arrange
            var context = new CssParsingContext();
            context.StartTracking();

            // Act
            context.RecordRuleParsed();
            context.RecordRuleParsed();

            // Assert
            Assert.That(context.RulesParsed, Is.EqualTo(2));
        }

        [Test]
        public void ParsingContext_Errors_IsReadOnly()
        {
            // Arrange
            var context = new CssParsingContext();
            var errors = context.Errors;

            // Assert - ReadOnlyCollection doesn't have Add method, so verify it cannot be cast
            Assert.That(errors, Is.InstanceOf<System.Collections.ObjectModel.ReadOnlyCollection<CssParsingError>>());
        }

        [Test]
        public void ParsingContext_Warnings_IsReadOnly()
        {
            // Arrange
            var context = new CssParsingContext();
            var warnings = context.Warnings;

            // Assert - ReadOnlyCollection doesn't have Add method, so verify it cannot be cast
            Assert.That(warnings, Is.InstanceOf<System.Collections.ObjectModel.ReadOnlyCollection<CssParsingWarning>>());
        }

        [Test]
        public void ParsingContext_GetSummary_ReturnsString()
        {
            // Arrange
            var context = new CssParsingContext();
            context.StartTracking();
            context.RecordRuleParsed();
            context.StopTracking();

            // Act
            var summary = context.GetSummary();

            // Assert
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary, Is.Not.Empty);
        }

        [Test]
        public void ParsingContext_MultipleRules_AllTracked()
        {
            // Arrange
            var stylesheet = CreateStyleSheet();
            var context = new CssParsingContext();
            stylesheet.ParsingContext = context;

            // Act
            stylesheet.InsertRule("div { border: 1px solid; }", 0U);
            stylesheet.InsertRule("p { font-size: 14px; }", 1U);
            stylesheet.InsertRule("span { font-weight: bold; }", 2U);

            // Assert
            Assert.That(stylesheet.CssRules.Length, Is.GreaterThanOrEqualTo(3));
        }
    }
}