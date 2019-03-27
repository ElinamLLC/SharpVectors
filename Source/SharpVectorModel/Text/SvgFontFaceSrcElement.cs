using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceSrcElement interface corresponds to the 'font-face-src' element. 
    /// </summary>
    public sealed class SvgFontFaceSrcElement : SvgElement, ISvgFontFaceSrcElement
    {
        #region Constructors and Destructor

        public SvgFontFaceSrcElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion
    }
}

