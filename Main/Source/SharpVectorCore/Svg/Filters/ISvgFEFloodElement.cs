using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEFloodElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString      In1{get;}
	}
}
