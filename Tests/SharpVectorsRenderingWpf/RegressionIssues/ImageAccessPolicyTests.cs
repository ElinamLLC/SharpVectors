using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Gdi;
using SharpVectors.Renderers.Utils;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ImageAccessPolicyTests
    {
        [TestCase(true, true, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        public void LocalRasterImage_RespectsImageSettings(bool loadImages, bool allowFiles, bool visible)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
            File.WriteAllBytes(path, MakePng());
            try
            {
                var settings = new WpfDrawingSettings { LoadImages = loadImages, AllowLocalImageFiles = allowFiles };
                string svg = ImageSvg(new Uri(path).AbsoluteUri);
                AssertPixel(svg, settings, visible);
            }
            finally { File.Delete(path); }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmbeddedRasterImage_RespectsLoadImages(bool enabled)
        {
            var settings = new WpfDrawingSettings { LoadImages = enabled, AllowLocalImageFiles = false };
            AssertPixel(ImageSvg("data:image/png;base64," + Convert.ToBase64String(MakePng())), settings, enabled);
        }

        [TestCase(true, true, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        public void GdiBitmapLoader_RespectsImageSettings(bool loadImages, bool allowFiles, bool visible)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
            File.WriteAllBytes(path, MakePng());
            try
            {
                var window = new GdiSvgWindow(10, 10, null) { LoadImages = loadImages, AllowLocalImageFiles = allowFiles };
                var document = window.CreateEmptySvgDocument();
                document.LoadXml(ImageSvg(new Uri(path).AbsoluteUri));
                var element = (SvgImageElement)document.DocumentElement.FirstChild;
                using (var bitmap = GdiImageRendering.GetBitmap(element))
                {
                    Assert.That(bitmap != null, Is.EqualTo(visible));
                    if (bitmap != null) Assert.That(bitmap.Width, Is.EqualTo(1));
                }
            }
            finally { File.Delete(path); }
        }

        [Test]
        public void BlockedImageColorProfile_DoesNotPreventEmbeddedBitmapRendering()
        {
            string data = Convert.ToBase64String(MakePng());
            string svg = "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10'>" +
                "<defs><color-profile id='profile' href='file://example.invalid/share/profile.icc'/></defs>" +
                "<image width='10' height='10' color-profile='profile' href='data:image/png;base64," + data + "'/></svg>";
            AssertPixel(svg, new WpfDrawingSettings { AllowLocalImageFiles = false }, true);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void LocalSvgImage_RespectsLocalFilePolicy(bool allow, bool visible)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".svg");
            File.WriteAllText(path, "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10'><rect width='10' height='10' fill='red'/></svg>");
            try
            {
                var settings = new WpfDrawingSettings { AllowLocalImageFiles = allow };
                AssertPixel(ImageSvg(new Uri(path).AbsoluteUri), settings, visible);
            }
            finally { File.Delete(path); }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void NestedSvgImage_RespectsSerializerWriteSetting(bool allowWrites)
        {
            string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "nested.svg");
            string savedImage = Path.Combine(directory, "_embeddedimage1.png");
            File.WriteAllText(path, ImageSvg("data:image/png;base64," + Convert.ToBase64String(MakePng())));
            try
            {
                var settings = new WpfDrawingSettings { AllowImageFileWrites = allowWrites };
                settings.Visitors.ImageVisitor = new EmbeddedImageSerializerVisitor(directory);
                AssertPixel(ImageSvg(new Uri(path).AbsoluteUri), settings, true);
                Assert.That(File.Exists(savedImage), Is.EqualTo(allowWrites));
            }
            finally
            {
                if (File.Exists(savedImage)) File.Delete(savedImage);
                File.Delete(path);
                Directory.Delete(directory);
            }
        }

        [Test]
        public void EmbeddedSvgImage_InheritsLocalImageRestriction()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
            File.WriteAllBytes(path, MakePng());
            try
            {
                string nested = ImageSvg(new Uri(path).AbsoluteUri);
                string uri = "data:image/svg+xml;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(nested));
                var settings = new WpfDrawingSettings { AllowLocalImageFiles = false };
                settings.Visitors.ImageVisitor = new EmbeddedImageVisitor();
                AssertPixel(ImageSvg(uri), settings, false);
            }
            finally { File.Delete(path); }
        }

        [Test]
        public void DisabledLocalFiles_BlockDirectModelAccess()
        {
            var settings = new WpfDrawingSettings { AllowLocalImageFiles = false };
            var window = CreateWindow(settings);
            using (var input = new StringReader(ImageSvg("file:///C:/unavailable.svg"))) window.LoadDocument(input, settings);
            var image = (SvgImageElement)((SvgDocument)window.Document).DocumentElement.FirstChild;
            Assert.That(image.IsSvgImage, Is.False);
            Assert.That(image.SvgWindow, Is.Null);
            Assert.That(image.UriReference.ReferencedResource, Is.Null);
            Assert.That(image.UriReference.ReferencedNode, Is.Null);
        }

        [TestCase("file://example.invalid/share/image.png", false, false)]
        [TestCase("file://example.invalid/share/image.svg", false, false)]
        [TestCase("file:////example.invalid/share/image.png", true, true)]
        [TestCase("file:///C:/images/image.png", false, true)]
        public void ImageResourcePolicy_ClassifiesUrisWithoutNetworkAccess(string value, bool allowUnc, bool allowed)
        {
            var window = CreateWindow(new WpfDrawingSettings());
            window.AllowUncImagePaths = allowUnc;
            Assert.That(window.CanLoadImageResource(new Uri(value)), Is.EqualTo(allowed));
            window.AllowLocalImageFiles = false;
            Assert.That(window.CanLoadImageResource(new Uri(value)), Is.False);
        }

        [Test]
        public void ImageResourcePolicy_IsInheritedAndCloned()
        {
            var settings = new WpfDrawingSettings
            {
                LoadImages = false,
                AllowLocalImageFiles = false,
                AllowUncImagePaths = true,
                AllowImageFileWrites = false
            };
            var copy = new WpfDrawingSettings(settings);
            Assert.That(copy.AllowImageFileWrites, Is.False);
            var window = CreateWindow(copy);
            using (var input = new StringReader("<svg xmlns='http://www.w3.org/2000/svg'/>")) window.LoadDocument(input, copy);
            var child = window.CreateOwnedWindow();
            Assert.That(child.LoadImages, Is.False);
            Assert.That(child.AllowLocalImageFiles, Is.False);
            Assert.That(child.AllowUncImagePaths, Is.True);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void BuiltInImageSerializer_RespectsWriteSetting(bool allowWrites)
        {
            string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string file = Path.Combine(directory, "_embeddedimage1.png");
            try
            {
                var settings = new WpfDrawingSettings { AllowImageFileWrites = allowWrites };
                settings.Visitors.ImageVisitor = new EmbeddedImageSerializerVisitor(directory);
                AssertPixel(ImageSvg("data:image/png;base64," + Convert.ToBase64String(MakePng())), settings, true);
                Assert.That(File.Exists(file), Is.EqualTo(allowWrites));
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
                Directory.Delete(directory);
            }
        }

        private static WpfSvgWindow CreateWindow(WpfDrawingSettings settings)
        {
            var window = new WpfSvgWindow(10, 10, new WpfDrawingRenderer(settings));
            window.BaseUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return window;
        }

        private static string ImageSvg(string href)
        {
            return "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10'><image width='10' height='10' href='" + href + "'/></svg>";
        }

        private static byte[] MakePng()
        {
            var bitmap = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[] { 0, 0, 255, 255 }, 4);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private static void AssertPixel(string svg, WpfDrawingSettings settings, bool visible)
        {
            DrawingGroup drawing;
            using (var reader = new FileSvgReader(settings))
            using (var input = new StringReader(svg)) drawing = reader.Read(input);
            Assert.That(drawing, Is.Not.Null);
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, 10, 10));
                dc.DrawDrawing(drawing);
            }
            var bitmap = new RenderTargetBitmap(10, 10, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var pixel = new byte[4];
            bitmap.CopyPixels(new Int32Rect(5, 5, 1, 1), pixel, 4, 0);
            Assert.That(pixel[2], Is.EqualTo(255));
            Assert.That(pixel[1], Is.EqualTo(visible ? 0 : 255));
            Assert.That(pixel[0], Is.EqualTo(visible ? 0 : 255));
        }
    }
}
