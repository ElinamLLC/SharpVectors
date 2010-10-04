// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgUseElement interface corresponds to the 'use' element. 
	/// </summary>
	public interface ISvgUseElement : ISvgElement, ISvgUriReference, ISvgTests, ISvgStylable,
		ISvgLangSpace, ISvgExternalResourcesRequired, ISvgTransformable, IEventTarget
	{                                     
		ISvgAnimatedLength X { get; }
		ISvgAnimatedLength Y { get; }
		ISvgAnimatedLength Width { get; }
		ISvgAnimatedLength Height { get; }
		ISvgElementInstance InstanceRoot { get; }
		ISvgElementInstance AnimatedInstanceRoot { get; }
	}
}
