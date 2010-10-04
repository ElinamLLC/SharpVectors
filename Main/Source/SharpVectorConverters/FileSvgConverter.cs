using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualBasic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

using SharpVectors.Net;
using SharpVectors.Xml;
using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils; 

namespace SharpVectors.Converters
{
    public sealed class FileSvgConverter
    {
        #region Private Fields

        private bool _saveXaml;
        private bool _saveZaml;

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private WpfSvgWindow       _wpfWindow;
        private WpfDrawingRenderer _wpfRenderer;

        private XmlReaderSettings  _settings;

        #endregion

        #region Constructors and Destructor

        public FileSvgConverter()
        {
            _wpfRenderer = new WpfDrawingRenderer(); 
            _wpfWindow   = new WpfSvgWindow(640, 480, _wpfRenderer);
        }

        public FileSvgConverter(bool saveXaml, bool saveZaml)
            : this()
        {
            _saveXaml   = saveXaml;
            _saveZaml   = saveZaml;
        }

        #endregion

        #region Public Properties

        public bool SaveXaml
        {
            get
            {
                return _saveXaml;
            }
            set
            {
                _saveXaml = value;
            }
        }

        public bool SaveZaml
        {
            get
            {
                return _saveZaml;
            }
            set
            {
                _saveZaml = value;
            }
        }

        public bool WriterErrorOccurred
        {
            get
            {
                return _writerErrorOccurred;
            }
        }

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

        public XmlReaderSettings CustomSettings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;

                if (_wpfWindow != null)
                {
                    _wpfWindow.CustomSettings = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public bool Convert(string svgFileName, string xamlFileName)
        {
            if (!_saveXaml && !_saveZaml)
            {
                return false;
            }

            if (String.IsNullOrEmpty(svgFileName) || !File.Exists(svgFileName))
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

        #endregion

        #region Load Method

        private bool ProcessFile(string fileName, string xamlFileName)
        {
            _wpfRenderer.LinkVisitor       = new LinkVisitor();
            _wpfRenderer.ImageVisitor      = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.Source = fileName;

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            DrawingGroup renderedDrawing = _wpfRenderer.Drawing as DrawingGroup;
            if (renderedDrawing == null)
            {
                return false;
            }

            SaveFile(renderedDrawing, fileName, xamlFileName);

            renderedDrawing = null;

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

                fileNameWithoutExt = Strings.StrConv(fileNameWithoutExt, VbStrConv.Narrow, 0x0011);
                string workingDir  = Path.GetDirectoryName(fileName);
                xamlFileName       = Path.Combine(workingDir, fileNameWithoutExt + ".xaml");
            }
            try
            {
                XmlXamlWriter xamlWriter = new XmlXamlWriter();

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

                    XmlWriterSettings writerSettings  = new XmlWriterSettings();
                    writerSettings.Indent             = true;
                    writerSettings.OmitXmlDeclaration = true;
                    writerSettings.Encoding           = Encoding.UTF8;
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

            if (_saveZaml)
            {   
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

                FileStream zamlDestFile = File.Create(Path.ChangeExtension(xamlFileName, ".zaml"));

                GZipStream zipStream = new GZipStream(zamlDestFile, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);

                zipStream.Close();

                zamlDestFile.Close();
            }

            if (!_saveXaml && File.Exists(xamlFileName))
            {
                File.Delete(xamlFileName);
            }

            return true;
        }

        #endregion
    }
}
