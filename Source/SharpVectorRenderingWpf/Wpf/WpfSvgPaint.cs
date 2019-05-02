using System;
using System.Xml;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
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

        private WpfSvgPaint(string str)
            : base(str)
        {
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

        public bool IsUserSpace
        {
            get {
                if (_paintFill != null)
                {
                    return _paintFill.IsUserSpace;
                }
                return false;
            }
        }

        public WpfFillType FillType
        {
            get {
                if (_paintFill != null)
                {
                    return _paintFill.FillType;
                }
                return WpfFillType.None;
            }
        }

        public bool IsFillTransformable
        {
            get {
                if (_paintFill != null)
                {
                    if (_paintFill.FillType == WpfFillType.Gradient)
                    {
                        if (!_paintFill.IsUserSpace)
                        {
                            return false;
                        }
                    }
                    if (_paintFill.FillType == WpfFillType.Pattern)
                    {
                        if (!_paintFill.IsUserSpace)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public WpfSvgPaint WpfFallback
        {
            get {
                if (this.Fallback != null)
                {
                    WpfSvgPaint fallbackPaint = new WpfSvgPaint(this.Fallback.CssText);
                    fallbackPaint._propertyName = this._propertyName;
                    fallbackPaint._element = this._element;
                    fallbackPaint._context = this._context;

                    return fallbackPaint;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public Brush GetBrush()
        {
            return GetBrush(null, "fill", true);
        }

        public Brush GetBrush(bool setOpacity)
        {
            return GetBrush(null, "fill", setOpacity);
        }

        public Brush GetBrush(Geometry geometry)
        {
            return GetBrush(geometry, "fill", true);
        }

        public Brush GetBrush(Geometry geometry, bool setOpacity)
        {
            return GetBrush(geometry, "fill", setOpacity);
        }

        public Pen GetPen(bool setOpacity = true)
        {
            return this.GetPen(null, setOpacity);
        }

        public Pen GetPen(Geometry geometry, bool setOpacity = true)
        {
            double strokeWidth = GetStrokeWidth();
            if (strokeWidth.Equals(0.0d)) 
                return null;

            WpfSvgPaintContext paintContext = null;

            SvgPaintType paintType = this.PaintType;

            WpfSvgPaint stroke;
            if (paintType == SvgPaintType.None)
            {
                return null;
            }
            if (paintType == SvgPaintType.CurrentColor)
            {
                stroke = new WpfSvgPaint(_context, _element, "color");
            }
            else if (paintType == SvgPaintType.ContextFill)
            {
                paintContext = GetFillContext();
                if (paintContext != null)
                {
                    stroke = paintContext.Fill;
                }
                else
                {
                    stroke = this;
                }
            }
            else if (paintType == SvgPaintType.ContextStroke)
            {
                paintContext = GetStrokeContext();
                if (paintContext != null)
                {
                    stroke = paintContext.Stroke;
                }
                else
                {
                    stroke = this;
                }
            }
            else
            {
                stroke = this;
            }

            Pen pen = new Pen(stroke.GetBrush(geometry, "stroke", setOpacity), strokeWidth);

            pen.StartLineCap  = pen.EndLineCap = GetLineCap();
            pen.LineJoin      = GetLineJoin();
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
                    if (dashArray[i].Equals(0.0d))
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

        public WpfSvgPaint GetScopeStroke()
        {
            WpfSvgPaintContext paintContext = this.GetStrokeContext();
            if (paintContext != null)
            {
                return paintContext.Stroke;
            }
            return null;
        }

        #endregion

        #region Private Methods

        private WpfSvgPaintContext GetFillContext()
        {
            WpfSvgPaintContext paintContext = null;
            if (_element != null && _context != null)
            {
                SvgElement element = _element;
                paintContext = _context.GetPaintContext(element.UniqueId);
                while (element != null && (paintContext != null && paintContext.Fill == null))
                {
                    element = element.ParentNode as SvgElement;
                    paintContext = null;
                    if (element != null)
                    {
                        paintContext = _context.GetPaintContext(element.UniqueId);

                        if (paintContext != null && paintContext.HasTarget)
                        {
                            if (paintContext.Fill == null)
                            {
                                paintContext = _context.GetPaintContext(paintContext.TargetId);
                            }
                        }
                    }
                }
            }
            return paintContext;
        }

        private WpfSvgPaintContext GetStrokeContext()
        {
            WpfSvgPaintContext paintContext = null;
            if (_element != null && _context != null)
            {
                SvgElement element = _element;
                paintContext = _context.GetPaintContext(element.UniqueId);
                while (element != null && (paintContext != null && paintContext.Stroke == null))
                {
                    element = element.ParentNode as SvgElement;
                    paintContext = null;
                    if (element != null)
                    {
                        paintContext = _context.GetPaintContext(element.UniqueId);

                        if (paintContext != null && paintContext.HasTarget)
                        {
                            if (paintContext.Stroke == null)
                            {
                                paintContext = _context.GetPaintContext(paintContext.TargetId);
                            }
                        }
                    }
                }
            }
            return paintContext;
        }

        private double GetOpacity(string fillOrStroke)
        {
            double opacityValue = 1;

            string opacity = _element.GetPropertyValue(fillOrStroke + "-opacity");
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue *= SvgNumber.ParseNumber(opacity);
            }

            opacity = _element.GetPropertyValue("opacity");
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue *= SvgNumber.ParseNumber(opacity);
            }

            opacityValue = Math.Min(opacityValue, 1);
            opacityValue = Math.Max(opacityValue, 0);

            return opacityValue;
        }

        private PenLineCap GetLineCap()
        {
            switch (_element.GetPropertyValue("stroke-linecap").Trim())
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
            switch (_element.GetPropertyValue("stroke-linejoin").Trim())
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
            if (strokeWidth.Length == 0)
                strokeWidth = "1px";

            SvgLength strokeWidthLength = new SvgLength(_element, "stroke-width", 
                SvgLengthDirection.Viewport, strokeWidth);

            return strokeWidthLength.Value;
        }

        private double GetMiterLimit(double strokeWidth)
        {
            // Use this to prevent the default value of "4" being used...
            string miterLimitAttr = _element.GetAttribute("stroke-miterlimit");
            if (string.IsNullOrWhiteSpace(miterLimitAttr))
            {
                string strokeLinecap = _element.GetAttribute("stroke-linecap"); 
                if (string.Equals(strokeLinecap, "round", StringComparison.OrdinalIgnoreCase))
                {
                    return 1.0d;
                }              
                return -1.0d;
            }                 

            string miterLimitStr = _element.GetPropertyValue("stroke-miterlimit");
            if (string.IsNullOrWhiteSpace(miterLimitStr) || (float)(strokeWidth) <= 0)
            {
                return -1.0d;
            }

            double miterLimit = SvgNumber.ParseNumber(miterLimitStr);
            if (miterLimit < 1)
            {
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr, 
                    "stroke-miterlimit can not be less then 1");
            }

            //if (miterLimit < 1.0d)
            //{
            //    return -1.0d;
            //}

            double ratioLimit = miterLimit / strokeWidth;
            if (ratioLimit >= 1.8d)
            {
                return miterLimit;
            }

            return 1.0d;
        }

        private DoubleCollection GetDashArray(double strokeWidth)
        {
            string dashArrayText = _element.GetPropertyValue("stroke-dasharray");
            if (string.IsNullOrWhiteSpace(dashArrayText))
            {
                return null;
            }

            if (dashArrayText.Equals("none", StringComparison.OrdinalIgnoreCase)
                || dashArrayText.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                // NOTE: Rule changed in Second Edition of Test Suite 1.1
                //string dashArrayAttr = _element.GetAttribute("stroke-dasharray");
                //if (string.IsNullOrWhiteSpace(dashArrayAttr) ||
                //    dashArrayAttr.Equals("none", StringComparison.OrdinalIgnoreCase)
                //    || dashArrayAttr.Equals("null", StringComparison.OrdinalIgnoreCase))
                //{
                //    return null;
                //}
                //dashArrayText = dashArrayAttr;

                return null;
            }

            SvgNumberList list = new SvgNumberList(dashArrayText);

            uint len = list.NumberOfItems;
            DoubleCollection dashArray = new DoubleCollection((int)len);

            for (uint i = 0; i < len; i++)
            {
                //divide by strokeWidth to take care of the difference between Svg and WPF
                dashArray.Add(list.GetItem(i).Value / strokeWidth);
            }

            return dashArray;
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

            return 0;
        }

        private WpfFill GetPaintFill(string uri)
        {
            string absoluteUri = _element.ResolveUri(uri);

            if (_element.Imported && _element.ImportDocument != null && _element.ImportNode != null)
            {
                // We need to determine whether the provided URI refers to element in the
                // original document or in the current document...
                SvgStyleableElement styleElm = _element.ImportNode as SvgStyleableElement;
                if (styleElm != null)
                {
                    string propertyValue = styleElm.GetComputedStyle("").GetPropertyValue(_propertyName);

                    if (!string.IsNullOrWhiteSpace(propertyValue))
                    {
                        WpfSvgPaint importFill = new WpfSvgPaint(_context, styleElm, _propertyName);
                        if (string.Equals(uri, importFill.Uri, StringComparison.OrdinalIgnoreCase))
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

        private WpfSvgPaint GetDeferredFill()
        {
            var nodeList = _element.OwnerDocument.SelectNodes("//*[@fill='currentColor']");
            if (nodeList == null || nodeList.Count == 0)
            {
                return null;
            }
            // Try a shortcut...
            SvgStyleableElement svgElement = _element.ParentNode as SvgStyleableElement;
            if (svgElement != null && svgElement.HasAttribute("fill")
                && string.Equals("currentColor", svgElement.GetAttribute("fill")))
            {
                string color = svgElement.GetAttribute("color");
                if (!string.IsNullOrWhiteSpace(color))
                {
                    return new WpfSvgPaint(_context, svgElement, "color");
                }
            }

            foreach (XmlNode nodeItem in nodeList)
            {
                svgElement = nodeItem as SvgStyleableElement;
                if (svgElement == null)
                {
                    continue;
                }

                var childNodeList = svgElement.GetElementsByTagName(_element.LocalName);
                if (childNodeList != null && childNodeList.Count != 0)
                {
                    foreach (var childNode in childNodeList)
                    {
                        if (childNode == _element)
                        {
                            string color = svgElement.GetAttribute("color");
                            if (!string.IsNullOrWhiteSpace(color))
                            {
                                return new WpfSvgPaint(_context, svgElement, "color");
                            }
                       }
                    }
                }
            }
            return null;
        }

        private Brush GetBrush(Geometry geometry, string propPrefix, bool setOpacity)
        {
            WpfSvgPaintContext paintContext = null;

            SvgPaintType paintType = this.PaintType;

            SvgPaint fill;
            if (paintType == SvgPaintType.None)
            {
                return null;
            }
            if (paintType == SvgPaintType.CurrentColor)
            {
                //TODO: Find a better way to support currentColor specified on parent element.
                var deferredFill = this.GetDeferredFill();
                if (deferredFill == null)
                {
                    fill = new WpfSvgPaint(_context, _element, "color");
                }
                else
                {
                    fill = deferredFill;
                }
            }
            else if (paintType == SvgPaintType.ContextFill)
            {
                paintContext = GetFillContext();
                if (paintContext != null)
                {
                    fill = paintContext.Fill;
                }
                else
                {
                    fill = this;
                }
            }
            else if (paintType == SvgPaintType.ContextStroke)
            {
                paintContext = GetStrokeContext();
                if (paintContext != null)
                {
                    fill = paintContext.Stroke;
                }
                else
                {
                    fill = this;
                }
            }
            else
            {
                fill = this;
            }

            SvgPaintType fillType = fill.PaintType;
            if (fillType == SvgPaintType.Uri     || fillType == SvgPaintType.UriCurrentColor ||
                fillType == SvgPaintType.UriNone || fillType == SvgPaintType.UriRgbColor     ||
                fillType == SvgPaintType.UriRgbColorIccColor)
            {
                _paintFill = GetPaintFill(fill.Uri);
                if (_paintFill != null)
                {
                    Brush brush = null;
                    if (geometry != null)
                    {
                        brush = _paintFill.GetBrush(geometry.Bounds, _context, geometry.Transform);
                    }
                    else
                    {
                        brush = _paintFill.GetBrush(Rect.Empty, _context, null);
                    }

                    if (brush != null)
                    {
                        brush.Opacity = GetOpacity(propPrefix);
                    }

                    return brush;
                }
                else
                {
                    if (paintType == SvgPaintType.UriNone || paintType == SvgPaintType.Uri)
                    {
                        return null;
                    }
                    if (paintType == SvgPaintType.UriCurrentColor)
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

            Color? solidColor = WpfConvert.ToColor(fill.RgbColor);
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
