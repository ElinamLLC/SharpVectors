using System;
using System.Globalization;

using System.Windows;
using System.Windows.Media;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// This provides the options for the drawing/rendering engine of the WPF.
    /// </summary>
    [Serializable]
    public sealed class WpfDrawingSettings : DependencyObject, ICloneable
    {
        #region Private Fields

        private bool _textAsGeometry;
        private bool _includeRuntime;
        private bool _optimizePath;

        private int _pixelWidth;
        private int _pixelHeight;
        private bool _ensureViewboxPosition;
        private bool _ensureViewboxSize;
        private bool _ignoreRootViewbox;

        // Formating properties
        private CultureInfo _culture;
        private CultureInfo _neutralCulture;

        // Text rendering fonts properties
        private string            _defaultFontName;
        private static FontFamily _defaultFontFamily;
        private static FontFamily _genericSerif;
        private static FontFamily _genericSansSerif;
        private static FontFamily _genericMonospace;

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
            _defaultFontName       = "Arial Unicode MS";
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDrawingSettings"/> class
        /// with the specified initial drawing or rendering settings, a copy constructor.
        /// </summary>
        /// <param name="settings">
        /// This specifies the initial options for the rendering or drawing engine.
        /// </param>
        public WpfDrawingSettings(WpfDrawingSettings settings)
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
        }

        #endregion

        #region Public Properties

        public int PixelWidth
        {
            get {
                return _pixelWidth;
            }
            set {
                _pixelWidth = value;
            }
        }

        public int PixelHeight
        {
            get {
                return _pixelHeight;
            }
            set {
                _pixelHeight = value;
            }
        }

        public bool HasPixelSize
        {
            get {
                return (_pixelWidth >= 0 && _pixelHeight >= 0);
            }
        }

        /// <summary>
        /// Gets or sets a value to indicate turning of viewbox at the root of the drawing.
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
            get
            {
                return _optimizePath;
            }
            set
            {
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
            get
            {
                return _textAsGeometry;
            }
            set
            {
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
            get
            {
                return _includeRuntime;
            }
            set
            {
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
            get
            {
                return _culture;
            }
            set
            {
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
            get
            {
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
            get
            {
                return _defaultFontName;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _defaultFontName   = value;
                    _defaultFontFamily = new FontFamily(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the globally available default font family.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="FontFamily"/> specifying the globally
        /// available font family. The default is a <c>Arial Unicode MS</c> font
        /// family.
        /// </value>
        public static FontFamily DefaultFontFamily
        {
            get
            {
                if (_defaultFontFamily == null)
                {
                    _defaultFontFamily = new FontFamily("Arial Unicode MS");
                }

                return _defaultFontFamily;
            }
            set
            {
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
            get
            {
                if (_genericSerif == null)
                {
                    _genericSerif = new FontFamily("Times New Roman");
                }

                return _genericSerif;
            }
            set
            {
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
            get
            {
                if (_genericSansSerif == null)
                {
                    // Possibilities: Tahoma, Arial, Verdana, Trebuchet, MS Sans Serif, Helvetica
                    _genericSansSerif = new FontFamily("Tahoma");
                }

                return _genericSansSerif;
            }
            set
            {
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
            get
            {
                if (_genericMonospace == null)
                {
                    // Possibilities: Courier New, MS Gothic
                    _genericMonospace = new FontFamily("MS Gothic");
                }

                return _genericMonospace;
            }
            set
            {
                if (value != null)
                {
                    _genericMonospace = value;
                }
            }
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
        public WpfDrawingSettings Clone()
        {
            WpfDrawingSettings settings = new WpfDrawingSettings(this);

            return settings;
        }

        /// <summary>
        /// This creates a new settings object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new settings object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this style object. If you need just a copy,
        /// use the copy constructor to create a new instance.
        /// </remarks>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
