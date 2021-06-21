namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// This defines an interface which applies to all elements where this element or one of its 
	/// descendants can reference an external resource. 
	/// </summary>
	public interface ISvgExternalResourcesRequired
	{
		/// <summary>
		/// Corresponds to attribute externalResourcesRequired on the 
		/// given element.
		/// </summary>
		ISvgAnimatedBoolean ExternalResourcesRequired { get; }
	}
}
