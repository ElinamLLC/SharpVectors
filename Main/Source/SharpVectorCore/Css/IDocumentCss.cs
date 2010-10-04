// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>	

using System;
using System.Xml;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// This interface represents a document with a CSS view.
	/// </summary>
	public interface IDocumentCss : Stylesheets.IDocumentStyle
	{
		/// <summary>
		/// This method is used to retrieve the override style declaration for a specified element and a specified pseudo-element.
		/// </summary>
		ICssStyleDeclaration GetOverrideStyle(XmlElement elt, string pseudoElt);
	}
}
