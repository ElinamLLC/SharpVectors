using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom;
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
        		  _uniqueColor = renderer.GetNextHitColor(this.Element);

            var graphics = renderer.GdiGraphics;

            _graphicsContainer = graphics.BeginContainer();

            SetQuality(graphics);
            SetTransform(graphics);
        }

        public override void Render(GdiGraphicsRenderer renderer)
        {
            var graphics = renderer.GdiGraphics;

            SvgRenderingHint hint = _svgElement.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            if (_svgElement.ParentNode is SvgClipPathElement)
            {
                return;
            }
            var comparer = StringComparison.OrdinalIgnoreCase;

            SvgStyleableElement styleElm = (SvgStyleableElement)_svgElement;

			string sVisibility = styleElm.GetPropertyValue(CssConstants.PropVisibility);
			string sDisplay    = styleElm.GetPropertyValue(CssConstants.PropDisplay);
            if (string.Equals(sVisibility, CssConstants.ValHidden, comparer) 
                || string.Equals(sDisplay, CssConstants.ValNone, comparer))
            {
                return;
            }

            GraphicsPath gp = CreatePath(_svgElement);
			if (gp == null)
			{
                return;
			}

			SetClip(graphics);

			GdiSvgPaint fillPaint = new GdiSvgPaint(styleElm, "fill");
			Brush brush = fillPaint.GetBrush(gp);

			GdiSvgPaint strokePaint = new GdiSvgPaint(styleElm, "stroke");
			Pen pen = strokePaint.GetPen(gp);

			if (brush != null) 
			{
				if (brush is PathGradientBrush) 
				{
					var gps = fillPaint.PaintFill as GdiRadialGradientFill;
						
					graphics.SetClip(gps.GetRadialRegion(gp.GetBounds()), CombineMode.Exclude);

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
					var gps = strokePaint.PaintFill as GdiRadialGradientFill;
					GdiGraphicsContainer container = graphics.BeginContainer();

					graphics.SetClip(gps.GetRadialRegion(gp.GetBounds()), CombineMode.Exclude);

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

			PaintMarkers(renderer, styleElm, graphics);
        }

        #endregion

        #region Private Methods

        private Brush GetBrush(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(_svgElement as SvgStyleableElement, "fill");
            return paint.GetBrush(gp);
        }

        private Pen GetPen(GraphicsPath gp)
        {
            GdiSvgPaint paint = new GdiSvgPaint(_svgElement as SvgStyleableElement, "stroke");
            return paint.GetPen(gp);
        }
		
        #endregion
    }
}
