using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using SharpVectors.Dom;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Utils;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class FontFileSecurityTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void EmbeddedFont_UsesContentAddressedCacheInsteadOfFamilyPath(bool absoluteFamily)
        {
            byte[] data = Encoding.UTF8.GetBytes("font-test-" + Guid.NewGuid().ToString("N"));
            string name = "font-test-" + Guid.NewGuid().ToString("N");
            string target = Path.Combine(Path.GetTempPath(), name + ".ttf");
            string family = absoluteFamily ? Path.Combine(Path.GetTempPath(), name) : @"..\" + name;
            string cachePath = GetCachePath(data, ".ttf");
            var settings = new WpfDrawingSettings { ExternalResourcesAccessMode = ExternalResourcesAccessModes.Ignore };
            try
            {
                var window = CreateWindow(settings);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedSvg(family, data))))
                {
                    window.LoadDocument(stream, settings);
                }
                window.AwaitTasks("SvgDocument");

                Assert.That(File.Exists(target), Is.False);
                Assert.That(File.ReadAllBytes(cachePath), Is.EqualTo(data));
                var document = (SvgDocument)window.Document;
                Assert.That(document.FontFamilies, Has.Count.EqualTo(1));
                Assert.That(document.FontFamilies[0].FontUri, Is.EqualTo(cachePath));
            }
            finally
            {
                if (File.Exists(cachePath)) File.Delete(cachePath);
                if (File.Exists(target)) File.Delete(target);
            }
        }

        [TestCase("file://example.invalid/share/font.ttf", true)]
        [TestCase("file://localhost/C$/font.ttf", true)]
        [TestCase("file:////example.invalid/share/font.ttf", true)]
        [TestCase(@"\\example.invalid\share\font.ttf", true)]
        [TestCase("file:///C:/fonts/font.ttf", false)]
        public void FontPathClassification_RecognizesUncUris(string path, bool isUnc)
        {
            var method = typeof(SvgDocument).GetMethod("GetAllowedFontPath", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null);
            var uri = new Uri(path, UriKind.Absolute);
            Assert.That(method.Invoke(null, new object[] { uri, false }), Is.EqualTo(isUnc ? null : uri.LocalPath));
            Assert.That(method.Invoke(null, new object[] { uri, true }), Is.EqualTo(uri.LocalPath));
            Assert.That(new WpfDrawingSettings().AllowUncFontPaths, Is.False);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ExistingMismatchedCacheEntry_IsNotOverwritten(bool sameLength)
        {
            byte[] data = Encoding.UTF8.GetBytes("font-test-" + Guid.NewGuid().ToString("N"));
            byte[] existing = sameLength ? (byte[])data.Clone() : Encoding.UTF8.GetBytes("different data");
            if (sameLength) existing[0] ^= 1;
            string cachePath = GetCachePath(data, ".ttf");
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
            File.WriteAllBytes(cachePath, existing);
            try
            {
                var settings = new WpfDrawingSettings();
                var window = CreateWindow(settings);
                using (var reader = new StringReader(EmbeddedSvg("FontTest", data)))
                {
                    Assert.Throws<IOException>(() => window.LoadDocument(reader, settings));
                }
                Assert.That(File.ReadAllBytes(cachePath), Is.EqualTo(existing));
            }
            finally
            {
                File.Delete(cachePath);
            }
        }

        [Test]
        public void ConcurrentEmbeddedFontCacheWrites_ProduceOneCompleteEntry()
        {
            byte[] data = Encoding.UTF8.GetBytes("font-test-" + Guid.NewGuid().ToString("N"));
            string cachePath = GetCachePath(data, ".ttf");
            var method = typeof(SvgDocument).GetMethod("CacheEmbeddedFont", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null);
            try
            {
                var tasks = Enumerable.Range(0, 8).Select(i => Task.Run(() =>
                    (string)method.Invoke(null, new object[] { data, ".ttf", false }))).ToArray();
                Task.WaitAll(tasks);
                Assert.That(tasks.Select(task => task.Result), Is.All.EqualTo(cachePath));
                Assert.That(File.ReadAllBytes(cachePath), Is.EqualTo(data));
            }
            finally
            {
                if (File.Exists(cachePath)) File.Delete(cachePath);
            }
        }

        [Test]
        public void EmbeddedWoff_UsesContentAddressedConvertedFont()
        {
            byte[] data = Convert.FromBase64String("d09GRgABAAAAAABEAAEAAAAAACAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAB0ZXN0AAAAQAAAAAQAAAAEAQIDBAECAwQ=");
            byte[] unique = Guid.NewGuid().ToByteArray();
            Array.Copy(unique, 0, data, 64, 4);
            string woffPath = GetCachePath(data, ".woff");
            string convertedPath = null;
            string svg = "<svg xmlns='http://www.w3.org/2000/svg'><style>@font-face{font-family:WoffTest;src:url(data:font/woff;base64," +
                Convert.ToBase64String(data) + ");}</style></svg>";
            try
            {
                var settings = new WpfDrawingSettings();
                var window = CreateWindow(settings);
                using (var reader = new StringReader(svg)) window.LoadDocument(reader, settings);
                window.AwaitTasks("SvgDocument");
                var fonts = ((SvgDocument)window.Document).FontFamilies;
                Assert.That(fonts, Has.Count.EqualTo(1));
                convertedPath = fonts[0].FontUri;
                byte[] converted = File.ReadAllBytes(convertedPath);
                Assert.That(converted.Take(4).ToArray(), Is.EqualTo(new byte[] { 0, 1, 0, 0 }));
                Assert.That(convertedPath, Is.EqualTo(GetCachePath(converted, ".ttf")));
                Assert.That(fonts[0].SourceUri, Is.EqualTo(woffPath));
            }
            finally
            {
                if (File.Exists(woffPath)) File.Delete(woffPath);
                if (convertedPath != null && File.Exists(convertedPath)) File.Delete(convertedPath);
            }
        }

        [TestCase("stream")]
        [TestCase("text")]
        [TestCase("xml")]
        [TestCase("uri")]
        public void DisabledFontLoading_IsAppliedBeforeEveryDocumentLoad(string source)
        {
            byte[] data = Encoding.UTF8.GetBytes("font-test-" + Guid.NewGuid().ToString("N"));
            string cachePath = GetCachePath(data, ".ttf");
            string svg = EmbeddedSvg("FontTest", data);
            var settings = new WpfDrawingSettings { LoadFonts = false };
            var window = CreateWindow(settings);
            string svgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".svg");
            try
            {
                switch (source)
                {
                    case "stream":
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(svg))) window.LoadDocument(stream, settings);
                        break;
                    case "text":
                        using (var reader = new StringReader(svg)) window.LoadDocument(reader, settings);
                        break;
                    case "xml":
                        using (var reader = XmlReader.Create(new StringReader(svg))) window.LoadDocument(reader, settings);
                        break;
                    case "uri":
                        File.WriteAllText(svgPath, svg);
                        window.LoadDocument(new Uri(svgPath), settings);
                        break;
                }
                window.AwaitTasks("SvgDocument");
                Assert.That(window.LoadFonts, Is.False);
                Assert.That(File.Exists(cachePath), Is.False);
                Assert.That(((SvgDocument)window.Document).FontFamilies, Is.Null);
            }
            finally
            {
                if (File.Exists(svgPath)) File.Delete(svgPath);
                if (File.Exists(cachePath)) File.Delete(cachePath);
            }
        }

        [Test]
        public void DisabledFontFileWrites_SkipsEmbeddedFont()
        {
            byte[] data = Encoding.UTF8.GetBytes("font-test-" + Guid.NewGuid().ToString("N"));
            string cachePath = GetCachePath(data, ".ttf");
            var settings = new WpfDrawingSettings { AllowFontFileWrites = false };
            try
            {
                var window = CreateWindow(settings);
                using (var reader = new StringReader(EmbeddedSvg("FontTest", data))) window.LoadDocument(reader, settings);
                window.AwaitTasks("SvgDocument");
                Assert.That(File.Exists(cachePath), Is.False);
                Assert.That(((SvgDocument)window.Document).FontFamilies, Is.Null);
            }
            finally
            {
                if (File.Exists(cachePath)) File.Delete(cachePath);
            }
        }

        [Test]
        public void DisabledFontFileWrites_SkipsLocalWoffConversion()
        {
            string fontPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".woff");
            File.WriteAllBytes(fontPath, new byte[] { 1, 2, 3, 4 });
            string svg = "<svg xmlns='http://www.w3.org/2000/svg'><defs><font-face><font-face-src>" +
                "<font-face-uri href='" + new Uri(fontPath).AbsoluteUri + "'/></font-face-src></font-face></defs></svg>";
            try
            {
                var settings = new WpfDrawingSettings { AllowFontFileWrites = false };
                var window = CreateWindow(settings);
                using (var reader = new StringReader(svg)) window.LoadDocument(reader, settings);
                window.AwaitTasks("SvgDocument");
                Assert.That(((SvgDocument)window.Document).FontFamilies, Is.Null);
            }
            finally
            {
                File.Delete(fontPath);
            }
        }

        [TestCase(true, "css")]
        [TestCase(false, "css")]
        [TestCase(true, "svg")]
        [TestCase(false, "svg")]
        public void LocalFontFilePolicy_ControlsCssAndSvgReferences(bool allow, string source)
        {
            string fontPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".ttf");
            string svgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".svg");
            File.WriteAllText(fontPath, "test font");
            string fontUrl = new Uri(fontPath).AbsoluteUri;
            string reference = source == "css"
                ? "<style>@font-face{font-family:Test;src:url(" + Path.GetFileName(fontPath) + ");}</style>"
                : "<defs><font-face><font-face-src><font-face-uri href='" + fontUrl + "'/></font-face-src></font-face></defs>";
            string svg = "<svg xmlns='http://www.w3.org/2000/svg'>" + reference + "</svg>";
            try
            {
                File.WriteAllText(svgPath, svg);
                var settings = new WpfDrawingSettings { AllowLocalFontFiles = allow, AllowFontFileWrites = false };
                var window = CreateWindow(settings);
                window.LoadDocument(new Uri(svgPath), settings);
                window.AwaitTasks("SvgDocument");
                var fonts = ((SvgDocument)window.Document).FontFamilies;
                if (allow)
                {
                    Assert.That(fonts, Has.Count.EqualTo(1));
                    Assert.That(fonts.All(font => font.FontUri == fontPath), Is.True);
                }
                else
                {
                    Assert.That(fonts, Is.Null);
                }
            }
            finally
            {
                File.Delete(fontPath);
                if (File.Exists(svgPath)) File.Delete(svgPath);
            }
        }

        [Test]
        public void FontSettings_AreCopiedAndInherited()
        {
            var settings = new WpfDrawingSettings
            {
                LoadFonts = false,
                AllowLocalFontFiles = false,
                AllowFontFileWrites = false,
                AllowUncFontPaths = true
            };
            var copy = new WpfDrawingSettings(settings);
            var window = CreateWindow(copy);
            using (var reader = new StringReader("<svg xmlns='http://www.w3.org/2000/svg'/>")) window.LoadDocument(reader, copy);
            var child = window.CreateOwnedWindow();
            Assert.That(child.LoadFonts, Is.False);
            Assert.That(child.AllowLocalFontFiles, Is.False);
            Assert.That(child.AllowFontFileWrites, Is.False);
            Assert.That(child.AllowUncFontPaths, Is.True);
        }

        private static WpfSvgWindow CreateWindow(WpfDrawingSettings settings)
        {
            return new WpfSvgWindow(32, 32, new WpfDrawingRenderer(settings));
        }

        private static string EmbeddedSvg(string family, byte[] data)
        {
            return "<svg xmlns='http://www.w3.org/2000/svg'><style>@font-face{font-family:\"" + family +
                "\";src:url(data:application/x-font-ttf;base64," + Convert.ToBase64String(data) + ");}</style><rect width='1' height='1'/></svg>";
        }

        private static string GetCachePath(byte[] data, string extension)
        {
            using (var sha256 = SHA256.Create())
            {
                return Path.Combine(Path.GetTempPath(), "SharpVectors", BitConverter.ToString(sha256.ComputeHash(data)).Replace("-", string.Empty) + extension);
            }
        }
    }
}
