namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTextPathElement interface corresponds to the 'textPath' element. 
	/// </summary>
	public interface ISvgTextPathElement : ISvgUriReference, ISvgTextContentElement, IElementVisitorTarget
    {
		ISvgAnimatedLength StartOffset{get;}
		ISvgAnimatedEnumeration Method{get;}
		ISvgAnimatedEnumeration Spacing{get;}
	}
}
