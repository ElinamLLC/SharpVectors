using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>      
    /// Defines the interface required for a rendering node to interact with the renderer and the SVG DOM
    /// </summary>
    /// <remarks>
    /// Rename: WpfElementRenderer, WpfRenderingObject
    /// </remarks>
    public abstract class WpfRenderingBase : WpfRendererObject
    {
        #region Private Fields

        protected bool _isReady;
        protected string _elementId;
        protected string _uniqueId;
        protected SvgElement _svgElement;
        protected WpfSvgPaintContext _paintContext;

        #endregion

        #region Constructors and Destructor

        protected WpfRenderingBase(SvgElement element)
            : this(element, null)
        {
        }

        protected WpfRenderingBase(SvgElement element, WpfDrawingContext context)
            : base(context)
        {
            _isReady    = true;
            _svgElement = element;
            if (element != null)
            {
                _uniqueId  = element.UniqueId;
                _elementId = this.GetElementName();
            }
        }

        #endregion

        #region Public Properties

        public SvgElement Element
        {
            get { return _svgElement; }
        }

        public virtual bool IsRecursive
        {
            get {
                return false;
            }
        }

        public bool IsReady
        {
            get {
                return _isReady;
            }
            protected set {
                _isReady = value;
            }
        }

        public WpfSvgPaintContext PaintContext
        {
            get {
                return _paintContext;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool NeedRender(WpfDrawingRenderer renderer)
        {
            if (_svgElement.GetAttribute("display") == "none")
            {
                return false;
            }
            return true;
        }

        // define empty handlers by default
        public virtual void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }
            _isReady = false;
            _context = renderer.Context;

            if (_svgElement != null && _context != null)
            {
                if (string.IsNullOrWhiteSpace(_uniqueId))
                {
                    _uniqueId = _svgElement.UniqueId;
                }
                _paintContext = new WpfSvgPaintContext(_uniqueId);
                _context.RegisterPaintContext(_paintContext);
            }
        }

        public virtual void Render(WpfDrawingRenderer renderer)
        {
            _isReady = false;
        }

        public virtual void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_svgElement != null && _context != null)
            {
                if (string.IsNullOrWhiteSpace(_uniqueId))
                {
                    _uniqueId = _svgElement.UniqueId;
                }
                _context.UnRegisterPaintContext(_uniqueId);
            }
            _isReady = true;
        }

        #endregion

        #region Protected Methods

        protected virtual void Initialize(SvgElement element)
        {
            _isReady    = true;
            _elementId  = null;
            _uniqueId   = null;
            _svgElement = element;
            if (element != null)
            {
                _uniqueId  = element.UniqueId;
                _elementId = this.GetElementName();
            }
            _paintContext = null;
        }

        protected string GetElementName()
        {
            if (!string.IsNullOrWhiteSpace(_elementId))
            {
                return _elementId;
            }
            _elementId = GetElementName(_svgElement, _context);

            return _elementId;
        }

        protected string GetElementClass()
        {
            return GetElementClassName(_svgElement, _context);
        }

        protected void Rendered(Drawing drawing)
        {
            if (drawing != null && _context != null)
            {
                if (string.IsNullOrWhiteSpace(_elementId) 
                    || !string.Equals(_elementId, _svgElement.Id))
                {
                    //_elementId = this.GetElementName();
                    _elementId = _svgElement.Id;
                }
                if (string.IsNullOrWhiteSpace(_uniqueId))
                {
                    _uniqueId = _svgElement.UniqueId;
                }

                _context.RegisterDrawing(_elementId, _uniqueId, drawing);
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
