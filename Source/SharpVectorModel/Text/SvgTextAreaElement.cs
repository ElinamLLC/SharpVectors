using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTextAreaElement.
	/// </summary>
	public sealed class SvgTextAreaElement : SvgTextBaseElement, ISvgTextElement
	{
        public SvgTextAreaElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
		}
    }
}
