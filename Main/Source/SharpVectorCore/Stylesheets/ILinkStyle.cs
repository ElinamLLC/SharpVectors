// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>
 
using System;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The LinkStyle interface provides a mechanism by which a 
	/// style sheet can be retrieved from the node responsible for 
	/// linking it into a document. An instance of the LinkStyle 
	/// interface can be obtained using binding-specific casting 
	/// methods on an instance of a linking node (HTMLLinkElement, 
	/// HTMLStyleElement or ProcessingInstruction in DOM Level 2). 
	/// </summary>
	public interface ILinkStyle
	{
		/// <summary>
		/// The style sheet
		/// </summary>
		IStyleSheet Sheet {get;}
	}
}
