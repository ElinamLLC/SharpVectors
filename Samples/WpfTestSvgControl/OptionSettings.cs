using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

using SharpVectors.Renderers.Wpf;

namespace WpfTestSvgControl
{
    [Serializable]
    public sealed class OptionSettings : INotifyPropertyChanged, ICloneable
    {
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

        [DllImport("Shlwapi.dll", EntryPoint = "PathIsDirectoryEmpty")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDirectoryEmpty([MarshalAs(UnmanagedType.LPStr)]string directory);

        private bool _hidePathsRoot;
        private bool _showInputFile;
        private bool _showOutputFile;
        private bool _recursiveSearch;

        private string _defaultSvgPath;
        private string _currentSvgPath;

        private string _selectedValuePath;

        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public OptionSettings()
        {
            _wpfSettings = new WpfDrawingSettings();
            string currentDir = Path.GetFullPath(@".\Samples");
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            _currentSvgPath = currentDir;
            _defaultSvgPath = currentDir;

            _showInputFile   = false;
            _showOutputFile  = false;
            _recursiveSearch = true;
        }

        public OptionSettings(WpfDrawingSettings wpfSettings, string testPath)
        {
            _wpfSettings    = wpfSettings;
            _currentSvgPath = testPath;

            _showInputFile   = false;
            _showOutputFile  = false;
            _recursiveSearch = true;

            if (wpfSettings == null)
            {
                _wpfSettings = new WpfDrawingSettings();
            }
            if (string.IsNullOrWhiteSpace(testPath))
            {
                string currentDir = Path.GetFullPath(@".\Samples");
                _currentSvgPath = currentDir;
            }
            if (!Directory.Exists(_currentSvgPath))
            {
                Directory.CreateDirectory(_currentSvgPath);
            }
            _defaultSvgPath = _currentSvgPath;
        }

        public OptionSettings(OptionSettings source)
        {
            if (source ==  null)
            {
                return;
            }
            _hidePathsRoot   = source._hidePathsRoot;
            _defaultSvgPath  = source._defaultSvgPath;
            _currentSvgPath  = source._currentSvgPath;
            _showInputFile   = source._showInputFile;
            _showOutputFile  = source._showOutputFile;
            _recursiveSearch = source._recursiveSearch;
            _wpfSettings     = source._wpfSettings;
        }

        #endregion

        #region Public Properties

        public bool HidePathsRoot
        {
            get {
                return _hidePathsRoot;
            }
            set {
                bool isChanged = (_hidePathsRoot != value);
                _hidePathsRoot = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("HidePathsRoot");
                }
            }
        }

        public bool ShowInputFile
        {
            get {
                return _showInputFile;
            }
            set {
                bool isChanged = (_showInputFile != value);
                _showInputFile = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("ShowInputFile");
                }
            }
        }

        public bool ShowOutputFile
        {
            get {
                return _showOutputFile;
            }
            set {
                bool isChanged = (_showOutputFile != value);
                _showOutputFile = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("ShowOutputFile");
                }
            }
        }

        public bool RecursiveSearch
        {
            get {
                return _recursiveSearch;
            }
            set {
                bool isChanged = (_recursiveSearch != value);
                _recursiveSearch = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("RecursiveSearch");
                }
            }
        }

        public string DefaultSvgPath
        {
            get {
                return _defaultSvgPath;
            }
            set {
                bool isChanged = !string.Equals(_defaultSvgPath, value, StringComparison.OrdinalIgnoreCase);
                _defaultSvgPath = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("DefaultSvgPath");
                }
            }
        }

        public string CurrentSvgPath
        {
            get {
                return _currentSvgPath;
            }
            set {
                bool isChanged = !string.Equals(_currentSvgPath, value, StringComparison.OrdinalIgnoreCase);
                _currentSvgPath = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("CurrentSvgPath");
                }
            }
        }

        public string SelectedValuePath
        {
            get {
                return _selectedValuePath;
            }
            set {
                bool isChanged = !string.Equals(_defaultSvgPath, value, StringComparison.OrdinalIgnoreCase);
                _selectedValuePath = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("SelectedValuePath");
                }
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
                    bool isChanged = (_wpfSettings != value);

                    _wpfSettings = value;

                    if (isChanged)
                    {
                        this.RaisePropertyChanged("ConversionSettings");
                    }
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

        public bool IsCurrentSvgPathChanged(string svgPath)
        {
            if (string.IsNullOrWhiteSpace(svgPath) ||
                string.IsNullOrWhiteSpace(_currentSvgPath))
            {
                return true;
            }
            string currentPath = new string(svgPath.ToCharArray());
            if (!currentPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentPath = currentPath + "\\";
            }

            string currentSvgPath = new string(_currentSvgPath.ToCharArray());
            if (!_currentSvgPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentSvgPath = _currentSvgPath + "\\";
            }

            return !(string.Equals(currentPath, currentSvgPath, StringComparison.OrdinalIgnoreCase));
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
                            case "DefaultSvgPath":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_defaultSvgPath.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _defaultSvgPath = Path.Combine(basePath, optionValue.Replace(ParentSymbol, ""));
                                    }
                                    else
                                    {
                                        _defaultSvgPath = optionValue;
                                    }
                                }
                                else
                                {
                                    _defaultSvgPath = optionValue;
                                }
                                break;
                            case "CurrentSvgPath":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_currentSvgPath.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _currentSvgPath = Path.Combine(basePath, optionValue.Replace(ParentSymbol, ""));
                                    }
                                    else
                                    {
                                        _currentSvgPath = optionValue;
                                    }
                                }
                                else
                                {
                                    _currentSvgPath = optionValue;
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
                            case "ShowInputFile":
                                _showInputFile = optionValue;
                                break;
                            case "ShowOutputFile":
                                _showOutputFile = optionValue;
                                break;
                            case "RecursiveSearch":
                                _recursiveSearch = optionValue;
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
            this.SaveOption(writer, "ShowInputFile", _showInputFile);
            this.SaveOption(writer, "ShowOutputFile", _showOutputFile);
            this.SaveOption(writer, "RecursiveSearch", _recursiveSearch);
            this.SaveOption(writer, "DefaultSvgPath", this.GetPath(_defaultSvgPath));
            this.SaveOption(writer, "CurrentSvgPath", this.GetPath(_currentSvgPath));
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
            if (_defaultSvgPath != null)
            {
                optSettings._defaultSvgPath = new string(_defaultSvgPath.ToCharArray());
            }
            if (_currentSvgPath != null)
            {
                optSettings._currentSvgPath = new string(_currentSvgPath.ToCharArray());
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
