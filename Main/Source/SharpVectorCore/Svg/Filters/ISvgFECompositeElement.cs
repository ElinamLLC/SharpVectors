using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFECompositeElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString In1{get;}
		ISvgAnimatedString In2{get;}
		ISvgAnimatedEnumeration Operator{get;}
		ISvgAnimatedNumber K1{get;}      
		ISvgAnimatedNumber K2{get;}      
		ISvgAnimatedNumber K3{get;}      
		ISvgAnimatedNumber K4{get;}      
	}
}
