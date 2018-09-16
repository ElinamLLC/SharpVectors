namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceFormatElement interface corresponds to the 'font-face-format' element. 
    /// </summary>
    public sealed class SvgFontFaceFormatElement : SvgElement, ISvgFontFaceFormatElement
    {
        #region Constructors and Destructor

        public SvgFontFaceFormatElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFontFaceFormatElement Properties

        // attribute name = "string" <string>
        public string String
        {
            get { return this.GetAttribute("string"); }
            set { this.SetAttribute("string", value); }
        }

        #endregion
    }
}

