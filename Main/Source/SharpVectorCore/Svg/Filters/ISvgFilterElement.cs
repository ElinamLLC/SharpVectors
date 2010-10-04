using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFilterElement :
		ISvgElement,
		ISvgUriReference,
		ISvgLangSpace,
		ISvgExternalResourcesRequired,
		ISvgStylable
	{
		ISvgAnimatedEnumeration FilterUnits{get;}
		ISvgAnimatedEnumeration PrimitiveUnits{get;}
		ISvgAnimatedLength X{get;}
		ISvgAnimatedLength Y {get;}
		ISvgAnimatedLength Width {get;}
		ISvgAnimatedLength Height {get;}
		ISvgAnimatedInteger FilterResX{get;}
		ISvgAnimatedInteger FilterResY{get;}
		void SetFilterRes(ulong filterResX,ulong filterResY);  
	}
}
