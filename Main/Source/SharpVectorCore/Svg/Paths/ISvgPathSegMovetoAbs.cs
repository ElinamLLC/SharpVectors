// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegMovetoAbs interface corresponds to an "absolute moveto" (M) path data command. 
	/// </summary>
	public interface ISvgPathSegMovetoAbs : ISvgPathSeg
	{
        double X { get; set; }
        double Y { get; set; }
	}
}
