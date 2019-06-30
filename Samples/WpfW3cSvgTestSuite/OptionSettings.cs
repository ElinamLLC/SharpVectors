using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.InteropServices;

using SharpVectors.Renderers.Wpf;

namespace WpfW3cSvgTestSuite
{
    [Serializable]
    public sealed class OptionSettings : ICloneable
    {
        #region Private Fields

        private const string ParentSymbol = "..\\";
        private const string SharpVectors = "SharpVectors";

        private const string FullTestSuite 
            = "https://github.com/ElinamLLC/SharpVectors-TestSuites/raw/master/FullTestSuite.zip";

        [DllImport("Shlwapi.dll", EntryPoint = "PathIsDirectoryEmpty")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDirectoryEmpty([MarshalAs(UnmanagedType.LPStr)]string directory);

        private bool _hidePathsRoot;
        private string _webSuitePath;
        private string _localSuitePath;

        private string _selectedValuePath;

        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public OptionSettings()
        {
            _wpfSettings = new WpfDrawingSettings();
            string currentDir = Path.GetFullPath(@"..\..\FullTestSuite");
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            _localSuitePath = currentDir;
            _webSuitePath = FullTestSuite;
        }

        public OptionSettings(WpfDrawingSettings wpfSettings, string testPath)
        {
            _wpfSettings    = wpfSettings;
            _localSuitePath = testPath;

            if (wpfSettings == null)
            {
                _wpfSettings = new WpfDrawingSettings();
            }
            if (string.IsNullOrWhiteSpace(testPath))
            {
                string currentDir = Path.GetFullPath(@"..\..\FullTestSuite");
                _localSuitePath = currentDir;
            }
            _webSuitePath = FullTestSuite;
            if (!Directory.Exists(_localSuitePath))
            {
                Directory.CreateDirectory(_localSuitePath);
            }
        }

        public OptionSettings(OptionSettings source)
        {
            _hidePathsRoot  = source._hidePathsRoot;
            _webSuitePath   = source._webSuitePath;
            _localSuitePath = source._localSuitePath;
            _wpfSettings    = source._wpfSettings;
        }

        #endregion

        #region Public Properties

        public bool HidePathsRoot
        {
            get {
                return _hidePathsRoot;
            }
            set {
                _hidePathsRoot = value;
            }
        }

        public string WebSuitePath
        {
            get {
                return _webSuitePath;
            }
            set {
                _webSuitePath = value;
            }
        }

        public string LocalSuitePath
        {
            get {
                return _localSuitePath;
            }
            set {
                _localSuitePath = value;
            }
        }

        public string SelectedValuePath
        {
            get {
                return _selectedValuePath;
            }
            set {
                _selectedValuePath = value;
            }
        }

        public WpfDrawingSettings ConversionSettings
        {
            get {
                return _wpfSettings;
            }
            set {
                if (value != null)
                {
                    _wpfSettings = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public string GetPath(string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return inputPath;
            }
            if (_hidePathsRoot)
            {
                Uri fullPath = new Uri(inputPath, UriKind.Absolute);

                // Make relative path to the SharpVectors folder...
                int indexOf = inputPath.IndexOf(SharpVectors, StringComparison.OrdinalIgnoreCase);
                if (indexOf > 0)
                {
                    Uri relRoot = new Uri(inputPath.Substring(0, indexOf), UriKind.Absolute);

                    string relPath = relRoot.MakeRelativeUri(fullPath).ToString();
                    relPath = relPath.Replace('/', '\\');

                    relPath = Uri.UnescapeDataString(relPath);
                    if (!relPath.StartsWith(ParentSymbol, StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = ParentSymbol + relPath;
                    }

                    return relPath;
                }
            }
            return inputPath;
        }

        public void Load(string settingsPath)
        {
            if (string.IsNullOrWhiteSpace(settingsPath) ||
                File.Exists(settingsPath) == false)
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = false;
            settings.IgnoreComments   = true;
            settings.IgnoreProcessingInstructions = true;

            using (XmlReader reader = XmlReader.Create(settingsPath, settings))
            {
                this.Load(reader);
            }
        }

        public void Save(string settingsPath)
        {
            if (string.IsNullOrWhiteSpace(settingsPath))
            {
                return;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent      = true;
            settings.IndentChars = "    ";
            settings.Encoding    = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(settingsPath, settings))
            {
                this.Save(writer);
            }
        }

        public bool IsLocalSuitePathChanged(string currentSuitePath)
        {
            if (string.IsNullOrWhiteSpace(currentSuitePath) ||
                string.IsNullOrWhiteSpace(_localSuitePath))
            {
                return true;
            }
            string currentPath = string.Copy(currentSuitePath);
            if (!currentSuitePath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentPath = currentSuitePath + "\\";
            }

            string suitePath = string.Copy(_localSuitePath);
            if (!_localSuitePath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                suitePath = _localSuitePath + "\\";
            }

            return !(string.Equals(currentPath, suitePath, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsTestSuiteAvailable(string testPath)
        {
            if (string.IsNullOrWhiteSpace(testPath) || Directory.Exists(testPath) == false)
            {
                return false;
            }
            string svgDir = Path.Combine(testPath, "svg");
            if (!Directory.Exists(svgDir) || IsDirectoryEmpty(svgDir) == true)
            {
                return false;
            }
            string pngDir = Path.Combine(testPath, "png");
            if (!Directory.Exists(pngDir) || IsDirectoryEmpty(pngDir) == true)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void Load(XmlReader reader)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "option", comparer))
                {
                    string optionName = reader.GetAttribute("name");
                    string optionType = reader.GetAttribute("type");
                    if (string.Equals(optionType, "String", comparer))
                    {
                        string optionValue = reader.ReadElementContentAsString();

                        switch (optionName)
                        {
                            case "WebSuitePath":
                                _webSuitePath = optionValue;
                                break;
                            case "LocalSuitePath":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = string.Copy(_localSuitePath);
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _localSuitePath = Path.Combine(basePath, optionValue.Replace(ParentSymbol, ""));
                                    }
                                    else
                                    {
                                        _localSuitePath = optionValue;
                                    }
                                }
                                else
                                {
                                    _localSuitePath = optionValue;
                                }
                                break;
                            case "SelectedValuePath":
                                _selectedValuePath = optionValue;
                                break;
                        }
                    }
                    else if (string.Equals(optionType, "Boolean", comparer))
                    {
                        bool optionValue = reader.ReadElementContentAsBoolean();
                        switch (optionName)
                        {
                            case "HidePathsRoot":
                                _hidePathsRoot = optionValue;
                                break;

                            case "TextAsGeometry":
                                _wpfSettings.TextAsGeometry = optionValue;
                                break;
                            case "IncludeRuntime":
                                _wpfSettings.IncludeRuntime = optionValue;
                                break;

                            case "IgnoreRootViewbox":
                                _wpfSettings.IgnoreRootViewbox = optionValue;
                                break;
                            case "EnsureViewboxSize":
                                _wpfSettings.EnsureViewboxSize = optionValue;
                                break;
                            case "EnsureViewboxPosition":
                                _wpfSettings.EnsureViewboxPosition = optionValue;
                                break;
                        }
                    }

                }
            }
        }

        private void Save(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("options");

            this.SaveOption(writer, "HidePathsRoot", _hidePathsRoot);
            this.SaveOption(writer, "WebSuitePath", _webSuitePath);
            this.SaveOption(writer, "LocalSuitePath", this.GetPath(_localSuitePath));
            this.SaveOption(writer, "SelectedValuePath", _selectedValuePath);

            if (_wpfSettings != null)
            {
                this.SaveOption(writer, "TextAsGeometry", _wpfSettings.TextAsGeometry);
                this.SaveOption(writer, "IncludeRuntime", _wpfSettings.IncludeRuntime);

                this.SaveOption(writer, "IgnoreRootViewbox", _wpfSettings.IgnoreRootViewbox);
                this.SaveOption(writer, "EnsureViewboxSize", _wpfSettings.EnsureViewboxSize);
                this.SaveOption(writer, "EnsureViewboxPosition", _wpfSettings.EnsureViewboxPosition);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void SaveOption(XmlWriter writer, string name, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            writer.WriteStartElement("option");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "String");
            writer.WriteString(value);
            writer.WriteEndElement();
        }
        private void SaveOption(XmlWriter writer, string name, bool value)
        {
            writer.WriteStartElement("option");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "Boolean");
            writer.WriteString(value ? "true" : "false");
            writer.WriteEndElement();
        }

        #endregion

        #region  ICloneable Members

        public OptionSettings Clone()
        {
            OptionSettings optSettings = new OptionSettings(this);

            if (_wpfSettings != null)
            {
                optSettings._wpfSettings = _wpfSettings.Clone();
            }
            if (_webSuitePath != null)
            {
                optSettings._webSuitePath = string.Copy(_webSuitePath);
            }
            if (_localSuitePath != null)
            {
                optSettings._localSuitePath = string.Copy(_localSuitePath);
            }

            return optSettings;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
