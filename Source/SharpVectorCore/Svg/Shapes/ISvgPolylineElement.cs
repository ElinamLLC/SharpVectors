using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPolylineElement interface corresponds to the 'polyline' element
	/// </summary>
	public interface ISvgPolylineElement : ISvgElement, ISvgTests, ISvgLangSpace, ISvgExternalResourcesRequired, 
        ISvgStylable, ISvgTransformable, ISvgAnimatedPoints, IEventTarget, IElementVisitorTarget
    {

	}
}
