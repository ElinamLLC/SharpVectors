using System;
using System.IO;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    public sealed class EmbeddedImageVisitor : WpfEmbeddedImageVisitor
    {
        public EmbeddedImageVisitor()
        {
        }

        public override ImageSource Visit(SvgImageElement element, WpfDrawingContext context)
        {
            string sURI    = element.Href.AnimVal.Replace(" ", "");
            int nColon     = sURI.IndexOf(":", StringComparison.OrdinalIgnoreCase);
            int nSemiColon = sURI.IndexOf(";", StringComparison.OrdinalIgnoreCase);
            int nComma     = sURI.IndexOf(",", StringComparison.OrdinalIgnoreCase);

            string sMimeType  = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

            string sContent   = SvgObject.RemoveWhitespace(sURI.Substring(nComma + 1));
            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                0, sContent.Length);

            switch (sMimeType.Trim())
            {
                case "image/svg+xml":
                    using (var stream = new MemoryStream(imageBytes))
                    using (var reader = new FileSvgReader(context.Settings))
                        return new DrawingImage(reader.Read(stream));
                default:
                    return new EmbeddedBitmapSource(new MemoryStream(imageBytes));
            }
        }
    }
}
