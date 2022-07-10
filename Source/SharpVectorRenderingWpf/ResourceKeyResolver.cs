using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using Microsoft.CSharp;

namespace SharpVectors.Renderers
{
    /// <summary>
    /// The default implememation of the <see cref="IResourceKeyResolver"/> interface. It implementations the default
    /// method of resolving the resource key; either by the unmodified SVG file name, or a formatted name using the file
    /// name and/or resource index if a format string is provided.
    /// </summary>
    [Serializable]
    public sealed class ResourceKeyResolver : WpfSettings<ResourceKeyResolver>, IResourceKeyResolver
    {
        #region Public Const Fields

        public static readonly string TagName    = "${name}";
        public static readonly string TagNumber  = "${number}";

        public static readonly string XmlTagName = "resolver";

        public static readonly string DefaultPenNameFormat = "Pen{0}";
        public static readonly string DefaultColorNameFormat = "Color{0}";
        public static readonly string DefaultBrushNameFormat = "Brush{0}";
        public static readonly string DefaultResourceNameFormat = string.Empty;

        #endregion

        #region Private Fields

        private string _nameFormat;
        private IDictionary<string, string> _nameParameters;

        #endregion

        #region Constructors and Destructor

        public ResourceKeyResolver()
            : this(string.Empty)
        {

        }

        public ResourceKeyResolver(string nameFormat)
        {
            this.NameFormat = nameFormat;
        }

        #endregion

        #region Public Properties

        public string NameFormat
        {
            get {
                return _nameFormat;
            }
            set {
                _nameFormat = string.Empty;
                if (string.IsNullOrWhiteSpace(value) == false &&
                    ResourceKeyResolver.ValidateResourceNameFormat(value))
                {
                    _nameFormat = value;
                }
            }
        }

        #endregion

        #region ICloneable Members

        public override ResourceKeyResolver Clone()
        {
            var resolver = new ResourceKeyResolver();
            return resolver;
        }

        #endregion

        #region IXmlSerializable Members

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

            _nameFormat = string.Empty;
            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            Debug.Assert(string.Equals(reader.Name, XmlTagName, _comparer));
            if (reader.NodeType != XmlNodeType.Element ||
                !String.Equals(reader.Name, XmlTagName, _comparer) ||
                reader.IsEmptyElement)
            {
                return;
            }

            string attrValue = reader.GetAttribute("type");
            if (string.IsNullOrWhiteSpace(attrValue) || 
                !string.Equals(attrValue, this.ResolverType.ToString(), _comparer))
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "property", _comparer))
                    {
                        if (string.Equals(reader.GetAttribute("name"), "ResourceNameFormat", _comparer))
                        {
                            _nameFormat = reader.GetAttribute("value");
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (string.Equals(reader.Name, XmlTagName, _comparer))
                    {
                        break;
                    }
                }
            }
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

            writer.WriteStartElement(XmlTagName); // start - resolver
            writer.WriteAttributeString("type", this.ResolverType.ToString());
            WriteProperty(writer, "ResourceNameFormat", _nameFormat);
            writer.WriteEndElement();             // end - resolver
        }

        #endregion

        #region IResourceKeyResolver Members

        /// <summary>
        /// Gets a value specifying the resource key resolver type.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ResourceKeyResolverType"/> specifying the type of the resource key resolver.
        /// </value>
        public ResourceKeyResolverType ResolverType
        {
            get {
                return ResourceKeyResolverType.Default;
            }
        }

        /// <summary>
        /// Gets a value specifying whether the resource key resolver is valid or not.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the resource resolver is valid, otherwise; it is <see langword="false"/>.
        /// </value>
        public bool IsValid
        {
            get {
                return true;
            }
        }

        /// <summary>
        /// This signals the start of a resource key resolving process.
        /// </summary>
        public void BeginResolve()
        {
            if (string.IsNullOrWhiteSpace(_nameFormat) == false &&
                ResourceKeyResolver.ValidateResourceNameFormat(_nameFormat) == false)
            {
                _nameFormat = string.Empty;
            }
            else if (_nameParameters == null)
            {
                _nameParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                _nameParameters.Add(ResourceKeyResolver.TagName, string.Empty);
                _nameParameters.Add(ResourceKeyResolver.TagNumber, string.Empty);
            }
        }

        /// <summary>
        /// This signals the end of a resource key resolving process.
        /// </summary>
        public void EndResolve()
        {
            if (_nameParameters != null)
            {
                _nameParameters[ResourceKeyResolver.TagName] = string.Empty;
                _nameParameters[ResourceKeyResolver.TagNumber] = string.Empty;
            }
        }

        /// <summary>
        /// Generates the resource key to be applied to the specified resource object, created from the file name 
        /// and from the specified source (directory).
        /// </summary>
        /// <param name="resource">The target resource object (<see cref="DrawingGroup"/>, <see cref="DrawingImage"/>) </param>
        /// <param name="index">The index of the resource file.</param>
        /// <param name="fileName">The file name of the SVG file without the extension.</param>
        /// <param name="fileSource">The source directory of the SVG file.</param>
        /// <returns>
        /// A <see cref="String"/> containing the key to be used to identify the specified resource. This must be at least
        /// 3 characters and less than 255 characters.
        /// </returns>
        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource)
        {
            if (index < 0)
            {
                throw new ArgumentException("The specified index is invalid", nameof(index));
            }
            NotNullNotEmpty(fileName, nameof(fileName));

            var useNameFormat = string.IsNullOrWhiteSpace(_nameFormat) == false && _nameFormat.Length > 6;
            if (useNameFormat)
            {
                _nameParameters[ResourceKeyResolver.TagName] = fileName;
                _nameParameters[ResourceKeyResolver.TagNumber] = index.ToString();
                return _nameParameters.Aggregate(_nameFormat, (s, kv) => s.Replace(kv.Key, kv.Value));
            }

            return fileName;
        }

        #endregion

        #region Public Static Methods

        public static Func<IResourceKeyResolver> GetResolver(ResourceKeyResolverType keyResolverType)
        {
            Func<IResourceKeyResolver> func = null;

            switch (keyResolverType)
            {
                case ResourceKeyResolverType.Default:
                    func = () => new ResourceKeyResolver();
                    break;

                case ResourceKeyResolverType.Dictionary:
                    func = () => new DictionaryKeyResolver();
                    break;

                case ResourceKeyResolverType.CodeSnippet:
                    func = () => new CodeSnippetKeyResolver();
                    break;
            }
            return func;
        }

        public static bool ValidateNameFormat(string nameFormat, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return !isRequired;
            }

            try
            {
                int testNumber = 1;
                string nameValue = string.Format(nameFormat, testNumber);

                return string.IsNullOrWhiteSpace(nameValue) != true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return false;
            }
        }

        public static bool ValidateResourceNameFormat(string nameFormat)
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return true;
            }

            try
            {
                var nameParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                nameParameters.Add(TagName, string.Empty);
                nameParameters.Add(TagNumber, string.Empty);

                string name1 = "test1";
                string number1 = "1";
                string name2 = "test2";
                string number2 = "2";

                nameParameters[TagName] = name1;
                nameParameters[TagNumber] = number1;
                var resourceName1 = nameParameters.Aggregate(nameFormat, (s, kv) => s.Replace(kv.Key, kv.Value));

                nameParameters[TagName] = name2;
                nameParameters[TagNumber] = number2;
                var resourceName2 = nameParameters.Aggregate(nameFormat, (s, kv) => s.Replace(kv.Key, kv.Value));

                return !string.IsNullOrWhiteSpace(resourceName1) &&
                    !string.Equals(resourceName1, resourceName2, StringComparison.Ordinal);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return false;
            }
        }

        #endregion
    }

    [Serializable]
    public sealed class DictionaryKeyResolver : WpfSettings<DictionaryKeyResolver>, IResourceKeyResolver
    {
        #region Private Fields

        private IDictionary<string, string> _keyDictionary;

        #endregion

        #region Constructors and Destructor

        public DictionaryKeyResolver()
            : this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
        }

        public DictionaryKeyResolver(IDictionary<string, string> keyDictionary)
        {
            if (keyDictionary != null)
            {
                _keyDictionary = keyDictionary;
            }
            else
            {
                _keyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region Public Properties

        public IDictionary<string, string> Dictionary
        {
            get {
                return _keyDictionary;
            }
            set {
                _keyDictionary = value;
            }
        }

        #endregion

        #region ICloneable Members

        public override DictionaryKeyResolver Clone()
        {
            var resolver = new DictionaryKeyResolver();
            return resolver;
        }

        #endregion

        #region IXmlSerializable Members

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
            _keyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var XmlTagName = ResourceKeyResolver.XmlTagName;
            NotNull(reader, nameof(reader));
            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            Debug.Assert(string.Equals(reader.Name, XmlTagName, _comparer));
            if (reader.NodeType != XmlNodeType.Element ||
                !String.Equals(reader.Name, XmlTagName, _comparer) ||
                reader.IsEmptyElement)
            {
                return;
            }

            string attrValue = reader.GetAttribute("type");
            if (string.IsNullOrWhiteSpace(attrValue) ||
                !string.Equals(attrValue, this.ResolverType.ToString(), _comparer))
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "property", _comparer))
                    {
                        string strName  = reader.GetAttribute("name");
                        string strValue = reader.GetAttribute("value");
                        if (!string.IsNullOrWhiteSpace(strName) && !string.IsNullOrWhiteSpace(strValue))
                        {
                            _keyDictionary[strName] = strValue;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (string.Equals(reader.Name, XmlTagName, _comparer))
                    {
                        break;
                    }
                }
            }
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

            var XmlTagName = ResourceKeyResolver.XmlTagName;
            writer.WriteStartElement(XmlTagName); // start - resolver
            writer.WriteAttributeString("type", this.ResolverType.ToString());
            if (_keyDictionary != null && _keyDictionary.Count != 0)
            {
                foreach (var keyValue in _keyDictionary)
                {
                    WriteProperty(writer, keyValue.Key, keyValue.Value);
                }
            }
            writer.WriteEndElement();             // end - resolver
        }

        #endregion

        #region IResourceKeyResolver Members

        /// <summary>
        /// Gets a value specifying the resource key resolver type.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ResourceKeyResolverType"/> specifying the type of the resource key resolver.
        /// </value>
        public ResourceKeyResolverType ResolverType
        {
            get {
                return ResourceKeyResolverType.Dictionary;
            }
        }

        /// <summary>
        /// Gets a value specifying whether the resource key resolver is valid or not.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the resource resolver is valid, otherwise; it is <see langword="false"/>.
        /// </value>
        public bool IsValid
        {
            get {
                return (_keyDictionary != null && _keyDictionary.Count != 0);
            }
        }

        /// <summary>
        /// This signals the start of a resource key resolving process.
        /// </summary>
        public void BeginResolve()
        {
        }

        /// <summary>
        /// This signals the end of a resource key resolving process.
        /// </summary>
        public void EndResolve()
        {
        }

        /// <summary>
        /// Generates the resource key to be applied to the specified resource object, created from the file name 
        /// and from the specified source (directory).
        /// </summary>
        /// <param name="resource">The target resource object (<see cref="DrawingGroup"/>, <see cref="DrawingImage"/>) </param>
        /// <param name="index">The index of the resource file.</param>
        /// <param name="fileName">The file name of the SVG file without the extension.</param>
        /// <param name="fileSource">The source directory of the SVG file.</param>
        /// <returns>
        /// A <see cref="string"/> containing the key to be used to identify the specified resource. This must be at least
        /// 3 characters and less than 255 characters.
        /// </returns>
        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource)
        {
            if (index < 0)
            {
                throw new ArgumentException("The specified index is invalid", nameof(index));
            }
            NotNullNotEmpty(fileName, nameof(fileName));

            if (_keyDictionary != null && _keyDictionary.Count != 0 && _keyDictionary.ContainsKey(fileName))
            {
                var keyValue = _keyDictionary[fileName];
                if (!string.IsNullOrWhiteSpace(keyValue) && (keyValue.Length >= 3 && keyValue.Length < 255))
                {
                    return keyValue;
                }
            }

            return fileName;
        }

        #endregion
    }

    [Serializable]
    public sealed class CodeSnippetKeyResolver : WpfSettings<CodeSnippetKeyResolver>, IResourceKeyResolver
    {
        #region Public Fields

        public const string SnippetClass = "SharpVectors.Renderers.SnippetKeyResolver";

        #endregion

        #region Private Fields

        private string _codeSnippet;
        private IResourceKeyResolver _keyResolver;

        #endregion

        #region Constructors and Destructor

        public CodeSnippetKeyResolver()
        {
        }

        public CodeSnippetKeyResolver(string codeSnippet)
        {
            _codeSnippet = codeSnippet;
            this.CreateKeyResolver();
        }

        #endregion

        #region Public Properties

        public string CodeSnippet
        {
            get {
                return _codeSnippet;
            }
            set {
                _codeSnippet = value;
                this.CreateKeyResolver();
            }
        }

        #endregion

        #region ICloneable Members

        public override CodeSnippetKeyResolver Clone()
        {
            var resolver = new CodeSnippetKeyResolver();
            return resolver;
        }

        #endregion

        #region IXmlSerializable Members

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
            var XmlTagName = ResourceKeyResolver.XmlTagName;
            NotNull(reader, nameof(reader));
            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            Debug.Assert(string.Equals(reader.Name, XmlTagName, _comparer));
            if (reader.NodeType != XmlNodeType.Element ||
                !String.Equals(reader.Name, XmlTagName, _comparer) ||
                reader.IsEmptyElement)
            {
                return;
            }

            string attrValue = reader.GetAttribute("type");
            if (string.IsNullOrWhiteSpace(attrValue) ||
                !string.Equals(attrValue, this.ResolverType.ToString(), _comparer))
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "property", _comparer))
                    {
                        if (string.Equals(reader.GetAttribute("name"), "CodeSnippet", _comparer))
                        {
                            while (reader.MoveToNextAttribute());
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.CDATA)
                            {
                                _codeSnippet = reader.Value;
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (string.Equals(reader.Name, XmlTagName, _comparer))
                    {
                        break;
                    }
                }
            }
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

            var XmlTagName = ResourceKeyResolver.XmlTagName;
            writer.WriteStartElement(XmlTagName); // start - resolver
            writer.WriteAttributeString("type", this.ResolverType.ToString());
            WriteCData(writer, "CodeSnippet", _codeSnippet);
            writer.WriteEndElement();             // end - resolver
        }

        #endregion

        #region IResourceKeyResolver Members

        /// <summary>
        /// Gets a value specifying the resource key resolver type.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ResourceKeyResolverType"/> specifying the type of the resource key resolver.
        /// </value>
        public ResourceKeyResolverType ResolverType
        {
            get {
                return ResourceKeyResolverType.CodeSnippet;
            }
        }

        /// <summary>
        /// Gets a value specifying whether the resource key resolver is valid or not.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the resource resolver is valid, otherwise; it is <see langword="false"/>.
        /// </value>
        public bool IsValid
        {
            get {
                return string.IsNullOrWhiteSpace(_codeSnippet) == false && _keyResolver != null;
            }
        }

        /// <summary>
        /// This signals the start of a resource key resolving process.
        /// </summary>
        public void BeginResolve()
        {
        }

        /// <summary>
        /// This signals the end of a resource key resolving process.
        /// </summary>
        public void EndResolve()
        {
        }

        /// <summary>
        /// Generates the resource key to be applied to the specified resource object, created from the file name 
        /// and from the specified source (directory).
        /// </summary>
        /// <param name="resource">The target resource object (<see cref="DrawingGroup"/>, <see cref="DrawingImage"/>) </param>
        /// <param name="index">The index of the resource file.</param>
        /// <param name="fileName">The file name of the SVG file without the extension.</param>
        /// <param name="fileSource">The source directory of the SVG file.</param>
        /// <returns>
        /// A <see cref="String"/> containing the key to be used to identify the specified resource. This must be at least
        /// 3 characters and less than 255 characters.
        /// </returns>
        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource)
        {
            if (index < 0)
            {
                throw new ArgumentException("The specified index is invalid", nameof(index));
            }
            NotNullNotEmpty(fileName, nameof(fileName));

            if (_keyResolver != null)
            {
                var keyValue = _keyResolver.Resolve(resource, index, fileName, fileSource);
                if (!string.IsNullOrWhiteSpace(keyValue) && (keyValue.Length >= 3 && keyValue.Length < 255))
                {
                    return keyValue;
                }
            }

            return fileName;
        }

        #endregion

        #region Public Methods

        public static Tuple<bool, string> CompileSnippet(string codeSnippet)
        {
            try
            {
                CodeSnippetKeyResolver keyResolver = new CodeSnippetKeyResolver();
                keyResolver._codeSnippet = codeSnippet;

                StringBuilder compileOutput = new StringBuilder();
                CompilerResults compileResults = keyResolver.CompileSnippets();
                if (compileResults == null)
                {
                    compileOutput.AppendLine("Code compilation failed.");
                    return Tuple.Create<bool, string>(false, compileOutput.ToString());
                }

                if (compileResults.Errors.HasErrors)
                {
                    foreach (CompilerError error in compileResults.Errors)
                    {
                        string errorText = error.ErrorText.Trim();
                        if (string.IsNullOrWhiteSpace(errorText))
                        {
                            continue;
                        }
                        compileOutput.AppendLine(String.Format("Error ({0}): Line({1}) - {2}", 
                            error.ErrorNumber, error.Line, errorText));
                    }

                    return Tuple.Create<bool, string>(false, compileOutput.ToString());
                }

                if (compileOutput.Length == 0)
                {
                    compileOutput.AppendLine("Code compilation successful.");
                }

                return Tuple.Create<bool, string>(true, compileOutput.ToString());
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, string>(false, ex.ToString());
            }
        }

        public static Tuple<bool, CodeSnippetKeyResolver, string> CompileResolver(string codeSnippet)
        {
            try
            {
                CodeSnippetKeyResolver keyResolver = new CodeSnippetKeyResolver();
                keyResolver._codeSnippet = codeSnippet;

                StringBuilder compileOutput = new StringBuilder();
                CompilerResults compileResults = keyResolver.CompileSnippets();
                if (compileResults == null)
                {
                    compileOutput.AppendLine("Code compilation failed.");
                    return Tuple.Create<bool, CodeSnippetKeyResolver, string>(false, null, compileOutput.ToString());
                }

                if (compileResults.Errors.HasErrors)
                {
                    foreach (CompilerError error in compileResults.Errors)
                    {
                        string errorText = error.ErrorText.Trim();
                        if (string.IsNullOrWhiteSpace(errorText))
                        {
                            continue;
                        }
                        compileOutput.AppendLine(String.Format("Error ({0}): Line({1}) - {2}", 
                            error.ErrorNumber, error.Line, errorText));
                    }

                    return Tuple.Create<bool, CodeSnippetKeyResolver, string>(false, null, compileOutput.ToString());
                }

                Assembly compiledAssembly = compileResults.CompiledAssembly;

                keyResolver._keyResolver = (IResourceKeyResolver)compiledAssembly.CreateInstance(SnippetClass);

                compileOutput.AppendLine("Code compilation successful.");
                return Tuple.Create<bool, CodeSnippetKeyResolver, string>(keyResolver._keyResolver != null, keyResolver, compileOutput.ToString());
            }
            catch (Exception ex)
            {
                return Tuple.Create<bool, CodeSnippetKeyResolver, string>(false, null, ex.ToString());
            }
        }

        #endregion

        #region Private Methods

        private void CreateKeyResolver()
        {
            try
            {
                _keyResolver = null;
                CompilerResults results = this.CompileSnippets();
                if (results == null)
                {
                    return;
                }
                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        string errorText = error.ErrorText.Trim();
                        if (string.IsNullOrWhiteSpace(errorText))
                        {
                            continue;
                        }
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, errorText));
                    }

                    Trace.TraceError(sb.ToString());
                    return;
                }

                Assembly compiledAssembly = results.CompiledAssembly;

                _keyResolver = (IResourceKeyResolver)compiledAssembly.CreateInstance(SnippetClass);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private CompilerResults CompileSnippets()
        {
            if (string.IsNullOrWhiteSpace(_codeSnippet))
            {
                return null;
            }

            CompilerParameters parameters = new CompilerParameters();

            // Add reference to assemblies 
            Assembly currAssembly = this.GetType().Assembly;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var referencedPaths = currAssembly.GetReferencedAssemblies()
                .Select(name => loadedAssemblies.SingleOrDefault(a => a.FullName == name.FullName)?.Location)
                .Where(l => l != null);
            foreach (var referencedPath in referencedPaths)
            {
                Trace.WriteLine(referencedPath);
                parameters.ReferencedAssemblies.Add(referencedPath);
            }
            //foreach (AssemblyName assemName in currAssembly.GetReferencedAssemblies())
            //{
            //    string assemPath = Assembly.Load(assemName.FullName).Location;
            //    parameters.ReferencedAssemblies.Add(assemPath);
            //}

            parameters.ReferencedAssemblies.Add(currAssembly.Location);

            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = true;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = false;

            parameters.IncludeDebugInformation = false;

            parameters.CompilerOptions = "/optimize+";

            var providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, _codeSnippet);

            return results;
        }

        #endregion
    }
}
