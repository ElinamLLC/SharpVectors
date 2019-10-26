using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceUriElement interface corresponds to the 'font-face-uri' element. 
    /// </summary>
    public sealed class SvgFontFaceUriElement : SvgElement, ISvgFontFaceUriElement
    {
        #region Constructors and Destructor

        public SvgFontFaceUriElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion
    }
}

