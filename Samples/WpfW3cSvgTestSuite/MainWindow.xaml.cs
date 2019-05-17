using System;
using System.IO;
using System.Xml;
using System.Text;
using System.ComponentModel;
using System.IO.Compression;
using System.Collections.Generic;

using IoPath = System.IO.Path;

using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Resources;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private const int LeftPane         = 325;
        private const int LeftBottomPane   = 220;

        private const string TestSuiteDir = "";
        private const string SvgSubDir    = "";
        private const string PngSubDir    = "";

        private const string AppTitle       = "Svg Test Suite";
        private const string AppErrorTitle  = "Svg Test Suite - Error";
        private const string SvgTestSuite   = "SvgTestSuite.xml";
        private const string SvgTestResults = "SvgTestResults.xml";
  
        private bool   _isTreeModified;

        private bool   _isTreeChangedPending;

        private bool   _isSuiteAvailable;
        private bool   _leftSplitterChanging;
        private bool   _isBottomSplitterChanging;

        private string _suitePath;
        private string _svgPath;
        private string _pngPath;

        private string _testFilePath;
        private string _testResultsPath;

        private string _drawingDir;

        private SvgPage     _svgPage;
        private XamlPage    _xamlPage;
        private DrawingPage _drawingPage;
        private BrowserPage _browserPage;
        private AboutPage _aboutPage;
        private DebugPage _debugPage;
        private SettingsPage _settingsPage;

        private ImageSource _folderClose;
        private ImageSource _folderOpen;

        private DirectoryInfo _directoryInfo;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private DrawingGroup _currentDrawing;

        private IList<SvgTestResult> _testResults;

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

            this.Loaded  += OnWindowLoaded;
            this.Closing += OnWindowClosing;

            _drawingDir = IoPath.Combine(IoPath.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), "XamlDrawings");

            if (!Directory.Exists(_drawingDir))
            {
                Directory.CreateDirectory(_drawingDir);
            }

            _directoryInfo = new DirectoryInfo(_drawingDir);

            _wpfSettings         = new WpfDrawingSettings();

            _fileReader          = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            try
            {
                //                _folderClose = new BitmapImage();
                //var folderClose = new BitmapImage();
                //folderClose.BeginInit();
                //folderClose.UriSource = new Uri("Images/FolderClose.png", UriKind.Relative);
                //folderClose.EndInit();
                //_folderClose = folderClose;

                _folderClose = this.GetImage(new Uri("Images/FolderClose.svg", UriKind.Relative));

                //var folderOpen = new BitmapImage();
                //folderOpen.BeginInit();
                //folderOpen.UriSource = new Uri("Images/FolderOpen.png", UriKind.Relative);
                //folderOpen.EndInit();
                //_folderOpen = folderOpen;

                _folderOpen = this.GetImage(new Uri("Images/FolderOpen.svg", UriKind.Relative));
            }
            catch (Exception ex)
            {
                _folderClose = null;
                _folderOpen = null;

                MessageBox.Show(ex.ToString(), "Svg Test Suite - Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Public Properties

        public XamlPage XamlPage
        {
            get {
                return _xamlPage;
            }
            set {
                _xamlPage = value;
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

                    // Recreated the conveter
                    _fileReader = new FileSvgReader(_wpfSettings);
                    _fileReader.SaveXaml = true;
                    _fileReader.SaveZaml = false;
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double width  = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            this.Width  = Math.Min(1200, width) * 0.90;
            this.Height = Math.Min(1080, height) * 0.90;

            this.Left = (width  - this.Width) / 2.0;
            this.Top  = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            ColumnDefinition colExpander = mainGrid.ColumnDefinitions[0];
            colExpander.Width = new GridLength(LeftPane, GridUnitType.Pixel); 

            RowDefinition rowExpander = bottomGrid.RowDefinitions[2];
            rowExpander.Height = new GridLength(24, GridUnitType.Pixel);
        }

        #endregion

        #region Private Event Handler Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            leftExpander.IsExpanded   = false;
            bottomExpander.IsExpanded = false;

            // Retrieve the display pages...
            _svgPage      = frameSvgInput.Content   as SvgPage;
            _xamlPage     = frameXamlOutput.Content as XamlPage;
            _drawingPage  = frameDrawing.Content    as DrawingPage;
            _browserPage  = frameBrowser.Content    as BrowserPage;
            _aboutPage    = frameAbout.Content      as AboutPage;
            _debugPage    = frameDebugging.Content as DebugPage;
            _settingsPage = frameSettings.Content as SettingsPage;

            if (_svgPage != null && _settingsPage != null)
            {
                _settingsPage.MainWindow = this;
            }

            if (_debugPage != null)
            {
                _debugPage.Startup();
            }

            if (_fileReader != null)
            {
                _fileReader.SaveXaml = Directory.Exists(_drawingDir);
            }

            frameAbout.Navigating += OnFrameAboutNavigating;
            frameAbout.Navigated += OnFrameAboutNavigated;

            string currentDir = IoPath.GetFullPath(@"..\..\FullTestSuite");

            if (Directory.Exists(currentDir))
            {
                this.txtSvgSuitePath.Text = currentDir;
            }

            _isTreeChangedPending   = false;
            testApply.IsEnabled     = false;
            testInfoPanel.IsEnabled = false;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!_isTreeModified || string.IsNullOrWhiteSpace(_testFilePath) ||
                !File.Exists(_testFilePath))
            {
                return;
            }

            string backupFile = IoPath.ChangeExtension(_testFilePath, ".bak");
            try
            {
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
                File.Move(_testFilePath, backupFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.Encoding = Encoding.UTF8;

                using (XmlWriter writer = XmlWriter.Create(_testFilePath, settings))
                {
                    this.SaveTreeView(writer);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(backupFile))
                {
                    File.Move(backupFile, _testFilePath);
                }

                MessageBox.Show(ex.ToString(), AppErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (!string.IsNullOrWhiteSpace(_testResultsPath))
            {
                if (!string.IsNullOrWhiteSpace(_testResultsPath))
                {
                    if (File.Exists(_testResultsPath))
                    {
                        backupFile = IoPath.ChangeExtension(_testResultsPath, ".bak");
                        try
                        {
                            if (File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            File.Move(_testResultsPath, backupFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), AppErrorTitle,
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    try
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.IndentChars = "    ";
                        settings.Encoding = Encoding.UTF8;

                        using (XmlWriter writer = XmlWriter.Create(_testResultsPath, settings))
                        {
                            this.SaveTestResults(writer);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                        {
                            File.Move(backupFile, _testResultsPath);
                        }

                        MessageBox.Show(ex.ToString(), AppErrorTitle,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            if (_debugPage != null)
            {
                _debugPage.Startup();
            }
        }

        private void OnBrowseForSvgSuitePath(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            dlg.Description         = "Select the location of the W3C SVG 1.1 Full Test Suite";
            dlg.RootFolder          = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSvgSuitePath.Text = dlg.SelectedPath;
            }   
        }

        private void OnShowSvgTestResults(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            var dlg = new SvgTestResultsWindow
            {
                Owner = this,
                Results = _testResults
            };

            // Open the dialog box modally 
            dlg.ShowDialog();
        }

        private void OnSvgSuitePathTextChanged(object sender, TextChangedEventArgs e)
        {
            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                return;
            }

            if (!Directory.Exists(IoPath.Combine(selectePath, "svg")))
            {
                return;
            }
            if (!Directory.Exists(IoPath.Combine(selectePath, "png")))
            {
                return;
            }

            this.InitializePath(selectePath);
        }

        private void EnableTestPanel(bool isEnabled)
        {
            testInfoPanel.IsEnabled = isEnabled;
            if (!isEnabled)
            {
                stateComboBox.SelectedIndex = -1;
                testComment.Text    = string.Empty;

                testFilePath.Text   = string.Empty;
                testDescrition.Text = string.Empty;

                if (_svgPage != null)
                {
                    _svgPage.UnloadDocument();
                }
                if (_xamlPage != null)
                {
                    _xamlPage.UnloadDocument();
                }
                if (_drawingPage != null)
                {
                    _drawingPage.UnloadDocument();
                }
                if (_browserPage != null)
                {
                    _browserPage.UnloadDocument();
                }
                if (_aboutPage != null)
                {
                    _aboutPage.UnloadDocument();
                }
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
            SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
            if (testInfo == null)
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

            testInfo.State   = (SvgTestState)selIndex;
            testInfo.Comment = testComment.Text;

            Ellipse bullet = header.Bullet as Ellipse;
            if (bullet != null)
            {
                bullet.Fill = testInfo.StateBrush;
            }
            if (!_isTreeModified)
            {
                this.Title = this.Title + " *";

                _isTreeModified = true;
            }

            _isTreeChangedPending = false;
            testApply.IsEnabled   = false;
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            _currentDrawing = null;

            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                EnableTestPanel(false);
                return;
            }

            SvgTestInfo testItem = selItem.Tag as SvgTestInfo;
            if (testItem == null)
            {
                EnableTestPanel(false);
                return;
            }

            if (!_isSuiteAvailable)
            {
                EnableTestPanel(false);
                return;
            }

            string svgFilePath = IoPath.Combine(_suitePath, "svg\\" + testItem.FileName);
            if (!File.Exists(svgFilePath))
            {
                EnableTestPanel(false);
                return;
            }
            string pngFilePath = IoPath.Combine(_suitePath,
                "png\\full-" + IoPath.ChangeExtension(testItem.FileName, ".png"));
            if (!File.Exists(pngFilePath))
            {
                EnableTestPanel(false);
                return;
            }

            this.Cursor = Cursors.Wait;

            try
            {
                if (_svgPage != null)
                {
                    _svgPage.LoadDocument(svgFilePath, testItem, null);
                }
            }
            catch (Exception ex)
            {
                _isTreeChangedPending = false;
                this.Cursor = Cursors.Arrow;

                MessageBox.Show(ex.ToString(), AppErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            try
            {
                DrawingGroup drawing = _fileReader.Read(svgFilePath, _directoryInfo);
                if (drawing == null)
                {
                    return;
                }

                if (_drawingPage != null)
                {
                    _drawingPage.LoadDocument(pngFilePath, testItem, drawing);
                }

                if (_xamlPage != null && !string.IsNullOrWhiteSpace(_drawingDir))
                {
                    string xamlFilePath = IoPath.Combine(_drawingDir,
                        IoPath.ChangeExtension(testItem.FileName, ".xaml"));

                    if (File.Exists(xamlFilePath))
                    {
                        _xamlPage.LoadDocument(xamlFilePath, testItem, null);

                        // Delete the file after loading it...
                        File.Delete(xamlFilePath);
                    }
                }

                if (frameAbout != null)
                {
                    if (frameAbout.CanGoBack)
                    {
                        frameAbout.GoBack();
                        var entry = frameAbout.RemoveBackEntry();
                        while (entry != null)
                        {
                            entry = frameAbout.RemoveBackEntry();
                        }
                    }
                    frameAbout.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                }

                if (_aboutPage != null)
                {
                    _aboutPage.LoadDocument(svgFilePath, testItem, null);
                }

                if (_browserPage != null)
                {
                    _browserPage.LoadDocument(svgFilePath, testItem, drawing);
                }

                testFilePath.Text = svgFilePath;

                testDescrition.Text = testItem.Description;

                stateComboBox.SelectedIndex = (int)testItem.State;
                testComment.Text            = testItem.Comment;

                _isTreeChangedPending = false;
                testApply.IsEnabled   = false;
                EnableTestPanel(true);

                _currentDrawing = drawing;
            }
            catch (Exception ex)
            {
                _currentDrawing = null;

                //EnableTestPanel(false);
                _isTreeChangedPending = false;

                MessageBox.Show(ex.ToString(), AppErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }  
        }

        private void OnFrameAboutNavigated(object sender, NavigationEventArgs e)
        {
            if (frameAbout.CanGoBack || frameAbout.CanGoForward)
            {
                frameAbout.NavigationUIVisibility = NavigationUIVisibility.Automatic;
            } 
            else
            {
                frameAbout.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            }
        }

        private void OnFrameAboutNavigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void OnTreeViewItemUnselected(object sender, RoutedEventArgs e)
        {
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
                    SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
                    if (testInfo != null)
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

        private void OnStateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isTreeChangedPending = true;
            testApply.IsEnabled = true;
        }

        private void OnCommentTextChanged(object sender, TextChangedEventArgs e)
        {
            _isTreeChangedPending = true;
            testApply.IsEnabled = true;
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

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];
            rowExpander.Width = new GridLength(24, GridUnitType.Pixel);
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

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];
            rowExpander.Width = new GridLength(LeftPane, GridUnitType.Pixel);
        }

        private void OnLeftSplitterMove(object sender, MouseEventArgs e)
        {
            _leftSplitterChanging = true;

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];

            leftExpander.IsExpanded = rowExpander.ActualWidth > 24;

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

            RowDefinition rowExpander = bottomGrid.RowDefinitions[2];
            rowExpander.Height = new GridLength(24, GridUnitType.Pixel);
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

            RowDefinition rowExpander = bottomGrid.RowDefinitions[2];
            rowExpander.Height = new GridLength(LeftBottomPane, GridUnitType.Pixel);
        }

        private void OnBottomSplitterMove(object sender, MouseEventArgs e)
        {
            _isBottomSplitterChanging = true;

            RowDefinition rowExpander = bottomGrid.RowDefinitions[2];

            bottomExpander.IsExpanded = rowExpander.ActualHeight > 24;

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
            settings.OptimizePath = true;

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
                bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                    string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase);

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

        private void InitializePath(string suitePath)
        {   
            if (string.IsNullOrWhiteSpace(suitePath) || !Directory.Exists(suitePath))
            {
                return;
            }

            string svgPath = IoPath.Combine(suitePath, "svg");
            if (!Directory.Exists(svgPath))
            {
                return;
            }

            string pngPath = IoPath.Combine(suitePath, "png");
            if (!Directory.Exists(pngPath))
            {
                return;
            }

            _svgPath          = svgPath;
            _pngPath          = pngPath;

            _suitePath        = suitePath;
            _isSuiteAvailable = true;

            _testResults = new List<SvgTestResult>();

            _testResultsPath = IoPath.GetFullPath(SvgTestResults);
            if (!string.IsNullOrWhiteSpace(_testResultsPath) && File.Exists(_testResultsPath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(_testResultsPath, settings))
                {
                    LoadTestResults(reader);
                }
            }

            string fullFilePath = IoPath.GetFullPath(SvgTestSuite);
            if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false;
                settings.IgnoreComments   = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(fullFilePath, settings))
                {
                    LoadTreeView(reader);
                }

                leftExpander.IsExpanded = true;

                _testFilePath = fullFilePath;

                btnSvgTestResults.IsEnabled = true;
            }   
        }

        private void LoadTestResults(XmlReader reader)
        {
            if (_testResults == null || _testResults.Count != 0)
            {
                _testResults = new List<SvgTestResult>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "result", StringComparison.OrdinalIgnoreCase))
                {
                    SvgTestResult testResult = new SvgTestResult(reader);
                    if (testResult.IsValid)
                    {
                        _testResults.Add(testResult);
                    }
                }
            }
        }

        private void SaveTestResults(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            if (_testResults == null || _testResults.Count == 0)
            {
                return;
            }

            writer.WriteStartDocument();
            writer.WriteStartElement("results");

            for (int i = 0; i < _testResults.Count; i++)
            {
                SvgTestResult testResult = _testResults[i];
                if (testResult != null && testResult.IsValid)
                {
                    testResult.WriteXml(writer);
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void LoadTreeView(XmlReader reader)
        {
            SvgTestResult testResult = new SvgTestResult();

            treeView.BeginInit();
            treeView.Items.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                {
                    // <category label="Animations">
                    string category = reader.GetAttribute("label");
                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        SvgTestCategory testCategory = new SvgTestCategory(category);

                        TextBlock headerText = new TextBlock();
                        headerText.Text   = category;
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
                            bullet.Height = 16;
                            bullet.Width  = 16;
                            bullet.Fill   = Brushes.Goldenrod;
                            bullet.Stroke = Brushes.DarkGray;
                            bullet.StrokeThickness = 1;

                            decorator.Bullet = bullet;
                        }
                        decorator.Margin = new Thickness(0, 0, 10, 0);
                        decorator.Child = headerText;

                        TreeViewItem categoryItem = new TreeViewItem();
                        categoryItem.Header     = decorator;
                        categoryItem.Margin     = new Thickness(0, 3, 0, 3);
                        categoryItem.FontSize   = 14;
                        categoryItem.FontWeight = FontWeights.Bold;

                        treeView.Items.Add(categoryItem);

                        LoadTreeViewCategory(reader, categoryItem, testCategory);

                        if (testCategory.IsValid)
                        {
                            testResult.Categories.Add(testCategory);
                        }
                    }
                }
            }

            if (_testResults == null)
            {
                _testResults = new List<SvgTestResult>();
            }

            bool saveResults = false;
            if (testResult.IsValid)
            {
                if (_testResults.Count == 0)
                {
                    _testResults.Add(testResult);

                    saveResults = true;
                }
                else
                {
                    int foundAt = -1;
                    for (int i = 0; i < _testResults.Count; i++)
                    {
                        SvgTestResult nextResult = _testResults[i];
                        if (nextResult != null && nextResult.IsValid)
                        {
                            if (string.Equals(nextResult.Version, testResult.Version))
                            {
                                foundAt = i;
                                break;
                            }
                        }
                    }

                    if (foundAt >= 0)
                    {
                        SvgTestResult nextResult = _testResults[foundAt];

                        if (!SvgTestResult.AreEqual(nextResult, testResult))
                        {
                            _testResults[foundAt] = testResult;
                            saveResults = true;
                        }
                    }
                    else
                    {
                        _testResults.Add(testResult);

                        saveResults = true;
                    }
                }
            }
            if (saveResults)
            {
                if (!string.IsNullOrWhiteSpace(_testResultsPath))
                {
                    string backupFile = null;
                    if (File.Exists(_testResultsPath))
                    {
                        backupFile = IoPath.ChangeExtension(_testResultsPath, ".bak");
                        try
                        {
                            if (File.Exists(backupFile))
                            {
                                File.Delete(backupFile);
                            }
                            File.Move(_testResultsPath, backupFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), AppErrorTitle,
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    try
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.IndentChars = "    ";
                        settings.Encoding = Encoding.UTF8;

                        using (XmlWriter writer = XmlWriter.Create(_testResultsPath, settings))
                        {
                            this.SaveTestResults(writer);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
                        {
                            File.Move(backupFile, _testResultsPath);
                        }

                        MessageBox.Show(ex.ToString(), AppErrorTitle,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            treeView.EndInit();
        }

        private void LoadTreeViewCategory(XmlReader reader, TreeViewItem categoryItem, SvgTestCategory testCategory)
        {
            int total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            int itemCount = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Equals(reader.Name, "test", StringComparison.OrdinalIgnoreCase))
                    {
                        SvgTestInfo testInfo = new SvgTestInfo(reader);
                        if (!testInfo.IsEmpty)
                        {
                            TextBlock headerText = new TextBlock();
                            headerText.Text   = string.Format("({0:D3}) - {1}", itemCount, testInfo.Title);
                            headerText.Margin = new Thickness(3, 0, 0, 0);

                            Ellipse bullet         = new Ellipse();
                            bullet.Height          = 16;
                            bullet.Width           = 16;
                            bullet.Fill            = testInfo.StateBrush;
                            bullet.Stroke          = Brushes.DarkGray;
                            bullet.StrokeThickness = 1;

                            BulletDecorator decorator = new BulletDecorator();
                            decorator.Bullet = bullet;
                            decorator.Margin = new Thickness(0, 0, 10, 0);
                            decorator.Child  = headerText;

                            TreeViewItem treeItem = new TreeViewItem();
                            treeItem.Header     = decorator;
                            treeItem.Padding    = new Thickness(3);
                            treeItem.FontSize   = 12;
                            treeItem.FontWeight = FontWeights.Normal;
                            treeItem.Tag        = testInfo;                            

                            categoryItem.Items.Add(treeItem);

                            itemCount++;

                            total++;
                            SvgTestState testState = testInfo.State;

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
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {   
                    if (string.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }

            testCategory.SetValues(total, unknowns, failures, successes, partials);
        }

        private void SaveTreeView(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            ItemCollection treeNodes = treeView.Items;
            if (treeNodes == null || treeNodes.Count == 0)
            {
                return;
            }

            SvgTestResult testResult = new SvgTestResult();

            writer.WriteStartDocument();
            writer.WriteStartElement("tests");

            for (int i = 0; i < treeNodes.Count; i++)
            {
                TreeViewItem categoryItem = treeNodes[i] as TreeViewItem;
                if (categoryItem != null)
                {
                    BulletDecorator header = categoryItem.Header as BulletDecorator;
                    if (header == null)
                    {
                        continue;
                    }
                    TextBlock headerText = header.Child as TextBlock;
                    if (headerText == null)
                    {
                        continue;
                    }

                    SvgTestCategory testCategory = new SvgTestCategory(headerText.Text);

                    this.SaveTreeViewCategory(writer, categoryItem, testCategory);

                    if (testCategory.IsValid)
                    {
                        testResult.Categories.Add(testCategory);
                    }
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            if (_testResults == null)
            {
                _testResults = new List<SvgTestResult>();
            }

            if (testResult.IsValid)
            {
                if (_testResults.Count == 0)
                {
                    _testResults.Add(testResult);
                }
            }
            else
            {
                int foundAt = -1;
                for (int i = 0; i < _testResults.Count; i++)
                {
                    SvgTestResult nextResult = _testResults[i];
                    if (nextResult != null && nextResult.IsValid)
                    {
                        if (string.Equals(nextResult.Version, testResult.Version))
                        {
                            foundAt = i;
                            break;
                        }
                    }
                }

                if (foundAt >= 0)
                {
                    SvgTestResult nextResult = _testResults[foundAt];

                    if (!SvgTestResult.AreEqual(nextResult, testResult))
                    {
                        _testResults[foundAt] = testResult;
                    }
                }
                else
                {
                    _testResults.Add(testResult);
                }
            }
        }

        private void SaveTreeViewCategory(XmlWriter writer, TreeViewItem categoryItem, SvgTestCategory testCategory)
        {
            int total = 0, unknowns = 0, failures = 0, successes = 0, partials = 0;

            writer.WriteStartElement("category");

            writer.WriteAttributeString("label", testCategory.Label);

            ItemCollection treeItems = categoryItem.Items;
            for (int j = 0; j < treeItems.Count; j++)
            {
                TreeViewItem treeItem = treeItems[j] as TreeViewItem;
                if (treeItem != null)
                {
                    SvgTestInfo testInfo = treeItem.Tag as SvgTestInfo;
                    if (testInfo != null)
                    {
                        testInfo.WriteXml(writer);

                        total++;
                        SvgTestState testState = testInfo.State;

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
                    }
                }
            }

            writer.WriteEndElement();

            testCategory.SetValues(total, unknowns, failures, successes, partials);
        }

        #endregion
    }
}
