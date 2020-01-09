using System;
using System.Xml;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public enum GdiTextMode
    {
        Rendering,
        Measuring,
        Outlining
    }

    public sealed class GdiTextRendering : GdiRendering
    {
        #region Private Constants

        private const string Whitespace = " ";

        private readonly static Regex _tabNewline = new Regex(@"[\n\f\t]", RegexOptions.Compiled);
        private readonly static Regex _decimalNumber = new Regex(@"^\d", RegexOptions.Compiled);
        private static readonly Regex _multipleSpaces = new Regex(@" {2,}", RegexOptions.Compiled);

        private static readonly Regex _regExCaps = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Private Fields

        private string _actualFontName;
        private GraphicsPath _graphicsPath;
        private GdiTextMode _textMode;
        private GdiGraphics _graphics;

        #endregion

        #region Constructor and Destructor

        public GdiTextRendering(SvgElement element)
            : base(element)
        {
            _textMode = GdiTextMode.Rendering;
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive
        {
            get {
                return true;
            }
        }

        public GdiTextMode TextMode
        {
            get {
                return _textMode;
            }
            set {
                _textMode = value;
                if (_textMode == GdiTextMode.Outlining)
                {
                    if (_graphicsPath == null)
                    {
                        _graphicsPath = new GraphicsPath();
                    }
                }
            }
        }

        public GraphicsPath Path
        {
            get {
                return _graphicsPath;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(GdiGraphicsRenderer renderer)
        {
            if (_uniqueColor.IsEmpty && _textMode != GdiTextMode.Outlining)
            {
                _uniqueColor = renderer.GetNextHitColor(_svgElement);
            }

            var graphics = renderer.GdiGraphics;

            _graphicsContainer = graphics.BeginContainer();
            SetQuality(graphics);
            SetTransform(graphics);
        }

        public override void Render(GdiGraphicsRenderer renderer)
        {
            _graphics = renderer.GdiGraphics;

            //SvgRenderingHint hint = _svgElement.RenderingHint;
            //if (hint == SvgRenderingHint.Clipping)
            //{
            //    return;
            //}
            //if (_svgElement.ParentNode is SvgClipPathElement)
            //{
            //    return;
            //}

            SvgTextBaseElement textElement = _svgElement as SvgTextBaseElement;
            if (textElement == null)
            {
                return;
            }

            string sVisibility = textElement.GetPropertyValue("visibility");
            string sDisplay = textElement.GetPropertyValue("display");
            if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            {
                return;
            }

            if (_textMode != GdiTextMode.Outlining)
            {
                SetClip(_graphics);
            }

            PointF ctp = new PointF(0, 0); // current text position

            ctp = GetCurrentTextPosition(textElement, ctp);
            string sBaselineShift = textElement.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                float textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%", StringComparison.OrdinalIgnoreCase))
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

            // For for fonts loading in the background...
            var svgDoc = _svgElement.OwnerDocument;
            if (svgDoc.IsFontsLoaded == false)
            {
                //TODO: Use of SpinUntil is known to CPU heavy, but will work for now...
                SpinWait.SpinUntil(() => svgDoc.IsFontsLoaded == true);
            }

            XmlNodeType nodeType = XmlNodeType.None;
            foreach (XmlNode child in _svgElement.ChildNodes)
            {
                SvgStyleableElement stylable = child as SvgStyleableElement;
                if (stylable != null)
                {
                    sVisibility = stylable.GetPropertyValue("visibility");
                    sDisplay = stylable.GetPropertyValue("display");
                    if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
                    {
                        continue;
                    }
                }

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
                    if (string.Equals(nodeName, "tref"))
                    {
                        AddTRefElementPath((SvgTRefElement)child, ref ctp);
                    }
                    else if (string.Equals(nodeName, "tspan"))
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
            GdiSvgPaint paint = new GdiSvgPaint(_svgElement as SvgStyleableElement, "fill");
            return paint.GetBrush(gp);
        }

        private Pen GetPen(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(_svgElement as SvgStyleableElement, "stroke");
            return paint.GetPen(gp);
        }

        #region Private Text Methods

        private string TrimText(SvgTextContentElement element, string val)
        {
            if (element.XmlSpace != "preserve")
                val = val.Replace("\n", string.Empty);
            val = _tabNewline.Replace(val, " ");

            //if (element.XmlSpace == "preserve" || element.XmlSpace == "default")
            if (element.XmlSpace == "preserve")
            {
                return val;
            }
            if (element.XmlSpace == "default")
            {
                return _multipleSpaces.Replace(val, " ");
                //return val;
            }
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
            FontFamily family = GetFontFamily(element);
            int style         = GetFontStyle(element);
            StringFormat sf   = GetStringFormat(element);

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

            if (_textMode == GdiTextMode.Outlining)
            {
                _graphicsPath.AddPath(textGeometry, false);
                return;
            }

            GdiSvgPaint fillPaint = new GdiSvgPaint(element, "fill");
            Brush brush = fillPaint.GetBrush(textGeometry);

            GdiSvgPaint strokePaint = new GdiSvgPaint(element, "stroke");
            Pen pen = strokePaint.GetPen(textGeometry);

            if (brush != null)
            {
                if (brush is PathGradientBrush)
                {
                    var gps = fillPaint.PaintFill as GdiRadialGradientFill;

                    _graphics.SetClip(gps.GetRadialRegion(textGeometry.GetBounds()), CombineMode.Exclude);

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
                    var gps = strokePaint.PaintFill as GdiRadialGradientFill;
                    GdiGraphicsContainer container = _graphics.BeginContainer();

                    _graphics.SetClip(gps.GetRadialRegion(textGeometry.GetBounds()), CombineMode.Exclude);

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

        private string GetTRefText(SvgTRefElement element)
        {
            XmlElement refElement = element.ReferencedElement;
            if (refElement != null)
            {
                return TrimText(element, refElement.InnerText);
            }
            return string.Empty;
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
                SvgTextBaseElement textElement = (SvgTextBaseElement)element.SelectSingleNode("ancestor::svg:text",
                    element.OwnerDocument.NamespaceManager);

                float textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%", StringComparison.OrdinalIgnoreCase))
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

        private int GetFontStyle(SvgTextContentElement element)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;

            int style = (int)FontStyle.Regular;
            string fontWeight = element.GetPropertyValue("font-weight");
            if (fontWeight == "bold" || fontWeight == "bolder" || fontWeight == "600" || fontWeight == "700" || fontWeight == "800" || fontWeight == "900")
            {
                style = style | (int)FontStyle.Bold;
            }

            if (string.Equals(element.GetPropertyValue("font-style"), "italic", comparer))
            {
                style = style | (int)FontStyle.Italic;
            }

            string textDeco = element.GetPropertyValue("text-decoration");
            if (string.Equals(textDeco, "line-through", comparer))
            {
                style = style | (int)FontStyle.Strikeout;
            }
            else if (string.Equals(textDeco, "underline", comparer))
            {
                style = style | (int)FontStyle.Underline;
            }
            return style;
        }

        //private FontFamily GetFontFamily0(SvgTextContentElement element)
        //{
        //    var comparer = StringComparison.OrdinalIgnoreCase;

        //    string fontFamily  = element.GetPropertyValue("font-family");
        //    string[] fontNames = fontFamily.Split(new char[1] { ',' });

        //    FontFamily family;

        //    foreach (string fn in fontNames)
        //    {
        //        try
        //        {
        //            string fontName = fn.Trim(new char[] { ' ', '\'', '"' });

        //            if (string.Equals(fontName, "serif", comparer))
        //                family = FontFamily.GenericSerif;
        //            else if (fontName == "sans-serif")
        //                family = FontFamily.GenericSansSerif;
        //            else if (fontName == "monospace")
        //                family = FontFamily.GenericMonospace;
        //            else
        //                family = new FontFamily(fontName);		// Font(,fontSize).FontFamily;	

        //            return family;
        //        }
        //        catch (Exception ex)
        //        {
        //            Trace.TraceError(ex.ToString());
        //        }
        //    }

        //    // no known font-family was found => default to arial
        //    return new FontFamily("Arial");
        //}

        private FontFamily GetFontFamily(SvgTextContentElement element)
        {
            string fontFamily  = element.GetPropertyValue("font-family");
            string[] fontNames = fontFamily.Split(new char[1] { ',' });

            var systemFontFamilies = FontFamily.Families;
            FontFamily family;

            var comparer = StringComparison.OrdinalIgnoreCase;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[] { ' ', '\'', '"' });

                    if (string.Equals(fontName, "serif", comparer))
                    {
                        family = GdiRenderingSettings.GenericSerif;
                    }
                    else if (string.Equals(fontName, "sans-serif", comparer)
                        || string.Equals(fontName, "sans serif", comparer))
                    {
                        family = GdiRenderingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", comparer))
                    {
                        family = GdiRenderingSettings.GenericMonospace;
                    }
                    else
                    {
                        var funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Name, fontName, comparer));
                        family = systemFontFamilies.FirstOrDefault(funcFamily);
                    }
                    if (family != null)
                    {
                        return family;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            // No known font-family was found => default to "Arial"
            return GdiRenderingSettings.DefaultFontFamily;
        }

        private GdiFontFamilyInfo GetTextFontFamilyInfo(SvgTextContentElement element)
        {
            _actualFontName = null;

            string fontFamily = element.GetPropertyValue("font-family");
            string[] fontNames = fontNames = fontFamily.Split(new char[1] { ',' });

            GdiFontStyles fontStyle = GetTextFontStyle(element);
            GdiFontWeights fontWeight = GetTextFontWeight(element);
            GdiFontStretches fontStretch = GetTextFontStretch(element);

            var comparer = StringComparison.OrdinalIgnoreCase;

            var docElement = element.OwnerDocument;

            ISet<string> svgFontFamilies = docElement.SvgFontFamilies;
            IDictionary<string, string> styledFontIds = docElement.StyledFontIds;

            IList<string> svgFontNames = null;
            if (svgFontFamilies != null && svgFontFamilies.Count != 0)
            {
                svgFontNames = new List<string>();
            }
            var systemFontFamilies = FontFamily.Families;

            FontFamily family = null;
            // using separate pointer to give less priority to generic font names
            FontFamily genericFamily = null;

            GdiFontFamilyType familyType = GdiFontFamilyType.None;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[] { ' ', '\'', '"' });
                    if (svgFontFamilies != null && svgFontFamilies.Count != 0)
                    {
                        if (svgFontFamilies.Contains(fontName))
                        {
                            svgFontNames.Add(fontName);
                            continue;
                        }
                        if (styledFontIds.ContainsKey(fontName))
                        {
                            string mappedFontName = styledFontIds[fontName];
                            if (svgFontFamilies.Contains(mappedFontName))
                            {
                                svgFontNames.Add(mappedFontName);
                                continue;
                            }
                        }
                    }

                    if (string.Equals(fontName, "serif", comparer))
                    {
                        genericFamily = GdiRenderingSettings.GenericSerif;
                    }
                    else if (string.Equals(fontName, "sans-serif", comparer)
                        || string.Equals(fontName, "sans serif", comparer))
                    {
                        genericFamily = GdiRenderingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", comparer))
                    {
                        genericFamily = GdiRenderingSettings.GenericMonospace;
                    }
                    else if (styledFontIds.ContainsKey(fontName))
                    {
                        string mappedFontName = styledFontIds[fontName];
                        var funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Name, mappedFontName, comparer));
                        family = systemFontFamilies.FirstOrDefault(funcFamily);
                        if (family != null)
                        {
                            _actualFontName = mappedFontName;
                            familyType = GdiFontFamilyType.System;
                        }
                    }
                    else
                    {
                        string normalizedFontName;
                        var funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Name, fontName, comparer));
                        family = systemFontFamilies.FirstOrDefault(funcFamily);
                        if (family != null)
                        {
                            _actualFontName = fontName;
                            familyType = GdiFontFamilyType.System;
                        }
                        else if (fontName.IndexOf('-') > 0)
                        {
                            normalizedFontName = fontName.Replace("-", " ");
                            funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Name,
                                normalizedFontName, comparer));
                            family = systemFontFamilies.FirstOrDefault(funcFamily);
                            if (family != null)
                            {
                                _actualFontName = normalizedFontName;
                                familyType = GdiFontFamilyType.System;
                            }
                        }
                        else if (SplitByCaps(fontName, out normalizedFontName))
                        {
                            funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Name,
                               normalizedFontName, comparer));
                            family = systemFontFamilies.FirstOrDefault(funcFamily);
                            if (family != null)
                            {
                                _actualFontName = normalizedFontName;
                                familyType = GdiFontFamilyType.System;
                            }
                        }
                    }

                    if (family != null)
                    {
                        return new GdiFontFamilyInfo(familyType, _actualFontName, family,
                            fontWeight, fontStyle, fontStretch);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            //// If set, use the SVG-Font...Not Ready Yet!!!
            //if (svgFontNames != null && svgFontNames.Count != 0)
            //{
            //    IList<SvgFontElement> svgFonts = docElement.GetFonts(svgFontNames);
            //    if (svgFonts != null && svgFonts.Count != 0)
            //    {
            //        string fontVariant = element.GetPropertyValue("font-variant");

            //        // For a single match...
            //        if (svgFonts.Count == 1)
            //        {
            //            var fontFamilyInfo = new GdiFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
            //                fontWeight, fontStyle, fontStretch);

            //            fontFamilyInfo.Variant = fontVariant;
            //            return fontFamilyInfo;
            //        }

            //        // For multiple matches, we will test the variants...
            //        if (string.IsNullOrWhiteSpace(fontVariant))
            //        {
            //            // Not found, return the first match...
            //            var fontFamilyInfo = new GdiFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
            //                fontWeight, fontStyle, fontStretch);

            //            fontFamilyInfo.Variant = fontVariant;
            //            return fontFamilyInfo;
            //        }

            //        foreach (var svgFont in svgFonts)
            //        {
            //            var fontFace = svgFont.FontFace;
            //            if (fontFace == null)
            //            {
            //                continue;
            //            }
            //            if (fontVariant.Equals(fontFace.FontVariant, comparer))
            //            {
            //                var fontFamilyInfo = new GdiFontFamilyInfo(svgFont.FontFamily, svgFont,
            //                    fontWeight, fontStyle, fontStretch);

            //                fontFamilyInfo.Variant = fontVariant;
            //                return fontFamilyInfo;
            //            }
            //        }

            //        // If the variant is not found, return the first match...
            //        {
            //            var fontFamilyInfo = new GdiFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
            //                fontWeight, fontStyle, fontStretch);

            //            fontFamilyInfo.Variant = fontVariant;
            //            return fontFamilyInfo;
            //        }
            //    }
            //}

            if (genericFamily != null)
            {
                return new GdiFontFamilyInfo(GdiFontFamilyType.Generic, _actualFontName, genericFamily,
                    fontWeight, fontStyle, fontStretch);
            }

            // No known font-family was found => default to "Arial"
            return new GdiFontFamilyInfo(familyType, _actualFontName,
                GdiRenderingSettings.DefaultFontFamily, fontWeight, fontStyle, fontStretch);
        }

        private StringFormat GetStringFormat(SvgTextContentElement element)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;

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
                if (string.Equals(anchor, "middle", comparer))
                    sf.Alignment = StringAlignment.Center;
                if (string.Equals(anchor, "end", comparer))
                    sf.Alignment = StringAlignment.Far;
            }

            string dir = element.GetPropertyValue("direction");
            if (string.Equals(dir, "rtl", comparer))
            {
                if (sf.Alignment == StringAlignment.Far)
                    sf.Alignment = StringAlignment.Near;
                else if (sf.Alignment == StringAlignment.Near)
                    sf.Alignment = StringAlignment.Far;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            }

            dir = element.GetPropertyValue("writing-mode");
            if (string.Equals(dir, "tb", comparer))
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
            if (_decimalNumber.IsMatch(str))
            {
                // svg length
                var fontLength = new SvgLength(element, "font-size", SvgLengthDirection.Viewport, str, "10px");
                fontSize = (float)fontLength.Value;
            }

            return fontSize;
        }

        private static bool SplitByCaps(string input, out string output)
        {
            output = input;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            output = _regExCaps.Replace(input, " ");

            return output.Length > input.Length;
        }

        #endregion

        #region FontWeight Methods

        private GdiFontWeights GetTextFontWeight(SvgTextContentElement element)
        {
            string fontWeight = element.GetPropertyValue("font-weight");
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return GdiFontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return GdiFontWeights.Normal;
                case "bold":
                    return GdiFontWeights.Bold;
                case "100":
                    return GdiFontWeights.Thin;
                case "200":
                    return GdiFontWeights.ExtraLight;
                case "300":
                    return GdiFontWeights.Light;
                case "400":
                    return GdiFontWeights.Normal;
                case "500":
                    return GdiFontWeights.Medium;
                case "600":
                    return GdiFontWeights.SemiBold;
                case "700":
                    return GdiFontWeights.Bold;
                case "800":
                    return GdiFontWeights.ExtraBold;
                case "900":
                    return GdiFontWeights.Black;
                case "950":
                    return GdiFontWeights.UltraBlack;
            }

            if (string.Equals(fontWeight, "bolder", StringComparison.OrdinalIgnoreCase))
            {
                SvgTransformableElement parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!string.IsNullOrWhiteSpace(fontWeight))
                    {
                        return this.GetBolderFontWeight(fontWeight);
                    }
                }
                return GdiFontWeights.ExtraBold;
            }
            if (string.Equals(fontWeight, "lighter", StringComparison.OrdinalIgnoreCase))
            {
                SvgTransformableElement parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!string.IsNullOrWhiteSpace(fontWeight))
                    {
                        return this.GetLighterFontWeight(fontWeight);
                    }
                }
                return GdiFontWeights.Light;
            }

            return GdiFontWeights.Normal;
        }

        private GdiFontWeights GetBolderFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return GdiFontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return GdiFontWeights.Bold;
                case "bold":
                    return GdiFontWeights.ExtraBold;
                case "100":
                    return GdiFontWeights.ExtraLight;
                case "200":
                    return GdiFontWeights.Light;
                case "300":
                    return GdiFontWeights.Normal;
                case "400":
                    return GdiFontWeights.Bold;
                case "500":
                    return GdiFontWeights.SemiBold;
                case "600":
                    return GdiFontWeights.Bold;
                case "700":
                    return GdiFontWeights.ExtraBold;
                case "800":
                    return GdiFontWeights.Black;
                case "900":
                    return GdiFontWeights.UltraBlack;
                case "950":
                    return GdiFontWeights.UltraBlack;
            }

            return GdiFontWeights.Normal;
        }

        private GdiFontWeights GetLighterFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return GdiFontWeights.Normal;
            }

            switch (fontWeight)
            {
                case "normal":
                    return GdiFontWeights.Light;
                case "bold":
                    return GdiFontWeights.Normal;

                case "100":
                    return GdiFontWeights.Thin;
                case "200":
                    return GdiFontWeights.Thin;
                case "300":
                    return GdiFontWeights.ExtraLight;
                case "400":
                    return GdiFontWeights.Light;
                case "500":
                    return GdiFontWeights.Normal;
                case "600":
                    return GdiFontWeights.Medium;
                case "700":
                    return GdiFontWeights.SemiBold;
                case "800":
                    return GdiFontWeights.Bold;
                case "900":
                    return GdiFontWeights.ExtraBold;
                case "950":
                    return GdiFontWeights.Black;
            }

            return GdiFontWeights.Normal;
        }

        #endregion

        #region FontStyle/Stretch Methods

        private GdiFontStyles GetTextFontStyle(SvgTextContentElement element)
        {
            string fontStyle = element.GetPropertyValue("font-style");
            if (string.IsNullOrWhiteSpace(fontStyle))
            {
                return GdiFontStyles.Normal;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            if (string.Equals(fontStyle, "normal", comparer))
            {
                return GdiFontStyles.Normal;
            }
            if (string.Equals(fontStyle, "italic", comparer))
            {
                return GdiFontStyles.Italic;
            }
            if (string.Equals(fontStyle, "oblique", comparer))
            {
                return GdiFontStyles.Oblique;
            }

            return GdiFontStyles.Normal;
        }

        private GdiFontStretches GetTextFontStretch(SvgTextContentElement element)
        {
            string fontStretch = element.GetPropertyValue("font-stretch");
            if (string.IsNullOrWhiteSpace(fontStretch))
            {
                return GdiFontStretches.Normal;
            }

            switch (fontStretch)
            {
                case "normal":
                    return GdiFontStretches.Normal;
                case "ultra-condensed":
                    return GdiFontStretches.UltraCondensed;
                case "extra-condensed":
                    return GdiFontStretches.ExtraCondensed;
                case "condensed":
                    return GdiFontStretches.Condensed;
                case "semi-condensed":
                    return GdiFontStretches.SemiCondensed;
                case "semi-expanded":
                    return GdiFontStretches.SemiExpanded;
                case "expanded":
                    return GdiFontStretches.Expanded;
                case "extra-expanded":
                    return GdiFontStretches.ExtraExpanded;
                case "ultra-expanded":
                    return GdiFontStretches.UltraExpanded;
            }

            return GdiFontStretches.Normal;
        }

        #endregion

        #endregion
    }
}
