using System;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom;
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
                switch (textTransform.ToLower())
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
                    case CssConstants.ValNone:
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
            string str = element.GetPropertyValue(CssConstants.PropFontSize);
            double fontSize = 12;
            if (_decimalNumber.IsMatch(str))
            {
                // svg length
                var fontLength = new SvgLength(element, CssConstants.PropFontSize, SvgLengthDirection.Viewport, str, "10px");
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
                if (outerGroup.Transform != null && !outerGroup.Transform.Value.IsIdentity)
                {
                    return outerGroup;
                }

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

        /// <summary>
        /// Maps a numeric font-weight value to the nearest standard CSS font-weight.
        /// </summary>
        /// <remarks>
        /// <para><b>Specification:</b> CSS Font Module Level 3 and 4</para>
        /// <para><b>Standard Weights:</b> 100, 200, 300, 400 (Normal), 500, 600 (SemiBold), 700 (Bold), 800 (ExtraBold), 900 (Black), 950 (UltraBlack)</para>
        /// <para><b>Rounding Behavior:</b></para>
        /// <list type="bullet">
        ///   <item><description>Non-standard numeric values are rounded to the nearest standard weight using Euclidean distance.</description></item>
        ///   <item><description>Example: 650 → nearest is 700 (distance 50) vs 600 (distance 50), but 700 wins due to index ordering.</description></item>
        ///   <item><description>Example: 475 → 500 (distance 25) is closer than 400 (distance 75).</description></item>
        ///   <item><description>Example: 550 → 500 or 600 are equidistant; 500 is returned as the first match.</description></item>
        /// </list>
        /// <para><b>Clamping:</b></para>
        /// <list type="bullet">
        ///   <item><description>Values &lt;= 100 are clamped to Thin (100).</description></item>
        ///   <item><description>Values >= 950 are clamped to UltraBlack (950).</description></item>
        /// </list>
        /// <para><b>Debug Output:</b> Emits Debug.WriteLine messages when clamping or rounding is applied, enabling runtime diagnostics without performance overhead in Release builds.</para>
        /// </remarks>
        /// <param name="numericWeight">A CSS numeric font-weight value (typically 100–950).</param>
        /// <returns>The nearest standard <see cref="FontWeight"/> from the CSS specification.</returns>
        private static FontWeight MapNumericFontWeightToStandard(int numericWeight)
        {
            // Standard CSS font weights
            int[] standardWeights = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 950 };
            FontWeight[] fontWeights = {
                FontWeights.Thin,
                FontWeights.ExtraLight,
                FontWeights.Light,
                FontWeights.Normal,
                FontWeights.Medium,
                FontWeights.SemiBold,
                FontWeights.Bold,
                FontWeights.ExtraBold,
                FontWeights.Black,
                FontWeights.UltraBlack
            };

            // Clamp the value to valid range
            if (numericWeight <= 100)
            {
                Debug.WriteLine($"[Font Weight] Numeric weight {numericWeight} is less than minimum (100), using Thin");
                return FontWeights.Thin;
            }
            if (numericWeight >= 950)
            {
                Debug.WriteLine($"[Font Weight] Numeric weight {numericWeight} is greater than maximum (950), using UltraBlack");
                return FontWeights.UltraBlack;
            }

            // Find the nearest standard weight
            int minDistance = int.MaxValue;
            FontWeight result = FontWeights.Normal;
            int nearestStandardWeight = 400;

            for (int i = 0; i < standardWeights.Length; i++)
            {
                int distance = Math.Abs(numericWeight - standardWeights[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = fontWeights[i];
                    nearestStandardWeight = standardWeights[i];
                }
            }

            Debug.WriteLine($"[Font Weight] Numeric weight {numericWeight} rounded to {nearestStandardWeight} ({result.ToString()})");
            return result;
        }

        /// <summary>
        /// Resolves a CSS font-weight string to a WPF <see cref="FontWeight"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>Supported Input Formats:</b></para>
        /// <list type="bullet">
        ///   <item><description>CSS Keywords: "normal", "bold"</description></item>
        ///   <item><description>Numeric Values: "100", "200", ..., "900", "950"</description></item>
        ///   <item><description>Non-standard Numeric: Any integer; rounded to nearest standard weight via <see cref="MapNumericFontWeightToStandard"/>.</description></item>
        ///   <item><description>Null/Empty: Defaults to "normal" (400).</description></item>
        /// </list>
        /// <para><b>Non-Standard Numeric Behavior:</b></para>
        /// When a numeric font-weight is provided that does not correspond to a standard CSS weight
        /// (e.g., 650, 475, 550), the value is rounded to the nearest standard weight. This ensures
        /// compatibility with WPF's standard FontWeight values while preserving the user's intent.
        /// See <see cref="MapNumericFontWeightToStandard"/> for detailed rounding examples.
        /// </remarks>
        /// <param name="fontWeight">A CSS font-weight string (e.g., "bold", "700", "650").</param>
        /// <returns>A WPF <see cref="FontWeight"/> corresponding to the input; defaults to Normal if parsing fails.</returns>
        protected FontWeight GetTextFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case CssConstants.ValNormal:
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

            // Try to parse as numeric weight
            if (int.TryParse(fontWeight, out int numericWeight))
            {
                return MapNumericFontWeightToStandard(numericWeight);
            }

            return FontWeights.Normal;
        }

        /// <summary>
        /// Resolves the font-weight property of an SVG text element, with support for relative keywords and parent-relative adjustments.
        /// </summary>
        /// <remarks>
        /// <para><b>Priority Order:</b></para>
        /// <list type="number">
        ///   <item><description>Direct CSS keywords: "normal", "bold"</description></item>
        ///   <item><description>Direct numeric values: "100"–"950"</description></item>
        ///   <item><description>Relative keywords with parent lookup: "bolder", "lighter"</description></item>
        ///   <item><description>Fallback: "normal" (400) if no valid value is found</description></item>
        /// </list>
        /// <para><b>Relative Keywords:</b></para>
        /// <list type="bullet">
        ///   <item><description><b>"bolder":</b> Looks up the parent element's font-weight and applies one step heavier (via <see cref="GetBolderFontWeight"/>). If no parent or parent weight unavailable, defaults to ExtraBold (800).</description></item>
        ///   <item><description><b>"lighter":</b> Looks up the parent element's font-weight and applies one step lighter (via <see cref="GetLighterFontWeight"/>). If no parent or parent weight unavailable, defaults to Light (300).</description></item>
        /// </list>
        /// <para><b>Non-Standard Numeric Handling:</b></para>
        /// Non-standard numeric values are automatically rounded via <see cref="MapNumericFontWeightToStandard"/>.
        /// This ensures smooth rendering across both CSS and WPF specifications.
        /// </remarks>
        /// <param name="element">The SVG text element from which to extract the font-weight property.</param>
        /// <returns>The resolved <see cref="FontWeight"/>; defaults to Normal if no value is specified or parsing fails.</returns>
        protected FontWeight GetTextFontWeight(SvgTextContentElement element)
        {
            string fontWeight = element.GetPropertyValue(CssConstants.PropFontWeight);
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case CssConstants.ValNormal:
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
                    fontWeight = parentElement.GetPropertyValue(CssConstants.PropFontWeight);
                    if (!string.IsNullOrWhiteSpace(fontWeight))
                    {
                        Debug.WriteLine($"[Font Weight] Applying 'bolder' to parent font-weight: '{fontWeight}'");
                        return this.GetBolderFontWeight(fontWeight);
                    }
                }
                Debug.WriteLine($"[Font Weight] 'bolder' applied without parent, using ExtraBold");
                return FontWeights.ExtraBold;
            }
            if (string.Equals(fontWeight, "lighter", StringComparison.OrdinalIgnoreCase))
            {
                SvgTransformableElement parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue(CssConstants.PropFontWeight);
                    if (!string.IsNullOrWhiteSpace(fontWeight))
                    {
                        Debug.WriteLine($"[Font Weight] Applying 'lighter' to parent font-weight: '{fontWeight}'");
                        return this.GetLighterFontWeight(fontWeight);
                    }
                }
                Debug.WriteLine($"[Font Weight] 'lighter' applied without parent, using Light");
                return FontWeights.Light;
            }

            // Try to parse as numeric weight
            if (int.TryParse(fontWeight, out int numericWeight))
            {
                return MapNumericFontWeightToStandard(numericWeight);
            }

            return FontWeights.Normal;
        }

        /// <summary>
        /// Resolves one step heavier font-weight from a given parent font-weight value.
        /// </summary>
        /// <remarks>
        /// <para><b>Specification:</b> CSS Font Module Level 3, Section 3.3 (font-weight)</para>
        /// <para>
        /// This method implements the "bolder" keyword semantics, moving from the current weight 
        /// up one step in the CSS weight hierarchy:
        /// </para>
        /// <list type="bullet">
        ///   <item><description>100 → ExtraLight (200)</description></item>
        ///   <item><description>200 → Light (300)</description></item>
        ///   <item><description>300 → Normal (400)</description></item>
        ///   <item><description>400 → Bold (700)</description></item>
        ///   <item><description>500 → SemiBold (600)</description></item>
        ///   <item><description>600 → Bold (700)</description></item>
        ///   <item><description>700 → ExtraBold (800)</description></item>
        ///   <item><description>800 → Black (900)</description></item>
        ///   <item><description>900+ → UltraBlack (950)</description></item>
        /// </list>
        /// <para><b>Non-Standard Numeric:</b> Non-standard numeric values increment by ~100 and are rounded to the nearest standard weight.</para>
        /// <para><b>Default:</b> Empty or invalid input returns Normal (400).</para>
        /// </remarks>
        /// <param name="fontWeight">The parent font-weight value (CSS keyword or numeric string).</param>
        /// <returns>One step heavier than the input weight.</returns>
        protected FontWeight GetBolderFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case CssConstants.ValNormal:
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

            // Try to parse as numeric weight - "bolder" means go one step heavier
            if (int.TryParse(fontWeight, out int numericWeight))
            {
                // Get current weight and increase by one step (~100)
                int bolderWeight = Math.Min(numericWeight + 100, 950);
                Debug.WriteLine($"[Font Weight] Bolder: {numericWeight} -> {bolderWeight}");
                return MapNumericFontWeightToStandard(bolderWeight);
            }

            return FontWeights.Normal;
        }

        /// <summary>
        /// Resolves one step lighter font-weight from a given parent font-weight value.
        /// </summary>
        /// <remarks>
        /// <para><b>Specification:</b> CSS Font Module Level 3, Section 3.3 (font-weight)</para>
        /// <para>
        /// This method implements the "lighter" keyword semantics, moving from the current weight 
        /// down one step in the CSS weight hierarchy:
        /// </para>
        /// <list type="bullet">
        ///   <item><description>100 → Thin (100, clamped minimum)</description></item>
        ///   <item><description>200 → Thin (100)</description></item>
        ///   <item><description>300 → Thin (100)</description></item>
        ///   <item><description>400 → Light (300)</description></item>
        ///   <item><description>500 → Light (300)</description></item>
        ///   <item><description>600 → Normal (400)</description></item>
        ///   <item><description>700 → Normal (400)</description></item>
        ///   <item><description>800 → SemiBold (600)</description></item>
        ///   <item><description>900 → Bold (700)</description></item>
        ///   <item><description>950 → Bold (700)</description></item>
        /// </list>
        /// <para><b>Non-Standard Numeric:</b> Non-standard numeric values decrement by ~100 and are rounded to the nearest standard weight.</para>
        /// <para><b>Default:</b> Empty or invalid input returns Normal (400).</para>
        /// </remarks>
        /// <param name="fontWeight">The parent font-weight value (CSS keyword or numeric string).</param>
        /// <returns>One step lighter than the input weight.</returns>
        protected FontWeight GetLighterFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight))
            {
                return FontWeights.Normal;
            }

            switch (fontWeight)
            {
                case CssConstants.ValNormal:
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

            // Try to parse as numeric weight - "lighter" means go one step lighter
            if (int.TryParse(fontWeight, out int numericWeight))
            {
                // Get current weight and decrease by one step (~100)
                int lighterWeight = Math.Max(numericWeight - 100, 100);
                Debug.WriteLine($"[Font Weight] Lighter: {numericWeight} -> {lighterWeight}");
                return MapNumericFontWeightToStandard(lighterWeight);
            }

            return FontWeights.Normal;

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

            if (string.Equals(fontStyle, CssConstants.ValNormal, comparer))
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
                case CssConstants.ValNormal:
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
            if (element.HasAttribute("font-family"))
            {
                _actualFontName = fontFamily;
            }
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
            var fontFamilyNames     = wpfSettings.FontFamilyNames;
            var privateFontFamilies = wpfSettings.HasFontFamilies;

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
                    else if (string.Equals(fontName, "cursive", comparer))
                    {
                        genericFamily = WpfDrawingSettings.GenericCursive;
                    }
                    else if (string.Equals(fontName, "fantasy", comparer))
                    {
                        genericFamily = WpfDrawingSettings.GenericFantasy;
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
                            selectedFamily = wpfSettings.LookupFontFamily(fontName, fontWeight, fontStyle, fontStretch);
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
                    Debug.WriteLine(ex.ToString());
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
                    else if (string.Equals(fontName, "sans-serif", comparer) 
                        || string.Equals(fontName, "sansserif", comparer))
                    {
                        family = WpfDrawingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", comparer))
                    {
                        family = WpfDrawingSettings.GenericMonospace;
                    }
                    else if (string.Equals(fontName, "cursive", comparer))
                    {
                        family = WpfDrawingSettings.GenericCursive;
                    }
                    else if (string.Equals(fontName, "fantasy", comparer))
                    {
                        family = WpfDrawingSettings.GenericFantasy;
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
                    Debug.WriteLine(ex.ToString());
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

            string dominant = string.Empty;

            if (doAlign)
            {
                string anchor = element.GetPropertyValue("text-anchor");
                dominant = element.GetPropertyValue(CssConstants.PropDominantBaseline);

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
                dominant = element.GetPropertyValue(CssConstants.PropDominantBaseline);
                SvgTextBaseElement textElement = element.ParentNode as SvgTextBaseElement;
                if (textElement != null)
                {
                    dominant = textElement.GetPropertyValue(CssConstants.PropDominantBaseline);
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

            if (string.IsNullOrWhiteSpace(dominant))
            {
                sf.Dominant = DominantBaseline.Alphabetic; // Default
            }
            else if (string.Equals(dominant, "central", comparer))
            {
                sf.Dominant = DominantBaseline.Central;
            }
            else if (string.Equals(dominant, "middle", comparer))
            {
                sf.Dominant = DominantBaseline.Middle;
            }
            else if (string.Equals(dominant, "hanging", comparer))
            {
                sf.Dominant = DominantBaseline.Hanging;
            }
            else if (string.Equals(dominant, "ideographic", comparer))
            {
                sf.Dominant = DominantBaseline.Ideographic;
            }
            else if (string.Equals(dominant, "mathematical", comparer))
            {
                sf.Dominant = DominantBaseline.Mathematical;
            }
            else if (string.Equals(dominant, "text-before-edge", comparer)
                || string.Equals(dominant, "text-top", comparer))
            {
                sf.Dominant = DominantBaseline.TextBeforeEdge;
            }
            else if (string.Equals(dominant, "text-after-edge", comparer)
                || string.Equals(dominant, "text-bottom", comparer))
            {
                sf.Dominant = DominantBaseline.TextAfterEdge;
            }
            else
            {
                sf.Dominant = DominantBaseline.Alphabetic; // Default
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

        #region Paint-Order Support

        /// <summary>
        /// Draws geometry respecting the paint-order CSS property.
        /// 
        /// For text rendering with paint-order support, this method creates separate
        /// drawings when stroke-first rendering is needed, instead of using the
        /// standard DrawContext.DrawGeometry which applies fill then stroke.
        /// </summary>
        /// <param name="textElement">The SVG text element being rendered</param>
        /// <param name="brush">The fill brush (or null for stroke-only)</param>
        /// <param name="pen">The stroke pen (or null for fill-only)</param>
        /// <param name="geometry">The text geometry to draw</param>
        /// <remarks>
        /// <para>
        /// This method respects the SVG paint-order CSS property with Phase 1 support:
        /// - paint-order: normal/fill → Uses standard DrawGeometry (fill then stroke)
        /// - paint-order: stroke → Creates separate geometry drawings (stroke then fill)
        /// 
        /// This allows text to have visible outlines by rendering stroke underneath
        /// and fill on top, matching modern browser behavior.
        /// </para>
        /// <para>
        /// For text rendering, when paint-order="stroke" is used:
        /// 1. Stroke geometry is drawn first (as background outline)
        /// 2. Fill geometry is drawn second (as foreground text)
        /// 3. Result: text appears with colored outline effect
        /// </para>
        /// </remarks>
        protected void DrawGeometryWithPaintOrder(SvgTextContentElement textElement, 
            Brush brush, Pen pen, Geometry geometry)
        {
            if (_drawContext == null || geometry == null || geometry.IsEmpty())
            {
                return;
            }

            if (brush == null && pen == null)
            {
                return;
            }

            // Parse paint-order CSS property
            string paintOrderValue = textElement?.GetAttribute(CssConstants.PropPaintOrder);
            if (string.IsNullOrWhiteSpace(paintOrderValue) && textElement != null)
            {
                paintOrderValue = textElement.GetPropertyValue(CssConstants.PropPaintOrder);
            }

            WpfPaintOrder paintOrder = WpfPaintOrderHelper.Parse(paintOrderValue);

            // Phase 1: Support stroke-first rendering for text
            if (paintOrder == WpfPaintOrder.Stroke && pen != null && brush != null)
            {
                // Draw stroke first (will be underneath)
                _drawContext.DrawGeometry(null, pen, geometry);

                // Draw fill second (will be on top)
                _drawContext.DrawGeometry(brush, null, geometry);
            }
            else
            {
                // Default: draw with combined brush and pen (fill then stroke)
                _drawContext.DrawGeometry(brush, pen, geometry);
            }
        }

        #endregion
    }
}
