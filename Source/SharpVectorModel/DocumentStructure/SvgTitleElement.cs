using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTitleElement interface corresponds to the 'title' element. 
	/// </summary>
    public sealed class SvgTitleElement : SvgStyleableElement, ISvgTitleElement
	{
		public SvgTitleElement(string prefix, string localname, string ns, SvgDocument doc) 
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
