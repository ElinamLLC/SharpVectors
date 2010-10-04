// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>75</completed>

using System;
using System.Xml;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgElement.
    /// </summary>
    public class SvgElement : CssXmlElement, ISvgElement
    {
        #region Private Fields

        private bool                _isImported;
        private SvgElement          _importNode;
        private SvgDocument         _importDocument;
        private ISvgElementInstance _elementInstance;

        #endregion

        #region Constructors and Destructors

        public SvgElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties

        public bool Imported
        {
            get 
            { 
                return _isImported; 
            }
            set 
            { 
                _isImported = value; 
            }
        }

        public SvgElement ImportNode
        {
            get 
            { 
                return _importNode; 
            }
            set 
            { 
                _importNode = value; 
            }
        }

        public SvgDocument ImportDocument
        {
            get 
            { 
                return _importDocument; 
            }
            set 
            { 
                _importDocument = value; 
            }
        }

        #endregion

        #region ISvgElement Members

        public new SvgDocument OwnerDocument
        {
            get
            {
                return base.OwnerDocument as SvgDocument;
            }
        }

        public string Id
        {
            get
            {
                return GetAttribute("id");
            }
            set
            {
                SetAttribute("id", value);
            }
        }

        public ISvgSvgElement OwnerSvgElement
        {
            get
            {
                if (this.Equals(OwnerDocument.DocumentElement))
                {
                    return null;
                }
                else
                {
                    XmlNode parent = ParentNode;
                    while (parent != null && !(parent is SvgSvgElement))
                    {
                        parent = parent.ParentNode;
                    }
                    return parent as SvgSvgElement;
                }
            }
        }

        public ISvgElement ViewportElement
        {
            get
            {
                if (this.Equals(OwnerDocument.DocumentElement))
                {
                    return null;
                }
                else
                {
                    XmlNode parent = ParentNode;
                    while (parent != null && !(parent is SvgSvgElement) && !(parent is SvgSymbolElement))
                    {
                        parent = parent.ParentNode;
                    }

                    return parent as SvgElement;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public virtual bool IsRenderable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// </value>
        public virtual SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.None;
            }
        }

        #endregion

        #region ISvgLangSpace Members

        public string XmlSpace
        {
            get
            {
                string s = GetAttribute("xml:space");
                if (String.IsNullOrEmpty(s))
                {
                    SvgElement par = this.ParentNode as SvgElement;
                    if (par != null)
                    {
                        s = par.XmlSpace;
                    }
                    else
                    {
                        s = "default";
                    }
                }

                return s;
            }
            set
            {
                SetAttribute("xml:space", value);
            }
        }

        public string XmlLang
        {
            get
            {
                string s = this.GetAttribute("xml:lang");
                if (String.IsNullOrEmpty(s))
                {
                    SvgElement par = this.ParentNode as SvgElement;
                    if (par != null)
                    {
                        s = par.XmlLang;
                    }
                    else
                    {
                        s = String.Empty;
                    }
                }

                return s;
            }
            set
            {
                this.SetAttribute("xml:lang", value);
            }
        }

        #endregion

        #region Other public methods

        public string ResolveUri(string uri)
        {
            uri = uri.Trim();
            if (uri.StartsWith("#"))
            {
                return uri;
            }
            else
            {
                string baseUri = BaseURI;
                if (baseUri.Length == 0)
                {
                    return uri;
                }
                else
                {
                    return new Uri(new Uri(baseUri), uri).AbsoluteUri;
                }
            }
        }

        /// <summary>
        /// Whenever an SvgElementInstance is created for an SvgElement this
        /// property is set. The value of this property is used by the renderer 
        /// to dispatch events. SvgElements that are &lt;use&gt;d exist in a 
        /// conceptual "instance tree" and the target of events for those elements
        /// is the conceptual instance node represented by the SvgElementInstance.
        /// <see cref="http://www.w3.org/TR/SVG/struct.html#UseElement"/>
        /// <see cref="http://www.w3.org/TR/SVG/struct.html#InterfaceSVGElementInstance"/>
        /// </summary>
        public ISvgElementInstance ElementInstance
        {
            get
            {
                return _elementInstance;
            }
            set
            {
                _elementInstance = value;
            }
        }

        #endregion
    }
}
