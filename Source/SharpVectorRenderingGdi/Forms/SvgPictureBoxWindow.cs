using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Gdi;

namespace SharpVectors.Renderers.Forms
{
    public class SvgPictureBoxWindow : SvgWindow
    {
        #region Private fields

        private bool _preferUserSize;
        private ISvgControl _svgControl;

        #endregion

        #region Contructors and Destructor

        public SvgPictureBoxWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
            _preferUserSize = true;
        }

        public SvgPictureBoxWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
            _preferUserSize = true;
        }

        public SvgPictureBoxWindow(ISvgControl control, ISvgRenderer renderer)
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

            _preferUserSize = true;
            _svgControl  = control;
        }

        private SvgPictureBoxWindow(ISvgControl control, SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "control cannot be null");
            }

            _preferUserSize = true;
            _svgControl  = control;
        }

        #endregion

        #region ISvgWindow Members

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
                Uri uri = new Uri(new Uri(Application.ExecutablePath), value);

                SvgDocument document = new SvgDocument(this);
                document.Load(uri.AbsoluteUri);

                this.Document = document;
            }
        }

        public override DirectoryInfo WorkingDir
        {
            get {
                return SvgApplicationContext.ExecutableDirectory;
            }
        }

        public override void Alert(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || _svgControl == null)
            {
                return;
            }

            _svgControl.HandleAlert(message);
        }

        public override ISvgRenderer CreateSvgRenderer()
        {
            return new GdiGraphicsRenderer(true);
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            if (innerWidth == 0 || innerHeight == 0)
            {
                return new SvgPictureBoxWindow(this, this.InnerWidth, this.InnerHeight);
            }
            return new SvgPictureBoxWindow(this, innerWidth, innerHeight);
        }

        #endregion
    }
}
