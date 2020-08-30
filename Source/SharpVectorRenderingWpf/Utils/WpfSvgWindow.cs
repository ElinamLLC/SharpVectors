using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using System.Windows;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Renderers.Utils
{
    public class WpfSvgWindow : SvgWindow
    {
        #region Private fields

        private bool _preferUserSize;
        private ISvgControl _svgControl;

        private XmlReaderSettings _settings;

        #endregion

        #region Contructors and Destructor

        public WpfSvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
//            _preferUserSize = true;
        }

        public WpfSvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
//            _preferUserSize = true;
        }

        public WpfSvgWindow(ISvgControl control, ISvgRenderer renderer)
            : base(control.Width, control.Height, renderer)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "control cannot be null");
            }
            if (this.BaseUrls == null)
            {
                this.BaseUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

//            _preferUserSize = true;
            _svgControl = control;
        }

        private WpfSvgWindow(ISvgControl control, SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "control cannot be null");
            }

//            _preferUserSize = true;
            _svgControl = control;
        }

        #endregion

        #region Public Properties

        public XmlReaderSettings CustomSettings
        {
            get {
                return _settings;
            }
            set {
                _settings = value;
            }
        }

        public override long InnerWidth
        {
            get {
                if (_preferUserSize && base.InnerWidth != 0 && base.InnerHeight != 0)
                {
                    return base.InnerWidth;
                }
                if (_svgControl != null)
                {
                    return _svgControl.Width;
                }
                return base.InnerWidth;
            }
            set {
                base.InnerWidth = value;
            }
        }

        public override long InnerHeight
        {
            get {
                if (_preferUserSize && base.InnerWidth != 0 && base.InnerHeight != 0)
                {
                    return base.InnerHeight;
                }
                if (_svgControl != null)
                {
                    return _svgControl.Height;
                }

                return base.InnerHeight;
            }
            set {
                base.InnerHeight = value;
            }
        }

        public override string Source
        {
            get {
                SvgDocument document = (SvgDocument)this.Document;
                return (document != null) ? document.Url : string.Empty;
            }
            set {
                Uri uri = new Uri(new Uri(Assembly.GetExecutingAssembly().Location), value);

                this.LoadDocument(uri, null);
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get {
                return WpfApplicationContext.ExecutableDirectory;
            }
        }

        #endregion

        #region Public Methods

        public void LoadDocument(Uri documentUri, WpfDrawingSettings drawingSettings)
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

            this.SetupStyleSheets(drawingSettings);
        }

        public void LoadDocument(string documentSource, WpfDrawingSettings drawingSettings)
        {
            if (string.IsNullOrWhiteSpace(documentSource))
            {
                return;
            }

            Uri uri = new Uri(new Uri(Assembly.GetExecutingAssembly().Location), documentSource);

            this.LoadDocument(uri, drawingSettings);
        }

        public void LoadDocument(Stream documentStream, WpfDrawingSettings drawingSettings)
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

            this.SetupStyleSheets(drawingSettings);
        }

        public void LoadDocument(TextReader textReader, WpfDrawingSettings drawingSettings)
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

            this.SetupStyleSheets(drawingSettings);
        }

        public void LoadDocument(XmlReader xmlReader, WpfDrawingSettings drawingSettings)
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

            this.SetupStyleSheets(drawingSettings);
        }

        public override void Alert(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || _svgControl == null)
            {
                return;
            }

            _svgControl.HandleAlert(message);
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            if (innerWidth == 0 || innerHeight == 0)
            {
                return new WpfSvgWindow(this, this.InnerWidth, this.InnerHeight);
            }
            return new WpfSvgWindow(this, innerWidth, innerHeight);
        }

        public virtual void SetupStyleSheets(WpfDrawingSettings drawingSettings)
        {
            if (drawingSettings == null)
            {
                return;
            }

            CssXmlDocument cssDocument = this.Document as CssXmlDocument;
            if (cssDocument == null)
            {
                return;
            }

            string userCssFilePath = drawingSettings.UserCssFilePath;
            if (!string.IsNullOrWhiteSpace(userCssFilePath) && File.Exists(userCssFilePath))
            {
                cssDocument.SetUserStyleSheet(userCssFilePath);
            }

            string userAgentCssFilePath = drawingSettings.UserAgentCssFilePath;
            if (!string.IsNullOrWhiteSpace(userAgentCssFilePath) && File.Exists(userAgentCssFilePath))
            {
                cssDocument.SetUserAgentStyleSheet(userAgentCssFilePath);
            }
        }

        #endregion
    }
}
