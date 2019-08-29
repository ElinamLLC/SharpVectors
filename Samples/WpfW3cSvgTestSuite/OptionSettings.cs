using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

using SharpVectors.Renderers.Wpf;

namespace WpfW3cSvgTestSuite
{
    [Serializable]
    public sealed class OptionSettings : INotifyPropertyChanged, ICloneable
    {
        #region Public Fields

        private const string TestsSvg   = @"..\..\FullTestSuite";
        private const string TestsSvg10 = @"..\..\W3cSvgTestSuites\Svg10";
        private const string TestsSvg11 = @"..\..\W3cSvgTestSuites\Svg11";
        private const string TestsSvg12 = @"..\..\W3cSvgTestSuites\Svg12";

        #endregion

        #region Private Interop Methods

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder,
            uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name,
            IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

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
            string currentDir = Path.GetFullPath(TestsSvg10);
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
                string currentDir = Path.GetFullPath(TestsSvg10);
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
            if (source == null)
            {
                return;
            }
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

        public static void OpenFolderAndSelectItem(string folderPath, string file)
        {
            if (string.IsNullOrEmpty(folderPath) || Directory.Exists(folderPath) == false)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(file))
            {
                var selectedIsFile = false;
                var selectedName = string.Empty;
                var dirInfo = new DirectoryInfo(folderPath);

                var firstFileName = dirInfo.EnumerateFiles()
                    .Select(f => f.Name)
                    .FirstOrDefault(name => !string.Equals(name, "Thumbs.db", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(firstFileName))
                {
                    selectedIsFile = true;
                    selectedName = firstFileName;
                }
                else
                {
                    if (!IsDirectoryEmpty(dirInfo.FullName))
                    {
                        var firstDirName = dirInfo.EnumerateDirectories()
                            .Select(f => f.Name)
                            .FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(firstDirName))
                        {
                            selectedIsFile = false;
                            selectedName = firstDirName;
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(selectedName))
                {
                    if (selectedIsFile)
                    {
                        file = selectedName;
                    }
                    else
                    {
                        folderPath = Path.Combine(folderPath, selectedName);
                    }
                }
            }

            IntPtr nativeFolder;
            uint psfgaoOut;
            SHParseDisplayName(folderPath, IntPtr.Zero, out nativeFolder, 0, out psfgaoOut);

            if (nativeFolder == IntPtr.Zero)
            {
                // Log error, can't find folder
                return;
            }

            IntPtr nativeFile = IntPtr.Zero;
            if (!string.IsNullOrWhiteSpace(file))
            {
                SHParseDisplayName(Path.Combine(folderPath, file),
                    IntPtr.Zero, out nativeFile, 0, out psfgaoOut);
            }

            IntPtr[] fileArray;
            if (nativeFile == IntPtr.Zero)
            {
                // Open the folder without the file selected if we can't find the file
                fileArray = new IntPtr[0];
            }
            else
            {
                fileArray = new IntPtr[] { nativeFile };
            }

            SHOpenFolderAndSelectItems(nativeFolder, (uint)fileArray.Length, fileArray, 0);

            Marshal.FreeCoTaskMem(nativeFolder);
            if (nativeFile != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(nativeFile);
            }
        }

        #endregion

        #region Private Methods

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

                                // Ignore old test suite directory, if found
                                if (string.IsNullOrWhiteSpace(_localSuitePath) ||
                                    !this.IsLocalSuitePathChanged(Path.GetFullPath(TestsSvg)))
                                {
                                    _localSuitePath = Path.GetFullPath(TestsSvg10);
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
