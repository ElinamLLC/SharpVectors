using System;

namespace SharpVectors.Dom.Svg
{
	public interface ISvgPatternElement : 
		ISvgElement,
		ISvgUriReference,
		ISvgTests,
		ISvgLangSpace,
		ISvgExternalResourcesRequired,
		ISvgStylable,
		ISvgFitToViewBox/*,
		ISvgUnitTypes */
	{
		ISvgAnimatedEnumeration   PatternUnits{get;}
		ISvgAnimatedEnumeration   PatternContentUnits{get;}
		ISvgAnimatedTransformList PatternTransform{get;}
		ISvgAnimatedLength        X{get;}
		ISvgAnimatedLength        Y{get;}
		ISvgAnimatedLength        Width{get;}
		ISvgAnimatedLength        Height{get;}
	}
}
