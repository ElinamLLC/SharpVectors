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
                return false;
            }
        }

        #endregion
    }
}

