using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSwitchRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfSwitchRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            Geometry clipGeom   = this.ClipGeometry;
            Transform transform = this.Transform;

            WpfDrawingContext context = renderer.Context;

            SvgSwitchElement switchElement = (SvgSwitchElement)_svgElement;

            string elementId = this.GetElementName();

            float opacityValue = -1;

            string opacity = switchElement.GetAttribute("opacity");
            if (string.IsNullOrWhiteSpace(opacity))
            {
                opacity = switchElement.GetPropertyValue("opacity");
            }
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            if (clipGeom != null || transform != null || (opacityValue >= 0 && opacityValue < 1) ||
                (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId)))
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

            // Register this drawing with the Drawing-Document...
            this.Rendered(_drawGroup);

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
