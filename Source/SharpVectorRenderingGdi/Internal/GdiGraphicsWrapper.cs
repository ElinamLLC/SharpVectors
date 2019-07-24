using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Wraps a Graphics object since it's sealed
    /// </summary>
    internal sealed class GdiGraphicsWrapper : GdiGraphics
    {
        #region Private Fields

        private Graphics _idMapGraphics;
        private Bitmap _idMapImage;

        #endregion

        #region Constructors and Destructor

        private GdiGraphicsWrapper(Image image, bool isStatic)
            : base(isStatic)
        {
            if (!isStatic)
            {
                _idMapImage    = new Bitmap(image.Width, image.Height, image.PixelFormat);
                _idMapGraphics = Graphics.FromImage(_idMapImage);
                _idMapGraphics.InterpolationMode  = InterpolationMode.NearestNeighbor;
                _idMapGraphics.SmoothingMode      = SmoothingMode.None;
                _idMapGraphics.CompositingQuality = CompositingQuality.Invalid;
            }
            _graphics = Graphics.FromImage(image);

            this.HitTestHelper = GdiHitTestHelper.NoHit;
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
                if (_idMapGraphics != null)
                    _idMapGraphics.Transform = value;

                _graphics.Transform = value;
            }
        }

        public override GdiHitTestHelper HitTestHelper { get; set; }
        
        #endregion

        #region Internal and Methods Properties

        internal Graphics IdMapGraphics
        {
            get { return _graphics; }
        }

        internal Bitmap IdMapRaster
        {
            get { return _idMapImage; }
        }

        #endregion

        #region Public Methods

        internal static GdiGraphicsWrapper FromImage(Image image, bool isStatic)
        {
            return new GdiGraphicsWrapper(image, isStatic);
        }

        #endregion

        #region Public Graphics members

        public override void Clear(Color color)
        {
            _graphics.Clear(color);
            if (_idMapGraphics != null)
                _idMapGraphics.Clear(Color.Empty);
        }

        public override GdiGraphicsContainer BeginContainer()
        {
            GdiGraphicsContainerImpl container = new GdiGraphicsContainerImpl();
            if (_idMapGraphics != null)
                container.IdMap = _idMapGraphics.BeginContainer();

            container.Main = _graphics.BeginContainer();

            return container;
        }

        public override void EndContainer(GdiGraphicsContainer container)
        {
            GdiGraphicsContainerImpl graphicsContainer = container as GdiGraphicsContainerImpl;
            if (graphicsContainer == null)
            {
                return;
            }

            if (_idMapGraphics != null)
                _idMapGraphics.EndContainer(graphicsContainer.IdMap);

            _graphics.EndContainer(graphicsContainer.Main);
        }

        public override void SetClip(GraphicsPath path)
        {
            _graphics.SetClip(path);
        }

        public override void SetClip(RectangleF rect)
        {
            if (_idMapGraphics != null) _idMapGraphics.SetClip(rect);
            _graphics.SetClip(rect);
        }

        public override void SetClip(Region region, CombineMode combineMode)
        {
            if (_idMapGraphics != null) _idMapGraphics.SetClip(region, combineMode);
            _graphics.SetClip(region, combineMode);
        }

        public override void TranslateClip(float x, float y)
        {
            if (_idMapGraphics != null) _idMapGraphics.TranslateClip(x, y);
            _graphics.TranslateClip(x, y);
        }

        public override void ResetClip()
        {
            if (_idMapGraphics != null) _idMapGraphics.ResetClip();
            _graphics.ResetClip();
        }

        public override void FillPath(GdiRendering grNode, Brush brush, GraphicsPath path)
        {
            if (_idMapGraphics != null)
            {
                Brush idBrush = new SolidBrush(grNode.UniqueColor);
                if (grNode.Element is SvgTextContentElement)
                {
                    _idMapGraphics.FillRectangle(idBrush, path.GetBounds());
                }
                else
                {
                    _idMapGraphics.FillPath(idBrush, path);
                }
            }
            _graphics.FillPath(brush, path);
        }

        public override void DrawPath(GdiRendering grNode, Pen pen, GraphicsPath path)
        {
            if (_idMapGraphics != null)
            {
                Pen idPen = new Pen(grNode.UniqueColor, pen.Width);
                _idMapGraphics.DrawPath(idPen, path);
            }
            _graphics.DrawPath(pen, path);
        }

        public override void TranslateTransform(float dx, float dy)
        {
            if (_idMapGraphics != null) _idMapGraphics.TranslateTransform(dx, dy);
            _graphics.TranslateTransform(dx, dy);
        }

        public override void ScaleTransform(float sx, float sy)
        {
            if (_idMapGraphics != null) _idMapGraphics.ScaleTransform(sx, sy);
            _graphics.ScaleTransform(sx, sy);
        }

        public override void RotateTransform(float angle)
        {
            if (_idMapGraphics != null) _idMapGraphics.RotateTransform(angle);
            _graphics.RotateTransform(angle);
        }

        public override void DrawImage(GdiRendering grNode, Image image, Rectangle destRect,
            float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes imageAttributes)
        {
            if (_idMapGraphics != null)
            {
                // This handles pointer-events for visibleFill visibleStroke and visible
                Color unique = grNode.UniqueColor;
                float r = (float)unique.R / 255;
                float g = (float)unique.G / 255;
                float b = (float)unique.B / 255;
                ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                    new float[] {0f, 0f, 0f, 0f, 0f},
                    new float[] {0f, 0f, 0f, 0f, 0f},
                    new float[] {0f, 0f, 0f, 0f, 0f},
                    new float[] {0f, 0f, 0f, 1f, 0f},
                    new float[] {r,  g,  b,  0f, 1f}
                });
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                _idMapGraphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, graphicsUnit, ia);
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
            if (_idMapGraphics != null)
            {
                _idMapGraphics.Dispose();
                _idMapGraphics = null;
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
            public GraphicsContainer IdMap;
            public GraphicsContainer Main;

            public GdiGraphicsContainerImpl()
            {
            }

            public GdiGraphicsContainerImpl(GraphicsContainer idmap, GraphicsContainer main)
            {
                this.IdMap = idmap;
                this.Main = main;
            }
        }

        #endregion
    }
}
