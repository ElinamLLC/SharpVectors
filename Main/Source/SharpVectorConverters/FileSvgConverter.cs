using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;

using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils; 

namespace SharpVectors.Converters
{
    /// <summary>
    /// <para>
    /// This converts an SVG file to the corresponding XAML file, which can 
    /// be viewed in WPF application. 
    /// </para>
    /// <para>
    /// The root object in the converted file is <see cref="DrawingGroup"/>.
    /// </para>
    /// </summary>
    public sealed class FileSvgConverter : SvgConverter
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

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private WpfSvgWindow       _wpfWindow;
        private WpfDrawingRenderer _wpfRenderer;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="FileSvgConverter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSvgConverter"/> class
        /// with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        /// This specifies the settings used by the rendering or drawing engine.
        /// If this is <see langword="null"/>, the default settings is used.
        /// </param>
        public FileSvgConverter(WpfDrawingSettings settings)
            : this(true, false, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSvgConverter"/> class
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
        public FileSvgConverter(bool saveXaml, bool saveZaml,
            WpfDrawingSettings settings)
            : base(saveXaml, saveZaml, settings)
        {
            _wpfRenderer = new WpfDrawingRenderer(this.DrawingSettings);
            _wpfWindow   = new WpfSvgWindow(640, 480, _wpfRenderer);
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
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to an XAML file.
        /// </overloads>
        /// <summary>
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to an XAML file with the same file name.
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
            return this.Convert(svgFileName, String.Empty);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG file, and saves
        /// the output to the specified XAML file.
        /// </summary>
        /// <param name="svgFileName">
        /// The full path of the SVG source file.
        /// </param>
        /// <param name="xamlFileName">
        /// The output XAML file. This is optional. If not specified, an XAML
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
        public bool Convert(string svgFileName, string xamlFileName)
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

            _xamlFile = null;
            _zamlFile = null;

            if (!this.SaveXaml && !this.SaveZaml)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(xamlFileName))
            {
                string workingDir = Path.GetDirectoryName(xamlFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgFileName, xamlFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified XAML file.
        /// </summary>
        /// <param name="svgStream">
        /// A stream providing access to the SVG source data.
        /// </param>
        /// <param name="xamlFileName">
        /// The output XAML file. This is optional. If not specified, an XAML
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="xamlFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgStream"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="xamlFileName"/> is empty.
        /// </exception>
        public bool Convert(Stream svgStream, string xamlFileName)
        {
            if (svgStream == null)
            {
                throw new ArgumentNullException("svgStream",
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (xamlFileName == null)
            {
                throw new ArgumentNullException("xamlFileName",
                    "The XAML destination file path cannot be null (or Nothing).");
            }
            if (xamlFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The XAML destination file path cannot be empty.", "xamlFileName");
            }

            _xamlFile = null;
            _zamlFile = null;

            if (!this.SaveXaml && !this.SaveZaml)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(xamlFileName))
            {
                string workingDir = Path.GetDirectoryName(xamlFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgStream, xamlFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified XAML file.
        /// </summary>
        /// <param name="svgTextReader">
        /// A text reader providing access to the SVG source data.
        /// </param>
        /// <param name="xamlFileName">
        /// The output XAML file. This is optional. If not specified, an XAML
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="xamlFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgTextReader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="xamlFileName"/> is empty.
        /// </exception>
        public bool Convert(TextReader svgTextReader, string xamlFileName)
        {
            if (svgTextReader == null)
            {
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (xamlFileName == null)
            {
                throw new ArgumentNullException("xamlFileName",
                    "The XAML destination file path cannot be null (or Nothing).");
            }
            if (xamlFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The XAML destination file path cannot be empty.", "xamlFileName");
            }

            _xamlFile = null;
            _zamlFile = null;

            if (!this.SaveXaml && !this.SaveZaml)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(xamlFileName))
            {
                string workingDir = Path.GetDirectoryName(xamlFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgTextReader, xamlFileName);
        }

        /// <summary>
        /// This performs the conversion of the specified SVG source, and saves
        /// the output to the specified XAML file.
        /// </summary>
        /// <param name="svgXmlReader">
        /// An XML reader providing access to the SVG source data.
        /// </param>
        /// <param name="xamlFileName">
        /// The output XAML file. This is optional. If not specified, an XAML
        /// file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the conversion is successful;
        /// otherwise, it return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="xamlFileName"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="svgXmlReader"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="xamlFileName"/> is empty.
        /// </exception>
        public bool Convert(XmlReader svgXmlReader, string xamlFileName)
        {
            if (svgXmlReader == null)
            {
                throw new ArgumentNullException("svgXmlReader",
                    "The SVG source file cannot be null (or Nothing).");
            }
            if (xamlFileName == null)
            {
                throw new ArgumentNullException("xamlFileName",
                    "The XAML destination file path cannot be null (or Nothing).");
            }
            if (xamlFileName.Length == 0)
            {
                throw new ArgumentException(
                    "The XAML destination file path cannot be empty.", "xamlFileName");
            }

            _xamlFile = null;
            _zamlFile = null;

            if (!this.SaveXaml && !this.SaveZaml)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(xamlFileName))
            {
                string workingDir = Path.GetDirectoryName(xamlFileName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }

            return this.ProcessFile(svgXmlReader, xamlFileName);
        }

        #endregion

        #region Private Methods

        #region ProcessFile Method

        private bool ProcessFile(string fileName, string xamlFileName)
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
                return false;
            }

            SaveFile(_drawing, fileName, xamlFileName);

            return true;
        }

        private bool ProcessFile(Stream svgStream, string xamlFileName)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgStream);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return false;
            }

            SaveFile(_drawing, xamlFileName, xamlFileName);

            return true;
        }

        private bool ProcessFile(TextReader svgTextReader, string xamlFileName)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgTextReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return false;
            }

            SaveFile(_drawing, xamlFileName, xamlFileName);

            return true;
        }

        private bool ProcessFile(XmlReader svgXmlReader, string xamlFileName)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgXmlReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null)
            {
                return false;
            }

            SaveFile(_drawing, xamlFileName, xamlFileName);

            return true;
        }

        #endregion

        #region SaveFile Method

        private bool SaveFile(Drawing drawing, string fileName, string xamlFileName)
        {
            _writerErrorOccurred = false;

            if (String.IsNullOrEmpty(xamlFileName))
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

                string workingDir  = Path.GetDirectoryName(fileName);
                xamlFileName       = Path.Combine(workingDir, 
                    fileNameWithoutExt + ".xaml");
            }
            else
            {
                string fileExt = Path.GetExtension(xamlFileName);
                if (String.IsNullOrEmpty(fileExt))
                {
                    xamlFileName += ".xaml";
                }
                else if (!String.Equals(fileExt, ".xaml", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    xamlFileName = Path.ChangeExtension(xamlFileName, ".xaml");
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
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (FileStream xamlFile = File.Create(xamlFileName))
                {
                    using (XmlWriter writer = XmlWriter.Create(
                        xamlFile, writerSettings))
                    {
                        System.Windows.Markup.XamlWriter.Save(
                            drawing, writer);
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
                                    drawing, writer);
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
