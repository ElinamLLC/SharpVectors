using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfEmbeddedImageVisitor : WpfVisitor
    {
        protected WpfEmbeddedImageVisitor()
        {   
        }

        public abstract ImageSource Visit(SvgImageElement element, 
            WpfDrawingContext context);
    }
}
