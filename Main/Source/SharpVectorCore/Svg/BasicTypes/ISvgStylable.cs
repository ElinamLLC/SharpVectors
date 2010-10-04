// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISvgStylable  
	{
		ISvgAnimatedString ClassName{get;}
		ICssStyleDeclaration Style{get;}
		ICssValue GetPresentationAttribute(string name);
	}
}
