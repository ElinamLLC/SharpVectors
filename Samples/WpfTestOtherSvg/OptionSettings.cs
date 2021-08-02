using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

using SharpVectors.Renderers.Wpf;
using WpfTestOtherSvg.Handlers;

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

        [DllImport("Shlwapi.dll", EntryPoint = "PathIsDirectoryEmpty")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDirectoryEmpty([MarshalAs(UnmanagedType.LPStr)]string directory);

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        private const string TestCss      = "test.css";
        private const string EmptyImage   = "empty.png";
        private const string CrashImage   = "crash.png";
        private const string TestRunner   = @"SvgRun\SvgRun.exe";

        private const string ParentSymbol = "..\\";
        private const string SharpVectors = "SharpVectors";

        private bool _hidePathsRoot;
        private bool _showInputFile;
        private bool _showOutputFile;
        private bool _recursiveSearch;

        private string _testsDirectory;
        private string _svgDirectory;
        private string _pngDirectory;
        private string _fontsDirectory;
        private string _imagesDirectory;

        private string _cacheDirectory;
        private string _toolsDirectory;

        private string _rsvgDirectory;
        private string _magickDirectory;

        private bool _isMagickInstalled;
        private uint _selectedNumber;

        private string _emptyImageFile;
        private string _crashImageFile;
        private string _testCssFile;
        private string _testRunnerFile;

        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public OptionSettings()
        {
            _wpfSettings = new WpfDrawingSettings();
            _testsDirectory = Path.GetFullPath(Path.Combine("..\\", "Tests"));
            if (!Directory.Exists(_testsDirectory))
            {
                Directory.CreateDirectory(_testsDirectory);
            }
            _pngDirectory    = Path.Combine(_testsDirectory, "png");
            _svgDirectory    = Path.Combine(_testsDirectory, "svg");
            _fontsDirectory  = Path.Combine(_testsDirectory, "fonts");
            _imagesDirectory = Path.Combine(_testsDirectory, "images");

            _cacheDirectory = Path.GetFullPath(Path.Combine("..\\", "Cache"));
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }

            _toolsDirectory = Path.GetFullPath(Path.Combine("..\\", "Tools"));
            if (!Directory.Exists(_toolsDirectory))
            {
                Directory.CreateDirectory(_toolsDirectory);
            }

            _crashImageFile  = Path.Combine(_testsDirectory, CrashImage);
            _emptyImageFile  = Path.Combine(_imagesDirectory, EmptyImage);

            _testCssFile     = Path.Combine(_testsDirectory, TestCss);
            _testRunnerFile  = Path.Combine(_toolsDirectory, TestRunner);

            _magickDirectory = MagickTestHandler.GetInstalledDir();
            if (!string.IsNullOrWhiteSpace(_magickDirectory) && Directory.Exists(_magickDirectory))
            {
                _isMagickInstalled = File.Exists(Path.Combine(_magickDirectory, MagickTestHandler.FileName));
            }

            _showInputFile   = false;
            _showOutputFile  = false;
            _recursiveSearch = true;

//            _wpfSettings.AddFontLocation(_fontDirectory);
        }

        public OptionSettings(OptionSettings source)
        {
            if (source ==  null)
            {
                return;
            }
            _hidePathsRoot     = source._hidePathsRoot;
            _isMagickInstalled = source._isMagickInstalled;
            _svgDirectory      = source._svgDirectory;
            _pngDirectory      = source._pngDirectory;
            _fontsDirectory    = source._fontsDirectory;
            _imagesDirectory   = source._imagesDirectory;

            _rsvgDirectory     = source._rsvgDirectory;
            _magickDirectory   = source._magickDirectory;

            _showInputFile     = source._showInputFile;
            _showOutputFile    = source._showOutputFile;
            _recursiveSearch   = source._recursiveSearch;
            _testCssFile       = source._testCssFile;
            _testRunnerFile    = source._testRunnerFile;
            _emptyImageFile    = source._emptyImageFile;
            _crashImageFile    = source._crashImageFile;
            _wpfSettings       = source._wpfSettings;
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

        public bool IsMagickInstalled
        {
            get {
                return _isMagickInstalled;
            }
            set {
                bool isChanged = (_isMagickInstalled != value);
                _isMagickInstalled = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("IsMagickInstalled");
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

        public string FontsDirectory
        {
            get {
                return _fontsDirectory;
            }
            set {
                bool isChanged = !string.Equals(_fontsDirectory, value, StringComparison.OrdinalIgnoreCase);
                _fontsDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("FontDirectory");
                }
            }
        }

        public string ImagesDirectory
        {
            get {
                return _imagesDirectory;
            }
            set {
                bool isChanged = !string.Equals(_imagesDirectory, value, StringComparison.OrdinalIgnoreCase);
                _imagesDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("ImageDirectory");
                }
            }
        }

        public string TestsDirectory
        {
            get {
                return _testsDirectory;
            }
            set {
                bool isChanged = !string.Equals(_testsDirectory, value, StringComparison.OrdinalIgnoreCase);
                _testsDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("TestDirectory");
                }
            }
        }

        public string ToolsDirectory
        {
            get {
                return _toolsDirectory;
            }
            set {
                bool isChanged = !string.Equals(_toolsDirectory, value, StringComparison.OrdinalIgnoreCase);
                _toolsDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("ToolsDirectory");
                }
            }
        }

        public string CacheDirectory
        {
            get {
                return _cacheDirectory;
            }
            set {
                bool isChanged = !string.Equals(_cacheDirectory, value, StringComparison.OrdinalIgnoreCase);
                _cacheDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("CacheDirectory");
                }
            }
        }

        public string RsvgDirectory
        {
            get {
                return _rsvgDirectory;
            }
            set {
                bool isChanged = !string.Equals(_rsvgDirectory, value, StringComparison.OrdinalIgnoreCase);
                _rsvgDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("RsvgDirectory");
                }
            }
        }

        public string MagickDirectory
        {
            get {
                return _magickDirectory;
            }
            set {
                bool isChanged = !string.Equals(_magickDirectory, value, StringComparison.OrdinalIgnoreCase);
                _magickDirectory = value;

                if (isChanged)
                {
                    this.RaisePropertyChanged("MagickDirectory");
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

        public string CrashImageFile
        {
            get {
                return _crashImageFile;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _crashImageFile = value;
                }
            }
        }

        public string TestCssFile
        {
            get {
                return _testCssFile;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _testCssFile = value;
                }
            }
        }

        public string TestRunnerFile
        {
            get {
                return _testRunnerFile;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _testRunnerFile = value;
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

        public bool IsTestAvailable()
        {
            if (string.IsNullOrWhiteSpace(_testsDirectory) || Directory.Exists(_testsDirectory) == false)
            {
                return false;
            }
            if (!Directory.Exists(_svgDirectory) || IsDirectoryEmpty(_svgDirectory) == true)
            {
                return false;
            }
            if (!Directory.Exists(_pngDirectory) || IsDirectoryEmpty(_pngDirectory) == true)
            {
                return false;
            }
            if (!Directory.Exists(_fontsDirectory) || IsDirectoryEmpty(_fontsDirectory) == true)
            {
                return false;
            }
            if (!Directory.Exists(_imagesDirectory) || IsDirectoryEmpty(_imagesDirectory) == true)
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
                var selectedName   = string.Empty;
                var dirInfo        = new DirectoryInfo(folderPath);

                var firstFileName = dirInfo.EnumerateFiles()
                    .Select(f => f.Name)
                    .FirstOrDefault(name => !string.Equals(name, "Thumbs.db", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(firstFileName))
                {
                    selectedIsFile = true;
                    selectedName   = firstFileName;
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
                            selectedName   = firstDirName;
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
                SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out nativeFile, 0, out psfgaoOut);
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
                                    var inputPath = new string(_fontsDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _fontsDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _fontsDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _fontsDirectory = optionValue;
                                }
                                break;
                            case "ImageDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_imagesDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _imagesDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _imagesDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _imagesDirectory = optionValue;
                                }
                                break;
                            case "RsvgDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_rsvgDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _rsvgDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _rsvgDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _rsvgDirectory = optionValue;
                                }
                                break;
                            case "MagickDirectory":
                                if (optionValue.StartsWith(ParentSymbol, comparer))
                                {
                                    var inputPath = new string(_magickDirectory.ToCharArray());
                                    int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                    if (indexOf > 0)
                                    {
                                        var basePath = inputPath.Substring(0, indexOf);
                                        _magickDirectory = Path.Combine(basePath, optionValue.Replace(ParentSymbol, string.Empty));
                                    }
                                    else
                                    {
                                        _magickDirectory = optionValue;
                                    }
                                }
                                else
                                {
                                    _magickDirectory = optionValue;
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
            this.SaveOption(writer, "FontDirectory", this.GetPath(_fontsDirectory));
            this.SaveOption(writer, "ImageDirectory", this.GetPath(_imagesDirectory));

            this.SaveOption(writer, "RsvgDirectory", this.GetPath(_rsvgDirectory));
            this.SaveOption(writer, "MagickDirectory", this.GetPath(_magickDirectory));

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
