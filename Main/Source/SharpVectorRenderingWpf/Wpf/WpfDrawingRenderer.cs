using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections.Generic; 

using SharpVectors.Xml;
using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfDrawingRenderer : DependencyObject, ISvgRenderer, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The renderer's <see cref="SvgWindow">SvgWindow</see> object.
        /// </summary>
        private ISvgWindow              _svgWindow;

        private WpfDrawingContext       _renderingContext;

        private WpfRenderingHelper      _svgRenderer;
 
        private WpfLinkVisitor          _linkVisitor;
        private WpfFontFamilyVisitor    _fontFamilyVisitor;
        private WpfEmbeddedImageVisitor _imageVisitor;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingRenderer()
        {
            _svgRenderer = new WpfRenderingHelper(this);
        }

        ~WpfDrawingRenderer()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public Drawing Drawing
        {
            get
            {
                if (_renderingContext == null)
                {
                    return null;
                }

                return _renderingContext.Root;
            }
        }

        public WpfDrawingContext Context
        {
            get
            {
                return _renderingContext;
            }
        }

        public WpfLinkVisitor LinkVisitor
        {
            get
            {
                return _linkVisitor;
            }
            set
            {
                _linkVisitor = value;
            }
        }

        public WpfEmbeddedImageVisitor ImageVisitor
        {
            get
            {
                return _imageVisitor;
            }
            set
            {
                _imageVisitor = value;
            }
        }

        public WpfFontFamilyVisitor FontFamilyVisitor
        {
            get
            {
                return _fontFamilyVisitor;
            }
            set
            {
                _fontFamilyVisitor = value;
            }
        }

        #endregion

        #region ISvgRenderer Members

        public ISvgWindow Window
        {
            get
            {
                return _svgWindow;
            }
            set
            {
                _svgWindow = value;
            }
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

            _renderingContext = new WpfDrawingContext(true);

            _renderingContext.Initialize(null, _fontFamilyVisitor, _imageVisitor);

            _renderingContext.BeginDrawing();

            _svgRenderer.Render(node);

            _renderingContext.EndDrawing();
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

            if (context == null)
            {
                _renderingContext = new WpfDrawingContext(true);

                _renderingContext.Initialize(null, _fontFamilyVisitor, _imageVisitor);
            }
            else
            {
                _renderingContext = context;
            }

            _renderingContext.BeginDrawing();

            _svgRenderer.Render(node);

            _renderingContext.EndDrawing();
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
            _renderingContext = new WpfDrawingContext(false);

            _renderingContext.Initialize(_linkVisitor, _fontFamilyVisitor, _imageVisitor);

            _renderingContext.BeginDrawing();

            _svgRenderer.Render(node);

            _renderingContext.EndDrawing();

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);
        }

        public SvgRectF InvalidRect
        {
            get
            {
                return SvgRectF.Empty;
            }
            set
            {
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
                _renderingContext = new WpfDrawingContext(true);

                _renderingContext.Initialize(null, _fontFamilyVisitor, _imageVisitor);
            }
            else
            {
                _renderingContext = context;
            }

            _renderingContext.BeginDrawing();

            _svgRenderer.RenderMask(node);

            _renderingContext.EndDrawing();
        }

        public void InvalidateRect(SvgRectF rect)
        {
        }

        public RenderEvent OnRender
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            return SvgRect.Empty;
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
        }

        #endregion
    }
}
