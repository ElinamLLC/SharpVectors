// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>
 
using System;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The StyleSheetList interface provides the abstraction of an
	/// ordered collection of style sheets. 
	/// </summary>
	public interface IStyleSheetList
	{
		/// <summary>
		/// The number of StyleSheets in the list. The range of valid child stylesheet indices is 0 to length-1 inclusive.
		/// </summary>
		ulong Length { get; }

		/// <summary>
		/// Used to retrieve a style sheet by ordinal index. If index is greater than or equal to the number of style sheets in the list, this returns null.
		/// </summary>
		IStyleSheet this[ulong index] { get; }
	}
}
