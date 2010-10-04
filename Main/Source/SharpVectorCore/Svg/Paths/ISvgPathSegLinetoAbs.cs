// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoAbs interface corresponds to an "absolute lineto" (L) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoAbs : ISvgPathSeg
	{
		double X{get;set;}
		double Y{get;set;}
	}
}