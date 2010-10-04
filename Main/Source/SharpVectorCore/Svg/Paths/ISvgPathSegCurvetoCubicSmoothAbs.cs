// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegCurvetoCubicSmoothAbs interface corresponds to an "absolute smooth cubic curveto" (S) path data command. 
	/// </summary>
	public interface ISvgPathSegCurvetoCubicSmoothAbs : ISvgPathSeg
	{
		double X{get;set;}
		double Y{get;set;}
		double X2{get;set;}
		double Y2{get;set;}
	}
}