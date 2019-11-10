using System;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSolidColorFill : WpfFill
    {
        #region Private Fields

        private bool _isUserSpace;
        private SvgSolidColorElement _solidColorElement;

        #endregion

        #region Constructors and Destructor

        public WpfSolidColorFill(SvgSolidColorElement gradientElement)
        {
            _isUserSpace     = false;
            _solidColorElement = gradientElement;
        }

        #endregion

        #region Public Properties

        public override bool IsUserSpace
        {
            get {
                return _isUserSpace;
            }
        }

        public override WpfFillType FillType
        {
            get {
                return WpfFillType.Solid;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context, Transform viewTransform)
        {
            string prop    = _solidColorElement.GetAttribute("solid-color");
            string opacity = _solidColorElement.GetAttribute("solid-opacity"); // no auto-inherit
            if (string.Equals(prop, "inherit", StringComparison.OrdinalIgnoreCase)) // if explicitly defined...
            {
                prop = _solidColorElement.GetPropertyValue("solid-color");
            }
            if (string.Equals(opacity, "inherit", StringComparison.OrdinalIgnoreCase)) // if explicitly defined...
            {
                opacity = _solidColorElement.GetPropertyValue("solid-opacity");
            }
            if (!string.IsNullOrWhiteSpace(prop))
            {
                if (string.Equals(prop, "currentColor", StringComparison.OrdinalIgnoreCase))
                {
                    var svgParent = _solidColorElement.ParentNode as SvgStyleableElement;
                    if (svgParent != null)
                    {
                        prop = svgParent.GetPropertyValue("color", "solid-color");
                    }
                }

                Color color = Colors.Transparent; // no auto-inherited...
                WpfSvgColor svgColor = new WpfSvgColor(_solidColorElement, "solid-color");
                color = svgColor.Color;

                if (color.A == 255)
                {
                    double alpha = 255;

                    if (!string.IsNullOrWhiteSpace(opacity))
                    {
                        alpha *= SvgNumber.ParseNumber(opacity);
                    }

                    alpha = Math.Min(alpha, 255);
                    alpha = Math.Max(alpha, 0);

                    color = Color.FromArgb((byte)Convert.ToInt32(alpha), color.R, color.G, color.B);
                }

                var brush = new SolidColorBrush(color);
                double opacityValue = 1;
                if (!string.IsNullOrWhiteSpace(opacity) && double.TryParse(opacity, out opacityValue))
                {
                    brush.Opacity = opacityValue;
                }

                return brush;
            }
            else
            {
                Color color = Colors.Black; // the default color...
                double alpha = 255;

                if (!string.IsNullOrWhiteSpace(opacity))
                {
                    alpha *= SvgNumber.ParseNumber(opacity);
                }

                alpha = Math.Min(alpha, 255);
                alpha = Math.Max(alpha, 0);

                color = Color.FromArgb((byte)Convert.ToInt32(alpha), color.R, color.G, color.B);

                return new SolidColorBrush(color);
            }
        }

        #endregion
    }
}
