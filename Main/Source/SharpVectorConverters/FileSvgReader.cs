using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils; 

namespace SharpVectors.Converters
{
    /// <summary>
    /// This converts a SVG file to <see cref="DrawingGroup"/> object, and can 
    /// optionally save the result to a file as XAML.
    /// </summary>
    public sealed class FileSvgReader : SvgConverter
    {
        #region Private Fields

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private string _imageFile;
        private string _xamlFile;
        private string _zamlFile;

        private DirectoryInfo _workingDir;

        /// <summary>
        /// This is the last drawing generated.
        /// </summary>
        private DrawingGroup _drawing;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private WpfSvgWindow       _wpfWindow;
        private WpfDrawingRenderer _wpfRenderer;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="FileSvgReader"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSvgReader"/> class
        /// with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public FileSvgReader(WpfDrawingSettings settings)
            : this(false, false, null, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSvgConverter"/> class
        /// with the specified drawing or rendering settings, the saving options
        /// and the working directory.
        /// </summary>
        /// <param name="saveXaml">
        /// This specifies whether to save result object tree in XAML file.
        /// </param>
        /// <param name="saveZaml">
        /// This specifies whether to save result object tree in ZAML file. The
        /// ZAML is simply a G-Zip compressed XAML format, similar to the SVGZ.
        /// </param>
        /// <param name="workingDir">
        /// The working directory, where converted outputs are saved.
        /// </param>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public FileSvgReader(bool saveXaml, bool saveZaml,
            DirectoryInfo workingDir, WpfDrawingSettings settings)
            : base(saveXaml, saveZaml, settings)
        {
            _wpfRenderer = new WpfDrawingRenderer(this.DrawingSettings);
            _wpfWindow   = new WpfSvgWindow(640, 480, _wpfRenderer);

            _workingDir = workingDir;

            if (_workingDir != null)
            {
                if (!_workingDir.Exists)
                {
                    _workingDir.Create();
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether a writer error occurred when
        /// using the custom XAML writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if an error occurred when using
        /// the custom XAML writer; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool WriterErrorOccurred
        {
            get
            {
                return _writerErrorOccurred;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to fall back and use
        /// the .NET Framework XAML writer when an error occurred in using the
        /// custom writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the converter falls back to using
        /// the system XAML writer when an error occurred in using the custom
        /// writer; otherwise, it is <see langword="false"/>. If <see langword="false"/>,
        /// an exception, which occurred in using the custom writer will be
        /// thrown. The default is <see langword="false"/>. 
        /// </value>
        public bool FallbackOnWriterError
        {
            get
            {
                return _fallbackOnWriterError;
            }
            set
            {
                _fallbackOnWriterError = value;
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
            get
            {
                return _drawing;
            }
        }

        /// <summary>
        /// Gets the output image file path if generated.
        /// </summary>
        /// <value>
        /// A string containing the full path to the image if generated; otherwise,
        /// it is <see langword="null"/>.
        /// </value>
        public string ImageFile
        {
            get
            {
                return _imageFile;
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
            get
            {
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
            get
            {
                return _zamlFile;
            }
        }

        #endregion

        #region Public Methods

        /// <overloads>
        /// Reads in the specified SVG file and converts it to WPF drawing.
        /// </overloads>
        /// <summary>
        /// Reads in the specified SVG file and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgFileName">
        /// The full path of the SVG source file.
        /// </param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgFileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="svgFileName"/> is empty.
        /// <para>-or-</para>
        /// If the <paramref name="svgFileName"/> does not exists.
        /// </exception>
        public DrawingGroup Read(string svgFileName)
        {
            if (svgFileName == null)
            {
                throw new ArgumentNullException("svgFileName",
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (svgFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The SVG source file cannot be empty.", "svgFileName");
            }
            if (!File.Exists(svgFileName))
            {
                throw new ArgumentException(
                    "The SVG source file must exists.", "svgFileName");
            }

            if (_workingDir == null)
            {
                svgFileName = Path.GetFullPath(svgFileName);
                _workingDir = new DirectoryInfo(
                    Path.GetDirectoryName(svgFileName));
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.LoadFile(svgFileName);
        }

        /// <summary>
        /// Reads in the specified SVG file and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgUri">
        /// A <see cref="System.Uri"/> specifying the path to the SVG file.
        /// </param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgUri"/> is <see langword="null"/>.
        /// </exception>
        public DrawingGroup Read(Uri svgUri)
        {
            if (svgUri == null)
            {
                throw new ArgumentNullException("svgUri",
                    "The SVG source file cannot be null (or Nothing).");
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.LoadFile(svgUri);
        }

        /// <summary>
        /// Reads in the specified SVG file stream and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgStream">The source SVG file stream.</param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgStream"/> is <see langword="null"/>.
        /// </exception>
        public DrawingGroup Read(Stream svgStream)
        {
            if (svgStream == null)
            {
                throw new ArgumentNullException("svgStream",
                    "The SVG source file cannot be null (or Nothing).");
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.LoadFile(svgStream);
        }

        /// <summary>
        /// Reads in the specified source from the SVG file reader and converts 
        /// it to WPF drawing.
        /// </summary>
        /// <param name="svgTextReader">
        /// A text reader providing access to the SVG file data.
        /// </param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgTextReader"/> is <see langword="null"/>.
        /// </exception>
        public DrawingGroup Read(TextReader svgTextReader)
        {
            if (svgTextReader == null)
            {
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.LoadFile(svgTextReader);
        }

        /// <summary>
        /// Reads in the specified source SVG file reader and converts it to 
        /// WPF drawing.
        /// </summary>
        /// <param name="svgXmlReader">
        /// An XML reader providing access to the SVG file data.
        /// </param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgXmlReader"/> is <see langword="null"/>.
        /// </exception>
        public DrawingGroup Read(XmlReader svgXmlReader)
        {
            if (svgXmlReader == null)
            {
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.LoadFile(svgXmlReader);
        }

        /// <summary>
        /// Reads in the specified SVG file, converting it to WPF drawing and
        /// saving the results to the specified directory if successful.
        /// </summary>
        /// <param name="svgFileName">
        /// The full path of the SVG source file.
        /// </param>
        /// <param name="destinationDir">
        /// The destination of the output XAML file, if the saving properties
        /// are enabled.
        /// </param>
        /// <returns>
        /// This returns the <see cref="DrawingGroup"/> representing the SVG file,
        /// if successful; otherwise, it returns <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="svgFileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="svgFileName"/> is empty.
        /// <para>-or-</para>
        /// If the <paramref name="svgFileName"/> does not exists.
        /// </exception>
        public DrawingGroup Read(string svgFileName, DirectoryInfo destinationDir)
        {
            _workingDir = destinationDir;

            if (_workingDir != null)
            {
                if (!_workingDir.Exists)
                {
                    _workingDir.Create();
                }
            }

            _imageFile = null;
            _xamlFile  = null;
            _zamlFile  = null;

            return this.Read(svgFileName);
        }

        /// <summary>
        /// Saves the last converted file to the specified file name.
        /// </summary>
        /// <param name="fileName">
        /// The full path of the output file.
        /// </param>
        /// <param name="asXaml">
        /// A value indicating whether to save the output to XAML file.
        /// </param>
        /// <param name="asZaml">
        /// A value indicating whether to save the output to ZAML file, which
        /// is a G-zip compression of the XAML file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if either <paramref name="asXaml"/>
        /// or <paramref name="asZaml"/> is <see langword="true"/> and the operation
        /// is successful.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the output serialization properties are not enabled, this method
        /// can be used to save the output to a file.
        /// </para>
        /// <para>
        /// This will not change the output serialization properties of this object.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// If there is no converted drawing from a previous conversion process
        /// to be saved.
        /// </exception>
        public bool Save(string fileName, bool asXaml, bool asZaml)
        {   
            if (_drawing == null)
            {
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");
            }
 
            // We save the current states and properties...
            bool saveXaml = this.SaveXaml;
            bool saveZaml = this.SaveZaml;

            this.SaveXaml = asXaml;
            this.SaveZaml = asZaml;

            DirectoryInfo workingDir =  _workingDir;

            fileName    = Path.GetFullPath(fileName);
            _workingDir = new DirectoryInfo(Path.GetDirectoryName(fileName));

            bool savedResult = this.SaveFile(fileName);

            // Restore the current states and properties...
            this.SaveXaml = saveXaml;
            this.SaveZaml = saveZaml;
            _workingDir   = workingDir;

            return savedResult;
        }

        public bool Save(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter",
                    "The text writer parameter is required and cannot be null (or Nothing).");
            }  
            if (_drawing == null)
            {
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");
            }

            return this.SaveFile(textWriter);
        }

        public bool Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream",
                    "The stream parameter is required and cannot be null (or Nothing).");
            }  
            if (_drawing == null)
            {
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");
            }

            return this.SaveFile(stream);
        }

        public bool SaveImage(string fileName, DirectoryInfo imageFileDir, 
            ImageEncoderType encoderType)
        {
            if (imageFileDir == null)
            {
                return this.SaveImageFile(fileName, String.Empty, encoderType);
            }
            else
            {   
                if (!imageFileDir.Exists)
                {
                    imageFileDir.Create();
                }

                string outputExt = GetImageFileExtention(encoderType);

                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(
                    fileName);

                string imageFileName = Path.Combine(imageFileDir.FullName,
                    fileNameWithoutExt + outputExt);

                return this.SaveImageFile(fileName, imageFileName, encoderType);
            }
        }

        public bool SaveImage(string fileName, FileInfo imageFileName,
            ImageEncoderType encoderType)
        {
            return this.SaveImageFile(fileName, 
                imageFileName == null ? String.Empty : imageFileName.FullName,
                encoderType);
        }

        #endregion

        #region Load Method

        private DrawingGroup LoadFile(string fileName)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(fileName);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return null;
            }

            SaveFile(fileName);

            return _drawing;
        }

        private DrawingGroup LoadFile(Stream stream)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(stream);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return null;
            }

            return _drawing;
        }

        private DrawingGroup LoadFile(Uri svgUri)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgUri);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return null;
            }

            return _drawing;
        }

        private DrawingGroup LoadFile(TextReader textReader)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(textReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return null;
            }

            return _drawing;
        }

        private DrawingGroup LoadFile(XmlReader xmlReader)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(xmlReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return null;
            }

            return _drawing;
        }

        #endregion

        #region SaveFile Method

        private bool SaveFile(Stream stream)
        {
            _writerErrorOccurred = false;

            if (this.UseFrameXamlWriter)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (XmlWriter writer = XmlWriter.Create(
                    stream, writerSettings))
                {
                    System.Windows.Markup.XamlWriter.Save(
                        _drawing, writer);
                }
            }
            else
            {
                try
                {
                    XmlXamlWriter xamlWriter = new XmlXamlWriter(
                        this.DrawingSettings);

                    xamlWriter.Save(_drawing, stream);
                }
                catch
                {
                    _writerErrorOccurred = true;

                    if (_fallbackOnWriterError)
                    {
                        XmlWriterSettings writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (XmlWriter writer = XmlWriter.Create(
                            stream, writerSettings))
                        {
                            System.Windows.Markup.XamlWriter.Save(
                                _drawing, writer);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        private bool SaveFile(TextWriter textWriter)
        {
            _writerErrorOccurred = false;

            if (this.UseFrameXamlWriter)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (XmlWriter writer = XmlWriter.Create(
                    textWriter, writerSettings))
                {
                    System.Windows.Markup.XamlWriter.Save(
                        _drawing, writer);
                }
            }
            else
            {
                try
                {
                    XmlXamlWriter xamlWriter = new XmlXamlWriter(
                        this.DrawingSettings);

                    xamlWriter.Save(_drawing, textWriter);
                }
                catch
                {
                    _writerErrorOccurred = true;

                    if (_fallbackOnWriterError)
                    {
                        XmlWriterSettings writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (XmlWriter writer = XmlWriter.Create(
                            textWriter, writerSettings))
                        {
                            System.Windows.Markup.XamlWriter.Save(
                                _drawing, writer);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        private bool SaveFile(string fileName)
        {
            if (_workingDir == null || (!this.SaveXaml && !this.SaveZaml))
            {
                return false;
            }

            _writerErrorOccurred = false;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            string xamlFileName = Path.Combine(_workingDir.FullName, 
                fileNameWithoutExt + ".xaml");

            if (File.Exists(xamlFileName))
            {
                File.SetAttributes(xamlFileName, FileAttributes.Normal);
                File.Delete(xamlFileName);
            }

            if (this.UseFrameXamlWriter)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (FileStream xamlFile = File.Create(xamlFileName))
                {
                    using (XmlWriter writer = XmlWriter.Create(
                        xamlFile, writerSettings))
                    {
                        System.Windows.Markup.XamlWriter.Save(
                            _drawing, writer);
                    }
                }
            }
            else
            {
                try
                {
                    XmlXamlWriter xamlWriter = new XmlXamlWriter(
                        this.DrawingSettings);

                    using (FileStream xamlFile = File.Create(xamlFileName))
                    {
                        xamlWriter.Save(_drawing, xamlFile);
                    }
                }
                catch
                {
                    _writerErrorOccurred = true;

                    if (_fallbackOnWriterError)
                    {
                        // If the file exist, we back it up and save a new file...
                        if (File.Exists(xamlFileName))
                        {
                            File.Move(xamlFileName, xamlFileName + ".bak");
                        }

                        XmlWriterSettings writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (FileStream xamlFile = File.Create(xamlFileName))
                        {
                            using (XmlWriter writer = XmlWriter.Create(
                                xamlFile, writerSettings))
                            {
                                System.Windows.Markup.XamlWriter.Save(
                                    _drawing, writer);
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
                string zamlFileName = Path.ChangeExtension(xamlFileName, ".zaml");

                if (File.Exists(zamlFileName))
                {
                    File.SetAttributes(zamlFileName, FileAttributes.Normal);
                    File.Delete(zamlFileName);
                }

                FileStream zamlSourceFile = new FileStream(
                    xamlFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

                GZipStream zipStream = new GZipStream(zamlDestFile, 
                    CompressionMode.Compress, true);
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

        #region SaveImageFile Method

        private bool SaveImageFile(string fileName, string imageFileName,
            ImageEncoderType encoderType)
        {
            if (_drawing == null)
            {
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");
            }

            string outputExt = GetImageFileExtention(encoderType);
            string outputFileName = null;
            if (String.IsNullOrEmpty(imageFileName))
            {
                string fileNameWithoutExt =
                    Path.GetFileNameWithoutExtension(fileName);

                string workingDir = Path.GetDirectoryName(fileName);
                outputFileName = Path.Combine(workingDir,
                    fileNameWithoutExt + outputExt);
            }
            else
            {
                string fileExt = Path.GetExtension(imageFileName);
                if (String.IsNullOrEmpty(fileExt))
                {
                    outputFileName = imageFileName + outputExt;
                }
                else if (!String.Equals(fileExt, outputExt,
                    StringComparison.OrdinalIgnoreCase))
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

            BitmapEncoder bitampEncoder = GetBitmapEncoder(outputExt,
                encoderType);

            // The image parameters...
            Rect drawingBounds = _drawing.Bounds;
            int pixelWidth = (int)drawingBounds.Width;
            int pixelHeight = (int)drawingBounds.Height;
            double dpiX = 96;
            double dpiY = 96;

            // The Visual to use as the source of the RenderTargetBitmap.
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (this.Background != null)
            {
                drawingContext.DrawRectangle(this.Background, null, _drawing.Bounds);
            }
            drawingContext.DrawDrawing(_drawing);
            drawingContext.Close();

            // The BitmapSource that is rendered with a Visual.
            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(
                pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            targetBitmap.Render(drawingVisual);

            // Encoding the RenderBitmapTarget as an image file.
            bitampEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            using (FileStream stream = File.Create(outputFileName))
            {
                bitampEncoder.Save(stream);
            }

            _imageFile = outputFileName;

            return true;
        }

        private static BitmapEncoder GetBitmapEncoder(string fileExtension,
            ImageEncoderType encoderType)
        {
            BitmapEncoder bitampEncoder = null;


            switch (encoderType)
            {
                case ImageEncoderType.BmpBitmap:
                    bitampEncoder = new BmpBitmapEncoder();
                    break;
                case ImageEncoderType.GifBitmap:
                    bitampEncoder = new GifBitmapEncoder();
                    break;
                case ImageEncoderType.JpegBitmap:
                    JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = jpgEncoder;
                    break;
                case ImageEncoderType.PngBitmap:
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = pngEncoder;
                    break;
                case ImageEncoderType.TiffBitmap:
                    bitampEncoder = new TiffBitmapEncoder();
                    break;
                case ImageEncoderType.WmpBitmap:
                    WmpBitmapEncoder wmpEncoder = new WmpBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = wmpEncoder;
                    break;
            }

            if (bitampEncoder == null)
            {
                bitampEncoder = new PngBitmapEncoder();
            }

            return bitampEncoder;
        }

        private static string GetImageFileExtention(ImageEncoderType encoderType)
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

        /// <summary>
        /// This releases the unmanaged resources used by the <see cref="SvgConverter"/> 
        /// and optionally releases the managed resources. 
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            _drawing     = null;
            _wpfWindow   = null;
            _wpfRenderer = null;

            base.Dispose(disposing);
        }

        #endregion
    }
}
