// <developer>niklas@protocol7.com</developer>
// <completed>90</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The root object in the document object hierarchy of an Svg document.
	/// </summary>
	/// <remarks>
	/// <p>
	/// When an 'svg'  element is embedded inline as a component of a
	/// document from another namespace, such as when an 'svg' element is
	/// embedded inline within an XHTML document
	/// [<a href="http://www.w3.org/TR/SVG/refs.html#ref-XHTML">XHTML</a>],
	/// then an
	/// <see cref="ISvgDocument">ISvgDocument</see> object will not exist;
	/// instead, the root object in the
	/// document object hierarchy will be a Document object of a different
	/// type, such as an HTMLDocument object.
	/// </p>
	/// <p>
	/// However, an <see cref="ISvgDocument">ISvgDocument</see> object will
	/// indeed exist when the root
	/// element of the XML document hierarchy is an 'svg' element, such as
	/// when viewing a stand-alone SVG file (i.e., a file with MIME type
	/// "image/svg+xml"). In this case, the
	/// <see cref="ISvgDocument">ISvgDocument</see> object will be the
	/// root object of the document object model hierarchy.
	/// </p>
	/// <p>
	/// In the case where an SVG document is embedded by reference, such as
	/// when an XHTML document has an 'object' element whose href attribute
	/// references an SVG document (i.e., a document whose MIME type is
	/// "image/svg+xml" and whose root element is thus an 'svg' element),
	/// there will exist two distinct DOM hierarchies. The first DOM hierarchy
	/// will be for the referencing document (e.g., an XHTML document). The
	/// second DOM hierarchy will be for the referenced SVG document. In this
	/// second DOM hierarchy, the root object of the document object model
	/// hierarchy is an <see cref="ISvgDocument">ISvgDocument</see> object.
	/// </p>
	/// <p>
	/// The <see cref="ISvgDocument">ISvgDocument</see> interface contains a
	/// similar list of attributes and
	/// methods to the HTMLDocument interface described in the
	/// <a href="http://www.w3.org/TR/REC-DOM-Level-1/level-one-html.html">Document
	/// Object Model (HTML) Level 1</a> chapter of the
	/// [<a href="http://www.w3.org/TR/SVG/refs.html#ref-DOM1">DOM1</a>] specification.
	/// </p>
	/// </remarks>
	public interface ISvgDocument : IDocument
	{
		/// <summary>
		/// The title of a document as specified by the title sub-element of
		/// the 'svg' root element.
		/// </summary>
		string Title
		{
			get;
		}
		
		/// <summary>
		/// Returns the URI of the page that linked to this page. The value
		/// is an empty string if the user navigated to the page directly
		/// (not through a link, but, for example, via a bookmark).
		/// </summary>
		string Referrer
		{
			get;
		}
		
		/// <summary>
		/// The domain name of the server that served the document, or a
		/// null string if the server cannot be identified by a domain name.
		/// </summary>
		string Domain
		{
			get;
		}
		
		/// <summary>
		/// The complete URI of the document.
		/// </summary>
		string Url
		{
			get;
		}
		
		/// <summary>
		/// The root 'svg' element in the document hierarchy.
		/// </summary>
		ISvgSvgElement RootElement
		{
			get;
		}
		
		/// <summary>
		/// The window object of the Svg document.
		/// </summary>
		ISvgWindow Window
		{
			get;
		}
	}
}
