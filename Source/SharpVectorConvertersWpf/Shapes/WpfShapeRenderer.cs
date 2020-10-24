using System.Windows;
using System.Windows.Controls;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Converters.Shapes
{
    public class WpfShapeRenderer : ISvgRenderer
    {
        public Style ItemStyle { get; set; }
        public ISvgWindow Window { get; set; }
        public SvgRectF InvalidRect { get; set; }
        public RenderEvent OnRender { get; set; }

        public Canvas Canvas { get; set; }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            ISvgSvgElement svgElement = element as ISvgSvgElement;
            return (svgElement != null) ? svgElement.ViewBox.AnimVal : SvgRect.Empty;
        }

        public void InvalidateRect(SvgRectF rect)
        { }

        public void Render(ISvgElement node)
        {
            ISvgElementVisitorTarget evt = node as ISvgElementVisitorTarget;
            if (evt != null)
            {
                RenderElement(evt);
            }
        }

        public void Render(ISvgDocument node)
        {
            RenderElement(node.RootElement);
        }

        private void RenderElement(ISvgElementVisitorTarget element)
        {
            if (this.Canvas == null)
                this.Canvas = new Canvas();
            ShapeRenderingVisitor visitor = new ShapeRenderingVisitor(this);

            visitor.BeginContainer();

            element.Accept(visitor);

            visitor.EndContainer();
        }
    }
}
