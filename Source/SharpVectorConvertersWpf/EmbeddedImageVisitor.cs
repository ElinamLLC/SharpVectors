using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    public sealed class EmbeddedImageVisitor : WpfEmbeddedImageVisitor
    {
        private IDictionary<string, ImageSource> _imageCache;

        public EmbeddedImageVisitor()
        {
            _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
        }

        public override void Initialize(WpfDrawingContext context)
        {
            if (this.IsInitialized)
            {
                return;
            }
            base.Initialize(context);

            _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            _imageCache = null;
        }

        public override ImageSource Visit(SvgImageElement element, WpfDrawingContext context)
        {
            if (_imageCache == null)
            {
                _imageCache = new Dictionary<string, ImageSource>(StringComparer.Ordinal);
            }

            var imageId = element.Id;
            if (_imageCache.Count != 0)
            {
                if (!string.IsNullOrWhiteSpace(imageId) && _imageCache.ContainsKey(imageId))
                {
                    return _imageCache[imageId];
                }
            }

            var imageSource = this.GetImage(element, context);

            if (!string.IsNullOrWhiteSpace(imageId))
            {
                _imageCache[imageId] = imageSource;
            }

            return imageSource;
        }

        private ImageSource GetImage(SvgImageElement element, WpfDrawingContext context)
        {
            if (context != null && context.Settings.IncludeRuntime == false)
            {
                return null;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            string sURI    = element.Href.AnimVal.Replace(" ", "");
            int nColon     = sURI.IndexOf(":", comparer);
            int nSemiColon = sURI.IndexOf(";", comparer);
            int nComma     = sURI.IndexOf(",", comparer);

            string sMimeType  = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

            string sContent   = SvgObject.RemoveWhitespace(sURI.Substring(nComma + 1));
            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                0, sContent.Length);
            bool isGZiped = sContent.StartsWith(SvgObject.GZipSignature, StringComparison.Ordinal);

            if (string.Equals(sMimeType, "image/svg+xml", comparer))
            {
                if (isGZiped)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var reader = new FileSvgReader(context.Settings, true))
                            {
                                return new DrawingImage(reader.Read(zipStream));
                            }
                        }
                    }
                }
                else
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        using (var reader = new FileSvgReader(context.Settings, true))
                        {
                            return new DrawingImage(reader.Read(stream));
                        }
                    }
                }
            }

            return new EmbeddedBitmapSource(new MemoryStream(imageBytes));
        }
    }
}
