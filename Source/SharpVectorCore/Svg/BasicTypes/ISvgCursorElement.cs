using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	public interface ISvgCursorElement : ISvgElement, ISvgUriReference, ISvgTests, ISvgExternalResourcesRequired 
	{
		ISvgAnimatedLength X{get;}
		ISvgAnimatedLength Y{get;}
	}
}
