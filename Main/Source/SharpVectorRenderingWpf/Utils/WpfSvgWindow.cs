using System;
using System.IO;
using System.Xml;
using System.Reflection;

using System.Windows;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Utils
{
    public class WpfSvgWindow : SvgWindow
    {
        #region Private fields

        private XmlReaderSettings _settings;

        #endregion

        #region Contructors and Destructor

        public WpfSvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
        }

        public WpfSvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
        }

        #endregion

        #region ISvgWindow Members

        public XmlReaderSettings CustomSettings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        public override long InnerWidth
        {
            get
            {
                return base.InnerWidth;
            }
            set
            {
                base.InnerWidth = value;
            }
        }

        public override long InnerHeight
        {
            get
            {
                return base.InnerHeight;
            }
            set
            {
                base.InnerHeight = value;
            }
        }

        public override string Source
        {
            get
            {
                SvgDocument document = (SvgDocument)this.Document;
                return (document != null) ? document.Url : String.Empty;
            }
            set
            {
                Uri uri = new Uri(new Uri(
                    Assembly.GetExecutingAssembly().Location), value);

                this.LoadDocument(uri);
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get
            {
                return WpfApplicationContext.ExecutableDirectory;
            }
        }

        public void LoadDocument(Uri documentUri)
        {
            if (documentUri == null || !documentUri.IsAbsoluteUri)
            {
                return;
            }

            SvgDocument document = new SvgDocument(this);
            if (_settings != null)
            {
                document.CustomSettings = _settings;
            }
            document.Load(documentUri.AbsoluteUri);

            this.Document = document;
        }

        public void LoadDocument(string documentSource)
        {
            if (String.IsNullOrEmpty(documentSource))
            {
                return;
            }

            Uri uri = new Uri(new Uri(
                Assembly.GetExecutingAssembly().Location),
                documentSource);

            this.LoadDocument(uri);
        }

        public void LoadDocument(Stream documentStream)
        {
            if (documentStream == null)
            {
                return;
            }

            SvgDocument document = new SvgDocument(this);
            if (_settings != null)
            {
                document.CustomSettings = _settings;
            }
            document.Load(documentStream);

            this.Document = document;
        }

        public void LoadDocument(TextReader textReader)
        {
            if (textReader == null)
            {
                return;
            }

            SvgDocument document = new SvgDocument(this);
            if (_settings != null)
            {
                document.CustomSettings = _settings;
            }
            document.Load(textReader);

            this.Document = document;
        }

        public void LoadDocument(XmlReader xmlReader)
        {
            if (xmlReader == null)
            {
                return;
            }

            SvgDocument document = new SvgDocument(this);
            if (_settings != null)
            {
                document.CustomSettings = _settings;
            }
            document.Load(xmlReader);

            this.Document = document;
        }

        public override void Alert(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            MessageBox.Show(message);
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            return new WpfSvgWindow(this, innerWidth, innerHeight);
        }

        #endregion
    }
}
