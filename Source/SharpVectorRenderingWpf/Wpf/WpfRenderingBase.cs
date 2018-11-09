using System;

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
            _svgElement = element;
        }

        #endregion

        #region Public Properties

        public SvgElement Element
        {
            get { return _svgElement; }
        }

        public virtual bool IsRecursive
        {
            get
            {
                return false;
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
            _context = renderer.Context;

            if (_svgElement != null && _context != null)
            {
                _paintContext = new WpfSvgPaintContext(_svgElement.UniqueId);
                _context.RegisterPaintContext(_paintContext);
            }
        }

        public virtual void Render(WpfDrawingRenderer renderer) { }

        public virtual void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_svgElement != null && _context != null)
            {
                _context.UnRegisterPaintContext(_svgElement.UniqueId);
            }
        }

        public string GetElementName()
        {
            return GetElementName(_svgElement, _context);
        }

        public string GetElementClass()
        {
            return GetElementClassName(_svgElement, _context);
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
