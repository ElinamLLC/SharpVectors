// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoVerticalRel interface corresponds to a "relative vertical lineto" (v) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoVerticalRel : ISvgPathSeg
	{
        double Y { get; set; }
	}
}