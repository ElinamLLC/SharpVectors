using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg; 
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfGroupRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfGroupRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            WpfDrawingContext context = renderer.Context;
            _drawGroup = new DrawingGroup();

            string elementId = this.GetElementName();
            if (!String.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
            {
                _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);

                context.RegisterId(elementId);

                if (context.IncludeRuntime)
                {
                    SvgObject.SetId(_drawGroup, elementId);
                }
            }

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            currentGroup.Children.Add(_drawGroup);
            context.Push(_drawGroup); 
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            if (_drawGroup != null)
            {
                Geometry clipGeom = this.ClipGeometry;
                if (clipGeom != null)
                {
                    _drawGroup.ClipGeometry = clipGeom;
                }

                Transform transform = this.Transform;
                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }

                float opacityValue = -1;

                SvgGElement element = (SvgGElement)_svgElement;
                string opacity = element.GetAttribute("opacity");
                if (opacity != null && opacity.Length > 0)
                {
                    opacityValue = (float)SvgNumber.ParseNumber(opacity);
                    opacityValue = Math.Min(opacityValue, 1);
                    opacityValue = Math.Max(opacityValue, 0);
                }

                if (opacityValue >= 0)
                {
                    _drawGroup.Opacity = opacityValue;
                }
            }

            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            Debug.Assert(_drawGroup != null);

            WpfDrawingContext context = renderer.Context;

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            // Remove the added group from the stack...
            context.Pop();

            // If the group is empty, we simply remove it...
            if (_drawGroup.Children.Count == 0 && _drawGroup.ClipGeometry == null &&
                _drawGroup.Transform == null)
            {
                currentGroup = context.Peek();
                if (currentGroup != null)
                {
                    currentGroup.Children.Remove(_drawGroup);
                }
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}
