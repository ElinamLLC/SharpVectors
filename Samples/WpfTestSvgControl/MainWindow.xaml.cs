using System;
using System.IO;
using System.IO.Compression;
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

namespace WpfTestSvgControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private const int LeftPane       = 350;
        private const int LeftBottomPane = 300;

        private const string AppTitle        = "SharpVectors: WPF Testing SVG";
        private const string AppErrorTitle   = "SharpVectors: WPF Testing SVG - Error";
        private const string SvgTestSettings = "SvgTestSettings.xml";

        private const string SvgFilePattern  = "*.svg*";

        private delegate void FileChangedToUIThread(FileSystemEventArgs e);

        private bool _leftSplitterChanging;
        private bool _isBottomSplitterChanging;

        private string _drawingDir;

        private bool _isShown;
        private bool _canDeleteXaml;

        private string _testSettingsPath;
        private string _svgFilePath;
        private string _xamlFilePath;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;
        private DrawingPage _drawingPage;
        private DebugPage _debugPage;
        private SettingsPage _settingsPage;

        private ImageSource _folderClose;
        private ImageSource _folderOpen;
        private ImageSource _fileThumbnail;

        private OptionSettings _optionSettings;

        #endregion

        #region Constructors and Destructor

        public MainWindow()
        {
            InitializeComponent();

            leftExpander.Expanded  += OnLeftExpanderExpanded;
            leftExpander.Collapsed += OnLeftExpanderCollapsed;
            leftSplitter.MouseMove += OnLeftSplitterMove;

            bottomExpander.Expanded  += OnBottomExpanderExpanded;
            bottomExpander.Collapsed += OnBottomExpanderCollapsed;
            bottomSplitter.MouseMove += OnBottomSplitterMove;

            this.Loaded   += OnWindowLoaded;
            this.Unloaded += OnWindowUnloaded;
            this.Closing  += OnWindowClosing;

            _drawingDir = IoPath.Combine(IoPath.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), DrawingPage.TemporalDirName);

            if (!Directory.Exists(_drawingDir))
            {
                Directory.CreateDirectory(_drawingDir);
            }

            _optionSettings = new OptionSettings();
            _testSettingsPath = IoPath.GetFullPath(SvgTestSettings);
            if (!string.IsNullOrWhiteSpace(_testSettingsPath) && File.Exists(_testSettingsPath))
            {
                _optionSettings.Load(_testSettingsPath);
                // Override any saved local directory, default to sample files.
                _optionSettings.CurrentSvgPath = _optionSettings.DefaultSvgPath;
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
        }

        #endregion

        #region Public Properties

        public OptionSettings OptionSettings
        {
            get {
                return _optionSettings;
            }
            set {
                if (value != null)
                {
                    _optionSettings = value;
                    if (_drawingPage != null)
                    {
                        _drawingPage.ConversionSettings = value.ConversionSettings;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public async Task BrowseForFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Title       = "Select An SVG File";
            dlg.DefaultExt  = "*.svg";
            dlg.Filter      = "All SVG Files (*.svg,*.svgz)|*.svg;*.svgz"
                                + "|Svg Uncompressed Files (*.svg)|*.svg"
                                + "|SVG Compressed Files (*.svgz)|*.svgz";

            bool? isSelected = dlg.ShowDialog();

            if (isSelected != null && isSelected.Value)
            {
                this.CloseFile();

                await this.LoadFile(dlg.FileName);

                TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
                if (selItem == null || selItem.Tag == null)
                {
                    return;
                }
                selItem.IsSelected = false;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double width  = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            this.Width  = Math.Min(1600, width) * 0.85;
            this.Height = height * 0.85;

            this.Left = (width - this.Width) / 2.0;
            this.Top  = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            ColumnDefinition colExpander = mainGrid.ColumnDefinitions[0];
            colExpander.Width = new GridLength(LeftPane, GridUnitType.Pixel);

            RowDefinition rowExpander = bottomGrid.RowDefinitions[2];
            rowExpander.Height = new GridLength(LeftBottomPane, GridUnitType.Pixel);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_isShown)
                return;

            _isShown = true;
        }

        #endregion

        #region Private Event Handlers

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            bottomExpander.IsExpanded = true;
            leftExpander.IsExpanded   = true;

            // Retrieve the display pages...
            _svgPage      = frameSvgInput.Content as SvgPage;
            _xamlPage     = frameXamlOutput.Content as XamlPage;
            _drawingPage  = frameDrawing.Content as DrawingPage;
            _debugPage    = frameDebugging.Content as DebugPage;
            _settingsPage = frameSettings.Content as SettingsPage;

            if (_svgPage != null)
            {
                _svgPage.MainWindow = this;
            }
            if (_xamlPage != null)
            {
                _xamlPage.MainWindow = this;
            }
            if (_drawingPage != null)
            {
                _drawingPage.WorkingDrawingDir = _drawingDir;
                _drawingPage.MainWindow = this;
            }
            if (_debugPage != null)
            {
                _debugPage.MainWindow = this;
                _debugPage.Startup();
            }
            if (_settingsPage != null)
            {
                _settingsPage.MainWindow = this;
            }

            tabSvgInput.Visibility   = _optionSettings.ShowInputFile ? Visibility.Visible : Visibility.Collapsed;
            tabXamlOutput.Visibility = _optionSettings.ShowOutputFile ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
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

            try
            {
                if (_canDeleteXaml && !string.IsNullOrWhiteSpace(_xamlFilePath) && File.Exists(_xamlFilePath))
                {
                    File.Delete(_xamlFilePath);
                }
                if (!string.IsNullOrWhiteSpace(_drawingDir) && Directory.Exists(_drawingDir))
                {
                    string[] imageFiles = Directory.GetFiles(_drawingDir, "*.png");
                    if (imageFiles != null && imageFiles.Length != 0)
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            File.Delete(imageFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            if (_debugPage != null)
            {
                _debugPage.Shutdown();
            }
        }

        private async void OnBrowseForSvgFile(object sender, RoutedEventArgs e)
        {
            await this.BrowseForFile();
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
            if (_xamlPage == null || string.IsNullOrWhiteSpace(_xamlFilePath))
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

                    if (!File.Exists(_xamlFilePath))
                    {
                        if (!_drawingPage.SaveDocument(_xamlFilePath))
                        {
                            return;
                        }
                    }

                    if (File.Exists(_xamlFilePath))
                    {
                        _xamlPage.LoadDocument(_xamlFilePath);
                    }
                    else
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
            if (sender == tabDrawing)
            {
                if (_drawingPage != null)
                {
                    _drawingPage.PageSelected(true);
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

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (_drawingPage == null)
            {
                return;
            }

            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                _drawingPage.SelectElement(null);
                return;
            }

            string selectedName = selItem.Tag as string;
            if (string.IsNullOrWhiteSpace(selectedName))
            {
                _drawingPage.SelectElement(null);
                return;
            }

            e.Handled = true;

            treeView.IsEnabled = false;

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                _drawingPage.SelectElement(selectedName);
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
        }

        private void OnTreeViewItemCollapsed(object sender, RoutedEventArgs e)
        {
            if (_folderClose == null)
            {
                return;
            }

            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            if (treeItem == null || (treeItem.Tag != null
                && !string.IsNullOrWhiteSpace(treeItem.Tag.ToString())))
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
            if (treeItem == null || (treeItem.Tag != null 
                && !string.IsNullOrWhiteSpace(treeItem.Tag.ToString())))
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

        #region Drag/Drop Methods

        private void OnDragEnter(object sender, DragEventArgs de)
        {
            if (de.Data.GetDataPresent(DataFormats.Text) ||
               de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                de.Effects = DragDropEffects.Copy;
            }
            else
            {
                de.Effects = DragDropEffects.None;
            }
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {

        }

        private async void OnDragDrop(object sender, DragEventArgs de)
        {
            string fileName = "";
            if (de.Data.GetDataPresent(DataFormats.Text))
            {
                fileName = (string)de.Data.GetData(DataFormats.Text);
            }
            else if (de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames;
                fileNames = (string[])de.Data.GetData(DataFormats.FileDrop);
                fileName = fileNames[0];
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
            }
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return;
            }

            this.CloseFile();

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                await this.LoadFile(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;
            }
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

        private async Task LoadFile(string fileName)
        {
            string fileExt = IoPath.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return;
            }

            bool generateXaml = _optionSettings.ShowOutputFile;

            if (string.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase))
            {
                _svgFilePath = fileName;

                if (_svgPage != null && _optionSettings.ShowInputFile)
                {
                    _svgPage.LoadDocument(fileName);
                }

                if (_drawingPage == null)
                {
                    return;
                }
                _drawingPage.SaveXaml = generateXaml;

                try
                {
                    if (await _drawingPage.LoadDocumentAsync(fileName))
                    {
                        this.Title = AppTitle + " - " + IoPath.GetFileName(fileName);

                        if (_xamlPage != null && !string.IsNullOrWhiteSpace(_drawingDir))
                        {
                            string xamlFilePath = IoPath.Combine(_drawingDir,
                                IoPath.GetFileNameWithoutExtension(fileName) + SvgConverter.XamlExt);

                            _xamlFilePath = xamlFilePath;
                            _canDeleteXaml = true;

                            if (File.Exists(xamlFilePath) && _optionSettings.ShowOutputFile)
                            {
                                _xamlPage.LoadDocument(xamlFilePath);
                            }
                        }

                        var drawingDocument = _drawingPage.DrawingDocument;

                        this.FillTreeView(drawingDocument);
                    }
                }
                catch
                {
                    // Try loading the XAML, if generated but the rendering failed...
                    if (_xamlPage != null && !string.IsNullOrWhiteSpace(_drawingDir))
                    {
                        string xamlFilePath = IoPath.Combine(_drawingDir,
                            IoPath.GetFileNameWithoutExtension(fileName) + SvgConverter.XamlExt);

                        _xamlFilePath  = xamlFilePath;
                        _canDeleteXaml = true;

                        if (File.Exists(xamlFilePath) && _optionSettings.ShowOutputFile)
                        {
                            _xamlPage.LoadDocument(xamlFilePath);
                        }
                    }
                    throw;
                }
            }
        }

        private void CloseFile()
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
                if (_drawingPage != null)
                {
                    _drawingPage.UnloadDocument();
                }

                if (_canDeleteXaml && !string.IsNullOrWhiteSpace(_xamlFilePath) && File.Exists(_xamlFilePath))
                {
                    File.Delete(_xamlFilePath);
                }
                if (!string.IsNullOrWhiteSpace(_drawingDir) && Directory.Exists(_drawingDir))
                {
                    string[] imageFiles = Directory.GetFiles(_drawingDir, "*.png");
                    if (imageFiles != null && imageFiles.Length != 0)
                    {
                        try
                        {
                            foreach (var imageFile in imageFiles)
                            {
                                if (File.Exists(imageFile))
                                {
                                    File.Delete(imageFile);
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            Trace.TraceError(ex.ToString());
                            // Image this, WPF will typically cache and/or lock loaded images
                        }
                    }
                }

                _svgFilePath   = null;
                _xamlFilePath  = null;
                _canDeleteXaml = false;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #region FillTreeView Methods

        private void FillTreeView(WpfDrawingDocument drawingDocument)
        {
            if (drawingDocument == null)
            {
                return;
            }

            treeView.BeginInit();
            treeView.Items.Clear();

            for (int i = 0; i < 1; i++)
            {
                TextBlock headerText = new TextBlock();
                headerText.Text   = i == 0 ? "SVG Element Names" : "SVG Element Unique Names";
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
                decorator.Tag   = "";

                TreeViewItem categoryItem = new TreeViewItem();
                categoryItem.Tag        = string.Empty;
                categoryItem.Header     = decorator;
                categoryItem.Margin     = new Thickness(0);
                categoryItem.Padding    = new Thickness(3);
                categoryItem.FontSize   = 14;
                categoryItem.FontWeight = FontWeights.Bold;

                treeView.Items.Add(categoryItem);

                // FillTreeView(i == 0 ? drawingDocument.ElementNames : drawingDocument.ElementUniqueNames, categoryItem);
                FillTreeView(i == 0 ? drawingDocument.DrawingNames : drawingDocument.DrawingUniqueNames, categoryItem);

                categoryItem.IsExpanded = (i == 0);
            }

            treeView.EndInit();

            leftExpander.IsExpanded   = true;
            bottomExpander.IsExpanded = true;
        }

        private void FillTreeView(ICollection<string> idItems, TreeViewItem treeItem)
        {
            if (idItems == null || idItems.Count == 0)
            {
                return;
            }

            int itemCount = 0;

            foreach (var idItem in idItems)
            {
                TextBlock itemText = new TextBlock();
                itemText.Text   = string.Format("({0:D3}) - {1}", itemCount, idItem);
                itemText.Margin = new Thickness(3, 0, 0, 0);

                BulletDecorator fileItem = new BulletDecorator();
                if (_fileThumbnail != null)
                {
                    Image image  = new Image();
                    image.Source = _fileThumbnail;
                    image.Height = 16;
                    image.Width  = 16;

                    fileItem.Bullet = image;
                }
                else
                {
                    Ellipse bullet = new Ellipse();
                    bullet.Height          = 16;
                    bullet.Width           = 16;
                    bullet.Fill            = Brushes.Goldenrod;
                    bullet.Stroke          = Brushes.DarkGray;
                    bullet.StrokeThickness = 1;

                    fileItem.Bullet = bullet;
                }
                fileItem.Margin = new Thickness(0, 0, 10, 0);
                fileItem.Child = itemText;

                TreeViewItem item = new TreeViewItem();
                item.Tag        = idItem;
                item.Header     = fileItem;
                item.Margin     = new Thickness(0);
                item.Padding    = new Thickness(2);
                item.FontSize   = 12;
                item.FontWeight = FontWeights.Normal;

                treeItem.Items.Add(item);

                itemCount++;
            }
        }

        #endregion

        #endregion
    }
}
