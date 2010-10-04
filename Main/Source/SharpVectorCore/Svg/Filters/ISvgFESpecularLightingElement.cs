using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFESpecularLightingElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString In1{get;}
		ISvgAnimatedNumber SurfaceScale{get;}
		ISvgAnimatedNumber SpecularConstant{get;}
		ISvgAnimatedNumber SpecularExponent{get;}
	}
}
