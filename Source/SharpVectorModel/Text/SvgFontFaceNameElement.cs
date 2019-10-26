using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceNameElement interface corresponds to the 'font-face-name' element. 
    /// </summary>
    public sealed class SvgFontFaceNameElement : SvgElement, ISvgFontFaceNameElement
    {
        #region Constructors and Destructor

        public SvgFontFaceNameElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFontFaceNameElement Properties

        // attribute name = "name" <string>
        public string FaceName
        {
            get { return this.GetAttribute("name"); }
            set { this.SetAttribute("name", value); }
        }

        #endregion
    }
}
