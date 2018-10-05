using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgUseElement interface corresponds to the 'use' element. 
	/// </summary>
	public interface ISvgUseElement : ISvgElement, ISvgUriReference, ISvgTests, ISvgStylable,
		ISvgLangSpace, ISvgExternalResourcesRequired, ISvgTransformable, IEventTarget, IElementVisitorTarget
    {                                     
		ISvgAnimatedLength X { get; }
		ISvgAnimatedLength Y { get; }
		ISvgAnimatedLength Width { get; }
		ISvgAnimatedLength Height { get; }
		ISvgElementInstance InstanceRoot { get; }
		ISvgElementInstance AnimatedInstanceRoot { get; }
	}
}
