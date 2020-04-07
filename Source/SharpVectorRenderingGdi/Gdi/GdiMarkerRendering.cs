using System;
using System.Drawing;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
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

		public void PaintMarker(GdiGraphicsRenderer renderer, GdiGraphics gr, 
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
		{
			ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;
			SvgMarkerElement markerElm     = (SvgMarkerElement) _svgElement;

			SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
			if (vertexPositions == null)
			{
				return;
			}
			var comparer = StringComparison.OrdinalIgnoreCase;

			bool mayHaveCurves = markerHostElm.MayHaveCurves;
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
			int end = start + len;

			for (int i = start; i < end; i++)
			{
                SvgPointF point = vertexPositions[i];

				GdiGraphicsContainer gc = gr.BeginContainer();

				gr.TranslateTransform(point.X, point.Y);

				if (markerElm.OrientType.AnimVal.Equals((ushort)SvgMarkerOrient.Angle))
				{
					double scaleValue = markerElm.OrientAngle.AnimVal.Value;
					if (!scaleValue.Equals(0))
					{
						gr.RotateTransform((float)scaleValue);
					}
				}
				else
				{
                    double angle;

					switch(markerPos)
					{
						case SvgMarkerPosition.Start:
							angle = markerHostElm.GetStartAngle(i);
							//angle = markerHostElm.GetStartAngle(i + 1);
							if (vertexPositions.Length >= 2)
							{
								SvgPointF pMarkerPoint1 = vertexPositions[start];
								SvgPointF pMarkerPoint2 = vertexPositions[end];
								float xDiff = pMarkerPoint2.X - pMarkerPoint1.X;
								float yDiff = pMarkerPoint2.Y - pMarkerPoint1.Y;
								double angleMarker = (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
								if (!angleMarker.Equals(angle))
								{
									angle = angleMarker;
								}
							}
							break;
						case SvgMarkerPosition.Mid:
							//angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
							angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
							break;
						default:
							angle = markerHostElm.GetEndAngle(i - 1);
							//double angle2 = markerHostElm.GetEndAngle(i);
							if (vertexPositions.Length >= 2)
							{
								SvgPointF pMarkerPoint1 = vertexPositions[start - 1];
								SvgPointF pMarkerPoint2 = vertexPositions[start];
								float xDiff = pMarkerPoint2.X - pMarkerPoint1.X;
								float yDiff = pMarkerPoint2.Y - pMarkerPoint1.Y;
								double angleMarker = (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
								if (!angleMarker.Equals(angle))
								{
									angle = angleMarker;
								}
							}
							//if (mayHaveCurves)
							//{
							//	angle = this.GetAngleAt(start - 1, angle, markerPos, markerHostElm);
							//}
							break;
					}
					gr.RotateTransform((float)angle);
				}

				// 'viewBox' and 'preserveAspectRatio' attributes
				// viewBox -> viewport(0, 0, markerWidth, markerHeight)
				var spar = (SvgPreserveAspectRatio)markerElm.PreserveAspectRatio.AnimVal;
				double[] translateAndScale = spar.FitToViewBox((SvgRect)markerElm.ViewBox.AnimVal,
					new SvgRect(0, 0, markerElm.MarkerWidth.AnimVal.Value, markerElm.MarkerHeight.AnimVal.Value));

				//// Warning at this time, refX and refY are relative to the painted element's coordinate system. 
				//// We need to move the reference point to the marker's coordinate system
				//float refX = (float)markerElm.RefX.AnimVal.Value;
				//float refY = (float)markerElm.RefY.AnimVal.Value;

				////if (!(refX.Equals(0) && refY.Equals(0)))
				////{
				////	var points = new PointF[] { new PointF(refX, refY) };
				////	gr.Transform.TransformPoints(points);

				////	refX = points[0].X;
				////	refY = points[0].Y;

				////	gr.TranslateTransform(-refX, -refY);
				////}

				//if (markerElm.MarkerUnits.AnimVal.Equals((ushort)SvgMarkerUnit.StrokeWidth))
				//{
				//	SvgLength strokeWidthLength = new SvgLength(refElement, 
				//                    "stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
				//	float strokeWidth = (float)strokeWidthLength.Value;
				//	gr.ScaleTransform(strokeWidth, strokeWidth);
				//}

				//gr.TranslateTransform(-(float)(markerElm.RefX.AnimVal.Value * translateAndScale[2]),
				//	-(float)(markerElm.RefY.AnimVal.Value * translateAndScale[3]));

				//gr.ScaleTransform((float)translateAndScale[2], (float)translateAndScale[3]);

				// compute an additional transform for 'strokeWidth' coordinate system
				ISvgAnimatedEnumeration markerUnits = markerElm.MarkerUnits;
				if (markerUnits.AnimVal.Equals((ushort)SvgMarkerUnit.StrokeWidth))
				{
					SvgLength strokeWidthLength = new SvgLength(refElement,
						"stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
					double strokeWidth = strokeWidthLength.Value;
					if (!strokeWidth.Equals(1))
					{
						gr.ScaleTransform((float)strokeWidth, (float)strokeWidth);
					}
				}

				gr.TranslateTransform(-(float)(markerElm.RefX.AnimVal.Value * translateAndScale[2]),
					-(float)(markerElm.RefY.AnimVal.Value * translateAndScale[3]));

				if (!(translateAndScale[2].Equals(1) && translateAndScale[3].Equals(1)))
				{
					gr.ScaleTransform((float)translateAndScale[2], (float)translateAndScale[3]);
				}

				//				gr.TranslateTransform(point.X, point.Y);

				RectangleF rectClip = RectangleF.Empty;

				if (markerUnits.AnimVal.Equals((ushort)SvgMarkerUnit.StrokeWidth))
				{
					string overflowAttr = markerElm.GetAttribute("overflow");
					if (string.IsNullOrWhiteSpace(overflowAttr)
						|| overflowAttr.Equals("scroll", comparer) || overflowAttr.Equals("hidden", comparer))
					{
						var markerClip = RectangleF.Empty;
						SvgRect clipRect = (SvgRect)markerElm.ViewBox.AnimVal;
						if (clipRect != null && !clipRect.IsEmpty)
						{
							rectClip = new RectangleF((float)clipRect.X, (float)clipRect.Y, 
								(float)clipRect.Width, (float)clipRect.Height);
						}
						else if (markerElm.IsSizeDefined)
						{
							rectClip = new RectangleF(0, 0,
								(float)markerElm.MarkerWidth.AnimVal.Value, (float)markerElm.MarkerHeight.AnimVal.Value);
						}
					}
				}

				if (rectClip.IsEmpty)
				{
					SetClip(gr);
				}
				else
				{
					gr.SetClip(rectClip);
				}

				renderer.RenderChildren(markerElm);

				gr.EndContainer(gc);
			}
		}

        #endregion
	}
}
