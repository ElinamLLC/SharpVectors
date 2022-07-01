using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows.Media;

using SharpVectors.IoC;
using SharpVectors.Dom;

namespace SharpVectors.Renderers.Wpf
{
    [Serializable]
    public sealed class WpfResourceSettings : WpfSettings<WpfResourceSettings>
    {
        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        public const string XmlTagName = "resourceSettings";
        public const string XmlVersion = "1.0.0";

        #region Private Fields

        private bool _bindToColors;
        private bool _bindToResources;
        private bool _bindPenToBrushes;

        private string _penNameFormat;
        private string _colorNameFormat;
        private string _brushNameFormat;

        private bool _resourceFreeze;
        private bool _useResourceIndex;
        private ResourceModeType _resourceMode;
        private ResourceAccessType _resourceAccess;

        private int _indentSpaces;
        private int _numericPrecision;

        private ISet<string> _svgSources;
        private IDictionary<Color, string> _colorPalette;

        private SvgContainer _resolverContainer;

        #endregion

        #region Constructors and Destructor

        public WpfResourceSettings()
        {
            _bindToColors       = true;
            _bindToResources    = true;
            _bindPenToBrushes   = true;
            _penNameFormat      = ResourceKeyResolver.DefaultPenNameFormat;
            _colorNameFormat    = ResourceKeyResolver.DefaultColorNameFormat;
            _brushNameFormat    = ResourceKeyResolver.DefaultBrushNameFormat;
            _resourceFreeze     = true;
            _useResourceIndex   = false;
            _resourceMode       = ResourceModeType.Drawing;
            _resourceAccess     = ResourceAccessType.Dynamic;

            _indentSpaces       = 2;
            _numericPrecision   = 4;

            _resolverContainer  = new SvgContainer();
            _svgSources         = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public WpfResourceSettings(WpfResourceSettings source)
            : base(source)
        {
            if (source == null)
            {
                return;
            }

            _bindToColors       = source._bindToColors;
            _bindToResources    = source._bindToResources;
            _bindPenToBrushes   = source._bindPenToBrushes;
            _penNameFormat      = source._penNameFormat;
            _colorNameFormat    = source._colorNameFormat;
            _brushNameFormat    = source._brushNameFormat;
            _resourceFreeze     = source._resourceFreeze;
            _useResourceIndex   = source._useResourceIndex;
            _resourceMode       = source._resourceMode;
            _resourceAccess     = source._resourceAccess;
            _colorPalette       = source._colorPalette;
            _svgSources         = source._svgSources;
            _resolverContainer  = source._resolverContainer;

            _indentSpaces       = source._indentSpaces;
            _numericPrecision   = source._numericPrecision;
        }

        #endregion

        #region Public Properties

        public bool BindToColors
        {
            get {
                return _bindToColors;
            }
            set {
                _bindToColors = value;
            }
        }

        public bool BindToResources
        {
            get {
                return _bindToResources;
            }
            set {
                _bindToResources = value;
            }
        }

        public bool BindPenToBrushes
        {
            get {
                return _bindPenToBrushes;
            }
            set {
                _bindPenToBrushes = value;
            }
        }

        public string PenNameFormat
        {
            get {
                return _penNameFormat;
            }
            set {
                if (ResourceKeyResolver.ValidateNameFormat(value, true))
                {
                    _penNameFormat = value;
                }
            }
        }

        public string ColorNameFormat
        {
            get {
                return _colorNameFormat;
            }
            set {
                if (ResourceKeyResolver.ValidateNameFormat(value, true))
                {
                    _colorNameFormat = value;
                }
            }
        }

        public string BrushNameFormat
        {
            get {
                return _brushNameFormat;
            }
            set {
                if (ResourceKeyResolver.ValidateNameFormat(value, true))
                {
                    _brushNameFormat = value;
                }
            }
        }

        public bool ResourceFreeze
        {
            get {
                return _resourceFreeze;
            }
            set {
                _resourceFreeze = value;
            }
        }

        public bool UseResourceIndex
        {
            get {
                return _useResourceIndex;
            }
            set {
                _useResourceIndex = value;
            }
        }

        public ResourceModeType ResourceMode
        {
            get {
                return _resourceMode;
            }
            set {
                _resourceMode = value;
            }
        }

        public ResourceAccessType ResourceAccess
        {
            get {
                return _resourceAccess;
            }
            set {
                _resourceAccess = value;
            }
        }

        public ResourceKeyResolverType ResourceKeyResolverType
        {
            get {
                var keyResolver = this.RetrieveResolver();
                if (keyResolver != null)
                {
                    return keyResolver.ResolverType;
                }

                return ResourceKeyResolverType.None;
            }
        }

        public int IndentSpaces
        {
            get {
                return _indentSpaces;
            }
            set {
                if (value >= 0 && value <= 8)
                {
                    _indentSpaces = value;
                }
            }
        }

        public int NumericPrecision
        {
            get {
                return _numericPrecision;
            }
            set {
                if (value >= 0 && value <= 99)
                {
                    _numericPrecision = value;
                }
                else
                {
                    _numericPrecision = -1;
                }
            }
        }

        public int SourceCount
        {
            get {
                return _svgSources.Count;
            }
        }

        public IEnumerable<string> Sources
        {
            get {
                return _svgSources;
            }
        }

        public IDictionary<Color, string> ColorPalette
        {
            get {
                return _colorPalette;
            }
            set {
                _colorPalette = value;
            }
        }

        #endregion

        #region Public Methods

        public void CopyTo(WpfDrawingResources resources)
        {
            if (resources == null)
            {
                return;
            }

            resources.BindToColors       = this._bindToColors;
            resources.BindToResources    = this._bindToResources;
            resources.BindPenToBrushes   = this._bindPenToBrushes;
            resources.PenNameFormat      = this._penNameFormat;
            resources.ColorNameFormat    = this._colorNameFormat;
            resources.BrushNameFormat    = this._brushNameFormat;
            resources.ResourceFreeze     = this._resourceFreeze;
            resources.UseResourceIndex   = this._useResourceIndex;
            resources.ResourceMode       = this._resourceMode;
            resources.ResourceAccess     = this._resourceAccess;
            resources.ColorPalette       = this._colorPalette;
        }

        private void RemoveResolver()
        {
            if (this.IsResolverRegistered() && _resolverContainer != null)
            {
                _resolverContainer.Dispose();
                _resolverContainer = null;
            }
        }

        public void RegisterResolver(IResourceKeyResolver keyResolver)
        {
            if (keyResolver == null)
            {
                return;
            }

            this.RemoveResolver();

            if (_resolverContainer == null)
            {
                _resolverContainer = new SvgContainer();
            }

            _resolverContainer.Register<IResourceKeyResolver>(delegate() { return keyResolver; }).AsSingleton();
        }

        public void RegisterResolver(Func<IResourceKeyResolver> keyResolver)
        {
            if (keyResolver == null)
            {
                return;
            }

            this.RemoveResolver();

            if (_resolverContainer == null)
            {
                _resolverContainer = new SvgContainer();
            }

            _resolverContainer.Register<IResourceKeyResolver>(keyResolver).AsSingleton();
        }

        public bool IsResolverRegistered()
        {
            if (_resolverContainer == null)
            {
                return false;
            }
            return _resolverContainer.HasService(typeof(IResourceKeyResolver));
        }

        public IResourceKeyResolver RetrieveResolver()
        {
            if (_resolverContainer == null)
            {
                return null;
            }
            return _resolverContainer.Resolve<IResourceKeyResolver>();
        }

        public bool AddSource(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return false;
            }
            // A file or directory
            if (sourcePath.EndsWith(SvgConstants.FileExt, StringComparison.OrdinalIgnoreCase)
                || sourcePath.EndsWith(SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase))
            {
                if (_svgSources.Contains(sourcePath))
                {
                    return true;
                }
                _svgSources.Add(sourcePath);
            }
            else
            {
                var sourceDir = StripEndBackSlash(sourcePath);
                if (_svgSources.Contains(sourceDir))
                {
                    return true;
                }
                _svgSources.Add(sourceDir);
            }

            return true;
        }

        public bool RemoveSource(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return false;
            }
            // A file or directory
            if (sourcePath.EndsWith(SvgConstants.FileExt, StringComparison.OrdinalIgnoreCase)
                || sourcePath.EndsWith(SvgConstants.FileExtZ, StringComparison.OrdinalIgnoreCase))
            {
                return _svgSources.Remove(sourcePath);
            }
            else
            {
                var sourceDir = StripEndBackSlash(sourcePath);
                return _svgSources.Remove(sourceDir);
            }
        }

        public void ClearSources()
        {
            _svgSources.Clear();
        }

        private static string StripEndBackSlash(string dir)
        {
            if (dir.EndsWith("\\"))
                return dir.Substring(0, dir.Length - 1);
            else
                return dir;
        }

        #endregion

        #region IXmlSerializable Members

        public void Load(string contentFile)
        {
            PathMustExist(contentFile, nameof(contentFile));

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(contentFile, settings);

                this.ReadXml(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Save(string contentFile)
        {
            NotNullNotEmpty(contentFile, nameof(contentFile));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = new string(' ', 4);
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(contentFile, settings);

                writer.WriteStartDocument();

                this.WriteXml(writer);

                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            NotNull(reader, nameof(reader));

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }
            if (!String.Equals(reader.Name, XmlTagName, _comparer))
            {
                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            NotNull(writer, nameof(writer));

            writer.WriteStartElement(XmlTagName);  // start - XmlTagName
            writer.WriteAttributeString("version", XmlVersion);

            writer.WriteStartElement("properties");  // start - properties
            WriteProperty(writer, "ResourceFreeze", _resourceFreeze);
            WriteProperty(writer, "UseResourceIndex", _useResourceIndex);
            WriteEnum(writer, "ResourceMode", _resourceMode.ToString());
            WriteEnum(writer, "ResourceAccess", _resourceAccess.ToString());
            WriteProperty(writer, "IndentSpaces", _indentSpaces);
            WriteProperty(writer, "NumericPrecision", _numericPrecision);
            writer.WriteEndElement();                // end - properties

            writer.WriteStartElement("naming");      // start - naming
            WriteProperty(writer, "PenNameFormat", _penNameFormat);
            WriteProperty(writer, "ColorNameFormat", _colorNameFormat);
            WriteProperty(writer, "BrushNameFormat", _brushNameFormat);

            var keyResolver = this.RetrieveResolver();
            if (keyResolver != null && keyResolver.ResolverType != ResourceKeyResolverType.None)
            {
                keyResolver.WriteXml(writer);
            }
            else
            {
                writer.WriteStartElement(ResourceKeyResolver.XmlTagName); // start - resolver
                writer.WriteAttributeString("type", ResourceKeyResolverType.None.ToString());
                writer.WriteEndElement();                                 // end - resolver
            }

            writer.WriteEndElement();                // end - naming

            writer.WriteStartElement("binding");      // start - binding
            WriteProperty(writer, "BindToResources", _bindToResources);
            WriteProperty(writer, "BindPenToBrushes", _bindPenToBrushes);
            WriteProperty(writer, "BindToColors", _bindToColors);
            writer.WriteEndElement();                // end - binding

            writer.WriteStartElement("palettes");    // start - palettes
            if (_colorPalette != null && _colorPalette.Count != 0)
            {
                foreach (var palette in _colorPalette)
                {
                    writer.WriteStartElement("palette");
                    writer.WriteAttributeString("color", palette.Key.ToString());
                    writer.WriteAttributeString("name", palette.Value);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();                // end - palettes

            writer.WriteStartElement("sources");    // start - sources
            if (_svgSources != null && _svgSources.Count != 0)
            {
                foreach (var svgSource in _svgSources)
                {
                    writer.WriteStartElement("sources");
                    writer.WriteString(svgSource);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();                // end - sources

            writer.WriteEndElement();           // end - XmlTagName
        }

        #endregion

        #region ICloneable Members

        public override WpfResourceSettings Clone()
        {
            var settings = new WpfResourceSettings(this);
            return settings;
        }

        #endregion
    }
}
