namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgFontFaceNameElement interface corresponds to the 'font-face-name' element. 
    /// </summary>
    public interface ISvgFontFaceNameElement : ISvgElement
    {
        // attribute name = "name" <string>
        string FaceName
        {
            get;
            set;
        }
    }
}

