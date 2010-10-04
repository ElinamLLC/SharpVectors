// <developer>kevin@kevlindev.com</developer>
// <completed>99</completed>

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgScriptElement interface corresponds to the 'script' element. 
    /// </summary>
    public interface ISvgScriptElement : ISvgElement, ISvgUriReference, 
        ISvgExternalResourcesRequired
    {
        string Type
        {
            get;
            set;
        }
    }
}
