using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiSvgPaint : SvgPaint
    {
        #region Private Fields

        private GdiFill _paintFill;
        private SvgStyleableElement _element;

        #endregion

        #region Constructors and Destructor

        public GdiSvgPaint(SvgStyleableElement elm, string propName)
            : base(elm.GetComputedStyle("").GetPropertyValue(propName))
        {
            _element = elm;
        }

        #endregion

        #region Public Properties

        public GdiFill PaintFill
        {
            get
            {
                return _paintFill;
            }
        }

        #endregion

        #region Public Methods

        public Brush GetBrush(GraphicsPath gp)
        {
            return GetBrush(gp, "fill");
        }

        public Pen GetPen(GraphicsPath gp)
        {
            float strokeWidth = GetStrokeWidth();
            if (strokeWidth == 0) return null;

            GdiSvgPaint stroke;
            if (PaintType == SvgPaintType.None)
            {
                return null;
            }
            else if (PaintType == SvgPaintType.CurrentColor)
            {
                stroke = new GdiSvgPaint(_element, "color");
            }
            else
            {
                stroke = this;
            }

            Pen pen = new Pen(stroke.GetBrush(gp, "stroke"), strokeWidth);

            pen.StartCap   = pen.EndCap = GetLineCap();
            pen.LineJoin   = GetLineJoin();
            pen.MiterLimit = GetMiterLimit();

            float[] fDashArray = GetDashArray(strokeWidth);
            if (fDashArray != null)
            {
                // Do not draw if dash array had a zero value in it

                for (int i = 0; i < fDashArray.Length; i++)
                {
                    if (fDashArray[i] == 0)
                        return null;
                }

                pen.DashPattern = fDashArray;
            }

            pen.DashOffset = GetDashOffset(strokeWidth);

            return pen;
        }

        #endregion

        #region Private Methods

        private int GetOpacity(string fillOrStroke)
        {
            double alpha = 255;
            string opacity;

            opacity = _element.GetPropertyValue(fillOrStroke + "-opacity");
            if (opacity.Length > 0) alpha *= SvgNumber.ParseNumber(opacity);

            opacity = _element.GetPropertyValue("opacity");
            if (opacity.Length > 0) alpha *= SvgNumber.ParseNumber(opacity);

            alpha = Math.Min(alpha, 255);
            alpha = Math.Max(alpha, 0);

            return Convert.ToInt32(alpha);
        }

        private LineCap GetLineCap()
        {
            switch (_element.GetPropertyValue("stroke-linecap"))
            {
                case "round":
                    return LineCap.Round;
                case "square":
                    return LineCap.Square;
                default:
                    return LineCap.Flat;
            }
        }

        private LineJoin GetLineJoin()
        {
            switch (_element.GetPropertyValue("stroke-linejoin"))
            {
                case "round":
                    return LineJoin.Round;
                case "bevel":
                    return LineJoin.Bevel;
                default:
                    return LineJoin.Miter;
            }
        }

        private float GetStrokeWidth()
        {
            string strokeWidth = _element.GetPropertyValue("stroke-width");
            if (strokeWidth.Length == 0) strokeWidth = "1px";

            SvgLength strokeWidthLength = new SvgLength(_element, "stroke-width", SvgLengthDirection.Viewport, strokeWidth);
            return (float)strokeWidthLength.Value;
        }

        private float GetMiterLimit()
        {
            string miterLimitStr = _element.GetPropertyValue("stroke-miterlimit");
            if (miterLimitStr.Length == 0) miterLimitStr = "4";

            float miterLimit = (float)SvgNumber.ParseNumber(miterLimitStr);
            if (miterLimit < 1) throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "stroke-miterlimit can not be less then 1");

            return miterLimit;
        }

        private float[] GetDashArray(float strokeWidth)
        {
            string dashArray = _element.GetPropertyValue("stroke-dasharray");

            if (dashArray.Length == 0 || dashArray == "none")
            {
                return null;
            }
            else
            {
                SvgNumberList list = new SvgNumberList(dashArray);

                uint len = list.NumberOfItems;
                float[] fDashArray = new float[len];

                for (uint i = 0; i < len; i++)
                {
                    //divide by strokeWidth to take care of the difference between Svg and GDI+
                    fDashArray[i] = (float)(list.GetItem(i).Value / strokeWidth);
                }

                if (len % 2 == 1)
                {
                    //odd number of values, duplicate
                    float[] tmpArray = new float[len * 2];
                    fDashArray.CopyTo(tmpArray, 0);
                    fDashArray.CopyTo(tmpArray, (int)len);

                    fDashArray = tmpArray;
                }

                return fDashArray;
            }
        }

        private float GetDashOffset(float strokeWidth)
        {
            string dashOffset = _element.GetPropertyValue("stroke-dashoffset");
            if (dashOffset.Length > 0)
            {
                //divide by strokeWidth to take care of the difference between Svg and GDI+
                SvgLength dashOffsetLength = new SvgLength(_element, "stroke-dashoffset", SvgLengthDirection.Viewport, dashOffset);
                return (float)dashOffsetLength.Value;
            }
            else
            {
                return 0;
            }
        }

        private GdiFill GetPaintFill(string uri)
        {
            string absoluteUri = _element.ResolveUri(uri);

            return GdiFill.CreateFill(_element.OwnerDocument, absoluteUri);
        }

        private Brush GetBrush(GraphicsPath gp, string propPrefix)
        {
            SvgPaint painter;
            SvgPaintType curPaintType = this.PaintType;
            if (curPaintType == SvgPaintType.None)
            {
                return null;
            }
            else if (curPaintType == SvgPaintType.CurrentColor)
            {
                painter = new GdiSvgPaint(_element, "color");
            }
            else
            {
                painter = this;
            }

            SvgPaintType paintType = painter.PaintType;
            if (paintType == SvgPaintType.Uri     || paintType == SvgPaintType.UriCurrentColor ||
                paintType == SvgPaintType.UriNone || paintType == SvgPaintType.UriRgbColor     ||
                paintType == SvgPaintType.UriRgbColorIccColor)
            {
                _paintFill = GetPaintFill(painter.Uri);
                if (_paintFill != null)
                {
                    Brush br = _paintFill.GetBrush(gp.GetBounds());

                    LinearGradientBrush lgb = br as LinearGradientBrush;
                    if (lgb != null)
                    {
                        int opacityl = GetOpacity(propPrefix);
                        for (int i = 0; i < lgb.InterpolationColors.Colors.Length; i++)
                        {
                            lgb.InterpolationColors.Colors[i] =
                                Color.FromArgb(opacityl, lgb.InterpolationColors.Colors[i]);
                        }
                        for (int i = 0; i < lgb.LinearColors.Length; i++)
                        {
                            lgb.LinearColors[i] = Color.FromArgb(opacityl, lgb.LinearColors[i]);
                        }

                        return br;
                    }

                    PathGradientBrush pgb = br as PathGradientBrush;
                    if (pgb != null)
                    {
                        int opacityl = GetOpacity(propPrefix);
                        for (int i = 0; i < pgb.InterpolationColors.Colors.Length; i++)
                        {
                            pgb.InterpolationColors.Colors[i] =
                                Color.FromArgb(opacityl, pgb.InterpolationColors.Colors[i]);
                        }
                        for (int i = 0; i < pgb.SurroundColors.Length; i++)
                        {
                            pgb.SurroundColors[i] = Color.FromArgb(opacityl, pgb.SurroundColors[i]);
                        }

                        return br;
                    }
                }
                else
                {
                    if (curPaintType == SvgPaintType.UriNone ||
                        curPaintType == SvgPaintType.Uri)
                    {
                        return null;
                    }
                    else if (curPaintType == SvgPaintType.UriCurrentColor)
                    {
                        painter = new GdiSvgPaint(_element, "color");
                    }
                    else
                    {
                        painter = this;
                    }
                }
            }

            if (painter == null || painter.RgbColor == null)
            {
                return null;
            }
                                                           
            SolidBrush brush = new SolidBrush(GdiConverter.ToColor(painter.RgbColor));
            int opacity = GetOpacity(propPrefix);
            brush.Color = Color.FromArgb(opacity, brush.Color);
            return brush;
        }

        #endregion
    }
}
