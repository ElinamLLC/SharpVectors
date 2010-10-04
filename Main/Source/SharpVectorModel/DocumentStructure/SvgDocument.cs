// Define this to enable the dispatching of load events.  The implementation
// of load events requires that a complete implementation of SvgDocument.Load
// be supplied rather than relying on the base XmlDocument.Load behaviour.
// This is required because I know of no way to hook into the key stages of
// XML document creation in order to throw events at the right times during
// the load process.
// <developer>niklas@protocol7.com</developer>
// <completed>60</completed>

using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Xml;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Resources;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The root object in the document object hierarchy of an Svg document.
    /// </summary>
    /// <remarks>
    /// <p>
    /// When an 'svg'  element is embedded inline as a component of a
    /// document from another namespace, such as when an 'svg' element is
    /// embedded inline within an XHTML document
    /// [<a href="http://www.w3.org/TR/SVG/refs.html#ref-XHTML">XHTML</a>],
    /// then an
    /// <see cref="ISvgDocument">ISvgDocument</see> object will not exist;
    /// instead, the root object in the
    /// document object hierarchy will be a Document object of a different
    /// type, such as an HTMLDocument object.
    /// </p>
    /// <p>
    /// However, an <see cref="ISvgDocument">ISvgDocument</see> object will
    /// indeed exist when the root
    /// element of the XML document hierarchy is an 'svg' element, such as
    /// when viewing a stand-alone SVG file (i.e., a file with MIME type
    /// "image/svg+xml"). In this case, the
    /// <see cref="ISvgDocument">ISvgDocument</see> object will be the
    /// root object of the document object model hierarchy.
    /// </p>
    /// <p>
    /// In the case where an SVG document is embedded by reference, such as
    /// when an XHTML document has an 'object' element whose href attribute
    /// references an SVG document (i.e., a document whose MIME type is
    /// "image/svg+xml" and whose root element is thus an 'svg' element),
    /// there will exist two distinct DOM hierarchies. The first DOM hierarchy
    /// will be for the referencing document (e.g., an XHTML document). The
    /// second DOM hierarchy will be for the referenced SVG document. In this
    /// second DOM hierarchy, the root object of the document object model
    /// hierarchy is an <see cref="ISvgDocument">ISvgDocument</see> object.
    /// </p>
    /// <p>
    /// The <see cref="ISvgDocument">ISvgDocument</see> interface contains a
    /// similar list of attributes and
    /// methods to the HTMLDocument interface described in the
    /// <a href="http://www.w3.org/TR/REC-DOM-Level-1/level-one-html.html">Document
    /// Object Model (HTML) Level 1</a> chapter of the
    /// [<a href="http://www.w3.org/TR/SVG/refs.html#ref-DOM1">DOM1</a>] specification.
    /// </p>
    /// </remarks>
    public class SvgDocument : CssXmlDocument, ISvgDocument
    {
        #region Public Static Fields

        public const string SvgNamespace   = "http://www.w3.org/2000/svg";

        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";

        #endregion

        #region Private Fields
        
        private bool _ignoreComments;
        private bool _ignoreProcessingInstructions;
        private bool _ignoreWhitespace;

        private SvgWindow _window;

        private XmlReaderSettings _settings;

        #endregion

        #region Constructors

        private SvgDocument()
        {
            _ignoreComments               = false;
            _ignoreWhitespace             = false;
            _ignoreProcessingInstructions = false;

            this.PreserveWhitespace = true;
        }

        public SvgDocument(SvgWindow window)
            : this()
        {
            this._window = window;
            this._window.Document = this;

            // set up CSS properties
            AddStyleElement(SvgDocument.SvgNamespace, "style");
            CssPropertyProfile = CssPropertyProfile.SvgProfile;
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
            get 
            { 
                return _settings; 
            }
            set 
            { 
                _settings = value; 
            }
        }

        #endregion

        #region NamespaceManager

        private XmlNamespaceManager namespaceManager;

        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (namespaceManager == null)
                {
                    // Setup namespace manager and add default namespaces
                    namespaceManager = new XmlNamespaceManager(this.NameTable);
                    namespaceManager.AddNamespace(String.Empty, SvgDocument.SvgNamespace);
                    namespaceManager.AddNamespace("svg", SvgDocument.SvgNamespace);
                    namespaceManager.AddNamespace("xlink", SvgDocument.XLinkNamespace);
                }

                return namespaceManager;
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
            else if (ns == SvgNamespace)
            {
                return new SvgElement(prefix, localName, ns, this);
            }
            else
            {
                // Now, if the ns is empty, we try creating with the default namespace for cases
                // where the node is imported from an external SVG document...
                if (String.IsNullOrEmpty(ns))
                {
                    result = SvgElementFactory.Create(prefix, localName, SvgNamespace, this);
                    if (result != null)
                    {       
                        return result;
                    }
                }
            }

            return base.CreateElement(prefix, localName, ns);
        }

        #endregion

        #region Support collections

        private string[] supportedFeatures = new string[]
			{
				"org.w3c.svg.static",
				"http://www.w3.org/TR/Svg11/feature#Shape",
				"http://www.w3.org/TR/Svg11/feature#BasicText",
				"http://www.w3.org/TR/Svg11/feature#OpacityAttribute"
			};

        private string[] supportedExtensions = new string[] { };

        public override bool Supports(string feature, string version)
        {
            foreach (string supportedFeature in supportedFeatures)
            {
                if (supportedFeature == feature)
                {
                    return true;
                }
            }

            foreach (string supportedExtension in supportedExtensions)
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
            xmlResolver.Resolving     += OnXmlResolverResolving;
            xmlResolver.GettingEntity += OnXmlResolverGettingEntity;

            XmlReaderSettings settings = new XmlReaderSettings();

            settings.ProhibitDtd                  = false;

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
        /// <param name="url">
        /// URL for the file containing the XML document to load.
        /// </param>
        public override void Load(string url)
        {
            // Provide a support for the .svgz files...
            UriBuilder fileUrl = new UriBuilder(url);
            if (String.Equals(fileUrl.Scheme, "file"))
            {
                string fileExt = Path.GetExtension(url);
                if (String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream fileStream = File.OpenRead(fileUrl.Uri.LocalPath))
                    {
                        using (GZipStream zipStream = 
                            new GZipStream(fileStream, CompressionMode.Decompress))
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
            using (XmlReader reader = CreateValidatingXmlReader(url))
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
        /// <param name="reader"></param>
        public override void Load(TextReader reader)
        {
            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader xmlReader = XmlReader.Create(reader, settings))
            using (XmlReader xmlReader = CreateValidatingXmlReader(reader))
            {
                this.Load(xmlReader);
            }
        }

        /// <summary>
        /// Loads the XML document from the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the XML document to load.
        /// </param>
        public override void Load(Stream stream)
        {
            //XmlReaderSettings settings = this.GetXmlReaderSettings();

            //PrepareXmlResolver(settings);
            //using (XmlReader reader = XmlReader.Create(stream, settings))
            using (XmlReader reader = CreateValidatingXmlReader(String.Empty, stream))
            {
                this.Load(reader);
            }
        }

        #endregion

        #region Resource handling

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
                        if (name.StartsWith(_rootType.Namespace))
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
        /// <param name="role">The role.</param>
        /// <param name="ofObjectToReturn">The of object to return.</param>
        /// <returns></returns>
        private object OnXmlResolverGettingEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.IsFile)
            {
                string fullPath = absoluteUri.ToString();
                if (!String.IsNullOrEmpty(fullPath))
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

                return null;
            }

            string path = (absoluteUri.Host + absoluteUri.AbsolutePath.Replace('/', '.'));
            string foundResource = GetEntityUri(path);
            if (foundResource != null)
                return GetEntityFromUri(foundResource, ofObjectToReturn);

            return null;
        }

        /// <summary>
        /// Gettings the URI direct.
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
            if (String.IsNullOrEmpty(uri))
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
            XmlParserContext xmlParserContext   = GetXmlParserContext();
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
            XmlParserContext xmlParserContext   = GetXmlParserContext();
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
            if (String.IsNullOrEmpty(uri))
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
            absoluteUrl = absoluteUrl.Trim();
            if (absoluteUrl.StartsWith("#"))
            {
                return GetElementById(absoluteUrl.Substring(1));
            }
            else
            {
                Uri docUri = ResolveUri("");
                Uri absoluteUri = new Uri(absoluteUrl);

                string fragment = absoluteUri.Fragment;

                if (fragment.Length == 0)
                {
                    // no fragment => return entire document
                    if (docUri.AbsolutePath == absoluteUri.AbsolutePath)
                    {
                        return this;
                    }
                    else
                    {
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
            DirectoryInfo workingDir = this._window.WorkingDir;

            string baseUri = BaseURI;
            if (baseUri.Length == 0)
            {
                baseUri = "file:///" + workingDir.FullName.Replace('\\', '/');
            }

            return new Uri(new Uri(baseUri), uri);
        }

        #endregion

        #region ISvgDocument Members

        /// <summary>
        /// The title of the document which is the text content of the first child title element of the 'svg' root element.
        /// </summary>
        public string Title
        {
            get
            {
                string result = "";

                XmlNode node = SelectSingleNode("/svg:svg/svg:title[text()!='']", NamespaceManager);

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
        /// Returns the URI of the page that linked to this page. The value is an empty string if the user navigated to the page directly (not through a link, but, for example, via a bookmark).
        /// </summary>
        public string Referrer
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// The domain name of the server that served the document, or a null string if the server cannot be identified by a domain name.
        /// </summary>
        public string Domain
        {
            get
            {
                if (Url.Length == 0 ||
                    Url.StartsWith(Uri.UriSchemeFile))
                {
                    return null;
                }
                else
                {
                    return new Uri(Url).Host;
                }
            }
        }

        /// <summary>
        /// The root 'svg' element in the document hierarchy
        /// </summary>
        public ISvgSvgElement RootElement
        {
            get
            {
                return DocumentElement as ISvgSvgElement;
            }
        }

        internal Dictionary<string, XmlElement> collectedIds;

        public override XmlElement GetElementById(string elementId)
        {
            // TODO: handle element and attribute updates globally to watch for id changes.
            if (collectedIds == null)
            {
                collectedIds = new Dictionary<string, XmlElement>();
                // TODO: handle xml:id, handle duplicate ids?
                XmlNodeList ids = this.SelectNodes("//*/@id");
                foreach (XmlAttribute node in ids)
                {
                    try
                    {
                        collectedIds.Add(node.Value, node.OwnerElement);
                    }
                    catch
                    {
                        // Duplicate ID... what to do?
                    }
                }
            }

            // Find the item
            if (collectedIds.ContainsKey(elementId))
            {
                return collectedIds[elementId];
            }

            return null;
        }

        #endregion

        #region ISvgDocument Members from SVG 1.2

        public ISvgWindow Window
        {
            get
            {
                return _window;
            }
        }

        #endregion

        #region Other public properties

        public new SvgDocument OwnerDocument
        {
            get
            {
                return base.OwnerDocument as SvgDocument;
            }
        }

        #endregion
    }
}
