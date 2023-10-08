using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Resources;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Utils;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    using DpiScale = SharpVectors.Runtime.DpiScale;
    using DpiUtilities = SharpVectors.Runtime.DpiUtilities;

    /// <summary>
    /// This is a <see cref="Viewbox"/> control for viewing <c>SVG</c> file in <c>WPF</c> applications.
    /// </summary>
    /// <remarks>
    /// It wraps the drawing canvas, <see cref="SvgDrawingCanvas"/>, instead of image control, therefore any 
    /// interactivity support implemented in the drawing canvas will be available in the <see cref="Viewbox"/>.
    /// </remarks>
    /// <seealso cref="SvgCanvas"/>
    public class SvgViewbox : Viewbox, ISvgControl, IUriContext
    {
        #region Public Fields

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(SvgViewbox),
                new FrameworkPropertyMetadata(null, OnUriSourceChanged));

        /// <summary>
        /// Identifies the <see cref="UriSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(SvgViewbox),
                new FrameworkPropertyMetadata(null, OnUriSourceChanged));

        /// <summary>
        /// Identifies the <see cref="SvgSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SvgSourceProperty =
            DependencyProperty.Register("SvgSource", typeof(string), typeof(SvgViewbox),
                new FrameworkPropertyMetadata(null, OnSvgSourceChanged));

        /// <summary>
        /// Identifies the <see cref="StreamSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register("StreamSource", typeof(Stream), typeof(SvgViewbox),
                new FrameworkPropertyMetadata(null, OnStreamSourceChanged));

        /// <summary>
        /// The DependencyProperty for the MessageFontFamily property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      System Dialog Font
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageFontFamilyProperty =
           DependencyProperty.Register("MessageFontFamily", typeof(FontFamily), typeof(SvgViewbox),
               new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageFontSize property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      48 pixels
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageFontSizeProperty =
           DependencyProperty.Register("MessageFontSize", typeof(double), typeof(SvgViewbox),
              new FrameworkPropertyMetadata(48d, OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageOpacity property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      1 (full opacity)
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageOpacityProperty =
           DependencyProperty.Register("MessageOpacity", typeof(double), typeof(SvgViewbox),
              new FrameworkPropertyMetadata(1d, OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageText property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      "Loading..."
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty =
           DependencyProperty.Register("MessageText", typeof(string), typeof(SvgViewbox), new
              FrameworkPropertyMetadata("Loading...", OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageBackground property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      <see cref="Brushes.PapayaWhip"/>
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageBackgroundProperty =
           DependencyProperty.Register("MessageBackground", typeof(Brush), typeof(SvgViewbox),
              new FrameworkPropertyMetadata(Brushes.PapayaWhip, OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageFillBrush property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      <see cref="Brushes.Gold"/>
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageFillBrushProperty =
           DependencyProperty.Register("MessageFillBrush", typeof(Brush), typeof(SvgViewbox),
              new FrameworkPropertyMetadata(Brushes.Gold, OnMessageStyleChanged));

        /// <summary>
        /// The DependencyProperty for the MessageStrokeBrush property.
        /// <para>
        /// Flags:              Can be used in style rules
        /// </para>
        /// <para>
        /// Default Value:      <see cref="Brushes.Maroon"/>
        /// </para>
        /// </summary>
        public static readonly DependencyProperty MessageStrokeBrushProperty =
           DependencyProperty.Register("MessageStrokeBrush", typeof(Brush), typeof(SvgViewbox),
              new FrameworkPropertyMetadata(Brushes.Maroon, OnMessageStyleChanged));

        /// <summary>
        /// The <see cref="DependencyProperty"/> for the <c>AppName</c> property.
        /// </summary>
        public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register(
            "AppName", typeof(string), typeof(SvgViewbox), new FrameworkPropertyMetadata((string)null));

        #endregion

        #region Private Fields

        private const string DefaultTitle = "SharpVectors";

        private bool _isAutoSized;
        private bool _autoSize;
        private bool _textAsGeometry;
        private bool _includeRuntime;
        private bool _optimizePath;

        private bool _ensureViewboxPosition;
        private bool _ensureViewboxSize;
        private bool _ignoreRootViewbox;

        private DrawingGroup _svgDrawing;

        private string _appTitle;
        private CultureInfo _culture;

        private Uri _baseUri;
        private Uri _sourceUri;
        private string _sourceSvg;
        private Stream _sourceStream;

        private DpiScale _dpiScale;

        private SvgDrawingCanvas _drawingCanvas;

        private SvgInteractiveModes _interactiveMode;

        private event EventHandler<SvgAlertArgs> _svgAlerts;
        private event EventHandler<SvgErrorArgs> _svgErrors;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgViewbox"/> class.
        /// </summary>
        public SvgViewbox()
        {
            _textAsGeometry = false;
            _includeRuntime = true;
            _optimizePath   = true;
            _drawingCanvas  = new SvgDrawingCanvas();

            _appTitle       = DefaultTitle;

            this.Child      = _drawingCanvas;
        }

        /// <summary>
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        static SvgViewbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgViewbox),
                new FrameworkPropertyMetadata(typeof(SvgViewbox)));
        }

        #endregion

        #region Public Events

        /// <summary>
        /// This event occurs when an alert message needs to be displayed.
        /// </summary>
        /// <remarks>
        /// <para>If this event is not handled, the control may display the alert message using the standard message dialog.</para>
        /// <para>
        /// If you do not want to display the alert messages, handle this event and set <see cref="SvgAlertArgs.Handled"/> 
        /// property to <see langword="true"/>.
        /// </para>
        /// </remarks>
        public event EventHandler<SvgAlertArgs> Alert
        {
            add { _svgAlerts += value; }
            remove { _svgAlerts -= value; }
        }

        /// <summary>
        /// This event occurs when an error message needs to be displayed.
        /// </summary>
        /// <remarks>
        /// <para>If this event is not handled, the control may display the error message using the standard message dialog.</para>
        /// <para>
        /// If you do not want to display the error messages, handle this event and set <see cref="SvgErrorArgs.Handled"/> 
        /// property to <see langword="true"/>.
        /// </para>
        /// </remarks>
        public event EventHandler<SvgErrorArgs> Error
        {
            add { _svgErrors += value; }
            remove { _svgErrors -= value; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <c>name</c> of the parent assembly for this element.
        /// </summary>
        /// <value>
        /// A string containing the name of the parent assembly or the name of the assembly containing <c>SVG</c> file 
        /// referenced on this control in XAML, if the source type is <see cref="Uri"/>.
        /// </value>
        [Bindable(true), Category("Appearance")]
        public string AppName
        {
            get {
                return (string)GetValue(AppNameProperty);
            }
            set {
                this.SetValue(AppNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the application title, which is used to display the alert and error messages not handled
        /// by the user.
        /// </summary>
        /// <value>
        /// A string containg the application title. This cannot be <see langword="null"/> or empty. 
        /// The default is <c>SharpVectors</c>.
        /// </value>
        [DefaultValue(DefaultTitle)]
        [Description("The title of the application, used in displaying error and alert messages.")]
        public string AppTitle
        {
            get {
                return _appTitle;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _appTitle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the <c>SVG</c> file to load into this <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.Uri"/> specifying the path to the <c>SVG</c> source file. The file can be located on a computer, 
        /// network or assembly resources. Settings this to <see langword="null"/> will close any rendered <c>SVG</c> diagram.
        /// </value>
        /// <seealso cref="UriSource"/>
        /// <seealso cref="SvgSource"/>
        /// <seealso cref="StreamSource"/>
        public Uri Source
        {
            get {
                return (Uri)GetValue(SourceProperty);
            }
            set {
                _sourceUri = value;
                this.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path to the <c>SVG</c> file to load into this <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// A <see cref="Uri"/> specifying the path to the <c>SVG</c> source file. The file can be located on a computer, 
        /// network or assembly resources. Settings this to <see langword="null"/> will close any rendered <c>SVG</c> diagram.
        /// </value>
        /// <remarks>
        /// This is the same as the <see cref="Source"/> property, and added for consistency.
        /// </remarks>
        /// <seealso cref="SvgSource"/>
        /// <seealso cref="StreamSource"/>
        public Uri UriSource
        {
            get {
                return (Uri)GetValue(UriSourceProperty);
            }
            set {
                _sourceUri = value;
                this.SetValue(UriSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>SVG</c> contents to load into this <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the embedded <c>SVG</c> contents.
        /// Settings this to <see langword="null"/> will close any opened diagram.
        /// </value>
        /// <seealso cref="UriSource"/>
        /// <seealso cref="StreamSource"/>
        public string SvgSource
        {
            get {
                return (string)GetValue(SvgSourceProperty);
            }
            set {
                _sourceSvg = value;
                this.SetValue(SvgSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.Stream"/> to the SVG source to load into this <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.IO.Stream"/> specifying the stream to the SVG source.
        /// Settings this to <see langword="null"/> will close any opened diagram.
        /// </value>
        /// <remarks>
        /// <para>
        /// The stream source has precedence over the Uri <see cref="Source"/> property. 
        /// If set (not <see langword="null"/>), the stream source will be rendered instead of the Uri source.
        /// </para>
        /// <para>
        /// WPF controls do not implement the <see cref="IDisposable"/> interface and cannot properly dispose any
        /// stream set to it. To avoid this issue and also any problem of the user accidentally closing the stream,
        /// this control makes a copy of the stream to memory stream.
        /// </para>
        /// </remarks>
        /// <seealso cref="Source"/>
        /// <seealso cref="SvgSource"/>
        /// <seealso cref="UriSource"/>
        public Stream StreamSource
        {
            get {
                return (Stream)GetValue(StreamSourceProperty);
            }
            set {
                if (value == null)
                {
                    _sourceStream = null;
                    this.SetValue(StreamSourceProperty, null);
                }
                else
                {
                    MemoryStream svgStream = new MemoryStream();
                    // On dispose, the stream is close so copy it to the memory stream.
                    value.CopyTo(svgStream);

                    // Move the position to the start of the stream
                    svgStream.Seek(0, SeekOrigin.Begin);

                    _sourceStream = svgStream;
                    this.SetValue(StreamSourceProperty, svgStream);
                }
            }
        }

        /// <summary>
        /// Gets the drawing canvas, which is the child of this <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="SvgDrawingCanvas"/> specifying the child
        /// of this <see cref="Viewbox"/>, which handles the rendering.
        /// </value>
        public SvgDrawingCanvas DrawingCanvas
        {
            get {
                return _drawingCanvas;
            }
        }

        /// <summary>
        /// Gets the drawing from the SVG file conversion.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="DrawingGroup"/> specifying the converted drawings
        /// which is rendered in the canvas and displayed in the this viewbox.
        /// </value>
        public DrawingGroup Drawings
        {
            get {
                return _svgDrawing;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically resize this
        /// <see cref="Viewbox"/> based on the size of the loaded drawing.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this <see cref="Viewbox"/> is automatically resized based on the size of 
        /// the loaded drawing; otherwise, it is <see langword="false"/>. The default is <see langword="false"/>, and 
        /// the user-defined size or the parent assigned layout size is used.
        /// </value>
        public bool AutoSize
        {
            get {
                return _autoSize;
            }
            set {
                _autoSize = value;

                this.OnAutoSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the path geometry is 
        /// optimized using the <see cref="StreamGeometry"/>.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the path geometry is optimized using the <see cref="StreamGeometry"/>; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool OptimizePath
        {
            get {
                return _optimizePath;
            }
            set {
                _optimizePath = value;

                this.OnSettingsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texts are rendered as path geometry.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if texts are rendered as path geometries; otherwise, this is 
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The text features of the <c>SVG</c> format are complex and difficult to fully support without directly reverting
        /// the text paths (or glyph geometry). This options will, therefore, be removed in future versions of the library,
        /// as it is not always honored when set to <see langword="false"/>.
        /// </remarks>
        public bool TextAsGeometry
        {
            get {
                return _textAsGeometry;
            }
            set {
                _textAsGeometry = value;

                this.OnSettingsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <c>SharpVectors.Runtime.dll</c>
        /// classes are used in the generated output.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the <c>SharpVectors.Runtime.dll</c> classes and types are used in the 
        /// generated output; otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// The use of the <c>SharpVectors.Runtime.dll</c> prevents the hard-coded font path generated by the 
        /// <see cref="FormattedText"/> class, support for embedded images etc.
        /// </remarks>
        public bool IncludeRuntime
        {
            get {
                return _includeRuntime;
            }
            set {
                _includeRuntime = value;

                this.OnSettingsChanged();
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
        /// The library does not currently provide any means of splitting texts into its multi-language parts.
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

                    this.OnSettingsChanged();
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
        /// There are reported cases where are diagrams displayed in Inkscape program, but will not show when converted. 
        /// These are diagrams on the drawing canvas of Inkspace but outside the svg viewbox. 
        /// <para>
        /// When converted the drawings are also converted but not displayed due to clipping. Setting this property 
        /// to <see langword="true"/> will clear the clipping region on conversion.
        /// </para>
        /// </remarks>
        public bool IgnoreRootViewbox
        {
            get {
                return _ignoreRootViewbox;
            }
            set {
                _ignoreRootViewbox = value;

                this.OnAutoSizeChanged();
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
        /// box around the output, if a viewbox is defined. This will ensure that the original image size is saved.
        /// </remarks>
        public bool EnsureViewboxSize
        {
            get {
                return _ensureViewboxSize;
            }
            set {
                _ensureViewboxSize = value;

                this.OnAutoSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value to indicate applying a translate transform to the viewbox to ensure
        /// it is visible when rendered.
        /// </summary>
        /// <value>
        /// This determines whether a transformation is applied to the rendered drawing. For drawings where the top-left 
        /// position of the viewbox is off the screen, due to negative values, this will ensure the drawing is visible.
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

                this.OnAutoSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the font family of the desired font for the message text.
        /// </summary>
        /// <value>
        /// A <see cref="FontFamily"/> specifying the font for the message text.
        /// The default value is <see cref="SystemFonts.MessageFontFamily"/>.
        /// </value>
        public FontFamily MessageFontFamily
        {
            get { return (FontFamily)GetValue(MessageFontFamilyProperty); }
            set { SetValue(MessageFontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the desired font for the message text.
        /// </summary>
        /// <value>
        /// A value specifying the font size of the message text. The default is 48 pixels.
        ///  The font size must be a positive number.
        /// </value>
        public double MessageFontSize
        {
            get { return (double)GetValue(MessageFontSizeProperty); }
            set { SetValue(MessageFontSizeProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the opacity factor applied to the entire message text when it is 
        ///  rendered in the user interface (UI).
        /// </summary>
        /// <value>
        /// The opacity factor. Default opacity is 1.0. Expected values are between 0.0 and 1.0.
        /// </value>
        public double MessageOpacity
        {
            get { return (double)GetValue(MessageOpacityProperty); }
            set { SetValue(MessageOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> specifying the content of the message text.
        /// The default is "Loading...". This value can be overriden in the <see cref="Unload(bool,string)"/> method.
        /// </value>
        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a brush that describes the background of a message text.
        /// </summary>
        /// <value>
        /// A <see cref="Brush"/> specifying the brush that is used to fill the background of the 
        /// message text. The default is <see cref="Brushes.PapayaWhip"/>. If set to <see langword="null"/>,
        /// the background will not be drawn.
        /// </value>
        public Brush MessageBackground
        {
            get { return (Brush)GetValue(MessageBackgroundProperty); }
            set { SetValue(MessageBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush with which to fill the message text. 
        /// This is optional, and can be <see langword="null"/>. If the brush is <see langword="null"/>, no fill is drawn.
        /// </summary>
        /// <value>
        /// A <see cref="Brush"/> specifying the fill of the message text. The default is <see cref="Brushes.Gold"/>.
        /// </value>
        /// <remarks>
        /// If both the fill and stroke brushes of the message text are <see langword="null"/>, no text is drawn.
        /// </remarks>
        public Brush MessageFillBrush
        {
            get { return (Brush)GetValue(MessageFillBrushProperty); }
            set { SetValue(MessageFillBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush of the <see cref="Pen"/> with which to stroke the message text. 
        /// This is optional, and can be <see langword="null"/>. If the brush is <see langword="null"/>, no stroke is drawn.
        /// </summary>
        /// <value>
        /// A <see cref="Brush"/> specifying the brush of the <see cref="Pen"/> for stroking the message text. 
        /// The default is <see cref="Brushes.Maroon"/>.
        /// </value>
        /// <remarks>
        /// If both the fill and stroke brushes of the message text are <see langword="null"/>, no text is drawn.
        /// </remarks>
        public Brush MessageStrokeBrush
        {
            get { return (Brush)GetValue(MessageStrokeBrushProperty); }
            set { SetValue(MessageStrokeBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value specifying the interactive mode, which controls the level of information attached
        /// to the generated drawing.
        /// </summary>
        /// <value>An enumeration of the type <see cref="SvgInteractiveModes"/> specifying the interactive mode.
        /// The default is <see cref="SvgInteractiveModes.None"/>; no interactivity and may change in the future.</value>
        public SvgInteractiveModes InteractiveMode
        {
            get {
                return _interactiveMode;
            }
            set {
                _interactiveMode = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by using the supplied Uniform Resource Identifier (URI)
        /// and optionally processing the result asynchronously.
        /// </summary>
        /// <param name="uriSource">A reference to the SVG source file.</param>
        /// <param name="useAsync">
        /// A value indicating whether to process the result asynchronously. The default value is <see langword="false"/>,
        /// the SVG conversion is processed synchronously.
        /// </param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        public bool Load(Uri uriSource, bool useAsync = false)
        {
            if (uriSource == null)
            {
                return false;
            }

            var sourceUri = this.ResolveUri(uriSource);

            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                if (useAsync)
                {
                    MemoryStream drawingStream = new MemoryStream();

                    // Get the UI thread's context
                    var context = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() =>
                    {
                        DrawingGroup drawing = this.CreateDrawing(sourceUri, settings);
                        if (drawing != null)
                        {
                            _sourceUri = sourceUri;
                            _sourceStream = null;

                            XamlWriter.Save(drawing, drawingStream);
                            drawingStream.Seek(0, SeekOrigin.Begin);
                        }
                    }).ContinueWith((t) => {
                        if (drawingStream.Length != 0)
                        {
                            DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                            this.OnLoadDrawing(drawing);
                        }
                    }, context);

                    return true;
                }
                else
                {
                    DrawingGroup drawing = this.CreateDrawing(uriSource, settings);
                    if (drawing != null)
                    {
                        _sourceUri    = uriSource;
                        _sourceSvg    = null;
                        _sourceStream = null;

                        this.OnLoadDrawing(drawing);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return false;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by accessing text contents 
        /// and optionally processing the result asynchronously.
        /// </summary>
        /// <param name="svgSource">The stream source that sets the SVG source value.</param>
        /// <param name="useCopyStream">
        /// A value specifying whether to use a copy of the stream. The default is <see langword="true"/>,
        /// the SVG source stream is copied, rendered and stored.
        /// </param>
        /// <param name="useAsync">
        /// A value indicating whether to process the result asynchronously. The default value is <see langword="false"/>,
        /// the SVG conversion is processed synchronously.
        /// </param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        public bool Load(string svgSource, bool useAsync = false)
        {
            if (string.IsNullOrWhiteSpace(svgSource))
            {
                return false;
            }
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                _sourceUri    = null;
                _sourceSvg    = svgSource;
                _sourceStream = null;

                if (useAsync)
                {
                    MemoryStream drawingStream = new MemoryStream();

                    // Get the UI thread's context
                    var context = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() =>
                    {
                        DrawingGroup drawing = this.CreateDrawing(svgSource, settings);
                        if (drawing != null)
                        {
                            XamlWriter.Save(drawing, drawingStream);
                            drawingStream.Seek(0, SeekOrigin.Begin);
                        }
                    }).ContinueWith((t) => {
                        if (drawingStream.Length != 0)
                        {
                            DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                            this.OnLoadDrawing(drawing);
                        }
                    }, context);

                    return true;
                }
                else
                {
                    DrawingGroup drawing = this.CreateDrawing(svgSource, settings);
                    if (drawing != null)
                    {
                        this.OnLoadDrawing(drawing);

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return false;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by accessing a stream 
        /// and optionally processing the result asynchronously.
        /// </summary>
        /// <param name="streamSource">The stream source that sets the SVG source value.</param>
        /// <param name="useCopyStream">
        /// A value specifying whether to use a copy of the stream. The default is <see langword="true"/>,
        /// the SVG source stream is copied, rendered and stored.
        /// </param>
        /// <param name="useAsync">
        /// A value indicating whether to process the result asynchronously. The default value is <see langword="false"/>,
        /// the SVG conversion is processed synchronously.
        /// </param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The control will by default create a copy of the source stream to prevent any effect of disposing.
        /// If the source stream is stored, then use the <paramref name="useCopyStream"/> to prevent the control
        /// from creating its own copy.
        /// </remarks>
        public bool Load(Stream streamSource, bool useCopyStream = true, bool useAsync = false)
        {
            if (streamSource == null)
            {
                return false;
            }
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                Stream svgStream = streamSource;
                if (useCopyStream)
                {
                    svgStream = new MemoryStream();
                    // On dispose, the stream is close so copy it to the memory stream.
                    streamSource.CopyTo(svgStream); //NOTE: Not working within the task scope

                    // Move the position to the start of the stream
                    svgStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    if (svgStream.CanSeek && svgStream.Position != 0)
                    {
                        // Move the position to the start of the stream
                        svgStream.Seek(0, SeekOrigin.Begin);
                    }
                }

                _sourceUri    = null;
                _sourceSvg    = null;
                _sourceStream = svgStream;

                if (useAsync)
                {
                    MemoryStream drawingStream = new MemoryStream();

                    // Get the UI thread's context
                    var context = TaskScheduler.FromCurrentSynchronizationContext();

                    Task.Factory.StartNew(() =>
                    {
                        DrawingGroup drawing = this.CreateDrawing(svgStream, settings);
                        if (drawing != null)
                        {
                            XamlWriter.Save(drawing, drawingStream);
                            drawingStream.Seek(0, SeekOrigin.Begin);
                        }
                    }).ContinueWith((t) => {
                        if (drawingStream.Length != 0)
                        {
                            DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                            this.OnLoadDrawing(drawing);
                        }
                    }, context);

                    return true;
                }
                else
                {
                    DrawingGroup drawing = this.CreateDrawing(svgStream, settings);
                    if (drawing != null)
                    {
                        this.OnLoadDrawing(drawing);

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return false;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by using the supplied Uniform Resource Identifier (URI)
        /// and processing the result asynchronously.
        /// </summary>
        /// <param name="uriSource">A reference to the SVG source file.</param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        public Task<bool> LoadAsync(Uri uriSource)
        {
            if (_dpiScale == null)
            {
                _dpiScale = DpiUtilities.GetWindowScale(this);
            }
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            if (uriSource == null)
            {
                result.SetResult(false);
                return result.Task;
            }
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                MemoryStream drawingStream = new MemoryStream();

                // Get the UI thread's context
                var context = TaskScheduler.FromCurrentSynchronizationContext();

                return Task.Factory.StartNew<bool>(() =>
                {
                    DrawingGroup drawing = this.CreateDrawing(uriSource, settings);
                    if (drawing != null)
                    {
                        _sourceUri    = uriSource;
                        _sourceSvg    = null;
                        _sourceStream = null;

                        XamlWriter.Save(drawing, drawingStream);
                        drawingStream.Seek(0, SeekOrigin.Begin);

                        return true;
                    }
                    return false;
                }).ContinueWith((t) => {
                    if (t.Result && drawingStream.Length != 0)
                    {
                        DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                        this.OnLoadDrawing(drawing);

                        return true;
                    }
                    return false;
                }, context);
            }
            catch (Exception ex)
            {
                result.SetResult(false);
                result.SetException(ex);

                return result.Task;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by accessing text contents 
        /// and processing the result asynchronously.
        /// </summary>
        /// <param name="svgSource">The stream source that sets the SVG source value.</param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        public Task<bool> LoadAsync(string svgSource)
        {
            if (_dpiScale == null)
            {
                _dpiScale = DpiUtilities.GetWindowScale(this);
            }

            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            if (string.IsNullOrWhiteSpace(svgSource))
            {
                result.SetResult(false);
                return result.Task;
            }
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                _sourceUri    = null;
                _sourceSvg    = svgSource;
                _sourceStream = null;

                MemoryStream drawingStream = new MemoryStream();

                // Get the UI thread's context
                var context = TaskScheduler.FromCurrentSynchronizationContext();

                return Task.Factory.StartNew<bool>(() =>
                {
                    DrawingGroup drawing = this.CreateDrawing(svgSource, settings);
                    if (drawing != null)
                    {
                        XamlWriter.Save(drawing, drawingStream);
                        drawingStream.Seek(0, SeekOrigin.Begin);

                        return true;
                    }
                    return false;
                }).ContinueWith((t) => {
                    if (t.Result && drawingStream.Length != 0)
                    {
                        DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                        this.OnLoadDrawing(drawing);

                        return true;
                    }

                    return false;
                }, context);
            }
            catch (Exception ex)
            {
                result.SetResult(false);
                result.SetException(ex);

                return result.Task;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgViewbox"/> by accessing a stream 
        /// and processing the result asynchronously.
        /// </summary>
        /// <param name="streamSource">The stream source that sets the SVG source value.</param>
        /// <param name="useCopyStream">
        /// A value specifying whether to use a copy of the stream. The default is <see langword="true"/>,
        /// the SVG source stream is copied, rendered and stored.
        /// </param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The control will by default create a copy of the source stream to prevent any effect of disposing.
        /// If the source stream is stored, then use the <paramref name="useCopyStream"/> to prevent the control
        /// from creating its own copy.
        /// </remarks>
        public Task<bool> LoadAsync(Stream streamSource, bool useCopyStream = true)
        {
            if (_dpiScale == null)
            {
                _dpiScale = DpiUtilities.GetWindowScale(this);
            }
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            if (streamSource == null)
            {
                result.SetResult(false);
                return result.Task;
            }
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                Stream svgStream = streamSource;
                if (useCopyStream)
                {
                    svgStream = new MemoryStream();
                    // On dispose, the stream is close so copy it to the memory stream.
                    streamSource.CopyTo(svgStream); //NOTE: Not working within the task scope

                    // Move the position to the start of the stream
                    svgStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    if (svgStream.CanSeek && svgStream.Position != 0)
                    {
                        // Move the position to the start of the stream
                        svgStream.Seek(0, SeekOrigin.Begin);
                    }
                }

                _sourceUri    = null;
                _sourceSvg    = null;
                _sourceStream = svgStream;

                MemoryStream drawingStream = new MemoryStream();

                // Get the UI thread's context
                var context = TaskScheduler.FromCurrentSynchronizationContext();

                return Task.Factory.StartNew<bool>(() =>
                {
                    DrawingGroup drawing = this.CreateDrawing(svgStream, settings);
                    if (drawing != null)
                    {
                        XamlWriter.Save(drawing, drawingStream);
                        drawingStream.Seek(0, SeekOrigin.Begin);

                        return true;
                    }
                    return false;
                }).ContinueWith((t) => {
                    if (t.Result && drawingStream.Length != 0)
                    {
                        DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                        this.OnLoadDrawing(drawing);

                        return true;
                    }

                    return false;
                }, context);
            }
            catch (Exception ex)
            {
                result.SetResult(false);
                result.SetException(ex);

                return result.Task;
            }
        }

        /// <summary>
        /// This clears the <see cref="SvgViewbox"/> of any drawn diagram and optionally displays a 
        /// message.
        /// </summary>
        /// <param name="displayMessage">
        /// A value indicating whether to display a message after clearing the SVG rendered diagram.
        /// The value is <see langword="false"/>, not message is displayed.
        /// </param>
        /// <param name="message">
        /// This specifies the message to be displayed after clearing the diagram. Setting this parameter
        /// to a non-empty text will override any message set in the <see cref="MessageText"/>.
        /// The default value is <see cref="string.Empty"/>.
        /// </param>
        public void Unload(bool displayMessage = false, string message = "")
        {
            try
            {
                _sourceUri    = null;
                _sourceSvg    = null;
                _sourceStream = null;

                this.OnUnloadDiagram();

                _svgDrawing = null;

                if (_drawingCanvas != null)
                {
                    var messageText = this.MessageText;
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        messageText = message;
                    }

                    if (displayMessage && !string.IsNullOrWhiteSpace(messageText))
                    {
                        var messageDrawing = this.CreateMessageText(messageText);
                        if (messageDrawing != null)
                        {
                            _drawingCanvas.RenderDiagrams(messageDrawing);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the rendering settings or options to be used in rendering the SVG in this control.
        /// </summary>
        /// <returns> 
        /// An instance of <see cref="WpfDrawingSettings"/> specifying the rendering options or settings.
        /// </returns>
        protected virtual WpfDrawingSettings GetDrawingSettings()
        {
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath = _optimizePath;

            settings.IgnoreRootViewbox = _ignoreRootViewbox;
            settings.EnsureViewboxSize = _ensureViewboxSize;
            settings.EnsureViewboxPosition = _ensureViewboxPosition;

            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

            return settings;
        }

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized is set to true.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            if (this.Child != _drawingCanvas)
            {
                this.Child = _drawingCanvas;
            }

            base.OnInitialized(e);

            if (_sourceUri != null || _sourceStream != null || !string.IsNullOrWhiteSpace(_sourceSvg))
            {
                if (_svgDrawing == null)
                {
                    DrawingGroup drawing = this.CreateDrawing();
                    if (drawing != null)
                    {
                        this.OnLoadDrawing(drawing);
                    }
                }
            }
        }

        /// <summary>
        /// This handles changes in the rendering settings of this control.
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
            if (!this.IsInitialized || (_sourceUri == null &&
                _sourceStream == null && string.IsNullOrWhiteSpace(_sourceSvg)))
            {
                return;
            }

            DrawingGroup drawing = this.CreateDrawing();
            if (drawing != null)
            {
                this.OnLoadDrawing(drawing);
            }
        }

        /// <summary>
        /// This handles changes in the automatic resizing property of this control.
        /// </summary>
        protected virtual void OnAutoSizeChanged()
        {
            if (_autoSize)
            {
                if (this.IsInitialized && _svgDrawing != null)
                {
                    Rect rectDrawing = _svgDrawing.Bounds;
                    if (!rectDrawing.IsEmpty)
                    {
                        this.Width  = rectDrawing.Width;
                        this.Height = rectDrawing.Height;

                        _isAutoSized = true;
                    }
                }
            }
            else
            {
                if (_isAutoSized)
                {
                    this.Width  = double.NaN;
                    this.Height = double.NaN;
                }
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing()
        {
            WpfDrawingSettings settings = this.GetDrawingSettings();

            try
            {
                // 1. Load from the stream. The stream source has precedence
                if (_sourceStream != null)
                {
                    return this.CreateDrawing(_sourceStream, settings);
                }

                // 2. Load from the Uri, if available
                Uri svgSource = this.GetAbsoluteUri();
                if (svgSource != null)
                {
                    return this.CreateDrawing(svgSource, settings);
                }

                // 3. Load embedded SVG contents...
                if (!string.IsNullOrWhiteSpace(_sourceSvg))
                {
                    return this.CreateDrawing(_sourceSvg, settings);
                }

                return null;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return null;
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source file to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> defining the path to the SVG source.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(Uri svgSource, WpfDrawingSettings settings)
        {
            if (svgSource == null)
            {
                return null;
            }
            if (settings != null)
            {
                if (_dpiScale == null)
                {
                    _dpiScale = DpiUtilities.GetWindowScale(this);
                }
                settings.DpiScale = _dpiScale;
            }

            string scheme = null;
            if (this.DesignMode && svgSource.IsAbsoluteUri == false)
            {
                scheme = "pack";
            }
            else if (svgSource.IsAbsoluteUri == false)
            {
                scheme = "pack"; //TODO
            }
            else
            {
                scheme = svgSource.Scheme;
            }
            if (string.IsNullOrWhiteSpace(scheme))
            {
                return null;
            }

            try
            {
                var comparer = StringComparison.OrdinalIgnoreCase;

                DrawingGroup drawing = null;

                switch (scheme)
                {
                    case "file":
                    //case "ftp":
                    case "https":
                    case "http":
                        using (FileSvgReader reader = new FileSvgReader(settings))
                        {
                            drawing = reader.Read(svgSource);
                        }
                        break;
                    case "pack":
                        StreamResourceInfo svgStreamInfo = null;
                        if (svgSource.ToString().IndexOf("siteoforigin", comparer) >= 0)
                        {
                            svgStreamInfo = Application.GetRemoteStream(svgSource);
                        }
                        else
                        {
                            svgStreamInfo = Application.GetResourceStream(svgSource);
                        }

                        Stream svgStream = (svgStreamInfo != null) ? svgStreamInfo.Stream : null;

                        if (svgStream != null)
                        {
                            string fileExt = Path.GetExtension(svgSource.ToString());
                            bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                                string.Equals(fileExt, ".svgz", comparer);

                            if (isCompressed)
                            {
                                using (svgStream)
                                {
                                    using (GZipStream zipStream = new GZipStream(svgStream, CompressionMode.Decompress))
                                    {
                                        using (FileSvgReader reader = new FileSvgReader(settings))
                                        {
                                            drawing = reader.Read(zipStream);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (svgStream)
                                {
                                    using (FileSvgReader reader = new FileSvgReader(settings))
                                    {
                                        drawing = reader.Read(svgStream);
                                    }
                                }
                            }
                        }
                        break;
                    case "data":
                        var sourceData = svgSource.OriginalString.Replace(" ", string.Empty);

                        int nColon = sourceData.IndexOf(":", comparer);
                        int nSemiColon = sourceData.IndexOf(";", comparer);
                        int nComma = sourceData.IndexOf(",", comparer);

                        string sMimeType = sourceData.Substring(nColon + 1, nSemiColon - nColon - 1);
                        string sEncoding = sourceData.Substring(nSemiColon + 1, nComma - nSemiColon - 1);

                        if (string.Equals(sMimeType.Trim(), "image/svg+xml", comparer)
                            && string.Equals(sEncoding.Trim(), "base64", comparer))
                        {
                            string sContent = SvgObject.RemoveWhitespace(sourceData.Substring(nComma + 1));
                            byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                                0, sContent.Length);
                            bool isGZiped = sContent.StartsWith(SvgConstants.GZipSignature, StringComparison.Ordinal);
                            if (isGZiped)
                            {
                                using (var stream = new MemoryStream(imageBytes))
                                {
                                    using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                                    {
                                        using (var reader = new FileSvgReader(settings))
                                        {
                                            drawing = reader.Read(zipStream);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var stream = new MemoryStream(imageBytes))
                                {
                                    using (var reader = new FileSvgReader(settings))
                                    {
                                        drawing = reader.Read(stream);
                                    }
                                }
                            }
                        }
                        break;
                }

                return drawing;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return null;
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source stream to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgStream">A stream providing access to the SVG source data.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(Stream svgStream, WpfDrawingSettings settings)
        {
            try
            {
                if (svgStream == null)
                {
                    return null;
                }
                if (svgStream.CanSeek && svgStream.Position != 0)
                {
                    // Move the position to the start of the stream
                    svgStream.Seek(0, SeekOrigin.Begin);
                }

                DrawingGroup drawing = null;

                using (FileSvgReader reader = new FileSvgReader(settings))
                {
                    drawing = reader.Read(svgStream);
                }

                return drawing;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return null;
            }
        }

        /// <summary>
        /// Performs the conversion of a valid SVG source stream to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A stream providing access to the SVG source data.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(string svgSource, WpfDrawingSettings settings)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(svgSource))
                {
                    return null;
                }

                DrawingGroup drawing = null;

                var svgContent = svgSource.Trim();

                var cdataStart = "<![CDATA[";
                var cdataEnd = "]]>";

                if (svgContent.StartsWith(cdataStart, StringComparison.OrdinalIgnoreCase) ||
                    svgContent.EndsWith(cdataEnd, StringComparison.OrdinalIgnoreCase))
                {
                    var xmlDoc = XDocument.Parse(svgSource);
                    var cdataElement = xmlDoc.DescendantNodes().OfType<XCData>().FirstOrDefault();
                    if (cdataElement != null)
                    {
                        svgContent = cdataElement.Value;
                    }
                }

                using (FileSvgReader reader = new FileSvgReader(settings))
                {
                    var textReader = new StringReader(svgContent);
                    drawing = reader.Read(textReader);

                    textReader.Dispose();
                }

                return drawing;
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
                return null;
            }
        }

        protected virtual void OnHandleAlert(string message)
        {
            if (this.DesignMode)
            {
                return;
            }
            var alertArgs = new SvgAlertArgs(message);
            _svgAlerts?.Invoke(this, alertArgs);
            if (!alertArgs.Handled)
            {
                MessageBox.Show(alertArgs.Message, _appTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected virtual void OnHandleError(string message, Exception exception)
        {
            if (this.DesignMode)
            {
                return;
            }
            var errorArgs = new SvgErrorArgs(message, exception);
            _svgErrors?.Invoke(this, errorArgs);
            if (!errorArgs.Handled)
            {
                throw new SvgErrorException(errorArgs);
            }
        }

        #endregion

        #region Private Methods

        private void OnLoadDrawing(DrawingGroup drawing)
        {
            try
            {
                if (drawing == null || _drawingCanvas == null)
                {
                    return;
                }

                // Clear the current drawing
                this.OnUnloadDiagram();

                // Allow the contained canvas to render the object.
                _drawingCanvas.RenderDiagrams(drawing);

                // Pass any tooltip object to the contained canvas
                _drawingCanvas.ToolTip = this.ToolTip;

                // Keep an instance of the current drawing
                _svgDrawing = drawing;

                // Finally, force a resize of the viewbox
                this.OnAutoSizeChanged();
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
            }
        }

        private void OnUnloadDiagram()
        {
            try
            {
                if (_drawingCanvas != null)
                {
                    _drawingCanvas.UnloadDiagrams();

                    if (_isAutoSized)
                    {
                        this.Width  = double.NaN;
                        this.Height = double.NaN;
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnHandleError(null, ex);
            }
        }

        private Uri GetAbsoluteUri()
        {
            if (_sourceUri == null)
            {
                return null;
            }

            return this.ResolveUri(_sourceUri);
        }

        private Uri ResolveUri(Uri svgSource)
        {
            if (svgSource == null)
            {
                return null;
            }

            if (svgSource.IsAbsoluteUri)
            {
                return svgSource;
            }

            // Try getting a local file in the same directory....
            string svgPath = svgSource.ToString();
            if (svgPath[0] == '\\' || svgPath[0] == '/')
            {
                svgPath = svgPath.Substring(1);
            }
            svgPath = svgPath.Replace('/', '\\');

            Assembly assembly = this.GetExecutingAssembly(); // Assembly.GetExecutingAssembly();
            string localFile = PathUtils.Combine(assembly, svgPath);

            if (File.Exists(localFile))
            {
                return new Uri(localFile);
            }

            // Try getting it as resource file...
            if (_baseUri != null)
            {
                var validUri = new Uri(_baseUri, svgSource);
                if (validUri.IsAbsoluteUri)
                {
                    if (validUri.IsFile && File.Exists(validUri.LocalPath))
                    {
                        return validUri;
                    }
                }
            }
            var baseUri = System.Windows.Navigation.BaseUriHelper.GetBaseUri(this);
            if (baseUri != null)
            {
                var contextUri = new Uri(baseUri, svgSource);
                if (contextUri.IsFile && File.Exists(contextUri.LocalPath))
                {
                    return contextUri;
                }
            }

            string asmName = this.AppName;
            if (string.IsNullOrWhiteSpace(asmName))
            {
                if (assembly != null)
                {
                    asmName = assembly.GetName().Name;
                }
                else
                {
                    asmName = this.GetAppName();
                }
            }
            else
            {
                string uriDesign = string.Format("/{0};component/{1}", asmName, svgPath);

                return new Uri(uriDesign, UriKind.Relative);
            }

            if (this.DesignMode)
            {
                string uriDesign = string.Format("/{0};component/{1}", asmName, svgPath);

                return new Uri(uriDesign, UriKind.Relative);
            }

            string uriString = string.Format("pack://application:,,,/{0};component/{1}",
                asmName, svgPath);
            if (!WpfResources.ResourceExists(assembly, svgPath))
            {
                return null;
            }
            return new Uri(uriString);
        }

        // Convert the text string to a geometry and draw it to the control's DrawingContext.
        private DrawingGroup CreateMessageText(string messageText)
        {
            double opacity    = this.MessageOpacity;

            var fontFamily    = this.MessageFontFamily;
            var fontSize      = this.MessageFontSize;
            Brush fillBrush   = this.MessageFillBrush;
            Brush strokeBrush = this.MessageStrokeBrush;
            if ((fillBrush == null && strokeBrush == null) || opacity <= 0 
                || fontFamily == null || fontSize <= 3)
            {
                return null;
            }
            if (strokeBrush == null)
            {
                strokeBrush = Brushes.Transparent;
            }

            if (_dpiScale == null)
            {
                _dpiScale = DpiUtilities.GetWindowScale(this);
            }

            // Create a new DrawingGroup of the control.
            DrawingGroup drawingGroup = new DrawingGroup();

            drawingGroup.Opacity = opacity;

            // Open the DrawingGroup in order to access the DrawingContext.
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                // Create the formatted text based on the properties set.
                FormattedText formattedText = null;
#if DOTNET40 || DOTNET45 || DOTNET46
                formattedText = new FormattedText(messageText,
                    CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    this.MessageFontSize, Brushes.Black);
#else
                formattedText = new FormattedText(messageText,
                    CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                    this.MessageFontSize, Brushes.Black, _dpiScale.PixelsPerDip);
#endif

                // Build the geometry object that represents the text.
                Geometry textGeometry = formattedText.BuildGeometry(new Point(20, 0));

                // Draw a rounded rectangle under the text that is slightly larger than the text.
                var backgroundBrush = this.MessageBackground;
                if (backgroundBrush != null)
                {
                    drawingContext.DrawRoundedRectangle(backgroundBrush, null,
                        new Rect(new Size(formattedText.Width + 50, formattedText.Height + 5)), 5.0, 5.0);
                }

                // Draw the outline based on the properties that are set.
                drawingContext.DrawGeometry(MessageFillBrush, new Pen(MessageStrokeBrush, 1.5), textGeometry);

                // Return the updated DrawingGroup content to be used by the control.
                return drawingGroup;
            }
        }

        private static void OnUriSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgViewbox viewbox = obj as SvgViewbox;
            if (viewbox == null)
            {
                return;
            }

            viewbox._sourceUri = (Uri)args.NewValue;
            if (viewbox._sourceUri == null)
            {
                viewbox.OnUnloadDiagram();
            }
            else
            {
                viewbox.OnSettingsChanged();
            }
        }

        private static void OnSvgSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgViewbox viewbox = obj as SvgViewbox;
            if (viewbox == null)
            {
                return;
            }

            viewbox._sourceSvg = (string)args.NewValue;
            if (string.IsNullOrWhiteSpace(viewbox._sourceSvg))
            {
                viewbox.OnUnloadDiagram();
            }
            else
            {
                viewbox.OnSettingsChanged();
            }
        }

        private static void OnStreamSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgViewbox viewbox = obj as SvgViewbox;
            if (viewbox == null)
            {
                return;
            }

            viewbox._sourceStream = (Stream)args.NewValue;
            if (viewbox._sourceStream == null)
            {
                viewbox.OnUnloadDiagram();
            }
            else
            {
                viewbox.OnSettingsChanged();
            }
        }

        private static void OnMessageStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SvgViewbox viewbox = d as SvgViewbox;

            viewbox.InvalidateVisual();
        }

        private string GetAppName()
        {
            try
            {
                Assembly asm = this.GetEntryAssembly();

                if (asm != null)
                {
                    return asm.GetName().Name;
                }
            }
            catch
            {
                // Issue #125
            }
            return null;
        }

        private Assembly GetEntryAssembly()
        {
            string XDesProc = "XDesProc";   // WPF designer process - Designer Isolation
            string DevEnv = "DevEnv";     // WPF designer process - Surface Isolation
            string WpfSurface = "WpfSurface"; // WPF designer process - New .NETCore
            var comparer = StringComparison.OrdinalIgnoreCase;

            Assembly asm = null;
            try
            {
                // Should work at runtime...
                asm = Assembly.GetEntryAssembly(); //...but mostly loading the design-time process: XDesProc.exe
                if (asm != null)
                {
                    var appName = asm.GetName().Name;
                    if (appName.StartsWith(XDesProc, comparer)
                        || appName.StartsWith(DevEnv, comparer)
                        || appName.StartsWith(WpfSurface, comparer))
                    {
                        asm = null;
                    }
                }
                // Design time
                if (asm == null)
                {
#if NETCORE
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = SharpVectors.Dom.Utils.PathUtils.GetAssemblyFileName(assembly).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#else
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = Path.GetFileName(assembly.CodeBase).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#endif

                    if (asm == null)
                    {
                        asm = Application.ResourceAssembly;
                        if (asm != null)
                        {
                            var appName = asm.GetName().Name;
                            if (appName.StartsWith(XDesProc, comparer) || appName.StartsWith(DevEnv, comparer))
                            {
                                asm = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (asm == null)
                {
#if NETCORE
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = SharpVectors.Dom.Utils.PathUtils.GetAssemblyFileName(assembly).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#else
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assmName = Path.GetFileName(assembly.CodeBase).Trim()
                          where assmName.EndsWith(".exe", comparer)
                              && !assmName.StartsWith(XDesProc, comparer) // should not be XDesProc.exe
                              && !assmName.StartsWith(DevEnv, comparer)   // should not be DevEnv.exe
                              && !assmName.StartsWith(WpfSurface, comparer)   // should not be WpfSurface.exe
                          select assembly
                          ).FirstOrDefault();
#endif
                }

                Trace.TraceError(ex.ToString());
            }
            return asm;
        }

        private Assembly GetExecutingAssembly()
        {
            Assembly asm = null;
            try
            {
                var invalidAssemblies = new string[] { "SharpVectors.Converters.Wpf", "WpfSurface", "XDesProc", "DevEnv"};

                asm = Assembly.GetExecutingAssembly();
                string asmName = asm == null ? null : Path.GetFileName(asm.GetName().Name);
                if (asmName != null && !invalidAssemblies.Contains(asmName, StringComparer.OrdinalIgnoreCase))
                {
                    return asm;
                }
                else
                {
                    asm = Assembly.GetEntryAssembly();
                    asmName = asm == null ? null : Path.GetFileName(asm.GetName().Name);
                    if (asmName != null && !invalidAssemblies.Contains(asmName, StringComparer.OrdinalIgnoreCase))
                    {
                        return asm;
                    }
                }

                return this.GetEntryAssembly();
            }
            catch
            {
                asm = Assembly.GetEntryAssembly();
            }
            return asm;
        }

        #endregion

        #region IUriContext Members

        /// <summary>
        /// Gets or sets the base URI of the current application context.
        /// </summary>
        /// <value>
        /// The base URI of the application context.
        /// </value>
        public Uri BaseUri
        {
            get {
                return _baseUri;
            }
            set {
                _baseUri = value;
            }
        }

        #endregion

        #region ISvgControl Members

        /// <inheritdoc/>
        public bool DesignMode
        {
            get {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc/>
        int ISvgControl.Width
        {
            get {
                return (int)this.ActualWidth;
            }
        }

        /// <inheritdoc/>
        int ISvgControl.Height
        {
            get {
                return (int)this.ActualHeight;
            }
        }

        /// <inheritdoc/>
        void ISvgControl.HandleAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || this.DesignMode)
            {
                return;
            }
            this.OnHandleAlert(message);
        }

        /// <inheritdoc/>
        void ISvgControl.HandleError(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || this.DesignMode)
            {
                return;
            }
            this.OnHandleError(message, null);
        }

        /// <inheritdoc/>
        void ISvgControl.HandleError(Exception exception)
        {
            if (exception == null || this.DesignMode)
            {
                return;
            }
            this.OnHandleError(null, exception);
        }

        /// <inheritdoc/>
        void ISvgControl.HandleError(string message, Exception exception)
        {
            if ((string.IsNullOrWhiteSpace(message) && exception == null) || this.DesignMode)
            {
                return;
            }
            this.OnHandleError(message, exception);
        }

        #endregion
    }
}
