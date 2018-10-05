using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTSpanElement interface corresponds to the 'tspan' element. 
	/// </summary>
    public sealed class SvgTSpanElement : SvgTextPositioningElement, ISvgTSpanElement
	{
		public SvgTSpanElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
		}

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion
    }
}
