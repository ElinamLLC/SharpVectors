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

        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private SvgTests svgTests;
        private SvgUriReference svgURIReference;
        private SvgFitToViewBox svgFitToViewBox;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgImageElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
            svgURIReference = new SvgUriReference(this);
            svgFitToViewBox = new SvgFitToViewBox(this);
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
                if (!Href.AnimVal.StartsWith("data:"))
                {
                    try
                    {
                        string absoluteUri = svgURIReference.AbsoluteUri;
                        if (!String.IsNullOrEmpty(absoluteUri))
                        {
                            Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                            if (svgUri.IsFile)
                            {   
                                return absoluteUri.EndsWith(".svg", 
                                    StringComparison.OrdinalIgnoreCase) || 
                                    absoluteUri.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        WebResponse resource = svgURIReference.ReferencedResource;
                        if (resource == null)
                        {
                            return false;
                        }

                        // local files are returning as binary/octet-stream
                        // this "fix" tests the file extension for .svg and .svgz
                        string name = resource.ResponseUri.ToString().ToLower(CultureInfo.InvariantCulture);
                        return (resource.ContentType.StartsWith("image/svg+xml") ||
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

                        string absoluteUri = svgURIReference.AbsoluteUri;

                        Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                        if (svgUri.IsFile)
                        {
                            Stream resStream = File.OpenRead(svgUri.LocalPath);
                            doc.Load(absoluteUri, resStream);
                        }
                        else
                        {
                            Stream resStream = svgURIReference.ReferencedResource.GetResponseStream();
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
                if (width == null)
                {
                    width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
                }
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null)
                {
                    height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
                }
                return height;
            }

        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (x == null)
                {
                    x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                }
                return x;
            }

        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (y == null)
                {
                    y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return y;
            }

        }

        public ISvgColorProfileElement ColorProfile
        {
            get
            {
                string colorProfile = this.GetAttribute("color-profile");

                if (String.IsNullOrEmpty(colorProfile))
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
                            if (elementNode != null && String.Equals(colorProfile, 
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
                return svgURIReference.Href;
            }
        }

        public SvgUriReference UriReference
        {
            get
            {
                return svgURIReference;
            }
        }

        public XmlElement ReferencedElement
        {
            get
            {
                return svgURIReference.ReferencedNode as XmlElement;
            }
        }

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get
            {
                return svgFitToViewBox.PreserveAspectRatio;
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
                        x = null;
                        return;
                    case "y":
                        y = null;
                        return;
                    case "width":
                        width = null;
                        return;
                    case "height":
                        height = null;
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
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion
    }
}
