using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEDisplacementMapElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString In1{get;}
		ISvgAnimatedString In2{get;}
		ISvgAnimatedNumber Scale{get;}
		ISvgAnimatedEnumeration XChannelSelector{get;}
		ISvgAnimatedEnumeration YChannelSelector{get;}
	}
}
