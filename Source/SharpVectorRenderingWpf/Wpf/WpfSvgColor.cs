using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgColor : SvgColor
    {
        private readonly string _propertyName;
        private readonly SvgStyleableElement _element;

        public WpfSvgColor(SvgStyleableElement elm, string propertyName)
            : base(elm.GetComputedStyle("").GetPropertyValue(propertyName))
        {
            _element      = elm;
            _propertyName = propertyName;
        }

        public string Name
        {
            get {
                if (_rgbColor != null)
                {
                    return _rgbColor.Name;
                }
                return null;
            }
        }

        public Color Color
        {
            get {
                SvgColor colorToUse;
                if (ColorType == SvgColorType.CurrentColor)
                {
                    var cssDeclaration = _element.GetComputedStyle("");
                    string currentColor = cssDeclaration.GetPropertyValue("color");
                    if (!string.IsNullOrWhiteSpace(currentColor))
                    {
                        colorToUse = new SvgColor(currentColor);
                    }
                    else
                    {
                        currentColor = cssDeclaration.GetPropertyValue("solid-color");
                        if (!string.IsNullOrWhiteSpace(currentColor))
                        {
                            colorToUse = new SvgColor(currentColor);
                        }
                        else
                        {
                            colorToUse = new SvgColor("black");
                        }
                    }
                }
                else if (ColorType == SvgColorType.Unknown)
                {
                    colorToUse = new SvgColor("black");
                }
                else
                {
                    colorToUse = this;
                }

                ICssColor rgbColor = colorToUse.RgbColor;
                int red   = Convert.ToInt32(rgbColor.Red.GetFloatValue(CssPrimitiveType.Number));
                int green = Convert.ToInt32(rgbColor.Green.GetFloatValue(CssPrimitiveType.Number));
                int blue  = Convert.ToInt32(rgbColor.Blue.GetFloatValue(CssPrimitiveType.Number));

                if (rgbColor.HasAlpha)
                {
                    double dAlpha = rgbColor.Alpha.GetFloatValue(rgbColor.Alpha.PrimitiveType == CssPrimitiveType.Percentage ?
                        CssPrimitiveType.Number : CssPrimitiveType.Percentage);
                    if (!double.IsNaN(dAlpha) && !double.IsInfinity(dAlpha))
                    {
                        return Color.FromArgb(Convert.ToByte(dAlpha), Convert.ToByte(red),
                            Convert.ToByte(green), Convert.ToByte(blue));
                    }
                }

                return Color.FromArgb(Convert.ToByte(this.Alpha), Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
            }
        }

        public int Alpha
        {
            get {
                string propName;
                if (_propertyName.Equals("stop-color", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "stop-opacity";
                }
                else if (_propertyName.Equals("flood-color", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "flood-opacity";
                }
                else if (_propertyName.Equals("solid-opacity", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "solid-opacity";
                }
                else
                {
                    return 255;
                }

                double alpha = 255;

                string alphaText = _element.GetPropertyValue(propName);
                if (!string.IsNullOrWhiteSpace(alphaText))
                {
                    alpha *= SvgNumber.ParseNumber(alphaText);
                }

                alpha = Math.Min(alpha, 255);
                alpha = Math.Max(alpha, 0);

                return Convert.ToInt32(alpha);
            }
        }

        public double Opacity
        {
            get {
                string propName;
                if (_propertyName.Equals("stop-color", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "stop-opacity";
                }
                else if (_propertyName.Equals("flood-color", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "flood-opacity";
                }
                else if (_propertyName.Equals("solid-opacity", StringComparison.OrdinalIgnoreCase))
                {
                    propName = "solid-opacity";
                }
                else
                {
                    return 1.0f;
                }

                double alpha = 1.0f;

                string opacity = _element.GetPropertyValue(propName);
                if (!string.IsNullOrWhiteSpace(opacity))
                {
                    alpha = SvgNumber.ParseNumber(opacity);
                }

                alpha = Math.Min(alpha, 1.0f);
                alpha = Math.Max(alpha, 0.0f);

                return alpha;
            }
        }
    }
}
