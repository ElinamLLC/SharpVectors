namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgGradientElement interface is a base interface used by SvgLinearGradientElement 
    /// and SvgRadialGradientElement. 
	/// </summary>
	public interface ISvgGradientElement : ISvgElement, ISvgUriReference, ISvgExternalResourcesRequired, ISvgStylable 
	{
		ISvgAnimatedEnumeration   GradientUnits{get;}
		ISvgAnimatedTransformList GradientTransform{get;}
		ISvgAnimatedEnumeration   SpreadMethod{get;}
	
	}
}
