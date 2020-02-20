using System;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Xml;
using SharpVectors.Woffs;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Resources;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The root object in the document object hierarchy of an Svg document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an 'svg' element is embedded inline as a component of a document from another namespace, 
    /// such as when an 'svg' element is embedded inline within an XHTML document
    /// [<see href="http://www.w3.org/TR/SVG/refs.html#ref-XHTML">XHTML</see>], then an 
    /// <see cref="ISvgDocument">ISvgDocument</see> object will not exist; instead, the root object in the 
    /// document object hierarchy will be a Document object of a different type, such as an HTMLDocument object.
    /// </para>
    /// <para>
    /// However, an <see cref="ISvgDocument">ISvgDocument</see> object will indeed exist when the root
    /// element of the XML document hierarchy is an 'svg' element, such as when viewing a stand-alone SVG 
    /// file (i.e., a file with MIME type "image/svg+xml"). In this case, the <see cref="ISvgDocument">ISvgDocument</see> 
    /// object will be the root object of the document object model hierarchy.
    /// </para>
    /// <para>
    /// In the case where an SVG document is embedded by reference, such as when an XHTML document has an 'object' 
    /// element whose href attribute references an SVG document (i.e., a document whose MIME type is
    /// "image/svg+xml" and whose root element is thus an 'svg' element), there will exist two distinct DOM hierarchies. 
    /// The first DOM hierarchy will be for the referencing document (e.g., an XHTML document). 
    /// The second DOM hierarchy will be for the referenced SVG document. In this
    /// second DOM hierarchy, the root object of the document object model
    /// hierarchy is an <see cref="ISvgDocument">ISvgDocument</see> object.
    /// </para>
    /// <para>
    /// The <see cref="ISvgDocument">ISvgDocument</see> interface contains a similar list of attributes and
    /// methods to the HTMLDocument interface described in the
    /// <see href="http://www.w3.org/TR/REC-DOM-Level-1/level-one-html.html">Document
    /// Object Model (HTML) Level 1</see> chapter of the
    /// [<see href="http://www.w3.org/TR/SVG/refs.html#ref-DOM1">DOM1</see>] specification.
    /// </para>
    /// </remarks>
    public class SvgDocument : CssXmlDocument, ISvgDocument
    {
        #region Public Static Fields

        public static readonly int DotsPerInch = 96;

        public const string SvgNamespace   = "http://www.w3.org/2000/svg";
        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";
        /// <summary>
        /// Namespace URI to map to the xml prefix
        /// </summary>
        public const string XmlIdUrl = "http://www.w3.org/XML/1998/namespace";

        #endregion

        #region Private Static Fields

        // Resource handling

        /// <summary>
        /// Entities URIs corrections are cached here.
        /// Currently consists in mapping '_' to '' (nothing)
        /// </summary>
        private static IDictionary<string, string> _entitiesUris;

        /// <summary>
        /// Semaphore to access _entitiesUris
        /// </summary>
        private static readonly object _entitiesUrisLock = new object();

        /// <summary>
        /// Root where resources are embedded
        /// </summary>
        private static readonly Type _rootType = typeof(Root);

        private readonly string[] _supportedFeatures = {
            "org.w3c.svg.static",
            "http://www.w3.org/TR/Svg11/feature#Shape",
            "http://www.w3.org/TR/Svg11/feature#BasicText",
            "http://www.w3.org/TR/Svg11/feature#OpacityAttribute"
        };

        private readonly string[] _supportedExtensions = { };

        #endregion

        #region Private Fields

        private readonly bool _ignoreComments;
        private readonly bool _ignoreProcessingInstructions;
        private readonly bool _ignoreWhitespace;

        private bool _isFontsLoaded;
        private SvgWindow _window;

        private XmlReaderSettings _settings;

        private IList<SvgFontElement> _svgFonts;
        private ISet<string> _svgFontFamilies;

        private IList<SvgFontFamily> _fontFamilies;

        private double _dpi;

        private XmlNamespaceManager _namespaceManager;

        private IDictionary<string, string> _styledFontIds;
        private IDictionary<string, XmlElement> _xmlElementMap;
        private IDictionary<string, SvgElement> _svgElementMap;

        #endregion

        #region Constructors and Destructor

        private SvgDocument()
        {
            _isFontsLoaded                = true;
            _ignoreComments               = true;
            _ignoreWhitespace             = false;
            _ignoreProcessingInstructions = false;

            _dpi                          = DotsPerInch;

            this.PreserveWhitespace       = true;

            _styledFontIds                = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public SvgDocument(SvgWindow window)
            : this()
        {
            _window = window;
            _window.Document = this;

            // set up CSS properties
            this.AddStyleElement(SvgDocument.SvgNamespace, "style");
            this.CssPropertyProfile = CssPropertyProfile.SvgProfile;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Namespace resolution event delegate.
        /// </summary>
        public delegate void ResolveNamespaceDelegate(object sender, SvgResolveNamespaceEventArgs e);

        /// <summary>
        /// URI resolution event delegate
        /// </summary>
        public delegate void ResolveUriDelegate(object sender, SvgResolveUriEventArgs e);

        /// <summary>
        /// Occurs when a namespace is being resolved.
        /// </summary>
        public event ResolveNamespaceDelegate ResolveNamespace;

        /// <summary>
        /// Occurs when an URI is resolved (always).
        /// </summary>
        public event ResolveUriDelegate ResolvingUri;

        #endregion

        #region Public Properties

        public bool IsFontsLoaded
        {
            get {
                return _isFontsLoaded;
            }
        }

        public IList<SvgFontFamily> FontFamilies
        {
            get {
                return _fontFamilies;
            }
        }

        public XmlReaderSettings CustomSettings
        {
            get {
                return _settings;
            }
            set {
                _settings = value;
            }
        }

        /// <summary>
        /// Get or sets the dots per inch at which the objects should be rendered.
        /// </summary>
        /// <value>The current dots per inch value.</value>
        public double Dpi
        {
            get {
                return _dpi;
            }
            set {
                _dpi = value;
            }
        }

        public XmlNamespaceManager NamespaceManager
        {
            get {
                if (_namespaceManager == null)
                {
                    // Setup namespace manager and add default namespaces
                    _namespaceManager = new XmlNamespaceManager(this.NameTable);
                    _namespaceManager.AddNamespace(string.Empty, SvgDocument.SvgNamespace);
                    _namespaceManager.AddNamespace("svg", SvgDocument.SvgNamespace);
                    _namespaceManager.AddNamespace("xlink", SvgDocument.XLinkNamespace);
                    _namespaceManager.AddNamespace("xml", SvgDocument.XmlIdUrl);
                }

                return _namespaceManager;
            }
        }

        #endregion

        #region Type handling and creation of elements

        public override XmlElement CreateElement(string prefix, string localName, string ns)
        {
            XmlElement result = SvgElementFactory.Create(prefix, localName, ns, this);
            if (result != null)
            {
                return result;
            }

            if (string.Equals(ns, SvgNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return new SvgElement(prefix, localName, ns, this);
            }
            // Now, if the ns is empty, we try creating with the default namespace for cases
            // where the node is imported from an external SVG document...
            if (string.IsNullOrWhiteSpace(ns))
            {
                result = SvgElementFactory.Create(prefix, localName, SvgNamespace, this);
                if (result != null)
                {
                    return result;
                }
            }

            return base.CreateElement(prefix, localName, ns);
        }

        #endregion

        #region Support collections

        public override bool Supports(string feature, string version)
        {
            foreach (string supportedFeature in _supportedFeatures)
            {
                if (supportedFeature == feature)
                {
                    return true;
                }
            }

            foreach (string supportedExtension in _supportedExtensions)
            {
                if (supportedExtension == feature)
                {
                    return true;
                }
            }

            return base.Supports(feature, version);
        }

        #endregion

        #region Overrides of Load

        private XmlReaderSettings GetXmlReaderSettings()
        {
            if (_settings != null)
            {
                return _settings;
            }
            DynamicXmlUrlResolver xmlResolver = new DynamicXmlUrlResolver();
            xmlResolver.Resolving += OnXmlResolverResolving;
            xmlResolver.GettingEntity += OnXmlResolverGettingEntity;

            XmlReaderSettings settings = new XmlReaderSettings();

            settings.DtdProcessing = DtdProcessing.Parse;

            settings.IgnoreComments               = _ignoreComments;
            settings.IgnoreWhitespace             = _ignoreWhitespace;
            settings.IgnoreProcessingInstructions = _ignoreProcessingInstructions;
            settings.XmlResolver                  = xmlResolver;

            return settings;
        }

        /// <overloads>
        /// Loads an XML document.Loads the specified XML data.
        /// <blockquote>
        /// <b>Note</b>   The Load method always preserves significant white
        /// space. The PreserveWhitespace property determines whether or not
        /// white space is preserved. The default is false, whites space is
        /// not preserved.
        /// </blockquote>
        /// </overloads>
        /// <summary>
        /// Loads the XML document from the specified URL.
        /// </summary>
        /// <param name="filename">
        /// URL for the file containing the XML document to load.
        /// </param>
        public override void Load(string filename)
        {
            // Provide a support for the .svgz files...
            UriBuilder fileUrl = new UriBuilder(filename);
            if (string.Equals(fileUrl.Scheme, "file"))
            {
                string fileExt = Path.GetExtension(filename);
                if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream fileStream = File.OpenRead(fileUrl.Uri.LocalPath))
                    {
                        using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            this.Load(zipStream);
                        }
                    }
                    return;
                }
            }

            using (XmlReader reader = CreateValidatingXmlReader(filename))
            {
                this.Load(reader);
            }
        }

        /// <summary>
        /// Loads the XML document from the specified stream but with the
        /// specified base URL
        /// </summary>
        /// <param name="baseUrl">
        /// Base URL for the stream from which the XML document is loaded.
        /// </param>
        /// <param name="stream">
        /// The stream containing the XML document to load.
        /// </param>
        public void Load(string baseUrl, Stream stream)
        {
            using (XmlReader reader = CreateValidatingXmlReader(baseUrl, stream))
            {
                this.Load(reader);
            }
        }

        /// <summary>
        /// Loads the XML document from the specified
        /// <see cref="TextReader">TextReader</see>.
        /// </summary>
        /// <param name="txtReader"></param>
        public override void Load(TextReader txtReader)
        {
            using (XmlReader xmlReader = CreateValidatingXmlReader(txtReader))
            {
                this.Load(xmlReader);
            }
        }

        /// <summary>
        /// Loads the XML document from the specified stream.
        /// </summary>
        /// <param name="inStream">
        /// The stream containing the XML document to load.
        /// </param>
        public override void Load(Stream inStream)
        {
            using (XmlReader reader = CreateValidatingXmlReader(string.Empty, inStream))
            {
                this.Load(reader);
            }
        }

        #endregion

        #region Resource handling

        /// <summary>
        /// Given a transformed resource name, find a possible existing resource.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private static string GetEntityUri(string uri)
        {
            lock (_entitiesUrisLock)
            {
                if (_entitiesUris == null)
                {
                    _entitiesUris = new Dictionary<string, string>();
                    string[] names = _rootType.Assembly.GetManifestResourceNames();
                    foreach (string name in names)
                    {
                        if (name.StartsWith(_rootType.Namespace, StringComparison.OrdinalIgnoreCase))
                        {
                            string namePart = name.Substring(_rootType.Namespace.Length + 1); // the +1 is for the "."
                            _entitiesUris[namePart] = name;
                            _entitiesUris[namePart.Replace("_", "")] = name;
                        }
                    }
                }
                string entityUri;
                _entitiesUris.TryGetValue(uri, out entityUri);
                return entityUri;
            }
        }

        /// <summary>
        /// Handles DynamicXmlUrlResolver GettingEntity event.
        /// </summary>
        /// <param name="absoluteUri">The absolute URI.</param>
        /// <param name="ofObjectToReturn">The of object to return.</param>
        /// <returns></returns>
        private object OnXmlResolverGettingEntity(Uri absoluteUri, string cc, Type ofObjectToReturn)
        {
            string fullPath = absoluteUri.ToString();
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                fullPath = fullPath.Replace('\\', '/');

                bool useSvgDtd = false;

                if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Basic//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Basic/EN", StringComparison.OrdinalIgnoreCase))
                {
                    useSvgDtd = true;
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1/EN", StringComparison.OrdinalIgnoreCase))
                {
                    useSvgDtd = true;
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Full//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Full/EN", StringComparison.OrdinalIgnoreCase))
                {
                    useSvgDtd = true;
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.0//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.0/EN", StringComparison.OrdinalIgnoreCase))
                {
                    useSvgDtd = true;
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Tiny//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Tiny/EN", StringComparison.OrdinalIgnoreCase))
                {
                    useSvgDtd = true;
                }

                if (useSvgDtd)
                {
                    string resourceUri = GetEntityUri("svg11.dtd");
                    if (resourceUri != null)
                    {
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
                    }
                }
            }

            if (absoluteUri.IsFile)
            {
                return null;
            }

            string path = (absoluteUri.Host + absoluteUri.AbsolutePath.Replace('/', '.'));
            string foundResource = GetEntityUri(path);
            if (foundResource != null)
                return GetEntityFromUri(foundResource, ofObjectToReturn);

            return null;
        }

        /// <summary>
        /// Gets the URI direct.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="ofObjectToReturn">The of object to return.</param>
        /// <returns></returns>
        private static object GetEntityFromUri(string path, Type ofObjectToReturn)
        {
            if (ofObjectToReturn == typeof(Stream))
            {
                using (Stream resourceStream = _rootType.Assembly.GetManifestResourceStream(path))
                {
                    if (resourceStream != null)
                    {
                        // we copy the contents to a MemoryStream because the loader doesn't release original streams,
                        // resulting in an assembly lock
                        MemoryStream memoryStream = new MemoryStream();
                        resourceStream.CopyTo(memoryStream);

                        // Move the position to the start of the stream
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handles DynamicXmlResolver Resolve event.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns></returns>
        private string OnXmlResolverResolving(string relativeUri)
        {
            if (this.ResolvingUri != null)
            {
                SvgResolveUriEventArgs e = new SvgResolveUriEventArgs { Uri = relativeUri };
                ResolvingUri(this, e);
                relativeUri = e.Uri;
            }
            if (relativeUri.Equals("http://www.w3.org/2000/svg", StringComparison.OrdinalIgnoreCase))
            {
                relativeUri = "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd";
            }

            return relativeUri;
        }

        /// <summary>
        /// Gets/create an XML parser context, with predefined namespaces
        /// </summary>
        /// <returns></returns>
        private XmlParserContext GetXmlParserContext()
        {
            var xmlNamespaceManager = new DynamicXmlNamespaceManager(new NameTable());
            xmlNamespaceManager.Resolve += OnResolveXmlNamespaceManager;
            XmlParserContext xmlParserContext = new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);

            return xmlParserContext;
        }

        /// <summary>
        /// Handles DynamicXmlNamespaceManager Resolve event.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        private string OnResolveXmlNamespaceManager(string prefix)
        {
            string uri = null;
            if (this.ResolveNamespace != null)
            {
                 var evtArgs = new SvgResolveNamespaceEventArgs(prefix);
                ResolveNamespace(this, evtArgs);
                uri = evtArgs.Uri;
            }
            if (string.IsNullOrWhiteSpace(uri))
            {
                // some defaults added here
                switch (prefix)
                {
                    case "rdf":
                        uri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
                        break;
                    case "cc":
                        uri = "http://web.resource.org/cc";
                        break;
                    case "dc":
                        uri = "http://purl.org/dc/elements/1.1/";
                        break;
                    case "rdfs":
                        uri = "http://www.w3.org/2000/01/rdf-schema#";
                        break;
                    case "owl":
                        uri = "http://www.w3.org/2002/07/owl#";
                        break;
                    case "foaf":
                        uri = "http://xmlns.com/foaf/0.1/";
                        break;
                    case "xsd":
                        uri = "http://www.w3c.org/2001/XMLSchema#";
                        break;
                    case "xlink":
                        uri = "http://www.w3.org/1999/xlink";
                        break;
                }
            }
            return uri;
        }

        /// <summary>
        /// Creates the validating XML reader.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private XmlReader CreateValidatingXmlReader(string uri)
        {
            XmlReaderSettings xmlReaderSettings = GetXmlReaderSettings();
            XmlParserContext xmlParserContext = GetXmlParserContext();
            return XmlReader.Create(uri, xmlReaderSettings, xmlParserContext);
        }

        /// <summary>
        /// Creates the validating XML reader.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
        /// <returns></returns>
        private XmlReader CreateValidatingXmlReader(TextReader textReader)
        {
            XmlReaderSettings xmlReaderSettings = GetXmlReaderSettings();
            XmlParserContext xmlParserContext = GetXmlParserContext();
            return XmlReader.Create(textReader, xmlReaderSettings, xmlParserContext);
        }

        /// <summary>
        /// Creates the validating XML reader.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private XmlReader CreateValidatingXmlReader(string uri, Stream stream)
        {
            //return new XmlTextReader(uri, stream);
            XmlReaderSettings xmlReaderSettings = GetXmlReaderSettings();
            if (string.IsNullOrWhiteSpace(uri))
            {
                return XmlReader.Create(stream, xmlReaderSettings, uri);
            }
            XmlParserContext xmlParserContext = GetXmlParserContext();
            return XmlReader.Create(stream, xmlReaderSettings, xmlParserContext);
        }

        /// <summary>
        /// Creates the validating XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <returns></returns>
        private XmlReader CreateValidatingXmlReader(XmlReader xmlReader)
        {
            // Note: we only have part of the job done here, we need to also use a parser context
            return XmlReader.Create(xmlReader, GetXmlReaderSettings());
        }

        public XmlNode GetNodeByUri(Uri absoluteUri)
        {
            return GetNodeByUri(absoluteUri.AbsoluteUri);
        }

        public XmlNode GetNodeByUri(string absoluteUrl)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
            {
                return null;
            }

            absoluteUrl = absoluteUrl.Trim().Trim(new char[] { '\"', '\'' });
            if (absoluteUrl.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                return GetElementById(absoluteUrl.Substring(1));
            }

            Uri docUri = ResolveUri("");
            Uri absoluteUri = new Uri(absoluteUrl);

            if (absoluteUri.IsFile)
            {
                string localFile = absoluteUri.LocalPath;
                if (File.Exists(localFile) == false)
                {
                    Trace.TraceError("GetNodeByUri: Locally referenced file not found: " + localFile);
                    return null;
                }
                string fileExt = Path.GetExtension(localFile);
                if (!string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    Trace.TraceError("GetNodeByUri: Locally referenced file not valid: " + localFile);
                    return null;
                }
            }

            if (string.Equals(absoluteUri.Scheme, "data", StringComparison.OrdinalIgnoreCase))
            {
                Trace.TraceError("GetNodeByUri: The Uri Scheme is 'data' is not a valid XmlDode " + absoluteUri);
                return null;
            }

            string fragment = absoluteUri.Fragment;

            if (fragment.Length == 0)
            {
                // no fragment => return entire document
                if (docUri != null && string.Equals(docUri.AbsolutePath, 
                    absoluteUri.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                {
                    return this;
                }

                SvgDocument doc = new SvgDocument((SvgWindow)Window);

                XmlReaderSettings settings = this.GetXmlReaderSettings();

                settings.CloseInput = true;

                //PrepareXmlResolver(settings);

                using (XmlReader reader = XmlReader.Create(GetResource(absoluteUri).GetResponseStream(), 
                    settings, absoluteUri.AbsolutePath))
                {
                    doc.Load(reader);
                }

                return doc;
            }
            else
            {
                // got a fragment => return XmlElement
                string noFragment = absoluteUri.AbsoluteUri.Replace(fragment, "");
                SvgDocument doc = (SvgDocument)GetNodeByUri(new Uri(noFragment));
                return doc.GetElementById(fragment.Substring(1));
            }
        }

        public Uri ResolveUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return null;
            }
            Uri sourceUri = new Uri(uri, UriKind.RelativeOrAbsolute);
            if (sourceUri.IsAbsoluteUri)
            {
                return sourceUri;
            }

            string baseUri = this.BaseURI;
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                DirectoryInfo workingDir = _window.WorkingDir;

                baseUri = "file:///" + workingDir.FullName.Replace('\\', '/');
            }

            return new Uri(new Uri(baseUri), uri);
        }

        #endregion

        #region ISvgDocument Members

        /// <summary>
        /// The title of the document which is the text content of the first child title 
        /// element of the 'svg' root element.
        /// </summary>
        public string Title
        {
            get {
                string result = string.Empty;

                XmlNode node = this.SelectSingleNode("/svg:svg/svg:title[text()!='']", this.NamespaceManager);

                if (node != null)
                {
                    node.Normalize();
                    // NOTE: should probably use spec-defined whitespace
                    result = Regex.Replace(node.InnerText, @"\s\s+", " ");
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the URI of the page that linked to this page. The value is an empty 
        /// string if the user navigated to the page directly (not through a link, but, 
        /// for example, via a bookmark).
        /// </summary>
        public string Referrer
        {
            get {
                return string.Empty;
            }
        }

        /// <summary>
        /// The domain name of the server that served the document, or a null string if the 
        /// server cannot be identified by a domain name.
        /// </summary>
        public string Domain
        {
            get {
                if (Url.Length == 0 ||
                    Url.StartsWith(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                return new Uri(Url).Host;
            }
        }

        /// <summary>
        /// The root 'svg' element in the document hierarchy
        /// </summary>
        public ISvgSvgElement RootElement
        {
            get {
                return this.DocumentElement as ISvgSvgElement;
            }
        }

        public SvgElement GetSvgById(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return null;
            }
            return this.GetElementById(elementId) as SvgElement;
        }

        public SvgElement GetSvgByUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty)
            {
                return null;
            }
            return this.GetSvgByUniqueId(uniqueId.ToString());
        }

        public SvgElement GetSvgByUniqueId(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                return null;
            }
            if (_svgElementMap == null)
            {
                this.BuildElementUniqueMap();
            }

            // Find the item
            if (_svgElementMap.ContainsKey(uniqueId))
            {
                return _svgElementMap[uniqueId];
            }

            return null;
        }

        public override XmlElement GetElementById(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return null;
            }

            // TODO: handle element and attribute updates globally to watch for id changes.
            if (_xmlElementMap == null)
            {
                this.BuildElementMap();
            }

            // Find the item
            if (_xmlElementMap.ContainsKey(elementId))
            {
                return _xmlElementMap[elementId];
            }

            return null;
        }

        #endregion

        #region Other Public Properties and Methods

        //ISvgDocument Members from SVG 1.2
        /// <summary>
        /// 
        /// </summary>
        public ISvgWindow Window
        {
            get {
                return _window;
            }
        }

        public IList<SvgFontElement> SvgFonts
        {
            get {
                if (_svgFonts == null)
                {
                    _svgFonts = new List<SvgFontElement>();
                }
                return _svgFonts;
            }
        }

        public ISet<string> SvgFontFamilies
        {
            get {

                if (_svgFontFamilies == null && (_svgFonts != null && _svgFonts.Count != 0))
                {
                    _svgFontFamilies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var svgFont in _svgFonts)
                    {
                        _svgFontFamilies.Add(svgFont.FontFamily);
                    }
                }
                return _svgFontFamilies;
            }
        }

        public new SvgDocument OwnerDocument
        {
            get {
                return base.OwnerDocument as SvgDocument;
            }
        }

        public void RegisterFont(SvgFontElement svgFont)
        {
            if (svgFont == null)
            {
                return;
            }
            if (_svgFonts == null)
            {
                _svgFonts = new List<SvgFontElement>();
            }
            _svgFonts.Add(svgFont);
        }

        public IList<SvgFontElement> GetFonts(string fontFamily)
        {
            List<SvgFontElement> fontList = new List<SvgFontElement>();
            if (string.IsNullOrWhiteSpace(fontFamily) || (_svgFonts == null || _svgFonts.Count == 0))
            {
                return fontList;
            }
            foreach (var svgFont in _svgFonts)
            {
                if (string.Equals(svgFont.FontFamily, fontFamily, StringComparison.OrdinalIgnoreCase))
                {
                    fontList.Add(svgFont);
                }
            }

            return fontList;
        }
        public IList<SvgFontElement> GetFonts(IList<string> fontFamilies)
        {
            if (fontFamilies != null && fontFamilies.Count == 1)
            {
                return this.GetFonts(fontFamilies[0]);
            }
            List<SvgFontElement> fontList = new List<SvgFontElement>();
            if ((fontFamilies == null || fontFamilies.Count == 0) ||
                (_svgFonts == null || _svgFonts.Count == 0))
            {
                return fontList;
            }
            foreach (var svgFont in _svgFonts)
            {
                foreach (var fontFamily in fontFamilies)
                {
                    if (string.Equals(svgFont.FontFamily, fontFamily, StringComparison.OrdinalIgnoreCase))
                    {
                        fontList.Add(svgFont);
                        break;
                    }
                }
            }

            return fontList;
        }

        public IDictionary<string, string> StyledFontIds
        {
            get {
                return _styledFontIds;
            }
        }

        public IDictionary<string, XmlElement> ElementMap
        {
            get {
                if (_xmlElementMap == null)
                {
                    this.BuildElementMap();
                }
                return _xmlElementMap;
            }
        }

        public IDictionary<string, SvgElement> ElementUniqueMap
        {
            get {
                if (_svgElementMap == null)
                {
                    this.BuildElementUniqueMap();
                }
                return _svgElementMap;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual IList<Tuple<string, SvgFontFaceElement>> GetFontUrls()
        {
            List<Tuple<string, SvgFontFaceElement>> fontUrls = new List<Tuple<string, SvgFontFaceElement>>();
            if (this.IsLoaded == false)
            {
                return fontUrls;
            }

            XmlNodeList xmlList = this.SelectNodes(".//svg:font-face-uri", this.NamespaceManager);
            if (xmlList != null && xmlList.Count != 0)
            {
                foreach (XmlElement xmlNode in xmlList)
                {
                    SvgFontFaceElement fontFace = null;
                    var fontSource = xmlNode.ParentNode as SvgFontFaceSrcElement;
                    if (fontSource != null)
                    {
                        fontFace = fontSource.ParentNode as SvgFontFaceElement;
                    }

                    if (xmlNode.HasAttribute("href", SvgDocument.XLinkNamespace))
                    {
                        string fontUrl = xmlNode.GetAttribute("href", SvgDocument.XLinkNamespace);
                        if (!string.IsNullOrWhiteSpace(fontUrl))
                        {
                            fontUrls.Add(new Tuple<string, SvgFontFaceElement>(fontUrl, fontFace));
                        }
                    }
                    else if (xmlNode.HasAttribute("href"))
                    {
                        string fontUrl = xmlNode.GetAttribute("href");
                        if (!string.IsNullOrWhiteSpace(fontUrl))
                        {
                            fontUrls.Add(new Tuple<string, SvgFontFaceElement>(fontUrl, fontFace));
                        }
                    }
                }
            }

            IStyleSheetList styleSheets = this.StyleSheets;
            if (styleSheets != null && styleSheets.Count != 0)
            {
                foreach (var styleSheet in styleSheets)
                {
                    var cssSheet = styleSheet as CssStyleSheet;
                    if (cssSheet == null)
                    {
                        continue;
                    }

                    GetFontUrl(cssSheet, fontUrls, _styledFontIds);
                }
            }

            GetFontUrl(this.UserAgentStyleSheet, fontUrls, _styledFontIds);
            GetFontUrl(this.UserStyleSheet, fontUrls, _styledFontIds);

            return fontUrls;
        }

        private static void GetFontUrl(CssStyleSheet cssSheet, IList<Tuple<string, SvgFontFaceElement>> fontUrls, 
            IDictionary<string, string> styledFontIds)
        {
            if (cssSheet == null || fontUrls == null)
            {
                return;
            }

            var ruleList = cssSheet.CssRules;
            if (ruleList == null || ruleList.Length == 0 || !ruleList.HasFontRule)
            {
                return;
            }

            string fontFileDir = Path.GetTempPath();
            if (!Directory.Exists(fontFileDir))
            {
                fontFileDir = null;
            }
            else
            {
                fontFileDir = Path.Combine(fontFileDir, SvgWoffParser.DirectoryName);
                if (!Directory.Exists(fontFileDir))
                {
                    Directory.CreateDirectory(fontFileDir);
                }
            }

            foreach (var rule in ruleList)
            {
                if (rule.Type != CssRuleType.FontFaceRule)
                {
                    continue;
                }

                var fontRule = (CssFontFaceRule)rule;
                if (fontRule.IsEmbedded)
                {
                    if (string.IsNullOrWhiteSpace(fontFileDir))
                    {
                        continue;
                    }

                    string fontFamily = fontRule.FontFamily;
                    string fontEncoding = fontRule.EmbeddedEncoding;
                    string fontMimeType = fontRule.EmbeddedMimeType;
                    if (!string.IsNullOrWhiteSpace(fontFamily) 
                        && !string.IsNullOrWhiteSpace(fontFamily)
                        && !string.IsNullOrWhiteSpace(fontMimeType)
                        && !fontMimeType.Equals("base64", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileExt = null;
                        switch (fontMimeType)
                        {
                            case "image/svg+xml": fileExt = ".svg";               // (W3C: August 2011)
                                break;
                            case "application/x-font-ttf": 
                                fileExt = ".ttf";       // (IANA: March 2013)
                                break;
                            case "application/x-font-truetype": 
                            case "application/font-truetype": 
                                fileExt = ".ttf";
                                break;
                            case "application/x-font-opentype": 
                            case "application/font-opentype": 
                                fileExt = ".otf";   // (IANA: March 2013)
                                break;
                            case "application/font-woff": 
                            case "application/x-font-woff": 
                            case "font/woff": 
                                fileExt = ".woff";        //  (IANA: January 2013)
                                break;
                            case "application/font-woff2": 
                            case "application/x-font-woff2": 
                            case "font/woff2": 
                                fileExt = ".woff2";      //   (W3C W./E.Draft: May 2014/March 2016)
                                break;
                            case "application/vnd.ms-fontobject": fileExt = ".eot";  // (IA;NA: December 2005)
                                break;
                            case "application/font-sfnt": 
                            case "application/x-font-sfnt": 
                                fileExt = ".sfnt";         //  (IANA: March 2013) 
                                break;
                        }

                        if (!string.IsNullOrWhiteSpace(fileExt))
                        {
                            string fontPath = Path.Combine(fontFileDir, fontFamily + fileExt);
                            if (!File.Exists(fontPath))
                            {
                                string fontData   = fontRule.EmbeddedData;
                                byte[] imageBytes = Convert.FromBase64CharArray(fontData.ToCharArray(),
                                    0, fontData.Length);
                                using (var stream = File.OpenWrite(fontPath))
                                {
                                    stream.Write(imageBytes, 0, imageBytes.Length);
                                }
                            }
                            fontUrls.Add(new Tuple<string, SvgFontFaceElement>(fontPath, null));
                        }
                    }

                    continue;
                }
                string fontUrl = fontRule.FontUrl;
                if (!string.IsNullOrWhiteSpace(fontUrl))
                {
                    fontUrl = fontUrl.Trim();
                    if (!fontUrl.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        fontUrls.Add(new Tuple<string, SvgFontFaceElement>(fontUrl, null));
                    }
                    else if (styledFontIds != null)
                    {
                        string fontFamily = fontRule.FontFamily;
                        if (!string.IsNullOrWhiteSpace(fontFamily))
                        {
                            styledFontIds[fontFamily] = fontUrl.TrimStart('#').Trim('\'');
                        }
                    }
                }
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (_window == null || _window.LoadFonts == false)
            {
                return;
            }

            IList<Tuple<string, SvgFontFaceElement>> fontUrls = this.GetFontUrls();

            if (fontUrls == null || fontUrls.Count == 0)
            {
                return;
            }

            _isFontsLoaded = false;

            //TODO: Trying a background run...
            var loadTask = Task.Factory.StartNew(() => {
                SvgWindow ownedWindow = _window.CreateOwnedWindow();
                ownedWindow.LoadFonts = false;

                for (int i = 0; i < fontUrls.Count; i++)
                {
                    var fontUrl  = fontUrls[i].Item1;
                    var fontFace = fontUrls[i].Item2;
                    try
                    {
                        // remove any hash (won't work for local files)
                        int hashStart = fontUrl.IndexOf("#", StringComparison.OrdinalIgnoreCase);
                        if (hashStart > -1)
                        {
                            fontUrl = fontUrl.Substring(0, hashStart);
                        }

                        Uri fileUrl = this.ResolveUri(fontUrl);

                        if (fileUrl == null || fileUrl.IsAbsoluteUri == false)
                        {
                            continue;
                        }
                        string scheme = fileUrl.Scheme;
                        if (string.Equals(scheme, "file", StringComparison.OrdinalIgnoreCase))
                        {
                            this.LoadLocalFont(fileUrl.LocalPath, ownedWindow, fontFace);
                        }
                        else
                        {
                            throw new NotSupportedException("Loading fonts from a remote source is not supported.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                        //Ignore the exception
                    }
                }

                _isFontsLoaded = true;
            });

            _window.AddTask("SvgDocument", loadTask);
        }

        private void LoadLocalFont(string fontPath, SvgWindow ownedWindow, SvgFontFaceElement fontFace)
        {
            if (string.IsNullOrWhiteSpace(fontPath) || !File.Exists(fontPath))
            {
//                Trace.WriteLine("Private font not found: " + fontPath);
                return;
            }

            string fileExt = Path.GetExtension(fontPath);
            if (string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                SvgDocument document = new SvgDocument(ownedWindow);

                document.Load(fontPath);
                var svgFonts = document.SvgFonts;

                if (svgFonts != null && svgFonts.Count != 0)
                {
                    foreach (var svgFont in svgFonts)
                    {
                        if (fontFace != null && fontFace.HasAttribute("unicode-range"))
                        {
                            svgFont.SetAttribute("unicode-range", fontFace.GetAttribute("unicode-range"));
                        }

                        var fontNode = this.ImportNode(svgFont, true);
                        this.DocumentElement.AppendChild(fontNode);

//                        this.SvgFonts.Add(svgFont);
                    }
                }
            }
            else if (string.Equals(fileExt, ".woff", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileExt, ".woff2", StringComparison.OrdinalIgnoreCase))
            {
                if (_fontFamilies == null)
                {
                    _fontFamilies = new List<SvgFontFamily>();
                }

                var woffParser = new SvgWoffParser();
                if (woffParser.Import(fontPath))
                {
                    string fontExportPath = woffParser.DefaultExportPath;
                    if (File.Exists(fontExportPath))
                    {
                        var fontFamily = new SvgFontFamily(true, fontExportPath, fontPath);
                        _fontFamilies.Add(fontFamily);
                    }
                    else
                    {
                        fontExportPath = woffParser.GetExportPath();
                        if (woffParser.Export(fontExportPath))
                        {
                            var fontFamily = new SvgFontFamily(true, fontExportPath, fontPath);

                            _fontFamilies.Add(fontFamily);
                        }
                    }
                }
            }
            else if (string.Equals(fileExt, ".ttf", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileExt, ".otf", StringComparison.OrdinalIgnoreCase))
            {
                if (_fontFamilies == null)
                {
                    _fontFamilies = new List<SvgFontFamily>();
                }
                var fontFamily = new SvgFontFamily(false, fontPath);

                _fontFamilies.Add(fontFamily);
            }
            else if (string.Equals(fileExt, ".ttc", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileExt, ".otc", StringComparison.OrdinalIgnoreCase))
            {
                if (_fontFamilies == null)
                {
                    _fontFamilies = new List<SvgFontFamily>();
                }
                var fontFamily = new SvgFontFamily(false, fontPath);

                _fontFamilies.Add(fontFamily);
            }
        }

        #endregion

        #region Private Methods

        private void BuildElementMap()
        {
            _xmlElementMap = new Dictionary<string, XmlElement>(StringComparer.Ordinal);

            XmlNodeList ids = this.SelectNodes("//*/@id");
            foreach (XmlAttribute node in ids)
            {
                string valueKey = node.Value;
                if (!_xmlElementMap.ContainsKey(valueKey))
                {
                    _xmlElementMap.Add(node.Value, node.OwnerElement);
                }
            }

            // Get the nodes that have xml:ids which mach the given id
            ids = this.SelectNodes("//*/@xml:id", this.NamespaceManager);
            foreach (XmlAttribute node in ids)
            {
                string valueKey = node.Value;
                if (string.Equals(valueKey, "svg-root"))
                {
                    continue;
                }
                if (!_xmlElementMap.ContainsKey(valueKey))
                {
                    _xmlElementMap.Add(node.Value, node.OwnerElement);
                }
            }
        }

        private void BuildElementUniqueMap()
        {
            _svgElementMap = new Dictionary<string, SvgElement>(StringComparer.Ordinal);

            XmlNodeList ids = this.SelectNodes("//*/@uniqueId");
            foreach (XmlAttribute node in ids)
            {
                string valueKey = node.Value;
                if (!_svgElementMap.ContainsKey(valueKey))
                {
                    var svgElement = node.OwnerElement as SvgElement;
                    if (svgElement != null)
                    {
                        _svgElementMap.Add(node.Value, svgElement);
                    }
                }
            }
        }

        #endregion
    }
}
