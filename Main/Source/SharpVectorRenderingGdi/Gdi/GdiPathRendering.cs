using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiPathRendering : GdiRendering
	{
        #region Constructor and Destructor

		public GdiPathRendering(SvgElement element) 
            : base(element)
		{
		}

        #endregion

		#region Public Methods

        public override void BeforeRender(GdiGraphicsRenderer renderer)
        {
            if (_uniqueColor.IsEmpty)
        		  _uniqueColor = renderer.GetNextColor(this.Element);

            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;

            _graphicsContainer = graphics.BeginContainer();
            SetQuality(graphics);
            Transform(graphics);
        }

        public override void Render(GdiGraphicsRenderer renderer)
        {
            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;

            SvgRenderingHint hint = element.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            if (element.ParentNode is SvgClipPathElement)
            {
                return;
            }

            SvgStyleableElement styleElm = (SvgStyleableElement)element;

			string sVisibility = styleElm.GetPropertyValue("visibility");
			string sDisplay    = styleElm.GetPropertyValue("display");
            if (String.Equals(sVisibility, "hidden") || String.Equals(sDisplay, "none"))
            {
                return;
            }

            GraphicsPath gp = CreatePath(element);

			if (gp != null)
			{
				Clip(graphics);

				GdiSvgPaint fillPaint = new GdiSvgPaint(styleElm, "fill");
				Brush brush = fillPaint.GetBrush(gp);

				GdiSvgPaint strokePaint = new GdiSvgPaint(styleElm, "stroke");
				Pen pen = strokePaint.GetPen(gp);

				if (brush != null) 
				{
					if (brush is PathGradientBrush) 
					{
						GdiGradientFill gps = fillPaint.PaintFill as GdiGradientFill;
						
						graphics.SetClip(gps.GetRadialGradientRegion(gp.GetBounds()), CombineMode.Exclude);

						SolidBrush tempBrush = new SolidBrush(((PathGradientBrush)brush).InterpolationColors.Colors[0]);
						graphics.FillPath(this, tempBrush,gp);
						tempBrush.Dispose();
						graphics.ResetClip();
					}

					graphics.FillPath(this, brush, gp);
					brush.Dispose();
                    brush = null;
				}

				if (pen != null) 
				{
					if (pen.Brush is PathGradientBrush) 
					{
						GdiGradientFill gps = strokePaint.PaintFill as GdiGradientFill;
						GdiGraphicsContainer container = graphics.BeginContainer();

						graphics.SetClip(gps.GetRadialGradientRegion(gp.GetBounds()), CombineMode.Exclude);

						SolidBrush tempBrush = new SolidBrush(((PathGradientBrush)pen.Brush).InterpolationColors.Colors[0]);
						Pen tempPen = new Pen(tempBrush, pen.Width);
						graphics.DrawPath(this, tempPen,gp);
						tempPen.Dispose();
						tempBrush.Dispose();

						graphics.EndContainer(container);
					}

					graphics.DrawPath(this, pen, gp);
					pen.Dispose();
                    pen = null;
				}

                gp.Dispose();
                gp = null;
			}

			PaintMarkers(renderer, styleElm, graphics);
        }

        #endregion

        #region Private Methods

        private Brush GetBrush(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(element as SvgStyleableElement, "fill");
            return paint.GetBrush(gp);
        }

        private Pen GetPen(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(element as SvgStyleableElement, "stroke");
            return paint.GetPen(gp);
        }
		
        #endregion
    }
}
