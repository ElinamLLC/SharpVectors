using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SharpVectors.Renderers.Gdi
{
    public abstract class GdiGraphics : IDisposable
    {
        #region Private/Protected Fields

        private bool _isDisposed; // To detect redundant calls
        protected Graphics _graphics;
        protected bool _isStatic;

        #endregion

        #region Constructors and Destructor

        protected GdiGraphics(bool isStatic)
        {
            _isStatic = isStatic;
        }

        ~GdiGraphics()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public abstract GdiHitTestHelper HitTestHelper { get; set; }

        #endregion

        #region Private Fields

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
            protected set {
                _isDisposed = value;
            }
        }

        public bool IsStatic
        {
            get {
                return _isStatic;
            }
            protected set {
                _isStatic = value;
            }
        }

        public virtual Graphics Graphics
        {
            get {
                return _graphics;
            }
        }

        public virtual Matrix Transform
        {
            get {
                if (_graphics != null)
                {
                    return _graphics.Transform;
                }
                return new Matrix();
            }
            set {
                if (_graphics != null)
                {
                    _graphics.Transform = value;
                }
            }
        }

        public virtual SmoothingMode SmoothingMode
        {
            get {
                if (_graphics != null)
                {
                    return _graphics.SmoothingMode;
                }
                return SmoothingMode.Default;
            }
            set {
                if (_graphics != null)
                {
                    _graphics.SmoothingMode = value;
                }
            }
        }

        #endregion

        #region Public Static Methods

        public static GdiGraphics Create(Image image, bool isStatic)
        {
            return GdiGraphicsWrapper.FromImage(image, isStatic);
        }

        #endregion

        #region Public Abstract Methods

        public void DrawImage(GdiRendering grNode, Image image, RectangleF destRect, 
            RectangleF srcRect, GraphicsUnit graphicsUnit, ImageAttributes imageAttributes)
        {
            this.DrawImage(grNode, image, destRect.Snap(), 
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, graphicsUnit, imageAttributes);
        }

        public abstract GdiGraphicsContainer BeginContainer();
        public abstract void Clear(Color color);
        public abstract void DrawImage(GdiRendering grNode, Image image, Rectangle destRect, 
            float srcX, float srcY, float srcWidth, float srcHeight, 
            GraphicsUnit graphicsUnit, ImageAttributes imageAttributes);
        public abstract void DrawPath(GdiRendering grNode, Pen pen, GraphicsPath path);
        public abstract void EndContainer(GdiGraphicsContainer container);
        public abstract void FillPath(GdiRendering grNode, Brush brush, GraphicsPath path);
        public abstract void ResetClip();
        public abstract void RotateTransform(float angle);
        public abstract void ScaleTransform(float sx, float sy);
        public abstract void SetClip(GraphicsPath path);
        public abstract void SetClip(RectangleF rect);
        public abstract void SetClip(Region region, CombineMode combineMode);
        public abstract void TranslateClip(float x, float y);
        public abstract void TranslateTransform(float dx, float dy);

        #endregion

        #region IDisposable Members

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        #endregion
    }
}