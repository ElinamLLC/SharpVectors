// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoVerticalAbs interface corresponds to an "absolute vertical lineto" (V) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoVerticalAbs : ISvgPathSeg
	{
        double Y { get; set; }
	}
}