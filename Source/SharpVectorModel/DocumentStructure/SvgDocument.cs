// Define this to enable the dispatching of load events.  The implementation
// of load events requires that a complete implementation of SvgDocument.Load
// be supplied rather than relying on the base XmlDocument.Load behaviour.
// This is required because I know of no way to hook into the key stages of
// XML document creation in order to throw events at the right times during
// the load process.

using System;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Xml;
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
    /// When an 'svg'  element is embedded inline as a component of a
    /// document from another namespace, such as when an 'svg' element is
    /// embedded inline within an XHTML document
    /// [<see href="http://www.w3.org/TR/SVG/refs.html#ref-XHTML">XHTML</see>],
    /// then an
    /// <see cref="ISvgDocument">ISvgDocument</see> object will not exist;
    /// instead, the root object in the
    /// document object hierarchy will be a Document object of a different
    /// type, such as an HTMLDocument object.
    /// </para>
    /// <para>
    /// However, an <see cref="ISvgDocument">ISvgDocument</see> object will
    /// indeed exist when the root
    /// element of the XML document hierarchy is an 'svg' element, such as
    /// when viewing a stand-alone SVG file (i.e., a file with MIME type
    /// "image/svg+xml"). In this case, the
    /// <see cref="ISvgDocument">ISvgDocument</see> object will be the
    /// root object of the document object model hierarchy.
    /// </para>
    /// <para>
    /// In the case where an SVG document is embedded by reference, such as
    /// when an XHTML document has an 'object' element whose href attribute
    /// references an SVG document (i.e., a document whose MIME type is
    /// "image/svg+xml" and whose root element is thus an 'svg' element),
    /// there will exist two distinct DOM hierarchies. The first DOM hierarchy
    /// will be for the referencing document (e.g., an XHTML document). The
    /// second DOM hierarchy will be for the referenced SVG document. In this
    /// second DOM hierarchy, the root object of the document object model
    /// hierarchy is an <see cref="ISvgDocument">ISvgDocument</see> object.
    /// </para>
    /// <para>
    /// The <see cref="ISvgDocument">ISvgDocument</see> interface contains a
    /// similar list of attributes and
    /// methods to the HTMLDocument interface described in the
    /// <see href="http://www.w3.org/TR/REC-DOM-Level-1/level-one-html.html">Document
    /// Object Model (HTML) Level 1</see> chapter of the
    /// [<see href="http://www.w3.org/TR/SVG/refs.html#ref-DOM1">DOM1</see>] specification.
    /// </para>
    /// </remarks>
    /// 
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

        private SvgWindow _window;

        private XmlReaderSettings _settings;

        private IList<SvgFontElement> _svgFonts;

        private double _dpi;

        private XmlNamespaceManager _namespaceManager;

        private IDictionary<string, XmlElement> _collectedIds;

        #endregion

        #region Constructors and Destructor

        private SvgDocument()
        {
            _ignoreComments               = false;
            _ignoreWhitespace             = false;
            _ignoreProcessingInstructions = false;

            _dpi                          = DotsPerInch;

            this.PreserveWhitespace       = true;
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

            if (ns == SvgNamespace)
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

        //private void PrepareXmlResolver(XmlReaderSettings settings)
        //{   
        //    /*// TODO: 1.2 has removed the DTD, can we do this safely?
        //    if (reader != null && reader is XmlValidatingReader)
        //    {
        //        XmlValidatingReader valReader = (XmlValidatingReader)reader;
        //        valReader.ValidationType = ValidationType.None;
        //    }
        //    return;

        //    LocalDtdXmlUrlResolver localDtdXmlUrlResolver = new LocalDtdXmlUrlResolver();
        //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd", @"dtd\svg10.dtd");
        //          localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/SVG/DTD/svg10.dtd", @"dtd\svg10.dtd");
        //          localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-tiny.dtd", @"dtd\svg11-tiny.dtd");
        //          localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-basic.dtd", @"dtd\svg11-basic.dtd");
        //          localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", @"dtd\svg11.dtd");

        //          if (reader != null && reader is XmlValidatingReader)
        //          {
        //              XmlValidatingReader valReader = (XmlValidatingReader)reader;

        //              valReader.ValidationType = ValidationType.None;
        //              valReader.XmlResolver = localDtdXmlUrlResolver;
        //          }

        //          this.XmlResolver = localDtdXmlUrlResolver;*/

        //    //LocalDtdXmlUrlResolver localDtdXmlUrlResolver = new LocalDtdXmlUrlResolver();
        //    //localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd", @"dtd\svg10.dtd");
        //    //localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/SVG/DTD/svg10.dtd", @"dtd\svg10.dtd");
        //    //localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-tiny.dtd", @"dtd\svg11-tiny.dtd");
        //    //localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-basic.dtd", @"dtd\svg11-basic.dtd");
        //    //localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", @"dtd\svg11.dtd");

        //    //string currentDir = Path.GetDirectoryName(
        //    //    System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    //string localDtd = Path.Combine(currentDir, "dtd");
        //    //if (Directory.Exists(localDtd))
        //    //{
        //    //    LocalDtdXmlUrlResolver localDtdXmlUrlResolver = new LocalDtdXmlUrlResolver();
        //    //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd", Path.Combine(localDtd, "svg10.dtd"));
        //    //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/TR/SVG/DTD/svg10.dtd", Path.Combine(localDtd, "svg10.dtd"));
        //    //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-tiny.dtd", Path.Combine(localDtd, "svg11-tiny.dtd"));
        //    //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11-basic.dtd", Path.Combine(localDtd, "svg11-basic.dtd"));
        //    //    localDtdXmlUrlResolver.AddDtd("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", Path.Combine(localDtd, "svg11.dtd"));

        //    //    settings.XmlResolver = localDtdXmlUrlResolver;			
        //    //}
        //    //else
        //    //{
        //    //    settings.XmlResolver = null;			
        //    //}
        //}

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

            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader reader = XmlReader.Create(url, settings))
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
            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader reader = XmlReader.Create(stream, settings, baseUrl))
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
            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader xmlReader = XmlReader.Create(reader, settings))
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
            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader reader = XmlReader.Create(stream, settings))
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

                if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Basic//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Basic/EN", StringComparison.OrdinalIgnoreCase))
                {
                    string resourceUri = GetEntityUri("www.w3.org.Graphics.SVG.1.1.DTD.svg11-basic.dtd");
                    if (resourceUri != null)
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1/EN", StringComparison.OrdinalIgnoreCase))
                {
                    string resourceUri = GetEntityUri("www.w3.org.Graphics.SVG.1.1.DTD.svg11.dtd");
                    if (resourceUri != null)
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Full//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Full/EN", StringComparison.OrdinalIgnoreCase))
                {
                    string resourceUri = GetEntityUri("www.w3.org.Graphics.SVG.1.1.DTD.svg11.dtd");
                    if (resourceUri != null)
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.0//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.0/EN", StringComparison.OrdinalIgnoreCase))
                {
                    string resourceUri = GetEntityUri("www.w3.org.TR.2001.REC-SVG-20010904.DTD.svg10.dtd");
                    if (resourceUri != null)
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
                }
                else if (fullPath.EndsWith("-//W3C//DTD SVG 1.1 Tiny//EN", StringComparison.OrdinalIgnoreCase) ||
                    fullPath.EndsWith("-/W3C/DTD SVG 1.1 Tiny/EN", StringComparison.OrdinalIgnoreCase))
                {
                    string resourceUri = GetEntityUri("www.w3.org.Graphics.SVG.1.1.DTD.svg11-tiny.dtd");
                    if (resourceUri != null)
                        return GetEntityFromUri(resourceUri, ofObjectToReturn);
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
                        int resourceLength = (int)resourceStream.Length;
                        MemoryStream memoryStream = new MemoryStream(resourceLength);
                        resourceStream.Read(memoryStream.GetBuffer(), 0, resourceLength);

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
            DynamicXmlNamespaceManager xmlNamespaceManager = new DynamicXmlNamespaceManager(new NameTable());
            xmlNamespaceManager.Resolve += OnResolveXmlNamespaceManager;
            XmlParserContext xmlParserContext = new XmlParserContext(null,
                xmlNamespaceManager, null, XmlSpace.None);

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
                SvgResolveNamespaceEventArgs e = new SvgResolveNamespaceEventArgs(prefix);
                ResolveNamespace(this, e);
                uri = e.Uri;
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
            else
            {
                XmlParserContext xmlParserContext = GetXmlParserContext();
                return XmlReader.Create(stream, xmlReaderSettings, xmlParserContext);
            }
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

            absoluteUrl = absoluteUrl.Trim();
            if (absoluteUrl.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                return GetElementById(absoluteUrl.Substring(1));
            }
            else
            {
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
                    if (docUri != null && docUri.AbsolutePath == absoluteUri.AbsolutePath)
                    {
                        return this;
                    }

                    SvgDocument doc = new SvgDocument((SvgWindow)Window);

                    XmlReaderSettings settings = this.GetXmlReaderSettings();

                    settings.CloseInput = true;

                    //PrepareXmlResolver(settings);

                    using (XmlReader reader = XmlReader.Create(
                        GetResource(absoluteUri).GetResponseStream(), settings,
                        absoluteUri.AbsolutePath))
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
                string result = "";

                XmlNode node = SelectSingleNode("/svg:svg/svg:title[text()!='']", this.NamespaceManager);

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
                return DocumentElement as ISvgSvgElement;
            }
        }

        public override XmlElement GetElementById(string elementId)
        {
            // TODO: handle element and attribute updates globally to watch for id changes.
            if (_collectedIds == null)
            {
                _collectedIds = new Dictionary<string, XmlElement>(StringComparer.Ordinal);

                XmlNodeList ids = this.SelectNodes("//*/@id");
                foreach (XmlAttribute node in ids)
                {
                    string valueKey = node.Value;
                    if (!_collectedIds.ContainsKey(valueKey))
                    {
                        _collectedIds.Add(node.Value, node.OwnerElement);
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
                    if (!_collectedIds.ContainsKey(valueKey))
                    {
                        _collectedIds.Add(node.Value, node.OwnerElement);
                    }
                }
            }

            // Find the item
            if (_collectedIds.ContainsKey(elementId))
            {
                return _collectedIds[elementId];
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
            private set {
                if (value != null)
                {
                    _svgFonts = value;
                }
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

        #endregion

        #region Protected Methods

        protected virtual IList<string> GetFontUrls()
        {
            List<string> fontUrls = new List<string>();
            if (this.IsLoaded == false)
            {
                return fontUrls;
            }

            XmlNodeList xmlList = this.SelectNodes(".//svg:font-face-uri", this.NamespaceManager);
            if (xmlList != null && xmlList.Count != 0)
            {
                foreach (XmlElement xmlNode in xmlList)
                {
                    if (xmlNode.HasAttribute("href", SvgDocument.XLinkNamespace))
                    {
                        string fontUrl = xmlNode.GetAttribute("href", SvgDocument.XLinkNamespace);
                        if (!string.IsNullOrWhiteSpace(fontUrl))
                        {
                            fontUrls.Add(fontUrl);
                        }
                    }
                    else if (xmlNode.HasAttribute("href"))
                    {
                        string fontUrl = xmlNode.GetAttribute("href");
                        if (!string.IsNullOrWhiteSpace(fontUrl))
                        {
                            fontUrls.Add(fontUrl);
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

                    GetFontUrl(cssSheet, fontUrls);
                }
            }

            GetFontUrl(this.UserAgentStyleSheet, fontUrls);
            GetFontUrl(this.UserStyleSheet, fontUrls);

            return fontUrls;
        }

        private static void GetFontUrl(CssStyleSheet cssSheet, IList<string> fontUrls)
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

            foreach (var rule in ruleList)
            {
                if (rule.Type == CssRuleType.FontFaceRule)
                {
                    var fontRule = (CssFontFaceRule)rule;
                    string fontUrl = fontRule.FontUrl;
                    if (!string.IsNullOrWhiteSpace(fontUrl))
                    {
                        fontUrl = fontUrl.Trim();
                        if (!fontUrl.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                        {
                            fontUrls.Add(fontUrl);
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

            IList<string> fontUrls = this.GetFontUrls();

            if (fontUrls == null || fontUrls.Count == 0)
            {
                return;
            }

            SvgWindow ownedWindow = _window.CreateOwnedWindow();
            ownedWindow.LoadFonts = false;

            for (int i = 0; i < fontUrls.Count; i++)
            {
                var fontUrl = fontUrls[i];
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
                        this.LoadLocalFont(fileUrl.LocalPath, ownedWindow);
                    }
                    else
                    {
                        //TODO
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    //Ignore the exception
                }
            }
        }

        private void LoadLocalFont(string fontPath, SvgWindow ownedWindow)
        {
            if (string.IsNullOrWhiteSpace(fontPath) || !File.Exists(fontPath))
            {
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
                        var fontNode = this.ImportNode(svgFont, true);
                        this.DocumentElement.AppendChild(fontNode);

//                        this.SvgFonts.Add(svgFont);
                    }
                }

                document = null;
            }
            else if (string.Equals(fileExt, ".ttf", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileExt, ".otf", StringComparison.OrdinalIgnoreCase))
            {
            }
        }

        #endregion
    }
}
