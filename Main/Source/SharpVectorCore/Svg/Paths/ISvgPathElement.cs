// <developer>niklas@protocol7.com</developer>
// <completed>10</completed>

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathElement interface corresponds to the 'path' element. 
	/// </summary>
	public interface ISvgPathElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable,	ISvgAnimatedPathData, IEventTarget
  {
		ISvgAnimatedNumber PathLength{get;}
		 double GetTotalLength();
		 ISvgPoint GetPointAtLength(double distance);
		 int GetPathSegAtLength(double distance);

		ISvgPathSegClosePath CreateSvgPathSegClosePath();
		 ISvgPathSegMovetoAbs CreateSvgPathSegMovetoAbs(double x, double y);
		 ISvgPathSegMovetoRel CreateSvgPathSegMovetoRel(double x, double y);
		 ISvgPathSegLinetoAbs CreateSvgPathSegLinetoAbs(double x, double y);
		 ISvgPathSegLinetoRel CreateSvgPathSegLinetoRel(double x, double y);
		 ISvgPathSegCurvetoCubicAbs CreateSvgPathSegCurvetoCubicAbs(double x, 
											double y, 
											double x1, 
			 								double y1, 
			 								double x2, 
			 								double y2);
		 ISvgPathSegCurvetoCubicRel CreateSvgPathSegCurvetoCubicRel(double x, 
			 								double y, 
			 								double x1, 
			 								double y1, 
			 								double x2, 
			 								double y2);
		 ISvgPathSegCurvetoQuadraticAbs CreateSvgPathSegCurvetoQuadraticAbs(double x, 
			 								double y, 
			 								double x1, 
			 								double y1);
		 ISvgPathSegCurvetoQuadraticRel CreateSvgPathSegCurvetoQuadraticRel(double x, 
			 								double y, 
			 								double x1, 
			 								double y1);

		 ISvgPathSegArcAbs CreateSvgPathSegArcAbs(double x,
										   double y,
										   double r1,
										   double r2,
										   double angle,
										   bool largeArcFlag,
										   bool sweepFlag);

		 ISvgPathSegArcRel CreateSvgPathSegArcRel(double x,
										   double y,
										   double r1,
										   double r2,
										   double angle,
										   bool largeArcFlag,
										   bool sweepFlag);

		 ISvgPathSegLinetoHorizontalAbs CreateSvgPathSegLinetoHorizontalAbs(double x);

		 ISvgPathSegLinetoHorizontalRel CreateSvgPathSegLinetoHorizontalRel(double x);

		 ISvgPathSegLinetoVerticalAbs CreateSvgPathSegLinetoVerticalAbs(double y);

		 ISvgPathSegLinetoVerticalRel CreateSvgPathSegLinetoVerticalRel(double y);

		 ISvgPathSegCurvetoCubicSmoothAbs CreateSvgPathSegCurvetoCubicSmoothAbs(double x,
																		 double y,
																		 double x2,
																		 double y2);

		 ISvgPathSegCurvetoCubicSmoothRel CreateSvgPathSegCurvetoCubicSmoothRel(double x,
																		 double y,
																		 double x2,
																		 double y2);

		 ISvgPathSegCurvetoQuadraticSmoothAbs CreateSvgPathSegCurvetoQuadraticSmoothAbs(double x,
																				 double y);

		 ISvgPathSegCurvetoQuadraticSmoothRel CreateSvgPathSegCurvetoQuadraticSmoothRel(double x,
																				 double y);
	}
}
