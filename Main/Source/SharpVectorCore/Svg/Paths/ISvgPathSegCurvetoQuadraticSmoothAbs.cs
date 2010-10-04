// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegCurvetoQuadraticSmoothAbs interface corresponds to an "absolute smooth quadratic curveto" (T) path data command.
	/// </summary>
	public interface ISvgPathSegCurvetoQuadraticSmoothAbs : ISvgPathSeg
	{
		double X{get;set;}
		double Y{get;set;}
	}
}