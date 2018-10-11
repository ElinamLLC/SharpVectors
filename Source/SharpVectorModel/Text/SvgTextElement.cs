using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTextElement.
	/// </summary>
	public sealed class SvgTextElement : SvgTextPositioningElement, ISvgTextElement
	{
        public SvgTextElement(string prefix, string localname, string ns, SvgDocument doc) 
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
