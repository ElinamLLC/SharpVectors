using System;
using System.IO;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    public class Issue309_StrokeDasharrayTest
    {
        [Test]
        public void Issue309_StrokeDasharray_VarRepro_ShouldRenderWithoutCrash()
        {
            // This test reproduces the Issue309 crash with CSS variables in stroke-dasharray
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, "Data/Issue309_stroke-dasharray-var-repro.svg");

            FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                IgnoreRootViewbox = false
            };

            // Act: Render to drawing - this should not crash
            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // Assert: Verify the drawing was created without crashing
            Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
            Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            Assert.That(drawing.Bounds.Height, Is.GreaterThan(0), "Drawing height should be positive");
        }

        [Test]
        public void Issue309_StrokeDasharray_Full_ShouldRenderWithoutCrash()
        {
            // This test uses the full Issue309 SVG file with multiple CSS variables
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, "Data/Issue309_stroke-dasharray.svg");

            if (!File.Exists(testSvgPath))
            {
                Assert.Ignore("Full test file not available");
                return;
            }

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                IgnoreRootViewbox = false
            };

            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
            Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            Assert.That(drawing.Bounds.Height, Is.GreaterThan(0), "Drawing height should be positive");
        }
    }
}
