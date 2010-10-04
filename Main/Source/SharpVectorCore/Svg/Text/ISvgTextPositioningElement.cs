// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTextPositioningElement interface is inherited by text-related interfaces: SvgTextElement, SvgTSpanElement, SvgTRefElement and SvgAltGlyphElement. 
	/// </summary>
	public interface ISvgTextPositioningElement	: ISvgTextContentElement
	{                                       
		ISvgAnimatedLengthList X{get;}

		ISvgAnimatedLengthList Y{get;}

		ISvgAnimatedLengthList Dx{get;}

		ISvgAnimatedLengthList Dy{get;}

		ISvgAnimatedNumberList Rotate{get;}
	}
}
