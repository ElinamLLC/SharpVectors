// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoHorizontalRel interface corresponds to a "relative horizontal lineto" (h) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoHorizontalRel : ISvgPathSeg
	{
		double X{get;set;}
	}
}