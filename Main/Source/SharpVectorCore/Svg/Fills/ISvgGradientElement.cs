using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgGradientElement interface is a base interface used by SvgLinearGradientElement and SvgRadialGradientElement. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>20</completed>
	public interface ISvgGradientElement : 
		ISvgElement,
		ISvgUriReference,
		ISvgExternalResourcesRequired,
		ISvgStylable 
	{
		ISvgAnimatedEnumeration   GradientUnits{get;}
		ISvgAnimatedTransformList GradientTransform{get;}
		ISvgAnimatedEnumeration   SpreadMethod{get;}
	
	}
}
