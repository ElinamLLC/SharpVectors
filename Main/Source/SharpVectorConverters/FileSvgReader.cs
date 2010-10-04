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
    public sealed class FileSvgReader
    {
        #region Private Fields

        private bool _saveXaml;
        private bool _saveZaml;

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private DirectoryInfo _workingDir;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private WpfSvgWindow       _wpfWindow;
        private WpfDrawingRenderer _wpfRenderer;

        private XmlReaderSettings  _settings;

        #endregion

        #region Constructors and Destructor

        public FileSvgReader()
        {
            _wpfRenderer = new WpfDrawingRenderer(); 
            _wpfWindow   = new WpfSvgWindow(640, 480, _wpfRenderer);
        }

        public FileSvgReader(bool saveXaml, bool saveZaml, 
            DirectoryInfo workingDir) : this()
        {
            _saveXaml   = saveXaml;
            _saveZaml   = saveZaml;
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

        public DrawingGroup Read(string svgFileName)
        {
            if (String.IsNullOrEmpty(svgFileName) || !File.Exists(svgFileName))
            {
                return null;
            }

            return this.LoadFile(svgFileName);
        }

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

            return this.Read(svgFileName);
        }

        #endregion

        #region Load Method

        private DrawingGroup LoadFile(string fileName)
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
                return null;
            }

            SaveFile(renderedDrawing, fileName);

            return renderedDrawing;
        }

        #endregion

        #region SaveFile Method

        private void SaveFile(Drawing drawing, string fileName)
        {
            if (_workingDir == null || (!_saveXaml && !_saveZaml))
            {
                return;
            }

            _writerErrorOccurred = false;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            fileNameWithoutExt  = Strings.StrConv(fileNameWithoutExt, VbStrConv.Narrow, 0x0011);
            string xamlFilePath = Path.Combine(_workingDir.FullName, fileNameWithoutExt + ".xaml");
            try
            {
                XmlXamlWriter xamlWriter = new XmlXamlWriter();

                using (FileStream xamlFile = File.Create(xamlFilePath))
                {
                    xamlWriter.Save(drawing, xamlFile);
                }
            }
            catch
            {
                _writerErrorOccurred = true;

                if (_fallbackOnWriterError)
                {
                    if (File.Exists(xamlFilePath))
                    {
                        File.Move(xamlFilePath, xamlFilePath + ".bak");
                    }

                    XmlWriterSettings writerSettings = new XmlWriterSettings();
                    writerSettings.Indent = true;
                    writerSettings.OmitXmlDeclaration = true;
                    writerSettings.Encoding = Encoding.UTF8;
                    using (FileStream xamlFile = File.Create(xamlFilePath))
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
                FileStream zamlSourceFile = new FileStream(xamlFilePath, FileMode.Open, 
                    FileAccess.Read, FileShare.Read);
                byte[] buffer = new byte[zamlSourceFile.Length];
                // Read the file to ensure it is readable.
                int count = zamlSourceFile.Read(buffer, 0, buffer.Length);
                if (count != buffer.Length)
                {
                    zamlSourceFile.Close();
                    return;
                }
                zamlSourceFile.Close();

                FileStream zamlDestFile = File.Create(Path.ChangeExtension(xamlFilePath, ".zaml"));

                GZipStream zipStream = new GZipStream(zamlDestFile, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);

                zipStream.Close();

                zamlDestFile.Close();
            }

            if (!_saveXaml && File.Exists(xamlFilePath))
            {
                File.Delete(xamlFilePath);
            }
        }

        #endregion
    }
}
