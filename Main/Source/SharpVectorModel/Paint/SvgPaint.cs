using System;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public class SvgPaint : SvgColor, ISvgPaint
    {
        #region Private Fields

        private string _uri;
        private SvgPaintType _paintType;

        #endregion

        #region Constructors and Destructor

        public SvgPaint(string str)
            : base()
        {
            _uri = String.Empty;
            ParsePaint(str);
        }

        #endregion

        #region Private methods

        private void ParsePaint(string str)
        {
            bool hasUri = false;
            bool hasRgb = false;
            bool hasIcc = false;
            bool hasNone = false;
            bool hasCurrentColor = false;

            str = str.Trim();

            if (str.StartsWith("url("))
            {
                hasUri = true;
                int endUri = str.IndexOf(")");
                _uri = str.Substring(4, endUri - 4);
                str = str.Substring(endUri + 1).Trim();
            }

            if (str.Equals("currentColor"))
            {
                base.ParseColor(str);
                hasCurrentColor = true;
            }
            else if (str.Equals("none"))
            {
                hasNone = true;
            }
            else if (str.Length > 0)
            {
                base.ParseColor(str);
                hasRgb = true;
                hasIcc = (base.ColorType == SvgColorType.RgbColorIccColor);
            }

            SetPaintType(hasUri, hasRgb, hasIcc, hasNone, hasCurrentColor);
        }

        private void SetPaintType(bool hasUri, bool hasRgb, bool hasIcc, 
            bool hasNone, bool hasCurrentColor)
        {
            if (hasUri)
            {
                if (hasRgb)
                {
                    if (hasIcc)
                    {
                        _paintType = SvgPaintType.UriRgbColorIccColor;
                    }
                    else
                    {
                        _paintType = SvgPaintType.UriRgbColor;
                    }
                }
                else if (hasNone)
                {
                    _paintType = SvgPaintType.UriNone;
                }
                else if (hasCurrentColor)
                {
                    _paintType = SvgPaintType.UriCurrentColor;
                }
                else
                {
                    _paintType = SvgPaintType.Uri;
                }
            }
            else
            {
                if (hasRgb)
                {
                    if (hasIcc)
                    {
                        _paintType = SvgPaintType.RgbColorIccColor;
                    }
                    else
                    {
                        _paintType = SvgPaintType.RgbColor;
                    }
                }
                else if (hasNone)
                {
                    _paintType = SvgPaintType.None;
                }
                else if (hasCurrentColor)
                {
                    _paintType = SvgPaintType.CurrentColor;
                }
                else
                {
                    _paintType = SvgPaintType.Unknown;
                }
            }
        }

        #endregion

        #region ISvgPaint Members

        public override string CssText
        {
            get
            {
                string cssText;
                switch (_paintType)
                {
                    case SvgPaintType.CurrentColor:
                    case SvgPaintType.RgbColor:
                    case SvgPaintType.RgbColorIccColor:
                        cssText = base.CssText;
                        break;
                    case SvgPaintType.None:
                        cssText = "none";
                        break;
                    case SvgPaintType.UriNone:
                        cssText = "url(" + _uri + ") none";
                        break;
                    case SvgPaintType.Uri:
                        cssText = "url(" + _uri + ")";
                        break;
                    case SvgPaintType.UriCurrentColor:
                    case SvgPaintType.UriRgbColor:
                    case SvgPaintType.UriRgbColorIccColor:
                        cssText = "url(" + _uri + ") " + base.CssText;
                        break;
                    default:
                        cssText = String.Empty;
                        break;
                }
                return cssText;
            }
            set
            {
                ParsePaint(value);
            }
        }

        public SvgPaintType PaintType
        {
            get
            {
                return _paintType;
            }
        }
        
        public string Uri
        {
            get
            {
                return _uri;
            }
        }

        public void SetUri(string uri)
        {
            _paintType = SvgPaintType.Uri;
            _uri = uri;
        }

        public void SetPaint(SvgPaintType paintType, string uri, string rgbColor, string iccColor)
        {
            _paintType = paintType;

            // check URI
            switch (_paintType)
            {
                case SvgPaintType.Uri:
                case SvgPaintType.UriCurrentColor:
                case SvgPaintType.UriNone:
                case SvgPaintType.UriRgbColor:
                case SvgPaintType.UriRgbColorIccColor:
                    if (uri == null)
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing URI");
                    }
                    else
                    {
                        _uri = uri;

                    }
                    break;
                default:
                    if (uri != null)
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "URI must be null");
                    }
                    break;
            }

            // check RGB and ICC color
            switch (_paintType)
            {
                case SvgPaintType.CurrentColor:
                case SvgPaintType.UriCurrentColor:
                    base.ParseColor("currentColor");
                    break;
                case SvgPaintType.RgbColor:
                case SvgPaintType.UriRgbColor:
                    if (rgbColor != null && rgbColor.Length > 0)
                    {
                        base.SetRgbColor(rgbColor);
                    }
                    else
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing RGB color");
                    }
                    break;
                case SvgPaintType.RgbColorIccColor:
                case SvgPaintType.UriRgbColorIccColor:
                    if (rgbColor != null && rgbColor.Length > 0 &&
                        iccColor != null && iccColor.Length > 0)
                    {
                        base.SetRgbColorIccColor(rgbColor, iccColor);
                    }
                    else
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing RGB or ICC color");
                    }
                    break;
                default:
                    if (rgbColor != null)
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "rgbColor must be null");
                    }
                    break;
            }
        }

        #endregion
    }
}
