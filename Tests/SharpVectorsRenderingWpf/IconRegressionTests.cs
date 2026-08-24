using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Rendering.Wpf.Tests
{
    // This is a diagnostic test to inspect geometry filtering behavior
    [TestFixture]
    public class IconRegressionTests
    {
        /// <summary>
        /// Diagnostic test: Render icon-like SVG and inspect the generated DrawingGroup
        /// to see if path figures are being inappropriately filtered or removed.
        /// </summary>
        [Test]
        [Explicit("Diagnostic: inspect generated geometry for icon regression")]
        public void IconRegression_InspectGeneratedGeometry()
        {
            // Arrange: Load the icon test SVG
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, "Data/IconRegression_Test.svg");

            FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                IgnoreRootViewbox = false,
                OptimizePath = true  // Test optimized path parsing with filter
            };

            // Act: Render the SVG
            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // Assert & Inspect
            Assert.IsNotNull(drawing, "SVG should render successfully");
            Assert.That(drawing.Children.Count, Is.GreaterThan(0), "Drawing should contain elements");

            // Collect all geometry drawings and inspect their bounds
            var geometries = CollectGeometries(drawing).ToList();
            Console.WriteLine($"Total geometry drawings found: {geometries.Count}");

            int smallGeometries = 0;
            int zeroAreaGeometries = 0;

            foreach (var geom in geometries)
            {
                if (geom.Geometry is PathGeometry pathGeom)
                {
                    Rect bounds = pathGeom.Bounds;
                    Console.WriteLine($"  PathGeometry - Bounds: {bounds.Width:F4} x {bounds.Height:F4}, " +
                        $"Figures: {pathGeom.Figures.Count}, Empty: {pathGeom.IsEmpty()}");

                    if (bounds.Width < 0.001 && bounds.Height < 0.001)
                    {
                        zeroAreaGeometries++;
                    }
                    if (bounds.Width < 0.1 || bounds.Height < 0.1)
                    {
                        smallGeometries++;
                    }
                }
                else
                {
                    Rect bounds = geom.Geometry.Bounds;
                    Console.WriteLine($"  {geom.Geometry.GetType().Name} - Bounds: {bounds.Width:F4} x {bounds.Height:F4}");
                }
            }

            Console.WriteLine($"\nSummary:");
            Console.WriteLine($"  Small geometries (< 0.1 units): {smallGeometries}");
            Console.WriteLine($"  Zero-area geometries (< 0.001 units): {zeroAreaGeometries}");

            // If all geometries were filtered out, this is a problem
            Assert.That(geometries.Count, Is.GreaterThan(0), 
                "Icon elements should produce visible geometry drawings");
        }

        private IEnumerable<GeometryDrawing> CollectGeometries(Drawing drawing)
        {
            if (drawing == null) yield break;

            if (drawing is GeometryDrawing geomDrawing)
            {
                yield return geomDrawing;
            }
            else if (drawing is DrawingGroup group)
            {
                foreach (var child in group.Children)
                {
                    foreach (var geom in CollectGeometries(child))
                    {
                        yield return geom;
                    }
                }
            }
        }
    }
}
