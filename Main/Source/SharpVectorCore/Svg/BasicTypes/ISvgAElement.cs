// <developer>don@donxml.com</developer>
// <completed>100</completed>

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	public interface ISvgAElement : ISvgElement, ISvgUriReference, ISvgTests,
		ISvgLangSpace, ISvgExternalResourcesRequired, ISvgStylable, 
        ISvgTransformable, IEventTarget
	{
		ISvgAnimatedString Target{get;}
	}
}
