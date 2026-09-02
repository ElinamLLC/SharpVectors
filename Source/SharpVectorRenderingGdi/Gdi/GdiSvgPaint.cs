using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;

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
            : base(ResolveCssVariables(elm.GetComputedStyle(string.Empty).GetPropertyValue(propName)))
        {
            _element = elm;
        }

        /// <summary>
        /// Helper method to resolve or strip CSS variables that are unresolved.
        /// If a property value contains unresolved var() functions, returns an empty string
        /// to allow fallback to defaults instead of crashing.
        /// </summary>
        private static string ResolveCssVariables(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // If the value starts with var( and doesn't have a comma (indicating a fallback),
            // or if it's purely a var() function, we likely have an unresolved variable.
            // Return empty string to use defaults.
            if ((value.StartsWith("var(", StringComparison.OrdinalIgnoreCase) && !value.Contains(",")))
            {
                return string.Empty;
            }

            // If the value contains var( with fallback, try to extract the fallback value
            if (value.IndexOf("var(", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Simple case: "var(--name, fallback)"
                // Find the comma and extract everything after it
                int openParen = value.IndexOf("(", StringComparison.OrdinalIgnoreCase);
                int comma = value.IndexOf(",", openParen);
                if (comma > 0)
                {
                    int closeParen = value.LastIndexOf(")");
                    if (closeParen > comma)
                    {
                        string fallback = value.Substring(comma + 1, closeParen - comma - 1).Trim();
                        if (!string.IsNullOrEmpty(fallback))
                        {
                            return fallback;
                        }
                    }
                }
                // If we can't extract a fallback, return empty
                return string.Empty;
            }

            return value;
        }

        #endregion

        #region Public Properties

        public GdiFill PaintFill
        {
            get {
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
            if (strokeWidth.Equals(0))
                return null;

            GdiSvgPaint stroke;
            if (PaintType == SvgPaintType.None)
            {
                return null;
            }
            else if (PaintType == SvgPaintType.CurrentColor)
            {
                stroke = new GdiSvgPaint(_element, CssConstants.PropColor);
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
                    if (fDashArray[i].Equals(0))
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
            if (opacity.Length > 0) 
                alpha *= SvgNumber.ParseNumber(opacity);

            opacity = _element.GetPropertyValue("opacity");
            if (opacity.Length > 0) 
                alpha *= SvgNumber.ParseNumber(opacity);

            alpha = Math.Min(alpha, 255);
            alpha = Math.Max(alpha, 0);

            return Convert.ToInt32(alpha);
        }

        private float GetOpacityValue(string fillOrStroke)
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

            return (float)opacityValue;
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
            if (miterLimit < 1)
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "stroke-miterlimit can not be less then 1");

            return miterLimit;
        }

        private float[] GetDashArray(float strokeWidth)
        {
            string dashArray = _element.GetPropertyValue("stroke-dasharray");

            if (string.IsNullOrWhiteSpace(dashArray) 
                || dashArray.Equals(CssConstants.ValNone, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Handle CSS variables: resolve unresolved CSS variables and extract fallbacks
            // This prevents crashes when CSS variables are not properly resolved by the CSS engine.
            dashArray = dashArray.Trim();
            if (dashArray.IndexOf("var(", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Use CssVariableResolver to handle complex variable scenarios
                // Note: CssVariableResolver is in WPF namespace, so we replicate the logic here for GDI
                dashArray = ResolveCssVariablesForGdi(dashArray);

                if (string.IsNullOrWhiteSpace(dashArray))
                {
                    return null;
                }
            }

            try
            {
                SvgNumberList list = new SvgNumberList(dashArray);

                uint len = list.NumberOfItems;
                float[] fDashArray = new float[len];

                for (uint i = 0; i < len; i++)
                {
                    // divide by strokeWidth to take care of the difference between Svg and GDI+
                    fDashArray[i] = (float)(list.GetItem(i).Value / strokeWidth);
                }

                if (len % 2 == 1)
                {
                    // odd number of values, duplicate
                    float[] tmpArray = new float[len * 2];
                    fDashArray.CopyTo(tmpArray, 0);
                    fDashArray.CopyTo(tmpArray, (int)len);

                    fDashArray = tmpArray;
                }

                return fDashArray;
            }
            catch (Exception ex)
            {
                // If parsing fails for any reason (malformed values, etc.), return null as fallback
                // This prevents crashes and allows the SVG to render without the dash pattern
                System.Diagnostics.Debug.WriteLine($"Failed to parse stroke-dasharray value '{dashArray}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Resolves CSS variable references in GDI context (mirrors WPF CssVariableResolver logic).
        /// </summary>
        private static string ResolveCssVariablesForGdi(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = value.Trim();

            // If the value doesn't contain var(, it's a literal value
            if (value.IndexOf("var(", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return value;
            }

            // Try to extract a var() call and its fallback
            int varStart = value.IndexOf("var(", StringComparison.OrdinalIgnoreCase);
            if (varStart < 0)
            {
                return value;
            }

            // Find the matching closing paren
            int openParenPos = varStart + 3;
            int closeParenPos = FindMatchingCloseParenGdi(value, openParenPos);

            if (closeParenPos < 0)
            {
                // Malformed var()
                return value;
            }

            // Extract the content inside var(...) = e.g., "--my-color, fallback"
            string content = value.Substring(openParenPos + 1, closeParenPos - openParenPos - 1);

            // Split on first comma to separate variable name from fallback
            int commaPos = FindFirstTopLevelCommaGdi(content);
            if (commaPos >= 0)
            {
                // Has fallback: var(--name, fallback)
                string fallback = content.Substring(commaPos + 1).Trim();
                if (!string.IsNullOrEmpty(fallback))
                {
                    // Check if the fallback is itself a var() call (recursive)
                    if (fallback.IndexOf("var(", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Recursively resolve the fallback (limited depth)
                        return ResolveCssVariablesForGdi(fallback);
                    }
                    else
                    {
                        // Fallback is a literal value
                        return fallback;
                    }
                }
            }

            // No fallback or empty fallback: return empty
            return string.Empty;
        }

        /// <summary>
        /// Finds the position of the first top-level comma (not inside nested parens).
        /// </summary>
        private static int FindFirstTopLevelCommaGdi(string value)
        {
            int depth = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '(')
                {
                    depth++;
                }
                else if (value[i] == ')')
                {
                    depth--;
                }
                else if (value[i] == ',' && depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds the position of the closing parenthesis that matches an opening parenthesis.
        /// </summary>
        private static int FindMatchingCloseParenGdi(string value, int startPos)
        {
            if (startPos < 0 || startPos >= value.Length || value[startPos] != '(')
            {
                return -1;
            }

            int depth = 1;
            for (int i = startPos + 1; i < value.Length; i++)
            {
                if (value[i] == '(')
                {
                    depth++;
                }
                else if (value[i] == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private float GetDashOffset(float strokeWidth)
        {
            string dashOffset = _element.GetPropertyValue("stroke-dashoffset");
            if (dashOffset.Length > 0)
            {
                //divide by strokeWidth to take care of the difference between Svg and GDI+
                var dashOffsetLength = new SvgLength(_element, "stroke-dashoffset", SvgLengthDirection.Viewport, dashOffset);
                return (float)dashOffsetLength.Value;
            }
            return 0;
        }

        private GdiFill GetPaintFill(string uri)
        {
            string absoluteUri = _element.ResolveUri(uri);

            return GdiFill.CreateFill(_element.OwnerDocument, absoluteUri);
        }

        private Brush GetBrush(GraphicsPath gp, string propPrefix)
        {
            SvgPaintType curPaintType = this.PaintType;
            if (curPaintType == SvgPaintType.None)
            {
                return null;
            }
            
            SvgPaint painter = this;
            if (curPaintType == SvgPaintType.CurrentColor)
            {
                painter = new GdiSvgPaint(_element, CssConstants.PropColor);
            }

            SvgPaintType paintType = painter.PaintType;
            if (paintType == SvgPaintType.Uri || paintType == SvgPaintType.UriCurrentColor ||
                paintType == SvgPaintType.UriNone || paintType == SvgPaintType.UriRgbColor ||
                paintType == SvgPaintType.UriRgbColorIccColor)
            {
                _paintFill = GetPaintFill(painter.Uri);
                if (_paintFill != null)
                {
                    Brush br = _paintFill.GetBrush(gp.GetBounds(), this.GetOpacityValue(propPrefix));

                    if (_paintFill.FillType == GdiFillType.Pattern)
                    {
                        return br;
                    }
                    if (_paintFill.FillType == GdiFillType.LinearGradient)
                    {
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
                    }
                    if (_paintFill.FillType == GdiFillType.RadialGradient)
                    {
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
                        painter = new GdiSvgPaint(_element, CssConstants.PropColor);
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

            var brush = new SolidBrush(GdiConverter.ToColor(painter.RgbColor));
            int opacity = GetOpacity(propPrefix);
            brush.Color = Color.FromArgb(opacity, brush.Color);
            return brush;
        }

        #endregion
    }
}
