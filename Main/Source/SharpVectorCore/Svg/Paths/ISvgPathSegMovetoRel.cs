// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegMovetoRel interface corresponds to an "relative moveto" (m) path data command. 
	/// </summary>
	public interface ISvgPathSegMovetoRel : ISvgPathSeg
	{
        double X { get; set; }
        double Y { get; set; }
	}
}