// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;

using SharpVectors.Xml;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// A XmlDocument with CSS support
    /// </summary>
    public class CssXmlDocument : Document, IDocumentCss, ICssView
    {
        #region Private Fields

        internal List<string[]> styleElements = new List<string[]>();
        internal MediaList _currentMedia = new MediaList("all");
        public CssStyleSheet UserAgentStyleSheet;
        public CssStyleSheet UserStyleSheet;

        private bool _static;
        private StyleSheetList _StyleSheets;
        private CssPropertyProfile _cssPropertyProfile = new CssPropertyProfile();

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of CssXmlDocument
        /// </summary>
        public CssXmlDocument()
            : base()
        {
            setupNodeChangeListeners();

            SharpVectors.Net.ExtendedHttpWebRequest.Register();
            SharpVectors.Net.DataWebRequest.Register();
        }

        /// <summary>
        /// Initializes a new instance of CssXmlDocument
        /// </summary>
        /// <param name="nt">The name table to use</param>
        public CssXmlDocument(XmlNameTable nt)
            : base(nt)
        {
            setupNodeChangeListeners();

            SharpVectors.Net.ExtendedHttpWebRequest.Register();
            SharpVectors.Net.DataWebRequest.Register();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CssXmlDocument"/> handles DOM dynamic changes.
        /// Sometimes (when loading or rendering) this needs to be disabled.
        /// See <see href="StaticSection"/> for more information about use
        /// </summary>
        /// <value><c>true</c> if static; otherwise, <c>false</c>.</value>
        public bool Static
        {
            get { return _static; }
            set { _static = value; }
        }

        public MediaList Media
        {
            get
            {
                return _currentMedia;
            }
            set
            {
                _currentMedia = value;

            }
        }

        public CssPropertyProfile CssPropertyProfile
        {
            get
            {
                return _cssPropertyProfile;
            }
            set
            {
                _cssPropertyProfile = value;
            }
        }

        public string Url
        {
            get
            {
                return BaseURI;
            }
        }

        /// <summary>
        /// All the stylesheets associated with this document
        /// </summary>
        public IStyleSheetList StyleSheets
        {
            get
            {
                if (_StyleSheets == null)
                {
                    _StyleSheets = new StyleSheetList(this);
                }
                return _StyleSheets;
            }
        }

        #endregion

        #region Public Methods

        public override XmlElement CreateElement(string prefix, string localName, string ns)
        {
            return new CssXmlElement(prefix, localName, ns, this);
        }

        /// <summary>
        /// Loads a XML document, compare to XmlDocument.Load()
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename)
        {
            using (StaticSection.Use(this))
            {   
                // remove any hash (won't work for local files)
                int hashStart = filename.IndexOf("#");
                if (hashStart > -1)
                {
                    filename = filename.Substring(0, hashStart);
                }
                base.Load(filename);
            }
        }

        public override void LoadXml(string xml)
        {
            using (StaticSection.Use(this))
            {
                //base.LoadXml(xml);
                // we use a stream here, only not to use SvgDocument.Load(XmlReader)
                using (var xmlStream = new StringReader(xml))
                {
                    Load(xmlStream);
                }
            }
        }

        //JR added in
        public override void Load(XmlReader reader)
        {
            using (StaticSection.Use(this))
            {
                base.Load(reader);
            }
        }

        /// <summary>
        /// Adds a element type to be used as style elements (e.g. as in the HTML style element)
        /// </summary>
        /// <param name="ns">The namespace URI of the element</param>
        /// <param name="localName">The local-name of the element</param>
        public void AddStyleElement(string ns, string localName)
        {
            styleElements.Add(new string[] { ns, localName });
        }

        /// <summary>
        /// Sets the user agent stylesheet for this document
        /// </summary>
        /// <param name="href">The URI to the stylesheet</param>
        public void SetUserAgentStyleSheet(string href)
        {
            UserAgentStyleSheet = new CssStyleSheet(null, href, String.Empty, String.Empty, null, CssStyleSheetType.UserAgent);
        }

        /// <summary>
        /// Sets the user stylesheet for this document
        /// </summary>
        /// <param name="href">The URI to the stylesheet</param>
        public void SetUserStyleSheet(string href)
        {
            UserStyleSheet = new CssStyleSheet(null, href, String.Empty, String.Empty, null, CssStyleSheetType.User);
        }

        public void AddStyleSheet(string href)
        {

            UserStyleSheet = new CssStyleSheet(null, href, String.Empty, String.Empty, null, CssStyleSheetType.User);
        }

        public WebResponse GetResource(Uri absoluteUri)
        {
            WebRequest request = WebRequest.Create(absoluteUri);
            WebResponse response = request.GetResponse();

            return response;
        }

        #endregion

        #region IDocumentCss Members

        /// <summary>
        /// This method is used to retrieve the override style declaration for a specified element and a specified pseudo-element.
        /// </summary>
        /// <param name="elt">The element whose style is to be modified. This parameter cannot be null.</param>
        /// <param name="pseudoElt">The pseudo-element or null if none.</param>
        /// <returns>The override style declaration.</returns>
        public SharpVectors.Dom.Css.ICssStyleDeclaration GetOverrideStyle(System.Xml.XmlElement elt, string pseudoElt)
        {
            throw new NotImplementedException("CssXmlDocument.GetOverrideStyle()");
        }

        #endregion

        #region IViewCss Members

        /// <summary>
        /// This method is used to get the computed style as it is defined in [CSS2].
        /// </summary>
        /// <param name="elt">The element whose style is to be computed. This parameter cannot be null.</param>
        /// <param name="pseudoElt">The pseudo-element or null if none.</param>
        /// <returns>The computed style. The CSSStyleDeclaration is read-only and contains only absolute values.</returns>
        public SharpVectors.Dom.Css.ICssStyleDeclaration GetComputedStyle(System.Xml.XmlElement elt, string pseudoElt)
        {
            if (elt == null) throw new NullReferenceException();
            else if (!(elt is CssXmlElement)) throw new DomException(DomExceptionType.SyntaxErr, "element must of type CssXmlElement");
            else
            {
                return ((CssXmlElement)elt).GetComputedStyle(pseudoElt);
            }
        }

        #endregion

        #region Update handling

        private void setupNodeChangeListeners()
        {
            XmlNodeChangedEventHandler handler = new XmlNodeChangedEventHandler(NodeChangedEvent);

            NodeChanged += handler;
            NodeInserted += handler;
            //NodeRemoving += handler;
            NodeRemoved += handler;
        }

        public void NodeChangedEvent(Object src, XmlNodeChangedEventArgs args)
        {
            if (!Static)
            {
                #region Attribute updates
                // xmlns:xml is auto-inserted whenever a selectNode is performed, we don't want those events
                if (args.Node is XmlText && args.NewParent is XmlAttribute && args.NewParent.Name != "xmlns:xml")
                {
                    XmlAttribute attr = args.NewParent as XmlAttribute;
                    CssXmlElement elm = attr.OwnerElement as CssXmlElement;
                    if (elm != null)
                    {
                        elm.AttributeChange(attr, args);
                    }
                }
                else if (args.Node is XmlAttribute && args.Node.Name != "xmlns:xml")
                {
                    // the cause of the change is a XmlAttribute => happens during inserting or removing
                    CssXmlElement oldElm = args.OldParent as CssXmlElement;
                    if (oldElm != null) oldElm.AttributeChange(args.Node, args);

                    CssXmlElement newElm = args.NewParent as CssXmlElement;
                    if (newElm != null) newElm.AttributeChange(args.Node, args);
                }
                #endregion

                #region OnElementChange
                if (args.Node is XmlText && args.NewParent is XmlElement)
                {
                    CssXmlElement element = (CssXmlElement)args.NewParent;
                    element.ElementChange(src, args);
                }
                else if (args.Node is CssXmlElement)
                {
                    if (args.Action == XmlNodeChangedAction.Insert || args.Action == XmlNodeChangedAction.Change)
                    {
                        // Changes to a child XML node may affect the sibling offsets (for example in tspan)
                        // By calling the parent's OnElementChange, invalidation will propogate back to Node
                        CssXmlElement newParent = (CssXmlElement)args.NewParent;
                        newParent.ElementChange(src, args);
                    }

                    if (args.Action == XmlNodeChangedAction.Remove)
                    {
                        // Removing a child XML node may affect the sibling offsets (for example in tspan)
                        CssXmlElement oldParent = (CssXmlElement)args.OldParent;
                        oldParent.ElementChange(src, args);
                    }
                }
                #endregion

            }
        }

        #endregion
    }
}
