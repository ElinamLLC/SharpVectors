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

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get {
                return false;
            }
        }

        public override bool IsHorizontal
        {
            get {
                return true;
            }
        }

        #endregion
    }
}

