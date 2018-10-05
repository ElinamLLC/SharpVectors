using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Globalization;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgImageElement : SvgTransformableElement, ISvgImageElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private SvgTests _svgTests;
        private SvgUriReference _uriReference;
        private SvgFitToViewBox _fitToViewBox;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgImageElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests                  = new SvgTests(this);
            _uriReference              = new SvgUriReference(this);
            _fitToViewBox              = new SvgFitToViewBox(this);
        }

        #endregion

        #region Public properties

        //public SvgRect CalculatedViewbox
        //{
        //    get
        //    {
        //        SvgRect viewBox = null;

        //        if (IsSvgImage)
        //        {
        //            SvgDocument doc = GetImageDocument();
        //            SvgSvgElement outerSvg = (SvgSvgElement)doc.DocumentElement;

        //            if (outerSvg.HasAttribute("viewBox"))
        //            {
        //                viewBox = (SvgRect)outerSvg.ViewBox.AnimVal;
        //            }
        //            else
        //            {
        //                viewBox = SvgRect.Empty;
        //            }
        //        }
        //        else
        //        {
        //            viewBox = new SvgRect(0, 0, Bitmap.Size.Width, Bitmap.Size.Height);
        //        }

        //        return viewBox;
        //    }
        //}

        public bool IsSvgImage
        {
            get
            {
                if (!Href.AnimVal.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string absoluteUri = _uriReference.AbsoluteUri;
                        if (!string.IsNullOrWhiteSpace(absoluteUri))
                        {
                            Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                            if (svgUri.IsFile)
                            {   
                                return absoluteUri.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) || 
                                    absoluteUri.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        WebResponse resource = _uriReference.ReferencedResource;
                        if (resource == null)
                        {
                            return false;
                        }

                        // local files are returning as binary/octet-stream
                        // this "fix" tests the file extension for .svg and .svgz
                        string name = resource.ResponseUri.ToString().ToLower(CultureInfo.InvariantCulture);
                        return (resource.ContentType.StartsWith("image/svg+xml", StringComparison.OrdinalIgnoreCase) ||
                            name.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                            name.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase));
                    }
                    catch (WebException)
                    {
                        return false;
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        public SvgWindow SvgWindow
        {
            get
            {
                if (this.IsSvgImage)
                {
                    SvgWindow parentWindow = (SvgWindow)OwnerDocument.Window;

                    if (parentWindow != null)
                    {
                        SvgWindow wnd = parentWindow.CreateOwnedWindow(
                            (long)Width.AnimVal.Value, (long)Height.AnimVal.Value);

                        SvgDocument doc = new SvgDocument(wnd);
                        wnd.Document = doc;

                        string absoluteUri = _uriReference.AbsoluteUri;

                        Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                        if (svgUri.IsFile)
                        {
                            Stream resStream = File.OpenRead(svgUri.LocalPath);
                            doc.Load(absoluteUri, resStream);
                        }
                        else
                        {
                            Stream resStream = _uriReference.ReferencedResource.GetResponseStream();
                            doc.Load(absoluteUri, resStream);
                        }

                        return wnd;
                    }
                }

                return null;
            }
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Image"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Image;
            }
        }

        #endregion

        #region ISvgImageElement Members

        public ISvgAnimatedLength Width
        {
            get
            {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
                }
                return _height;
            }

        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                }
                return _x;
            }

        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return _y;
            }

        }

        public ISvgColorProfileElement ColorProfile
        {
            get
            {
                string colorProfile = this.GetAttribute("color-profile");

                if (string.IsNullOrWhiteSpace(colorProfile))
                {
                    return null;
                }

                XmlElement profileElement = this.OwnerDocument.GetElementById(colorProfile);
                if (profileElement == null)
                {
                    XmlElement root = this.OwnerDocument.DocumentElement;
                    XmlNodeList elemList = root.GetElementsByTagName("color-profile");
                    if (elemList != null && elemList.Count != 0)
                    {  
                        for (int i = 0; i < elemList.Count; i++)
                        {
                            XmlElement elementNode = elemList[i] as XmlElement;
                            if (elementNode != null && string.Equals(colorProfile, 
                                elementNode.GetAttribute("id")))
                            {
                                profileElement = elementNode;
                                break;
                            }                                     
                        }
                    }
                }

                return profileElement as SvgColorProfileElement;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                return _uriReference.Href;
            }
        }

        public SvgUriReference UriReference
        {
            get
            {
                return _uriReference;
            }
        }

        public XmlElement ReferencedElement
        {
            get
            {
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get
            {
                return _fitToViewBox.PreserveAspectRatio;
            }
        }

        #endregion

        #region ISvgImageElement Members from SVG 1.2

        public SvgDocument GetImageDocument()
        {
            SvgWindow window = this.SvgWindow;
            if (window == null)
            {
                return null;
            }
            else
            {
                return (SvgDocument)window.Document;
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                // This list may be too long to be useful...
                switch (attribute.LocalName)
                {
                    // Additional attributes
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

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return _externalResourcesRequired.ExternalResourcesRequired;
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
    }
}
