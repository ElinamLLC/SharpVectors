// <developer>tabascopete78@yahoo.com</developer>
// <completed>50</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used by SvgClipPathElement.
	/// </summary>
	public interface ISvgClipPathElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable
	{
		ISvgAnimatedEnumeration ClipPathUnits {get;}
	}
}
