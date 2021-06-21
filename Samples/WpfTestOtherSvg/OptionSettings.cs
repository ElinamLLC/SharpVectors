using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

using SharpVectors.Renderers.Wpf;

namespace WpfTestOtherSvg
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

        private string _svgDirectory;
        private string _pngDirectory;
        private string _fontDirectory;
        private string _imageDirectory;

        private uint _selectedNumber;

        private string _emptyImageFile;

        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public OptionSettings()
        {
            _wpfSettings = new WpfDrawingSettings();
            string currentDir = Path.GetFullPath(Path.Combine("..\\", "Tests"));
            if (!Directory.Exists(currentDir))
            {
                Directory.CreateDirectory(currentDir);
            }
            _pngDirectory   = Path.Combine(currentDir, "png");
            _svgDirectory   = Path.Combine(currentDir, "svg");
            _fontDirectory  = Path.Combine(currentDir, "fonts");
            _imageDirectory = Path.Combine(currentDir, "images");
            _emptyImageFile = Path.Combine(currentDir, @"images\empty.png");

            _showInputFile   = false;
            _showOutputFile  = false;
            _recursiveSearch = true;

            _wpfSettings.AddFontLocation(_fontDirectory);
        }

        public OptionSettings(OptionSettings source)
        {
            if (source ==  null)
            {
                return;
            }
            _hidePathsRoot   = source._hidePathsRoot;
            _svgDirectory    = source._svgDirectory;
            _pngDirectory    = source._pngDirectory;
            _fontDirectory   = source._fontDirectory;
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

        public string SvgDirectory
        {
            get {
                return _svgDirectory;
            }
            set {
                bool isChanged = !string.Equals(_svgDirectory, value, StringComparison.OrdinalIgnoreCase);
                _svgDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("SvgDirectory");
                }
            }
        }

        public string PngDirectory
        {
            get {
                return _pngDirectory;
            }
            set {
                bool isChanged = !string.Equals(_pngDirectory, value, StringComparison.OrdinalIgnoreCase);
                _pngDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("PngDirectory");
                }
            }
        }

        public string FontDirectory
        {
            get {
                return _fontDirectory;
            }
            set {
                bool isChanged = !string.Equals(_fontDirectory, value, StringComparison.OrdinalIgnoreCase);
                _fontDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("FontDirectory");
                }
            }
        }

        public string ImageDirectory
        {
            get {
                return _imageDirectory;
            }
            set {
                bool isChanged = !string.Equals(_imageDirectory, value, StringComparison.OrdinalIgnoreCase);
                _imageDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("ImageDirectory");
                }
            }
        }

        public uint SelectedNumber
        {
            get {
                return _selectedNumber;
            }
            set {
                bool isChanged = _selectedNumber != value;
                _selectedNumber = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("SelectedNumber");
                }
            }
        }

        public string EmptyImageFile
        {
            get {
                return _emptyImageFile;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _emptyImageFile = value;
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
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
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

        public bool IsSvgDirectoryChanged(string svgPath)
        {
            if (string.IsNullOrWhiteSpace(svgPath) || string.IsNullOrWhiteSpace(_svgDirectory))
            {
                return true;
            }
            string currentPath = new string(svgPath.ToCharArray());
            if (!currentPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentPath = currentPath + "\\";
            }

            string currentSvgPath = new string(_svgDirectory.ToCharArray());
            if (!_svgDirectory.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentSvgPath = _svgDirectory + "\\";
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
                            case "SvgDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_svgDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _svgDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _svgDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _svgDirectory = optionValue;
                                }
                                break;
                            case "PngDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_pngDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _pngDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _pngDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _pngDirectory = optionValue;
                                }
                                break;
                            case "FontDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_fontDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _fontDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _fontDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _fontDirectory = optionValue;
                                }
                                break;
                            case "ImageDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_imageDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _imageDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _imageDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _imageDirectory = optionValue;
                                }
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
                    else if (string.Equals(optionType, "Number", comparer))
                    {
                        string optionValue = reader.ReadElementContentAsString();
                        switch (optionName)
                        {
                            case "SelectedNumber":
                                _selectedNumber = 1;
                                uint.TryParse(optionValue, out _selectedNumber);
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
            this.SaveOption(writer, "SvgDirectory", this.GetPath(_svgDirectory));
            this.SaveOption(writer, "PngDirectory", this.GetPath(_pngDirectory));
            this.SaveOption(writer, "FontDirectory", this.GetPath(_fontDirectory));
            this.SaveOption(writer, "ImageDirectory", this.GetPath(_imageDirectory));
            this.SaveOption(writer, "SelectedNumber", _selectedNumber);

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
        private void SaveOption(XmlWriter writer, string name, uint value)
        {
            writer.WriteStartElement("option");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "Number");
            writer.WriteString(value.ToString());
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
            if (_svgDirectory != null)
            {
                optSettings._svgDirectory = new string(_svgDirectory.ToCharArray());
            }
            if (_pngDirectory != null)
            {
                optSettings._pngDirectory = new string(_pngDirectory.ToCharArray());
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
