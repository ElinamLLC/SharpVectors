using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

using SharpVectors.Net;
using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Stylesheets
{
    /// <summary>
    /// The StyleSheet interface is the abstract base interface for any type of style sheet. It represents a 
    /// single style sheet associated with a structured document. In HTML, the StyleSheet interface represents 
    /// either an external style sheet, included via the HTML LINK element, or an inline STYLE element. 
    /// In XML, this interface represents an external style sheet, included via a style sheet processing instruction.
    /// </summary>
    public class StyleSheet : IStyleSheet
    {
        #region Private Fields

        private static readonly Regex _reParser = new Regex(@"(?<name>[a-z]+)=[""'](?<value>[^""']*)[""']");

        private bool _triedDownload;
        private bool _succeededDownload;
        private MediaList _media;
        private string _title;
        private string _href;
        private IStyleSheet _parentStyleSheet;
        private XmlNode _ownerNode;
        private bool _disabled;
        private string _type;
        private string _sheetContent;

        #endregion

        #region Constructors and Destructor

        protected StyleSheet(string media)
        {
            _title = string.Empty;
            _href  = string.Empty;
            _type  = string.Empty;

            if (string.IsNullOrWhiteSpace(media))
            {
                _media = new MediaList();
            }
            else
            {
                _media = new MediaList(media);
            }
        }

        public StyleSheet(XmlProcessingInstruction pi)
            : this(string.Empty)
        {
            Match match = _reParser.Match(pi.Data);

            while (match.Success)
            {
                string name = match.Groups["name"].Value;
                string val  = match.Groups["value"].Value;

                switch (name)
                {
                    case "href":
                        _href = val;
                        break;
                    case "type":
                        _type = val;
                        break;
                    case "title":
                        _title = val;
                        break;
                    case "media":
                        _media = new MediaList(val);
                        break;
                }
                match = match.NextMatch();
            }

            _ownerNode = pi;
        }

        public StyleSheet(XmlElement styleElement)
            : this(string.Empty)
        {
            if (styleElement.HasAttribute("href"))
                _href = styleElement.Attributes["href"].Value;
            if (styleElement.HasAttribute("type"))
                _type = styleElement.Attributes["type"].Value;
            if (styleElement.HasAttribute("title"))
                _title = styleElement.Attributes["title"].Value;
            if (styleElement.HasAttribute("media"))
                _media = new MediaList(styleElement.Attributes["media"].Value);

            _ownerNode = styleElement;
        }

        public StyleSheet(XmlNode ownerNode, string href, string type, string title, string media)
            : this(media)
        {
            _ownerNode = ownerNode;
            _href      = href;
            _type      = type;
            _title     = title;
        }

        #endregion

        #region Public Properties

        public string SheetContent
        {
            get {
                // yavor87 committed on Sep 19, 2016: Creative-Safety-Supply/sharpvectors
                //TODO: Fixed stackoverlfow exception in cases when @import css stylesheet statement is used
                //if (this.OwnerNode is XmlElement)
                if (this.OwnerNode is XmlElement && string.IsNullOrWhiteSpace(_href))
                {
                    return this.OwnerNode.InnerText;
                }

                // a PI
                if (!_triedDownload)
                {
                    LoadSheet();
                }
                if (_succeededDownload)
                {
                    return _sheetContent;
                }
                return string.Empty;
            }

        }

        #endregion

        #region Internal Methods

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
            if (this.OwnerNode != null)
                return this.OwnerNode;
            return ((StyleSheet)this.ParentStyleSheet).ResolveOwnerNode();
        }

        #endregion

        #region Protected Methods

        internal void LoadSheet()
        {
            var absoluteUri = this.AbsoluteHref;
            if (absoluteUri == null)
            {
                return;
            }
            if (absoluteUri.IsAbsoluteUri && absoluteUri.IsFile)
            {
                try
                {
                    _succeededDownload = true;
                    StreamReader str = new StreamReader(absoluteUri.LocalPath, Encoding.Default, true);
                    _sheetContent = str.ReadToEnd();
                    str.Close();
                }
                catch
                {
                    _succeededDownload = false;
                    _sheetContent = string.Empty;
                }
            }
            else
            {
                WebRequest request = new ExtendedHttpWebRequest(AbsoluteHref);
                _triedDownload = true;
                try
                {
                    WebResponse response = request.GetResponse();

                    _succeededDownload = true;
                    StreamReader str = new StreamReader(response.GetResponseStream(), Encoding.Default, true);
                    _sheetContent = str.ReadToEnd();
                    str.Close();
                }
                catch
                {
                    _succeededDownload = false;
                    _sheetContent = string.Empty;
                }
            }
        }

        #endregion

        #region IStyleSheet Members

        /// <summary>
        /// The intended destination media for style information. The media is often specified in the 
        /// ownerNode. If no media has been specified, the MediaList will be empty. See the media 
        /// attribute definition for the LINK element in HTML 4.0, and the media pseudo-attribute 
        /// for the XML style sheet processing instruction . Modifying the media list may cause a 
        /// change to the attribute disabled.
        /// </summary>
        public IMediaList Media
        {
            get {
                return _media;
            }
        }

        /// <summary>
        /// The advisory title. The title is often specified in the ownerNode. See the title 
        /// attribute definition for the LINK element in HTML 4.0, and the title pseudo-attribute 
        /// for the XML style sheet processing instruction.
        /// </summary>
        public string Title
        {
            get {
                return _title;
            }
        }

        /// <summary>
        /// If the style sheet is a linked style sheet, the value of its attribute is its location. 
        /// For inline style sheets, the value of this attribute is null. See the href attribute 
        /// definition for the LINK element in HTML 4.0, and the href pseudo-attribute for the 
        /// XML style sheet processing instruction.
        /// </summary>
        public string Href
        {
            get {
                return _href;
            }
        }
        /// <summary>
        /// The resolved absolute URL to the stylesheet
        /// </summary>
        public Uri AbsoluteHref
        {
            get {
                Uri uri = null;
                if (_ownerNode != null)
                {
                    // Fixed logic for cases in which document base uri is unknown
                    if (!string.IsNullOrWhiteSpace(_ownerNode.BaseURI))
                    {
                        uri = new Uri(new Uri(_ownerNode.BaseURI), Href);
                    }
                    else
                    {
                        uri = new Uri(Href, UriKind.RelativeOrAbsolute);
                    }
                }
                if (uri == null)
                {
                    throw new InvalidDataException();
                }
                return uri;
            }
        }

        /// <summary>
        /// For style sheet languages that support the concept of style sheet inclusion, this 
        /// attribute represents the including style sheet, if one exists. If the style sheet 
        /// is a top-level style sheet, or the style sheet language does not support inclusion, 
        /// the value of this attribute is null.
        /// </summary>
        public IStyleSheet ParentStyleSheet
        {
            get {
                return _parentStyleSheet;
            }
            set {
                _parentStyleSheet = value;
            }
        }

        /// <summary>
        /// The node that associates this style sheet with the document. For HTML, this may be 
        /// the corresponding LINK or STYLE element. For XML, it may be the linking processing 
        /// instruction. For style sheets that are included by other style sheets, the value 
        /// of this attribute is null.
        /// </summary>
        public XmlNode OwnerNode
        {
            get {
                return _ownerNode;
            }
        }

        /// <summary>
        /// false if the style sheet is applied to the document. true if it is not. Modifying 
        /// this attribute may cause a new resolution of style for the document. A stylesheet 
        /// only applies if both an appropriate medium definition is present and the disabled 
        /// attribute is false. So, if the media doesn't apply to the current user agent, 
        /// the disabled attribute is ignored.
        /// </summary>
        public bool Disabled
        {
            get {
                return _disabled;
            }
            set {
                _disabled = value;
            }
        }

        /// <summary>
        /// This specifies the style sheet language for this style sheet. The style sheet language 
        /// is specified as a content type (e.g. "text/css"). The content type is often specified 
        /// in the ownerNode. Also see the type attribute definition for the LINK element in HTML 4.0, 
        /// and the type pseudo-attribute for the XML style sheet processing instruction.
        /// </summary>
        public string Type
        {
            get {
                return _type;
            }
        }

        #endregion
    }
}
