using System;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveColorValue : CssPrimitiveValue
    {
        public CssPrimitiveColorValue(int color, bool readOnly)
            : base(color.ToString(CssNumber.Format), readOnly)
        {
            if (color <= 0)
            {
                SetFloatValue(0);
            }
            else if (color > 0 && color < 1)
            {
                color = (int)(255 * color);
                SetFloatValue(color);
            }
            else if (color > 255)
            {
                SetFloatValue(255);
            }
            else
            {
                SetFloatValue(color);
            }
            SetPrimitiveType(CssPrimitiveType.Number);
        }

        public CssPrimitiveColorValue(string cssText, bool readOnly)
            : base(cssText, readOnly)
        {
            OnSetCssText(cssText);
        }

        public override string CssText
        {
            get {
                double dbl = GetFloatValue(CssPrimitiveType.Number);
                return Convert.ToInt32(dbl).ToString();
            }
        }

        protected override void OnSetCssText(string cssText)
        {
            if (cssText.EndsWith("%", StringComparison.OrdinalIgnoreCase))
            {
                cssText = cssText.Remove(cssText.Length - 1, 1);
                SetPrimitiveType(CssPrimitiveType.Percentage);
            }
            else
            {
                SetPrimitiveType(CssPrimitiveType.Number);

            }
            var color = double.Parse(cssText, CssNumber.Format);
            if (color <= 0)
            {
                color = 0;
            }
            else if (color > 0 && color < 1)
            {
                color = (int)(255 * color);
            }
            else if (color > 255)
            {
                color = 255;
            }
            SetFloatValue(color);
        }

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            double ret = double.NaN;
            switch (unitType)
            {
                case CssPrimitiveType.Number:
                    if (PrimitiveType == CssPrimitiveType.Number)
                    {
                        ret = _floatValue;
                    }
                    else if (PrimitiveType == CssPrimitiveType.Percentage)
                    {
                        ret = _floatValue / 100 * 255D;
                    }
                    break;
                case CssPrimitiveType.Percentage:
                    if (PrimitiveType == CssPrimitiveType.Percentage)
                    {
                        ret = _floatValue;
                    }
                    else if (PrimitiveType == CssPrimitiveType.Number)
                    {
                        ret = _floatValue * 255D;
                    }
                    break;
            }
            if (double.IsNaN(ret))
            {
                throw new DomException(DomExceptionType.InvalidAccessErr);
            }
            return ret;
        }
    }
}
