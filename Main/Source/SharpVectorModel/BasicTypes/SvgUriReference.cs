using System;
using System.Xml;
using System.IO;
using System.Net;
using SharpVectors.Net;
using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgUriReference : ISvgUriReference
    {
        #region Private Fields

        private string absoluteUri;
        private SvgElement ownerElement;
        private ISvgAnimatedString href;

        #endregion

        #region Constructors and Destructor

        public SvgUriReference(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
            this.ownerElement.attributeChangeHandler += new NodeChangeHandler(AttributeChange);
        }

        #endregion

        #region Public Events

        public event NodeChangeHandler NodeChanged;

        #endregion

        #region Public Properties

        public string AbsoluteUri
        {
            get
            {
                if (absoluteUri == null)
                {
                    if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                    {
                        string href = Href.AnimVal.Trim();

                        if (href.StartsWith("#"))
                        {
                            return href;
                        }
                        else
                        {
                            string baseUri = ownerElement.BaseURI;
                            if (baseUri.Length == 0)
                            {
                                Uri sourceUri = new Uri(Href.AnimVal, UriKind.RelativeOrAbsolute);
                                if (sourceUri.IsAbsoluteUri)
                                {
                                    absoluteUri = sourceUri.ToString(); 
                                }
                            }
                            else
                            {
                                Uri sourceUri = null;
                                string xmlBaseUrl = this.GetBaseUrl();
                                if (!String.IsNullOrEmpty(xmlBaseUrl))
                                {
                                    sourceUri = new Uri(new Uri(ownerElement.BaseURI), 
                                        Path.Combine(xmlBaseUrl, Href.AnimVal));
                                }
                                else
                                {
                                    sourceUri = new Uri(new Uri(ownerElement.BaseURI), Href.AnimVal);
                                }

                                absoluteUri = sourceUri.ToString();
                            }
                        }
                    }
                }
                return absoluteUri;
            }
        }

        public XmlNode ReferencedNode
        {
            get
            {
                if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                {
                    XmlNode referencedNode = ownerElement.OwnerDocument.GetNodeByUri(AbsoluteUri);

                    ISvgExternalResourcesRequired extReqElm =
                        ownerElement as ISvgExternalResourcesRequired;

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
                if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                {
                    string absoluteUri = this.AbsoluteUri;
                    if (!String.IsNullOrEmpty(absoluteUri))
                    {
                        WebResponse referencedResource = ownerElement.OwnerDocument.GetResource(
                            new Uri(absoluteUri));

                        ISvgExternalResourcesRequired extReqElm =
                            ownerElement as ISvgExternalResourcesRequired;

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

        private void AttributeChange(Object src, XmlNodeChangedEventArgs args)
        {
            XmlAttribute attribute = src as XmlAttribute;

            if (attribute.NamespaceURI == SvgDocument.XLinkNamespace &&
                attribute.LocalName == "href")
            {
                href = null;
                absoluteUri = null;
            }
        }

        private void ReferencedNodeChange(Object src, XmlNodeChangedEventArgs args)
        {
            if (NodeChanged != null)
            {
                NodeChanged(src, args);
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                if (href == null)
                {
                    href = new SvgAnimatedString(ownerElement.GetAttribute("href", 
                        SvgDocument.XLinkNamespace));
                }
                return href;
            }
        }

        #endregion

        #region Private Methods

        private string GetBaseUrl()
        {
            if (ownerElement.HasAttribute("xml:base"))
            {
                return ownerElement.GetAttribute("xml:base"); 
            }
            XmlElement parentNode = ownerElement.ParentNode as XmlElement;
            if (parentNode != null && parentNode.HasAttribute("xml:base"))
            {
                return parentNode.GetAttribute("xml:base"); 
            }

            return null;
        }

        #endregion
    }
}
