using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	/// <summary>
	/// Summary description for <see cref="GdiUseRendering"/>.
	/// </summary>
	public sealed class GdiUseRendering : GdiRendering
	{
        #region Constructor and Destructor
		
        public GdiUseRendering(SvgElement element) 
            : base(element)
		{
		}

        #endregion

        #region Public Methods

        public override void Render(GdiGraphicsRenderer renderer)
		{
			base.Render(renderer);

            var graphics = renderer.GdiGraphics;

			//var svgElm = (SvgUseElement) _svgElement;

			//float x      = (float)svgElm.X.AnimVal.Value;
			//float y      = (float)svgElm.Y.AnimVal.Value;
			//float width  = (float)svgElm.Width.AnimVal.Value;
			//float height = (float)svgElm.Height.AnimVal.Value;

			//RectangleF elmRect = new RectangleF(x, y, width, height);

   //         //if (element.ParentNode is SvgElement)
   //         //{
   //         //    // TODO: should it be moved with x and y?
   //         //}

			//FitToViewbox(graphics, elmRect);
		}

        #endregion
	}
}
