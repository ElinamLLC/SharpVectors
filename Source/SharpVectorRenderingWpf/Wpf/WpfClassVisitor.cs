using System;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfClassVisitor : WpfVisitor
    {
        protected WpfClassVisitor()
        {
        }

        public abstract string Visit(SvgElement element, WpfDrawingContext context);
    }
}
