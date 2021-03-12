using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;

using DpiScale     = SharpVectors.Runtime.DpiScale;
using DpiUtilities = SharpVectors.Runtime.DpiUtilities;

namespace SharpVectors.Converters
{
    public enum SvgControlSizeMode
    {
        None,
        ContentToSizeNoStretch,
        ContentToSizeStretch,
        SizeToContent
    }

    /// <summary>
    /// </summary>
    public class SvgControl : Control, IUriContext
    {
        #region Public Fields

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(SvgControl),
                new FrameworkPropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// Identifies the <see cref="StreamSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register("StreamSource", typeof(Stream), typeof(SvgControl),
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
           DependencyProperty.Register("MessageFontFamily", typeof(FontFamily), typeof(SvgControl),
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
           DependencyProperty.Register("MessageFontSize", typeof(double), typeof(SvgControl),
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
           DependencyProperty.Register("MessageOpacity", typeof(double), typeof(SvgControl),
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
           DependencyProperty.Register("MessageText", typeof(string), typeof(SvgControl), new
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
           DependencyProperty.Register("MessageBackground", typeof(Brush), typeof(SvgControl),
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
           DependencyProperty.Register("MessageFillBrush", typeof(Brush), typeof(SvgControl),
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
           DependencyProperty.Register("MessageStrokeBrush", typeof(Brush), typeof(SvgControl),
              new FrameworkPropertyMetadata(Brushes.Maroon, OnMessageStyleChanged));

        public static DependencyProperty SizeModeProperty = DependencyProperty.Register("SizeMode",
            typeof(SvgControlSizeMode), typeof(SvgControl), new FrameworkPropertyMetadata(SvgControlSizeMode.ContentToSizeNoStretch,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnSizeTypeChanged)));

        #endregion

        #region Private Fields

        private CultureInfo _culture;

        private Uri _baseUri;
        private Uri _sourceUri;
        private Stream _sourceStream;

        private DpiScale _dpiScale;

        private bool _isAutoSized;
        private bool _autoSize;
        private bool _textAsGeometry;
        private bool _includeRuntime;
        private bool _optimizePath;

        private DrawingGroup _renderedDrawing;
        private ScaleTransform _renderScale;
        private TranslateTransform _renderOffset;

        #endregion

        #region Constructors and Destructor

        public SvgControl()
        {
            this.ClipToBounds        = true;
            this.SnapsToDevicePixels = true;

            _renderScale  = new ScaleTransform();
            _renderOffset = new TranslateTransform();
        }

        static SvgControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgControl), 
                new FrameworkPropertyMetadata(typeof(SvgControl)));

            ClipToBoundsProperty.OverrideMetadata(typeof(SvgControl), new FrameworkPropertyMetadata(true));
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(SvgControl), new FrameworkPropertyMetadata(true));
        }

        #endregion

        #region Private Dependency Properties Callback

        private static void OnSizeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SvgControl ctrl = d as SvgControl;
            ctrl.RecalcImage();
        }

        #endregion

        #region Public Properties

        public SvgControlSizeMode SizeMode
        {
            get { return (SvgControlSizeMode)this.GetValue(SizeModeProperty); }
            set { this.SetValue(SizeModeProperty, value); }
        }

        public DrawingGroup RenderedDrawing
        {
            get {
                return _renderedDrawing;
            }
            protected set {
                _renderedDrawing = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to the SVG file to load into this 
        /// <see cref="Canvas"/>.
        /// </summary>
        /// <value>
        /// A <see cref="System.Uri"/> specifying the path to the SVG source file.
        /// The file can be located on a computer, network or assembly resources.
        /// Settings this to <see langword="null"/> will close any opened diagram.
        /// </value>
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
        /// Gets or sets the <see cref="System.IO.Stream"/> to the SVG source to load into this 
        /// <see cref="Canvas"/>.
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
        /// Gets or sets a value indicating whether to automatically resize this
        /// <see cref="Canvas"/> based on the size of the loaded drawing.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this <see cref="Canvas"/> is
        /// automatically resized based on the size of the loaded drawing;
        /// otherwise, it is <see langword="false"/>. The default is 
        /// <see langword="false"/>, and the user-defined size or the parent assigned
        /// layout size is used.
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

                this.OnSettingsChanged();
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

                this.OnSettingsChanged();
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

                    this.OnSettingsChanged();
                }
            }
        }

        /// <summary>
        /// Gets the drawing from the SVG file conversion.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="DrawingGroup"/> specifying the converted drawings
        /// which is rendered in this canvas.
        public DrawingGroup Drawings
        {
            get {
                return _renderedDrawing;
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

        #region Public Methods

        public void RenderDiagrams(DrawingGroup renderedGroup)
        {
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgControl"/> by using the supplied Uniform Resource Identifier (URI)
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

            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath   = _optimizePath;
            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

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
                            _sourceUri    = sourceUri;
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
                        _sourceStream = null;

                        this.OnLoadDrawing(drawing);

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                //TODO: Rethrow the exception?
                return false;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgControl"/> by accessing a stream 
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
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath   = _optimizePath;
            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

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
            catch
            {
                //TODO: Rethrow the exception?
                return false;
            }
        }

        /// <summary>
        /// This sets the source SVG for a <see cref="SvgControl"/> by using the supplied Uniform Resource Identifier (URI)
        /// and processing the result asynchronously.
        /// </summary>
        /// <param name="uriSource">A reference to the SVG source file.</param>
        /// <returns>
        /// A value that indicates whether the operation was successful. This is <see langword="true"/>
        /// if successful, otherwise, it is <see langword="false"/>.
        /// </returns>
        public Task<bool> LoadAsync(Uri uriSource)
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            if (uriSource == null)
            {
                result.SetResult(false);
                return result.Task;
            }
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath   = _optimizePath;
            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

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
        /// This sets the source SVG for a <see cref="SvgControl"/> by accessing a stream 
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
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            if (streamSource == null)
            {
                result.SetResult(false);
                return result.Task;
            }
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath   = _optimizePath;
            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

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
        /// This clears the <see cref="SvgControl"/> of any drawn diagram and optionally displays a 
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
            _sourceUri    = null;
            _sourceStream = null;

            this.OnUnloadDiagram();

            _renderedDrawing = null;

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
                    this.RenderDiagrams(messageDrawing);
                }
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized is set to true.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_sourceUri != null || _sourceStream != null)
            {
                if (_renderedDrawing == null)
                {
                    DrawingGroup drawing = this.CreateDrawing();
                    if (drawing != null)
                    {
                        this.OnLoadDrawing(drawing);
                    }
                }
            }
        }

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);
        //    //if (loadImage != null)
        //    //{
        //    //    _render = new SVGRender();
        //    //    _render.ExternalFileLoader = this.ExternalFileLoader;
        //    //    _render.OverrideColor = OverrideColor;
        //    //    _render.UseAnimations = this.UseAnimations;
        //    //    loadImage(_render);
        //    //    loadImage = null;
        //    //}
        //}

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.RecalcImage();
            this.InvalidateVisual();
        }

        // Notice TemplateBinding Background must be removed from the Border in the default template 
        // (or remove Border from the template) Border renders the background after the child render has been called
        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(dc);
            if (this.Background != null)
                drawingContext.DrawRectangle(this.Background, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            if (_renderedDrawing == null)
                return;

            drawingContext.PushTransform(_renderOffset);
            drawingContext.PushTransform(_renderScale);
            drawingContext.DrawDrawing(_renderedDrawing);
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size result = base.MeasureOverride(constraint);

            if (this.SizeMode == SvgControlSizeMode.SizeToContent)
            {
                if (_renderedDrawing != null && !_renderedDrawing.Bounds.Size.IsEmpty)
                {
                    result = _renderedDrawing.Bounds.Size;
                }
            }
            if (constraint.Width > 0 && constraint.Width < result.Width)
            {
                result.Width = constraint.Width;
            }
            if (constraint.Height > 0 && constraint.Height < result.Height)
            {
                result.Height = constraint.Height;
            }
            return result;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size result = base.ArrangeOverride(arrangeBounds);

            if (this.SizeMode == SvgControlSizeMode.SizeToContent)
            {
                if (_renderedDrawing != null && !_renderedDrawing.Bounds.Size.IsEmpty)
                {
                    result = _renderedDrawing.Bounds.Size;
                }
            }
            if (arrangeBounds.Width > 0 && arrangeBounds.Width < result.Width)
            {
                result.Width = arrangeBounds.Width;
            }
            if (arrangeBounds.Height > 0 && arrangeBounds.Height < result.Height)
            {
                result.Height = arrangeBounds.Height;
            }
            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This handles changes in the rendering settings of this control.
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
            if (!this.IsInitialized || (_sourceUri == null && _sourceStream == null))
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
                if (this.IsInitialized && _renderedDrawing != null)
                {
                    Rect rectDrawing = _renderedDrawing.Bounds;
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
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing()
        {
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = _includeRuntime;
            settings.TextAsGeometry = _textAsGeometry;
            settings.OptimizePath   = _optimizePath;
            if (_culture != null)
            {
                settings.CultureInfo = _culture;
            }

            try
            {
                // The stream source has precedence
                if (_sourceStream != null)
                {
                    return this.CreateDrawing(_sourceStream, settings);
                }

                Uri svgSource = this.GetAbsoluteUri();
                if (svgSource != null)
                {
                    return this.CreateDrawing(svgSource, settings);
                }

                return null;
            }
            catch
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return null;
                }

                throw;
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
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
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

            string scheme = svgSource.Scheme;
            if (string.IsNullOrWhiteSpace(scheme))
            {
                return null;
            }

            var comparer = StringComparison.OrdinalIgnoreCase;

            DrawingGroup drawing = null;

            switch (scheme)
            {
                case "file":
                //case "ftp":
                case "https":
                case "http":
                    using (var reader = new FileSvgReader(settings))
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
                                using (var zipStream = new GZipStream(svgStream, CompressionMode.Decompress))
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
                            using (svgStream)
                            {
                                using (var reader = new FileSvgReader(settings))
                                {
                                    drawing = reader.Read(svgStream);
                                }
                            }
                        }
                    }
                    break;
                case "data":
                    var sourceData = svgSource.OriginalString.Replace(" ", string.Empty);

                    int nColon     = sourceData.IndexOf(":", comparer);
                    int nSemiColon = sourceData.IndexOf(";", comparer);
                    int nComma     = sourceData.IndexOf(",", comparer);

                    string sMimeType = sourceData.Substring(nColon + 1, nSemiColon - nColon - 1);
                    string sEncoding = sourceData.Substring(nSemiColon + 1, nComma - nSemiColon - 1);

                    if (string.Equals(sMimeType.Trim(), "image/svg+xml", comparer)
                        && string.Equals(sEncoding.Trim(), "base64", comparer))
                    {
                        string sContent   = SvgObject.RemoveWhitespace(sourceData.Substring(nComma + 1));
                        byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                            0, sContent.Length);
                        bool isGZiped = sContent.StartsWith(SvgObject.GZipSignature, StringComparison.Ordinal);
                        if (isGZiped)
                        {
                            using (var stream = new MemoryStream(imageBytes))
                            {
                                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
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

        /// <summary>
        /// Performs the conversion of a valid SVG source stream to the <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgStream">A stream providing access to the SVG source data.</param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        /// <returns>
        /// This returns <see cref="DrawingGroup"/> if successful; otherwise, it
        /// returns <see langword="null"/>.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing(Stream svgStream, WpfDrawingSettings settings)
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

            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(svgStream);
            }

            return drawing;
        }

        #endregion

        #region Private Methods

        private void RecalcImage()
        {
            if (_renderedDrawing == null)
                return;

            var sizeType     = this.SizeMode;
            var actualWidth  = this.ActualWidth;
            var actualHeight = this.ActualHeight;

            Rect r = _renderedDrawing.Bounds;
            if (sizeType == SvgControlSizeMode.None)
            {
                _renderScale.ScaleX = 1;
                _renderScale.ScaleY = 1;
                switch (this.HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        _renderOffset.X = actualWidth / 2 - r.Width / 2 - r.Left;
                        break;
                    case HorizontalAlignment.Right:
                        _renderOffset.X = actualWidth - r.Right;
                        break;
                    default:
                        _renderOffset.X = -r.Left; // move to left by default
                        break;
                }
                switch (this.VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        _renderOffset.Y = actualHeight / 2 - r.Height / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        _renderOffset.Y = actualHeight - r.Height - r.Top;
                        break;
                    default:
                        _renderOffset.Y = -r.Top; // move to top by default
                        break;
                }
                return;
            }
            if (sizeType == SvgControlSizeMode.ContentToSizeNoStretch)
            {
                this.SizeToContentNoStretch(this.HorizontalContentAlignment, this.VerticalContentAlignment);
                return;
            }
            if (sizeType == SvgControlSizeMode.ContentToSizeStretch)
            {
                double xscale = actualWidth / r.Width;
                double yscale = actualHeight / r.Height;
                _renderScale.CenterX = r.Left;
                _renderScale.CenterY = r.Top;
                _renderScale.ScaleX  = xscale;
                _renderScale.ScaleY  = yscale;

                _renderOffset.X = -r.Left;
                _renderOffset.Y = -r.Top;
                return;
            }
            if (sizeType == SvgControlSizeMode.SizeToContent)
            {
                if (r.Width > actualWidth || r.Height > actualHeight)
                {
                    this.SizeToContentNoStretch(HorizontalAlignment.Left, VerticalAlignment.Top);
                }
                else
                {
                    _renderScale.CenterX = r.Left;
                    _renderScale.CenterY = r.Top;
                    _renderScale.ScaleX  = 1;
                    _renderScale.ScaleY  = 1;

                    _renderOffset.X = -r.Left; // move to left by default
                    _renderOffset.Y = -r.Top;  // move to top by default
                }
                return;
            }
        }

        private void SizeToContentNoStretch(HorizontalAlignment hAlignment, VerticalAlignment vAlignment)
        {
            var actualWidth  = this.ActualWidth;
            var actualHeight = this.ActualHeight;

            Rect r = _renderedDrawing.Bounds;
            double xscale = actualWidth / r.Width;
            double yscale = actualHeight / r.Height;
            double scale = xscale;
            if (scale > yscale)
            {
                scale = yscale;
            }

            _renderScale.CenterX = r.Left;
            _renderScale.CenterY = r.Top;
            _renderScale.ScaleX  = scale;
            _renderScale.ScaleY  = scale;

            _renderOffset.X = -r.Left;

            if (scale < xscale)
            {
                switch (this.HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        double width = r.Width * scale;
                        _renderOffset.X = actualWidth / 2 - width / 2 - r.Left;
                        break;
                    case HorizontalAlignment.Right:
                        _renderOffset.X = actualWidth - r.Right * scale;
                        break;
                }
            }

            _renderOffset.Y = -r.Top;

            if (scale < yscale)
            {
                switch (this.VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        double height = r.Height * scale;
                        _renderOffset.Y = actualHeight / 2 - height / 2 - r.Top;
                        break;
                    case VerticalAlignment.Bottom:
                        _renderOffset.Y = actualHeight - r.Height * scale - r.Top;
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnLoadDrawing(DrawingGroup drawing)
        {
            if (drawing == null)
            {
                return;
            }

            this.OnUnloadDiagram();

            this.RenderDiagrams(drawing);

            _renderedDrawing = drawing;

            this.OnAutoSizeChanged();
        }

        private void OnUnloadDiagram()
        {
            _renderedDrawing = null;

            _renderOffset = new TranslateTransform();
            _renderScale = new ScaleTransform();

            this.InvalidateMeasure();
            //            this.InvalidateVisual();
            this.UpdateLayout();

//            this.UnloadDiagrams();

            if (_isAutoSized)
            {
                this.Width  = double.NaN;
                this.Height = double.NaN;
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

            Assembly assembly = Assembly.GetExecutingAssembly();
            string localFile = Path.Combine(Path.GetDirectoryName(assembly.Location), svgPath);

            if (File.Exists(localFile))
            {
                return new Uri(localFile);
            }

            // Try getting it as resource file...
            if (_baseUri != null)
            {
                return new Uri(_baseUri, svgSource);
            }

            string asmName = assembly.GetName().Name;
            string uriString = string.Format("pack://application:,,,/{0};component/{1}",
                asmName, svgPath);

            return new Uri(uriString);
        }

        // Convert the text string to a geometry and draw it to the control's DrawingContext.
        private DrawingGroup CreateMessageText(string messageText)
        {
            double opacity = this.MessageOpacity;

            var fontFamily = this.MessageFontFamily;
            var fontSize = this.MessageFontSize;
            Brush fillBrush = this.MessageFillBrush;
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

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgControl svgCanvas = obj as SvgControl;
            if (svgCanvas == null)
            {
                return;
            }

            svgCanvas._sourceUri = (Uri)args.NewValue;
            if (svgCanvas._sourceUri == null)
            {
                svgCanvas.OnUnloadDiagram();
            }
            else
            {
                svgCanvas.OnSettingsChanged();
            }
        }

        private static void OnStreamSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SvgControl svgCanvas = obj as SvgControl;
            if (svgCanvas == null)
            {
                return;
            }

            svgCanvas._sourceStream = (Stream)args.NewValue;
            if (svgCanvas._sourceStream == null)
            {
                svgCanvas.OnUnloadDiagram();
            }
            else
            {
                svgCanvas.OnSettingsChanged();
            }
        }

        private static void OnMessageStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SvgControl canvas = d as SvgControl;

            canvas.InvalidateVisual();
        }

        #endregion
    }
}
