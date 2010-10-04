// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using System.Xml;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The StyleSheet interface is the abstract base interface for 
	/// any type of style sheet. It represents a single style sheet 
	/// associated with a structured document. In HTML, the 
	/// StyleSheet interface represents either an external style 
	/// sheet, included via the HTML LINK element, or an inline 
	/// STYLE element. In XML, this interface represents an external
	/// style sheet, included via a style sheet processing 
	/// instruction. 
	/// </summary>
	public interface IStyleSheet
	{
		/// <summary>
		/// The intended destination media for style information. The media is often specified in the ownerNode. If no media has been specified, the MediaList will be empty. See the media attribute definition for the LINK element in HTML 4.0, and the media pseudo-attribute for the XML style sheet processing instruction . Modifying the media list may cause a change to the attribute disabled.
		/// </summary>
		IMediaList Media
		{
			get;
		}
	
		/// <summary>
		/// The advisory title. The title is often specified in the ownerNode. See the title attribute definition for the LINK element in HTML 4.0, and the title pseudo-attribute for the XML style sheet processing instruction.
		/// </summary>
		string Title
		{
			get;
		}
	
		/// <summary>
		/// If the style sheet is a linked style sheet, the value of its attribute is its location. For inline style sheets, the value of this attribute is null. See the href attribute definition for the LINK element in HTML 4.0, and the href pseudo-attribute for the XML style sheet processing
		/// </summary>
		string Href
		{
			get;
		}
	
		/// <summary>
		/// For style sheet languages that support the concept of style sheet inclusion, this attribute represents the including style sheet, if one exists. If the style sheet is a top-level style sheet, or the style sheet language does not support inclusion, the value of this attribute is null.
		/// </summary>
		IStyleSheet ParentStyleSheet
		{
			get;
		}
	
		/// <summary>
		/// The node that associates this style sheet with the document. For HTML, this may be the corresponding LINK or STYLE element. For XML, it may be the linking processing instruction. For style sheets that are included by other style sheets, the value of this attribute is null
		/// </summary>
		XmlNode OwnerNode
		{
			get;
		}
	
		/// <summary>
		/// false if the style sheet is applied to the document. true if it is not. Modifying this attribute may cause a new resolution of style for the document. A stylesheet only applies if both an appropriate medium definition is present and the disabled attribute is false. So, if the media doesn't apply to the current user agent, the disabled attribute is ignored.
		/// </summary>
		bool Disabled
		{
			get;
			set;
		}
	
		/// <summary>
		/// This specifies the style sheet language for this style sheet. The style sheet language is specified as a content type (e.g. "text/css"). The content type is often specified in the ownerNode. Also see the type attribute definition for the LINK element in HTML 4.0, and the type pseudo-attribute for the XML style sheet processing instruction.
		/// </summary>
		string Type
		{
			get;
		}
	}
}

