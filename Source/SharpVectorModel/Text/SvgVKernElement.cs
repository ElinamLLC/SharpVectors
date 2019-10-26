using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgVKernElement interface corresponds to the 'vkern' element. 
    /// </summary>
    public sealed class SvgVKernElement : SvgKernElement, ISvgVKernElement
    {
        #region Constructors and Destructor

        public SvgVKernElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsHorizontal
        {
            get {
                return false;
            }
        }

        #endregion
    }
}

