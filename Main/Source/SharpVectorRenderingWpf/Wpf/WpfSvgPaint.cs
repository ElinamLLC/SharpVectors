using System;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgPaint : SvgPaint
    {
        #region Private Fields

        private string  _propertyName;
        private WpfFill _paintFill;
        private SvgStyleableElement _element;
        private WpfDrawingContext _context;

        #endregion

        #region Constructors and Destructor

        public WpfSvgPaint(WpfDrawingContext context, SvgStyleableElement elm, string propName)
            : base(elm.GetComputedStyle("").GetPropertyValue(propName))
        {
            _propertyName = propName;
            _element      = elm;
            _context      = context;
        }

        #endregion

        #region Public Properties

        public WpfFill PaintServer
        {
            get
            {
                return _paintFill;
            }
        }

        #endregion

        #region Public Methods

        public Brush GetBrush()
        {
            return GetBrush("fill", true);
        }

        public Brush GetBrush(bool setOpacity)
        {
            return GetBrush("fill", setOpacity);
        }

        public Pen GetPen()
        {
            double strokeWidth = GetStrokeWidth();
            if (strokeWidth == 0) 
                return null;

            WpfSvgPaint stroke;
            if (PaintType == SvgPaintType.None)
            {
                return null;
            }
            else if (PaintType == SvgPaintType.CurrentColor)
            {
                stroke = new WpfSvgPaint(_context, _element, "color");
            }
            else
            {
                stroke = this;
            }

            Pen pen = new Pen(stroke.GetBrush("stroke", true), strokeWidth);

            pen.StartLineCap = pen.EndLineCap = GetLineCap();
            pen.LineJoin     = GetLineJoin();
            double miterLimit = GetMiterLimit(strokeWidth);
            if (miterLimit > 0)
            {
                pen.MiterLimit = miterLimit;
            }

            //pen.MiterLimit = 1.0f;

            DoubleCollection dashArray = GetDashArray(strokeWidth);
            if (dashArray != null && dashArray.Count != 0)
            {
                bool isValidDashes = true;
                //Do not draw if dash array had a zero value in it
                for (int i = 0; i < dashArray.Count; i++)
                {
                    if (dashArray[i] == 0)
                    {
                        isValidDashes = false;
                    }
                }

                if (isValidDashes)
                {   
                    DashStyle dashStyle = new DashStyle(dashArray, GetDashOffset(strokeWidth));

                    pen.DashStyle = dashStyle;
                    // This is the one that works well for the XAML, the default is not Flat as
                    // stated in the documentations...
                    pen.DashCap   = PenLineCap.Flat; 
                }
            }

            return pen;
        }

        #endregion

        #region Private Methods

        private double GetOpacity(string fillOrStroke)
        {
            double opacityValue = 1;

            string opacity = _element.GetPropertyValue(fillOrStroke + "-opacity");
            if (opacity != null && opacity.Length > 0)
            {
                opacityValue *= SvgNumber.ParseNumber(opacity);
            }

            opacity = _element.GetPropertyValue("opacity");
            if (opacity != null && opacity.Length > 0)
            {
                opacityValue *= SvgNumber.ParseNumber(opacity);
            }

            opacityValue = Math.Min(opacityValue, 1);
            opacityValue = Math.Max(opacityValue, 0);

            return opacityValue;
        }

        private PenLineCap GetLineCap()
        {
            switch (_element.GetPropertyValue("stroke-linecap"))
            {
                case "round":
                    return PenLineCap.Round;
                case "square":
                    return PenLineCap.Square;
                case "butt":
                    return PenLineCap.Flat;
               case "triangle":
                    return PenLineCap.Triangle;
                default:
                    return PenLineCap.Flat;
            }
        }

        private PenLineJoin GetLineJoin()
        {
            switch (_element.GetPropertyValue("stroke-linejoin"))
            {
                case "round":
                    return PenLineJoin.Round;
                case "bevel":
                    return PenLineJoin.Bevel;
                default:
                    return PenLineJoin.Miter;
            }
        }

        private double GetStrokeWidth()
        {
            string strokeWidth = _element.GetPropertyValue("stroke-width");
            if (strokeWidth.Length == 0) strokeWidth = "1px";

            SvgLength strokeWidthLength = new SvgLength(_element, "stroke-width", 
                SvgLengthDirection.Viewport, strokeWidth);

            return strokeWidthLength.Value;
        }

        private double GetMiterLimit(double strokeWidth)
        {
            // Use this to prevent the default value of "4" being used...
            string miterLimitAttr = _element.GetAttribute("stroke-miterlimit");
            if (String.IsNullOrEmpty(miterLimitAttr))
            {
                string strokeLinecap = _element.GetAttribute("stroke-linecap"); 
                if (String.Equals(strokeLinecap, "round", StringComparison.OrdinalIgnoreCase))
                {
                    return 1.0d;
                }              
                return -1.0d;
            }                 

            string miterLimitStr = _element.GetPropertyValue("stroke-miterlimit");
            if (String.IsNullOrEmpty(miterLimitStr) || (float)(strokeWidth) <= 0)
            {
                return -1.0d;
            }

            double miterLimit = SvgNumber.ParseNumber(miterLimitStr);
            if (miterLimit < 1) 
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr, 
                    "stroke-miterlimit can not be less then 1");

            //if (miterLimit < 1.0d)
            //{
            //    return -1.0d;
            //}

            double ratioLimit = miterLimit / strokeWidth;
            if (ratioLimit >= 1.8d)
            {
                return miterLimit;
            }
            else
            {
                return 1.0d;
            }
        }

        private DoubleCollection GetDashArray(double strokeWidth)
        {
            string dashArrayText = _element.GetPropertyValue("stroke-dasharray");
            if (String.IsNullOrEmpty(dashArrayText))
            {
                return null;
            }

            if (dashArrayText == "none")
            {
                return null;
            }
            else
            {
                SvgNumberList list = new SvgNumberList(dashArrayText);

                uint len = list.NumberOfItems;
                //float[] fDashArray = new float[len];
                DoubleCollection dashArray = new DoubleCollection((int)len);

                for (uint i = 0; i < len; i++)
                {
                    //divide by strokeWidth to take care of the difference between Svg and WPF
                    dashArray.Add(list.GetItem(i).Value / strokeWidth);
                }

                return dashArray;
            }
        }

        private double GetDashOffset(double strokeWidth)
        {
            string dashOffset = _element.GetPropertyValue("stroke-dashoffset");
            if (dashOffset.Length > 0)
            {
                //divide by strokeWidth to take care of the difference between Svg and GDI+
                SvgLength dashOffsetLength = new SvgLength(_element, "stroke-dashoffset", 
                    SvgLengthDirection.Viewport, dashOffset);
                return dashOffsetLength.Value;
            }
            else
            {
                return 0;
            }
        }

        private WpfFill GetPaintFill(string uri)
        {
            string absoluteUri = _element.ResolveUri(uri);

            if (_element.Imported && _element.ImportDocument != null && 
                _element.ImportNode != null)
            {
                // We need to determine whether the provided URI refers to element in the
                // original document or in the current document...
                SvgStyleableElement styleElm = _element.ImportNode as SvgStyleableElement;
                if (styleElm != null)
                {
                    string propertyValue =
                        styleElm.GetComputedStyle("").GetPropertyValue(_propertyName);

                    if (!String.IsNullOrEmpty(propertyValue))
                    {
                        WpfSvgPaint importFill = new WpfSvgPaint(_context, styleElm, _propertyName);
                        if (String.Equals(uri, importFill.Uri, StringComparison.OrdinalIgnoreCase))
                        {
                            WpfFill fill = WpfFill.CreateFill(_element.ImportDocument, absoluteUri);
                            if (fill != null)
                            {
                                return fill;
                            }
                        }
                    }
                }
            }

            return WpfFill.CreateFill(_element.OwnerDocument, absoluteUri);
        }

        private Brush GetBrush(string propPrefix, bool setOpacity)
        {
            SvgPaint fill;
            if (PaintType == SvgPaintType.None)
            {
                return null;
            }
            else if (PaintType == SvgPaintType.CurrentColor)
            {
                fill = new WpfSvgPaint(_context, _element, "color");
            }
            else
            {
                fill = this;
            }

            SvgPaintType paintType = fill.PaintType;
            if (paintType == SvgPaintType.Uri     || paintType == SvgPaintType.UriCurrentColor ||
                paintType == SvgPaintType.UriNone || paintType == SvgPaintType.UriRgbColor     ||
                paintType == SvgPaintType.UriRgbColorIccColor)
            {
                _paintFill = GetPaintFill(fill.Uri);
                if (_paintFill != null)
                {
                    Brush brush = _paintFill.GetBrush(_context);

                    if (brush != null)
                    {
                        brush.Opacity = GetOpacity(propPrefix);
                    }

                    return brush;
                }
                else
                {
                    if (PaintType == SvgPaintType.UriNone || PaintType == SvgPaintType.Uri)
                    {
                        return null;
                    }
                    else if (PaintType == SvgPaintType.UriCurrentColor)
                    {
                        fill = new WpfSvgPaint(_context, _element, "color");
                    }
                    else
                    {
                        fill = this;
                    }
                }
            }

            if (fill == null || fill.RgbColor == null)
            {
                return null;
            }

            Color? solidColor = WpfConverter.ToColor(fill.RgbColor);
            if (solidColor == null)
            {
                return null;
            }

            SolidColorBrush solidBrush = new SolidColorBrush(solidColor.Value);
            //int opacity = GetOpacity(propPrefix);
            //solidBrush.Color = Color.FromArgb(opacity, brush.Color);
            if (setOpacity)
            {
                solidBrush.Opacity = GetOpacity(propPrefix);
            }
            return solidBrush;
        }

        #endregion
    }
}
