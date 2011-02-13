// <developer>niklas@protocol7.com</developer>
// <completed>75</completed>

using System;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

using SharpVectors.Net;
using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The StyleSheet interface is the abstract base interface for any type of style sheet. It represents a single style sheet associated with a structured document. In HTML, the StyleSheet interface represents either an external style sheet, included via the HTML LINK element, or an inline STYLE element. In XML, this interface represents an external style sheet, included via a style sheet processing instruction.
	/// </summary>
	public class StyleSheet : IStyleSheet
	{
        #region Private Fields

        private bool TriedDownload;
        private bool SucceededDownload;
        private MediaList _Media;
        private string _Title;
        private string _Href;
        private IStyleSheet _ParentStyleSheet;
        private XmlNode ownerNode;
        private bool _Disabled;
        private string _Type;
        private string sheetContent;

        #endregion

		#region Constructors and Destructor

        protected StyleSheet(string media)
		{
            _Title = String.Empty;
            _Href  = String.Empty;
            _Type  = String.Empty;

            if (String.IsNullOrEmpty(media))
            {
                _Media = new MediaList();
            }
            else
            {
                _Media = new MediaList(media);
            }
		}

        public StyleSheet(XmlProcessingInstruction pi)
            : this(String.Empty)
		{
			Regex re = new Regex(@"(?<name>[a-z]+)=[""'](?<value>[^""']*)[""']");
            Match match = re.Match(pi.Data);

			while(match.Success)
			{
				string name = match.Groups["name"].Value;
				string val = match.Groups["value"].Value;

				switch(name)
				{
					case "href":
						_Href = val;
						break;
					case "type":
						_Type = val;
						break;
					case "title":
						_Title = val;
						break;
					case "media":
						_Media = new MediaList(val);
						break;
				}
				match = match.NextMatch();
			}

			ownerNode = pi;
		}

        public StyleSheet(XmlElement styleElement)
            : this(String.Empty)
		{
			if (styleElement.HasAttribute("href")) 
                _Href = styleElement.Attributes["href"].Value;
			if (styleElement.HasAttribute("type")) 
                _Type = styleElement.Attributes["type"].Value;
			if (styleElement.HasAttribute("title")) 
                _Title = styleElement.Attributes["title"].Value;
			if (styleElement.HasAttribute("media")) 
                _Media = new MediaList(styleElement.Attributes["media"].Value);

			ownerNode = styleElement;
		}

		public StyleSheet(XmlNode ownerNode, string href, string type, string title, string media)
            : this(media)
		{
			this.ownerNode = ownerNode;

			_Href  = href;
			_Type  = type;
			_Title = title;
		}

		#endregion

		#region Protected Properties

		internal string SheetContent
		{
			get
			{
				if(OwnerNode is XmlElement)
				{
                    return OwnerNode.InnerText;
				}
				else
				{
					// a PI
					if(!TriedDownload)
					{
						LoadSheet();
					}
					if(SucceededDownload) return sheetContent;
					else return String.Empty;
				}
			}

		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="ml">The medialist that the document is using</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		protected internal virtual void GetStylesForElement(XmlElement elt, string pseudoElt, 
            MediaList ml, CssCollectedStyleDeclaration csd)
		{
			
		}

        internal XmlNode ResolveOwnerNode()
        {
            if (OwnerNode != null) return OwnerNode;
            else
            {
                return ((StyleSheet)ParentStyleSheet).ResolveOwnerNode();
            }
        }

		#endregion

		#region Public Methods

		#endregion

		#region Protected Methods

		internal void LoadSheet()
		{
			//WebRequest request = (WebRequest)WebRequest.Create(AbsoluteHref);
            WebRequest request = new ExtendedHttpWebRequest(AbsoluteHref);
			TriedDownload = true;
			try
			{
				WebResponse response = (WebResponse)request.GetResponse();

				SucceededDownload = true;
				System.IO.StreamReader str = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default, true);
				sheetContent = str.ReadToEnd();
				str.Close();
			}
			catch
			{
				SucceededDownload = false;
				sheetContent = String.Empty;
			}

		}

		#endregion

		#region IStyleSheet Members

		/// <summary>
		/// The intended destination media for style information. The media is often specified in the ownerNode. If no media has been specified, the MediaList will be empty. See the media attribute definition for the LINK element in HTML 4.0, and the media pseudo-attribute for the XML style sheet processing instruction . Modifying the media list may cause a change to the attribute disabled.
		/// </summary>
		public IMediaList Media
		{
			get
			{
				return _Media;
			}
		}

		/// <summary>
		/// The advisory title. The title is often specified in the ownerNode. See the title attribute definition for the LINK element in HTML 4.0, and the title pseudo-attribute for the XML style sheet processing instruction.
		/// </summary>
		public string Title
		{
			get
			{
				return _Title;
			}
		}

		/// <summary>
		/// If the style sheet is a linked style sheet, the value of its attribute is its location. For inline style sheets, the value of this attribute is null. See the href attribute definition for the LINK element in HTML 4.0, and the href pseudo-attribute for the XML style sheet processing instruction.
		/// </summary>
		public string Href
		{
			get
			{
				return _Href;
			}
		}
		/// <summary>
		/// The resolved absolute URL to the stylesheet
		/// </summary>
        public Uri AbsoluteHref
        {
            get
            {
                Uri u = null;
                if (ownerNode != null)
                {
                    if (ownerNode.BaseURI != null)
                    {
                        u = new Uri(new Uri(ownerNode.BaseURI), Href);
                    }
                    //else
                    //{                           
                    //    u = new Uri(ApplicationContext.DocumentDirectoryUri, Href);
                    //}
                }
                
                if (u == null)
                {
                    throw new InvalidDataException();
                }

                return u;
            }
        }

		/// <summary>
		/// For style sheet languages that support the concept of style sheet inclusion, this attribute represents the including style sheet, if one exists. If the style sheet is a top-level style sheet, or the style sheet language does not support inclusion, the value of this attribute is null.
		/// </summary>
        public IStyleSheet ParentStyleSheet
        {
            get
            {
                return _ParentStyleSheet;
            }
            set
            {
                _ParentStyleSheet = value;
            }
        }

		/// <summary>
		/// The node that associates this style sheet with the document. For HTML, this may be the corresponding LINK or STYLE element. For XML, it may be the linking processing instruction. For style sheets that are included by other style sheets, the value of this attribute is null.
		/// </summary>
		public XmlNode OwnerNode
		{
			get
			{
				return ownerNode;
			}
		}

		/// <summary>
		/// false if the style sheet is applied to the document. true if it is not. Modifying this attribute may cause a new resolution of style for the document. A stylesheet only applies if both an appropriate medium definition is present and the disabled attribute is false. So, if the media doesn't apply to the current user agent, the disabled attribute is ignored.
		/// </summary>
		public bool Disabled
		{
			get
			{
				return _Disabled;
			}
			set
			{
				_Disabled = value;
			}
		}

		/// <summary>
		/// This specifies the style sheet language for this style sheet. The style sheet language is specified as a content type (e.g. "text/css"). The content type is often specified in the ownerNode. Also see the type attribute definition for the LINK element in HTML 4.0, and the type pseudo-attribute for the XML style sheet processing instruction.
		/// </summary>
		public string Type
		{
			get
			{
				return _Type;
			}
		}

		#endregion
	}
}
