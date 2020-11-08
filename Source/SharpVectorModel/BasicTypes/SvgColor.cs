using System;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgColor.
    /// </summary>
    public class SvgColor : CssValue, ISvgColor
    {
        #region Private Level Fields 

        protected CssColor _rgbColor;
        private SvgColorType _colorType;

        #endregion

        #region Constructors and Destructor

        protected SvgColor()
            : base(CssValueType.PrimitiveValue, string.Empty, false)
        {
        }

        public SvgColor(string str)
            : base(CssValueType.PrimitiveValue, str, false)
        {
            ParseColor(str);
        }

        #endregion

        #region Public Properties

        public override string CssText
        {
            get {
                string ret;
                switch (ColorType)
                {
                    case SvgColorType.RgbColor:
                        ret = _rgbColor.CssText;
                        break;
                    case SvgColorType.RgbColorIccColor:
                        ret = _rgbColor.CssText;
                        break;
                    case SvgColorType.CurrentColor:
                        ret = "currentColor";
                        break;
                    default:
                        ret = string.Empty;
                        break;
                }
                return ret;
            }
            set {
                base.CssText = value;
                ParseColor(value);
            }
        }

        public SvgColorType ColorType
        {
            get {
                return _colorType;
            }
        }

        public ICssColor RgbColor
        {
            get {
                return _rgbColor;
            }
        }

        public ISvgIccColor IccColor
        {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods

        public void SetRgbColor(string rgbColor)
        {
            SetColor(SvgColorType.RgbColor, rgbColor, string.Empty);
        }

        public void SetRgbColorIccColor(string rgbColor, string iccColor)
        {
            SetColor(SvgColorType.RgbColorIccColor, rgbColor, iccColor);
        }

        public void SetColor(SvgColorType colorType, string rgbColor, string iccColor)
        {
            _colorType = colorType;
            if (!string.IsNullOrWhiteSpace(rgbColor))
            {
                try
                {
                    _rgbColor = new CssColor(rgbColor);

                    if (_rgbColor.IsSystemColor)
                    {
                        _colorType = SvgColorType.SystemColor;
                    }
                }
                catch (DomException domExc)
                {
                    throw new SvgException(SvgExceptionType.SvgInvalidValueErr,
                        "Invalid color value: " + rgbColor, domExc);
                }
            }
            else
            {
                _rgbColor = new CssColor(CssConstants.ValBlack);
            }

            //TODO--PAUL: deal with ICC colors
        }

        #endregion

        #region Protected Methods

        protected void ParseColor(string str)
        {
            str = str.Trim();
            if (str.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
            {
                SetColor(SvgColorType.CurrentColor, null, null);
            }
            else if (str.IndexOf("icc-color(", StringComparison.OrdinalIgnoreCase) > -1)
            {
                int iccStart  = str.IndexOf("icc-color(", StringComparison.OrdinalIgnoreCase);
                string strRgb = str.Substring(0, iccStart).Trim();
                string strIcc = str.Substring(iccStart);

                SetColor(SvgColorType.RgbColorIccColor, strRgb, strIcc);
            }
            else
            {
                SetColor(SvgColorType.RgbColor, str, string.Empty);
            }
        }

        #endregion
    }
}
