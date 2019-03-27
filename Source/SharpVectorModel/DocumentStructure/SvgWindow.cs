using System;
using System.IO;
using System.Xml;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgWindow : ISvgWindow
    {
        #region Private fields

        private bool _loadFonts;

        private long _innerWidth;
        private long _innerHeight;
        private SvgDocument _document;
        private SvgWindow _parentWindow;

        private ISvgRenderer _renderer;

        #endregion

        #region Contructors and Destructor

        protected SvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
        {
            _renderer = renderer;
            if (_renderer != null)
            {
                _renderer.Window = this;
            }

            _innerWidth  = innerWidth;
            _innerHeight = innerHeight;

            _loadFonts   = true;
        }

        protected SvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : this(innerWidth, innerHeight, parentWindow.Renderer)
        {
            _parentWindow = parentWindow;
        }

        #endregion

        #region Public properties

        public SvgWindow ParentWindow
        {
            get {
                return _parentWindow;
            }
        }

        public ISvgRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        public abstract DirectoryInfo WorkingDir
        {
            get;
        }

        #endregion

        #region Public properties

        internal bool LoadFonts
        {
            get { return _loadFonts; }
            set { _loadFonts = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create and assign an empty SvgDocument to this window.  This is needed only in situations where 
        /// the library user needs to create an SVG DOM tree outside of the usual LoadSvgDocument mechanism.
        /// </summary>
        public SvgDocument CreateEmptySvgDocument()
        {
            return _document = new SvgDocument(this);
        }

        #endregion

        #region ISvgWindow Members

        public virtual long InnerWidth
        {
            get {
                return _innerWidth;
            }
            set {
                _innerWidth = value;
            }
        }

        public virtual long InnerHeight
        {
            get {
                return _innerHeight;
            }
            set {
                _innerHeight = value;
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
            get {
                return null;
            }
        }

        public ISvgDocument Document
        {
            get {
                return _document;
            }
            set {
                _document = (SvgDocument)value;
            }
        }

        public abstract void Alert(string message);

        public virtual SvgWindow CreateOwnedWindow()
        {
            return this.CreateOwnedWindow(_innerWidth, _innerHeight);
        }

        public abstract SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight);

        public abstract string Source
        {
            get;
            set;
        }

        /// <summary>
        /// This is expected to be called by the host
        /// </summary>
        /// <param name="innerWidth">The new width of the control</param>
        /// <param name="innerHeight">The new height of the control</param>
        public virtual void Resize(int innerWidth, int innerHeight)
        {
            _innerWidth = innerWidth;
            _innerHeight = innerHeight;

            if (Document != null && Document.RootElement != null)
                (Document.RootElement as SvgSvgElement).Resize();
        }

        #endregion
    }
}
