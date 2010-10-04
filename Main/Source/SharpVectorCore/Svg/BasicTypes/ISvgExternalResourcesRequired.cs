namespace SharpVectors.Dom.Svg
{

	/// <summary>
	/// Interface SvgExternalResourcesRequired defines an interface 
	/// which applies to all elements where this element or one of its 
	/// descendants can reference an external resource. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ISvgExternalResourcesRequired
	{
		/// <summary>
		/// Corresponds to attribute externalResourcesRequired on the 
		/// given element.
		/// </summary>
		ISvgAnimatedBoolean ExternalResourcesRequired{get;}
	}

}
