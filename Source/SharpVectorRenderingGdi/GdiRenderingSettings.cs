using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

namespace SharpVectors.Renderers
{
    public sealed class GdiRenderingSettings : ICloneable
    {
        #region Private Fields

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

        private IDictionary<string, object> _properties;

        #endregion

        #region Constructors and Destructor

        public GdiRenderingSettings()
        {
            _defaultFontName       = "Arial";
            _neutralCulture        = CultureInfo.GetCultureInfo("en-us");
            _culture               = CultureInfo.GetCultureInfo("en-us");

            _pixelWidth            = -1;
            _pixelHeight           = -1;

            _ensureViewboxSize     = false;
            _ensureViewboxPosition = true;
            _ignoreRootViewbox     = false;
            _properties            = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public GdiRenderingSettings(GdiRenderingSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            _defaultFontName       = settings._defaultFontName;

            _neutralCulture        = settings._neutralCulture;
            _culture               = settings._culture;

            _pixelWidth            = settings._pixelWidth;
            _pixelHeight           = settings._pixelHeight;

            _ensureViewboxSize     = settings._ensureViewboxSize;
            _ensureViewboxPosition = settings._ensureViewboxPosition;
            _ignoreRootViewbox     = settings._ignoreRootViewbox;

            _userCssFilePath       = settings._userCssFilePath;
            _userAgentCssFilePath  = settings._userAgentCssFilePath;

            _properties            = settings._properties;
        }

        #endregion

        #region Public Properties

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
                if (string.IsNullOrWhiteSpace(name) || _properties == null || _properties.Count == 0)
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
                return (_pixelWidth > 0 && _pixelHeight > 0);
            }
        }

        public string UserCssFilePath
        {
            get {
                return _userCssFilePath;
            }
            set {
                _userCssFilePath = value;
            }
        }

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
        /// Gets or sets the main culture information used for rendering texts.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="CultureInfo"/> specifying the main
        /// culture information for texts. The default is the English culture.
        /// </value>
        /// <remarks>
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
                    // Possibilities: Tahoma, Arial, Verdana, Trebuchet, MS Sans Serif, Helvetica
                    _genericSansSerif = new FontFamily("Tahoma");
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
                    _genericMonospace = new FontFamily("MS Gothic");
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

        #endregion

        #region ICloneable Members

        public GdiRenderingSettings Clone()
        {
            GdiRenderingSettings clonedSettings = new GdiRenderingSettings(this);

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

            return clonedSettings;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
