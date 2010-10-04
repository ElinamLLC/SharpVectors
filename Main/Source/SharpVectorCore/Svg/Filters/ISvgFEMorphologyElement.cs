using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEMorphologyElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString In1{get;}
		ISvgAnimatedEnumeration Operator{get;}
		ISvgAnimatedNumber RadiusX{get;}
		ISvgAnimatedNumber RadiusY{get;}
	}
}
