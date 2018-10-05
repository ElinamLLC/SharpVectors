using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	public interface ISvgAElement : ISvgElement, ISvgUriReference, ISvgTests,
		ISvgLangSpace, ISvgExternalResourcesRequired, ISvgStylable, 
        ISvgTransformable, IEventTarget, IElementVisitorTarget
    {
		ISvgAnimatedString Target{get;}
	}
}
