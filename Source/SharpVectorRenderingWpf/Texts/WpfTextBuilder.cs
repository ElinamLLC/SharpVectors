using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Texts
{
    public abstract class WpfTextBuilder
    {
        private const double DefaultDpi = 96.0d;

        protected readonly string _fontName;
        protected readonly double _fontSize;
        protected readonly Uri _fontUri;

        protected readonly double _dpiX;
        protected readonly double _dpiY;

        protected bool _buildPathGeometry;

        protected CultureInfo _culture;

        protected TextDecorationCollection _textDecorations;

        protected WpfTextBuilder(CultureInfo culture, double fontSize)
        {
            var sysParam = typeof(SystemParameters);

            var dpiXProperty = sysParam.GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = sysParam.GetProperty("Dpi",  BindingFlags.NonPublic | BindingFlags.Static);

            _dpiX     = (int)dpiXProperty.GetValue(null, null);
            _dpiY     = (int)dpiYProperty.GetValue(null, null);
            _fontSize = fontSize;
            _culture  = culture;
        }

        protected WpfTextBuilder(CultureInfo culture, string fontName, double fontSize, Uri fontUri = null)
            : this(culture, fontSize)
        {
            _fontName = fontName;
            _fontUri = fontUri;
        }


        public static WpfTextBuilder Create(FontFamily fontFamily, CultureInfo culture, double fontSize)
        {
            WpfGlyphTextBuilder textBuilder = new WpfGlyphTextBuilder(fontFamily, culture, fontSize);

            return textBuilder;
        }

        public static WpfTextBuilder Create(FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight,
            CultureInfo culture, double fontSize)
        {
            WpfGlyphTextBuilder textBuilder = new WpfGlyphTextBuilder(fontFamily, fontStyle, fontWeight, 
                culture, fontSize);

            return textBuilder;
        }

        public static WpfTextBuilder Create(string familyInfo, CultureInfo culture, double fontSize)
        {
            if (string.IsNullOrWhiteSpace(familyInfo))
            {
                return new WpfGlyphTextBuilder(culture, fontSize);
            }

            WpfGlyphTextBuilder textBuilder = new WpfGlyphTextBuilder(culture, familyInfo, fontSize);

            return textBuilder;
        }

        public static WpfTextBuilder Create(WpfFontFamilyInfo familyInfo, CultureInfo culture, double fontSize)
        {
            if (familyInfo == null)
            {
                return new WpfGlyphTextBuilder(culture, fontSize);
            }
            if (familyInfo.FontFamilyType == WpfFontFamilyType.Svg)
            {
                var textBuilder = new WpfSvgTextBuilder(familyInfo.FontElement,
                    culture, familyInfo.Name, fontSize);

                textBuilder.FontStyle   = familyInfo.Style;
                textBuilder.FontWeight  = familyInfo.Weight;
                textBuilder.FontFamily  = familyInfo.Family;
                textBuilder.FontVariant = familyInfo.Variant;

                return textBuilder;
            }

            return new WpfGlyphTextBuilder(familyInfo, culture, fontSize);
        }

        public CultureInfo Culture
        {
            get {
                return _culture;
            }
        }

        public string XmlLang
        {
            get {
                if (_culture != null)
                {
                    return _culture.TwoLetterISOLanguageName;
                }
                return string.Empty;
            }
        }

        public abstract WpfFontFamilyType FontFamilyType
        {
            get;
        }        

        public string FontName
        {
            get {
                return _fontName;
            }
        }

        public double FontSize
        {
            get {
                return _fontSize;
            }
        }

        public double FontSizeInPoints
        {
            get {
                return _fontSize * 72.0d / _dpiY;
            }
        }

        public Uri FontUri
        {
            get {
                return _fontUri;
            }
        }

        /// <summary>Gets the DPI along X axis.</summary>
        /// <value>The DPI along the X axis.</value>
        public double PixelsPerInchX
        {
            get {
                return _dpiX;
            }
        }

        /// <summary>Gets the DPI along Y axis.</summary>
        /// <value>The DPI along the Y axis.</value>
        public double PixelsPerInchY
        {
            get {
                return _dpiY;
            }
        }

        /// <summary>Gets the DPI scale on the X axis.</summary>
        /// <value>The DPI scale for the X axis.</value>
        public double DpiScaleX
        {
            get {
                return _dpiX / DefaultDpi;
            }
        }

        /// <summary>Gets the DPI scale on the Yaxis.</summary>
        /// <value>The DPI scale for the Y axis.</value>
        public double DpiScaleY
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        /// <summary>Get or sets the PixelsPerDip at which the text should be rendered.</summary>
        /// <value>The current PixelsPerDip value.</value>
        public double PixelsPerDip
        {
            get {
                return _dpiY / DefaultDpi;
            }
        }

        public bool BuildPathGeometry
        {
            get {
                return _buildPathGeometry;
            }
            set {
                _buildPathGeometry = value;
            }
        }

        public abstract double Ascent { get; }

        /// <summary>
        /// Gets a value that determines whether to simulate a bold weight for the glyphs represented by the typeface.
        /// </summary>
        /// <value>true if bold simulation is used for glyphs; otherwise, false.</value>
        public abstract bool IsBoldSimulated { get; }

        /// <summary>
        /// Gets a value that determines whether to simulate an italic style for the glyphs represented by the typeface.
        /// </summary>
        /// <value>true if italic simulation is used for glyphs; otherwise, false.</value>
        public abstract bool IsObliqueSimulated { get; }

        /// <summary>
        /// Gets a value that indicates the distance from the baseline to the strikethrough for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the strikethrough position, measured from the baseline and expressed 
        /// as a fraction of the font em size.</value>
        public abstract double StrikethroughPosition { get; }

        /// <summary>
        /// Gets a value that indicates the thickness of the strikethrough relative to the font em size.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the strikethrough thickness, expressed as a fraction 
        /// of the font em size.</value>
        public abstract double StrikethroughThickness { get; }

        /// <summary>
        /// Gets a value that indicates the distance of the underline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the underline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        public abstract double UnderlinePosition { get; }

        /// <summary>
        /// Gets a value that indicates the thickness of the underline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the underline thickness, expressed as a fraction 
        /// of the font em size.</value>
        public abstract double UnderlineThickness { get; }

        /// <summary>
        /// Gets a value that indicates the distance of the overline from the baseline for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the overline position, measured from the baseline 
        /// and expressed as a fraction of the font em size.</value>
        public abstract double OverlinePosition { get; }

        /// <summary>
        /// Gets a value that indicates the thickness of the overline relative to the font em size for the typeface.
        /// </summary>
        /// <value>A <see cref="double"/> that indicates the overline thickness, expressed as a fraction 
        /// of the font em size.</value>
        public abstract double OverlineThickness { get; }

        /// <summary>
        /// Gets the distance from the baseline to the top of an English lowercase letter for a typeface. 
        /// The distance excludes ascenders.
        /// </summary>
        /// <value>
        /// A <see cref="double"/> that indicates the distance from the baseline to the top of an
        /// English lowercase letter (excluding ascenders), expressed as a fraction of the font em size.
        /// </value>
        public abstract double XHeight { get; }

        public abstract double Alphabetic { get; }

        public abstract double Width { get; }

        /// <summary>
        /// Gets the distance from the top of the first line to the baseline of the first
        /// line of a <see cref="WpfTextBuilder"/> object.
        /// </summary>
        /// <value>
        /// The distance from the top of the first line to the baseline of the first line,
        /// provided in device-independent units (1/96th inch per unit).
        /// </value>
        public virtual double Baseline
        {
            get {
                return this.Ascent;
            }
        }

        /// <summary>
        /// Gets or sets the System.Windows.FlowDirection of a <see cref="WpfTextBuilder"/> object.
        /// </summary>
        /// <value>
        /// The System.Windows.FlowDirection of the formatted text.
        /// </value>
        public FlowDirection FlowDirection { get; set; }

        /// <summary>
        /// Gets or sets the alignment of text within a <see cref="WpfTextBuilder"/> object.
        /// </summary>
        /// <value>
        /// One of the System.Windows.TextAlignment values that specifies the alignment of
        /// text within a <see cref="WpfTextBuilder"/> object.
        /// </value>
        public TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Gets or sets the means by which the omission of text is indicated.
        /// </summary>
        /// <value>
        /// One of the System.Windows.TextTrimming values that specifies how the omission
        /// of text is indicated. The default is System.Windows.TextTrimming.WordEllipsis.
        /// </value>
        public TextTrimming Trimming { get; set; }

        /// <summary>
        /// Sets the System.Windows.TextDecorationCollection for the entire set of characters
        /// in the <see cref="WpfTextBuilder"/> object.
        /// </summary>
        /// <value name="textDecorations">The System.Windows.TextDecorationCollection to apply to the text.</value>
        public TextDecorationCollection TextDecorations
        {
            get {
                return _textDecorations;
            }
            set {
                _textDecorations = value;
            }
        }
        
        public abstract IList<Rect> MeasureChars(SvgTextContentElement element, string text, bool canBeWhitespace = true);

        public abstract Size MeasureText(SvgTextContentElement element, string text, bool canBeWhitespace = true);

        public abstract Geometry Build(SvgTextContentElement element, string text, double x, double y);
    }
}
