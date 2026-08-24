using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using System;
using System.IO;
using System.Windows.Media;

namespace SharpVectors.Rendering.Wpf.Tests
{
    [TestFixture]
    public class Issue284_OverflowDebugSvgRenderingTest
    {
        private const string TestFileName = "Issue284_Overflow.svg";

        [Test]
        public void Issue284_Overflow_DebugSvgRenderingClips()
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
                IncludeRuntime = false,
                IgnoreRootViewbox = false
            };

            using (var reader = new FileSvgReader(settings))
            {
                var drawing = reader.Read(testSvgPath);
                Assert.IsNotNull(drawing);

                // Inspect the render tree to find where clips are being applied
                InspectDrawingForClips(drawing, 0);
            }
        }

        private void InspectDrawingForClips(Drawing drawing, int level)
        {
            string indent = new string(' ', level * 2);

            if (drawing is DrawingGroup dg)
            {
                System.Console.WriteLine($"{indent}DrawingGroup: Bounds={dg.Bounds}, Children={dg.Children.Count}");

                if (dg.ClipGeometry != null)
                {
                    var clipBounds = dg.ClipGeometry.Bounds;
                    System.Console.WriteLine($"{indent}  *** CLIP ***  Bounds={clipBounds}");
                }

                if (dg.Transform != null)
                {
                    System.Console.WriteLine($"{indent}  Transform: {dg.Transform}");
                }

                int childIndex = 0;
                foreach (var child in dg.Children)
                {
                    System.Console.WriteLine($"{indent}  [Child {childIndex}]");
                    InspectDrawingForClips(child, level + 2);
                    childIndex++;
                }
            }
            else if (drawing is GeometryDrawing gd)
            {
                System.Console.WriteLine($"{indent}GeometryDrawing: Bounds={gd.Bounds}");
            }
            else
            {
                System.Console.WriteLine($"{indent}{drawing.GetType().Name}");
            }
        }
    }
}
