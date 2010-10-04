// <developer>niklas@protocol7.com</developer>
// <completed>90</completed>

using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
    public sealed class SvgPolylineElement : SvgPolyElement, ISvgPolylineElement
	{
		#region Constructors and Destructor
		
		public SvgPolylineElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
		
		}

		#endregion
	}
}
