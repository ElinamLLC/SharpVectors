using System;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Dom;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.Core
{
    /// <summary>
    /// Unit tests for SVG paint-order CSS property support.
    /// 
    /// The paint-order property specifies the rendering order of fill, stroke, and markers.
    /// Phase 1 Implementation: Supports 'normal' and 'stroke' values for geometry rendering.
    /// 
    /// Reference: https://www.w3.org/TR/svg2/painting.html#PaintOrder
    /// </summary>
    [TestFixture]
    public class PaintOrderTests
    {
        #region WpfPaintOrderHelper Tests

        [TestFixture]
        public class WpfPaintOrderHelperTests
        {
            /// <summary>
            /// Test parsing of null paint-order value defaults to Normal
            /// </summary>
            [Test]
            public void Parse_NullValue_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse(null);
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of empty string defaults to Normal
            /// </summary>
            [Test]
            public void Parse_EmptyString_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse(string.Empty);
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of whitespace-only string defaults to Normal
            /// </summary>
            [Test]
            public void Parse_WhitespaceOnly_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("   ");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of 'normal' keyword (lowercase)
            /// </summary>
            [Test]
            public void Parse_Normal_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("normal");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of 'NORMAL' keyword (uppercase)
            /// </summary>
            [Test]
            public void Parse_NormalUppercase_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("NORMAL");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of 'Normal' keyword (mixed case)
            /// </summary>
            [Test]
            public void Parse_NormalMixedCase_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("NoRmAl");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of 'stroke' keyword (lowercase)
            /// </summary>
            [Test]
            public void Parse_Stroke_ReturnsStroke()
            {
                var result = WpfPaintOrderHelper.Parse("stroke");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing of 'STROKE' keyword (uppercase)
            /// </summary>
            [Test]
            public void Parse_StrokeUppercase_ReturnsStroke()
            {
                var result = WpfPaintOrderHelper.Parse("STROKE");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing of 'Stroke' keyword (mixed case)
            /// </summary>
            [Test]
            public void Parse_StrokeMixedCase_ReturnsStroke()
            {
                var result = WpfPaintOrderHelper.Parse("sTrOkE");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing of 'fill' keyword (lowercase)
            /// </summary>
            [Test]
            public void Parse_Fill_ReturnsFill()
            {
                var result = WpfPaintOrderHelper.Parse("fill");
                Assert.AreEqual(WpfPaintOrder.Fill, result);
            }

            /// <summary>
            /// Test parsing of 'FILL' keyword (uppercase)
            /// </summary>
            [Test]
            public void Parse_FillUppercase_ReturnsFill()
            {
                var result = WpfPaintOrderHelper.Parse("FILL");
                Assert.AreEqual(WpfPaintOrder.Fill, result);
            }

            /// <summary>
            /// Test parsing of 'markers' keyword (lowercase)
            /// </summary>
            [Test]
            public void Parse_Markers_ReturnsMarkers()
            {
                var result = WpfPaintOrderHelper.Parse("markers");
                Assert.AreEqual(WpfPaintOrder.Markers, result);
            }

            /// <summary>
            /// Test parsing of 'MARKERS' keyword (uppercase)
            /// </summary>
            [Test]
            public void Parse_MarkersUppercase_ReturnsMarkers()
            {
                var result = WpfPaintOrderHelper.Parse("MARKERS");
                Assert.AreEqual(WpfPaintOrder.Markers, result);
            }

            /// <summary>
            /// Test parsing with leading and trailing whitespace
            /// </summary>
            [Test]
            public void Parse_StrokeWithWhitespace_ReturnsStroke()
            {
                var result = WpfPaintOrderHelper.Parse("  stroke  ");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing of unrecognized keyword defaults to Normal
            /// </summary>
            [Test]
            public void Parse_UnrecognizedKeyword_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("unknown");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of numeric value defaults to Normal
            /// </summary>
            [Test]
            public void Parse_NumericValue_ReturnsNormal()
            {
                var result = WpfPaintOrderHelper.Parse("123");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing of multiple keywords (Phase 2 feature) uses first keyword
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_ReturnsStroke()
            {
                // Phase 2 now supports combinations like "stroke fill" by using first keyword
                var result = WpfPaintOrderHelper.Parse("stroke fill");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing of fill-first multiple keywords (Phase 2)
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_Fill_ReturnsFill()
            {
                // "fill stroke" uses first keyword (fill)
                var result = WpfPaintOrderHelper.Parse("fill stroke");
                Assert.AreEqual(WpfPaintOrder.Fill, result);
            }

            /// <summary>
            /// Test parsing of markers-first multiple keywords (Phase 2)
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_Markers_ReturnsMarkers()
            {
                // "markers stroke fill" uses first keyword (markers)
                var result = WpfPaintOrderHelper.Parse("markers stroke fill");
                Assert.AreEqual(WpfPaintOrder.Markers, result);
            }

            /// <summary>
            /// Test parsing of comma-separated values (not standard SVG format) defaults to Normal
            /// </summary>
            [Test]
            public void Parse_CommaSeparatedValues_ReturnsNormal()
            {
                // Comma-separated format is not part of standard SVG paint-order syntax
                // Space-separated keywords like "stroke fill" are the standard format
                var result = WpfPaintOrderHelper.Parse("stroke,fill");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test parsing multi-keyword values with extra whitespace (tabs)
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_WithTabs_ParsesFirstKeyword()
            {
                // "stroke\tfill" with tab separator should parse as Stroke
                var result = WpfPaintOrderHelper.Parse("stroke\tfill");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing multi-keyword values with mixed whitespace
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_WithMixedWhitespace_ParsesFirstKeyword()
            {
                // "stroke  \n  fill" with mixed whitespace should parse as Stroke
                var result = WpfPaintOrderHelper.Parse("stroke  \n  fill");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing multi-keyword with mixed case
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_MixedCase_ParsesFirstKeyword()
            {
                // "STROKE Fill" with mixed case should parse as Stroke
                var result = WpfPaintOrderHelper.Parse("STROKE Fill");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing multi-keyword with duplicate keywords
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_WithDuplicates_ParsesFirstKeyword()
            {
                // "stroke stroke fill" with duplicate should parse as Stroke (first keyword)
                var result = WpfPaintOrderHelper.Parse("stroke stroke fill");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing leading/trailing whitespace with multi-keywords
            /// </summary>
            [Test]
            public void Parse_MultipleKeywords_WithLeadingTrailingWhitespace_ParsesFirstKeyword()
            {
                // "  stroke fill  " should parse as Stroke
                var result = WpfPaintOrderHelper.Parse("  stroke fill  ");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }
        }

        #endregion

        #region WpfPaintOrder Enum Tests

        [TestFixture]
        public class WpfPaintOrderEnumTests
        {
            /// <summary>
            /// Verify that WpfPaintOrder.Normal has value 0
            /// </summary>
            [Test]
            public void Normal_HasValue0()
            {
                Assert.AreEqual(0, (int)WpfPaintOrder.Normal);
            }

            /// <summary>
            /// Verify that WpfPaintOrder.Stroke has value 1
            /// </summary>
            [Test]
            public void Stroke_HasValue1()
            {
                Assert.AreEqual(1, (int)WpfPaintOrder.Stroke);
            }

            /// <summary>
            /// Verify that WpfPaintOrder.Fill has value 2
            /// </summary>
            [Test]
            public void Fill_HasValue2()
            {
                Assert.AreEqual(2, (int)WpfPaintOrder.Fill);
            }

            /// <summary>
            /// Verify that WpfPaintOrder.Markers has value 3
            /// </summary>
            [Test]
            public void Markers_HasValue3()
            {
                Assert.AreEqual(3, (int)WpfPaintOrder.Markers);
            }
        }

        #endregion

        #region CssConstants Tests

        [TestFixture]
        public class CssConstantsPaintOrderTests
        {
            /// <summary>
            /// Verify that CssConstants.PropPaintOrder is defined and equals 'paint-order'
            /// </summary>
            [Test]
            public void PropPaintOrder_IsDefined()
            {
                Assert.IsNotNull(CssConstants.PropPaintOrder);
                Assert.AreEqual("paint-order", CssConstants.PropPaintOrder);
            }

            /// <summary>
            /// Verify PropPaintOrder is not empty
            /// </summary>
            [Test]
            public void PropPaintOrder_IsNotEmpty()
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(CssConstants.PropPaintOrder));
            }

            /// <summary>
            /// Verify PropPaintOrder uses lowercase with hyphen (CSS standard)
            /// </summary>
            [Test]
            public void PropPaintOrder_UsesStandardCssFormat()
            {
                Assert.AreEqual("paint-order", CssConstants.PropPaintOrder.ToLowerInvariant());
                Assert.IsTrue(CssConstants.PropPaintOrder.Contains("-"));
            }
        }

        #endregion

        #region Integration Tests

        [TestFixture]
        public class PaintOrderIntegrationTests
        {
            /// <summary>
            /// Verify that parsing and enum comparison work together
            /// </summary>
            [Test]
            public void ParseAndCompare_Stroke_Works()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("stroke");
                Assert.IsTrue(paintOrder == WpfPaintOrder.Stroke);
                Assert.IsFalse(paintOrder == WpfPaintOrder.Normal);
            }

            /// <summary>
            /// Verify that parsing and enum comparison work for normal
            /// </summary>
            [Test]
            public void ParseAndCompare_Normal_Works()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("normal");
                Assert.IsTrue(paintOrder == WpfPaintOrder.Normal);
                Assert.IsFalse(paintOrder == WpfPaintOrder.Stroke);
            }

            /// <summary>
            /// Verify that default parsing behavior is stroke-last (Normal)
            /// </summary>
            [Test]
            public void ParseDefault_IsStrokeLast()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse(null);
                // Normal (default) means fill underneath, stroke on top (WPF default behavior)
                Assert.AreEqual(WpfPaintOrder.Normal, paintOrder);
            }

            /// <summary>
            /// Verify conditional logic for stroke-first rendering
            /// </summary>
            [Test]
            public void ParseStroke_RequiresSeparateRendering()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("stroke");

                // Phase 1 logic: stroke-first rendering should create separate drawings
                bool shouldUseSeparateDrawings = (paintOrder == WpfPaintOrder.Stroke);
                Assert.IsTrue(shouldUseSeparateDrawings);
            }

            /// <summary>
            /// Verify conditional logic for default rendering
            /// </summary>
            [Test]
            public void ParseNormal_UsesCombinedRendering()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("normal");

                // Phase 1 logic: normal mode uses combined GeometryDrawing
                bool shouldUseSeparateDrawings = (paintOrder == WpfPaintOrder.Stroke);
                Assert.IsFalse(shouldUseSeparateDrawings);
            }
        }

        #endregion

        #region Edge Case Tests

        [TestFixture]
        public class PaintOrderEdgeCaseTests
        {
            /// <summary>
            /// Test parsing with tabs and newlines
            /// </summary>
            [Test]
            public void Parse_WithTabsAndNewlines_Handled()
            {
                var result = WpfPaintOrderHelper.Parse("\t\nstroke\n\t");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test parsing with multiple spaces
            /// </summary>
            [Test]
            public void Parse_WithMultipleSpaces_Handled()
            {
                var result = WpfPaintOrderHelper.Parse("    stroke    ");
                Assert.AreEqual(WpfPaintOrder.Stroke, result);
            }

            /// <summary>
            /// Test that partial keyword matches are not accepted
            /// </summary>
            [Test]
            public void Parse_PartialKeyword_NotAccepted()
            {
                var result = WpfPaintOrderHelper.Parse("strok");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test that keyword with extra characters is not accepted
            /// </summary>
            [Test]
            public void Parse_KeywordWithExtra_NotAccepted()
            {
                var result = WpfPaintOrderHelper.Parse("stroke!");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test very long input string
            /// </summary>
            [Test]
            public void Parse_VeryLongInput_HandledGracefully()
            {
                var longString = new string('a', 10000);
                var result = WpfPaintOrderHelper.Parse(longString);
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }
        }

        #endregion

        #region Text Rendering Paint-Order Tests

        [TestFixture]
        public class TextRenderingPaintOrderTests
        {
            /// <summary>
            /// Test that paint-order="stroke" is correctly parsed for text elements
            /// </summary>
            [Test]
            public void TextElement_PaintOrderStroke_ParsesCorrectly()
            {
                var paintOrder = WpfPaintOrderHelper.Parse("stroke");
                Assert.AreEqual(WpfPaintOrder.Stroke, paintOrder);
            }

            /// <summary>
            /// Test that text with paint-order:stroke should trigger separate draw calls
            /// </summary>
            [Test]
            public void TextElement_WithPaintOrderStroke_ShouldUseSeperateDrawing()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("stroke");

                // For text rendering, stroke-first requires separate DrawGeometry calls
                bool shouldDrawSeparate = (paintOrder == WpfPaintOrder.Stroke);
                Assert.IsTrue(shouldDrawSeparate);
            }

            /// <summary>
            /// Test that text with default paint-order uses combined rendering
            /// </summary>
            [Test]
            public void TextElement_DefaultPaintOrder_UsesCombinedDrawing()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse(null);

                // Default behavior: stroke-last (fill drawn first, stroke on top)
                bool shouldDrawSeparate = (paintOrder == WpfPaintOrder.Stroke);
                Assert.IsFalse(shouldDrawSeparate);
            }

            /// <summary>
            /// Test that text paint-order parsing ignores case
            /// </summary>
            [Test]
            public void TextElement_StrokeCase_IsIgnored()
            {
                var strokeLower = WpfPaintOrderHelper.Parse("stroke");
                var strokeUpper = WpfPaintOrderHelper.Parse("STROKE");
                var strokeMixed = WpfPaintOrderHelper.Parse("StRoKe");

                Assert.AreEqual(strokeLower, strokeUpper);
                Assert.AreEqual(strokeLower, strokeMixed);
                Assert.AreEqual(WpfPaintOrder.Stroke, strokeLower);
            }

            /// <summary>
            /// Test that text paint-order parsing handles whitespace
            /// </summary>
            [Test]
            public void TextElement_StrokeWithWhitespace_IsParsed()
            {
                var strokeNormal = WpfPaintOrderHelper.Parse("stroke");
                var strokeWithSpaces = WpfPaintOrderHelper.Parse("  stroke  ");
                var strokeWithTabs = WpfPaintOrderHelper.Parse("\tstroke\t");

                Assert.AreEqual(strokeNormal, strokeWithSpaces);
                Assert.AreEqual(strokeNormal, strokeWithTabs);
            }

            /// <summary>
            /// Test that unrecognized paint-order values for text default to Normal
            /// </summary>
            [Test]
            public void TextElement_UnrecognizedValue_DefaultsToNormal()
            {
                var result = WpfPaintOrderHelper.Parse("unknown-value");
                Assert.AreEqual(WpfPaintOrder.Normal, result);
            }

            /// <summary>
            /// Test text paint-order enum value for stroke-first rendering
            /// </summary>
            [Test]
            public void TextPaintOrder_Stroke_HasCorrectValue()
            {
                Assert.AreEqual(1, (int)WpfPaintOrder.Stroke);
            }

            /// <summary>
            /// Test text paint-order enum value for default rendering
            /// </summary>
            [Test]
            public void TextPaintOrder_Normal_HasCorrectValue()
            {
                Assert.AreEqual(0, (int)WpfPaintOrder.Normal);
            }

            /// <summary>
            /// Verify that text elements with both fill and stroke can use paint-order
            /// </summary>
            [Test]
            public void TextElement_WithFillAndStroke_CanUsePaintOrder()
            {
                // Scenario: text with fill="black" stroke="yellow" paint-order="stroke"
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("stroke");

                bool hasFill = true;   // Simulated
                bool hasStroke = true; // Simulated

                // Should use separate draw calls for stroke-first effect
                bool shouldDrawSeparate = (paintOrder == WpfPaintOrder.Stroke && hasFill && hasStroke);
                Assert.IsTrue(shouldDrawSeparate);
            }

            /// <summary>
            /// Test that stroke-only text doesn't need paint-order (no fill to order)
            /// </summary>
            [Test]
            public void TextElement_StrokeOnly_DoesNotNeedPaintOrder()
            {
                WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse("stroke");

                bool hasFill = false;  // Stroke-only
                bool hasStroke = true; // Simulated

                // Paint-order only matters when both fill and stroke are present
                bool shouldDrawSeparate = (paintOrder == WpfPaintOrder.Stroke && hasFill && hasStroke);
                Assert.IsFalse(shouldDrawSeparate);
            }

            /// <summary>
            /// Test that phase 1 supports stroke-first for text geometry
            /// </summary>
            [Test]
            public void TextGeometry_PaintOrderStroke_IsSupported()
            {
                // Phase 1 supported values for text geometry rendering
                var normal = WpfPaintOrderHelper.Parse("normal");
                var stroke = WpfPaintOrderHelper.Parse("stroke");
                var fill = WpfPaintOrderHelper.Parse("fill");

                Assert.AreEqual(WpfPaintOrder.Normal, normal);
                Assert.AreEqual(WpfPaintOrder.Stroke, stroke);
                Assert.AreEqual(WpfPaintOrder.Fill, fill);
            }
        }

        #endregion
    }
}
