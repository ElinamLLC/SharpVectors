using System;
using System.Xml;
using System.Globalization;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgUseElement : SvgTransformableElement, ISvgUseElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;
        private ISvgElementInstance _instanceRoot;

        private SvgTests _svgTests;
        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        // For rendering support...
        private bool _transformAdded;
        private string _saveTransform;
        private string _saveWidth;
        private string _saveHeight;

        #endregion

        #region Constructors and Destructor

        public SvgUseElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
            //            _uriReference.NodeChanged += OnReferencedNodeChange;

            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests = new SvgTests(this);
        }

        #endregion

        #region ISvgUseElement Members

        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                }
                return _x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return _y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width",
                        SvgLengthDirection.Horizontal, string.Empty);
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height",
                        SvgLengthDirection.Vertical, string.Empty);
                }
                return _height;
            }
        }

        public ISvgElementInstance InstanceRoot
        {
            get {
                if (_instanceRoot == null)
                {
                    _instanceRoot = new SvgElementInstance(ReferencedElement, this, null);
                }
                return _instanceRoot;
            }
        }

        public ISvgElementInstance AnimatedInstanceRoot
        {
            get {
                return InstanceRoot;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public XmlElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);

            if (this.HasChildNodes)
            {
                visitor.BeginContainer(this);
                foreach (var item in this.ChildNodes)
                {
                    var evt = item as IElementVisitorTarget;
                    if (evt != null)
                    {
                        evt.Accept(visitor);
                    }
                }
                visitor.EndContainer(this);
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return _svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return _svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return _svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return _svgTests.HasExtension(extension);
        }

        #endregion

        #region Update Handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "x":
                        _x = null;
                        return;
                    case "y":
                        _y = null;
                        return;
                    case "width":
                        _width = null;
                        return;
                    case "height":
                        _height = null;
                        return;
                }
            }
            else if (attribute.NamespaceURI == SvgDocument.XLinkNamespace)
            {
                switch (attribute.LocalName)
                {
                    case "href":
                        _instanceRoot = null;
                        break;
                }
            }

            base.HandleAttributeChange(attribute);
        }

        public void OnReferencedNodeChange(object src, XmlNodeChangedEventArgs args)
        {
            //TODO - This is getting called too often!
            //instanceRoot = null;
        }

        #endregion

        #region Rendering

        public void CopyToReferencedElement(XmlElement refEl)
        {
            // X and Y become a translate portion of any transform, width and height may get passed on
            if (!X.AnimVal.Value.Equals(0) || !Y.AnimVal.Value.Equals(0))
            {
                _saveTransform = this.GetAttribute("transform");
                if (string.IsNullOrWhiteSpace(_saveTransform))
                {
                    string transform = string.Format(CultureInfo.InvariantCulture,
                        "translate({0},{1})", X.AnimVal.Value, Y.AnimVal.Value);

                    this.SetAttribute("transform", transform);
                }
                else
                {
                    string transform = string.Format(CultureInfo.InvariantCulture,
                        "{0} translate({1},{2})", _saveTransform, X.AnimVal.Value, Y.AnimVal.Value);

                    this.SetAttribute("transform", transform);
                }

                _transformAdded = true;
            }

            // if (refEl is SvgSymbolElement)
            if (string.Equals(refEl.Name, "symbol", StringComparison.Ordinal))
            {
                _saveWidth = refEl.GetAttribute("width");
                _saveHeight = refEl.GetAttribute("height");

                refEl.SetAttribute("width", (HasAttribute("width")) ? GetAttribute("width") : "100%");
                refEl.SetAttribute("height", (HasAttribute("height")) ? GetAttribute("height") : "100%");

                SvgSymbolElement symbolElement = (SvgSymbolElement)refEl;
                symbolElement.Width  = null;
                symbolElement.Height = null;
            }
        }

        public void RestoreReferencedElement(XmlElement refEl)
        {
            if (!string.IsNullOrWhiteSpace(_saveTransform))
            { 
                this.SetAttribute("transform", _saveTransform);
            }
            else if (_transformAdded)
            {
                this.RemoveAttribute("transform");
            }
            if (_saveWidth != null && _saveHeight != null)
            {
                refEl.SetAttribute("width", _saveWidth);
                refEl.SetAttribute("height", _saveHeight);
            }

            _transformAdded = false;
            _saveTransform  = null;
            _saveWidth      = null;
            _saveHeight     = null;
        }

        #endregion
    }
}
