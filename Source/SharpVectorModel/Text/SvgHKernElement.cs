using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgHKernElement interface corresponds to the 'hkern' element. 
    /// </summary>
    public sealed class SvgHKernElement : SvgKernElement, ISvgHKernElement
    {
        #region Constructors and Destructor

        public SvgHKernElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsHorizontal
        {
            get {
                return true;
            }
        }

        #endregion
    }
}

