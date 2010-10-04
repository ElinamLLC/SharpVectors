using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Wraps a Graphics object since it's sealed
    /// </summary>
    public sealed class GdiGraphicsWrapper : IDisposable
    {
        #region Private Fields

        private bool _isStatic;
        private Graphics _graphics;
        private Graphics _idMapGraphics;
        private Bitmap _idMapImage;
        
        #endregion

        #region Constructors

        private GdiGraphicsWrapper(Image image, bool isStatic)
        {
            this._isStatic = isStatic;
            if (!IsStatic)
            {
                _idMapImage = new Bitmap(image.Width, image.Height);
                _idMapGraphics = Graphics.FromImage(_idMapImage);
                _idMapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                _idMapGraphics.SmoothingMode = SmoothingMode.None;
                _idMapGraphics.CompositingQuality = CompositingQuality.Invalid;
            }
            _graphics = Graphics.FromImage(image);
        }

        private GdiGraphicsWrapper(IntPtr hdc, bool isStatic)
        {
            this._isStatic = isStatic;
            if (!IsStatic)
            {
                // This will get resized when the actual size is known
                _idMapImage = new Bitmap(0, 0);
                _idMapGraphics = Graphics.FromImage(_idMapImage);
                _idMapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                _idMapGraphics.SmoothingMode = SmoothingMode.None;
                _idMapGraphics.CompositingQuality = CompositingQuality.Invalid;
            }
            _graphics = Graphics.FromHdc(hdc);
        }

        #endregion

        public static GdiGraphicsWrapper FromImage(Image image, bool isStatic)
        {
            return new GdiGraphicsWrapper(image, isStatic);
        }

        public static GdiGraphicsWrapper FromHdc(IntPtr hdc, bool isStatic)
        {
            return new GdiGraphicsWrapper(hdc, isStatic);
        }

        #region Properties

        public bool IsStatic
        {
            get { return _isStatic; }
            set
            {
                _isStatic = value;
                _idMapGraphics.Dispose();
                _idMapGraphics = null;
            }
        }

        public Graphics Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        public Graphics IdMapGraphics
        {
            get { return _graphics; }
        }

        public Bitmap IdMapRaster
        {
            get { return _idMapImage; }
        }

        #endregion

        #region Graphics members

        public void Clear(Color color)
        {
            _graphics.Clear(color);
            if (_idMapGraphics != null) _idMapGraphics.Clear(Color.Empty);
        }

        public void Dispose()
        {
            _graphics.Dispose();
            if (_idMapGraphics != null) _idMapGraphics.Dispose();
        }

        public GdiGraphicsContainer BeginContainer()
        {
            GdiGraphicsContainer container = new GdiGraphicsContainer();
            if (_idMapGraphics != null) 
                container.IdMap = _idMapGraphics.BeginContainer();

            container.Main = _graphics.BeginContainer();
            
            return container;
        }

        public void EndContainer(GdiGraphicsContainer container)
        {
            if (_idMapGraphics != null) 
                _idMapGraphics.EndContainer(container.IdMap);

            _graphics.EndContainer(container.Main);
        }

        public SmoothingMode SmoothingMode
        {
            get { return _graphics.SmoothingMode; }
            set { _graphics.SmoothingMode = value; }
        }

        public Matrix Transform
        {
            get { return _graphics.Transform; }
            set
            {
                if (_idMapGraphics != null) _idMapGraphics.Transform = value;
                _graphics.Transform = value;
            }
        }

        public void SetClip(GraphicsPath path)
        {
            _graphics.SetClip(path);
        }

        public void SetClip(RectangleF rect)
        {
            if (_idMapGraphics != null) _idMapGraphics.SetClip(rect);
            _graphics.SetClip(rect);
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            if (_idMapGraphics != null) _idMapGraphics.SetClip(region, combineMode);
            _graphics.SetClip(region, combineMode);
        }

        public void TranslateClip(float x, float y)
        {
            if (_idMapGraphics != null) _idMapGraphics.TranslateClip(x, y);
            _graphics.TranslateClip(x, y);
        }

        public void ResetClip()
        {
            if (_idMapGraphics != null) _idMapGraphics.ResetClip();
            _graphics.ResetClip();
        }

        public void FillPath(GdiRendering grNode, Brush brush, GraphicsPath path)
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

        public void DrawPath(GdiRendering grNode, Pen pen, GraphicsPath path)
        {
            if (_idMapGraphics != null)
            {
                Pen idPen = new Pen(grNode.UniqueColor, pen.Width);
                _idMapGraphics.DrawPath(idPen, path);
            }
            _graphics.DrawPath(pen, path);
        }

        public void TranslateTransform(float dx, float dy)
        {
            if (_idMapGraphics != null) _idMapGraphics.TranslateTransform(dx, dy);
            _graphics.TranslateTransform(dx, dy);
        }

        public void ScaleTransform(float sx, float sy)
        {
            if (_idMapGraphics != null) _idMapGraphics.ScaleTransform(sx, sy);
            _graphics.ScaleTransform(sx, sy);
        }

        public void RotateTransform(float angle)
        {
            if (_idMapGraphics != null) _idMapGraphics.RotateTransform(angle);
            _graphics.RotateTransform(angle);
        }

        public void DrawImage(GdiRendering grNode, Image image, Rectangle destRect, 
            float srcX, float srcY, float srcWidth, float srcHeight, 
            GraphicsUnit graphicsUnit, ImageAttributes imageAttributes)
        {
            if (_idMapGraphics != null)
            {
                // This handles pointer-events for visibleFill visibleStroke and visible
                /*Brush idBrush = new SolidBrush(grNode.UniqueColor);
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(destRect);
                _idMapGraphics.FillPath(idBrush, gp);*/
                Color unique = grNode.UniqueColor;
                float r = (float)unique.R / 255;
                float g = (float)unique.G / 255;
                float b = (float)unique.B / 255;
                ColorMatrix colorMatrix = new ColorMatrix(
                  new float[][] { new float[] {0f, 0f, 0f, 0f, 0f},
                          new float[] {0f, 0f, 0f, 0f, 0f},
                          new float[] {0f, 0f, 0f, 0f, 0f},
                          new float[] {0f, 0f, 0f, 1f, 0f},
                          new float[] {r,  g,  b,  0f, 1f} });
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                _idMapGraphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, graphicsUnit, ia);
            }
            _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, graphicsUnit, imageAttributes);
        }

        #endregion
    }

    /// <summary>
    /// Wraps a GraphicsContainer because it is sealed.
    /// This is a helper for GraphicsWrapper so that it can save
    /// multiple container states. It holds the containers
    /// for both the idMapGraphics and the main graphics
    /// being rendered in the GraphicsWrapper.
    /// </summary>
    public sealed class GdiGraphicsContainer
    {
        internal GraphicsContainer IdMap;
        internal GraphicsContainer Main;

        public GdiGraphicsContainer()
        {   
        }

        public GdiGraphicsContainer(GraphicsContainer idmap, GraphicsContainer main)
        {
            this.IdMap = idmap;
            this.Main  = main;
        }
    }
}
