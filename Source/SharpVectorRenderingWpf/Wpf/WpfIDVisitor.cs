using System;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfIDVisitor : WpfVisitor
    {
        protected WpfIDVisitor()
        {
        }

        public abstract string Visit(SvgElement element, WpfDrawingContext context);
    }
}
