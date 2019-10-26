using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTextElement.
	/// </summary>
	public abstract class SvgTextBaseElement : SvgTextPositioningElement, ISvgTextElement
	{
        protected SvgTextBaseElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
		}

        public override ISvgAnimatedLength LetterSpacing
        {
            get {
                return new SvgAnimatedLength(this, "letter-spacing", SvgLengthDirection.Horizontal, "0");
            }
        }

        public override ISvgAnimatedLength TextLength
        {
            get {
                return new SvgAnimatedLength(this, "textLength", SvgLengthDirection.Horizontal, "0");
            }
        }

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion
    }
}
