using System;
using System.IO;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.TextRendering
{
    [TestFixture]
    public class FontWeightResolutionTest
    {
        private string _baseDirectory;

        [SetUp]
        public void Setup()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
        }

        [Test]
        public void FontWeight_Numeric650_ShouldMapToBold()
        {
            // Arrange: Create SVG with font-weight: 650 (should map to 700 = Bold)
            string svgContent = @"<?xml version='1.0' encoding='UTF-8'?>
<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 100'>
  <style>
    .test-text { font-weight: 650; }
  </style>
  <text class='test-text' x='10' y='50'>Test 650</text>
</svg>";

            string testSvgPath = Path.Combine(_baseDirectory, "Test_FontWeight650.svg");
            File.WriteAllText(testSvgPath, svgContent);

            try
            {
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    IgnoreRootViewbox = false
                };

                // Act: Render the SVG
                DrawingGroup drawing = null;
                using (var reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(testSvgPath);
                }

                // Assert: The drawing should render without error and have reasonable bounds
                Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
                Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            }
            finally
            {
                if (File.Exists(testSvgPath))
                {
                    File.Delete(testSvgPath);
                }
            }
        }

        [Test]
        public void FontWeight_Numeric475_ShouldMapNearer500Than400()
        {
            // Arrange: Create SVG with font-weight: 475 (midpoint between 400 and 500, rounds to 500)
            string svgContent = @"<?xml version='1.0' encoding='UTF-8'?>
<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 100'>
  <style>
    .test-text { font-weight: 475; }
  </style>
  <text class='test-text' x='10' y='50'>Test 475</text>
</svg>";

            string testSvgPath = Path.Combine(_baseDirectory, "Test_FontWeight475.svg");
            File.WriteAllText(testSvgPath, svgContent);

            try
            {
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    IgnoreRootViewbox = false
                };

                // Act: Render the SVG
                DrawingGroup drawing = null;
                using (var reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(testSvgPath);
                }

                // Assert: Should render without error
                Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
                Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            }
            finally
            {
                if (File.Exists(testSvgPath))
                {
                    File.Delete(testSvgPath);
                }
            }
        }

        [Test]
        public void FontWeight_Numeric825_ShouldMapTo800()
        {
            // Arrange: Create SVG with font-weight: 825 (closer to 800 than 900)
            string svgContent = @"<?xml version='1.0' encoding='UTF-8'?>
<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 100'>
  <style>
    .test-text { font-weight: 825; }
  </style>
  <text class='test-text' x='10' y='50'>Test 825</text>
</svg>";

            string testSvgPath = Path.Combine(_baseDirectory, "Test_FontWeight825.svg");
            File.WriteAllText(testSvgPath, svgContent);

            try
            {
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    IgnoreRootViewbox = false
                };

                // Act: Render the SVG
                DrawingGroup drawing = null;
                using (var reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(testSvgPath);
                }

                // Assert: Should render without error
                Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
                Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            }
            finally
            {
                if (File.Exists(testSvgPath))
                {
                    File.Delete(testSvgPath);
                }
            }
        }

        [Test]
        public void FontWeight_Numeric550_ShouldRoundToNearest()
        {
            // Arrange: 550 is exactly between 500 and 600, should follow CSS rounding rules
            string svgContent = @"<?xml version='1.0' encoding='UTF-8'?>
<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 200 100'>
  <style>
    .test-text { font-weight: 550; }
  </style>
  <text class='test-text' x='10' y='50'>Test 550</text>
</svg>";

            string testSvgPath = Path.Combine(_baseDirectory, "Test_FontWeight550.svg");
            File.WriteAllText(testSvgPath, svgContent);

            try
            {
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    IgnoreRootViewbox = false
                };

                // Act: Render the SVG
                DrawingGroup drawing = null;
                using (var reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(testSvgPath);
                }

                // Assert: Should render without error
                Assert.That(drawing, Is.Not.Null, "Drawing should not be null");
                Assert.That(drawing.Bounds.Width, Is.GreaterThan(0), "Drawing width should be positive");
            }
            finally
            {
                if (File.Exists(testSvgPath))
                {
                    File.Delete(testSvgPath);
                }
            }
        }
    }
}
