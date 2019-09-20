using System;
using System.Xml;
using System.Text;
using System.IO;
using System.IO.Compression;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This converts the SVG file to static or bitmap image, which is saved to a file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The image is save with the <see cref="PixelFormats.Pbgra32"/> format,
    /// since that is the only pixel format which does not throw an exception
    /// with the <see cref="RenderTargetBitmap"/>.
    /// </para>
    /// <para>
    /// The DPI used is 96.
    /// </para>
    /// </remarks>
    public sealed class ImageSvgConverter : SvgConverter
    {
        #region Private Fields

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private string _xamlFile;
        private string _zamlFile;

        /// <summary>
        /// This is the last drawing generated.
        /// </summary>
        private DrawingGroup _drawing;

        private ImageEncoderType _encoderType;
        private BitmapEncoder _bitampEncoder;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ImageSvgConverter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSvgConverter"/> class
        /// with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public ImageSvgConverter(WpfDrawingSettings settings)
            : this(false, false, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSvgConverter"/> class
        /// with the specified drawing or rendering settings and the saving options.
        /// </summary>
        /// <param name="saveXaml">
        /// This specifies whether to save result object tree in image file.
        /// </param>
        /// <param name="saveZaml">
        /// This specifies whether to save result object tree in ZAML file. The
        /// ZAML is simply a G-Zip compressed image format, similar to the SVGZ.
        /// </param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public ImageSvgConverter(bool saveXaml, bool saveZaml,
            WpfDrawingSettings settings) : base(saveXaml, saveZaml, settings)
        {
            long pixelWidth  = 0;
            long pixelHeight = 0;

            if (settings != null)
            {
                settings.EnsureViewboxSize = true;

                if (settings.HasPixelSize)
                {
                    pixelWidth  = settings.PixelWidth;
                    pixelHeight = settings.PixelHeight;
                }
            }

            _encoderType = ImageEncoderType.PngBitmap;
            _wpfRenderer = new WpfDrawingRenderer(this.DrawingSettings);
            _wpfWindow   = new WpfSvgWindow(pixelWidth, pixelHeight, _wpfRenderer);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether a writer error occurred when
        /// using the custom image writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if an error occurred when using
        /// the custom image writer; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool WriterErrorOccurred
        {
            get {
                return _writerErrorOccurred;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to fall back and use
        /// the .NET Framework image writer when an error occurred in using the
        /// custom writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the converter falls back to using
        /// the system image writer when an error occurred in using the custom
        /// writer; otherwise, it is <see langword="false"/>. If <see langword="false"/>,
        /// an exception, which occurred in using the custom writer will be
        /// thrown. The default is <see langword="false"/>. 
        /// </value>
        public bool FallbackOnWriterError
        {
            get {
                return _fallbackOnWriterError;
            }
            set {
                _fallbackOnWriterError = value;
            }
        }

        /// <summary>
        /// Gets or set the bitmap encoder type to use in encoding the drawing 
        /// to an image file.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ImageEncoderType"/> specifying
        /// the bitmap encoder. The default is the <see cref="ImageEncoderType.PngBitmap"/>.
        /// </value>
        public ImageEncoderType EncoderType
        {
            get {
                return _encoderType;
            }
            set {
                _encoderType = value;
            }
        }

        /// <summary>
        /// Gets or sets a custom bitmap encoder to use in encoding the drawing
        /// to an image file.
        /// </summary>
        /// <value>
        /// A derived <see cref="BitmapEncoder"/> object specifying the bitmap
        /// encoder for encoding the images. The default is <see langword="null"/>,
        /// and the <see cref="EncoderType"/> property determines the encoder used.
        /// </value>
        /// <remarks>
        /// If the value of this is set, it must match the MIME type or file 
        /// extension defined by the <see cref="EncoderType"/> property for it 
        /// to be used.
        /// </remarks>
        public BitmapEncoder Encoder
        {
            get {
                return _bitampEncoder;
            }
            set {
                _bitampEncoder = value;
            }
        }

        /// <summary>
        /// Gets the last created drawing.
        /// </summary>
        /// <value>
        /// A <see cref="DrawingGroup"/> specifying the last converted drawing.
        /// </value>
        public DrawingGroup Drawing
        {
            get {
                return _drawing;
            }
        }

        /// <summary>
        /// Gets the output XAML file path if generated.
        /// </summary>
        /// <value>
        /// A string containing the full path to the XAML if generated; otherwise,
        /// it is <see langword="null"/>.
        /// </value>
        public string XamlFile
        {
            get {
                return _xamlFile;
            }
        }

        /// <summary>
        /// Gets the output ZAML file path if generated.
        /// </summary>
        /// <value>
        /// A string containing the full path to the ZAML if generated; otherwise,
        /// it is <see langword="null"/>.
        /// </value>
        public string ZamlFile
        {
            get {
                return _zamlFile;
            }
        }

        #endregion

        #region Public Methods

        /// <overloads>
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to an image file.
        /// </overloads>
        /// <summary>
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to an image file with the same file name.
        /// </summary>
        /// <param name="svgFileName">
        /// The full path of the SVG source file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgFileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="svgFileName"/> is empty.
        /// <para>-or-</para>
        /// If the <paramref name="svgFileName"/> does not exists.
        /// </exception>
        public bool Convert(string svgFileName)
        {
            return this.Convert(svgFileName, string.Empty);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to the specified image file.
        /// </summary>
        /// <param name="svgFileName">
        /// The full path of the SVG source file.
        /// </param>
        /// <param name="imageFileName">
        /// The output image file. This is optional. If not specified, an image
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgFileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="svgFileName"/> is empty.
        /// <para>-or-</para>
        /// If the <paramref name="svgFileName"/> does not exists.
        /// </exception>
        public bool Convert(string svgFileName, string imageFileName)
        {
            if (svgFileName == null)
            {
                throw new ArgumentNullException(nameof(svgFileName),
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (svgFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The SVG source file cannot be empty.", nameof(svgFileName));
            }
            if (!File.Exists(svgFileName))
            {
                throw new ArgumentException(
                    "The SVG source file must exists.", nameof(svgFileName));
            }

            _xamlFile = null;
            _zamlFile = null;

            if (!string.IsNullOrWhiteSpace(imageFileName))
            {
                string workingDir = Path.GetDirectoryName(imageFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgFileName, imageFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified image file.
        /// </summary>
        /// <param name="svgStream">
        /// A stream providing access to the SVG source data.
        /// </param>
        /// <param name="imageFileName">
        /// The output image file. This is optional. If not specified, an image
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="imageFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgStream"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="imageFileName"/> is empty.
        /// </exception>
        public bool Convert(Stream svgStream, string imageFileName)
        {
            if (svgStream == null)
            {
                throw new ArgumentNullException(nameof(svgStream),
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (imageFileName == null)
            {
                throw new ArgumentNullException(nameof(imageFileName),
                    "The image destination file path cannot be null (or Nothing).");
            }
            if (imageFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The image destination file path cannot be empty.", nameof(imageFileName));
            }

            _xamlFile = null;
            _zamlFile = null;

            if (!string.IsNullOrWhiteSpace(imageFileName))
            {
                string workingDir = Path.GetDirectoryName(imageFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgStream, imageFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified image file.
        /// </summary>
        /// <param name="svgTextReader">
        /// A text reader providing access to the SVG source data.
        /// </param>
        /// <param name="imageFileName">
        /// The output image file. This is optional. If not specified, an image
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="imageFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgTextReader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="imageFileName"/> is empty.
        /// </exception>
        public bool Convert(TextReader svgTextReader, string imageFileName)
        {
            if (svgTextReader == null)
            {
                throw new ArgumentNullException(nameof(svgTextReader),
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (imageFileName == null)
            {
                throw new ArgumentNullException(nameof(imageFileName),
                    "The image destination file path cannot be null (or Nothing).");
            }
            if (imageFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The image destination file path cannot be empty.", nameof(imageFileName));
            }


            _xamlFile = null;
            _zamlFile = null;

            if (!string.IsNullOrWhiteSpace(imageFileName))
            {
                string workingDir = Path.GetDirectoryName(imageFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgTextReader, imageFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified image file.
        /// </summary>
        /// <param name="svgXmlReader">
        /// An XML reader providing access to the SVG source data.
        /// </param>
        /// <param name="imageFileName">
        /// The output image file. This is optional. If not specified, an image
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="imageFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgXmlReader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="imageFileName"/> is empty.
        /// </exception>
        public bool Convert(XmlReader svgXmlReader, string imageFileName)
        {
            if (svgXmlReader == null)
            {
                throw new ArgumentNullException(nameof(svgXmlReader),
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (imageFileName == null)
            {
                throw new ArgumentNullException(nameof(imageFileName),
                    "The image destination file path cannot be null (or Nothing).");
            }
            if (imageFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The image destination file path cannot be empty.", nameof(imageFileName));
            }


            _xamlFile = null;
            _zamlFile = null;

            if (!string.IsNullOrWhiteSpace(imageFileName))
            {
                string workingDir = Path.GetDirectoryName(imageFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgXmlReader, imageFileName);
        }

        #endregion

        #region Private Methods

        #region ProcessFile Method

        private bool ProcessFile(string fileName, string imageFileName)
        {
            this.BeginProcessing();

            _wpfWindow.LoadDocument(fileName, _wpfSettings);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                this.EndProcessing();

                return false;
            }

            // Save to the image file...
            SaveImageFile(_drawing, fileName, imageFileName);

            // Save to image and/or ZAML file if required...
            if (this.SaveXaml || this.SaveZaml)
            {
                SaveXamlFile(_drawing, fileName, imageFileName);
            }

            this.EndProcessing();

            return true;
        }

        private bool ProcessFile(Stream svgStream, string imageFileName)
        {
            this.BeginProcessing();

            _wpfWindow.LoadDocument(svgStream, _wpfSettings);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                this.EndProcessing();

                return false;
            }

            // Save to the image file...
            SaveImageFile(_drawing, imageFileName, imageFileName);

            // Save to image and/or ZAML file if required...
            if (this.SaveXaml || this.SaveZaml)
            {
                SaveXamlFile(_drawing, imageFileName, imageFileName);
            }

            this.EndProcessing();

            return true;
        }

        private bool ProcessFile(TextReader svgTextReader, string imageFileName)
        {
            this.BeginProcessing();

            _wpfWindow.LoadDocument(svgTextReader, _wpfSettings);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                this.EndProcessing();

                return false;
            }

            // Save to the image file...
            SaveImageFile(_drawing, imageFileName, imageFileName);

            // Save to image and/or ZAML file if required...
            if (this.SaveXaml || this.SaveZaml)
            {
                SaveXamlFile(_drawing, imageFileName, imageFileName);
            }

            this.EndProcessing();

            return true;
        }

        private bool ProcessFile(XmlReader svgXmlReader, string imageFileName)
        {
            this.BeginProcessing();

            _wpfWindow.LoadDocument(svgXmlReader, _wpfSettings);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                this.EndProcessing();

                return false;
            }

            // Save to the image file...
            SaveImageFile(_drawing, imageFileName, imageFileName);

            // Save to image and/or ZAML file if required...
            if (this.SaveXaml || this.SaveZaml)
            {
                SaveXamlFile(_drawing, imageFileName, imageFileName);
            }

            this.EndProcessing();

            return true;
        }

        #endregion

        #region SaveImageFile Method

        private bool SaveImageFile(Drawing drawing, string fileName, string imageFileName)
        {
            string outputExt      = this.GetImageFileExtention();
            string outputFileName = null;
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

                string workingDir = Path.GetDirectoryName(fileName);
                outputFileName = Path.Combine(workingDir, fileNameWithoutExt + outputExt);
            }
            else
            {
                string fileExt = Path.GetExtension(imageFileName);
                if (string.IsNullOrWhiteSpace(fileExt))
                {
                    outputFileName = imageFileName + outputExt;
                }
                else if (!string.Equals(fileExt, outputExt, StringComparison.OrdinalIgnoreCase))
                {
                    outputFileName = Path.ChangeExtension(imageFileName, outputExt);
                }
                else
                {
                    outputFileName = imageFileName;
                }
            }

            string outputFileDir = Path.GetDirectoryName(outputFileName);
            if (!Directory.Exists(outputFileDir))
            {
                Directory.CreateDirectory(outputFileDir);
            }

            BitmapEncoder bitmapEncoder = this.GetBitmapEncoder(outputExt);

            // The image parameters...
            Rect drawingBounds = drawing.Bounds;
            int pixelWidth     = (int)drawingBounds.Width;
            int pixelHeight    = (int)drawingBounds.Height;
            double dpiX        = 96;
            double dpiY        = 96;

            // The Visual to use as the source of the RenderTargetBitmap.
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (this.Background != null)
            {
                drawingContext.DrawRectangle(this.Background, null, drawing.Bounds);
            }
            drawingContext.DrawDrawing(drawing);
            drawingContext.Close();

            // The BitmapSource that is rendered with a Visual.
            var targetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            targetBitmap.Render(drawingVisual);

            // Encoding the RenderBitmapTarget as an image file.
            bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            using (FileStream stream = File.Create(outputFileName))
            {
                bitmapEncoder.Save(stream);
            }

            return true;
        }

        private BitmapEncoder GetBitmapEncoder(string fileExtension)
        {
            BitmapEncoder bitmapEncoder = null;

            if (_bitampEncoder != null && _bitampEncoder.CodecInfo != null)
            {
                string mimeType           = string.Empty;
                BitmapCodecInfo codecInfo = _bitampEncoder.CodecInfo;
                string mimeTypes          = codecInfo.MimeTypes;
                string fileExtensions     = codecInfo.FileExtensions;
                switch (_encoderType)
                {
                    case ImageEncoderType.BmpBitmap:
                        mimeType = "image/bmp";
                        break;
                    case ImageEncoderType.GifBitmap:
                        mimeType = "image/gif";
                        break;
                    case ImageEncoderType.JpegBitmap:
                        mimeType = "image/jpeg,image/jpe,image/jpg";
                        break;
                    case ImageEncoderType.PngBitmap:
                        mimeType = "image/png";
                        break;
                    case ImageEncoderType.TiffBitmap:
                        mimeType = "image/tiff,image/tif";
                        break;
                    case ImageEncoderType.WmpBitmap:
                        mimeType = "image/vnd.ms-photo";
                        break;
                }

                if (!string.IsNullOrWhiteSpace(fileExtensions) &&
                    fileExtensions.IndexOf(fileExtension, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bitmapEncoder = _bitampEncoder;
                }
                else if (!string.IsNullOrWhiteSpace(mimeTypes) &&
                    !string.IsNullOrWhiteSpace(mimeType))
                {
                    string[] arrayMimeTypes = mimeType.Split(',');
                    for (int i = 0; i < arrayMimeTypes.Length; i++)
                    {
                        if (mimeTypes.IndexOf(arrayMimeTypes[i], StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            bitmapEncoder = _bitampEncoder;
                            break;
                        }
                    }
                }
            }

            if (bitmapEncoder == null)
            {
                bitmapEncoder = GetBitmapEncoder(_encoderType);
            }

            return bitmapEncoder;
        }

        private string GetImageFileExtention()
        {
            return GetImageFileExtention(_encoderType);
        }

        #endregion

        #region SaveXamlFile Method

        private bool SaveXamlFile(Drawing drawing, string fileName, string imageFileName)
        {
            _writerErrorOccurred = false;

            string xamlFileName = null;
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

                string workingDir = Path.GetDirectoryName(fileName);
                xamlFileName = Path.Combine(workingDir, fileNameWithoutExt + XamlExt);
            }
            else
            {
                string fileExt = Path.GetExtension(imageFileName);
                if (string.IsNullOrWhiteSpace(fileExt))
                {
                    xamlFileName = imageFileName + XamlExt;
                }
                else if (!string.Equals(fileExt, XamlExt, StringComparison.OrdinalIgnoreCase))
                {
                    xamlFileName = Path.ChangeExtension(imageFileName, XamlExt);
                }
            }

            if (File.Exists(xamlFileName))
            {
                File.SetAttributes(xamlFileName, FileAttributes.Normal);
                File.Delete(xamlFileName);
            }

            if (this.UseFrameXamlWriter)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent             = true;
                writerSettings.Encoding           = Encoding.UTF8;
                writerSettings.OmitXmlDeclaration = true;

                using (FileStream xamlFile = File.Create(xamlFileName))
                {
                    using (XmlWriter writer = XmlWriter.Create(xamlFile, writerSettings))
                    {
                        System.Windows.Markup.XamlWriter.Save(drawing, writer);
                    }
                }
            }
            else
            {
                try
                {
                    XmlXamlWriter xamlWriter = new XmlXamlWriter(this.DrawingSettings);

                    using (FileStream xamlFile = File.Create(xamlFileName))
                    {
                        xamlWriter.Save(drawing, xamlFile);
                    }
                }
                catch
                {
                    _writerErrorOccurred = true;

                    if (_fallbackOnWriterError)
                    {
                        if (File.Exists(xamlFileName))
                        {
                            File.Move(xamlFileName, xamlFileName + BackupExt);
                        }

                        XmlWriterSettings writerSettings = new XmlWriterSettings();
                        writerSettings.Indent             = true;
                        writerSettings.Encoding           = Encoding.UTF8;
                        writerSettings.OmitXmlDeclaration = true;

                        using (FileStream xamlFile = File.Create(xamlFileName))
                        {
                            using (XmlWriter writer = XmlWriter.Create(xamlFile, writerSettings))
                            {
                                System.Windows.Markup.XamlWriter.Save(drawing, writer);
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (this.SaveZaml)
            {
                string zamlFileName = Path.ChangeExtension(xamlFileName, CompressedXamlExt);

                if (File.Exists(zamlFileName))
                {
                    File.SetAttributes(zamlFileName, FileAttributes.Normal);
                    File.Delete(zamlFileName);
                }

                FileStream zamlSourceFile = new FileStream(xamlFileName, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                byte[] buffer = new byte[zamlSourceFile.Length];
                // Read the file to ensure it is readable.
                int count = zamlSourceFile.Read(buffer, 0, buffer.Length);
                if (count != buffer.Length)
                {
                    zamlSourceFile.Close();
                    return false;
                }
                zamlSourceFile.Close();

                FileStream zamlDestFile = File.Create(zamlFileName);

                GZipStream zipStream = new GZipStream(zamlDestFile, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);

                zipStream.Close();

                zamlDestFile.Close();

                _zamlFile = zamlFileName;
            }
            _xamlFile = xamlFileName;

            if (!this.SaveXaml && File.Exists(xamlFileName))
            {
                File.Delete(xamlFileName);
                _xamlFile = null;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
