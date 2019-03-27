using System;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    public class SvgPaint : SvgColor, ISvgPaint
    {
        #region Private Fields

        private string _uri;
        private SvgPaintType _paintType;

        private SvgPaint _fallback;

        #endregion

        #region Constructors and Destructor

        public SvgPaint(string str)
        {
            _uri = string.Empty;
            ParsePaint(str);
        }

        #endregion

        #region Public Properties

        public SvgPaint Fallback
        {
            get {
                return _fallback;
            }
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

            const StringComparison compareType = StringComparison.OrdinalIgnoreCase;

            str = str.Trim();

            if (string.IsNullOrWhiteSpace(str) || str.Equals("none", compareType) 
                || str.Equals("transparent", compareType) || str.Equals("null", compareType))
            {
                hasNone = true;
            }
            else if (str.Equals("currentColor", compareType))
            {
                base.ParseColor(str);
                hasCurrentColor = true;
            }
            else if (str.Equals("context-fill", compareType) || str.Equals("contextFill", compareType))
            {
                _paintType = SvgPaintType.ContextFill;
                return;
            }
            else if (str.Equals("context-stroke", compareType) || str.Equals("contextStroke", compareType))
            {
                _paintType = SvgPaintType.ContextStroke;
                return;
            }
            else
            {
                List<string> strList = new List<string>();

                while (!string.IsNullOrWhiteSpace(str))
                {
                    if (str.StartsWith("url(", compareType))
                    {
                        var endUri = str.IndexOf(')', 4);
                        strList.Add(str.Substring(0, endUri + 1));
                        str = str.Substring(endUri + 1).Trim();
                    }
                    else if (str.StartsWith("rgb(", compareType))
                    {
                        var leftParen = str.IndexOf(')', 4);
                        strList.Add(str.Substring(0, leftParen + 1));
                        str = str.Substring(leftParen + 1).Trim();
                    }
                    else if (str.StartsWith("rgba(", compareType))
                    {
                        var leftParen = str.IndexOf(')', 5);
                        strList.Add(str.Substring(0, leftParen + 1));
                        str = str.Substring(leftParen + 1).Trim();
                    }
                    else if (str.StartsWith("hsl(", compareType))
                    {
                        var leftParen = str.IndexOf(')', 4);
                        strList.Add(str.Substring(0, leftParen + 1));
                        str = str.Substring(leftParen + 1).Trim();
                    }
                    else if (str.StartsWith("hsla(", compareType))
                    {
                        var leftParen = str.IndexOf(')', 5);
                        strList.Add(str.Substring(0, leftParen + 1));
                        str = str.Substring(leftParen + 1).Trim();
                    }
                    else if (str.StartsWith("#", compareType)) // Otherwise try and parse as colour
                    {
                        switch (CountHexDigits(str, 1))
                        {
                            // RGB syntax variations
                            case 3:
                                strList.Add(str.Substring(0, 4));
                                str = str.Substring(4).Trim();
                                break;
                            case 6:
                                strList.Add(str.Substring(0, 7));
                                str = str.Substring(7).Trim();
                                break;
                            // RGB transparency variations
                            case 4:
                                strList.Add(str.Substring(0, 5));
                                str = str.Substring(5).Trim();
                                break;
                            case 8:
                                strList.Add(str.Substring(0, 9));
                                str = str.Substring(9).Trim();
                                break;
                            default:
                                strList.Add(str);
                                break;
                        }
                    }
                    else
                    {
                        strList.Add(str.Trim());
                        break;
                    }
                }

                if (strList.Count > 1)
                {
                    _fallback = new SvgPaint(strList[1]);

                    this.ParsePaint(strList[0]);
                    return;
                }
                else
                {
                    str = strList[0];

                    if (str.StartsWith("url(", compareType))
                    {
                        hasUri = true;
                        int endUri = str.IndexOf(")", compareType);
                        _uri = str.Substring(4, endUri - 4);
                        str = str.Substring(endUri + 1).Trim();
                    }

                    if (str.Length > 0)
                    {
                        base.ParseColor(str);
                        hasRgb = true;
                        hasIcc = (base.ColorType == SvgColorType.RgbColorIccColor);
                    }
                }
            }

            SetPaintType(hasUri, hasRgb, hasIcc, hasNone, hasCurrentColor);
        }

        // This routine is taken from SVG.net sources: https://svg.codeplex.com/
        private static int CountHexDigits(string value, int start)
        {
            int i = (start < 0) ? 0 : start;
            int count = 0;
            while (i < value.Length &&
                   ((value[i] >= '0' && value[i] <= '9') ||
                    (value[i] >= 'a' && value[i] <= 'f') ||
                    (value[i] >= 'A' && value[i] <= 'F')))
            {
                count++;
                i++;
            }
            return count;
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
                        cssText = string.Empty;
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
