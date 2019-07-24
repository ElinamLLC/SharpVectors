using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;
using System.Drawing.Imaging;

namespace SharpVectors.Renderers.Gdi
{
    internal sealed class GdiGraphicsListenerImpl : GdiGraphicsListener
    {
        #region Constructors and Destructor

        public GdiGraphicsListenerImpl(Image image, bool isStatic)
            : base(isStatic)
        {
            var hitTestHelper = new GdiHitTestHelperImpl(image.Width, image.Height, image.PixelFormat);
            this.HitTestHelper = hitTestHelper;

            _graphics                    = Graphics.FromImage(hitTestHelper.IdMapRaster);
            _graphics.InterpolationMode  = InterpolationMode.NearestNeighbor;
            _graphics.SmoothingMode      = SmoothingMode.None;
            _graphics.CompositingQuality = CompositingQuality.Invalid;
        }

        #endregion

        #region Public Methods

        public override void Clear(Color color)
        {
            if (_graphics != null)
            {
                _graphics.Clear(Color.Empty);
            }
        }

        public override GdiGraphicsContainer BeginContainer()
        {
            if (_graphics != null)
            {
                return new GdiGraphicsContainerImpl(_graphics.BeginContainer());
            }

            return null;
        }

        public override void EndContainer(GdiGraphicsContainer container)
        {
            if (_graphics == null)
            {
                return;
            }

            var graphicsContainer = container as GdiGraphicsContainerImpl;
            if (graphicsContainer == null)
            {
                return;
            }
            _graphics.EndContainer(graphicsContainer.Container);
        }

        public override void SetClip(GraphicsPath path)
        {
            if (_graphics != null)
            {
                _graphics.SetClip(path);
            }
        }

        public override void SetClip(RectangleF rect)
        {
            if (_graphics != null)
            {
                _graphics.SetClip(rect);
            }
        }

        public override void SetClip(Region region, CombineMode combineMode)
        {
            if (_graphics != null)
            {
                _graphics.SetClip(region, combineMode);
            }
        }

        public override void TranslateClip(float x, float y)
        {
            if (_graphics != null)
            {
                _graphics.TranslateClip(x, y);
            }
        }

        public override void ResetClip()
        {
            if (_graphics != null)
            {
                _graphics.ResetClip();
            }
        }

        public override void FillPath(GdiRendering grNode, Brush brush, GraphicsPath path)
        {
            if (_graphics != null)
            {
                Brush idBrush = new SolidBrush(grNode.UniqueColor);
                if (grNode.Element is SvgTextContentElement)
                {
                    _graphics.FillRectangle(idBrush, path.GetBounds());
                }
                else
                {
                    _graphics.FillPath(idBrush, path);
                }
            }
        }

        public override void DrawPath(GdiRendering grNode, Pen pen, GraphicsPath path)
        {
            if (_graphics != null)
            {
                Pen idPen = new Pen(grNode.UniqueColor, pen.Width);
                _graphics.DrawPath(idPen, path);
            }
        }

        public override void TranslateTransform(float dx, float dy)
        {
            if (_graphics != null)
            {
                _graphics.TranslateTransform(dx, dy);
            }
        }

        public override void ScaleTransform(float sx, float sy)
        {
            if (_graphics != null)
            {
                _graphics.ScaleTransform(sx, sy);
            }
        }

        public override void RotateTransform(float angle)
        {
            if (_graphics != null)
            {
                _graphics.RotateTransform(angle);
            }
        }

        public override void DrawImage(GdiRendering grNode, Image image, Rectangle destRect,
            float srcX, float srcY, float srcWidth, float srcHeight,
            GraphicsUnit graphicsUnit, ImageAttributes imageAttributes)
        {
            if (_graphics != null)
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
                _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, graphicsUnit, ia);
            }
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

            public GdiGraphicsContainerImpl(GraphicsContainer container)
            {
                this.Container = container;
            }
        }

        #endregion
    }
}
