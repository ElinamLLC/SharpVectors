using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace GdiW3cSvgTestSuite
{
    public enum DockingTheme
    {
        LightTheme = 0,
        BlueTheme  = 1,
        DarkTheme  = 2
    }

    [Serializable]
    public sealed class OptionSettings : ICloneable
    {
        #region Public Fields

        public const string SettingsFileName = "SvgTestSettings.xml";

        #endregion

        #region Private Interop Methods

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder,
            uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name,
            IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        #endregion

        #region Private Fields

        private const string ParentSymbol = "..\\";
        private const string SharpVectors = "SharpVectors";

        private const string FullTestSuite 
            = "https://github.com/ElinamLLC/SharpVectors-TestSuites/raw/master/FullTestSuite.zip";

        [DllImport("Shlwapi.dll", EntryPoint = "PathIsDirectoryEmpty")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsDirectoryEmpty([MarshalAs(UnmanagedType.LPStr)]string directory);

        private static readonly XmlSerializerNamespaces EmptyXmlSerializerNamespace = new XmlSerializerNamespaces(
            new XmlQualifiedName[] { new XmlQualifiedName("") });

        private bool _hidePathsRoot;
        private string _webSuitePath;
        private string _localSuitePath;

        private string _selectedValuePath;

        private IList<SvgTestSuite> _testSuites;

        private WindowPosition _winPosition;

        private DockingTheme _theme;

        #endregion

        #region Constructors and Destructor

        public OptionSettings()
            : this(string.Empty)
        {
        }

        public OptionSettings(string testPath)
        {
            _localSuitePath = testPath;
            _testSuites     = SvgTestSuite.Create();

            // For the start the default is selected
            var selectedSuite = SvgTestSuite.GetDefault(_testSuites);
            if (selectedSuite != null)
            {
                if (string.IsNullOrWhiteSpace(testPath))
                {
                    _localSuitePath = selectedSuite.LocalSuitePath;
                }
                _webSuitePath = selectedSuite.WebSuitePath;
            }

            if (!Directory.Exists(_localSuitePath))
            {
                Directory.CreateDirectory(_localSuitePath);
            }
            _theme = DockingTheme.LightTheme;
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
            _theme          = source._theme;
            _winPosition    = source._winPosition;
            _testSuites     = source._testSuites;
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

        public SvgTestSuite SelectedTestSuite
        {
            get {
                if (_testSuites != null && _testSuites.Count != 0)
                {
                    return SvgTestSuite.GetSelected(_testSuites);
                }
                return null;
            }
        }

        public IList<SvgTestSuite> TestSuites
        {
            get {
                return _testSuites;
            }
        }

        public DockingTheme Theme
        {
            get {
                return _theme;
            }
            set {
                _theme = value;
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

        public void Load(string settingsPath, Form mainForm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(settingsPath) || File.Exists(settingsPath) == false)
                {
                    return;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace             = false;
                settings.IgnoreComments               = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(settingsPath, settings))
                {
                    this.Load(reader, mainForm);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public void Save(string settingsPath, Form mainForm)
        {
            try
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
                    this.Save(writer, mainForm);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public bool IsLocalSuitePathChanged(string currentSuitePath)
        {
            if (string.IsNullOrWhiteSpace(currentSuitePath) ||
                string.IsNullOrWhiteSpace(_localSuitePath))
            {
                return true;
            }
            string currentPath = new string(currentSuitePath.ToCharArray());
            if (!currentSuitePath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                currentPath = currentSuitePath + "\\";
            }

            string suitePath = new string(_localSuitePath.ToCharArray());
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

        private void Load(XmlReader reader, Form mainForm)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;

            List<SvgTestSuite> testSuites = new List<SvgTestSuite>(SvgTestSuite.TestSuiteCount);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "option", comparer))
                    {
                        string optionName = reader.GetAttribute("name");
                        string optionType = reader.GetAttribute("type");
                        if (string.Equals(optionType, "String", comparer))
                        {
                            string optionValue = reader.ReadElementContentAsString();

                            switch (optionName)
                            {
                                //case "WebSuitePath":
                                //    _webSuitePath = optionValue;
                                //    break;
                                //case "LocalSuitePath":
                                //    if (optionValue.StartsWith(ParentSymbol, comparer))
                                //    {
                                //        var inputPath = string.Copy(_localSuitePath);
                                //        int indexOf = inputPath.IndexOf(SharpVectors, comparer);

                                //        if (indexOf > 0)
                                //        {
                                //            var basePath    = inputPath.Substring(0, indexOf);
                                //            _localSuitePath = Path.Combine(basePath, optionValue.Replace(ParentSymbol, ""));
                                //        }
                                //        else
                                //        {
                                //            _localSuitePath = optionValue;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        _localSuitePath = optionValue;
                                //    }

                                //    // Ignore old test suite directory, if found
                                //    if (string.IsNullOrWhiteSpace(_localSuitePath) || 
                                //        !this.IsLocalSuitePathChanged(Path.GetFullPath(TestsSvg)))
                                //    {
                                //        _localSuitePath = Path.GetFullPath(TestsSvg10);
                                //    }
                                //    break;
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
                            }
                        }
                        else if (string.Equals(optionType, "Other", comparer))
                        {
                            string optionValue = reader.ReadElementContentAsString();
                            switch (optionName)
                            {
                                case "Theme":
                                    _theme = (DockingTheme)Enum.Parse(typeof(DockingTheme), optionValue, true);
                                    break;
                            }
                        }
                    }
                    else if (string.Equals(reader.Name, "testSuite", comparer))
                    {
                        if (!reader.IsEmptyElement)
                        {
                            SvgTestSuite testSuite = new SvgTestSuite(reader);
                            if (testSuite.IsValid())
                            {
                                testSuites.Add(testSuite);
                            }
                        }
                    }
                    else if (string.Equals(reader.Name, "placements", comparer))
                    {
                        if (reader.IsEmptyElement == false)
                        {
                            if (reader.ReadToFollowing("WindowPosition"))
                            {
                                var xs = new XmlSerializer(typeof(WindowPosition));
                                _winPosition = xs.Deserialize(reader) as WindowPosition;
                            }
                        }
                    }
                }
            }

            if (testSuites.Count == SvgTestSuite.TestSuiteCount)
            {
                var selectedSuite = SvgTestSuite.GetSelected(testSuites);
                if (selectedSuite != null)
                {
                    _localSuitePath = selectedSuite.LocalSuitePath;
                    _webSuitePath = selectedSuite.WebSuitePath;

                    _testSuites = testSuites;
                }
            }

            if (mainForm != null && _winPosition != null)
            {
                try
                {
                    switch (_winPosition.WindowState)
                    {
                        case FormWindowState.Maximized:
                            mainForm.Location      = _winPosition.MaximisedPoint;
                            mainForm.StartPosition = FormStartPosition.Manual;
                            break;
                        case FormWindowState.Normal:
                            if (_winPosition.IsIdenticalScreen())
                            {
                                mainForm.Location = _winPosition.Location;
                                mainForm.Size = _winPosition.Size;
                                mainForm.StartPosition = FormStartPosition.Manual;
                            }
                            break;
                        case FormWindowState.Minimized:
                            _winPosition.WindowState = FormWindowState.Normal;
                            break;
                        default:
                            break;
                    }
                    mainForm.WindowState = _winPosition.WindowState;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        private void Save(XmlWriter writer, Form mainForm)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("options");

            this.SaveOption(writer, "HidePathsRoot", _hidePathsRoot);
            //this.SaveOption(writer, "WebSuitePath", _webSuitePath);
            //this.SaveOption(writer, "LocalSuitePath", this.GetPath(_localSuitePath));
            this.SaveOption(writer, "SelectedValuePath", _selectedValuePath);
            this.SaveOption(writer, "Theme", _theme);

            if (_testSuites != null && _testSuites.Count != 0)
            {
                var selectedSuite = SvgTestSuite.GetSelected(_testSuites);
                if (selectedSuite != null)
                {
                    selectedSuite.LocalSuitePath = _localSuitePath;
                    selectedSuite.WebSuitePath = _webSuitePath;
                }

                writer.WriteStartElement("testSuites");

                foreach (var testSuite in _testSuites)
                {
                    testSuite.WriteXml(writer);
                }

                writer.WriteEndElement();
            }

            try
            {
                _winPosition = new WindowPosition();
                if (mainForm != null)
                {
                    _winPosition.Location    = mainForm.Location;
                    _winPosition.Size        = mainForm.Size;
                    _winPosition.WindowState = mainForm.WindowState;
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        _winPosition.WorkingAreas.Add(screen.WorkingArea);
                    }
                }

                this.SaveWindowPosition(writer);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void SaveOption(XmlWriter writer, string name, DockingTheme theme)
        {
            writer.WriteStartElement("option");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "Other");
            writer.WriteString(theme.ToString());
            writer.WriteEndElement();
        }

        private void SaveWindowPosition(XmlWriter writer)
        {
            writer.WriteStartElement("placements");
            if (_winPosition != null)
            {
                var xs = new XmlSerializer(typeof(WindowPosition));
                xs.Serialize(writer, _winPosition, EmptyXmlSerializerNamespace);
            }
            writer.WriteEndElement();
        }

        #endregion

        #region  ICloneable Members

        public OptionSettings Clone()
        {
            OptionSettings clonedSettings = new OptionSettings(this);

            if (_webSuitePath != null)
            {
                clonedSettings._webSuitePath = new string(_webSuitePath.ToCharArray());
            }
            if (_localSuitePath != null)
            {
                clonedSettings._localSuitePath = new string(_localSuitePath.ToCharArray());
            }
            if (_winPosition != null)
            {
                clonedSettings._winPosition = _winPosition.Clone();
            }
            if (_testSuites != null)
            {
                int itemCount = _testSuites.Count;
                List<SvgTestSuite> clonedTestSuites = new List<SvgTestSuite>(itemCount);
                for (int i = 0; i < itemCount; i++)
                {
                    clonedTestSuites.Add(_testSuites[i].Clone());
                }
                clonedSettings._testSuites = clonedTestSuites;
            }

            return clonedSettings;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region Private WindowPosition

        public sealed class WindowPosition : ICloneable
        {
            private Size _winSize;
            private Point _winLocation;
            private Point _maximizedPoint;
            private FormWindowState _winState;
            private List<Rectangle> _workingAreas;

            public WindowPosition()
            {
                _workingAreas = new List<Rectangle>();
            }

            public WindowPosition(WindowPosition source)
                : this()
            {
                if (source != null)
                {
                    _winSize        = source._winSize;
                    _winLocation    = source._winLocation;
                    _maximizedPoint = source._maximizedPoint;
                    _winState       = source._winState;
                    _workingAreas   = source._workingAreas;
                }
            }

            public Point Location
            {
                get { return _winLocation; }
                set { _winLocation = value; }
            }

            public Size Size
            {
                get { return _winSize; }
                set { _winSize = value; }
            }

            public FormWindowState WindowState
            {
                get { return _winState; }
                set { _winState = value; }
            }

            public Point MaximisedPoint
            {
                get { return _maximizedPoint; }
                set { _maximizedPoint = value; }
            }

            public List<Rectangle> WorkingAreas
            {
                get { return _workingAreas; }
                set { _workingAreas = value; }
            }

            public bool IsIdenticalScreen()
            {
                if (_workingAreas == null || _workingAreas.Count != Screen.AllScreens.Length)
                {
                    return false;
                }
                for (int i = 0; i < _workingAreas.Count; i++)
                {
                    if (_workingAreas[i] == Screen.AllScreens[i].WorkingArea)
                    {
                        return true;
                    }
                }
                return false;
            }

            public WindowPosition Clone()
            {
                WindowPosition cloned = new WindowPosition(this);

                return cloned;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }
        }

        #endregion
    }
}
