using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEDiffuseLightingElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedString In1{get;}
		ISvgAnimatedNumber SurfaceScale{get;}
 		ISvgAnimatedNumber DiffuseConstant{get;}
		ISvgAnimatedNumber KernelUnitLengthX{get;}
		ISvgAnimatedNumber KernelUnitLengthY{get;}
	}
}
