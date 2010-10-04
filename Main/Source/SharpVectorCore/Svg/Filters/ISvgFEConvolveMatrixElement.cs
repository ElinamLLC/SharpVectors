using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEConvolveMatrixElement:
		ISvgElement,
		ISvgFilterPrimitiveStandardAttributes 
	{
		ISvgAnimatedInteger OrderX{get;}
		ISvgAnimatedInteger OrderY{get;}
		ISvgAnimatedNumberList KernelMatrix{get;}
		ISvgAnimatedNumber Divisor{get;}
		ISvgAnimatedNumber Bias{get;}
		ISvgAnimatedInteger TargetX{get;}
		ISvgAnimatedInteger TargetY{get;}
		ISvgAnimatedEnumeration EdgeMode{get;}
		ISvgAnimatedNumber KernelUnitLengthX{get;}
		ISvgAnimatedNumber KernelUnitLengthY{get;}
		bool PreserveAlpha{get;}     
	}
}
