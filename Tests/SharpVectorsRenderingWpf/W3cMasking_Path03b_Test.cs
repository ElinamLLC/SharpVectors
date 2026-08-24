using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Rendering.Wpf.Tests
{
    [TestFixture]
    public class W3cMasking_Path03b_Test
    {
        [Test]
        public void MaskingPath03b_ViewportsClipContent()
        {
            // This test verifies that the W3C SVG 1.1 masking-path-03-b.svg test
            // renders with correct viewport clipping behavior.
            // 
            // Expected behavior:
            // - Root SVG has no explicit overflow, so should default to hidden per SVG 1.1 spec
            // - Content should be clipped to the 480x360 viewport
            // - Inner SVG at (115,100,250,160) should also clip its content (also defaults to hidden)

            string svgPath = @"D:\Visual Studio\Workspaces\SharpVectors\Samples\W3cSvgTestSuites\Svg11\svg\masking-path-03-b.svg";

            if (!File.Exists(svgPath))
            {
                Assert.Ignore($"W3C SVG file not found at {svgPath}");
            }

            var settings = new WpfDrawingSettings { IncludeRuntime = false };
            var reader = new FileSvgReader(settings);
            var drawing = reader.Read(svgPath);

            // The drawing should be properly bounded to the viewport
            Assert.That(drawing, Is.Not.Null);

            // Root SVG viewBox is 480x360, so the drawing should be bounded to that
            var bounds = drawing.Bounds;
            Assert.That(bounds.Width, Is.EqualTo(480).Within(1), 
                "Root SVG width should be clipped to viewport width (480)");
            Assert.That(bounds.Height, Is.EqualTo(360).Within(1), 
                "Root SVG height should be clipped to viewport height (360)");

            // Check that we have clipping geometries applied
            int clipCount = CountClipGeometries(drawing);

            // We expect:
            // 1. Root SVG clip (0,0,480,360) at level 1
            // 2. Inner SVG clip (115,100,250,160) at level 4
            Assert.That(clipCount, Is.EqualTo(2), 
                "Expected exactly 2 clip geometries (root + inner SVG viewport clipping)");
        }

        private int CountClipGeometries(Drawing drawing, ref Rect? expectedRootClip, ref Rect? expectedInnerClip)
        {
            int count = 0;
            CountClipsRecursive(drawing, 0, ref count, ref expectedRootClip, ref expectedInnerClip);
            return count;
        }

        private int CountClipGeometries(Drawing drawing)
        {
            int count = 0;
            Rect? unused1 = null;
            Rect? unused2 = null;
            CountClipsRecursive(drawing, 0, ref count, ref unused1, ref unused2);
            return count;
        }

        private void CountClipsRecursive(Drawing drawing, int depth, ref int count, ref Rect? rootClip, ref Rect? innerClip)
        {
            if (drawing is DrawingGroup dg)
            {
                if (dg.ClipGeometry != null)
                {
                    count++;
                    var bounds = dg.ClipGeometry.Bounds;
                    TestContext.WriteLine($"  Level {depth}: Clip at {bounds}");

                    // Try to identify which clip this is
                    if (Math.Abs(bounds.Width - 480) < 1 && Math.Abs(bounds.Height - 360) < 1)
                    {
                        rootClip = bounds;
                    }
                    else if (Math.Abs(bounds.Width - 250) < 1 && Math.Abs(bounds.Height - 160) < 1)
                    {
                        innerClip = bounds;
                    }
                }

                foreach (var child in dg.Children)
                {
                    CountClipsRecursive(child, depth + 1, ref count, ref rootClip, ref innerClip);
                }
            }
        }
    }
}
