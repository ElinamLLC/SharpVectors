using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

using SharpVectors.Xml;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Renderers.Forms
{
    public class SvgPictureBoxWindow : SvgWindow
    {
        #region Private fields

        private SvgPictureBox _svgPictureBox;

        #endregion

        #region Contructors and Destructor

        public SvgPictureBoxWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
        }

        public SvgPictureBoxWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
        }

        public SvgPictureBoxWindow(SvgPictureBox control, ISvgRenderer renderer)
            : base(control.Width, control.Height, renderer)
        {
            if (control == null)
            {
                throw new NullReferenceException("control cannot be null");
            }

            _svgPictureBox = control;
        }

        #endregion

        #region ISvgWindow Members

        public override long InnerWidth
        {
            get
            {
                if (_svgPictureBox != null)
                {
                    return _svgPictureBox.Width;
                }

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
                if (_svgPictureBox != null)
                {
                    return _svgPictureBox.Height;
                }

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
                Uri uri = new Uri(new Uri(Application.ExecutablePath), value);

                SvgDocument document = new SvgDocument(this);
                document.Load(uri.AbsoluteUri);

                this.Document = document;
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get
            {
                return SvgApplicationContext.ExecutableDirectory;
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
            return new SvgPictureBoxWindow(this, innerWidth, innerHeight);
        }

        #endregion
    }
}
