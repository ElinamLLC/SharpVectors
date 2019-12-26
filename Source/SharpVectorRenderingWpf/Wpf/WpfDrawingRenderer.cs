using System;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Rename: WpfRendererSession
    /// </remarks>
    public sealed class WpfDrawingRenderer : WpfRendererObject, ISvgRenderer
    {
        #region Private Fields

        private bool _isEmbedded;

        private SvgRectF _invalidRect;
        /// <summary>
        /// The renderer's <see cref="SvgWindow">SvgWindow</see> object.
        /// </summary>
        private ISvgWindow _svgWindow;

        private WpfDrawingSettings _settings;

        private WpfRenderingHelper _svgRenderer;

        private WpfLinkVisitor _linkVisitor;
        private WpfFontFamilyVisitor _fontFamilyVisitor;
        private WpfEmbeddedImageVisitor _imageVisitor;

        private WpfDrawingDocument _drawingDocument;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingRenderer()
            : this(new WpfDrawingSettings(), false)
        {
        }

        public WpfDrawingRenderer(WpfDrawingSettings settings)
            : this(settings, false)
        {
        }

        public WpfDrawingRenderer(WpfDrawingSettings settings, bool isEmbedded)
        {
            _isEmbedded  = isEmbedded;
            _svgRenderer = new WpfRenderingHelper(this);
            _settings    = settings;
        }

        #endregion

        #region Public Properties

        public DrawingGroup Drawing
        {
            get {
                if (_context == null)
                {
                    return null;
                }

                return _context.Root;
            }
        }

        public WpfLinkVisitor LinkVisitor
        {
            get {
                return _linkVisitor;
            }
            set {
                _linkVisitor = value;
            }
        }

        public WpfEmbeddedImageVisitor ImageVisitor
        {
            get {
                return _imageVisitor;
            }
            set {
                _imageVisitor = value;
            }
        }

        public WpfFontFamilyVisitor FontFamilyVisitor
        {
            get {
                return _fontFamilyVisitor;
            }
            set {
                _fontFamilyVisitor = value;
            }
        }

        #endregion

        #region ISvgRenderer Members

        public ISvgWindow Window
        {
            get {
                return _svgWindow;
            }
            set {
                _svgWindow = value;
            }
        }

        public void BeginRender(WpfDrawingDocument drawingDocument)
        {
            if (_svgRenderer == null)
            {
                _svgRenderer = new WpfRenderingHelper(this);
            }

            _drawingDocument = drawingDocument;
        }

        public void EndRender()
        {
            _svgRenderer     = null;
            _drawingDocument = null;
        }

        public void Render(ISvgElement node)
        {
            //throw new NotImplementedException();

            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //if (graphics != null && graphics.Graphics != null)
            //{
            //    _svgRenderer.Render(node);
            //}

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);

            if (_svgRenderer == null)
            {
                _svgRenderer = new WpfRenderingHelper(this);
            }

            _context = new WpfDrawingContext(true, _settings);

            _context.Initialize(null, _fontFamilyVisitor, _imageVisitor);

            _context.BeginDrawing(_drawingDocument);

            _svgRenderer.Render(node);

            _context.EndDrawing();
        }

        public void Render(ISvgElement node, WpfDrawingContext context)
        {
            //throw new NotImplementedException();

            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //if (graphics != null && graphics.Graphics != null)
            //{
            //    _svgRenderer.Render(node);
            //}

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);

            if (_svgRenderer == null)
            {
                _svgRenderer = new WpfRenderingHelper(this);
            }

            if (context == null)
            {
                _context = new WpfDrawingContext(true, _settings);

                _context.Initialize(null, _fontFamilyVisitor, _imageVisitor);
            }
            else
            {
                _context = context;
            }

            _context.BeginDrawing(_drawingDocument);

            _svgRenderer.Render(node);

            _context.EndDrawing();
        }

        public void RenderAs(SvgElement node, WpfDrawingContext context)
        {
            if (_svgRenderer == null)
            {
                _svgRenderer = new WpfRenderingHelper(this);
            }

            if (context == null)
            {
                _context = new WpfDrawingContext(true, _settings);

                _context.Initialize(null, _fontFamilyVisitor, _imageVisitor);
            }
            else
            {
                _context = context;
            }

            _context.BeginDrawing(_drawingDocument);

            _svgRenderer.RenderAs(node);

            _context.EndDrawing();
        }

        public void Render(ISvgDocument node)
        {
            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //_renderingContext = new WpfDrawingContext(new DrawingGroup());
            if (_svgRenderer == null)
            {
                _svgRenderer = new WpfRenderingHelper(this);
            }

            _context = new WpfDrawingContext(_isEmbedded, _settings);

            _context.Initialize(_linkVisitor, _fontFamilyVisitor, _imageVisitor);

            _context.BeginDrawing(_drawingDocument);

            _svgRenderer.Render(node);

            _context.EndDrawing();

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);
        }

        public SvgRectF InvalidRect
        {
            get {
                return _invalidRect;
            }
            set {
                _invalidRect = value;
            }
        }

        public void RenderChildren(ISvgElement node)
        {
            _svgRenderer.RenderChildren(node);
        }

        public void RenderMask(ISvgElement node, WpfDrawingContext context)
        {
            if (context == null)
            {
                _context = new WpfDrawingContext(true, _settings);

                _context.Initialize(null, _fontFamilyVisitor, _imageVisitor);
            }
            else
            {
                _context = context;
            }

            _context.BeginDrawing(_drawingDocument);

            _svgRenderer.RenderMask(node);

            _context.EndDrawing();
        }

        public void InvalidateRect(SvgRectF rect)
        {
            _invalidRect = rect;
        }

        public RenderEvent OnRender
        {
            get {
                return null;
            }
            set {
            }
        }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            return SvgRect.Empty;
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
