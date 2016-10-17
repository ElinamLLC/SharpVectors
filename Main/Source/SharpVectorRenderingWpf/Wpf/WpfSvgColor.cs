using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgColor : SvgColor
    {
        private string _propertyName;
        private SvgStyleableElement _element;

        public WpfSvgColor(SvgStyleableElement elm, string propertyName) 
            : base(elm.GetComputedStyle("").GetPropertyValue(propertyName))
        {
            _element      = elm;
            _propertyName = propertyName;
        }

        public Color Color
        {
            get
            {
                SvgColor colorToUse;
                if (ColorType == SvgColorType.CurrentColor)
                {
                    string sCurColor = _element.GetComputedStyle("").GetPropertyValue("color");
                    colorToUse = new SvgColor(sCurColor);
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

                return Color.FromArgb((byte)this.Alpha, (byte)red, (byte)green, (byte)blue);
            }
        }

        public int Alpha
        {   
            get
            {
                string propName;
                if (_propertyName.Equals("stop-color"))
                {
                    propName = "stop-opacity";
                }
                else if (_propertyName.Equals("flood-color"))
                {
                    propName = "flood-opacity";
                }
                else
                {
                    return 255;
                }

                double alpha = 255;
                string opacity;

                opacity = _element.GetPropertyValue(propName);
                if (opacity.Length > 0)
                    alpha *= SvgNumber.ParseNumber(opacity);

                alpha = Math.Min(alpha, 255);
                alpha = Math.Max(alpha, 0);

                return Convert.ToInt32(alpha);
            }
        }

        public double Opacity
        {
            get
            {
                string propName;
                if (_propertyName.Equals("stop-color"))
                {
                    propName = "stop-opacity";
                }
                else if (_propertyName.Equals("flood-color"))
                {
                    propName = "flood-opacity";
                }
                else
                {
                    return 1.0f;
                }

                double alpha = 1.0f;
                string opacity;

                opacity = _element.GetPropertyValue(propName);
                if (opacity != null && opacity.Length > 0)
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
