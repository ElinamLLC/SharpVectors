using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
    public sealed class SvgPolylineElement : SvgPolyElement, ISvgPolylineElement
	{
		public SvgPolylineElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{		
		}

        public override bool IsClosed
        {
            get {
                return false;
            }
        }

        #region IElementVisitorTarget Members

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion
    }
}
