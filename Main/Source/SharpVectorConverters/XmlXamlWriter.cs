using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Markup;
using System.Windows.Markup.Primitives;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This is a customized XAML writer, which provides Extensible Application 
    /// Markup Language (XAML) serialization of provided runtime objects into XAML.
    /// </summary>
    /// <remarks>
    /// This is designed to be used by the SVG to XAML converters, and may not be
    /// useful in general applications.
    /// </remarks>
    public sealed class XmlXamlWriter
    {
        #region Private Fields

        private bool _nullExtension;
        private Type _nullType;

        private string _windowsDir;
        private string _windowsPath;

        private CultureInfo _culture;

        private NamespaceCache _namespaceCache;
        private WpfDrawingSettings _wpfSettings;

        private Dictionary<Type, string> _contentProperties;
        private Dictionary<string, NamespaceMap> _dicNamespaceMap;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="XmlXamlWriter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlXamlWriter"/> class
        /// with the default settings.
        /// </summary>
        public XmlXamlWriter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlXamlWriter"/> class
        /// with the specified settings.
        /// </summary>
        /// <param name="settings">
        /// An instance of <see cref="WpfDrawingSettings"/> specifying the
        /// rendering options.
        /// </param>
        public XmlXamlWriter(WpfDrawingSettings settings)
        {
            _culture           = CultureInfo.InvariantCulture;

            _nullType          = typeof(NullExtension);
            _namespaceCache    = new NamespaceCache(_culture);
            _dicNamespaceMap   = new Dictionary<string, NamespaceMap>(StringComparer.OrdinalIgnoreCase);
            _contentProperties = new Dictionary<Type, string>();

            _windowsPath = "%WINDIR%";
            _windowsDir  = Environment.ExpandEnvironmentVariables(_windowsPath).ToLower();

            _windowsDir  = _windowsDir.Replace(@"\", "/");
            _wpfSettings = settings;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to include a null markup
        /// extension in the output XAML.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the null markup extension is
        /// included in the output XAML; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool IncludeNullExtension
        {
            get
            {
                return _nullExtension;
            }
            set
            {
                _nullExtension = value;
            }
        }

        #endregion

        #region Public Methods

        public static string Convert(object obj)
        {
            XmlXamlWriter writer = new XmlXamlWriter();

            return writer.Save(obj);
        }

        // Summary:
        // Returns a Extensible Application Markup Language (XAML) string that serializes
        // the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // Returns:
        // Extensible Application Markup Language (XAML) string that can be written
        // to a stream or file. The logical tree of all elements that fall under the
        // provided obj element will be serialized.
        //
        // Exceptions:
        // System.Security.SecurityException:
        // the application is not running in full trust.
        //
        // System.ArgumentNullException:
        // obj is null.
        public string Save(object obj)
        {
            if (obj == null)
            {
                return String.Empty;
            }

            if (_contentProperties == null)
            {
                _contentProperties = new Dictionary<Type, string>();
            }

            //TODO--PAUL: For now just cheat...
            string nsName = obj.GetType().Namespace;

            if (nsName != null && nsName.StartsWith("System.Windows"))
            {
                _namespaceCache.IsFrameworkRoot = true;
            }

            ResolveXmlNamespaces(obj);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                WriteObject(null, obj, xmlWriter, true);
            }
            writer.Close();

            _contentProperties = null;

            return builder.ToString();
        }

        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information into a provided
        // stream to serialize the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // stream:
        // Destination stream for the serialized XAML information.
        //
        // Exceptions:
        // System.Security.SecurityException:
        // the application is not running in full trust.
        //
        // System.ArgumentNullException:
        // obj is null -or- stream is null.
        public void Save(object obj, Stream stream)
        {
            if (obj == null)
            {
                return;
            }

            if (_contentProperties == null)
            {
                _contentProperties = new Dictionary<Type, string>();
            }

            //TODO--PAUL: For now just cheat...
            string nsName = obj.GetType().Namespace;

            if (nsName != null && nsName.StartsWith("System.Windows"))
            {
                _namespaceCache.IsFrameworkRoot = true;
            }

            ResolveXmlNamespaces(obj);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
            {
                WriteObject(null, obj, xmlWriter, true);
            }

            _contentProperties = null;
        }

        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information as the source
        // for a provided text writer object. The output of the text writer can then
        // be used to serialize the provided object.
        //
        // Parameters:
        // writer:
        // TextWriter instance to use to write the serialized XAML information.
        //
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // Exceptions:
        // System.ArgumentNullException:
        // obj is null -or- writer is null.
        //
        // System.Security.SecurityException:
        // the application is not running in full trust.
        public void Save(object obj, TextWriter writer)
        {
            if (obj == null)
            {
                return;
            }

            if (_contentProperties == null)
            {
                _contentProperties = new Dictionary<Type, string>();
            }

            //TODO--PAUL: For now just cheat...
            string nsName = obj.GetType().Namespace;

            if (nsName != null && nsName.StartsWith("System.Windows"))
            {
                _namespaceCache.IsFrameworkRoot = true;
            }

            ResolveXmlNamespaces(obj);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                WriteObject(null, obj, xmlWriter, true);
            }

            _contentProperties = null;
        }

        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information as the source
        // for a provided XML writer object. The output of the XML writer can then be
        // used to serialize the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // xmlWriter:
        // Writer to use to write the serialized XAML information.
        //
        // Exceptions:
        // System.ArgumentNullException:
        // obj is null -or- manager is null.
        //
        // System.Security.SecurityException:
        // the application is not running in full trust.
        public void Save(object obj, XmlWriter xmlWriter)
        {
            if (obj == null)
            {
                return;
            }

            if (_contentProperties == null)
            {
                _contentProperties = new Dictionary<Type, string>();
            }

            //TODO--PAUL: For now just cheat...
            string nsName = obj.GetType().Namespace;

            if (nsName != null && nsName.StartsWith("System.Windows"))
            {
                _namespaceCache.IsFrameworkRoot = true;
            }

            ResolveXmlNamespaces(obj);

            WriteObject(null, obj, xmlWriter, true);

            _contentProperties = null;
        }

        #endregion

        #region Private Methods

        private void WriteObject(object key, object obj, XmlWriter writer, bool isRoot)
        {
            List<MarkupProperty> propertyElements = new List<MarkupProperty>();
            MarkupProperty contentProperty = null;
            string contentPropertyName = null;
            MarkupObject markupObj = MarkupWriter.GetMarkupObjectFor(obj);
            Type objectType = markupObj.ObjectType;

            string ns = _namespaceCache.GetNamespaceUriFor(objectType);
            string prefix = _namespaceCache.GetDefaultPrefixFor(ns);

            if (isRoot)
            {
                if (String.IsNullOrEmpty(prefix))
                {
                    if (String.IsNullOrEmpty(ns))
                    {
                        writer.WriteStartElement(markupObj.ObjectType.Name, NamespaceCache.DefaultNamespace);
                        writer.WriteAttributeString("xmlns",
                            NamespaceCache.XmlnsNamespace, NamespaceCache.DefaultNamespace);
                    }
                    else
                    {
                        writer.WriteStartElement(markupObj.ObjectType.Name, ns);
                        writer.WriteAttributeString("xmlns", NamespaceCache.XmlnsNamespace, ns);
                    }
                }
                else
                {
                    writer.WriteStartElement(prefix, markupObj.ObjectType.Name, ns);
                }
                writer.WriteAttributeString("xmlns", "x",
                    NamespaceCache.XmlnsNamespace, NamespaceCache.XamlNamespace);

                foreach (NamespaceMap map in _dicNamespaceMap.Values)
                {
                    if (!String.IsNullOrEmpty(map.Prefix) && !String.Equals(map.Prefix, "x"))
                        writer.WriteAttributeString("xmlns", map.Prefix, NamespaceCache.XmlnsNamespace, map.XmlNamespace);
                }
            }
            else
            {
                //TODO: Fix - the best way to handle this case...
                if (markupObj.ObjectType.Name == "PathFigureCollection" && markupObj.Instance != null)
                {
                    WriteState writeState = writer.WriteState;

                    if (writeState == WriteState.Element)
                    {
                        //writer.WriteAttributeString("Figures", 
                        //    markupObj.Instance.ToString());
                        writer.WriteAttributeString("Figures", 
                            System.Convert.ToString(markupObj.Instance, _culture));
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(prefix))
                        {
                            writer.WriteStartElement("PathGeometry.Figures");
                        }
                        else
                        {
                            writer.WriteStartElement("PathGeometry.Figures", ns);
                        }
                        //writer.WriteString(markupObj.Instance.ToString());
                        writer.WriteString(System.Convert.ToString(
                            markupObj.Instance, _culture));
                        writer.WriteEndElement();
                    }
                    return;
                }
                else
                {
                    if (String.IsNullOrEmpty(prefix))
                    {
                        writer.WriteStartElement(markupObj.ObjectType.Name);
                    }
                    else
                    {
                        writer.WriteStartElement(markupObj.ObjectType.Name, ns);
                    }
                }
            }

            // Add the x:Name for object like Geometry/Drawing not derived from FrameworkElement...
            DependencyObject dep = obj as DependencyObject;
            if (dep != null)
            {
                string nameValue = dep.GetValue(FrameworkElement.NameProperty) as string;
                if (!String.IsNullOrEmpty(nameValue) && !(dep is FrameworkElement))
                {
                    writer.WriteAttributeString("x", "Name", NamespaceCache.XamlNamespace, nameValue);
                }
            }

            if (key != null)
            {
                string keyString = key.ToString();
                if (keyString.Length > 0)
                {
                    writer.WriteAttributeString("x", "Key", NamespaceCache.XamlNamespace, keyString);
                }
                else
                {
                    //TODO: key may not be a string, what about x:Type...
                    throw new NotImplementedException(
                        "Sample XamlWriter cannot yet handle keys that aren't strings");
                }
            }

            //Look for CPA info in our cache that keeps contentProperty names per Type
            //If it doesn't have an entry, go get the info and store it.
            if (!_contentProperties.ContainsKey(objectType))
            {
                string lookedUpContentProperty = String.Empty;
                foreach (Attribute attr in markupObj.Attributes)
                {
                    ContentPropertyAttribute cpa = attr as ContentPropertyAttribute;
                    if (cpa != null)
                    {
                        lookedUpContentProperty = cpa.Name;
                        //Once content property is found, come out of the loop.
                        break;
                    }
                }

                _contentProperties.Add(objectType, lookedUpContentProperty);
            }

            contentPropertyName = _contentProperties[objectType];
            string contentString = String.Empty;

            foreach (MarkupProperty markupProperty in markupObj.Properties)
            {
                if (markupProperty.Name != contentPropertyName)
                {
                    if (markupProperty.IsValueAsString)
                    {
                        contentString = markupProperty.Value as string;
                    }
                    else if (!markupProperty.IsComposite)
                    {
                        string temp = markupProperty.StringValue;

                        if (markupProperty.IsAttached)
                        {
                            string ns1 = _namespaceCache.GetNamespaceUriFor(markupProperty.DependencyProperty.OwnerType);
                            string prefix1 = _namespaceCache.GetDefaultPrefixFor(ns1);

                            if (String.IsNullOrEmpty(prefix1))
                            {
                                writer.WriteAttributeString(markupProperty.Name, temp);
                            }
                            else
                            {
                                writer.WriteAttributeString(markupProperty.Name, ns1, temp);
                            }
                        }
                        else
                        {
                            if (markupProperty.Name == "FontUri" &&
                                (_wpfSettings != null && _wpfSettings.IncludeRuntime))
                            {
                                string fontUri = temp.ToLower();
                                fontUri = fontUri.Replace(_windowsDir, _windowsPath);

                                StringBuilder builder = new StringBuilder();
                                builder.Append("{");
                                builder.Append("svg");
                                builder.Append(":");
                                builder.Append("SvgFontUri ");
                                builder.Append(fontUri);
                                builder.Append("}");

                                writer.WriteAttributeString(markupProperty.Name, builder.ToString());
                            }
                            else
                            {
                                writer.WriteAttributeString(markupProperty.Name, temp);
                            }
                        }
                    }
                    else if (markupProperty.Value.GetType() == _nullType)
                    {
                        if (_nullExtension)
                        {
                            writer.WriteAttributeString(markupProperty.Name, "{x:Null}");
                        }
                    }
                    else
                    {
                        propertyElements.Add(markupProperty);
                    }
                }
                else
                {
                    contentProperty = markupProperty;
                }
            }

            if (contentProperty != null || propertyElements.Count > 0 || contentString != String.Empty)
            {
                foreach (MarkupProperty markupProp in propertyElements)
                {
                    string ns2 = _namespaceCache.GetNamespaceUriFor(markupObj.ObjectType);
                    string prefix2 = null;
                    if (!String.IsNullOrEmpty(ns2))
                    {
                        prefix2 = _namespaceCache.GetDefaultPrefixFor(ns2);
                    }

                    string propElementName = markupObj.ObjectType.Name + "." + markupProp.Name;
                    if (String.IsNullOrEmpty(prefix2))
                    {
                        writer.WriteStartElement(propElementName);
                    }
                    else
                    {
                        writer.WriteStartElement(prefix2, propElementName, ns2);
                    }

                    WriteChildren(writer, markupProp);
                    writer.WriteEndElement();
                }

                if (contentString != String.Empty)
                {
                    writer.WriteValue(contentString);
                }
                else if (contentProperty != null)
                {
                    if (contentProperty.Value is string)
                    {
                        writer.WriteValue(contentProperty.StringValue);
                    }
                    else
                    {
                        WriteChildren(writer, contentProperty);
                    }
                }
            }
            writer.WriteEndElement();
        }

        private void WriteChildren(XmlWriter writer, MarkupProperty markupProp)
        {
            if (!markupProp.IsComposite)
            {
                WriteObject(null, markupProp.Value, writer, false);
            }
            else
            {
                IList collection = markupProp.Value as IList;
                IDictionary dictionary = markupProp.Value as IDictionary;
                if (collection != null)
                {
                    foreach (object obj in collection)
                    {
                        WriteObject(null, obj, writer, false);
                    }
                }
                else if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        WriteObject(key, dictionary[key], writer, false);
                    }
                }
                else
                {
                    WriteObject(null, markupProp.Value, writer, false);
                }
            }
        }

        private void ResolveXmlNamespaces(object obj)
        {
            List<MarkupProperty> propertyElements = new List<MarkupProperty>();
            MarkupProperty contentProperty = null;
            string contentPropertyName = null;
            MarkupObject markupObj = MarkupWriter.GetMarkupObjectFor(obj);
            Type objectType = markupObj.ObjectType;

            string ns = _namespaceCache.GetNamespaceUriFor(objectType);
            if (!String.IsNullOrEmpty(ns))
            {
                string prefix = _namespaceCache.GetDefaultPrefixFor(ns);
                _dicNamespaceMap[ns] = new NamespaceMap(prefix, ns);
            }

            //Look for CPA info in our cache that keeps contentProperty names per Type
            //If it doesn't have an entry, go get the info and store it.
            if (!_contentProperties.ContainsKey(objectType))
            {
                string lookedUpContentProperty = String.Empty;

                foreach (Attribute attr in markupObj.Attributes)
                {
                    ContentPropertyAttribute cpa = attr as ContentPropertyAttribute;
                    if (cpa != null)
                    {
                        lookedUpContentProperty = cpa.Name;
                        //Once content property is found, come out of the loop.
                        break;
                    }
                }

                _contentProperties.Add(objectType, lookedUpContentProperty);
            }

            contentPropertyName = _contentProperties[objectType];

            string contentString = String.Empty;

            foreach (MarkupProperty markupProperty in markupObj.Properties)
            {
                if (markupProperty.Name != contentPropertyName)
                {
                    if (markupProperty.IsValueAsString)
                    {
                        contentString = markupProperty.Value as string;
                    }
                    else if (!markupProperty.IsComposite)
                    {
                        //Bug Fix DX-0120123
                        if (markupProperty.DependencyProperty != null)
                        {
                            string ns1 = _namespaceCache.GetNamespaceUriFor(
                                markupProperty.DependencyProperty.OwnerType);
                            string prefix1 = _namespaceCache.GetDefaultPrefixFor(ns1);

                            if (!String.IsNullOrEmpty(prefix1))
                            {
                                _dicNamespaceMap[ns1] = new NamespaceMap(prefix1, ns1);
                            }
                        }
                    }
                    else if (markupProperty.Value.GetType() == _nullType)
                    {
                    }
                    else
                    {
                        propertyElements.Add(markupProperty);
                    }
                }
                else
                {
                    contentProperty = markupProperty;
                }
            }

            if (contentProperty != null || propertyElements.Count > 0 || contentString != String.Empty)
            {
                foreach (MarkupProperty markupProp in propertyElements)
                {
                    string ns2 = _namespaceCache.GetNamespaceUriFor(markupObj.ObjectType);
                    if (!String.IsNullOrEmpty(ns2))
                    {
                        string prefix2 = _namespaceCache.GetDefaultPrefixFor(ns2);
                        _dicNamespaceMap[ns2] = new NamespaceMap(prefix2, ns2);
                    }
                    ResolveChildXmlNamespaces(markupProp);
                }

                if (contentProperty != null)
                {
                    if (!(contentProperty.Value is String))
                    {
                        ResolveChildXmlNamespaces(contentProperty);
                    }
                }
            }
        }

        private void ResolveChildXmlNamespaces(MarkupProperty markupProp)
        {
            if (!markupProp.IsComposite)
            {
                ResolveXmlNamespaces(markupProp);
            }
            else
            {
                IList collection = markupProp.Value as IList;
                IDictionary dictionary = markupProp.Value as IDictionary;
                if (collection != null)
                {
                    foreach (object obj in collection)
                    {
                        ResolveXmlNamespaces(obj);
                    }
                }
                else if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        ResolveXmlNamespaces(dictionary[key]);
                    }
                }
                else
                {
                    ResolveXmlNamespaces(markupProp.Value);
                }
            }
        }

        #endregion

        #region NamespaceCache Class

        private sealed class NamespaceCache
        {
            public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
            public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
            public const string DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

            public const string ClrNamespace = "clr-namespace:";

            private bool _isFrameworkRoot;
            private Dictionary<string, string> _defaultPrefixes;
            private Dictionary<Assembly, Dictionary<string, string>> _xmlnsDefinitions;
            private CultureInfo _culture;

            public NamespaceCache(CultureInfo culture)
            {
                _culture = culture;

                _defaultPrefixes = new Dictionary<string, string>();
                _xmlnsDefinitions = new Dictionary<Assembly, Dictionary<string, string>>();
            }

            public bool IsFrameworkRoot
            {
                get
                {
                    return _isFrameworkRoot;
                }
                set
                {
                    _isFrameworkRoot = value;
                }
            }

            public string GetDefaultPrefixFor(string uri)
            {
                string uriPrefix;
                _defaultPrefixes.TryGetValue(uri, out uriPrefix);
                if (uriPrefix != null)
                {
                    return uriPrefix;
                }
                uriPrefix = "assembly";
                if (!uri.StartsWith(ClrNamespace, StringComparison.OrdinalIgnoreCase))
                {
                    return uriPrefix;
                }
                string assNamespace = uri.Substring(ClrNamespace.Length, uri.IndexOf(";",
                    StringComparison.OrdinalIgnoreCase) - ClrNamespace.Length);
                if (!String.IsNullOrEmpty(assNamespace))
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < assNamespace.Length; i++)
                    {
                        char ch = assNamespace[i];
                        if ((ch >= 'A') && (ch <= 'Z'))
                        {
                            builder.Append(ch.ToString().ToLower());
                        }
                    }
                    if (builder.Length > 0)
                    {
                        uriPrefix = builder.ToString();
                    }
                }

                return uriPrefix;
            }

            public string GetNamespaceUriFor(Type type)
            {
                string typeNamespace = String.Empty;
                if (type.Namespace == null)
                {
                    return String.Format(_culture, "clr-namespace:;assembly={0}",
                        new object[] { type.Assembly.GetName().Name });
                }
                if (!GetMappingsFor(type.Assembly).TryGetValue(type.Namespace, out typeNamespace))
                {
                    if (!String.Equals(type.Namespace, "System.Windows.Markup.Primitives"))
                    {
                        typeNamespace = String.Format(_culture,
                            "clr-namespace:{0};assembly={1}", new object[] { type.Namespace, 
                                type.Assembly.GetName().Name });
                    }
                }

                return typeNamespace;
            }

            public static string GetAssemblyNameFromType(Type type)
            {
                string[] names = type.Assembly.FullName.Split(',');
                if (names.Length > 0)
                {
                    return names[0];
                }

                return String.Empty;
            }

            private Dictionary<string, string> GetMappingsFor(Assembly assembly)
            {
                Dictionary<string, string> dictionary;
                if (_xmlnsDefinitions.TryGetValue(assembly, out dictionary))
                {
                    return dictionary;
                }
                foreach (XmlnsPrefixAttribute attribute in assembly.GetCustomAttributes(
                    typeof(XmlnsPrefixAttribute), true))
                {
                    _defaultPrefixes[attribute.XmlNamespace] = attribute.Prefix;
                }
                //TODO--PAUL: For now just cheat...
                if (_isFrameworkRoot)
                {
                    _defaultPrefixes[DefaultNamespace] = String.Empty;
                }

                dictionary = new Dictionary<string, string>();
                _xmlnsDefinitions[assembly] = dictionary;
                foreach (XmlnsDefinitionAttribute attribute in assembly.GetCustomAttributes(
                    typeof(XmlnsDefinitionAttribute), true))
                {
                    if (attribute.AssemblyName == null)
                    {
                        string prefix1 = null;
                        string prefix2 = null;
                        string prefix3 = null;
                        if (dictionary.TryGetValue(attribute.ClrNamespace, out prefix1) &&
                            _defaultPrefixes.TryGetValue(prefix1, out prefix2))
                        {
                            _defaultPrefixes.TryGetValue(attribute.XmlNamespace, out prefix3);
                        }
                        if (((prefix1 == null) || (prefix2 == null)) ||
                            ((prefix3 != null) && (prefix2.Length > prefix3.Length)))
                        {
                            dictionary[attribute.ClrNamespace] = attribute.XmlNamespace;
                        }
                    }
                    else
                    {
                        Assembly nextAssembly = Assembly.Load(new AssemblyName(attribute.AssemblyName));
                        if (nextAssembly != null)
                        {
                            GetMappingsFor(nextAssembly)[attribute.ClrNamespace] = attribute.XmlNamespace;
                        }
                    }
                }

                return dictionary;
            }
        }

        #endregion

        #region NamespaceMap Class

        private sealed class NamespaceMap
        {
            private string _prefix;
            private string _xmlNamespace;

            public NamespaceMap(string prefix, string xmlNamespace)
            {
                _prefix = prefix;
                _xmlNamespace = xmlNamespace;
            }

            public string Prefix
            {
                get
                {
                    return _prefix;
                }
                set
                {
                    _prefix = value;
                }
            }

            public string XmlNamespace
            {
                get
                {
                    return _xmlNamespace;
                }
                set
                {
                    _xmlNamespace = value;
                }
            }
        }

        #endregion
    }
}
