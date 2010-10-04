using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgStopElement interface corresponds to the 'stop' element. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ISvgStopElement : 
		ISvgElement,
		ISvgStylable 
	{
		ISvgAnimatedNumber Offset{get;}
	}
}
