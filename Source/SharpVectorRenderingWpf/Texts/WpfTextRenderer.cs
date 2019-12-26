using System;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public abstract class WpfTextRenderer : WpfRendererObject
    {
        #region Protected Fields

        protected const string Whitespace            = " ";
        protected const char NonBreakingChar         = '\u00A0';
        protected readonly static string NonBreaking = char.ConvertFromUtf32(NonBreakingChar);

        protected readonly static Regex _tabNewline     = new Regex(@"[\n\f\t]", RegexOptions.Compiled);
        protected readonly static Regex _decimalNumber  = new Regex(@"^\d", RegexOptions.Compiled);
        protected static readonly Regex _multipleSpaces = new Regex(@" {2,}", RegexOptions.Compiled);

        protected string _actualFontName;

        protected DrawingContext _drawContext;
        protected SvgTextBaseElement _textElement;

        protected WpfTextRendering _textRendering;

        #endregion

        #region Private Fields

        private static object _fontSynch = new object();
        private static IDictionary<string, FontFamily> _systemFonts;

        #endregion

        #region Constructors and Destructor

        protected WpfTextRenderer(SvgTextBaseElement textElement, WpfTextRendering textRendering)
        {
            if (textElement == null)
            {
                throw new ArgumentNullException(nameof(textElement),
                    "The SVG text element is required, and cannot be null (or Nothing).");
            }
            if (textRendering == null)
            {
                throw new ArgumentNullException(nameof(textRendering),
                    "The text rendering object is required, and cannot be null (or Nothing).");
            }

            _textElement   = textElement;
            _textRendering = textRendering;
        }

        #endregion

        #region Public Properties

        public override bool IsInitialized
        {
            get {
                return (_drawContext != null && _context != null);
            }
        }

        public DrawingContext DrawContext
        {
            get {
                return _drawContext;
            }
        }

        public SvgTextBaseElement TextElement
        {
            get {
                return _textElement;
            }
        }

        #endregion

        #region Protected Properties

        protected bool IsMeasuring
        {
            get {
                if (_textRendering != null)
                {
                    return _textRendering.IsMeasuring;
                }

                return false;
            }
        }

        protected bool IsTextPath
        {
            get {
                if (_textRendering != null)
                {
                    return _textRendering.IsTextPath;
                }

                return false;
            }
            set {
                if (_textRendering != null)
                {
                    _textRendering.IsTextPath = value;
                }
            }
        }

        protected double TextWidth
        {
            get {
                if (_textRendering != null)
                {
                    return _textRendering.TextWidth;
                }

                return 0;
            }
        }

        protected CultureInfo TextCulture
        {
            get {
                if (_textRendering != null)
                {
                    var textContext = _textRendering.TextContext;
                    if (textContext != null)
                    {
                        return textContext.Culture;
                    }
                }
                return null;
            }
        }

        protected WpfTextContext TextContext
        {
            get {
                if (_textRendering != null)
                {
                    return _textRendering.TextContext;
                }
                return null;
            }
        }

        #endregion

        #region Public Methods

        public virtual void SetElement(SvgTextBaseElement textElement)
        {
            _drawContext = null;
            _context     = null;
            _textElement = textElement;
        }

        public virtual void Initialize(DrawingContext textContext, WpfDrawingContext drawContext)
        {
            if (textContext == null)
            {
                throw new ArgumentNullException(nameof(textContext),
                    "The text context is required, and cannot be null (or Nothing).");
            }
            if (drawContext == null)
            {
                throw new ArgumentNullException(nameof(drawContext),
                    "The drawing context is required, and cannot be null (or Nothing).");
            }

            _drawContext = textContext;
            _context     = drawContext;
        }

        public virtual void Uninitialize()
        {
            _drawContext = null;
            _context     = null;
        }

        public abstract void RenderText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement);

        public abstract void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement);

        #region TRef/TSpan Methods

        public static string TrimText(SvgTextContentElement element, string val)
        {
            if (element.XmlSpace != "preserve")
                val = val.Replace("\n", string.Empty);
            val = _tabNewline.Replace(val, " ");

            var textTransform = element.GetPropertyValue("text-transform");
            if (!string.IsNullOrWhiteSpace(textTransform))
            {
                switch (textTransform)
                {
                    case "capitalize":
                        val = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(val);
                        break;
                    case "uppercase":
                        val = val.ToUpper(CultureInfo.CurrentCulture);
                        break;
                    case "lowercase":
                        val = val.ToLower(CultureInfo.CurrentCulture);
                        break;
                    case "full-width":
                    case "full-size-kana":
                    case "none":
                    default:
                        break;
                }
            }

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

        public static string GetText(SvgTextContentElement element, XmlNode child, XmlNode spaceNode = null)
        {
            if (spaceNode != null)
            {
                return TrimText(element, child.Value + spaceNode.Value);
            }
            return TrimText(element, child.Value);
        }

        public static string GetText(SvgTRefElement element)
        {
            XmlElement refElement = element.ReferencedElement;
            if (refElement != null)
            {
                return TrimText(element, refElement.InnerText);
            }
            return string.Empty;
        }

        public static string GetText(SvgAltGlyphElement element)
        {
            XmlElement refElement = element.ReferencedElement;
            if (refElement != null)
            {
                return TrimText(element, element.InnerText);
            }
            return string.Empty;
        }

        #endregion

        #region TextPosition/Size Methods

        public static double GetComputedFontSize(SvgTextContentElement element)
        {
            string str = element.GetPropertyValue("font-size");
            double fontSize = 12;
            if (_decimalNumber.IsMatch(str))
            {
                // svg length
                var fontLength = new SvgLength(element, "font-size", SvgLengthDirection.Viewport, str, "10px");
                fontSize = fontLength.Value;
            }

            return fontSize;
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Helper Methods

        protected void SetTextWidth(double textWidth)
        {
            if (_textRendering != null && !textWidth.Equals(0))
            {
                _textRendering.SetTextWidth(textWidth);
            }
        }

        protected void AddTextWidth(Point location, double textWidth)
        {
            if (_textRendering != null && !textWidth.Equals(0))
            {
                var textContext = _textRendering.TextContext;
                if (textContext != null)
                {
                    textContext.AddTextSize(location, textWidth);
                }
                _textRendering.AddTextWidth(textWidth);
            }
        }

        protected Brush GetBrush()
        {
            WpfSvgPaint paint = new WpfSvgPaint(_context, _textElement, "fill");

            return paint.GetBrush();
        }

        protected Pen GetPen()
        {
            WpfSvgPaint paint = new WpfSvgPaint(_context, _textElement, "stroke");

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

        protected FontWeight GetTextFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
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

            return FontWeights.Normal;
        }

        protected FontWeight GetTextFontWeight(SvgTextContentElement element)
        {
            string fontWeight = element.GetPropertyValue("font-weight");
            if (string.IsNullOrWhiteSpace(fontWeight))
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
                return FontWeights.ExtraBold;
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
                return FontWeights.Light;
            }

            return FontWeights.Normal;
        }

        protected FontWeight GetBolderFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
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
            if (string.IsNullOrWhiteSpace(fontWeight))
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
            return this.GetTextFontStyle(element.GetPropertyValue("font-style"));
        }

        protected FontStyle GetTextFontStyle(string fontStyle)
        {
            if (string.IsNullOrWhiteSpace(fontStyle))
            {
                return FontStyles.Normal;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            if (string.Equals(fontStyle, "normal", comparer))
            {
                return FontStyles.Normal;
            }
            if (string.Equals(fontStyle, "italic", comparer))
            {
                return FontStyles.Italic;
            }
            if (string.Equals(fontStyle, "oblique", comparer))
            {
                return FontStyles.Oblique;
            }

            return FontStyles.Normal;
        }

        protected FontStretch GetTextFontStretch(SvgTextContentElement element)
        {
            string fontStretch = element.GetPropertyValue("font-stretch");
            if (string.IsNullOrWhiteSpace(fontStretch))
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
            var comparer = StringComparison.OrdinalIgnoreCase;

            string textDeco = element.GetPropertyValue("text-decoration");
            if (string.IsNullOrWhiteSpace(textDeco))
            {
                return null;
            }
            if (string.Equals(textDeco, "line-through", comparer))
            {
                return TextDecorations.Strikethrough;
            }
            if (string.Equals(textDeco, "underline", comparer))
            {
                return TextDecorations.Underline;
            }
            if (string.Equals(textDeco, "overline", comparer))
            {
                return TextDecorations.OverLine;
            }

            return null;
        }

        protected WpfFontFamilyInfo GetTextFontFamilyInfo(SvgTextContentElement element)
        {
            _actualFontName = null;

            string fontFamily  = element.GetPropertyValue("font-family");
            string[] fontNames = fontFamily.Split(new char[1] { ',' });

            FontStyle fontStyle     = GetTextFontStyle(element);
            FontWeight fontWeight   = GetTextFontWeight(element);
            FontStretch fontStretch = GetTextFontStretch(element);

            var comparer   = StringComparison.OrdinalIgnoreCase;
            var docElement = element.OwnerDocument;

            ISet<string> svgFontFamilies = docElement.SvgFontFamilies;
            IDictionary<string, string> styledFontIds = docElement.StyledFontIds;

            IList<string> svgFontNames   = null;
            if (svgFontFamilies != null && svgFontFamilies.Count != 0)
            {
                svgFontNames = new List<string>();
            }
            var wpfSettings = _context.Settings;
            var fontFamilyNames = wpfSettings.FontFamilyNames;
            var privateFontFamilies = wpfSettings.HasFontFamilies;

            var docFontFamilies = docElement.FontFamilies;
            if (docFontFamilies != null && docFontFamilies.Count != 0)
            {
                foreach (var docFontFamily in docFontFamilies)
                {
                    if (!docFontFamily.IsLoaded)
                    {
                        wpfSettings.AddFontLocation(docFontFamily.FontUri);
                        docFontFamily.IsLoaded = true;
                    }
                }
            }

            FontFamily selectedFamily = null;
            // using separate pointer to give less priority to generic font names
            FontFamily genericFamily = null; 

            WpfFontFamilyType familyType = WpfFontFamilyType.None;

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
                        genericFamily = WpfDrawingSettings.GenericSerif;
                    }
                    else if (string.Equals(fontName, "sans-serif", comparer)
                        || string.Equals(fontName, "sans serif", comparer))
                    {
                        genericFamily = WpfDrawingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", comparer))
                    {
                        genericFamily = WpfDrawingSettings.GenericMonospace;
                    }
                    else if (styledFontIds.ContainsKey(fontName))
                    {
                        string mappedFontName = styledFontIds[fontName];
                        selectedFamily = LookupFontFamily(mappedFontName, fontFamilyNames);
                        if (selectedFamily != null)
                        {
                            _actualFontName = mappedFontName;
                            familyType = WpfFontFamilyType.System;
                        }
                    }
                    else
                    {
                        // Try looking up fonts in the system font registry...
                        selectedFamily = LookupFontFamily(fontName, fontFamilyNames);
                        if (selectedFamily != null)
                        {
                            _actualFontName = fontName;
                            familyType = WpfFontFamilyType.System;
                        }

                        // If not found, look through private fonts if available..
                        if (selectedFamily == null && privateFontFamilies)
                        {
                            selectedFamily = wpfSettings.LookupFontFamily(fontName);
                            if (selectedFamily != null)
                            {
                                _actualFontName = fontName;
                                familyType = WpfFontFamilyType.Private;
                            }
                        }
                    }

                    if (selectedFamily != null)
                    {
                        return new WpfFontFamilyInfo(familyType, _actualFontName, selectedFamily, 
                            fontWeight, fontStyle, fontStretch);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            // If set, use the SVG-Font...
            if (svgFontNames != null && svgFontNames.Count != 0)
            {
                FontFamily altFamily = (genericFamily != null) ? genericFamily : WpfDrawingSettings.DefaultFontFamily;

                IList<SvgFontElement> svgFonts = docElement.GetFonts(svgFontNames);
                if (svgFonts != null && svgFonts.Count != 0)
                {
                    string fontVariant = element.GetPropertyValue("font-variant");

                    // For a single match...
                    if (svgFonts.Count == 1)
                    {
                        var fontFamilyInfo = new WpfFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
                            fontWeight, fontStyle, fontStretch);

                        fontFamilyInfo.Variant = fontVariant;
                        // For rendering that do not support the SVG Fonts...
                        fontFamilyInfo.Family = altFamily;
                        return fontFamilyInfo;
                    }

                    // For the defined font style...
                    if (fontStyle != FontStyles.Normal)
                    {
                        // Then it is either oblique or italic
                        SvgFontElement closeFont = null;
                        SvgFontElement closestFont = null;
                        bool isItalic = fontStyle.Equals(FontStyles.Italic);
                        foreach (var svgFont in svgFonts)
                        {
                            var fontFace = svgFont.FontFace;
                            if (fontFace == null)
                            {
                                continue;
                            }
                            var typefaceStyle = GetTextFontStyle(fontFace.FontStyle);
                            if (fontStyle.Equals(typefaceStyle))
                            {
                                closeFont = svgFont;
                                if (closestFont == null)
                                {
                                    closestFont = svgFont;
                                }
                                var typefaceWeight = GetTextFontWeight(fontFace.FontWeight);
                                if (fontVariant.Equals(fontFace.FontVariant, comparer))
                                {
                                    closestFont = svgFont;
                                    if (fontWeight.Equals(typefaceWeight))
                                    {
                                        var fontFamilyInfo = new WpfFontFamilyInfo(svgFont.FontFamily, svgFont,
                                        fontWeight, fontStyle, fontStretch);

                                        fontFamilyInfo.Variant = fontVariant;
                                        // For rendering that do not support the SVG Fonts...
                                        fontFamilyInfo.Family = altFamily;
                                        return fontFamilyInfo;
                                    }
                                }
                            }
                            if (closeFont == null)
                            {
                                if (isItalic && typefaceStyle == FontStyles.Oblique)
                                {
                                    closeFont = svgFont;
                                }
                                if (!isItalic && typefaceStyle == FontStyles.Italic)
                                {
                                    closeFont = svgFont;
                                }
                            }
                        }
                        if (closestFont != null)
                        {
                            closeFont = closestFont;
                        }

                        if (closeFont != null)
                        {
                            var fontFamilyInfo = new WpfFontFamilyInfo(closeFont.FontFamily, closeFont,
                                fontWeight, fontStyle, fontStretch);

                            fontFamilyInfo.Variant = fontVariant;
                            // For rendering that do not support the SVG Fonts...
                            fontFamilyInfo.Family = altFamily;
                            return fontFamilyInfo;
                        }
                    }

                    SvgFontElement variantFont = null;
                    // For multiple matches, we will test the variants...
                    if (!string.IsNullOrWhiteSpace(fontVariant))
                    {
                        foreach (var svgFont in svgFonts)
                        {
                            var fontFace = svgFont.FontFace;
                            if (fontFace == null)
                            {
                                continue;
                            }
                            if (fontVariant.Equals(fontFace.FontVariant, comparer))
                            {
                                variantFont = svgFont;
                                // Check for more perfect match...
                                var typefaceWeight = GetTextFontWeight(fontFace.FontWeight);
                                var typefaceStyle = GetTextFontStyle(fontFace.FontStyle);
                                if (fontStyle.Equals(typefaceStyle) && fontWeight.Equals(typefaceWeight))
                                {
                                    var fontFamilyInfo = new WpfFontFamilyInfo(svgFont.FontFamily, svgFont,
                                        fontWeight, fontStyle, fontStretch);

                                    fontFamilyInfo.Variant = fontVariant;
                                    // For rendering that do not support the SVG Fonts...
                                    fontFamilyInfo.Family = altFamily;
                                    return fontFamilyInfo;
                                }
                            }
                        }

                        //if (variantFont != null)
                        //{
                        //    // If there was a matching variant but either style or weight not matched...
                        //    var fontFamilyInfo = new WpfFontFamilyInfo(variantFont.FontFamily, variantFont,
                        //        fontWeight, fontStyle, fontStretch);

                        //    fontFamilyInfo.Variant = fontVariant;
                        //    // For rendering that do not support the SVG Fonts...
                        //    fontFamilyInfo.Family = altFamily;
                        //    return fontFamilyInfo;
                        //}
                    }

                    // For the defined font weights...
                    if (fontWeight != FontWeights.Normal && fontWeight != FontWeights.Regular)
                    {
                        int weightValue   = fontWeight.ToOpenTypeWeight();
                        int selectedValue = int.MaxValue;
                        SvgFontElement sameWeightFont = null;
                        SvgFontElement closestFont    = null;
                        foreach (var svgFont in svgFonts)
                        {
                            var fontFace = svgFont.FontFace;
                            if (fontFace == null)
                            {
                                continue;
                            }
                            var typefaceWeight = GetTextFontWeight(fontFace.FontWeight);
                            if (fontWeight.Equals(typefaceWeight))
                            {
                                sameWeightFont = svgFont;
                                var typefaceStyle  = GetTextFontStyle(fontFace.FontStyle);
                                if (fontStyle.Equals(typefaceStyle))
                                {
                                    var fontFamilyInfo = new WpfFontFamilyInfo(svgFont.FontFamily, svgFont,
                                        fontWeight, fontStyle, fontStretch);

                                    fontFamilyInfo.Variant = fontVariant;
                                    // For rendering that do not support the SVG Fonts...
                                    fontFamilyInfo.Family = altFamily;
                                    return fontFamilyInfo;
                                }
                            }

                            int weightDiff = Math.Abs(weightValue - typefaceWeight.ToOpenTypeWeight());
                            if (weightDiff < selectedValue)
                            {
                                closestFont  = svgFont;
                                selectedValue = weightDiff;
                            }
                        }

                        // If the weights matched, but not the style
                        if (sameWeightFont != null)
                        {
                            var fontFamilyInfo = new WpfFontFamilyInfo(sameWeightFont.FontFamily, sameWeightFont,
                                fontWeight, fontStyle, fontStretch);

                            fontFamilyInfo.Variant = fontVariant;
                            // For rendering that do not support the SVG Fonts...
                            fontFamilyInfo.Family = altFamily;
                            return fontFamilyInfo;
                        }
                        if (closestFont != null)
                        {
                            var fontFamilyInfo = new WpfFontFamilyInfo(closestFont.FontFamily, closestFont,
                                fontWeight, fontStyle, fontStretch);

                            fontFamilyInfo.Variant = fontVariant;
                            // For rendering that do not support the SVG Fonts...
                            fontFamilyInfo.Family = altFamily;
                            return fontFamilyInfo;
                        }
                    }

                    if (variantFont != null)
                    {
                        // If there was a matching variant but either style or weight not matched...
                        var fontFamilyInfo = new WpfFontFamilyInfo(variantFont.FontFamily, variantFont,
                            fontWeight, fontStyle, fontStretch);

                        fontFamilyInfo.Variant = fontVariant;
                        // For rendering that do not support the SVG Fonts...
                        fontFamilyInfo.Family = altFamily;
                        return fontFamilyInfo;
                    }                    
                    else // If the variant is not found, return the first match...
                    {
                        var fontFamilyInfo = new WpfFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
                            fontWeight, fontStyle, fontStretch);

                        fontFamilyInfo.Variant = fontVariant;
                        // For rendering that do not support the SVG Fonts...
                        fontFamilyInfo.Family = altFamily;
                        return fontFamilyInfo;
                    }

                    //// For multiple matches, we will test the variants...
                    //if (string.IsNullOrWhiteSpace(fontVariant))
                    //{
                    //    // Not found, return the first match...
                    //    var fontFamilyInfo = new WpfFontFamilyInfo(svgFonts[0].FontFamily, svgFonts[0],
                    //        fontWeight, fontStyle, fontStretch);

                    //    fontFamilyInfo.Variant = fontVariant;
                    //    // For rendering that do not support the SVG Fonts...
                    //    fontFamilyInfo.Family = altFamily;
                    //    return fontFamilyInfo;
                    //}

                }
            }

            if (genericFamily != null)
            {
                return new WpfFontFamilyInfo(WpfFontFamilyType.Generic, _actualFontName, genericFamily,
                    fontWeight, fontStyle, fontStretch);
            }

            // No known font-family was found => default to "Arial"
            return new WpfFontFamilyInfo(familyType, _actualFontName, 
                WpfDrawingSettings.DefaultFontFamily, fontWeight, fontStyle, fontStretch);
        }

        protected FontFamily GetTextFontFamily(SvgTextContentElement element)
        {
            _actualFontName = null;

            string fontFamily  = element.GetPropertyValue("font-family");
            string[] fontNames = fontFamily.Split(new char[1] { ',' });

            var systemFontFamilies = Fonts.SystemFontFamilies;
            FontFamily family;

            var comparer = StringComparison.OrdinalIgnoreCase;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[] { ' ', '\'', '"' });

                    if (string.Equals(fontName, "serif", comparer))
                    {
                        family = WpfDrawingSettings.GenericSerif;
                    }
                    else if (string.Equals(fontName, "sans-serif", comparer))
                    {
                        family = WpfDrawingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", comparer))
                    {
                        family = WpfDrawingSettings.GenericMonospace;
                    }
                    else
                    {
                        var funcFamily = new Func<FontFamily, bool>(ff => string.Equals(ff.Source, fontName, comparer));
                        family = systemFontFamilies.FirstOrDefault(funcFamily);
                        if (family != null)
                        {
                            _actualFontName = fontName;
                        }
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

            // No known font-family was found => default to "Arial Unicode MS"
            return WpfDrawingSettings.DefaultFontFamily;
        }

        protected WpfTextStringFormat GetTextStringFormat(SvgTextContentElement element)
        {
            WpfTextStringFormat sf = WpfTextStringFormat.Default;

            bool doAlign = true;
            var elemName = element.LocalName;
//            if (element is SvgTSpanElement || element is SvgTRefElement)
            if (string.Equals(elemName, "tspan", StringComparison.Ordinal)
                || string.Equals(elemName, "tref", StringComparison.Ordinal))
            {
                var posElement = (SvgTextPositioningElement)element;
                if (posElement.X.AnimVal.NumberOfItems == 0)
                    doAlign = false;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            string dir = element.GetPropertyValue("direction");
            bool isRightToLeft = string.Equals(dir, "rtl", comparer);
            sf.Direction = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            if (doAlign)
            {
                string anchor = element.GetPropertyValue("text-anchor");

                if (isRightToLeft)
                {
                    if (string.Equals(anchor, "middle", comparer))
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (string.Equals(anchor, "end", comparer))
                        sf.Anchor = WpfTextAnchor.Start;
                    else
                        sf.Anchor = WpfTextAnchor.End;
                }
                else
                {
                    if (string.Equals(anchor, "middle", comparer))
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (string.Equals(anchor, "end", comparer))
                        sf.Anchor = WpfTextAnchor.End;
                }
            }
            else
            {
                SvgTextBaseElement textElement = element.ParentNode as SvgTextBaseElement;
                if (textElement != null)
                {
                    string anchor = textElement.GetPropertyValue("text-anchor");
                    if (isRightToLeft)
                    {
                        if (string.Equals(anchor, "middle", comparer))
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (string.Equals(anchor, "end", comparer))
                            sf.Anchor = WpfTextAnchor.Start;
                        else
                            sf.Anchor = WpfTextAnchor.End;
                    }
                    else
                    {
                        if (string.Equals(anchor, "middle", comparer))
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (string.Equals(anchor, "end", comparer))
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

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private static FontFamily LookupFontFamily(string fontName, IDictionary<string, string> fontFamilyNames)
        {
            lock (_fontSynch)
            {
                if (string.IsNullOrWhiteSpace(fontName))
                {
                    return null;
                }

                if (_systemFonts == null || _systemFonts.Count == 0)
                {
                    BuildSystemFonts();
                }

                if (_systemFonts.ContainsKey(fontName))
                {
                    return _systemFonts[fontName];
                }

                if (fontFamilyNames != null && fontFamilyNames.Count != 0)
                {
                    if (fontFamilyNames.ContainsKey(fontName))
                    {
                        var internalName = fontFamilyNames[fontName];

                        if (_systemFonts.ContainsKey(internalName))
                        {
                            return _systemFonts[internalName];
                        }
                    }
                }

                string normalizedName = null;
                if (fontName.IndexOf('-') > 0)
                {
                    normalizedName = fontName.Replace("-", " ");
                    if (_systemFonts.ContainsKey(normalizedName))
                    {
                        return _systemFonts[normalizedName];
                    }
                }

                if (SplitByCaps(fontName, out normalizedName))
                {
                    normalizedName = fontName.Replace("-", " ");
                    if (_systemFonts.ContainsKey(normalizedName))
                    {
                        return _systemFonts[normalizedName];
                    }
                }

                return null;
            }
        }

        private static void BuildSystemFonts()
        {
            if (_systemFonts == null)
            {
                _systemFonts = new Dictionary<string, FontFamily>(StringComparer.OrdinalIgnoreCase);
            }
            if (_systemFonts.Count != 0)
            {
                return;
            }
            var fontFamilies = Fonts.SystemFontFamilies;
            foreach (var fontFamily in fontFamilies)
            {
                var fontName = fontFamily.Source;
                var hashIndex = fontName.IndexOf('#');
                if (hashIndex > 0)
                {
                    fontName = fontName.Substring(hashIndex + 1);
                }
                _systemFonts.Add(fontName, fontFamily);

                var fontNames = fontFamily.FamilyNames;
                if (fontNames != null && fontNames.Count != 0)
                {
                    foreach (var value in fontNames.Values)
                    {
                        if (!_systemFonts.ContainsKey(value))
                        {
                            _systemFonts.Add(value, fontFamily);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
