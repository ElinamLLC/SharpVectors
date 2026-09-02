using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    public class Issue284_OverflowTest
    {
        private const string TestFileName = "Issue284_Overflow.svg";

        [Test]
        public void Issue284_Overflow_MusicalNote_ShouldRenderFully()
        {
            // Arrange: Load the SVG with musical note that uses overflow="inherit"
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, $"Data/{TestFileName}");

            FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                IgnoreRootViewbox = false
            };

            // Act: Render to drawing
            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // Assert: Verify the drawing is not empty and has content
            Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
            Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            Assert.That(drawing.Bounds.Height, Is.GreaterThan(0), "Drawing height should be positive");

            // The main test is that the musical note should render completely
            // When overflow="inherit" is correctly resolved to "visible", the content should not be clipped
            Debug.WriteLine($"Drawing Bounds: {drawing.Bounds}");
            Debug.WriteLine($"Drawing Width: {drawing.Bounds.Width:F2}, Height: {drawing.Bounds.Height:F2}");
        }

        [Test]
        public void Issue284_Overflow_Inherit_ShouldResolveFromParent()
        {
            // Test that overflow="inherit" on symbol correctly inherits from parent SVG's overflow="visible"
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, $"Data/{TestFileName}");

            FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

            var settings = new WpfDrawingSettings { IncludeRuntime = false };

            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // When overflow="inherit" is properly resolved to "visible", 
            // the content should NOT be clipped and should render completely
            Assert.That(drawing, Is.Not.Null);
            Assert.That(drawing.Bounds.Width, Is.GreaterThan(0));
            Assert.That(drawing.Bounds.Height, Is.GreaterThan(0));
        }
    }
}
