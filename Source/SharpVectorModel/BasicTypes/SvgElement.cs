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

        private Guid _uniqueId;

        private ISvgElementInstance _elementInstance;

        #endregion

        #region Constructors and Destructors

        public SvgElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uniqueId = Guid.NewGuid();
        }

        #endregion

        #region Public Properties

        public Guid UniqueId
        {
            get {
                return _uniqueId;
            }
        }

        public SvgElement ImportNode
        {
            get {
                return (SvgElement)_importNode;
            }
            set {
                _importNode = value;
            }
        }

        public SvgDocument ImportDocument
        {
            get {
                return (SvgDocument)_importDocument;
            }
            set {
                _importDocument = value;
            }
        }

        #endregion

        #region ISvgElement Members

        public new SvgDocument OwnerDocument
        {
            get {
                return base.OwnerDocument as SvgDocument;
            }
        }

        public string Id
        {
            get {
                return this.GetAttribute("id");
            }
            set {
                this.SetAttribute("id", value);
            }
        }

        public ISvgSvgElement OwnerSvgElement
        {
            get {
                if (this.Equals(OwnerDocument.DocumentElement))
                {
                    return null;
                }
                XmlNode parent = ParentNode;
                while (parent != null && !(parent is SvgSvgElement))
                {
                    parent = parent.ParentNode;
                }
                return parent as SvgSvgElement;
            }
        }

        public ISvgElement ViewportElement
        {
            get {
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
            get {
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
            get {
                return SvgRenderingHint.None;
            }
        }

        #endregion

        #region ISvgLangSpace Members

        public string XmlSpace
        {
            get {
                string s = GetAttribute("xml:space");
                if (string.IsNullOrWhiteSpace(s))
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
            set {
                SetAttribute("xml:space", value);
            }
        }

        public string XmlLang
        {
            get {
                string s = this.GetAttribute("xml:lang");
                if (string.IsNullOrWhiteSpace(s))
                {
                    SvgElement par = this.ParentNode as SvgElement;
                    if (par != null)
                    {
                        s = par.XmlLang;
                    }
                    else
                    {
                        s = string.Empty;
                    }
                }

                return s;
            }
            set {
                this.SetAttribute("xml:lang", value);
            }
        }

        #endregion

        #region Other Public Properties and Methods

        /// <summary>
        /// Whenever an SvgElementInstance is created for an SvgElement this property is set. The value of 
        /// this property is used by the renderer to dispatch events. SvgElements that are &lt;use&gt;d exist in a 
        /// conceptual "instance tree" and the target of events for those elements is the conceptual instance 
        /// node represented by the SvgElementInstance.
        /// <see cref="http://www.w3.org/TR/SVG/struct.html#UseElement"/>
        /// <see cref="http://www.w3.org/TR/SVG/struct.html#InterfaceSVGElementInstance"/>
        /// </summary>
        public ISvgElementInstance ElementInstance
        {
            get {
                return _elementInstance;
            }
            set {
                _elementInstance = value;
            }
        }

        public string ResolveUri(string uri)
        {
            uri = uri.Trim();
            if (uri.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                return uri;
            }
            string baseUri = BaseURI;
            if (baseUri.Length == 0)
            {
                return uri;
            }
            return new Uri(new Uri(baseUri), uri).AbsoluteUri;
        }

        public float GetAttribute(string name, float defValue)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                float value;
                if (float.TryParse(this.GetAttribute(name), out value))
                {
                    return value;
                }
            }
            return defValue;
        }

        public void SetAttribute(string name, float value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.SetAttribute(name, value.ToString());
            }
        }

        public int GetAttribute(string name, int defValue)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                int value;
                if (int.TryParse(this.GetAttribute(name), out value))
                {
                    return value;
                }
            }
            return defValue;
        }

        public void SetAttribute(string name, int value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.SetAttribute(name, value.ToString());
            }
        }

        public double GetAttribute(string name, double defValue)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                double value;
                if (double.TryParse(this.GetAttribute(name), out value))
                {
                    return value;
                }
            }
            return defValue;
        }

        public void SetAttribute(string name, double value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.SetAttribute(name, value.ToString());
            }
        }

        public long GetAttribute(string name, long defValue)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                long value;
                if (long.TryParse(this.GetAttribute(name), out value))
                {
                    return value;
                }
            }
            return defValue;
        }

        public void SetAttribute(string name, long value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.SetAttribute(name, value.ToString());
            }
        }

        #endregion
    }
}
