using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Wraps a Graphics object since it's sealed
    /// </summary>
    internal sealed class GdiGraphicsImpl : GdiGraphics
    {
        #region Private Fields

        private GdiGraphicsListener _listenerGraphics;

        #endregion

        #region Constructors and Destructor

        private GdiGraphicsImpl(Image image, bool isStatic)
            : base(isStatic)
        {
            if (!isStatic)
            {
                _listenerGraphics = new GdiGraphicsListenerImpl(image, isStatic);
            }
            _graphics = Graphics.FromImage(image);
        }

        #endregion

        #region Public Properties

        public override SmoothingMode SmoothingMode
        {
            get { return _graphics.SmoothingMode; }
            set { _graphics.SmoothingMode = value; }
        }

        public override Matrix Transform
        {
            get { return _graphics.Transform; }
            set {
                if (_listenerGraphics != null)
                    _listenerGraphics.Transform = value;

                _graphics.Transform = value;
            }
        }

        public override GdiHitTestHelper HitTestHelper
        {
            get {
                if (_listenerGraphics != null)
                {
                    return _listenerGraphics.HitTestHelper;
                }
                return GdiHitTestHelper.NoHit;
            }
            set {
                if (_listenerGraphics != null)
                {
                    _listenerGraphics.HitTestHelper = value;
                }
            }
        }

        #endregion

        #region Public Methods

        internal static GdiGraphicsImpl FromImage(Image image, bool isStatic)
        {
            return new GdiGraphicsImpl(image, isStatic);
        }

        #endregion

        #region Public Graphics members

        public override void Clear(Color color)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.Clear(color);

            _graphics.Clear(color);
        }

        public override GdiGraphicsContainer BeginContainer()
        {
            GdiGraphicsContainerImpl container = new GdiGraphicsContainerImpl();
            if (_listenerGraphics != null)
                container.Listener = _listenerGraphics.BeginContainer();

            container.Container = _graphics.BeginContainer();

            return container;
        }

        public override void EndContainer(GdiGraphicsContainer container)
        {
            GdiGraphicsContainerImpl graphicsContainer = container as GdiGraphicsContainerImpl;
            if (graphicsContainer == null)
            {
                return;
            }

            if (_listenerGraphics != null)
                _listenerGraphics.EndContainer(graphicsContainer.Listener);

            _graphics.EndContainer(graphicsContainer.Container);
        }

        public override void SetClip(GraphicsPath path)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.SetClip(path);

            _graphics.SetClip(path);
        }

        public override void SetClip(RectangleF rect)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.SetClip(rect);

            _graphics.SetClip(rect);
        }

        public override void SetClip(Region region, CombineMode combineMode)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.SetClip(region, combineMode);

            _graphics.SetClip(region, combineMode);
        }

        public override void TranslateClip(float x, float y)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.TranslateClip(x, y);

            _graphics.TranslateClip(x, y);
        }

        public override void ResetClip()
        {
            if (_listenerGraphics != null)
                _listenerGraphics.ResetClip();

            _graphics.ResetClip();
        }

        public override void FillPath(GdiRendering grNode, Brush brush, GraphicsPath path)
        {
            if (_listenerGraphics != null)
            {
                _listenerGraphics.FillPath(grNode, brush, path);
            }
            _graphics.FillPath(brush, path);
        }

        public override void DrawPath(GdiRendering grNode, Pen pen, GraphicsPath path)
        {
            if (_listenerGraphics != null)
            {
                _listenerGraphics.DrawPath(grNode, pen, path);
            }
            _graphics.DrawPath(pen, path);
        }

        public override void TranslateTransform(float dx, float dy)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.TranslateTransform(dx, dy);

            _graphics.TranslateTransform(dx, dy);
        }

        public override void ScaleTransform(float sx, float sy)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.ScaleTransform(sx, sy);

            _graphics.ScaleTransform(sx, sy);
        }

        public override void RotateTransform(float angle)
        {
            if (_listenerGraphics != null)
                _listenerGraphics.RotateTransform(angle);

            _graphics.RotateTransform(angle);
        }

        public override void DrawImage(GdiRendering grNode, Image image, Rectangle destRect,
            float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes imageAttributes)
        {
            if (_listenerGraphics != null)
            {
                _listenerGraphics.DrawImage(grNode, image, destRect, srcX, srcY, srcWidth, 
                    srcHeight, graphicsUnit, imageAttributes);
            }
            _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, graphicsUnit, imageAttributes);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_listenerGraphics != null)
            {
                _listenerGraphics.Dispose();
                _listenerGraphics = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Inner Classes

        /// <summary>
        /// Wraps a GraphicsContainer because it is sealed.
        /// This is a helper for GraphicsWrapper so that it can save
        /// multiple container states. It holds the containers
        /// for both the idMapGraphics and the main graphics
        /// being rendered in the GraphicsWrapper.
        /// </summary>
        private sealed class GdiGraphicsContainerImpl : GdiGraphicsContainer
        {
            public GraphicsContainer Container;
            public GdiGraphicsContainer Listener;

            public GdiGraphicsContainerImpl()
            {
            }

            public GdiGraphicsContainerImpl(GraphicsContainer container, GdiGraphicsContainer listener)
            {
                this.Container = container;
                this.Listener  = listener;
            }
        }

        #endregion
    }

}
