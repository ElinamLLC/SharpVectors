using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgLineElement interface corresponds to the 'line' element. 
	/// </summary>
	public interface ISvgLineElement : ISvgElement, ISvgTests, ISvgLangSpace,
				ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable, IEventTarget, IElementVisitorTarget
    {
		ISvgAnimatedLength X1{get;}

		ISvgAnimatedLength Y1{get;}

		ISvgAnimatedLength X2{get;}
		
		ISvgAnimatedLength Y2{get;}
	}
}
