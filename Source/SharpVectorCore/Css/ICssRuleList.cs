using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSRuleList interface provides the abstraction of an 
	/// ordered collection of CSS rules. 
	/// </summary>
	public interface ICssRuleList : IList<ICssRule>
	{
		/// <summary>
		/// The number of CSSRules in the list. The range of valid child rule indices is 0 to length-1 inclusive.
		/// </summary>
		ulong Length
		{
			get;
		}

		/// <summary>
		/// Used to retrieve a CSS rule by ordinal index. The order in this collection represents the 
        /// order of the rules in the CSS style sheet. If index is greater than or equal to the number 
        /// of rules in the list, this returns null.
		/// </summary>
		ICssRule this[ulong Item]
		{
			get;
		}

        bool HasFontRule { get; }
	}
}
