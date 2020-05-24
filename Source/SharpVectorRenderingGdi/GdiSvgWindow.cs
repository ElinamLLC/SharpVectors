using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Gdi;
using SharpVectors.Renderers.Forms;

namespace SharpVectors.Renderers
{
    public class GdiSvgWindow : SvgWindow
    {
        #region Contructors and Destructor

        public GdiSvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
            if (this.BaseUrls == null)
            {
                this.BaseUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public GdiSvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
        }

        #endregion

        #region ISvgWindow Members

        public override long InnerWidth
        {
            get {
                return base.InnerWidth;
            }
            set {
                base.InnerWidth = value;
            }
        }

        public override long InnerHeight
        {
            get {
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
        }

        public override ISvgRenderer CreateSvgRenderer()
        {
            return new GdiGraphicsRenderer(true);
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            if (innerWidth == 0 || innerHeight == 0)
            {
                return new GdiSvgWindow(this, this.InnerWidth, this.InnerHeight);
            }
            return new GdiSvgWindow(this, innerWidth, innerHeight);
        }

        #endregion
    }
}
