using System;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all SVG to WPF converters.
    /// </summary>
    public abstract class SvgConverter : DependencyObject, IDisposable
    {
        #region Public Constant Fields

        public const string SvgExt            = ".svg";
        public const string CompressedSvgExt  = ".svgz";

        public const string XamlExt           = ".xaml";
        public const string CompressedXamlExt = ".zaml";

        public const string BackupExt         = ".bak";

        #endregion

        #region Private Fields

        private bool _saveXaml;
        private bool _saveZaml;
        private bool _useFrameXamlWriter;

        protected bool _isEmbedded;

        protected SolidColorBrush _background;

        protected WpfDrawingSettings _wpfSettings;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        protected WpfSvgWindow _wpfWindow;
        protected WpfDrawingRenderer _wpfRenderer;

        #endregion

        #region Constructors and Destrutor

        /// <overloads>
        /// Initializes a new instance of the <see cref="SvgConverter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgConverter"/> class
        /// with the default parameters and settings.
        /// </summary>
        protected SvgConverter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgConverter"/> class
        /// with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        protected SvgConverter(WpfDrawingSettings settings)
        {
            _saveXaml    = true;
            _saveZaml    = false;
            _wpfSettings = settings;

            if (_wpfSettings == null)
            {
                _wpfSettings = new WpfDrawingSettings();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgConverter"/> class
        /// with the specified drawing or rendering settings and the saving options.
        /// </summary>
        /// <param name="saveXaml">
        /// This specifies whether to save result object tree in XAML file.
        /// </param>
        /// <param name="saveZaml">
        /// This specifies whether to save result object tree in ZAML file. The
        /// ZAML is simply a G-Zip compressed XAML format, similar to the SVGZ.
        /// </param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        protected SvgConverter(bool saveXaml, bool saveZaml, WpfDrawingSettings settings)
            : this(settings)
        {
            _saveXaml = saveXaml;
            _saveZaml = saveZaml;
        }

        /// <summary>
        /// This allows a converter to attempt to free resources and perform 
        /// other cleanup operations before the converter is reclaimed by 
        /// garbage collection.
        /// </summary>
        ~SvgConverter()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to save the conversion
        /// output to the XAML file.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the conversion output is saved
        /// to the XAML file; otherwise, it is <see langword="false"/>.
        /// The default depends on the converter.
        /// </value>
        public bool SaveXaml
        {
            get {
                return _saveXaml;
            }
            set {
                _saveXaml = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to save the conversion
        /// output to the ZAML file.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the conversion output is saved
        /// to the ZAML file; otherwise, it is <see langword="false"/>.
        /// The default depends on the converter.
        /// </value>
        /// <remarks>
        /// The ZAML is simply a G-Zip compressed XAML format, similar to the
        /// SVGZ.
        /// </remarks>
        public bool SaveZaml
        {
            get {
                return _saveZaml;
            }
            set {
                _saveZaml = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the .NET framework
        /// version of the XAML writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the .NET framework version of the
        /// XAML writer is used; otherwise, a customized XAML writer, 
        /// <see cref="XmlXamlWriter"/>, is used. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The customized XAML writer is optimized for the conversion process,
        /// and it is recommended as the writer, unless in cases where it fails
        /// to produce accurate result.
        /// </remarks>
        public bool UseFrameXamlWriter
        {
            get {
                return _useFrameXamlWriter;
            }
            set {
                _useFrameXamlWriter = value;
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the background of a image.
        /// </summary>
        /// <value>
        /// The brush that is used to fill the background of the control. 
        /// The default is <see langword="null"/> or transparent.
        /// </value>
        public SolidColorBrush Background
        {
            get {
                return _background;
            }
            set {
                _background = value;
            }
        }

        /// <summary>
        /// Gets the settings used by the rendering or drawing engine.
        /// </summary>
        /// <value>
        /// An instance of <see cref="WpfDrawingSettings"/> specifying all
        /// the options for rendering or drawing.
        /// </value>
        public WpfDrawingSettings DrawingSettings
        {
            get {
                return _wpfSettings;
            }
        }

        public WpfSvgWindow SvgWindow
        {
            get {
                return _wpfWindow;
            }
        }

        public SvgDocument SvgDocument
        {
            get {
                if (_wpfWindow != null)
                {
                    return _wpfWindow.Document as SvgDocument;
                }
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void BeginProcessing(WpfDrawingDocument drawingDocument = null)
        {
            if (_wpfSettings == null)
            {
                return;
            }

            var visitors = _wpfSettings.Visitors;
            WpfLinkVisitor linkVisitor = visitors.LinkVisitor;
            if (linkVisitor == null)
            {
                linkVisitor = new LinkVisitor();
                visitors.LinkVisitor = linkVisitor;
            }
            WpfFontFamilyVisitor fontFamilyVisitor = visitors.FontFamilyVisitor;
            if (fontFamilyVisitor == null)
            {
                fontFamilyVisitor = new FontFamilyVisitor();
                visitors.FontFamilyVisitor = fontFamilyVisitor;
            }
            WpfEmbeddedImageVisitor imageVisitor = visitors.ImageVisitor;
            if (imageVisitor == null)
            {
                imageVisitor = new EmbeddedImageVisitor();
                visitors.ImageVisitor = imageVisitor;
            }
            WpfIDVisitor idVisitor = visitors.IDVisitor;
            if (idVisitor != null)
            {
                visitors.IDVisitor = idVisitor;
            }
            WpfClassVisitor classVisitor = visitors.ClassVisitor;
            if (classVisitor != null)
            {
                visitors.ClassVisitor = classVisitor;
            }

            if (_wpfRenderer != null)
            {
                _wpfRenderer.LinkVisitor       = linkVisitor;
                _wpfRenderer.ImageVisitor      = imageVisitor;
                _wpfRenderer.FontFamilyVisitor = fontFamilyVisitor;

                _wpfRenderer.BeginRender(drawingDocument);
            }
        }

        protected virtual void EndProcessing()
        {
            if (_wpfRenderer != null)
            {
                _wpfRenderer.EndRender();
            }

            if (!_isEmbedded)
            {
                if (_wpfSettings != null)
                {
                    var visitors = _wpfSettings.Visitors;
                    if (visitors != null)
                    {
                        visitors.Uninitialize();
                    }
                }
            }
        }

        protected static BitmapEncoder GetBitmapEncoder(ImageEncoderType encoderType)
        {
            BitmapEncoder bitmapEncoder = null;

            switch (encoderType)
            {
                case ImageEncoderType.BmpBitmap:
                    bitmapEncoder = new BmpBitmapEncoder();
                    break;
                case ImageEncoderType.GifBitmap:
                    bitmapEncoder = new GifBitmapEncoder();
                    break;
                case ImageEncoderType.JpegBitmap:
                    JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                    // Set the default/user options...
                    bitmapEncoder = jpgEncoder;
                    break;
                case ImageEncoderType.PngBitmap:
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    // Set the default/user options...
                    bitmapEncoder = pngEncoder;
                    break;
                case ImageEncoderType.TiffBitmap:
                    bitmapEncoder = new TiffBitmapEncoder();
                    break;
                case ImageEncoderType.WmpBitmap:
                    WmpBitmapEncoder wmpEncoder = new WmpBitmapEncoder();
                    // Set the default/user options...
                    bitmapEncoder = wmpEncoder;
                    break;
            }

            if (bitmapEncoder == null)
            {
                bitmapEncoder = new PngBitmapEncoder();
            }

            return bitmapEncoder;
        }

        protected static string GetImageFileExtention(ImageEncoderType encoderType)
        {
            switch (encoderType)
            {
                case ImageEncoderType.BmpBitmap:
                    return ".bmp";
                case ImageEncoderType.GifBitmap:
                    return ".gif";
                case ImageEncoderType.JpegBitmap:
                    return ".jpg";
                case ImageEncoderType.PngBitmap:
                    return ".png";
                case ImageEncoderType.TiffBitmap:
                    return ".tif";
                case ImageEncoderType.WmpBitmap:
                    return ".wdp";
            }

            return ".png";
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This releases all resources used by the <see cref="SvgConverter"/> object.
        /// </overloads>
        /// <summary>
        /// This releases all resources used by the <see cref="SvgConverter"/> object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This releases the unmanaged resources used by the <see cref="SvgConverter"/> 
        /// and optionally releases the managed resources. 
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
