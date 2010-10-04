using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Xml;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;
using SharpVectors.Dom.Stylesheets;

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
                    System.Reflection.Assembly.GetExecutingAssembly().Location), value);

                SvgDocument document = new SvgDocument(this);
                if (_settings != null)
                {
                    document.CustomSettings = _settings;
                }
                document.Load(uri.AbsoluteUri);

                this.Document = document;
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get
            {
                return WpfApplicationContext.ExecutableDirectory;
            }
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
