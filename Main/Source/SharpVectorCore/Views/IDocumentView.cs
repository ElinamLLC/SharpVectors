using System;

namespace SharpVectors.Dom.Views
{
	/// <summary>
	/// The DocumentView interface is implemented by Document objects in DOM implementations supporting DOM Views. It provides an attribute to retrieve the default view of a document.
	/// </summary>
	public interface IDocumentView
	{
		/// <summary>
		/// The default AbstractView for this Document, or null if none available
		/// </summary>
		IAbstractView DefaultView{get;}
	}
}
