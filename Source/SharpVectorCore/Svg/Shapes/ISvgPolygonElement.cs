using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPolygonElement interface corresponds to the 'polygon' element
	/// </summary>
	public interface ISvgPolygonElement	: ISvgElement, ISvgTests, ISvgLangSpace,
		ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable,
		ISvgAnimatedPoints, IEventTarget, IElementVisitorTarget
    {
	}
}
