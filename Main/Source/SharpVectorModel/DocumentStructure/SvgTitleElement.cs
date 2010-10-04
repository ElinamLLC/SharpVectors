using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgTitleElement interface corresponds to the 'title' element. 
	/// </summary>
    public sealed class SvgTitleElement : SvgStyleableElement, ISvgTitleElement
	{
		#region Constructors

		public SvgTitleElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
		}

		#endregion
	}
}
