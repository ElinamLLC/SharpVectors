using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiTextRendering : GdiRendering
    {
        #region Private Fields

        private GdiGraphicsWrapper _graphics;

        #endregion

        #region Constructor and Destructor

        public GdiTextRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(GdiGraphicsRenderer renderer)
        {
            if (_uniqueColor.IsEmpty)
                _uniqueColor = renderer.GetNextColor(element);

            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;

            _graphicsContainer = graphics.BeginContainer();
            SetQuality(graphics);
            Transform(graphics);
        }

        public override void Render(GdiGraphicsRenderer renderer)
        {
            _graphics = renderer.GraphicsWrapper;

            SvgRenderingHint hint = element.RenderingHint;
            if (hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            if (element.ParentNode is SvgClipPathElement)
            {
                return;
            }

            SvgTextElement textElement = element as SvgTextElement;
            if (textElement == null)
            {
                return;
            }

            string sVisibility = textElement.GetPropertyValue("visibility");
            string sDisplay    = textElement.GetPropertyValue("display");
            if (String.Equals(sVisibility, "hidden") || String.Equals(sDisplay, "none"))
            {
                return;
            }

            Clip(_graphics);

            PointF ctp = new PointF(0, 0); // current text position
            
            ctp = GetCurrentTextPosition(textElement, ctp);
            string sBaselineShift = textElement.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                float textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%"))
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100 * textFontSize;
                }
                else if (sBaselineShift == "sub")
                {
                    shiftBy = -0.6F * textFontSize;
                }
                else if (sBaselineShift == "super")
                {
                    shiftBy = 0.6F * textFontSize;
                }
                else if (sBaselineShift == "baseline")
                {
                    shiftBy = 0;
                }
                else
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
                }
            }

            XmlNodeType nodeType = XmlNodeType.None;
            foreach (XmlNode child in element.ChildNodes)
            {
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text)
                {
                    ctp.Y -= (float)shiftBy;
                    AddGraphicsPath(textElement, ref ctp, GetText(textElement, child));
                    ctp.Y += (float)shiftBy;
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    string nodeName = child.Name;
                    if (String.Equals(nodeName, "tref"))
                    {
                        AddTRefElementPath((SvgTRefElement)child, ref ctp);
                    }
                    else if (String.Equals(nodeName, "tspan"))
                    {
                        AddTSpanElementPath((SvgTSpanElement)child, ref ctp);
                    }
                }
            }

            PaintMarkers(renderer, textElement, _graphics);

            _graphics = null;
        }

        #endregion

        #region Private Methods

        private Brush GetBrush(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(element as SvgStyleableElement, "fill");
            return paint.GetBrush(gp);
        }

        private Pen GetPen(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(element as SvgStyleableElement, "stroke");
            return paint.GetPen(gp);
        }

        #region Private Text Methods

        private string TrimText(SvgTextContentElement element, string val)
        {
            Regex tabNewline = new Regex(@"[\n\f\t]");
            if (element.XmlSpace != "preserve")
                val = val.Replace("\n", String.Empty);
            val = tabNewline.Replace(val, " ");

            if (element.XmlSpace == "preserve")
                return val;
            else
                return val.Trim();
        }

        private string GetText(SvgTextContentElement element, XmlNode child)
        {
            return TrimText(element, child.Value);
        }

        private void AddGraphicsPath(SvgTextContentElement element, ref PointF ctp, string text)
        {
            if (text.Length == 0)
                return;

            float emSize      = GetComputedFontSize(element);
            FontFamily family = GetGDIFontFamily(element, emSize);
            int style         = GetGDIFontStyle(element);
            StringFormat sf   = GetGDIStringFormat(element);

            GraphicsPath textGeometry = new GraphicsPath();

            float xCorrection = 0;
            if (sf.Alignment == StringAlignment.Near) 
                xCorrection = emSize * 1 / 6;
            else if (sf.Alignment == StringAlignment.Far) 
                xCorrection = -emSize * 1 / 6;

            float yCorrection = (float)(family.GetCellAscent(FontStyle.Regular)) / (float)(family.GetEmHeight(FontStyle.Regular)) * emSize;

            // TODO: font property
            PointF p = new PointF(ctp.X - xCorrection, ctp.Y - yCorrection);

            textGeometry.AddString(text, family, style, emSize, p, sf);
            if (!textGeometry.GetBounds().IsEmpty)
            {
                float bboxWidth = textGeometry.GetBounds().Width;
                if (sf.Alignment == StringAlignment.Center) 
                    bboxWidth /= 2;
                else if (sf.Alignment == StringAlignment.Far) 
                    bboxWidth = 0;

                ctp.X += bboxWidth + emSize / 4;
            }

            GdiSvgPaint fillPaint = new GdiSvgPaint(element, "fill");
            Brush brush = fillPaint.GetBrush(textGeometry);

            GdiSvgPaint strokePaint = new GdiSvgPaint(element, "stroke");
            Pen pen = strokePaint.GetPen(textGeometry);

            if (brush != null)
            {
                if (brush is PathGradientBrush)
                {
                    GdiGradientFill gps = fillPaint.PaintFill as GdiGradientFill;

                    _graphics.SetClip(gps.GetRadialGradientRegion(textGeometry.GetBounds()), CombineMode.Exclude);

                    SolidBrush tempBrush = new SolidBrush(((PathGradientBrush)brush).InterpolationColors.Colors[0]);
                    _graphics.FillPath(this, tempBrush, textGeometry);
                    tempBrush.Dispose();
                    _graphics.ResetClip();
                }

                _graphics.FillPath(this, brush, textGeometry);
                brush.Dispose();
            }

            if (pen != null)
            {
                if (pen.Brush is PathGradientBrush)
                {
                    GdiGradientFill gps = strokePaint.PaintFill as GdiGradientFill;
                    GdiGraphicsContainer container = _graphics.BeginContainer();

                    _graphics.SetClip(gps.GetRadialGradientRegion(textGeometry.GetBounds()), CombineMode.Exclude);

                    SolidBrush tempBrush = new SolidBrush(((PathGradientBrush)pen.Brush).InterpolationColors.Colors[0]);
                    Pen tempPen = new Pen(tempBrush, pen.Width);
                    _graphics.DrawPath(this, tempPen, textGeometry);
                    tempPen.Dispose();
                    tempBrush.Dispose();

                    _graphics.EndContainer(container);
                }

                _graphics.DrawPath(this, pen, textGeometry);
                pen.Dispose();
            }

            textGeometry.Dispose();
        }

        public string GetTRefText(SvgTRefElement element)
        {
            XmlElement refElement = element.ReferencedElement;
            if (refElement != null)
            {
                return TrimText(element, refElement.InnerText);
            }
            else
            {
                return String.Empty;
            }
        }

        private void AddTRefElementPath(SvgTRefElement element, ref PointF ctp)
        {
            ctp = GetCurrentTextPosition(element, ctp);

            this.AddGraphicsPath(element, ref ctp, GetTRefText(element));
        }

        private void AddTSpanElementPath(SvgTSpanElement element, ref PointF ctp)
        {
            ctp = GetCurrentTextPosition(element, ctp);
            string sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                SvgTextElement textElement = (SvgTextElement)element.SelectSingleNode("ancestor::svg:text",
                    element.OwnerDocument.NamespaceManager);

                float textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%"))
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0, 
                        sBaselineShift.Length - 1)) / 100 * textFontSize;
                }
                else if (sBaselineShift == "sub")
                {
                    shiftBy = -0.6F * textFontSize;
                }
                else if (sBaselineShift == "super")
                {
                    shiftBy = 0.6F * textFontSize;
                }
                else if (sBaselineShift == "baseline")
                {
                    shiftBy = 0;
                }
                else
                {
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
                }
            }

            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    ctp.Y -= (float)shiftBy;
                    AddGraphicsPath(element, ref ctp, GetText(element, child));
                    ctp.Y += (float)shiftBy;
                }
            }
        }

        private PointF GetCurrentTextPosition(SvgTextPositioningElement posElement, PointF p)
        {
            if (posElement.X.AnimVal.NumberOfItems > 0)
            {
                p.X = (float)posElement.X.AnimVal.GetItem(0).Value;
            }
            if (posElement.Y.AnimVal.NumberOfItems > 0)
            {
                p.Y = (float)posElement.Y.AnimVal.GetItem(0).Value;
            }
            if (posElement.Dx.AnimVal.NumberOfItems > 0)
            {
                p.X += (float)posElement.Dx.AnimVal.GetItem(0).Value;
            }
            if (posElement.Dy.AnimVal.NumberOfItems > 0)
            {
                p.Y += (float)posElement.Dy.AnimVal.GetItem(0).Value;
            }
            return p;
        }

        private int GetGDIFontStyle(SvgTextContentElement element)
        {
            int style = (int)FontStyle.Regular;
            string fontWeight = element.GetPropertyValue("font-weight");
            if (fontWeight == "bold" || fontWeight == "bolder" || fontWeight == "600" || fontWeight == "700" || fontWeight == "800" || fontWeight == "900")
            {
                style = style | (int)FontStyle.Bold;
            }

            if (element.GetPropertyValue("font-style") == "italic")
            {
                style = style | (int)FontStyle.Italic;
            }

            string textDeco = element.GetPropertyValue("text-decoration");
            if (textDeco == "line-through")
            {
                style = style | (int)FontStyle.Strikeout;
            }
            else if (textDeco == "underline")
            {
                style = style | (int)FontStyle.Underline;
            }
            return style;
        }

        private FontFamily GetGDIFontFamily(SvgTextContentElement element, float fontSize)
        {
            string fontFamily  = element.GetPropertyValue("font-family");
            string[] fontNames = fontNames = fontFamily.Split(new char[1] { ',' });

            FontFamily family;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[] { ' ', '\'', '"' });

                    if (fontName == "serif") 
                        family = FontFamily.GenericSerif;
                    else if (fontName == "sans-serif") 
                        family = FontFamily.GenericSansSerif;
                    else if (fontName == "monospace") 
                        family = FontFamily.GenericMonospace;
                    else 
                        family = new FontFamily(fontName);		// Font(,fontSize).FontFamily;	

                    return family;
                }
                catch
                {
                }
            }

            // no known font-family was found => default to arial
            return new FontFamily("Arial");
        }

        private StringFormat GetGDIStringFormat(SvgTextContentElement element)
        {
            StringFormat sf = new StringFormat();

            bool doAlign = true;
            if (element is SvgTSpanElement || element is SvgTRefElement)
            {
                SvgTextPositioningElement posElement = (SvgTextPositioningElement)element;
                if (posElement.X.AnimVal.NumberOfItems == 0) doAlign = false;
            }

            if (doAlign)
            {
                string anchor = element.GetPropertyValue("text-anchor");
                if (anchor == "middle") 
                    sf.Alignment = StringAlignment.Center;
                if (anchor == "end") 
                    sf.Alignment = StringAlignment.Far;
            }

            string dir = element.GetPropertyValue("direction");
            if (dir == "rtl")
            {
                if (sf.Alignment == StringAlignment.Far) 
                    sf.Alignment = StringAlignment.Near;
                else if (sf.Alignment == StringAlignment.Near) 
                    sf.Alignment = StringAlignment.Far;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            }

            dir = element.GetPropertyValue("writing-mode");
            if (dir == "tb")
            {
                sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionVertical;
            }

            sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            return sf;
        }

        private float GetComputedFontSize(SvgTextContentElement element)
        {
            string str = element.GetPropertyValue("font-size");
            float fontSize = 12;
            if (str.EndsWith("%"))
            {
                // percentage of inherited value
            }
            else if (new Regex(@"^\d").IsMatch(str))
            {
                // svg length
                fontSize = (float)new SvgLength(element, "font-size", 
                    SvgLengthDirection.Viewport, str, "10px").Value;
            }
            else if (str == "larger")
            {
            }
            else if (str == "smaller")
            {

            }
            else
            {
                // check for absolute value
            }

            return fontSize;
        }

        #endregion

        #endregion
    }
}
