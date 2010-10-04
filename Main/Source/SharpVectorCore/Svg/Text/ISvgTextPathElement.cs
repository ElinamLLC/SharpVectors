// <developer></developer>
// <completed>0</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTextPathElement interface corresponds to the 'textPath' element. 
	/// </summary>
	public interface ISvgTextPathElement : ISvgUriReference, ISvgTextContentElement
	{
		ISvgAnimatedLength StartOffset{get;}
		ISvgAnimatedEnumeration Method{get;}
		ISvgAnimatedEnumeration Spacing{get;}
	}
}
