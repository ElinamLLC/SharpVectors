using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	/// <summary>
	/// Summary description for <see cref="GdiSvgRendering"/>.
	/// </summary>
	public sealed class GdiSvgRendering : GdiRendering
	{
        #region Constructor and Destructor
		
        public GdiSvgRendering(SvgElement element) 
            : base(element)
		{
		}

        #endregion

        #region Public Methods

        public override void Render(GdiGraphicsRenderer renderer)
		{
            var graphics = renderer.GdiGraphics;

			var svgElm = (SvgSvgElement) _svgElement;

			float x      = (float)svgElm.X.AnimVal.Value;
			float y      = (float)svgElm.Y.AnimVal.Value;
			float width  = (float)svgElm.Width.AnimVal.Value;
			float height = (float)svgElm.Height.AnimVal.Value;

			RectangleF elmRect = new RectangleF(x, y, width, height);

            //if (element.ParentNode is SvgElement)
            //{
            //    // TODO: should it be moved with x and y?
            //}

			FitToViewbox(graphics, elmRect);
		}

        #endregion
	}
}
