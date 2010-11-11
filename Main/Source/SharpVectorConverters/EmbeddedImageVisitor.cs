using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Collections.Generic;

using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

using SharpVectors.Runtime;

namespace SharpVectors.Converters
{
    public sealed class EmbeddedImageVisitor : WpfEmbeddedImageVisitor
    {
        public EmbeddedImageVisitor()
        {
        }

        public override BitmapSource Visit(SvgImageElement element, 
            WpfDrawingContext context)
        {
            string sURI    = element.Href.AnimVal;
            int nColon     = sURI.IndexOf(":");
            int nSemiColon = sURI.IndexOf(";");
            int nComma     = sURI.IndexOf(",");

            string sMimeType  = sURI.Substring(nColon + 1, 
                nSemiColon - nColon - 1);

            string sContent   = sURI.Substring(nComma + 1);
            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                0, sContent.Length);

            //BitmapImage imageSource = new BitmapImage();
            //imageSource.BeginInit();
            //imageSource.StreamSource = new MemoryStream(imageBytes);
            //imageSource.EndInit();

            return new EmbeddedBitmapSource(new MemoryStream(imageBytes));
        }
    }
}
