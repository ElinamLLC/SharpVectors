using System;
using System.Xml;
using System.IO;
using System.Net;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgUriReference : ISvgUriReference
    {
        #region Private Fields

        private string _absoluteUri;
        private SvgElement _ownerElement;
        private ISvgAnimatedString _href;

        #endregion

        #region Constructors and Destructor

        public SvgUriReference(SvgElement ownerElement)
        {
            _ownerElement = ownerElement;
            _ownerElement.attributeChangeHandler += OnAttributeChange;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get {
                if (_ownerElement != null)
                {
                    return !(_ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace) ||
                        _ownerElement.HasAttribute("href"));
                }
                return true;
            }
        }

        public string AbsoluteUri
        {
            get
            {
                if (_absoluteUri == null)
                {
                    // Since SVG 2, the xlink:href attribute is deprecated in favor of simply href. 
                    // We are supporting both options
                    if (_ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace) ||
                        _ownerElement.HasAttribute("href"))
                    {
                        string href = Href.AnimVal.Trim().Trim(new char[] { '\"', '\'' });

                        if (href.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                        {
                            return href;
                        }
                        else
                        {
                            string baseUri = _ownerElement.BaseURI;
                            if (baseUri.Length == 0)
                            {
                                Uri sourceUri = new Uri(Href.AnimVal, UriKind.RelativeOrAbsolute);
                                if (sourceUri.IsAbsoluteUri)
                                {
                                    _absoluteUri = sourceUri.ToString(); 
                                }
                            }
                            else
                            {
                                Uri sourceUri = null;
                                string xmlBaseUrl = this.GetBaseUrl();
                                if (!string.IsNullOrWhiteSpace(xmlBaseUrl))
                                {
                                    sourceUri = new Uri(new Uri(_ownerElement.BaseURI), 
                                        Path.Combine(xmlBaseUrl, Href.AnimVal));
                                }
                                else
                                {
                                    sourceUri = new Uri(new Uri(_ownerElement.BaseURI), Href.AnimVal);
                                }

                                _absoluteUri = sourceUri.ToString();
                            }
                        }
                    }
                }
                return _absoluteUri;
            }
        }

        public XmlNode ReferencedNode
        {
            get
            {
                string absoluteUri = this.AbsoluteUri;
                if (string.IsNullOrWhiteSpace(absoluteUri))
                {
                    return null;
                }


                // Since SVG 2, the xlink:href attribute is deprecated in favor of simply href. 
                // We are supporting both options
                if (_ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace)
                    || _ownerElement.HasAttribute("href"))
                {
                    XmlNode referencedNode = _ownerElement.OwnerDocument.GetNodeByUri(absoluteUri);

                    ISvgExternalResourcesRequired extReqElm = _ownerElement as ISvgExternalResourcesRequired;

                    if (referencedNode == null && extReqElm != null)
                    {
                        if (extReqElm.ExternalResourcesRequired.AnimVal)
                        {
                            throw new SvgExternalResourcesRequiredException();
                        }
                    }

                    return referencedNode;
                }

                return null;
            }
        }

        public WebResponse ReferencedResource
        {
            get
            {
                // Since SVG 2, the xlink:href attribute is deprecated in favor of simply href. 
                // We are supporting both options
                if (_ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace)
                    || _ownerElement.HasAttribute("href"))
                {
                    string absoluteUri = this.AbsoluteUri;
                    if (!string.IsNullOrWhiteSpace(absoluteUri))
                    {
                        WebResponse referencedResource = _ownerElement.OwnerDocument.GetResource(
                            new Uri(absoluteUri));

                        ISvgExternalResourcesRequired extReqElm =
                            _ownerElement as ISvgExternalResourcesRequired;

                        if (referencedResource == null && extReqElm != null)
                        {
                            if (extReqElm.ExternalResourcesRequired.AnimVal)
                            {
                                throw new SvgExternalResourcesRequiredException();
                            }
                        }

                        return referencedResource;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Update handling

        private void OnAttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            XmlAttribute attribute = src as XmlAttribute;

            // Since SVG 2, the xlink:href attribute is deprecated in favor of simply href. 
            // We are supporting both options
            if (attribute.NamespaceURI == SvgDocument.XLinkNamespace &&
                string.Equals(attribute.LocalName, "href", StringComparison.Ordinal))
            {
                _href = null;
                _absoluteUri = null;
            }
            else if (string.Equals(attribute.LocalName, "href", StringComparison.Ordinal))
            {
                _href = null;
                _absoluteUri = null;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                if (_href == null)
                {
                    // Since SVG 2, the xlink:href attribute is deprecated in favor of simply href. 
                    // We are supporting both options
                    if (_ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                    {
                        _href = new SvgAnimatedString(_ownerElement.GetAttribute("href", 
                            SvgDocument.XLinkNamespace));
                    }
                    else if (_ownerElement.HasAttribute("href"))
                    {
                        _href = new SvgAnimatedString(_ownerElement.GetAttribute("href"));
                    }
                }
                return _href;
            }
        }

        #endregion

        #region Private Methods

        private string GetBaseUrl()
        {
            if (_ownerElement.HasAttribute("xml:base"))
            {
                return _ownerElement.GetAttribute("xml:base"); 
            }
            XmlElement parentNode = _ownerElement.ParentNode as XmlElement;
            if (parentNode != null && parentNode.HasAttribute("xml:base"))
            {
                return parentNode.GetAttribute("xml:base"); 
            }

            return null;
        }

        #endregion
    }
}
