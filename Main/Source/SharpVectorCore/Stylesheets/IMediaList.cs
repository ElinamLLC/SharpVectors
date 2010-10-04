// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The MediaList interface provides the abstraction of an 
	/// ordered collection of media, without defining or constraining
	/// how this collection is implemented. An empty list is the same
	/// as a list that contains the medium "all". 
	/// </summary>
	public interface IMediaList
	{
		/// <summary>
		/// Adds the medium newMedium to the end of the list. If the newMedium is already used, it is first removed.
		/// </summary>
		/// <param name="newMedium">The new medium to add.</param>
		/// <exception cref="DomException">INVALID_CHARACTER_ERR: If the medium contains characters that are invalid in the underlying style language.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this list is readonly</exception>
		void AppendMedium(string newMedium);
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldMedium"></param>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this list is readonly.</exception>
		/// <exception cref="DomException">NOT_FOUND_ERR: Raised if oldMedium is not in the list.</exception>
		void DeleteMedium(string oldMedium);
	
		/// <summary>
		/// The number of media in the list. The range of valid media is 0 to length-1 inclusive.
		/// </summary>
		ulong Length {get;}
	
		/// <summary>
		/// The parsable textual representation of the media list. This is a comma-separated list of media.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this media list is readonly.</exception>
		string MediaText {get; set;}

		/// <summary>
		/// Returns the indexth in the list. If index is greater than or equal to the number of media in the list, this returns null.
		/// </summary>
		string this[ulong index] {get;}
	}
}
