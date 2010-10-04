using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfUseRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfUseRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            WpfDrawingContext context = renderer.Context;

            Geometry clipGeom   = this.ClipGeometry;
            Transform transform = this.Transform;

            if (transform == null && 
                (_svgElement.FirstChild != null && _svgElement.FirstChild == _svgElement.LastChild))
            {
                try
                {
                    SvgUseElement useElement = (SvgUseElement)_svgElement;

                    // If none of the following attribute exists, an exception is thrown...
                    double x      = useElement.X.AnimVal.Value;
                    double y      = useElement.Y.AnimVal.Value;
                    double width  = useElement.Width.AnimVal.Value;
                    double height = useElement.Height.AnimVal.Value;
                    if (width > 0 && height > 0)
                    {
                        Rect elementBounds = new Rect(x, y, width, height);

                        // Try handling the cases of "symbol" and "svg" sources within the "use"...
                        XmlNode childNode = _svgElement.FirstChild;
                        string childName  = childNode.Name;
                        if (String.Equals(childName, "symbol", StringComparison.OrdinalIgnoreCase))
                        {
                            SvgSymbolElement symbolElement = (SvgSymbolElement)childNode;

                            this.FitToViewbox(context, symbolElement, elementBounds);
                        }
                    }

                    transform = this.Transform;
                }
                catch
                {                   	
                }
            }

            if (clipGeom != null || transform != null)
            {
                _drawGroup = new DrawingGroup();

                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);

                if (clipGeom != null)
                {
                    _drawGroup.ClipGeometry = clipGeom;
                }

                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_drawGroup != null)
            {
                WpfDrawingContext context = renderer.Context;

                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null || currentGroup != _drawGroup)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                context.Pop();
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}
