using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

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

            if (clipGeom != null || transform != null)
            {
                WpfDrawingContext context = renderer.Context;
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
