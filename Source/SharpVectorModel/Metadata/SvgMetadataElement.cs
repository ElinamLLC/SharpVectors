using System;

namespace SharpVectors.Dom.Svg
{
	public sealed class SvgMetadataElement : SvgElement, ISvgMetadataElement
	{
        public SvgMetadataElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
		}
	}
}
