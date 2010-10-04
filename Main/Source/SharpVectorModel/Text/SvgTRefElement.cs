using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTRefElement.
	/// </summary>
    public sealed class SvgTRefElement : SvgTextPositioningElement, ISvgTRefElement
	{
        private SvgUriReference svgURIReference;

        public SvgTRefElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc) 
		{
			svgURIReference = new SvgUriReference(this);
		}

		#region ISvgURIReference Members

		public ISvgAnimatedString Href
		{
			get
			{
				return svgURIReference.Href;
			}
		}

		public XmlElement ReferencedElement
		{
			get
			{
				return svgURIReference.ReferencedNode as XmlElement;
			}
		}

		#endregion
	}
}
