using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Win32;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

using IoPath = System.IO.Path;
using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfTestOtherSvg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Public Fields

        public const string AppTitle        = "SharpVectors: WPF Test Application";
        public const string AppErrorTitle   = "SharpVectors: WPF Test Application - Error";
        public const string SvgTestSettings = "SvgTestSettings.xml";

        public const string SvgOldFileName   = "SvgTests.old";
        public const string SvgTestsFileName = "SvgTests.xml";

        #endregion

        #region Private Fields

        private const int LeftPane       = 410;
        private const int LeftBottomPane = 220;

        private const string SvgFilePattern  = "*.svg*";

        private delegate void FileChangedToUIThread(FileSystemEventArgs e);

        private bool _isTreeModified;
        private bool _leftSplitterChanging;
        private bool _isBottomSplitterChanging;

        private bool _isTreeChangedPending;

        private bool _isShown;
        private bool _isRecursiveSearch;

        private string _sourceDir;
        private string _testFilePath;
        private string _testSettingsPath;

        private string _svgFilePath;

        private TestsPage _testsPage;
        private TestsOtherPage _otherPage;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;
        private DebugPage _debugPage;
        private SettingsPage _settingsPage;

        private ImageSource _folderClose;
        private ImageSource _folderOpen;
        private ImageSource _fileThumbnail;

        private OptionSettings _optionSettings;

        private uint _testTotal; 
        private uint _testSuccesses; 
        private uint _testFailures;
        private uint _testPartials;
        private uint _testUnknowns; 

        private IList<SvgTestItem> _testItems;

        #endregion

        #region Constructors and Destructor

        public MainWindow()
        {
            InitializeComponent();

            leftExpander.Expanded    += OnLeftExpanderExpanded;
            leftExpander.Collapsed   += OnLeftExpanderCollapsed;
            leftSplitter.MouseMove   += OnLeftSplitterMove;  

            bottomExpander.Expanded  += OnBottomExpanderExpanded;
            bottomExpander.Collapsed += OnBottomExpanderCollapsed;
            bottomSplitter.MouseMove += OnBottomSplitterMove;

            this.Loaded   += OnWindowLoaded;
            this.Unloaded += OnWindowUnloaded;
            this.Closing  += OnWindowClosing;

            _optionSettings = new OptionSettings();
            _testSettingsPath = IoPath.GetFullPath(IoPath.Combine("..\\", SvgTestSettings));
            if (!string.IsNullOrWhiteSpace(_testSettingsPath) && File.Exists(_testSettingsPath))
            {
                _optionSettings.Load(_testSettingsPath);
            }

            _optionSettings.PropertyChanged += OnSettingsPropertyChanged;

            try
            {
                _folderClose   = this.GetImage(new Uri("Images/FolderClose.svg", UriKind.Relative));
                _folderOpen    = this.GetImage(new Uri("Images/FolderOpen.svg", UriKind.Relative));
                _fileThumbnail = this.GetImage(new Uri("Images/SvgLogoBasic.svg", UriKind.Relative));
            }
            catch (Exception ex)
            {
                _folderClose = null;
                _folderOpen = null;

                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _testTotal     = 0;
            _testUnknowns  = 0;
            _testFailures  = 0;
            _testSuccesses = 0;
            _testPartials  = 0;
            _testItems     = new List<SvgTestItem>((int)SvgTestItem.TestCount);

            this.DataContext = this;
        }

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public uint TestSuccesses
        {
            get {
                return _testSuccesses;
            }
            set {
                this.SetProperty(ref _testSuccesses, value, "TestSuccesses");
            }
        }

        public uint TestTotal
        {
            get {
                return _testTotal;
            }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                this.SetProperty(ref _testTotal, value, "TestTotal");
            }
        }

        public uint TestFailures
        {
            get {
                return _testFailures;
            }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                this.SetProperty(ref _testFailures, value, "TestFailures");
            }
        }

        public uint TestPartials
        {
            get {
                return _testPartials;
            }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                this.SetProperty(ref _testPartials, value, "TestPartials");
            }
        }

        public uint TestUnknowns
        {
            get {
                return _testUnknowns;
            }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                this.SetProperty(ref _testUnknowns, value, "TestUnknowns");
            }
        }

        public OptionSettings OptionSettings
        {
            get {
                return _optionSettings;
            }
            set {
                if (value != null)
                {
                    _optionSettings = value;

                    if (_optionSettings.IsSvgDirectoryChanged(_sourceDir))
                    {
                        this.InitTreeView(_optionSettings.SvgDirectory);
                    }
                    else if (_isRecursiveSearch != _optionSettings.RecursiveSearch)
                    {
                        this.InitTreeView(_optionSettings.SvgDirectory);
                    }
                }
            }
        }

        public TestsPage TestsPage
        {
            get {
                return _testsPage;
            }
        }

        public TestsOtherPage OtherPage
        {
            get {
                return _otherPage;
            }
        }

        public SvgPage SvgPage
        {
            get {
                return _svgPage;
            }
        }

        public XamlPage XamlPage
        {
            get {
                return _xamlPage;
            }
        }

        public DebugPage DebugPage
        {
            get {
                return _debugPage;
            }
        }

        public SettingsPage SettingsPage
        {
            get {
                return _settingsPage;
            }
        }

        #endregion

        #region Public Methods

        public void SaveTests()
        {
            this.SaveSettings();
            this.SaveTreeView(this.GetTestFilePath());
        }

        public void ApplyState(SvgTestState testState, bool moveNext = true)
        {
            TreeViewItem selTreeItem = treeView.SelectedItem as TreeViewItem;
            if (selTreeItem == null || selTreeItem.Tag == null)
            {
                return;
            }

            var testItem = selTreeItem.Tag as SvgTestItem;
            if (testItem == null)
            {
                return;
            }

            bottomExpander.IsExpanded = true;

            if (testItem.State != testState)
            {
                stateComboBox.SelectedIndex = (int)testState;
                testComment.Text = testItem.Comment;

                this.OnApplyTestState(selTreeItem);
            }

            if (moveNext && testItem.Tag != null)
            {
                int nextIndex = (int)testItem.Number;
                if (nextIndex >= _testItems.Count)
                {
                    return;
                }
                var nextItem = _testItems[nextIndex];
                TreeViewItem nextTreeItem = nextItem.Tag as TreeViewItem;
                if (nextTreeItem == null || nextTreeItem.Tag == null)
                {
                    return;
                }
                nextTreeItem.IsSelected = true;
                nextTreeItem.BringIntoView();

                TreeViewItem parentTreeItem = nextItem.Parent as TreeViewItem;
                if (parentTreeItem == null)
                {
                    return;
                }
                parentTreeItem.IsExpanded = true;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            this.Width = Math.Min(1600, width) * 0.85;
            this.Height = height * 0.85;

            this.Left = (width - this.Width) / 2.0;
            this.Top = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            ColumnDefinition colExpander = mainGrid.ColumnDefinitions[0];
            colExpander.Width = new GridLength(LeftPane, GridUnitType.Pixel);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_isShown)
                return;

            _isShown = true;

            if (!_optionSettings.IsTestAvailable())
            {
                PromptDialog dlg = new PromptDialog();
                dlg.Owner = this;
                dlg.OptionSettings = _optionSettings;

                var dialogResult = dlg.ShowDialog();

                if (dialogResult != null && dialogResult.Value)
                {
                    if (_optionSettings.IsTestAvailable())
                    {
                        this.InitializePath(_optionSettings.SvgDirectory);
                    }
                }
            }
        }

        #endregion

        #region Private Event Handlers

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.Title = AppTitle;

            bottomExpander.IsExpanded = false;
            leftExpander.IsExpanded   = false;

            // Retrieve the display pages...
            _testsPage    = frameTests.Content      as TestsPage;
            _otherPage    = frameOthers.Content     as TestsOtherPage;

            _svgPage      = frameSvgInput.Content   as SvgPage;
            _xamlPage     = frameXamlOutput.Content as XamlPage;
            _debugPage    = frameDebugging.Content  as DebugPage;
            _settingsPage = frameSettings.Content   as SettingsPage;

            if (_svgPage != null)
            {
                _svgPage.MainWindow = this;
            }
            if (_xamlPage != null)
            {
                _xamlPage.MainWindow = this;
            }
            if (_testsPage != null)
            {
//                _testsPage.WorkingDrawingDir = _drawingDir;
                _testsPage.MainWindow = this;
            }
            if (_debugPage != null)
            {
                _debugPage.Startup();
            }
            if (_settingsPage != null)
            {
                _settingsPage.MainWindow = this;
            }

            tabSvgInput.Visibility   = _optionSettings.ShowInputFile ? Visibility.Visible : Visibility.Collapsed;
            tabXamlOutput.Visibility = _optionSettings.ShowOutputFile ? Visibility.Visible : Visibility.Collapsed;

            this.InitializePath(_optionSettings.SvgDirectory);

            tabTests.IsSelected = true;
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.SaveSettings();
            this.SaveTreeView(this.GetTestFilePath());

            if (_debugPage != null)
            {
                _debugPage.Shutdown();
            }
        }

        private void OnApplyTestState(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeItem = treeView.SelectedItem as TreeViewItem;
            if (treeView == null || treeItem.Tag == null)
            {
                return;
            }

            this.OnApplyTestState(treeItem);
        }

        private void OnApplyTestState(TreeViewItem treeItem)
        {
            SvgTestItem testItem = treeItem.Tag as SvgTestItem;
            if (testItem == null)
            {
                return;
            }

            BulletDecorator header = treeItem.Header as BulletDecorator;
            if (header == null)
            {
                return;
            }
            int selIndex = stateComboBox.SelectedIndex;
            if (selIndex < 0)
            {
                return;
            }

            var oldState = testItem.State;
            var newState = (SvgTestState)selIndex;

            testItem.State   = newState;
            testItem.Comment = testComment.Text;

            Ellipse bullet = header.Bullet as Ellipse;
            if (bullet != null)
            {
                bullet.Fill = testItem.StateBrush;
            }
            if (!_isTreeModified)
            {
                this.Title = AppTitle + " *";

                _isTreeModified = true;
            }

            _isTreeChangedPending = false;
            testApply.IsEnabled = false;

            if (oldState != newState)
            {
                switch (oldState)
                {
                    case SvgTestState.Success:
                        this.TestSuccesses = _testSuccesses - 1;
                        break;
                    case SvgTestState.Failure:
                        this.TestFailures  = _testFailures - 1;
                        break;
                    case SvgTestState.Partial:
                        this.TestPartials  = _testPartials - 1;
                        break;
                    case SvgTestState.Unknown:
                        this.TestUnknowns  = _testUnknowns - 1;
                        break;
                }
                switch (newState)
                {
                    case SvgTestState.Success:
                        this.TestSuccesses = _testSuccesses + 1;
                        break;
                    case SvgTestState.Failure:
                        this.TestFailures  = _testFailures + 1;
                        break;
                    case SvgTestState.Partial:
                        this.TestPartials  = _testPartials + 1;
                        break;
                    case SvgTestState.Unknown:
                        this.TestUnknowns  = _testUnknowns + 1;
                        break;
                }
            }
        }

        private void OnStateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isTreeChangedPending = true;
            testApply.IsEnabled   = true;
        }

        private void OnCommentTextChanged(object sender, TextChangedEventArgs e)
        {
            _isTreeChangedPending = true;
            testApply.IsEnabled   = true;
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            var changedProp = e.PropertyName;
            if (string.IsNullOrWhiteSpace(changedProp))
            {
                return;
            }

            if (string.Equals(changedProp, "ShowInputFile", StringComparison.OrdinalIgnoreCase))
            {
                this.OnFillSvgInputChecked();
            }
            else if (string.Equals(changedProp, "ShowOutputFile", StringComparison.OrdinalIgnoreCase))
            {
                this.OnFillXamlOutputChecked();
            }
        }

        private void OnFillSvgInputChecked()
        {
            if (_svgPage == null)
            {
                tabSvgInput.Visibility = _optionSettings.ShowInputFile ? Visibility.Visible : Visibility.Collapsed;
                return;
            }

            Cursor saveCursor = this.Cursor;

            try
            {
                if (_optionSettings.ShowInputFile)
                {
                    this.Cursor      = Cursors.Wait;
                    this.ForceCursor = true;

                    if (File.Exists(_svgFilePath))
                    {
                        _svgPage.LoadDocument(_svgFilePath);
                    }
                    else
                    {
                        _svgPage.UnloadDocument();
                    }
                }
                else
                {
                    _svgPage.UnloadDocument();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor      = saveCursor;
                this.ForceCursor = false;

                tabSvgInput.Visibility = _optionSettings.ShowInputFile ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnFillXamlOutputChecked()
        {
            if (_xamlPage == null || _testsPage == null)
            {
                tabXamlOutput.Visibility = _optionSettings.ShowOutputFile ? Visibility.Visible : Visibility.Collapsed;
                return;
            }

            Cursor saveCursor = this.Cursor;

            try
            {
                if (_optionSettings.ShowOutputFile)
                {
                    this.Cursor      = Cursors.Wait;
                    this.ForceCursor = true;

                    if (!_testsPage.LoadXaml())
                    {
                        _xamlPage.UnloadDocument();
                    }
                }
                else
                {
                    _xamlPage.UnloadDocument();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor      = saveCursor;
                this.ForceCursor = false;

                tabXamlOutput.Visibility = _optionSettings.ShowOutputFile ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnTabItemGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender == tabTests)
            {
                if (_testsPage != null)
                {
                    _testsPage.PageSelected(true);
                }
            }
            if (sender == tabTests)
            {
                if (_otherPage != null)
                {
                    _otherPage.PageSelected(true);
                }
            }
            else if (sender == tabXamlOutput)
            {
                if (_xamlPage != null)
                {
                    _xamlPage.PageSelected(true);
                }
            }
            else if (sender == tabSvgInput)
            {
                if (_svgPage != null)
                {
                    _svgPage.PageSelected(true);
                }
            }
            else if (sender == tabSettings)
            {
                if (_settingsPage != null)
                {
                    _settingsPage.PageSelected(true);
                }
            }
            else if (sender == tabDebugging)
            {
                if (_debugPage != null)
                {
                    _debugPage.PageSelected(true);
                }
            }
        }

        #endregion

        #region TreeView Event Handlers

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private async void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                return;
            }

            _svgFilePath = null;

            var testItem = selItem.Tag as SvgTestItem;
            if (testItem == null)
            {
                return;
            }
            string svgFilePath = IoPath.Combine(_optionSettings.SvgDirectory, testItem.FileName);
            if (string.IsNullOrWhiteSpace(svgFilePath) || !File.Exists(svgFilePath))
            {
                return;
            }
            _svgFilePath = svgFilePath;
            var fileName = IoPath.GetFileNameWithoutExtension(testItem.FileName);
            string pngFilePath = IoPath.Combine(_optionSettings.PngDirectory, fileName + ".png");

            e.Handled = true;

            this.CloseDocuments();

            treeView.IsEnabled = false;

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                bool isSvgLoaded = true;

                if (_svgPage != null && _optionSettings.ShowInputFile)
                {
                    isSvgLoaded = _svgPage.LoadDocument(svgFilePath);
                }

                if (isSvgLoaded && _testsPage != null)
                {
                    isSvgLoaded = _testsPage.LoadTests(svgFilePath, pngFilePath);
                }

#if DOTNET40
                await TaskEx.Delay(2000);
#else
                await Task.Delay(2000);
#endif

                await _otherPage.LoadDocumentAsync(_optionSettings, svgFilePath);

                stateComboBox.SelectedIndex = (int)testItem.State;
                testComment.Text = testItem.Comment;

                _isTreeChangedPending = false;
                testApply.IsEnabled = false;

                if (_optionSettings != null)
                {
                    _optionSettings.SelectedNumber = testItem.Number;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;

                treeView.IsEnabled = true;
                treeView.Focus();
            }
        }

        private void OnTreeViewItemUnselected(object sender, RoutedEventArgs e)
        {
            if (_optionSettings != null)
            {
                _optionSettings.SelectedNumber = 0;
            }
            // Prompt for any un-applied modifications to avoid lost.
            if (_isTreeChangedPending)
            {
                TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
                if (treeItem == null)
                {
                    return;
                }

                if (treeItem != null && treeItem.Tag != null)
                {
                    SvgTestItem testItem = treeItem.Tag as SvgTestItem;
                    if (testItem != null)
                    {
                        MessageBoxResult boxResult = MessageBox.Show(
                            "The previously selected test item was modified.\nDo you want to apply the current modifications?",
                            "Svg Test Suite",
                           MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (boxResult == MessageBoxResult.Yes || boxResult == MessageBoxResult.OK)
                        {
                            this.OnApplyTestState(treeItem);
                        }
                    }
                }

                _isTreeChangedPending = false;
            }
        }

        private void OnTreeViewItemCollapsed(object sender, RoutedEventArgs e)
        {
            if (_folderClose == null)
            {
                return;
            }

            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            if (treeItem == null)
            {
                return;
            }

            BulletDecorator decorator = treeItem.Header as BulletDecorator;
            if (decorator == null)
            {
                return;
            }
            Image headerImage = decorator.Bullet as Image;
            if (headerImage == null)
            {
                return;
            }
            headerImage.Source = _folderClose;

            e.Handled = true;
        }

        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            if (_folderOpen == null)
            {
                return;
            }

            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            if (treeItem == null)
            {
                return;
            }

            BulletDecorator decorator = treeItem.Header as BulletDecorator;
            if (decorator == null)
            {
                return;
            }
            Image headerImage = decorator.Bullet as Image;
            if (headerImage == null)
            {
                return;
            }
            headerImage.Source = _folderOpen;

            e.Handled = true;
        }

        #endregion

        #region LeftExpander/Splitter Event Handlers

        private void OnLeftExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            if (_leftSplitterChanging || _isBottomSplitterChanging)
            {
                return;
            }
            // Prevent WPF silly event routing...
            if (e.Source != leftExpander)
            {
                return;
            }

            e.Handled = true;

            ColumnDefinition columnDef = mainGrid.ColumnDefinitions[0];
            columnDef.Width = new GridLength(24, GridUnitType.Pixel);
        }

        private void OnLeftExpanderExpanded(object sender, RoutedEventArgs e)
        {
            if (_leftSplitterChanging || _isBottomSplitterChanging)
            {
                return;
            }
            // Prevent WPF silly event routing...
            if (e.Source != leftExpander)
            {
                return;
            }

            e.Handled = true;

            ColumnDefinition columnDef = mainGrid.ColumnDefinitions[0];
            columnDef.Width = new GridLength(LeftPane, GridUnitType.Pixel);
        }

        private void OnLeftSplitterMove(object sender, MouseEventArgs e)
        {
            _leftSplitterChanging = true;

            ColumnDefinition columnDef = mainGrid.ColumnDefinitions[0];

            leftExpander.IsExpanded = columnDef.ActualWidth > 30;

            _leftSplitterChanging = false;

            e.Handled = true;
        }

        #endregion

        #region BottomExpander/Splitter Event Handlers

        private void OnBottomExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            if (_isBottomSplitterChanging)
            {
                return;
            }
            // Prevent WPF silly event routing...
            if (e.Source != bottomExpander)
            {
                return;
            }

            RowDefinition rowDef = bottomGrid.RowDefinitions[2];
            rowDef.Height = new GridLength(24, GridUnitType.Pixel);
        }

        private void OnBottomExpanderExpanded(object sender, RoutedEventArgs e)
        {
            if (_isBottomSplitterChanging)
            {
                return;
            }
            // Prevent WPF silly event routing...
            if (e.Source != bottomExpander)
            {
                return;
            }

            RowDefinition rowDef = bottomGrid.RowDefinitions[2];
            rowDef.Height = new GridLength(LeftBottomPane, GridUnitType.Pixel);
        }

        private void OnBottomSplitterMove(object sender, MouseEventArgs e)
        {
            _isBottomSplitterChanging = true;

            RowDefinition rowDef = bottomGrid.RowDefinitions[2];

            bottomExpander.IsExpanded = rowDef.ActualHeight > 30;

            _isBottomSplitterChanging = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This converts the SVG resource specified by the Uri to <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
        /// <returns>A <see cref="DrawingGroup"/> of the converted SVG resource.</returns>
        private DrawingGroup GetDrawing(Uri svgSource)
        {
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = false;
            settings.TextAsGeometry = true;
            settings.OptimizePath   = true;

            StreamResourceInfo svgStreamInfo = null;
            if (svgSource.ToString().IndexOf("siteoforigin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                svgStreamInfo = Application.GetRemoteStream(svgSource);
            }
            else
            {
                svgStreamInfo = Application.GetResourceStream(svgSource);
            }

            Stream svgStream = (svgStreamInfo != null) ? svgStreamInfo.Stream : null;

            if (svgStream != null)
            {
                string fileExt = IoPath.GetExtension(svgSource.ToString());
                bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) && string.Equals(
                    fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase);

                if (isCompressed)
                {
                    using (svgStream)
                    {
                        using (var zipStream = new GZipStream(svgStream, CompressionMode.Decompress))
                        {
                            using (FileSvgReader reader = new FileSvgReader(settings))
                            {
                                DrawingGroup drawGroup = reader.Read(zipStream);

                                if (drawGroup != null)
                                {
                                    return drawGroup;
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (svgStream)
                    {
                        using (FileSvgReader reader = new FileSvgReader(settings))
                        {
                            DrawingGroup drawGroup = reader.Read(svgStream);

                            if (drawGroup != null)
                            {
                                return drawGroup;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This converts the SVG resource specified by the Uri to <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
        /// <returns>A <see cref="DrawingImage"/> of the converted SVG resource.</returns>
        /// <remarks>
        /// This uses the <see cref="GetDrawing(Uri)"/> method to convert the SVG resource to <see cref="DrawingGroup"/>,
        /// which is then wrapped in <see cref="DrawingImage"/>.
        /// </remarks>
        private DrawingImage GetImage(Uri svgSource)
        {
            DrawingGroup drawGroup = this.GetDrawing(svgSource);
            if (drawGroup != null)
            {
                return new DrawingImage(drawGroup);
            }
            return null;
        }

        private string GetTestFilePath()
        {
            if (string.IsNullOrWhiteSpace(_testFilePath))
            {
                return IoPath.GetFullPath(IoPath.Combine("..\\", SvgTestsFileName));
            }
            return _testFilePath;
        }

        private string GetOldFilePath()
        {
            return IoPath.GetFullPath(IoPath.Combine("..\\", SvgOldFileName));
        }

        private void InitializePath(string selectePath)
        {
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                this.CloseDocuments();
                return;
            }

            this.CloseDocuments();

            string testFilePath = GetTestFilePath();
            if (File.Exists(testFilePath))
            {
                this.LoadTestView(testFilePath);

                if (_optionSettings != null && !string.IsNullOrWhiteSpace(selectePath))
                {
                    _sourceDir         = new string(selectePath.ToCharArray());
                    _isRecursiveSearch = _optionSettings.RecursiveSearch;
                }
            }
            else
            {
                this.InitTreeView(selectePath);
            }
        }

        private bool SetProperty<T>(ref T field, T newValue, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }

        private void CloseDocuments()
        {
            try
            {
                if (_svgPage != null)
                {
                    _svgPage.UnloadDocument();
                }
                if (_xamlPage != null)
                {
                    _xamlPage.UnloadDocument();
                }
                if (_testsPage != null)
                {
                    _testsPage.UnloadDocument();
                }
                if (_debugPage != null)
                {
                    _debugPage.ClearDocument();
                }

                _svgFilePath = null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #region InitTreeView Methods

        private IDictionary<string, SvgTestState> GetOldTestStates()
        {
            var svgTestPath = GetOldFilePath();
            if (!File.Exists(svgTestPath))
            {
                return null;
            }

            var svgTestStates = new Dictionary<string, SvgTestState>(StringComparer.OrdinalIgnoreCase);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
            settings.IgnoreProcessingInstructions = true;

            var comparer = StringComparison.OrdinalIgnoreCase;

            using (XmlReader reader = XmlReader.Create(svgTestPath, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (string.Equals(reader.Name, "test", comparer))
                        {
                            // <test number=2, source="test.svg" state="partial" comment=""/>
                            string fileName = reader.GetAttribute("filename");
                            if (!string.IsNullOrWhiteSpace(fileName))
                            {
                                string stateAttr = reader.GetAttribute("state");

                                var state = SvgTestState.Failure;

                                if (!string.IsNullOrWhiteSpace(stateAttr))
                                {
                                    if (stateAttr.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = SvgTestState.Unknown;
                                    }
                                    else if (stateAttr.Equals("failure", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = SvgTestState.Failure;
                                    }
                                    else if (stateAttr.Equals("success", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = SvgTestState.Success;
                                    }
                                    else if (stateAttr.Equals("partial", StringComparison.OrdinalIgnoreCase))
                                    {
                                        state = SvgTestState.Partial;
                                    }
                                }

                                svgTestStates.Add(fileName, state);
                            }
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (string.Equals(reader.Name, "tests", comparer))
                        {
                            break;
                        }
                    }
                }
            }
            return svgTestStates;
        }

        private void InitTreeView(string sourceDir)
        {
            if (!string.IsNullOrWhiteSpace(_sourceDir) && Directory.Exists(_sourceDir))
            {
                this.CloseDocuments();
            }
            _sourceDir = string.Empty;
            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                return;
            }

            if (_testItems == null || _testItems.Count != 0)
            {
                _testItems = new List<SvgTestItem>((int)SvgTestItem.TestCount);
            }

            _sourceDir         = new string(sourceDir.ToCharArray());
            _isRecursiveSearch = _optionSettings.RecursiveSearch;

            treeView.BeginInit();
            treeView.Items.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);

            TextBlock headerText = new TextBlock();
            headerText.Text   = "Tests";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            BulletDecorator decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image  = new Image();
                image.Source = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem rootItem = new TreeViewItem();
            rootItem.Tag        = string.Empty;
            rootItem.Header     = decorator;
            rootItem.Margin     = new Thickness(0);
            rootItem.Padding    = new Thickness(3);
            rootItem.FontSize   = 14;
            rootItem.FontWeight = FontWeights.Bold;

            treeView.Items.Add(rootItem);

            InitTreeView(sourceDir, rootItem);

            rootItem.IsExpanded = true;

            treeView.EndInit();

            leftExpander.IsExpanded   = true;

            _isTreeModified = true;
        }

        private void InitTreeView(string sourceDir, TreeViewItem rootTreeItem)
        {
            TextBlock headerText = new TextBlock();
            headerText.Text   = "Attributes";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            BulletDecorator decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image      = new Image();
                image.Source     = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet         = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet       = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem attributesItem = new TreeViewItem();
            attributesItem.Tag        = string.Empty;
            attributesItem.Header     = decorator;
            attributesItem.Margin     = new Thickness(0);
            attributesItem.Padding    = new Thickness(3);
            attributesItem.FontSize   = 14;
            attributesItem.FontWeight = FontWeights.Bold;

            rootTreeItem.Items.Add(attributesItem);

            headerText = new TextBlock();
            headerText.Text   = "Elements";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image      = new Image();
                image.Source     = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet         = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet       = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem elementsItem = new TreeViewItem();
            elementsItem.Tag        = string.Empty;
            elementsItem.Header     = decorator;
            elementsItem.Margin     = new Thickness(0);
            elementsItem.Padding    = new Thickness(3);
            elementsItem.FontSize   = 14;
            elementsItem.FontWeight = FontWeights.Bold;

            rootTreeItem.Items.Add(elementsItem);

            var svgTestStates = GetOldTestStates();

            var svgFiles = Directory.EnumerateFiles(sourceDir, SvgFilePattern, SearchOption.TopDirectoryOnly);

            uint total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            foreach (string svgFile in svgFiles)
            {
                total++;
                TextBlock itemText = new TextBlock();
                var fileName = IoPath.GetFileName(svgFile);
                itemText.Text   = string.Format("({0:D4}) - {1}", total, fileName);
                itemText.Margin = new Thickness(3, 0, 0, 0);

                var testItem = new SvgTestItem(total, fileName);
                if (svgTestStates != null && svgTestStates.ContainsKey(testItem.FileName))
                {
                    testItem.State = svgTestStates[fileName];
                }
                switch (testItem.State)
                {
                    case SvgTestState.Unknown:
                        unknowns++;
                        break;
                    case SvgTestState.Failure:
                        failures++;
                        break;
                    case SvgTestState.Success:
                        successes++;
                        break;
                    case SvgTestState.Partial:
                        partials++;
                        break;
                }

                _testItems.Add(testItem);

                BulletDecorator fileItem = new BulletDecorator();
                Ellipse bullet         = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = testItem.StateBrush;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                fileItem.Bullet = bullet;
                fileItem.Margin = new Thickness(0, 0, 10, 0);
                fileItem.Child  = itemText;

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Tag        = testItem;
                treeItem.Header     = fileItem;
                treeItem.Margin     = new Thickness(0);
                treeItem.Padding    = new Thickness(2);
                treeItem.FontSize   = 12;
                treeItem.FontWeight = FontWeights.Normal;

                testItem.Tag = treeItem;
                if (fileName.StartsWith("a-", StringComparison.OrdinalIgnoreCase))
                {
                    attributesItem.Items.Add(treeItem);
                    testItem.Parent = attributesItem;
                }
                else if (fileName.StartsWith("e-", StringComparison.OrdinalIgnoreCase))
                {
                    elementsItem.Items.Add(treeItem);
                    testItem.Parent = elementsItem;
                }
                else
                {
                    rootTreeItem.Items.Add(treeItem);
                    testItem.Parent = rootTreeItem;
                }
            }

            this.TestTotal     = total;
            this.TestUnknowns  = unknowns;
            this.TestFailures  = failures;
            this.TestSuccesses = successes;
            this.TestPartials  = partials;
        }

        #endregion

        #region LoadTestView Methods

        private void LoadTestView(string testFilePath)
        {
            if (string.IsNullOrWhiteSpace(testFilePath) || !File.Exists(testFilePath))
            {
                return;
            }

            if (_testItems == null || _testItems.Count != 0)
            {
                _testItems = new List<SvgTestItem>((int)SvgTestItem.TestCount);
            }

            TextBlock headerText = new TextBlock();
            headerText.Text   = "Tests";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            BulletDecorator decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image  = new Image();
                image.Source = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem rootItem = new TreeViewItem();
            rootItem.Tag        = string.Empty;
            rootItem.Header     = decorator;
            rootItem.Margin     = new Thickness(0);
            rootItem.Padding    = new Thickness(3);
            rootItem.FontSize   = 14;
            rootItem.FontWeight = FontWeights.Bold;

            treeView.Items.Add(rootItem);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace             = false;
            settings.IgnoreComments               = true;
            settings.IgnoreProcessingInstructions = true;

            using (XmlReader reader = XmlReader.Create(testFilePath, settings))
            {
                LoadTreeView(reader, rootItem);
            }

            rootItem.IsExpanded = true;

            treeView.EndInit();

            leftExpander.IsExpanded = true;

            _testFilePath = testFilePath;
        }

        private void LoadTreeView(XmlReader reader, TreeViewItem rootTreeItem)
        {
            TextBlock headerText = new TextBlock();
            headerText.Text   = "Attributes";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            BulletDecorator decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image      = new Image();
                image.Source     = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet         = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet       = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem attributesItem = new TreeViewItem();
            attributesItem.Tag        = string.Empty;
            attributesItem.Header     = decorator;
            attributesItem.Margin     = new Thickness(0);
            attributesItem.Padding    = new Thickness(3);
            attributesItem.FontSize   = 14;
            attributesItem.FontWeight = FontWeights.Bold;

            rootTreeItem.Items.Add(attributesItem);

            headerText = new TextBlock();
            headerText.Text   = "Elements";
            headerText.Margin = new Thickness(3, 0, 0, 0);

            decorator = new BulletDecorator();
            if (_folderClose != null)
            {
                Image image      = new Image();
                image.Source     = _folderClose;

                decorator.Bullet = image;
            }
            else
            {
                Ellipse bullet         = new Ellipse();
                bullet.Height          = 16;
                bullet.Width           = 16;
                bullet.Fill            = Brushes.Goldenrod;
                bullet.Stroke          = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet       = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = headerText;

            TreeViewItem elementsItem = new TreeViewItem();
            elementsItem.Tag        = string.Empty;
            elementsItem.Header     = decorator;
            elementsItem.Margin     = new Thickness(0);
            elementsItem.Padding    = new Thickness(3);
            elementsItem.FontSize   = 14;
            elementsItem.FontWeight = FontWeights.Bold;

            TreeViewItem selectedTreeItem   = null;
            TreeViewItem selectedParentItem = null;
            rootTreeItem.Items.Add(elementsItem);

            uint selectedNumber = 1;
            if (_optionSettings != null)
            {
                selectedNumber = _optionSettings.SelectedNumber;
            }

            uint total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            var comparer = StringComparison.OrdinalIgnoreCase;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "test", comparer))
                    {
                        SvgTestItem testItem = new SvgTestItem(reader);
                        if (!testItem.IsEmpty)
                        {
                            total++;
                            var fileName = testItem.FileName;

                            TextBlock itemText = new TextBlock();
                            itemText.Text   = string.Format("({0:D4}) - {1}", total, fileName);
                            itemText.Margin = new Thickness(3, 0, 0, 0);

                            Ellipse bullet         = new Ellipse();
                            bullet.Height          = 16;
                            bullet.Width           = 16;
                            bullet.Fill            = testItem.StateBrush;
                            bullet.Stroke          = Brushes.DarkGray;
                            bullet.StrokeThickness = 1;

                            decorator = new BulletDecorator();
                            decorator.Bullet = bullet;
                            decorator.Margin = new Thickness(0, 0, 10, 0);
                            decorator.Child  = itemText;

                            TreeViewItem treeItem = new TreeViewItem();
                            treeItem.Header     = decorator;
                            treeItem.Padding    = new Thickness(2);
                            treeItem.FontSize   = 12;
                            treeItem.FontWeight = FontWeights.Normal;
                            treeItem.Tag        = testItem;

                            testItem.Tag        = treeItem;
                            if (fileName.StartsWith("a-", StringComparison.OrdinalIgnoreCase))
                            {
                                attributesItem.Items.Add(treeItem);
                                testItem.Parent = attributesItem;
                            }
                            else if (fileName.StartsWith("e-", StringComparison.OrdinalIgnoreCase))
                            {
                                elementsItem.Items.Add(treeItem);
                                testItem.Parent = elementsItem;
                            }
                            else
                            {
                                rootTreeItem.Items.Add(treeItem);
                                testItem.Parent = rootTreeItem;
                            }

                            if (selectedTreeItem == null && testItem.Number == selectedNumber)
                            {
                                treeItem.IsSelected = true;
                                selectedTreeItem = treeItem;

                                if (fileName.StartsWith("a-", StringComparison.OrdinalIgnoreCase))
                                {
                                    selectedParentItem = attributesItem;
                                }
                                else if (fileName.StartsWith("e-", StringComparison.OrdinalIgnoreCase))
                                {
                                    selectedParentItem = elementsItem;
                                }
                                else
                                {
                                    selectedParentItem = rootTreeItem;
                                }
                            }

                            SvgTestState testState = testItem.State;

                            switch (testState)
                            {
                                case SvgTestState.Unknown:
                                    unknowns++;
                                    break;
                                case SvgTestState.Failure:
                                    failures++;
                                    break;
                                case SvgTestState.Success:
                                    successes++;
                                    break;
                                case SvgTestState.Partial:
                                    partials++;
                                    break;
                            }

                            _testItems.Add(testItem);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {   
                    if (string.Equals(reader.Name, "tests", comparer))
                    {
                        break;
                    }
                }
            }

            this.TestTotal     = total;
            this.TestUnknowns  = unknowns;
            this.TestFailures  = failures;
            this.TestSuccesses = successes;
            this.TestPartials  = partials;

            if (selectedTreeItem != null && selectedParentItem != null)
            {
                selectedParentItem.IsExpanded = true;
                selectedTreeItem.BringIntoView();
            }
        }

        #endregion

        #region Save Methods

        private void SaveSettings()
        {
            string backupFile = null;
            if (File.Exists(_testSettingsPath))
            {
                backupFile = IoPath.ChangeExtension(_testSettingsPath, SvgConverter.BackupExt);
                try
                {
                    if (File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }
                    File.Move(_testSettingsPath, backupFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
            }
            try
            {
                _optionSettings.Save(_testSettingsPath);
            }
            catch (Exception ex)
            {
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, _testSettingsPath);
                }

                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);                
            }
            if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
            {
                File.Delete(backupFile);
            }
        }

        private void SaveTreeView(string testFilePath)
        {
            if (!_isTreeModified || string.IsNullOrWhiteSpace(testFilePath) || _testItems == null)
            {
                return;
            }

            var backupFile = IoPath.ChangeExtension(testFilePath, ".bak");
            try
            {
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }

                if (File.Exists(testFilePath))
                {
                    File.Move(testFilePath, backupFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.Encoding = Encoding.UTF8;

                using (XmlWriter writer = XmlWriter.Create(testFilePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("tests");

                    for (int i = 0; i < _testItems.Count; i++)
                    {
                        _testItems[i].WriteXml(writer);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, testFilePath);
                }

                MessageBox.Show(ex.ToString(), AppErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
            {
                File.Delete(backupFile);
            }

            _testFilePath = testFilePath;

            if (_isTreeModified)
            {
                this.Title = AppTitle;

                _isTreeModified = false;
            }
        }

        #endregion

        #endregion
    }
}
