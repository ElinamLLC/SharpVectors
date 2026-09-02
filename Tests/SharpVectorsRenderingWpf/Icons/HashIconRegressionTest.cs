using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.IO;
using System.Windows.Media;
using System.Diagnostics;

namespace SharpVectors.Rendering.Wpf.Tests.Icons
{
    /// <summary>
    /// Diagnostic test for the "#" icon regression where all path figures are being filtered out.
    /// </summary>
    [TestFixture]
    public class HashIconRegressionTest
    {
        [Test]
        public void HashIcon_ShouldRenderNonEmpty()
        {
            // Set up Trace listener to capture diagnostics
            var listener = new ConsoleTraceListener();
            Trace.Listeners.Add(listener);
            Trace.AutoFlush = true;

            try
            {
                // Arrange
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
                string testSvgPath = Path.Combine(baseDirectory, "Data/HashIcon_Regression.svg");

                FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = false,
                    IgnoreRootViewbox = false,
                    OptimizePath = true  // Use optimized path parsing with filter
                };

                // Act
                DrawingGroup drawing = null;
                using (var reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(testSvgPath);
                }

                // Assert
                Assert.IsNotNull(drawing, "Drawing should not be null");
                Assert.That(drawing.Children.Count, Is.GreaterThan(0), 
                    "Drawing should contain rendering content. CURRENT BUG: All figures are filtered out, leaving empty DrawingGroup");

                // Print details for debugging
                Console.WriteLine($"Drawing children count: {drawing.Children.Count}");
                foreach (var child in drawing.Children)
                {
                    Console.WriteLine($"  Child type: {child.GetType().Name}");
                    if (child is DrawingGroup group)
                    {
                        Console.WriteLine($"    Group children: {group.Children.Count}");
                    }
                }
            }
            finally
            {
                Trace.Listeners.Remove(listener);
                listener.Dispose();
            }
        }
    }
}
