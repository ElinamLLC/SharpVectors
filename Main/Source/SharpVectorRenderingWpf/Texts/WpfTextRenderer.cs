using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public abstract class WpfTextRenderer
    {
        #region Protected Fields

        protected const string Whitespace = " ";

        protected readonly static Regex _tabNewline = new Regex(@"[\n\f\t]");
        protected readonly static Regex _decimalNumber = new Regex(@"^\d");

        protected string _actualFontName;

        protected DrawingContext    _textContext;
        protected SvgTextElement    _textElement;

        protected WpfDrawingContext _drawContext;
        protected WpfTextRendering  _textRendering;

        #endregion

        #region Constructors and Destructor

        protected WpfTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
        {
            if (textElement == null)
            {
                throw new ArgumentNullException("textElement",
                    "The SVG text element is required, and cannot be null (or Nothing).");
            }
            if (textRendering == null)
            {
                throw new ArgumentNullException("textRendering",
                    "The text rendering object is required, and cannot be null (or Nothing).");
            }

            _textElement   = textElement;
            _textRendering = textRendering;
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return (_textContext != null && _drawContext != null);
            }
        }

        public DrawingContext TextContext
        {
            get
            {
                return _textContext;
            }
        }

        public SvgTextElement TextElement
        {
            get
            {
                return _textElement;
            }
        }

        public WpfDrawingContext DrawContext
        {
            get
            {
                return _drawContext;
            }
        }


        #endregion

        #region Protected Properties

        protected bool IsMeasuring
        {
            get
            {
                if (_textRendering != null)
                {
                    return _textRendering.IsMeasuring;
                }

                return false;
            }
        }

        protected bool IsTextPath
        {
            get
            {
                if (_textRendering != null)
                {
                    return _textRendering.IsTextPath;
                }

                return false;
            }
            set
            {
                if (_textRendering != null)
                {
                    _textRendering.IsTextPath = value;
                }
            }
        }

        protected double TextWidth
        {
            get
            {
                if (_textRendering != null)
                {
                    return _textRendering.TextWidth;
                }

                return 0;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(DrawingContext textContext, WpfDrawingContext drawContext)
        {
            if (textContext == null)
            {
                throw new ArgumentNullException("textContext", 
                    "The text context is required, and cannot be null (or Nothing).");
            }
            if (drawContext == null)
            {
                throw new ArgumentNullException("drawContext",
                    "The drawing context is required, and cannot be null (or Nothing).");
            }

            _textContext = textContext;
            _drawContext = drawContext;
        }

        public virtual void Uninitialize()
        {
            _textContext = null;
            _drawContext = null;
        }

        public abstract void RenderSingleLineText(SvgTextContentElement element,
            ref Point startPos, string text, double rotate, WpfTextPlacement placement);

        public abstract void RenderTextRun(SvgTextContentElement element,
            ref Point startPos, string text, double rotate, WpfTextPlacement placement);

        #region TRef/TSpan Methods

        public static string TrimText(SvgTextContentElement element, string val)
        {
            if (element.XmlSpace != "preserve")
                val = val.Replace("\n", String.Empty);
            val = _tabNewline.Replace(val, " ");

            if (element.XmlSpace == "preserve" || element.XmlSpace == "default")
            {
                return val;
            }
            else
            {
                return val.Trim();
            }
        }

        public static string GetText(SvgTextContentElement element, XmlNode child)
        {
            return TrimText(element, child.Value);
        }

        public static string GetTRefText(SvgTRefElement element)
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

        #endregion

        #region TextPosition/Size Methods

        public static WpfTextPlacement GetCurrentTextPosition(SvgTextPositioningElement posElement, Point p)
        {
            ISvgLengthList xValues  = posElement.X.AnimVal;
            ISvgLengthList yValues  = posElement.Y.AnimVal;
            ISvgLengthList dxValues = posElement.Dx.AnimVal;
            ISvgLengthList dyValues = posElement.Dy.AnimVal;
            ISvgNumberList rValues  = posElement.Rotate.AnimVal;

            bool requiresGlyphPositioning = false;
            bool isXYGlyphPositioning     = false;
            bool isDxyGlyphPositioning    = false;
            bool isRotateGlyphPositioning = false;

            double xValue  = p.X;
            double yValue  = p.Y;
            double rValue  = 0;
            double dxValue = 0;
            double dyValue = 0;

            WpfTextPlacement textPlacement = null;

            if (xValues.NumberOfItems > 0)
            {
                if (xValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning     = true;
                    requiresGlyphPositioning = true;
                }

                xValue = xValues.GetItem(0).Value;
                p.X = xValue;
            }
            if (yValues.NumberOfItems > 0)
            {
                if (yValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning     = true;
                    requiresGlyphPositioning = true;
                }

                yValue = yValues.GetItem(0).Value;
                p.Y = yValue;
            }
            if (dxValues.NumberOfItems > 0)
            {
                if (dxValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning    = true;
                    requiresGlyphPositioning = true;
                }

                dxValue = dxValues.GetItem(0).Value;
                p.X += dxValue;
            }
            if (dyValues.NumberOfItems > 0)
            {
                if (dyValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning    = true;
                    requiresGlyphPositioning = true;
                }

                dyValue = dyValues.GetItem(0).Value;
                p.Y += dyValue;
            }
            if (rValues.NumberOfItems > 0)
            {
                if (rValues.NumberOfItems > 1)
                {
                    isRotateGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                rValue = rValues.GetItem(0).Value;
            }

            if (requiresGlyphPositioning)
            {
                uint xCount  = xValues.NumberOfItems;
                uint yCount  = yValues.NumberOfItems;
                uint dxCount = dxValues.NumberOfItems;
                uint dyCount = dyValues.NumberOfItems;
                uint rCount  = rValues.NumberOfItems;

                List<WpfTextPosition> textPositions = null;

                bool isRotateOnly = false;

                if (isXYGlyphPositioning)
                {
                    uint itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount      = Math.Max(itemCount, rCount);
                    textPositions  = new List<WpfTextPosition>((int)itemCount);

                    double xLast = 0;
                    double yLast = 0;

                    for (uint i = 0; i < itemCount; i++)
                    {
                        double xNext  = i < xCount  ? xValues.GetItem(i).Value  : xValue;
                        double yNext  = i < yCount  ? yValues.GetItem(i).Value  : yValue;
                        double rNext  = i < rCount  ? rValues.GetItem(i).Value  : rValue;
                        double dxNext = i < dxCount ? dxValues.GetItem(i).Value : dxValue;
                        double dyNext = i < dyCount ? dyValues.GetItem(i).Value : dyValue;

                        if (i < xCount)
                        {
                            xLast = xNext;
                        }
                        else
                        {
                            xNext = xLast;
                        }
                        if (i < yCount)
                        {
                            yLast = yNext;
                        }
                        else
                        {
                            yNext = yLast;
                        }

                        WpfTextPosition textPosition = new WpfTextPosition(
                            new Point(xNext + dxNext, yNext + dyNext), rNext);

                        textPositions.Add(textPosition);
                    }                     
                }
                else if (isDxyGlyphPositioning)
                {   
                }
                else if (isRotateGlyphPositioning)
                {
                    isRotateOnly   = true;
                    uint itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount      = Math.Max(itemCount, rCount);
                    textPositions  = new List<WpfTextPosition>((int)itemCount);

                    for (uint i = 0; i < itemCount; i++)
                    {
                        double rNext  = i < rCount  ? rValues.GetItem(i).Value  : rValue;

                        WpfTextPosition textPosition = new WpfTextPosition(p, rNext);

                        textPositions.Add(textPosition);
                    }                     
                }

                if (textPositions != null && textPositions.Count != 0)
                {
                    textPlacement = new WpfTextPlacement(p, rValue, textPositions, isRotateOnly);
                }
                else
                {
                    textPlacement = new WpfTextPlacement(p, rValue);
                }
            }
            else
            {
                textPlacement = new WpfTextPlacement(p, rValue);
            }

            return textPlacement;
        }

        public static double GetComputedFontSize(SvgTextContentElement element)
        {
            string str = element.GetPropertyValue("font-size");
            double fontSize = 12;
            if (str.EndsWith("%"))
            {
                // percentage of inherited value
            }
            else if (_decimalNumber.IsMatch(str))
            {
                // svg length
                fontSize = new SvgLength(element, "font-size",
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

        #region Protected Methods

        #region Helper Methods

        protected void SetTextWidth(double textWidth)
        {
            if (_textRendering != null && textWidth != 0)
            {
                _textRendering.SetTextWidth(textWidth);
            }
        }

        protected void AddTextWidth(double textWidth)
        {
            if (_textRendering != null && textWidth != 0)
            {
                _textRendering.AddTextWidth(textWidth);
            }
        }

        protected Brush GetBrush()
        {
            WpfSvgPaint paint = new WpfSvgPaint(_drawContext, _textElement, "fill");

            return paint.GetBrush();
        }

        protected Pen GetPen()
        {
            WpfSvgPaint paint = new WpfSvgPaint(_drawContext, _textElement, "stroke");

            return paint.GetPen();
        }

        /// <summary>
        /// This will extract a <see cref="PathGeometry"/> that is nested into GeometryGroup, which
        /// is normally created by the FormattedText.BuildGeometry() method.
        /// </summary>
        /// <param name="sourceGeometry"></param>
        /// <returns></returns>
        protected static Geometry ExtractTextPathGeometry(Geometry sourceGeometry)
        {
            GeometryGroup outerGroup = sourceGeometry as GeometryGroup;
            if (outerGroup != null && outerGroup.Children.Count == 1)
            {
                GeometryGroup innerGroup = outerGroup.Children[0] as GeometryGroup;
                if (innerGroup != null && innerGroup.Children.Count == 1)
                {
                    return innerGroup.Children[0];
                }

                return innerGroup;
            }

            return sourceGeometry;
        }

        #endregion

        #region FontWeight Methods

        protected FontWeight GetTextFontWeight(SvgTextContentElement element)
        {
            string fontWeight = element.GetPropertyValue("font-weight");
            if (String.IsNullOrEmpty(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Normal;
                case "bold":
                    return FontWeights.Bold;
                case "100":
                    return FontWeights.Thin;
                case "200":
                    return FontWeights.ExtraLight;
                case "300":
                    return FontWeights.Light;
                case "400":
                    return FontWeights.Normal;
                case "500":
                    return FontWeights.Medium;
                case "600":
                    return FontWeights.SemiBold;
                case "700":
                    return FontWeights.Bold;
                case "800":
                    return FontWeights.ExtraBold;
                case "900":
                    return FontWeights.Black;
                case "950":
                    return FontWeights.UltraBlack;
            }

            if (String.Equals(fontWeight, "bolder", StringComparison.OrdinalIgnoreCase))
            {
                SvgTransformableElement parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!String.IsNullOrEmpty(fontWeight))
                    {
                        return this.GetBolderFontWeight(fontWeight);
                    }
                }
                return FontWeights.ExtraBold;
            }
            if (String.Equals(fontWeight, "lighter", StringComparison.OrdinalIgnoreCase))
            {
                SvgTransformableElement parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!String.IsNullOrEmpty(fontWeight))
                    {
                        return this.GetLighterFontWeight(fontWeight);
                    }
                }
                return FontWeights.Light;
            }

            return FontWeights.Normal;
        }

        protected FontWeight GetBolderFontWeight(string fontWeight)
        {
            if (String.IsNullOrEmpty(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Bold;
                case "bold":
                    return FontWeights.ExtraBold;
                case "100":
                    return FontWeights.ExtraLight;
                case "200":
                    return FontWeights.Light;
                case "300":
                    return FontWeights.Normal;
                case "400":
                    return FontWeights.Bold;
                case "500":
                    return FontWeights.SemiBold;
                case "600":
                    return FontWeights.Bold;
                case "700":
                    return FontWeights.ExtraBold;
                case "800":
                    return FontWeights.Black;
                case "900":
                    return FontWeights.UltraBlack;
                case "950":
                    return FontWeights.UltraBlack;
            }

            return FontWeights.Normal;
        }

        protected FontWeight GetLighterFontWeight(string fontWeight)
        {
            if (String.IsNullOrEmpty(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Light;
                case "bold":
                    return FontWeights.Normal;

                case "100":
                    return FontWeights.Thin;
                case "200":
                    return FontWeights.Thin;
                case "300":
                    return FontWeights.ExtraLight;
                case "400":
                    return FontWeights.Light;
                case "500":
                    return FontWeights.Normal;
                case "600":
                    return FontWeights.Medium;
                case "700":
                    return FontWeights.SemiBold;
                case "800":
                    return FontWeights.Bold;
                case "900":
                    return FontWeights.ExtraBold;
                case "950":
                    return FontWeights.Black;
            }

            return FontWeights.Normal;
        }

        #endregion

        #region FontStyle/Stretch Methods

        protected FontStyle GetTextFontStyle(SvgTextContentElement element)
        {
            string fontStyle = element.GetPropertyValue("font-style");
            if (String.IsNullOrEmpty(fontStyle))
            {
                return FontStyles.Normal;
            }

            if (fontStyle == "normal")
            {
                return FontStyles.Normal;
            }
            if (fontStyle == "italic")
            {
                return FontStyles.Italic;
            }
            if (fontStyle == "oblique")
            {
                return FontStyles.Oblique;
            }

            return FontStyles.Normal;
        }

        protected FontStretch GetTextFontStretch(SvgTextContentElement element)
        {
            string fontStretch = element.GetPropertyValue("font-stretch");
            if (String.IsNullOrEmpty(fontStretch))
            {
                return FontStretches.Normal;
            }

            switch (fontStretch)
            {
                case "normal":
                    return FontStretches.Normal;
                case "ultra-condensed":
                    return FontStretches.UltraCondensed;
                case "extra-condensed":
                    return FontStretches.ExtraCondensed;
                case "condensed":
                    return FontStretches.Condensed;
                case "semi-condensed":
                    return FontStretches.SemiCondensed;
                case "semi-expanded":
                    return FontStretches.SemiExpanded;
                case "expanded":
                    return FontStretches.Expanded;
                case "extra-expanded":
                    return FontStretches.ExtraExpanded;
                case "ultra-expanded":
                    return FontStretches.UltraExpanded;
            }

            return FontStretches.Normal;
        }

        #endregion

        #region Other Text/Font Attributes

        protected TextDecorationCollection GetTextDecoration(SvgTextContentElement element)
        {
            string textDeco = element.GetPropertyValue("text-decoration");
            if (textDeco == "line-through")
            {
                return TextDecorations.Strikethrough;
            }
            if (textDeco == "underline")
            {
                return TextDecorations.Underline;
            }
            if (textDeco == "overline")
            {
                return TextDecorations.OverLine;
            }

            return null;
        }

        protected FontFamily GetTextFontFamily(SvgTextContentElement element, double fontSize)
        {
            _actualFontName = null;

            string fontFamily = element.GetPropertyValue("font-family");
            string[] fontNames = fontNames = fontFamily.Split(new char[1] { ',' });

            FontFamily family;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[] { ' ', '\'', '"' });

                    if (String.Equals(fontName, "serif", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericSerif;
                    }
                    else if (String.Equals(fontName, "sans-serif", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericSansSerif;
                    }
                    else if (String.Equals(fontName, "monospace", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericMonospace;
                    }
                    else
                    {
                        family = new FontFamily(fontName);
                        _actualFontName = fontName;
                    }

                    return family;
                }
                catch
                {
                }
            }

            // no known font-family was found => default to Arial
            return WpfDrawingSettings.DefaultFontFamily;
        }

        protected WpfTextStringFormat GetTextStringFormat(SvgTextContentElement element)
        {
            WpfTextStringFormat sf = WpfTextStringFormat.Default;

            bool doAlign = true;
            if (element is SvgTSpanElement || element is SvgTRefElement)
            {
                SvgTextPositioningElement posElement = (SvgTextPositioningElement)element;
                if (posElement.X.AnimVal.NumberOfItems == 0)
                    doAlign = false;
            }

            string dir = element.GetPropertyValue("direction");
            bool isRightToLeft = (dir == "rtl");
            sf.Direction = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            if (doAlign)
            {
                string anchor = element.GetPropertyValue("text-anchor");

                if (isRightToLeft)
                {
                    if (anchor == "middle")
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (anchor == "end")
                        sf.Anchor = WpfTextAnchor.Start;
                    else
                        sf.Anchor = WpfTextAnchor.End;
                }
                else
                {
                    if (anchor == "middle")
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (anchor == "end")
                        sf.Anchor = WpfTextAnchor.End;
                }
            }
            else
            {
                SvgTextElement textElement = element.ParentNode as SvgTextElement;
                if (textElement != null)
                {
                    string anchor = textElement.GetPropertyValue("text-anchor");
                    if (isRightToLeft)
                    {
                        if (anchor == "middle")
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (anchor == "end")
                            sf.Anchor = WpfTextAnchor.Start;
                        else
                            sf.Anchor = WpfTextAnchor.End;
                    }
                    else
                    {
                        if (anchor == "middle")
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (anchor == "end")
                            sf.Anchor = WpfTextAnchor.End;
                    }
                }
            }

            //if (isRightToLeft)
            //{
            //    if (sf.Alignment == TextAlignment.Right)
            //        sf.Alignment = TextAlignment.Left;
            //    else if (sf.Alignment == TextAlignment.Left)
            //        sf.Alignment = TextAlignment.Right;

            //    //sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            //}

            //dir = element.GetPropertyValue("writing-mode");
            //if (dir == "tb")
            //{
            //    sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionVertical;
            //}

            //sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            return sf;
        }

        #endregion

        #endregion
    }
}
