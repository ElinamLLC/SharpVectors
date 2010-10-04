// <developer>kevin@kevlindev.com</developer>
// <completed>0</completed>

using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>      
    /// Defines the interface required for a rendering node to interact with the renderer and the SVG DOM
    /// </summary>
    public abstract class GdiRenderingBase : IDisposable
    {
        #region Private Fields

        protected SvgElement element;
        protected SvgRectF screenRegion;

        #endregion

        #region Constructors and Destructor

        protected GdiRenderingBase(SvgElement element)
        {
            this.element      = element;
            this.screenRegion = SvgRectF.Empty;
        }

        ~GdiRenderingBase()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public SvgElement Element
        {
            get { return element; }
        }

        public SvgRectF ScreenRegion
        {
            get { return screenRegion; }
            set { screenRegion = value; }
        }

        public virtual bool IsRecursive
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool NeedRender(GdiGraphicsRenderer renderer)
        {
            // We make this assumption so that the first pass is still fast
            // That way we don't have to calculate the screen regions
            // Before a full rendering
            if (screenRegion == SvgRectF.Empty)
                return true;
            if (renderer.InvalidRect == SvgRectF.Empty)
                return true;
            if (renderer.InvalidRect.Intersects(screenRegion))
                // TODO: Eventually add a full path check here?
                return true;

            return false;
        }

        // define empty handlers by default
        public virtual void BeforeRender(GdiGraphicsRenderer renderer) { }
        public virtual void Render(GdiGraphicsRenderer renderer) { }
        public virtual void AfterRender(GdiGraphicsRenderer renderer) { }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}


