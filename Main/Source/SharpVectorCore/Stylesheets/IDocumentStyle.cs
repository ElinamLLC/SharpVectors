// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The DocumentStyle interface provides a mechanism by which 
	/// the style sheets embedded in a document can be retrieved. 
	/// The expectation is that an instance of the DocumentStyle 
	/// interface can be obtained by using binding-specific casting 
	/// methods on an instance of the Document interface. 
	/// </summary>
	public interface IDocumentStyle
	{
		/// <summary>
		/// A list containing all the style sheets explicitly linked into or embedded in a document. For HTML documents, this includes external style sheets, included via the HTML LINK element, and inline STYLE elements. In XML, this includes external style sheets, included via style sheet processing instructions (see [XML-StyleSheet]).
		/// </summary>
		IStyleSheetList StyleSheets { get; }
	}
}
