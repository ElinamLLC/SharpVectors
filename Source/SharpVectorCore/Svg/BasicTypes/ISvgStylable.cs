using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISvgStylable
    {
        ISvgAnimatedString ClassName { get; }
        ICssStyleDeclaration Style { get; }
        ICssValue GetPresentationAttribute(string name);
    }
}
