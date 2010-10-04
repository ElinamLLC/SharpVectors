using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgStopElement.
	/// </summary>
    public sealed class SvgStopElement : SvgStyleableElement, ISvgStopElement
	{
		public SvgStopElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
		}

		public ISvgAnimatedNumber Offset
		{
			get
			{
				string attr = GetAttribute("offset").Trim();
				if (attr.EndsWith("%"))
				{
					attr = attr.TrimEnd(new char[1]{'%'});
				}
				else
				{
					double tmp = SvgNumber.ParseNumber(attr) * 100;
					attr = tmp.ToString(SvgNumber.Format);
				}

				return new SvgAnimatedNumber(attr);
			}
		}
	}
}
