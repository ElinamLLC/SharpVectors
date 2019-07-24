using System;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Renders a Svg image to GDI+
    /// </summary>
    public sealed class GdiGraphicsRenderer : ISvgRenderer, IDisposable
    {
        #region Private Fields

        private bool _isStatic;
        private bool _disposeRaster;

        /// <summary>
        /// The bitmap containing the rendered Svg image.
        /// </summary>
        private Bitmap _rasterImage;

        /// <summary>
        /// The renderer's graphics wrapper object.
        /// </summary>
        private GdiGraphics _graphics;

        /// <summary>
        /// The renderer's back color.
        /// </summary>
        private Color _backColor;

        /// <summary>
        /// The renderer's <see cref="SvgWindow">SvgWindow</see> object.
        /// </summary>
        private SvgWindow _svgWindow;

        /// <summary>
        /// 
        /// </summary>
        private float _currentDownX;
        private float _currentDownY;
        private IEventTarget _currentTarget;
        private IEventTarget _currentDownTarget;

        private SvgRectF _invalidRect;

        private RenderEvent _onRender;

        private GdiHitTestHelper _hitTestHelper;

        private GdiRenderingHelper _svgRenderer;

        #endregion

        #region Constructors and Destructor

        public GdiGraphicsRenderer(int rasterWidth, int rasterHeight, bool disposeRaster = false)
            : this(true, disposeRaster)
        {
            _svgWindow = new GdiSvgWindow(rasterWidth, rasterHeight, this);
        }

        /// <summary>
        /// Initializes a new instance of the GdiRenderer class.
        /// </summary>
        public GdiGraphicsRenderer(bool isStatic = false, bool disposeRaster = true)
        {
            _isStatic      = isStatic;
            _disposeRaster = disposeRaster;
            _invalidRect   = SvgRectF.Empty;
            _backColor     = Color.White;
            _hitTestHelper = GdiHitTestHelper.NoHit;
            _svgRenderer   = new GdiRenderingHelper(this);
        }

        ~GdiGraphicsRenderer()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a bitmap image of the a rendered Svg document.
        /// </summary>
        public Bitmap RasterImage
        {
            get {
                return _rasterImage;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Window">Window</see> of the
        /// renderer.
        /// </summary>
        /// <value>
        /// The <see cref="Window">Window</see> of the renderer.
        /// </value>
        public SvgWindow Window
        {
            get {
                return _svgWindow;
            }
            set {
                _svgWindow = value;
            }
        }
        ISvgWindow ISvgRenderer.Window
        {
            get {
                return _svgWindow;
            }
            set {
                _svgWindow = value as SvgWindow;
            }
        }

        /// <summary>
        /// Gets or sets the back color of the renderer.
        /// </summary>
        /// <value>
        /// The back color of the renderer.
        /// </value>
        public Color BackColor
        {
            get {
                return _backColor;
            }
            set {
                _backColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GdiGraphics">GdiGraphics</see>
        /// object associated with this renderer.
        /// </summary>
        /// <value>
        /// The <see cref="GdiGraphics">GdiGraphics</see> object
        /// associated with this renderer.
        /// </value>
        public GdiGraphics GdiGraphics
        {
            get {
                return _graphics;
            }
            set {
                _graphics = value;
            }
        }

        /// <summary>
        /// The invalidated region
        /// </summary>   
        public SvgRectF InvalidRect
        {
            get {
                return _invalidRect;
            }
            set {
                _invalidRect = value;
            }
        }

        #endregion

        #region Public Methods

        public void InvalidateRect(SvgRectF rect)
        {
            if (_invalidRect == SvgRectF.Empty)
                _invalidRect = rect;
            else
                _invalidRect.Intersect(rect);
        }

        /// <summary>
        /// Renders the <see cref="SvgElement">SvgElement</see>.
        /// </summary>
        /// <param name="node">
        /// The <see cref="SvgElement">SvgElement</see> node to be
        /// rendered
        /// </param>
        /// <returns>
        /// The bitmap on which the rendering was performed.
        /// </returns>
        public void Render(ISvgElement node)
        {
            SvgRectF updatedRect;
            if (_invalidRect != SvgRectF.Empty)
                updatedRect = new SvgRectF(_invalidRect.X, _invalidRect.Y,
                    _invalidRect.Width, _invalidRect.Height);
            else
                updatedRect = SvgRectF.Empty;

            RendererBeforeRender();

            if (_graphics != null && _graphics.Graphics != null)
            {
                _svgRenderer.Render(node);
            }

            RendererAfterRender();

            if (_onRender != null)
                OnRender(updatedRect);
        }

        /// <summary>
        /// Renders the <see cref="SvgDocument">SvgDocument</see>.
        /// </summary>
        /// <param name="node">
        /// The <see cref="SvgDocument">SvgDocument</see> node to be
        /// rendered
        /// </param>
        /// <returns>
        /// The bitmap on which the rendering was performed.
        /// </returns>
        public void Render(ISvgDocument node)
        {
            SvgRectF updatedRect;
            if (_invalidRect != SvgRectF.Empty)
                updatedRect = new SvgRectF(_invalidRect.X, _invalidRect.Y,
                    _invalidRect.Width, _invalidRect.Height);
            else
                updatedRect = SvgRectF.Empty;

            RendererBeforeRender();

            if (_graphics != null && _graphics.Graphics != null)
            {
                _svgRenderer.Render(node);
            }

            RendererAfterRender();

            if (_onRender != null)
                OnRender(updatedRect);
        }

        public void RenderChildren(ISvgElement node)
        {
            _svgRenderer.RenderChildren(node);
        }

        public void ClearAll()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_rasterImage != null)
            {
                _rasterImage.Dispose();
                _rasterImage = null;
            }
            if (_hitTestHelper != null)
            {
                _hitTestHelper.Dispose();
                _hitTestHelper = null;
            }
            _hitTestHelper = GdiHitTestHelper.NoHit;
        }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            SvgTransformableElement transElement = element as SvgTransformableElement;
            if (transElement != null)
            {
                SvgRectF rect = this.GetElementBounds(transElement, margin);

                return new SvgRect(rect.X, rect.Y, rect.Width, rect.Height);
            }

            return null;
        }

        #endregion

        #region Event handlers

        public RenderEvent OnRender
        {
            get {
                return _onRender;
            }
            set {
                _onRender = value;
            }
        }

        /// <summary>
        /// Processes mouse events.
        /// </summary>
        /// <param name="type">
        /// A string describing the type of mouse event that occured.
        /// </param>
        /// <param name="e">
        /// The <see cref="MouseEventArgs">MouseEventArgs</see> that contains
        /// the event data.
        /// </param>
        public void OnMouseEvent(string type, MouseEventArgs e)
        {
            if (_hitTestHelper == null)
            {
                return;
            }

            try
            {
                this.ProcessMouseEvents(type, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        #region Miscellaneous Methods

        internal Color GetNextHitColor(SvgElement element)
        {
            if (_hitTestHelper != null)
            {
                return _hitTestHelper.GetNextHitColor(element);
            }

            return Color.White;
        }

        private SvgRectF GetElementBounds(SvgTransformableElement element, float margin)
        {
            SvgRenderingHint hint = element.RenderingHint;
            if (hint == SvgRenderingHint.Shape || hint == SvgRenderingHint.Text)
            {
                GraphicsPath gp = GdiRendering.CreatePath(element);
                if (gp == null)
                {
                    return SvgRectF.Empty;
                }
                ISvgMatrix svgMatrix = element.GetScreenCTM();

                Matrix matrix = new Matrix((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                      (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);
                SvgRectF bounds = SvgConverter.ToRect(gp.GetBounds(matrix));
                bounds = SvgRectF.Inflate(bounds, margin, margin);

                return bounds;
            }

            SvgUseElement useElement = element as SvgUseElement;
            if (useElement != null)
            {
                SvgTransformableElement refEl = useElement.ReferencedElement as SvgTransformableElement;
                if (refEl == null)
                    return SvgRectF.Empty;

                XmlElement refElParent = (XmlElement)refEl.ParentNode;
                element.OwnerDocument.Static = true;
                useElement.CopyToReferencedElement(refEl);
                element.AppendChild(refEl);

                SvgRectF bbox = this.GetElementBounds(refEl, margin);

                element.RemoveChild(refEl);
                useElement.RestoreReferencedElement(refEl);
                refElParent.AppendChild(refEl);
                element.OwnerDocument.Static = false;

                return bbox;
            }

            SvgRectF union = SvgRectF.Empty;
            SvgTransformableElement transformChild;
            foreach (XmlNode childNode in element.ChildNodes)
            {
                if (childNode is SvgDefsElement)
                    continue;
                if (childNode is ISvgTransformable)
                {
                    transformChild = (SvgTransformableElement)childNode;
                    SvgRectF bbox = this.GetElementBounds(transformChild, margin);
                    if (bbox != SvgRectF.Empty)
                    {
                        if (union == SvgRectF.Empty)
                            union = bbox;
                        else
                            union = SvgRectF.Union(union, bbox);
                    }
                }
            }

            return union;
        }

        #endregion

        #region Private Methods

        private void ProcessMouseEvents(string type, MouseEventArgs e)
        {
            //Color pixel = _idMapRaster.GetPixel(e.X, e.Y);
            //SvgElement svgElement = GetElementFromColor(pixel);

            SvgElement svgElement = null;
            var hitTestResult = _hitTestHelper.HitTest(e.X, e.Y);
            if (hitTestResult != null)
            {
                svgElement = hitTestResult.Element;
            }

            if (type == "mouseup")
            {
                type = type.Trim();
            }

            if (svgElement == null)
            {
                // jr patch
                if (_currentTarget != null && type == "mousemove")
                {
                    _currentTarget.DispatchEvent(new MouseEvent(EventType.MouseOut, true, false,
                        null, // todo: put view here
                        0, // todo: put detail here
                        e.X, e.Y, e.X, e.Y,
                        false, false, false, false,
                        0, null, false));
                }
                _currentTarget = null;
                return;
            }

            IEventTarget target;
            if (svgElement.ElementInstance != null)
                target = svgElement.ElementInstance as IEventTarget;
            else
                target = svgElement as IEventTarget;

            if (target == null)
            {
                // jr patch
                if (_currentTarget != null && type == "mousemove")
                {
                    _currentTarget.DispatchEvent(new MouseEvent(EventType.MouseOut, true, false,
                        null, // todo: put view here
                        0, // todo: put detail here
                        e.X, e.Y, e.X, e.Y,
                        false, false, false, false,
                        0, null, false));
                }
                _currentTarget = null;
                return;
            }

            switch (type)
            {
                case "mousemove":
                    {
                        if (_currentTarget == target)
                        {
                            target.DispatchEvent(new MouseEvent(EventType.MouseMove, true, false,
                                null, // todo: put view here
                                0, // todo: put detail here
                                e.X, e.Y, e.X, e.Y,
                                false, false, false, false,
                                0, null, false));
                        }
                        else
                        {
                            if (_currentTarget != null)
                            {
                                _currentTarget.DispatchEvent(new MouseEvent(EventType.MouseOut, true, false,
                                    null, // todo: put view here
                                    0, // todo: put detail here
                                    e.X, e.Y, e.X, e.Y,
                                    false, false, false, false,
                                    0, null, false));
                            }

                            target.DispatchEvent(new MouseEvent(EventType.MouseOver, true, false,
                                null, // todo: put view here
                                0, // todo: put detail here
                                e.X, e.Y, e.X, e.Y,
                                false, false, false, false,
                                0, null, false));
                        }
                        break;
                    }
                case "mousedown":
                    target.DispatchEvent(new MouseEvent(EventType.MouseDown, true, false,
                        null, // todo: put view here
                        0, // todo: put detail here
                        e.X, e.Y, e.X, e.Y,
                        false, false, false, false,
                        0, null, false));
                    _currentDownTarget = target;
                    _currentDownX = e.X;
                    _currentDownY = e.Y;
                    break;
                case "mouseup":
                    target.DispatchEvent(new MouseEvent(EventType.MouseUp, true, false,
                        null, // todo: put view here
                        0, // todo: put detail here
                        e.X, e.Y, e.X, e.Y,
                        false, false, false, false,
                        0, null, false));
                    if (Math.Abs(_currentDownX - e.X) < 5 && Math.Abs(_currentDownY - e.Y) < 5)
                    {
                        target.DispatchEvent(new MouseEvent(EventType.Click, true, false,
                            null, // todo: put view here
                            0, // todo: put detail here
                            e.X, e.Y, e.X, e.Y,
                            false, false, false, false,
                            0, null, false));
                    }
                    _currentDownTarget = null;
                    _currentDownX = 0;
                    _currentDownY = 0;
                    break;
            }
            _currentTarget = target;
        }

        /// <summary>
        /// BeforeRender - Make sure we have a Graphics object to render to.
        /// If we don't have one, then create one to match the SvgWindow's
        /// physical dimensions.
        /// </summary>
        private void RendererBeforeRender()
        {
            // Testing for null here allows "advanced" developers to create their own Graphics object for rendering
            if (_graphics == null)
            {
                // Get the current SVGWindow's width and height
                int innerWidth  = (int)_svgWindow.InnerWidth;
                int innerHeight = (int)_svgWindow.InnerHeight;

                // Make sure we have an actual area to render to
                if (innerWidth > 0 && innerHeight > 0)
                {
                    // See if we already have a rasterImage that matches the current SVGWindow dimensions
                    if (_rasterImage == null || _rasterImage.Width != innerWidth || _rasterImage.Height != innerHeight)
                    {
                        // Nope, so create one
                        if (_rasterImage != null)
                        {
                            _rasterImage.Dispose();
                            _rasterImage = null;
                        }
                        _rasterImage = new Bitmap(innerWidth, innerHeight);
                    }

                    // Make a GraphicsWrapper object from the rasterImage and clear it to the background color
//                    _graphics = GdiGraphicsWrapper.FromImage(_rasterImage, _isStatic);
                    _graphics = GdiGraphicsImpl.FromImage(_rasterImage, _isStatic);
                    _graphics.Clear(_backColor);

                    _hitTestHelper = _graphics.HitTestHelper;
                }
            }
        }

        /// <summary>
        /// AfterRender - Dispose of Graphics object created for rendering.
        /// </summary>
        private void RendererAfterRender()
        {
            if (_graphics != null)
            {
                if (_hitTestHelper != null)
                {
                    if (_hitTestHelper != _graphics.HitTestHelper)
                    {
                        _hitTestHelper.Dispose();
                        _hitTestHelper = _graphics.HitTestHelper;
                    }
                }

                _graphics.HitTestHelper = GdiHitTestHelper.NoHit; // Prevent disposal actual height test
                _graphics.Dispose();
                _graphics = null;
            }

            if (_hitTestHelper == null)
            {
                _hitTestHelper = GdiHitTestHelper.NoHit;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_hitTestHelper != null)
            {
                _hitTestHelper.Dispose();
            }
            if (_disposeRaster && _rasterImage != null)
            {
                _rasterImage.Dispose();
            }
            if (_graphics != null)
            {
                _graphics.Dispose();
            }

            _graphics      = null;
            _rasterImage   = null;
            _hitTestHelper = null;
        }

        #endregion
    }
}
