using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using SharpVectors.Dom;

namespace SharpVectors.Renderers.Wpf
{
    using DpiScale = SharpVectors.Runtime.DpiScale;
    using DpiUtilities = SharpVectors.Runtime.DpiUtilities;
    using SvgInteractiveModes = SharpVectors.Runtime.SvgInteractiveModes;

    /// <summary>
    /// This provides the options for the drawing/rendering engine of the WPF.
    /// </summary>
    [Serializable]
    public sealed class WpfDrawingSettings : WpfSettings<WpfDrawingSettings>
    {
        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        public const string XmlTagName = "drawingSettings";
        public const string XmlVersion = "1.0.0";

        #region Public Fields

        public const string PropertyNonePen     = "_NonePen";
        public const string PropertyNoneBrush   = "_NoneBrush";
        public const string PropertyIsResources = "_IsResources";

        #endregion

        #region Private Fields

        private bool _textAsGeometry;
        private bool _includeRuntime;
        private bool _optimizePath;

        private int _pixelWidth;
        private int _pixelHeight;
        private bool _ensureViewboxPosition;
        private bool _ensureViewboxSize;
        private bool _ignoreRootViewbox;

        private string _userCssFilePath;
        private string _userAgentCssFilePath;

        // Formating properties
        private CultureInfo _culture;
        private CultureInfo _neutralCulture;

        // Text rendering fonts properties
        private string _defaultFontName;
        private static FontFamily _defaultFontFamily;
        private static FontFamily _genericSerif;
        private static FontFamily _genericSansSerif;
        private static FontFamily _genericMonospace;

        private static FontFamily _genericCursive;
        private static FontFamily _genericFantasy;

        private WpfVisitors _wpfVisitors;

        private ISet<string> _fontLocations;
        private IList<FontFamily> _fontFamilies;
        private IDictionary<string, string> _fontFamilyNames;
        private IDictionary<string, IList<FontFamily>> _fontFamilyMap;

        private IDictionary<string, object> _properties;
        private IDictionary<string, string> _cssVariables;

        private object _fontSynch;

        private DpiScale _dpiScale;

        private SvgInteractiveModes _interactiveMode;

        private WpfDrawingResources _drawingResources;
        private AccessExternalResourcesMode _accessMode;
        private bool _canUseBitmap = true;

        #endregion

        #region Constructor and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="WpfDrawingSettings"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDrawingSettings"/> class
        /// with the default parameters and settings.
        /// </summary>
        public WpfDrawingSettings()
        {
            _defaultFontName       = "Arial";
            _textAsGeometry        = false;
            _optimizePath          = true;
            _includeRuntime        = true;
            _neutralCulture        = CultureInfo.GetCultureInfo("en-us");
            _culture               = CultureInfo.GetCultureInfo("en-us");

            _pixelWidth            = -1;
            _pixelHeight           = -1;

            _ensureViewboxSize     = false;
            _ensureViewboxPosition = true;
            _ignoreRootViewbox     = false;
            _wpfVisitors           = new WpfVisitors();
            _properties            = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            _fontSynch             = new object();
            _fontLocations         = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _fontFamilyNames       = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _fontFamilyMap         = new Dictionary<string, IList<FontFamily>>(StringComparer.OrdinalIgnoreCase);
            _cssVariables          = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _dpiScale              = DpiUtilities.GetSystemScale();
            _accessMode            = AccessExternalResourcesMode.Allow;
            _canUseBitmap          = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDrawingSettings"/> class
        /// with the specified initial drawing or rendering settings, a copy constructor.
        /// </summary>
        /// <param name="settings">
        /// This specifies the initial options for the rendering or drawing engine.
        /// </param>
        public WpfDrawingSettings(WpfDrawingSettings settings)
            : this()
        {
            if (settings == null)
            {
                return;
            }

            _defaultFontName       = settings._defaultFontName;
            _textAsGeometry        = settings._textAsGeometry;
            _optimizePath          = settings._optimizePath;
            _includeRuntime        = settings._includeRuntime;

            _neutralCulture        = settings._neutralCulture;
            _culture               = settings._culture;

            _pixelWidth            = settings._pixelWidth;
            _pixelHeight           = settings._pixelHeight;

            _ensureViewboxSize     = settings._ensureViewboxSize;
            _ensureViewboxPosition = settings._ensureViewboxPosition;
            _ignoreRootViewbox     = settings._ignoreRootViewbox;
            _wpfVisitors           = settings._wpfVisitors;

            _userCssFilePath       = settings._userCssFilePath;
            _userAgentCssFilePath  = settings._userAgentCssFilePath;

            _properties            = settings._properties;

            _fontSynch             = settings._fontSynch;
            _fontLocations         = settings._fontLocations;
            _fontFamilyNames       = settings._fontFamilyNames;
            _fontFamilyMap         = settings._fontFamilyMap;
            _cssVariables          = settings._cssVariables;
            _dpiScale              = settings._dpiScale;
            _accessMode            = settings._accessMode;
            _canUseBitmap          = settings._canUseBitmap;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return null;
                }
                if (_properties == null || _properties.Count == 0 || !_properties.ContainsKey(name))
                {
                    return null;
                }
                return _properties[name];
            }
            set {
                if (string.IsNullOrWhiteSpace(name) || _properties == null)
                {
                    return;
                }
                if (value == null)
                {
                    if (_properties.ContainsKey(name))
                    {
                        _properties.Remove(name);
                    }
                }
                else
                {
                    _properties[name] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int PixelWidth
        {
            get {
                return _pixelWidth;
            }
            set {
                _pixelWidth = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int PixelHeight
        {
            get {
                return _pixelHeight;
            }
            set {
                _pixelHeight = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public bool HasPixelSize
        {
            get {
                return (_pixelWidth > 0 && _pixelHeight > 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string UserCssFilePath
        {
            get {
                return _userCssFilePath;
            }
            set {
                _userCssFilePath = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string UserAgentCssFilePath
        {
            get {
                return _userAgentCssFilePath;
            }
            set {
                _userAgentCssFilePath = value;
            }
        }

        /// <summary>
        /// Gets or sets user-defined CSS custom properties for the rendering.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}"/> of user-defined styles. This value is
        /// never <see langword="null"/>.
        /// </value>
        public IDictionary<string, string> CssVariables
        {
            get {
                if (_cssVariables == null)
                {
                    _cssVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _cssVariables;
            }
            set {
                if (value != null)
                {
                    _cssVariables = value;
                }
                else
                {
                    _cssVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to indicate turning off viewbox at the root of the drawing.
        /// </summary>
        /// <value>
        /// For image outputs, this will force the original size to be saved.
        /// <para>
        /// The default value is <see langword="false"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// There are reported cases where are diagrams displayed in Inkscape program, but will not
        /// show when converted. These are diagrams on the drawing canvas of Inkspace but outside 
        /// the svg viewbox. 
        /// <para>
        /// When converted the drawings are also converted but not displayed due to
        /// clipping. Setting this property to <see langword="true"/> will clear the clipping region
        /// on conversion.
        /// </para>
        /// </remarks>
        public bool IgnoreRootViewbox
        {
            get {
                return _ignoreRootViewbox;
            }
            set {
                _ignoreRootViewbox = value;
            }
        }

        /// <summary>
        /// Gets or sets a value to indicate preserving the original viewbox size when saving images.
        /// </summary>
        /// <value>
        /// For image outputs, this will force the original size to be saved.
        /// <para>
        /// The default value is <see langword="false"/>. However, the ImageSvgConverter converted
        /// sets this to <see langword="true"/> by default.
        /// </para>
        /// </value>
        /// <remarks>
        /// Setting this to <see langword="true"/> will cause the rendering process to draw a transparent
        /// box around the output, if a viewbox is defined. This will ensure that the original image
        /// size is saved.
        /// </remarks>
        public bool EnsureViewboxSize
        {
            get {
                return _ensureViewboxSize;
            }
            set {
                _ensureViewboxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value to indicate applying a translate transform to the viewbox to ensure
        /// it is visible when rendered.
        /// </summary>
        /// <value>
        /// This determines whether a transformation is applied to the rendered drawing. For drawings
        /// where the top-left position of the viewbox is off the screen, due to negative values, this
        /// will ensure the drawing is visible.
        /// <para>
        /// The default value is <see langword="true"/>. Set this value to <see langword="false"/> if
        /// you wish to apply your own transformations to the drawings.
        /// </para>
        /// </value>
        public bool EnsureViewboxPosition
        {
            get {
                return _ensureViewboxPosition;
            }
            set {
                _ensureViewboxPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the path geometry is 
        /// optimized using the <see cref="StreamGeometry"/>.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the path geometry is optimized
        /// using the <see cref="StreamGeometry"/>; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool OptimizePath
        {
            get {
                return _optimizePath;
            }
            set {
                _optimizePath = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texts are rendered as
        /// path geometry.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if texts are rendered as path 
        /// geometries; otherwise, this is <see langword="false"/>. The default
        /// is <see langword="false"/>.
        /// </value>
        public bool TextAsGeometry
        {
            get {
                return _textAsGeometry;
            }
            set {
                _textAsGeometry = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <c>SharpVectors.Runtime.dll</c>
        /// classes are used in the generated output.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the <c>SharpVectors.Runtime.dll</c>
        /// classes and types are used in the generated output; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// The use of the <c>SharpVectors.Runtime.dll</c> prevents the hard-coded
        /// font path generated by the <see cref="FormattedText"/> class, support
        /// for embedded images etc.
        /// </remarks>
        public bool IncludeRuntime
        {
            get {
                return _includeRuntime;
            }
            set {
                _includeRuntime = value;
            }
        }

        /// <summary>
        /// Gets or sets the main culture information used for rendering texts.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="CultureInfo"/> specifying the main
        /// culture information for texts. The default is the English culture.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is the culture information passed to the <see cref="FormattedText"/>
        /// class instance for the text rendering.
        /// </para>
        /// <para>
        /// The library does not currently provide any means of splitting texts
        /// into its multi-language parts.
        /// </para>
        /// </remarks>
        public CultureInfo CultureInfo
        {
            get {
                return _culture;
            }
            set {
                if (value != null)
                {
                    _culture = value;
                }
            }
        }

        /// <summary>
        /// Gets the neutral language for text rendering.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="CultureInfo"/> specifying the neutral
        /// culture information for texts. The default is the English culture.
        /// </value>
        /// <remarks>
        /// For vertical text rendering, there is a basic text splitting into
        /// Western and other languages. This culture information is used to
        /// render the Western language part, and the mains culture information
        /// for the other languages.
        /// </remarks>
        public CultureInfo NeutralCultureInfo
        {
            get {
                return _neutralCulture;
            }
        }

        /// <summary>
        /// Gets or sets the default font family name, which is used when a text
        /// node does not specify a font family name.
        /// </summary>
        /// <value>
        /// A string containing the default font family name. The default is
        /// the <c>Arial Unicode MS</c> font, for its support of Unicode texts.
        /// This value cannot be <see langword="null"/> or empty.
        /// </value>
        public string DefaultFontName
        {
            get {
                return _defaultFontName;
            }
            set {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _defaultFontName = value;
                    _defaultFontFamily = new FontFamily(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public WpfVisitors Visitors
        {
            get {
                if (_wpfVisitors == null)
                {
                    _wpfVisitors = new WpfVisitors();
                }
                return _wpfVisitors;
            }
        }

        /// <summary>
        /// Gets or sets the globally available default font family.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="FontFamily"/> specifying the globally available font family. 
        /// The default is <c>Arial</c> font (since <c>Arial Unicode MS</c> is no longer shipped by MS).
        /// family.
        /// </value>
        public static FontFamily DefaultFontFamily
        {
            get {
                if (_defaultFontFamily == null)
                {
                    //_defaultFontFamily = new FontFamily("Arial Unicode MS");
                    _defaultFontFamily = new FontFamily("Arial");
                }

                return _defaultFontFamily;
            }
            set {
                if (value != null)
                {
                    _defaultFontFamily = value;
                }
            }
        }

        /// <summary>
        /// Gets or set the globally available generic serif font family.
        /// </summary>
        /// <value>
        /// An instance of <see cref="FontFamily"/> specifying the generic serif
        /// font family. The default is <c>Times New Roman</c> font family.
        /// </value>
        public static FontFamily GenericSerif
        {
            get {
                if (_genericSerif == null)
                {
                    _genericSerif = new FontFamily("Times New Roman");
                }

                return _genericSerif;
            }
            set {
                if (value != null)
                {
                    _genericSerif = value;
                }
            }
        }

        /// <summary>
        /// Gets or set the globally available generic sans serif font family.
        /// </summary>
        /// <value>
        /// An instance of <see cref="FontFamily"/> specifying the generic sans 
        /// serif font family. The default is <c>Tahoma</c> font family.
        /// </value>
        /// <remarks>
        /// The possible font names are <c>Tahoma</c>, <c>Arial</c>, 
        /// <c>Verdana</c>, <c>Trebuchet</c>, <c>MS Sans Serif</c> and <c>Helvetica</c>.
        /// </remarks>
        public static FontFamily GenericSansSerif
        {
            get {
                if (_genericSansSerif == null)
                {
                    // Possibilities: Tahoma, Verdana, Arial, Trebuchet, MS Sans Serif, Helvetica
                    _genericSansSerif = new FontFamily("Arial, Tohama");
                }

                return _genericSansSerif;
            }
            set {
                if (value != null)
                {
                    _genericSansSerif = value;
                }
            }
        }

        /// <summary>
        /// Gets or set the globally available generic Monospace font family.
        /// </summary>
        /// <value>
        /// An instance of <see cref="FontFamily"/> specifying the generic 
        /// Monospace font family. The default is <c>MS Gothic</c> font family.
        /// </value>
        public static FontFamily GenericMonospace
        {
            get {
                if (_genericMonospace == null)
                {
                    // Possibilities: Courier New, MS Gothic
                    _genericMonospace = new FontFamily("MS Gothic, Courier New");
                }

                return _genericMonospace;
            }
            set {
                if (value != null)
                {
                    _genericMonospace = value;
                }
            }
        }

        /// <summary>
        /// Gets or set the globally available generic cursive font family.
        /// </summary>
        /// <value>
        /// An instance of <see cref="FontFamily"/> specifying the generic 
        /// cursive font family. The default is <c>Comic Sans MS</c> font family.
        /// </value>
        public static FontFamily GenericCursive
        {
            get {
                if (_genericCursive == null)
                {
                    // Possibilities: Comic Sans MS, Comic Sans
                    _genericCursive = new FontFamily("Comic Sans MS, Comic Sans");
                }

                return _genericCursive;
            }
            set {
                if (value != null)
                {
                    _genericCursive = value;
                }
            }
        }

        /// <summary>
        /// Gets or set the globally available generic fantasy font family.
        /// </summary>
        /// <value>
        /// An instance of <see cref="FontFamily"/> specifying the generic 
        /// fantasy font family. The default is <c>Impact</c> font family.
        /// </value>
        public static FontFamily GenericFantasy
        {
            get {
                if (_genericFantasy == null)
                {
                    // Possibilities: Impact
                    _genericFantasy = new FontFamily("Impact");
                }

                return _genericFantasy;
            }
            set {
                if (value != null)
                {
                    _genericFantasy = value;
                }
            }
        }

        public AccessExternalResourcesMode AccessExternalResourcesMode
        {
            get {
                return _accessMode;
            }
            set {
                _accessMode = value;
            }
        }

        public bool CanUseBitmap
        {
            get
            {
                return _canUseBitmap;
            }
            set
            {
                _canUseBitmap = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IEnumerable<string> FontLocations
        {
            get {
                return _fontLocations;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IDictionary<string, string> FontFamilyNames
        {
            get {
                return _fontFamilyNames;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public bool HasFontFamilies
        {
            get {
                if (_fontFamilies == null || _fontFamilies.Count == 0)
                {
                    this.LoadFontFamilies();
                }
                return (_fontFamilies != null && _fontFamilies.Count != 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IEnumerable<FontFamily> FontFamilies
        {
            get {
                if (_fontFamilies == null || _fontFamilies.Count == 0)
                {
                    this.LoadFontFamilies();
                }
                return _fontFamilies;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public SvgInteractiveModes InteractiveMode
        {
            get {
                return _interactiveMode;
            }
            set {
                _interactiveMode = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DpiScale DpiScale
        {
            get {
                return _dpiScale;
            }
            set {
                if (value != null)
                {
                    _dpiScale = value;
                }
            }
        }

        public WpfDrawingResources DrawingResources
        {
            get {
                return _drawingResources;
            }
            set {
                _drawingResources = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappedName"></param>
        /// <param name="fontName"></param>
        public void AddFontFamilyName(string mappedName, string fontName)
        {
            lock(_fontSynch)
            {
                if (string.IsNullOrWhiteSpace(mappedName) || string.IsNullOrWhiteSpace(fontName))
                {
                    return;
                }
                if (_fontFamilyNames == null)
                {
                    _fontFamilyNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                if (!_fontFamilyNames.ContainsKey(mappedName))
                {
                    _fontFamilyNames.Add(mappedName, fontName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontLocation"></param>
        public void AddFontLocation(string fontLocation)
        {
            lock(_fontSynch)
            {
                if (string.IsNullOrWhiteSpace(fontLocation))
                {
                    return;
                }
                if (File.Exists(fontLocation) == false && Directory.Exists(fontLocation) == false)
                {
                    return;
                }

                if (_fontLocations == null)
                {
                    _fontLocations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
                if (Directory.Exists(fontLocation))
                {
                    // Fonts.GetFontFamilies(...) only works for directories with "\\" ending...
                    if (!fontLocation.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                    {
                        fontLocation += "\\";
                    }
                }
                if (_fontLocations.Contains(fontLocation))
                {
                    return;
                }
                _fontLocations.Add(fontLocation);

                // If font-families are already loaded, then load the new font path...
                if (_fontFamilies != null && _fontFamilies.Count != 0)
                {
                    this.AddFontFamilies(fontLocation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public FontFamily LookupFontFamily(string fontName, FontWeight weight, FontStyle style, FontStretch stretch)
        {
            lock (_fontSynch)
            {
                if (string.IsNullOrWhiteSpace(fontName))
                {
                    return null;
                }

                if (_fontFamilyMap == null || _fontFamilyMap.Count == 0)
                {
                    this.BuildDocumentFonts();
                }

                if (_fontFamilyMap.ContainsKey(fontName))
                {
                    return GetMatchingFontFamily(_fontFamilyMap[fontName], weight, style, stretch);
                }
                if (_fontFamilyNames != null && _fontFamilyNames.Count != 0)
                {
                    if (_fontFamilyNames.ContainsKey(fontName))
                    {
                        var internalName = _fontFamilyNames[fontName];

                        if (_fontFamilyMap.ContainsKey(internalName))
                        {
                            return GetMatchingFontFamily(_fontFamilyMap[internalName], weight, style, stretch);
                        }
                    }
                }

                string normalizedName = null;
                if (fontName.IndexOf('-') > 0)
                {
                    normalizedName = fontName.Replace("-", " ");
                    if (_fontFamilyMap.ContainsKey(normalizedName))
                    {
                        return GetMatchingFontFamily(_fontFamilyMap[normalizedName], weight, style, stretch);
                    }
                }

                if (WpfRendererObject.SplitByCaps(fontName, out normalizedName))
                {
                    normalizedName = fontName.Replace("-", " ");
                    if (_fontFamilyMap.ContainsKey(normalizedName))
                    {
                        return GetMatchingFontFamily(_fontFamilyMap[normalizedName], weight, style, stretch);
                    }
                }

                return null;
            }
        }

        private FontFamily GetMatchingFontFamily(IList<FontFamily> fontFamilies,
            FontWeight weight, FontStyle style, FontStretch stretch)
        {
            if (fontFamilies == null || fontFamilies.Count == 0)
            {
                return null;
            }
            // For a single match...
            if (fontFamilies.Count == 1)
            {
                return fontFamilies[0];
            }

            // 1. Look for a possibility of all properties matching
            foreach (FontFamily fontFamily in fontFamilies)
            {
                // Return the family typeface collection for the font family.
                FamilyTypefaceCollection familyTypefaces = fontFamily.FamilyTypefaces;

                // Enumerate the family typefaces in the collection.
                foreach (FamilyTypeface typeface in familyTypefaces)
                {
                    FontStyle fontStyle     = typeface.Style;
                    FontWeight fontWeight   = typeface.Weight;
                    FontStretch fontStretch = typeface.Stretch;

                    if (fontStyle.Equals(style) && fontWeight.Equals(weight) && fontStretch.Equals(stretch))
                    {
                        return fontFamily;
                    }
                }
            }

            // For the defined font style...
            if (style != FontStyles.Normal)
            {
                // Then it is either oblique or italic
                FontFamily closeFamily   = null;
                FontFamily closestFamily = null;
                bool isItalic            = style.Equals(FontStyles.Italic);

                foreach (FontFamily fontFamily in fontFamilies)
                {
                    // Return the family typeface collection for the font family.
                    FamilyTypefaceCollection familyTypefaces = fontFamily.FamilyTypefaces;

                    // Enumerate the family typefaces in the collection.
                    foreach (FamilyTypeface typeface in familyTypefaces)
                    {
                        FontStyle fontStyle     = typeface.Style;
                        FontWeight fontWeight   = typeface.Weight;
                        FontStretch fontStretch = typeface.Stretch;

                        if (fontStyle.Equals(style))
                        {
                            closeFamily = fontFamily;
                            if (closestFamily == null)
                            {
                                closestFamily = fontFamily;
                            }
                            if (fontStretch.Equals(stretch))
                            {
                                closestFamily = fontFamily;
                                if (fontWeight.Equals(weight))
                                {
                                    return fontFamily;
                                }
                            }
                        }
                        if (closeFamily == null)
                        {
                            if (isItalic && fontStyle == FontStyles.Oblique)
                            {
                                closeFamily = fontFamily;
                            }
                            if (!isItalic && fontStyle == FontStyles.Italic)
                            {
                                closeFamily = fontFamily;
                            }
                        }
                    }
                    if (closestFamily != null)
                    {
                        closeFamily = closestFamily;
                    }

                    if (closeFamily != null)
                    {
                        return closeFamily;
                    }
                }
            }

            // For the defined font weights...
            if (weight != FontWeights.Normal && weight != FontWeights.Regular)
            {
                int weightValue             = weight.ToOpenTypeWeight();
                int selectedValue           = int.MaxValue;
                FontFamily sameWeightFamily = null;
                FontFamily closestFamily    = null;
                foreach (FontFamily fontFamily in fontFamilies)
                {
                    // Return the family typeface collection for the font family.
                    FamilyTypefaceCollection familyTypefaces = fontFamily.FamilyTypefaces;

                    // Enumerate the family typefaces in the collection.
                    foreach (FamilyTypeface typeface in familyTypefaces)
                    {
                        FontStyle fontStyle     = typeface.Style;
                        FontWeight fontWeight   = typeface.Weight;
                        FontStretch fontStretch = typeface.Stretch;

                        if (fontWeight.Equals(weight))
                        {
                            sameWeightFamily = fontFamily;
                            if (fontStyle.Equals(style))
                            {
                                return fontFamily;
                            }
                        }

                        int weightDiff = Math.Abs(weightValue - fontWeight.ToOpenTypeWeight());
                        if (weightDiff < selectedValue)
                        {
                            closestFamily = fontFamily;
                            selectedValue = weightDiff;
                        }

                        // If the weights matched, but not the style
                        if (sameWeightFamily != null)
                        {
                            return sameWeightFamily;
                        }
                        if (closestFamily != null)
                        {
                            return closestFamily;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            NotNull(reader, nameof(reader));

            throw new NotImplementedException();
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            NotNull(writer, nameof(writer));

            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new settings object that is a deep copy of the current 
        /// instance.
        /// </overloads>
        /// <summary>
        /// This creates a new settings object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new settings object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this settings object. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override WpfDrawingSettings Clone()
        {
            WpfDrawingSettings clonedSettings = new WpfDrawingSettings(this);

            if (!string.IsNullOrWhiteSpace(_defaultFontName))
            {
                clonedSettings._defaultFontName = new string(_defaultFontName.ToCharArray());
            }
            if (!string.IsNullOrWhiteSpace(_userCssFilePath))
            {
                clonedSettings._userCssFilePath = new string(_userCssFilePath.ToCharArray());
            }
            if (!string.IsNullOrWhiteSpace(_userAgentCssFilePath))
            {
                clonedSettings._userAgentCssFilePath = new string(_userAgentCssFilePath.ToCharArray());
            }
            if (_culture != null)
            {
                clonedSettings._culture = (CultureInfo)_culture.Clone();
            }
            if (_neutralCulture != null)
            {
                clonedSettings._neutralCulture = (CultureInfo)_neutralCulture.Clone();
            }
            if (_dpiScale != null)
            {
                clonedSettings._dpiScale = _dpiScale.Clone();
            }

            return clonedSettings;
        }

        #endregion

        #region Private Methods

        private void BuildDocumentFonts()
        {
            if (_fontFamilyMap == null)
            {
                _fontFamilyMap = new Dictionary<string, IList<FontFamily>>(StringComparer.OrdinalIgnoreCase);
            }
            if (_fontFamilyMap.Count != 0)
            {
                return;
            }
            if (_fontFamilies == null || _fontFamilies.Count == 0)
            {
                this.LoadFontFamilies();
            }

            foreach (var fontFamily in _fontFamilies)
            {
                IList<FontFamily> fontFamilies = null;

                var fontName = fontFamily.Source;
                var hashIndex = fontName.IndexOf('#');
                if (hashIndex > 0)
                {
                    fontName = fontName.Substring(hashIndex + 1);
                }
                if (!string.IsNullOrWhiteSpace(fontName))
                {
                    if (!_fontFamilyMap.ContainsKey(fontName))
                    {
                        fontFamilies = new List<FontFamily>();
                        _fontFamilyMap.Add(fontName, fontFamilies);
                    }
                    else
                    {
                        fontFamilies = _fontFamilyMap[fontName];
                    }
                    fontFamilies.Add(fontFamily);
                }

                var fontNames = fontFamily.FamilyNames;
                if (fontNames != null && fontNames.Count != 0)
                {
                    foreach (var value in fontNames.Values)
                    {
                        if (!_fontFamilyMap.ContainsKey(value))
                        {
                            fontFamilies = new List<FontFamily>();
                            _fontFamilyMap.Add(value, fontFamilies);
                        }
                        else
                        {
                            fontFamilies = _fontFamilyMap[fontName];
                        }
                        fontFamilies.Add(fontFamily);
                    }
                }
            }
        }

        private void LoadFontFamilies()
        {
            if (_fontFamilies == null)
            {
                _fontFamilies = new List<FontFamily>();
            }
            if (_fontLocations == null)
            {
                _fontLocations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            foreach (var privateFontPath in _fontLocations)
            {
                this.AddFontFamilies(privateFontPath);
            }
        }

        private bool AddFontFamilies(string fontLocation)
        {
            if (_fontFamilies == null)
            {
                _fontFamilies = new List<FontFamily>();
            }

            bool isLoaded = false;
            if (File.Exists(fontLocation) || Directory.Exists(fontLocation))
            {
                var fontFamilies = Fonts.GetFontFamilies(fontLocation);
                foreach (var fontFamily in fontFamilies)
                {
                    _fontFamilies.Add(fontFamily);
                    isLoaded = true;
                }
                return isLoaded;
            }
            return isLoaded;
        }

        #endregion
    }
}
