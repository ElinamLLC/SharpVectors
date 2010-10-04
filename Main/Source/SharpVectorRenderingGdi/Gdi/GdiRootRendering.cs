using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	/// <summary>
	/// Summary description for SvgElementGraphicsNode.
	/// </summary>
	public sealed class GdiRootRendering : GdiRendering
	{
        #region Constructor and Destructor
		
        public GdiRootRendering(SvgElement element) 
            : base(element)
		{
		}

        #endregion

        #region Public Methods

        public override void Render(GdiGraphicsRenderer renderer)
		{
            GdiGraphicsWrapper graphics = renderer.GraphicsWrapper;

			SvgSvgElement svgElm = (SvgSvgElement) element;

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
