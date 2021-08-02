using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
using System.Diagnostics;
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
        private SvgExternalResourcesRequired _resourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgImageElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _resourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests          = new SvgTests(this);
            _uriReference      = new SvgUriReference(this);
            _fitToViewBox      = new SvgFitToViewBox(this);
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
            get {
                if (Href == null || Href.AnimVal == null)
                {
                    return false;
                }

                if (Href.AnimVal.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                try
                {
                    string absoluteUri = _uriReference.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(absoluteUri))
                    {
                        return false;
                    }

                    if (absoluteUri.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        // image elements can't reference elements in an svg file
                        Trace.WriteLine("Image elements cannot reference elements in an svg file. Uri: " + absoluteUri);
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(absoluteUri))
                    {
                        Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                        if (svgUri.IsFile)
                        {
                            return absoluteUri.EndsWith(SvgConstants.FileExt, StringComparison.OrdinalIgnoreCase) ||
                                absoluteUri.EndsWith(SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase);
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
                        name.EndsWith(SvgConstants.FileExt, StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase));
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
        }

        public SvgWindow SvgWindow
        {
            get {
                if (this.IsSvgImage)
                {
                    SvgWindow parentWindow = (SvgWindow)OwnerDocument.Window;

                    if (parentWindow != null)
                    {
                        var baseUrls = parentWindow.BaseUrls;

                        string absoluteUri = _uriReference.AbsoluteUri;
                        Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);
                        if (baseUrls != null && svgUri.IsAbsoluteUri)
                        {
                            var temp = svgUri.ToString();
                            if (baseUrls.Contains(svgUri.AbsoluteUri) || baseUrls.Contains(svgUri.ToString()))
                            {
                                return null;
                            }
                            if (svgUri.IsFile && baseUrls.Contains(svgUri.LocalPath))
                            {
                                return null;
                            }
                        }

                        SvgWindow wnd = parentWindow.CreateOwnedWindow(
                            (long)Width.AnimVal.Value, (long)Height.AnimVal.Value);

                        SvgDocument doc = new SvgDocument(wnd);
                        doc.Static = true;
                        wnd.Document = doc;

                        if (svgUri.IsFile)
                        {
                            if (!File.Exists(svgUri.LocalPath))
                            {
                                Trace.TraceError("Image file does not exist; " + svgUri.LocalPath);
                                return null;
                            }
                            string fileExt = Path.GetExtension(svgUri.LocalPath);
                            if (string.Equals(fileExt, SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase))
                            {
                                using (var fileStream = File.OpenRead(svgUri.LocalPath))
                                {
                                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                                    {
                                        doc.Load(absoluteUri, zipStream);
                                    }
                                }
                            }
                            else
                            {
                                using (Stream resStream = File.OpenRead(svgUri.LocalPath))
                                {
                                    doc.Load(absoluteUri, resStream);
                                }
                            }

                            baseUrls.Add(svgUri.ToString());
                        }
                        else
                        {
                            using (Stream resStream = _uriReference.ReferencedResource.GetResponseStream())
                            {
                                doc.Load(absoluteUri, resStream);
                            }

                            baseUrls.Add(absoluteUri);
                        }
                        return wnd;
                    }
                }

                return null;
            }
        }

        public bool IsRootReferenced(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri) || _uriReference == null)
            {
                return false;
            }
            try
            {
                string absoluteUri = _uriReference.AbsoluteUri;
                if (string.IsNullOrWhiteSpace(absoluteUri))
                {
                    return false;
                }

                var uriRoot = new Uri(baseUri, UriKind.Absolute);
                Uri svgUri = new Uri(absoluteUri, UriKind.Absolute);

                return svgUri.Equals(uriRoot);
            }
            catch (Exception)
            {
                return false;
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
            get {
                return SvgRenderingHint.Image;
            }
        }

        #endregion

        #region ISvgImageElement Members

        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, SvgConstants.ValZero);
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, SvgConstants.ValZero);
                }
                return _height;
            }

        }

        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, SvgConstants.ValZero);
                }
                return _x;
            }

        }

        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, SvgConstants.ValZero);
                }
                return _y;
            }

        }

        public ISvgColorProfileElement ColorProfile
        {
            get {
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
            get {
                return _uriReference.Href;
            }
        }

        public SvgUriReference UriReference
        {
            get {
                return _uriReference;
            }
        }

        public XmlElement ReferencedElement
        {
            get {
                if (_uriReference == null)
                {
                    return null;
                }
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get {
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

        public void Accept(ISvgElementVisitor visitor)
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
            get {
                return _resourcesRequired.ExternalResourcesRequired;
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
