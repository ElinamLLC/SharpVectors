using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
	public enum SvgMarkerPosition{Start, Mid, End}

    public sealed class GdiMarkerRendering : GdiRendering
	{
        #region Constructors and Destructor

		public GdiMarkerRendering(SvgElement element) : base(element)
		{
		}

        #endregion

        #region Public Methods

        // disable default rendering
        public override void BeforeRender(GdiGraphicsRenderer renderer)
		{
		}
        public override void Render(GdiGraphicsRenderer renderer)
        {
        }
        public override void AfterRender(GdiGraphicsRenderer renderer)
        {
        }

		public void PaintMarker(GdiGraphicsRenderer renderer, GdiGraphicsWrapper gr, 
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
		{
			ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;
			SvgMarkerElement markerElm     = (SvgMarkerElement) element;

            SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
			int start;
			int len;

			// Choose which part of the position array to use
			switch (markerPos)
			{
				case SvgMarkerPosition.Start:
					start = 0;
					len = 1;
					break;
				case SvgMarkerPosition.Mid:
					start = 1;
					len = vertexPositions.Length - 2;
					break;
				default:
					// == MarkerPosition.End
					start = vertexPositions.Length-1;
					len = 1;
					break;
			}

			for (int i = start; i < start+len; i++)
			{
                SvgPointF point = vertexPositions[i];

				GdiGraphicsContainer gc = gr.BeginContainer();

				gr.TranslateTransform(point.X, point.Y);

				if (markerElm.OrientType.AnimVal.Equals(SvgMarkerOrient.Angle))
				{
					gr.RotateTransform((float)markerElm.OrientAngle.AnimVal.Value);
				}
				else
				{
                    double angle;

					switch(markerPos)
					{
						case SvgMarkerPosition.Start:
							angle = markerHostElm.GetStartAngle(i + 1);
							break;
						case SvgMarkerPosition.Mid:
							//angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
							angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
							break;
						default:
							angle = markerHostElm.GetEndAngle(i);
							break;
					}
					gr.RotateTransform((float)angle);
				}

				if (markerElm.MarkerUnits.AnimVal.Equals(SvgMarkerUnit.StrokeWidth))
				{
					SvgLength strokeWidthLength = new SvgLength(refElement, 
                        "stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
					float strokeWidth = (float)strokeWidthLength.Value;
					gr.ScaleTransform(strokeWidth, strokeWidth);
				}

				SvgPreserveAspectRatio spar = 
                    (SvgPreserveAspectRatio)markerElm.PreserveAspectRatio.AnimVal;
                double[] translateAndScale = spar.FitToViewBox((SvgRect)markerElm.ViewBox.AnimVal,
					new SvgRect(0, 0, markerElm.MarkerWidth.AnimVal.Value,
						markerElm.MarkerHeight.AnimVal.Value));


				gr.TranslateTransform(-(float)(markerElm.RefX.AnimVal.Value * translateAndScale[2]),
					-(float)(markerElm.RefY.AnimVal.Value * translateAndScale[3]));

				gr.ScaleTransform((float)translateAndScale[2], (float)translateAndScale[3]);

				Clip(gr);
                             
                renderer.RenderChildren(markerElm);

				gr.EndContainer(gc);
			}
		}

        #endregion
	}
}
