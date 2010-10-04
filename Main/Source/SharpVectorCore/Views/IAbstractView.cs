// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>
using System;

namespace SharpVectors.Dom.Views
{
	/// <summary>
	/// A base interface that all views shall derive from
	/// </summary>
	public interface IAbstractView
	{
		/// <summary>
		/// The source DocumentView of which this is an AbstractView.
		/// </summary>
		IDocumentView Document{get;}
	}
}
