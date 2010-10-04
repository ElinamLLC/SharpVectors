using System;
using System.IO;
using System.Xml;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgWindow : ISvgWindow
    {
        #region Private fields

        private long innerWidth;
        private long innerHeight;
        private SvgDocument document;
        private SvgWindow parentWindow;

        private ISvgRenderer renderer;

        #endregion

        #region Contructors and Destructor

        protected SvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
        {
            this.renderer = renderer;
            if (this.renderer != null)
            {
                this.renderer.Window = this;
            }

            this.innerWidth  = innerWidth;
            this.innerHeight = innerHeight;
        }

        protected SvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : this(innerWidth, innerHeight, parentWindow.Renderer)
        {
            this.parentWindow = parentWindow;
        }

        #endregion

        #region Public properties
        
        public SvgWindow ParentWindow
        {
            get
            {
                return parentWindow;
            }
        }
        
        public ISvgRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        public abstract DirectoryInfo WorkingDir
        {
            get;
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Create and assign an empty SvgDocument to this window.  This is needed only in situations where the library user needs to create an SVG DOM tree outside of the usual LoadSvgDocument mechanism.
        /// </summary>
        public SvgDocument CreateEmptySvgDocument()
        {
            return document = new SvgDocument(this);
        }

        #endregion

        #region ISvgWindow Members

        public virtual long InnerWidth
        {
            get
            {
                return innerWidth;
            }
            set
            {
                this.innerWidth = value;
            }
        }

        public virtual long InnerHeight
        {
            get
            {
                return innerHeight;
            }
            set
            {
                this.innerHeight = value;
            }
        }

        public XmlDocumentFragment ParseXML(string source, XmlDocument document)
        {
            XmlDocumentFragment frag = document.CreateDocumentFragment();
            frag.InnerXml = source;
            return frag;
        }

        public string PrintNode(XmlNode node)
        {
            return node.OuterXml;
        }

        public IStyleSheet DefaultStyleSheet
        {
            get
            {
                return null;
            }
        }

        public ISvgDocument Document
        {
            get
            {
                return document;
            }
            set
            {
                document = (SvgDocument)value;
            }
        }

        public abstract void Alert(string message);

        public abstract SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight);

        public abstract string Source
        {
            get;
            set;
        }

        /// <summary>
        /// This is expected to be called by the host
        /// </summary>
        /// <param name="width">The new width of the control</param>
        /// <param name="height">The new height of the control</param>
        public virtual void Resize(int innerWidth, int innerHeight)
        {
            this.innerWidth  = innerWidth;
            this.innerHeight = innerHeight;

            if (Document != null && Document.RootElement != null)
                (Document.RootElement as SvgSvgElement).Resize();
        }

        #endregion
    }
}
