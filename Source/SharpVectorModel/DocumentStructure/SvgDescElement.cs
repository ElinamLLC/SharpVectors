using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgDescElement interface corresponds to the 'desc' element. 
    /// </summary>
    public sealed class SvgDescElement : SvgStyleableElement, ISvgDescElement
    {
        public SvgDescElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
        {
        }

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
    }
}
