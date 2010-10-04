// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

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
    }
}
