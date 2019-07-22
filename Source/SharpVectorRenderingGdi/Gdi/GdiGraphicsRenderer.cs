using System;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
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

        /// <summary>
        /// A counter that tracks the next hit color.
        /// </summary>
        private int _colorCounter;

        /// <summary>
        /// Maps a 'hit color' to a graphics node.
        /// </summary>
        /// <remarks>
        /// The 'hit color' is an integer identifier that identifies the graphics node that drew it.  
        /// When 'hit colors' are drawn onto a bitmap (ie. <see cref="_idMapRaster">id-mapppe raster</see> 
        /// the 'hit color' of each pixel with the help of <see cref="_colorMap">color map</see> 
        /// can identify for a given x, y coordinate the relevant graphics node a mouse event should be dispatched to.
        /// </remarks>
        private IDictionary<Color, SvgElement> _colorMap;

        /// <summary>
        /// The bitmap containing the rendered Svg image.
        /// </summary>
        private Bitmap _rasterImage;

        /// <summary>
        /// A secondary back-buffer used for invalidation repaints. The invalidRect will
        /// be bitblt to the rasterImage front buffer
        /// </summary>
        private Bitmap _invalidatedRasterImage;

        /// <summary>
        /// A bitmap image that consists of 'hit color' instead of visual color.  A 'hit color' is an 
        /// integer identifier that identifies the graphics node that drew it.  A 'hit color' can 
        /// therefore identify the graphics node that corresponds an x-y coordinates.
        /// </summary>
        private Bitmap _idMapRaster;

        /// <summary>
        /// The renderer's graphics wrapper object.
        /// </summary>
        private GdiGraphicsWrapper _graphics;

        /// <summary>
        /// The renderer's back color.
        /// </summary>
        private Color _backColor;

        /// <summary>
        /// The renderer's <see cref="SvgWindow">SvgWindow</see> object.
        /// </summary>
        private ISvgWindow _svgWindow;

        /// <summary>
        /// 
        /// </summary>
        private float _currentDownX;
        private float _currentDownY;
        private IEventTarget _currentTarget;
        private IEventTarget _currentDownTarget;

        private GdiRenderingHelper _svgRenderer;

        private SvgRectF _invalidRect;

        private RenderEvent _onRender;

        private bool _isStatic;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the GdiRenderer class.
        /// </summary>
        public GdiGraphicsRenderer(bool isStatic = false)
        {
            _isStatic     = isStatic;
            _invalidRect  = SvgRectF.Empty;
            _colorCounter = 0;
            _backColor    = Color.White;
            _colorMap     = new Dictionary<Color, SvgElement>();

            _svgRenderer  = new GdiRenderingHelper(this);
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
        /// Gets the image map of the rendered Svg document. This
        /// is a picture of how the renderer will map the (x,y) positions
        /// of mouse events to objects. You can display this raster
        /// to help in debugging of hit testing.
        /// </summary>
        public Bitmap IdMapRaster
        {
            get {
                return _idMapRaster;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Window">Window</see> of the
        /// renderer.
        /// </summary>
        /// <value>
        /// The <see cref="Window">Window</see> of the renderer.
        /// </value>
        public ISvgWindow Window
        {
            get {
                return _svgWindow;
            }
            set {
                _svgWindow = value;
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
        /// Gets or sets the <see cref="GraphicsWrapper">GraphicsWrapper</see>
        /// object associated with this renderer.
        /// </summary>
        /// <value>
        /// The <see cref="GraphicsWrapper">GraphicsWrapper</see> object
        /// associated with this renderer.
        /// </value>
        public GdiGraphicsWrapper GraphicsWrapper
        {
            get {
                return _graphics;
            }
            set {
                _graphics = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Graphics">Graphics</see> object
        /// associated with this renderer.
        /// </summary>
        /// <value>
        /// The <see cref="Graphics">Graphics</see> object associated
        /// with this renderer.
        /// </value>
        public Graphics Graphics
        {
            get {
                return _graphics.Graphics;
            }
            set {
                _graphics.Graphics = value;
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
            if (_idMapRaster != null)
            {
                _idMapRaster.Dispose();
                _idMapRaster = null;
            }
            if (_invalidatedRasterImage != null)
            {
                _invalidatedRasterImage.Dispose();
                _invalidatedRasterImage = null;
            }

            this.ClearMap();
        }

        public void ClearMap()
        {
            _colorMap = null;
            _colorMap = new Dictionary<Color, SvgElement>();
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
            if (_idMapRaster == null)
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

        /// <summary>
        /// Allocate a hit color for the specified graphics node.
        /// </summary>
        /// <param name="element">
        /// The <see cref="SvgElement">SvgElement</see> object for which to
        /// allocate a new hit color.
        /// </param>
        /// <returns>
        /// The hit color for the <see cref="SvgElement">SvgElement</see>
        /// object.
        /// </returns>
        internal Color GetNextColor(SvgElement element)
        {
            //	TODO: [newhoggy] It looks like there is a potential memory leak here.
            //	We only ever add to the graphicsNodes map, never remove
            //	from it, so it will grow every time this function is called.

            // The counter is used to generate IDs in the range [0,2^24-1]
            // The 24 bits of the counter are interpreted as follows:
            // [red 7 bits | green 7 bits | blue 7 bits |shuffle term 3 bits]
            // The shuffle term is used to define how the remaining high
            // bit is set on each color. The colors are generated in the
            // range [0,127] (7 bits) instead of [0,255]. Then the shuffle term
            // is used to adjust them into the range [0,255].
            // This algorithm has the feature that consecutive ids generate
            // visually distinct colors.
            int id = _colorCounter++; // Zero should be the first color.
            int shuffleTerm = id & 7;
            int r = 0x7f & (id >> 17);
            int g = 0x7f & (id >> 10);
            int b = 0x7f & (id >> 3);

            switch (shuffleTerm)
            {
                case 0: break;
                case 1: b |= 0x80; break;
                case 2: g |= 0x80; break;
                case 3: g |= 0x80; b |= 0x80; break;
                case 4: r |= 0x80; break;
                case 5: r |= 0x80; b |= 0x80; break;
                case 6: r |= 0x80; g |= 0x80; break;
                case 7: r |= 0x80; g |= 0x80; b |= 0x80; break;
            }

            Color color = Color.FromArgb(r, g, b);

            _colorMap.Add(color, element);

            return color;
        }

        internal void RemoveColor(Color color)
        {
            if (!color.IsEmpty)
            {
                _colorMap[color] = null;
                _colorMap.Remove(color);
            }
        }

        /// <summary>
        /// Gets the <see cref="SvgElement">SvgElement</see> object that
        /// corresponds to the given hit color.
        /// </summary>
        /// <param name="color">
        /// The hit color for which to get the corresponding
        /// <see cref="SvgElement">SvgElement</see> object.
        /// </param>
        /// <remarks>
        /// Returns <c>null</c> if a corresponding
        /// <see cref="SvgElement">SvgElement</see> object cannot be
        /// found for the given hit color.
        /// </remarks>
        /// <returns>
        /// The <see cref="SvgElement">SvgElement</see> object that
        /// corresponds to the given hit color
        /// </returns>
        private SvgElement GetElementFromColor(Color color)
        {
            if (color.A == 0)
            {
                return null;
            }
            if (_colorMap.ContainsKey(color))
            {
                return _colorMap[color];
            }
            return null;
        }

        /// <summary>
        /// TODO: This method is not used.
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// </returns>
        private static int ColorToId(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int shuffleTerm = 0;

            if (0 != (r & 0x80))
            {
                shuffleTerm |= 4;
                r &= 0x7f;
            }

            if (0 != (g & 0x80))
            {
                shuffleTerm |= 2;
                g &= 0x7f;
            }

            if (0 != (b & 0x80))
            {
                shuffleTerm |= 1;
                b &= 0x7f;
            }

            return (r << 17) + (g << 10) + (b << 3) + shuffleTerm;
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
            Color pixel = _idMapRaster.GetPixel(e.X, e.Y);
            SvgElement svgElement = GetElementFromColor(pixel);

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

                    // Maybe we are only repainting an invalidated section
                    if (_invalidRect != SvgRectF.Empty)
                    {
                        // TODO: Worry about pan...
                        if (_invalidRect.X < 0)
                            _invalidRect.X = 0;
                        if (_invalidRect.Y < 0)
                            _invalidRect.Y = 0;
                        if (_invalidRect.Right > innerWidth)
                            _invalidRect.Width = innerWidth - _invalidRect.X;
                        if (_invalidRect.Bottom > innerHeight)
                            _invalidRect.Height = innerHeight - _invalidRect.Y;

                        if (_invalidatedRasterImage == null || _invalidatedRasterImage.Width < _invalidRect.Right ||
                            _invalidatedRasterImage.Height < _invalidRect.Bottom)
                        {
                            // Nope, so create one
                            if (_invalidatedRasterImage != null)
                            {
                                _invalidatedRasterImage.Dispose();
                                _invalidatedRasterImage = null;
                            }
                            _invalidatedRasterImage = new Bitmap((int)_invalidRect.Right, (int)_invalidRect.Bottom);
                        }
                        // Make a GraphicsWrapper object from the regionRasterImage and clear it to the background color
                        _graphics = GdiGraphicsWrapper.FromImage(_invalidatedRasterImage, _isStatic);

                        _graphics.Clear(_backColor);
                    }
                    else
                    {
                        // Make a GraphicsWrapper object from the rasterImage and clear it to the background color
                        _graphics = GdiGraphicsWrapper.FromImage(_rasterImage, _isStatic);
                        _graphics.Clear(_backColor);
                    }
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
                // Check if we only invalidated a rect
                if (_invalidRect != SvgRectF.Empty)
                {
                    // We actually drew everything on invalidatedRasterImage and now we
                    // need to copy that to rasterImage
                    Graphics tempGraphics = Graphics.FromImage(_rasterImage);
                    tempGraphics.DrawImage(_invalidatedRasterImage, _invalidRect.X, _invalidRect.Y,
                      GdiConverter.ToRectangle(_invalidRect), GraphicsUnit.Pixel);
                    tempGraphics.Dispose();
                    tempGraphics = null;

                    // If we currently have an idMapRaster here, then we need to create
                    // a temporary graphics object to draw the invalidated portion from
                    // our main graphics window onto it.
                    if (_idMapRaster != null)
                    {
                        tempGraphics = Graphics.FromImage(_idMapRaster);
                        tempGraphics.DrawImage(_graphics.IdMapRaster, _invalidRect.X, _invalidRect.Y,
                          GdiConverter.ToRectangle(_invalidRect), GraphicsUnit.Pixel);
                        tempGraphics.Dispose();
                        tempGraphics = null;
                    }
                    else
                    {
                        _idMapRaster = _graphics.IdMapRaster;
                    }
                    // We have updated the invalid region
                    _invalidRect = SvgRectF.Empty;
                }
                else
                {
                    if (_idMapRaster != null && _idMapRaster != _graphics.IdMapRaster)
                        _idMapRaster.Dispose();
                    _idMapRaster = _graphics.IdMapRaster;
                }

                _graphics.Dispose();
                _graphics = null;
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
            if (_idMapRaster != null)
            {
                _idMapRaster.Dispose();
                _idMapRaster = null;
            }
            if (_invalidatedRasterImage != null)
            {
                _invalidatedRasterImage.Dispose();
                _invalidatedRasterImage = null;
            }
            if (_rasterImage != null)
            {
                _rasterImage.Dispose();
                _rasterImage = null;
            }
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
        }

        #endregion
    }
}
