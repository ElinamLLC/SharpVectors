using System.Windows;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfEmbeddedImageVisitor : DependencyObject
    {
        protected WpfEmbeddedImageVisitor()
        {   
        }

        public abstract BitmapSource Visit(SvgImageElement element, 
            WpfDrawingContext context);
    }
}
