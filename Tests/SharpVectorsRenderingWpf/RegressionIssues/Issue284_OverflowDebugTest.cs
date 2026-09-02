using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using System;
using System.IO;
using System.Xml;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    public class Issue284_OverflowDebugTest
    {
        private const string TestFileName = "Issue284_Overflow.svg";

        [Test]
        public void Issue284_Overflow_DebugCssInheritance()
        {
            // Arrange: Load the SVG file
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, $"Data/{TestFileName}");

            if (!File.Exists(testSvgPath))
            {
                Assert.Ignore("Test SVG file not found");
            }

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = true,
                IgnoreRootViewbox = false
            };

            using (var reader = new FileSvgReader(settings))
            {
                var drawing = reader.Read(testSvgPath);

                // Access the source document via DrawingDocument
                var drawingDoc = reader.DrawingDocument;
                Assert.IsNotNull(drawingDoc, "DrawingDocument should not be null");

                var document = drawingDoc.Document as SvgDocument;
                Assert.IsNotNull(document, "Document should be SvgDocument");

                // Get the root SVG element
                var rootSvg = document.DocumentElement as SvgSvgElement;
                Assert.IsNotNull(rootSvg, "Root should be SVG element");

                // Debug output - check if overflow attribute is present
                var overflowAttr = rootSvg.GetAttribute("overflow");
                System.Console.WriteLine($"Root SVG overflow attribute: '{overflowAttr}'");

                // Check presentation attribute
                var overflowPres = rootSvg.GetPresentationAttribute("overflow");
                System.Console.WriteLine($"Root SVG overflow presentation attribute: {overflowPres}");
                if (overflowPres != null)
                {
                    System.Console.WriteLine($"  CssText: '{overflowPres.CssText}'");
                }

                // Check computed style
                var computedStyle = rootSvg.GetComputedStyle(string.Empty);
                var overflowComputed = computedStyle.GetPropertyValue("overflow");
                System.Console.WriteLine($"Root SVG computed overflow: '{overflowComputed}'");

                // Check computed CSS value (what ShouldClipOverflow uses)
                var overflowCssValue = rootSvg.GetComputedCssValue("overflow", string.Empty);
                System.Console.WriteLine($"Root SVG computed CSS value: {overflowCssValue}");
                if (overflowCssValue != null)
                {
                    System.Console.WriteLine($"  CssText: '{overflowCssValue.CssText}'");
                }

                // Now check the nested SVG
                var svgElements = document.GetElementsByTagName("svg");
                if (svgElements.Count > 1)
                {
                    var nestedSvg = svgElements[1] as SvgSvgElement;
                    System.Console.WriteLine($"\nNested SVG overflow attribute: '{nestedSvg?.GetAttribute("overflow")}'");

                    var nestedComputedStyle = nestedSvg?.GetComputedStyle(string.Empty);
                    var nestedOverflowComputed = nestedComputedStyle?.GetPropertyValue("overflow");
                    System.Console.WriteLine($"Nested SVG computed overflow: '{nestedOverflowComputed}'");

                    var nestedOverflowCssValue = nestedSvg?.GetComputedCssValue("overflow", string.Empty);
                    System.Console.WriteLine($"Nested SVG computed CSS value: {nestedOverflowCssValue}");
                    if (nestedOverflowCssValue != null)
                    {
                        System.Console.WriteLine($"  CssText: '{nestedOverflowCssValue.CssText}'");
                    }
                }
            }
        }
    }
}
