using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgComponentTransferFunctionElement:
		ISvgElement
	{
		ISvgAnimatedEnumeration Type{get;}
		ISvgAnimatedNumberList TableValues{get;}
		ISvgAnimatedNumber Slope{get;}
		ISvgAnimatedNumber Intercept{get;}
		ISvgAnimatedNumber Amplitude{get;}
		ISvgAnimatedNumber Exponent{get;}
		ISvgAnimatedNumber Offset{get;}      
	}
}
