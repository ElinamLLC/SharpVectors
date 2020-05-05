using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

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

            SvgUseElement useElement = (SvgUseElement)_svgElement;

            float opacityValue = -1;

            string opacity = useElement.GetAttribute("opacity");
            if (string.IsNullOrWhiteSpace(opacity))
            {
                opacity = useElement.GetPropertyValue("opacity");
            }
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            _drawGroup = new DrawingGroup();

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            currentGroup.Children.Add(_drawGroup);
            context.Push(_drawGroup);

            string elementId = this.GetElementName();

            if (clipGeom != null || transform != null || (opacityValue >= 0 && opacityValue < 1) ||
                (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId)))
            {
                if (clipGeom != null)
                {
                    _drawGroup.ClipGeometry = clipGeom;
                }

                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }

                if (opacityValue >= 0 && opacityValue < 1)
                {
                    _drawGroup.Opacity = opacityValue;
                }

                if (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
                {
                    SvgObject.SetName(_drawGroup, elementId);

                    context.RegisterId(elementId);

                    if (context.IncludeRuntime)
                    {
                        SvgObject.SetId(_drawGroup, elementId);
                    }
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            // Register this drawing with the Drawing-Document...
            if (_drawGroup != null)
            {
                this.Rendered(_drawGroup);
            }

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

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _drawGroup = null;
        }

        #endregion
    }
}
