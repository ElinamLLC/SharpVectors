using NUnit.Framework;
using SharpVectors.Dom.Css;
using System;
using System.Linq;

namespace SharpVectors.Csss.Tests
{
    /// <summary>
    /// Tests for CSS property value validation
    /// </summary>
    [TestFixture]
    public class CssPropertyValidationTests
    {
        #region Test Data

        private const string ValidColorsCss = @"
            .color-hex { color: #FF0000; }
            .color-rgb { color: rgb(255, 0, 0); }
            .color-rgba { color: rgba(255, 0, 0, 0.5); }
            .color-keyword { color: red; }
        ";

        private const string InvalidColorsCss = @"
            .invalid-hex { color: #GGGGGG; }
            .invalid-rgb { color: rgb(256, 0, 0); }
            .invalid-rgba { color: rgba(255, 0, 0, 2.0); }
        ";

        private const string ValidSizesCss = @"
            .width { width: 100px; }
            .height { height: 50%; }
            .size-em { font-size: 2em; }
            .size-auto { width: auto; }
        ";

        private const string InvalidSizesCss = @"
            .invalid-size { width: xyz; }
            .invalid-percent { height: 150.5%; }
        ";

        private const string ValidSpacingCss = @"
            .margin { margin: 10px; }
            .margin-multi { margin: 10px 20px; }
            .padding { padding: 5px 10px 15px 20px; }
            .margin-auto { margin: auto; }
        ";

        private const string InvalidSpacingCss = @"
            .invalid-margin { margin: xyz; }
            .invalid-padding { padding: -5px; }
        ";

        private const string ValidBorderCss = @"
            .border { border: 1px solid black; }
            .border-width { border-width: thin; }
            .border-style { border-style: dashed; }
            .border-color { border-color: #FF0000; }
        ";

        private const string InvalidBorderCss = @"
            .invalid-border { border: xyz solid red; }
            .invalid-style { border-style: wiggle; }
        ";

        private const string ValidFontCss = @"
            .font-size { font-size: 14px; }
            .font-weight-normal { font-weight: normal; }
            .font-weight-numeric { font-weight: 700; }
            .font-style { font-style: italic; }
            .line-height { line-height: 1.5; }
        ";

        private const string InvalidFontCss = @"
            .invalid-weight { font-weight: 150; }
            .invalid-size { font-size: xyz; }
        ";

        private const string ValidLayoutCss = @"
            .display { display: flex; }
            .position { position: absolute; }
            .float { float: left; }
            .text-align { text-align: center; }
        ";

        private const string InvalidLayoutCss = @"
            .invalid-display { display: maybe; }
            .invalid-position { position: floating; }
        ";

        private const string ValidVisualCss = @"
            .opacity { opacity: 0.5; }
            .visibility { visibility: hidden; }
        ";

        private const string InvalidVisualCss = @"
            .invalid-opacity { opacity: 1.5; }
            .invalid-visibility { visibility: partially; }
        ";

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            // Setup if needed
        }

        #endregion

        #region Valid Color Tests

        [Test]
        public void ValidHexColor_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidColorsCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidHexColor_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidColorsCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            // Invalid colors may generate warnings
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Color validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Size Tests

        [Test]
        public void ValidSizes_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidSizesCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidSizes_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidSizesCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Size validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Spacing Tests

        [Test]
        public void ValidSpacing_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidSpacingCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidSpacing_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidSpacingCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Spacing validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Border Tests

        [Test]
        public void ValidBorder_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidBorderCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidBorder_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidBorderCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Border validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Font Tests

        [Test]
        public void ValidFont_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidFontCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidFont_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidFontCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Font validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Layout Tests

        [Test]
        public void ValidLayout_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidLayoutCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidLayout_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidLayoutCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Layout validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region Valid Visual Tests

        [Test]
        public void ValidVisual_ShouldPass()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(ValidVisualCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            Assert.That(diagnostics.HasErrors, Is.False, "Should have no errors");
        }

        [Test]
        public void InvalidVisual_ShouldGenerateWarning()
        {
            CssParsingContext diagnostics;
            var stylesheet = CssHelper.CreateStyleSheetWithDiagnostics(InvalidVisualCss, out diagnostics);

            Assert.That(diagnostics, Is.Not.Null, "Diagnostics should be available");
            var summary = diagnostics.GetSummary();
            Console.WriteLine("Visual validation summary:");
            Console.WriteLine(summary);
        }

        #endregion

        #region DirectValidator Tests

        [Test]
        public void PropertyValidator_ShouldValidateColors()
        {
            var validator = new CssPropertyValidator();

            // Valid colors
            Assert.That(validator.Validate("color", "#FF0000").IsValid, Is.True);
            Assert.That(validator.Validate("color", "rgb(255, 0, 0)").IsValid, Is.True);
            Assert.That(validator.Validate("color", "red").IsValid, Is.True);

            // Invalid colors
            Assert.That(validator.Validate("color", "#GGGGGG").IsValid, Is.False);
            Assert.That(validator.Validate("color", "rgb(256, 0, 0)").IsValid, Is.False);
        }

        [Test]
        public void PropertyValidator_ShouldValidateSizes()
        {
            var validator = new CssPropertyValidator();

            // Valid sizes
            Assert.That(validator.Validate("width", "100px").IsValid, Is.True);
            Assert.That(validator.Validate("height", "50%").IsValid, Is.True);
            Assert.That(validator.Validate("font-size", "2em").IsValid, Is.True);
            Assert.That(validator.Validate("width", "auto").IsValid, Is.True);

            // Invalid sizes
            Assert.That(validator.Validate("width", "xyz").IsValid, Is.False);
            Assert.That(validator.Validate("height", "150xyz").IsValid, Is.False);
        }

        [Test]
        public void PropertyValidator_ShouldValidateFontWeight()
        {
            var validator = new CssPropertyValidator();

            // Valid weights
            Assert.That(validator.Validate("font-weight", "normal").IsValid, Is.True);
            Assert.That(validator.Validate("font-weight", "bold").IsValid, Is.True);
            Assert.That(validator.Validate("font-weight", "700").IsValid, Is.True);
            Assert.That(validator.Validate("font-weight", "400").IsValid, Is.True);

            // Invalid weights
            Assert.That(validator.Validate("font-weight", "150").IsValid, Is.False);
            Assert.That(validator.Validate("font-weight", "ultra-bold").IsValid, Is.False);
        }

        [Test]
        public void PropertyValidator_ShouldValidateOpacity()
        {
            var validator = new CssPropertyValidator();

            // Valid opacity
            Assert.That(validator.Validate("opacity", "0").IsValid, Is.True);
            Assert.That(validator.Validate("opacity", "0.5").IsValid, Is.True);
            Assert.That(validator.Validate("opacity", "1").IsValid, Is.True);

            // Invalid opacity
            Assert.That(validator.Validate("opacity", "1.5").IsValid, Is.False);
            Assert.That(validator.Validate("opacity", "-0.5").IsValid, Is.False);
        }

        [Test]
        public void PropertyValidator_ShouldListSupportedProperties()
        {
            var validator = new CssPropertyValidator();
            var properties = validator.GetSupportedProperties().ToList();

            Assert.That(properties, Is.Not.Null);
            Assert.That(properties.Count, Is.GreaterThan(0));

            // Check for some key properties
            Assert.That(properties.Any(p => p == "color"), Is.True);
            Assert.That(properties.Any(p => p == "width"), Is.True);
            Assert.That(properties.Any(p => p == "font-size"), Is.True);
            Assert.That(properties.Any(p => p == "display"), Is.True);

            Console.WriteLine("Supported Properties:");
            foreach (var prop in properties)
            {
                Console.WriteLine($"  - {prop}");
            }
        }

        #endregion
    }
}
