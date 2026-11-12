using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class Issue320_DenseSvgRegressionTest
    {
        [Test]
        public void UnhoveredGroup_DoesNotRenderConnections()
        {
            const string svg = "<svg xmlns='http://www.w3.org/2000/svg' width='20' height='20'>" +
                "<style>.shaper:hover .liner { display: block; } .liner { display: none; }</style>" +
                "<g class='shaper'><rect width='10' height='10' fill='blue'/>" +
                "<g class='liner'><line x1='0' y1='0' x2='20' y2='20' stroke='red'/></g></g></svg>";
            DrawingGroup drawing;
            using (var reader = new FileSvgReader(new WpfDrawingSettings()))
            using (var input = new StringReader(svg))
            {
                drawing = reader.Read(input);
            }
            Assert.That(drawing, Is.Not.Null);
            var bitmap = Render(drawing, 20, 20);
            AssertPixel(bitmap, 5, 5, 0, 0, 255);
            AssertPixel(bitmap, 15, 15, 255, 255, 255);
        }

        [Test]
        [Explicit("Large SVG image comparison")]
        public void DenseSvg_RendersWithoutHoverOnlyConnections()
        {
            var settings = new WpfDrawingSettings();
            settings.Visitors.ImageVisitor = new EmbeddedImageSerializerVisitor(true);
            string svgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Issue320_DenseSVG.svg");
            FileAssert.Exists(svgPath);

            DrawingGroup drawing;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(svgPath);
            }
            Assert.That(drawing, Is.Not.Null);
            var bitmap = Render(drawing, 3217, 2017);
            AssertPixel(bitmap, 800, 300, 155, 155, 255);
            AssertPixel(bitmap, 1600, 1000, 155, 162, 255);
            AssertPixel(bitmap, 600, 700, 155, 155, 255);
        }

        private static BitmapSource Render(Drawing drawing, int width, int height)
        {
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
                context.DrawDrawing(drawing);
            }
            var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        private static void AssertPixel(BitmapSource bitmap, int x, int y, byte red, byte green, byte blue)
        {
            var pixels = new byte[4];
            bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            Assert.That(pixels[2], Is.EqualTo(red).Within(2), $"red at ({x}, {y})");
            Assert.That(pixels[1], Is.EqualTo(green).Within(2), $"green at ({x}, {y})");
            Assert.That(pixels[0], Is.EqualTo(blue).Within(2), $"blue at ({x}, {y})");
            Assert.That(pixels[3], Is.EqualTo(255), $"alpha at ({x}, {y})");
        }
    }
}
