using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Win32;

using IoPath = System.IO.Path;  
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace WpfTestSvgSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private delegate void FileChangedToUIThread(FileSystemEventArgs e);

        private string _titleBase;

        private bool _leftSplitterChanging;

        private string _drawingDir;

        private bool   _canDeleteXaml;

        private string _svgFilePath;
        private string _xamlFilePath;

        private FileSystemWatcher _fileWatcher;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;
        private DrawingPage _drawingPage;

        private BitmapImage _folderClose;
        private BitmapImage _folderOpen;
        private BitmapImage _fileThumbnail;

        #endregion

        #region Constructors and Destructor

        public MainWindow()
        {
            InitializeComponent();

            _titleBase = this.Title;

            leftExpander.Expanded  += new RoutedEventHandler(OnLeftExpanderExpanded);
            leftExpander.Collapsed += new RoutedEventHandler(OnLeftExpanderCollapsed);
            leftSplitter.MouseMove += new MouseEventHandler(OnLeftSplitterMove);

            this.Loaded   += new RoutedEventHandler(OnWindowLoaded);
            //this.Unloaded += new RoutedEventHandler(OnWindowUnloaded);
            this.Closing  += new CancelEventHandler(OnWindowClosing);

            _drawingDir = IoPath.Combine(IoPath.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), "XamlDrawings");

            if (!Directory.Exists(_drawingDir))
            {
                Directory.CreateDirectory(_drawingDir);
            }

            try
            {
                _folderClose = new BitmapImage();
                _folderClose.BeginInit();
                _folderClose.UriSource = new Uri("Images/FolderClose.png", UriKind.Relative);
                _folderClose.EndInit();

                _folderOpen = new BitmapImage();
                _folderOpen.BeginInit();
                _folderOpen.UriSource = new Uri("Images/FolderOpen.png", UriKind.Relative);
                _folderOpen.EndInit();

                _fileThumbnail = new BitmapImage();
                _fileThumbnail.BeginInit();
                _fileThumbnail.UriSource = new Uri("Images/Thumbnail.png", UriKind.Relative);
                _fileThumbnail.EndInit();
            }
            catch (Exception ex)
            {
                _folderClose = null;
                _folderOpen  = null;

                MessageBox.Show(ex.ToString(), "Wpf-Svg Test Sample",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

            this.Left = (width  - this.Width) / 2.0;
            this.Top  = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        #endregion

        #region Private Event Handlers

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            leftExpander.IsExpanded = false;

            // Retrieve the display pages...
            _svgPage     = frameSvgInput.Content as SvgPage;
            _xamlPage    = frameXamlOutput.Content as XamlPage;
            _drawingPage = frameDrawing.Content as DrawingPage;

            if (_drawingPage != null)
            {
                _drawingPage.XamlDrawingDir = _drawingDir;
            }

            this.txtSvgPath.Text = IoPath.GetFullPath(
                @".\Samples");
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_canDeleteXaml && !String.IsNullOrEmpty(_xamlFilePath) && File.Exists(_xamlFilePath))
            {
                File.Delete(_xamlFilePath);
            }
        }

        private void OnBrowseForSvgPath(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            dlg.Description = "Select the location of the SVG Files";

            string curDir = txtSvgPath.Text;
            if (!String.IsNullOrEmpty(curDir) && Directory.Exists(curDir))
            {
                dlg.SelectedPath = curDir;
            }
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSvgPath.Text = dlg.SelectedPath;
            }
        }

        private void OnBrowseForSvgFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter      = "SVG Files|*.svg;*.svgz"; ;
            dlg.FilterIndex = 1;

            bool? isSelected = dlg.ShowDialog();

            if (isSelected != null && isSelected.Value)
            {
                this.LoadFile(dlg.FileName);
            }
        }

        private void OnSvgPathTextChanged(object sender, TextChangedEventArgs e)
        {
            string selectePath = txtSvgPath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (String.IsNullOrEmpty(selectePath) || !Directory.Exists(selectePath))
            {
                return;
            }

            this.FillTreeView(selectePath);

            this.CloseFile();
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
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                return;
            }

            string svgFilePath = selItem.Tag as string;
            if (String.IsNullOrEmpty(svgFilePath) || !File.Exists(svgFilePath))
            {
                return;
            }

            e.Handled = true;

            this.CloseFile();

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                this.LoadFile(svgFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Wpf-Svg Test Sample",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;
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

        private void OnDragDrop(object sender, DragEventArgs de)
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
                fileName  = fileNames[0];
            }

            if (!String.IsNullOrEmpty(fileName))
            {
            }
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            this.CloseFile();

            try
            {
                this.Cursor      = Cursors.Wait;
                this.ForceCursor = true;

                this.LoadFile(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Wpf-Svg Test Sample",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor      = Cursors.Arrow;
                this.ForceCursor = false;
            }
        }

        #endregion

        #region LeftExpander/Splitter Event Handlers

        private void OnLeftExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            if (_leftSplitterChanging)
            {
                return;
            }

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];
            rowExpander.Width = new GridLength(24, GridUnitType.Pixel);
        }

        private void OnLeftExpanderExpanded(object sender, RoutedEventArgs e)
        {
            if (_leftSplitterChanging)
            {
                return;
            }

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];
            rowExpander.Width = new GridLength(280, GridUnitType.Pixel);
        }

        private void OnLeftSplitterMove(object sender, MouseEventArgs e)
        {
            _leftSplitterChanging = true;

            ColumnDefinition rowExpander = mainGrid.ColumnDefinitions[0];

            leftExpander.IsExpanded = rowExpander.ActualWidth > 24;

            _leftSplitterChanging = false;
        }

        #endregion

        #region FileSystemWatcher Event Handlers

        private void OnFileChangedToUIThread(FileSystemEventArgs e)
        {   
            // Stop watching.
            _fileWatcher.EnableRaisingEvents = false;

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                this.LoadFile(e.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Wpf-Svg Test Sample",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;
            }
        }

        // Define the event handlers.
        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                this.Dispatcher.BeginInvoke(new FileChangedToUIThread(OnFileChangedToUIThread),
                    System.Windows.Threading.DispatcherPriority.Normal, e);
            }
        }

        private void OnFileRenamed(object source, RenamedEventArgs e)
        {
        }

        #endregion

        #endregion

        #region Private Methods

        private void LoadFile(string fileName)
        {
            string fileExt = IoPath.GetExtension(fileName);
            if (String.IsNullOrEmpty(fileExt))
            {
                return;
            }

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
            }     

            if (String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                txtSvgFile.Text = fileName;
                txtSvgFileLabel.Text = "Selected SVG File:";

                _svgFilePath = fileName;

                if (_svgPage != null && 
                    (fillSvgInput.IsChecked != null && fillSvgInput.IsChecked.Value))
                {
                    _svgPage.LoadDocument(fileName);
                }

                try
                {
                    if (_drawingPage != null && _drawingPage.LoadDocument(fileName))
                    {
                        this.Title = _titleBase + " - " + IoPath.GetFileName(fileName);

                        if (_xamlPage != null && !String.IsNullOrEmpty(_drawingDir))
                        {
                            string xamlFilePath = IoPath.Combine(_drawingDir,
                                IoPath.GetFileNameWithoutExtension(fileName) + ".xaml");

                            _xamlFilePath  = xamlFilePath;
                            _canDeleteXaml = true;

                            if (File.Exists(xamlFilePath) &&
                                (fillXamlOutput.IsChecked != null && fillXamlOutput.IsChecked.Value))
                            {
                                _xamlPage.LoadDocument(xamlFilePath);

                                // Delete the file after loading it...
                                File.Delete(xamlFilePath);
                            }
                        }

                        if (_fileWatcher == null)
                        {
                            // Create a new FileSystemWatcher and set its properties.
                            _fileWatcher = new FileSystemWatcher();
                            // Watch for changes in LastAccess and LastWrite times
                            _fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;

                            _fileWatcher.IncludeSubdirectories = false;

                            // Add event handlers.
                            _fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
                            _fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
                            _fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
                            _fileWatcher.Renamed += new RenamedEventHandler(OnFileRenamed);
                        }

                        _fileWatcher.Path = IoPath.GetDirectoryName(fileName);
                        // Only watch current file
                        _fileWatcher.Filter = IoPath.GetFileName(fileName);
                        // Begin watching.
                        _fileWatcher.EnableRaisingEvents = true;
                    }
                }
                catch
                {
                    // Try loading the XAML, if generated but the rendering failed...
                    if (_xamlPage != null && !String.IsNullOrEmpty(_drawingDir))
                    {
                        string xamlFilePath = IoPath.Combine(_drawingDir,
                            IoPath.GetFileNameWithoutExtension(fileName) + ".xaml");

                        _xamlFilePath  = xamlFilePath;
                        _canDeleteXaml = true;

                        if (File.Exists(xamlFilePath) &&
                            (fillXamlOutput.IsChecked != null && fillXamlOutput.IsChecked.Value))
                        {
                            _xamlPage.LoadDocument(xamlFilePath);

                            // Delete the file after loading it...
                            File.Delete(xamlFilePath);
                        }
                    }
                    throw;
                }
            }
            else if (String.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
            {
                txtSvgFile.Text = fileName;
                txtSvgFileLabel.Text = "Selected XAML File:";

                if (_svgPage != null)
                {
                    _svgPage.UnloadDocument();
                    _svgFilePath = null;
                }

                try
                {
                    if (_drawingPage != null && _drawingPage.LoadDocument(fileName))
                    {
                        this.Title = _titleBase + " - " + IoPath.GetFileName(fileName);

                        _xamlFilePath  = fileName;
                        _canDeleteXaml = false;

                        if (_xamlPage != null)
                        {
                            _xamlPage.LoadDocument(fileName);
                        }
                    }
                }
                catch
                {     
                    if (_xamlPage != null)
                    {
                        _xamlFilePath  = fileName;
                        _canDeleteXaml = false;

                        _xamlPage.LoadDocument(fileName);
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

                if (_canDeleteXaml && !String.IsNullOrEmpty(_xamlFilePath) && File.Exists(_xamlFilePath))
                {
                    File.Delete(_xamlFilePath);
                }

                _svgFilePath   = null;
                _xamlFilePath  = null;
                _canDeleteXaml = false;
            }
            catch
            {
            }
        }

        #region FillTreeView Methods

        private void FillTreeView(string sourceDir)
        {
            if (String.IsNullOrEmpty(sourceDir) || !Directory.Exists(sourceDir))
            {
                return;
            }

            string[] svgFiles = Directory.GetFiles(sourceDir, "*.svg", SearchOption.TopDirectoryOnly);

            treeView.BeginInit();
            treeView.Items.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);

            TextBlock headerText = new TextBlock();
            headerText.Text = directoryInfo.Name;
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
                bullet.Width = 16;
                bullet.Fill = Brushes.Goldenrod;
                bullet.Stroke = Brushes.DarkGray;
                bullet.StrokeThickness = 1;

                decorator.Bullet = bullet;
            }
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child  = headerText;

            TreeViewItem categoryItem = new TreeViewItem();
            categoryItem.Tag        = String.Empty;
            categoryItem.Header     = decorator;
            categoryItem.Margin     = new Thickness(0);
            categoryItem.Padding    = new Thickness(3);
            categoryItem.FontSize   = 14;
            categoryItem.FontWeight = FontWeights.Bold;

            treeView.Items.Add(categoryItem);

            FillTreeView(sourceDir, svgFiles, categoryItem);

            categoryItem.IsExpanded = true;

            treeView.EndInit();

            leftExpander.IsExpanded = true;
        }

        private void FillTreeView(string sourceDir, string[] svgFiles, TreeViewItem treeItem)
        {     
            if (svgFiles != null && svgFiles.Length != 0)
            {
                for (int i = 0; i < svgFiles.Length; i++)
                {
                    string svgFile = svgFiles[i];

                    TextBlock itemText = new TextBlock();
                    itemText.Text   = String.Format("({0:D3}) - {1}", i, IoPath.GetFileName(svgFile));
                    itemText.Margin = new Thickness(3, 0, 0, 0);

                    BulletDecorator fileItem = new BulletDecorator();
                    if (_fileThumbnail != null)
                    {
                        Image image = new Image();
                        image.Source  = _fileThumbnail;
                        image.Height  = 16;
                        image.Width   = 16;

                        fileItem.Bullet = image;
                    }
                    else
                    {
                        Ellipse bullet = new Ellipse();
                        bullet.Height = 16;
                        bullet.Width  = 16;
                        bullet.Fill   = Brushes.Goldenrod;
                        bullet.Stroke = Brushes.DarkGray;
                        bullet.StrokeThickness = 1;

                        fileItem.Bullet = bullet;
                    }
                    fileItem.Margin = new Thickness(0, 0, 10, 0);
                    fileItem.Child = itemText;

                    TreeViewItem item = new TreeViewItem();
                    item.Tag        = svgFile;
                    item.Header     = fileItem;
                    item.Margin     = new Thickness(0);
                    item.Padding    = new Thickness(2);
                    item.FontSize   = 12;
                    item.FontWeight = FontWeights.Normal;

                    treeItem.Items.Add(item);
                }
            }

            if (String.IsNullOrEmpty(sourceDir) || !Directory.Exists(sourceDir))
            {
                return;
            }

            string[] directories = Directory.GetDirectories(sourceDir);
            if (directories != null && directories.Length != 0)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    string directory = directories[i];
                    svgFiles = Directory.GetFiles(directory, "*.svg", SearchOption.TopDirectoryOnly);
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                        TextBlock headerText = new TextBlock();
                        headerText.Text = directoryInfo.Name;
                        headerText.Margin = new Thickness(3, 0, 0, 0);

                        BulletDecorator decorator = new BulletDecorator();
                        if (_folderClose != null)
                        {
                            Image image = new Image();
                            image.Source = _folderClose;

                            decorator.Bullet = image;
                        }
                        else
                        {
                            Ellipse bullet = new Ellipse();
                            bullet.Height = 16;
                            bullet.Width = 16;
                            bullet.Fill = Brushes.Goldenrod;
                            bullet.Stroke = Brushes.DarkGray;
                            bullet.StrokeThickness = 1;

                            decorator.Bullet = bullet;
                        }
                        decorator.Margin = new Thickness(0, 0, 10, 0);
                        decorator.Child = headerText;

                        TreeViewItem categoryItem = new TreeViewItem();
                        categoryItem.Tag        = String.Empty;
                        categoryItem.Header     = decorator;
                        categoryItem.Margin     = new Thickness(0);
                        categoryItem.Padding    = new Thickness(3);
                        categoryItem.FontSize   = 14;
                        categoryItem.FontWeight = FontWeights.Bold;

                        treeItem.Items.Add(categoryItem);

                        FillTreeView(directory, svgFiles, categoryItem);

                        if (!categoryItem.HasItems)
                        {
                            treeItem.Items.Remove(categoryItem);
                        }
                    }
                }
            }
        }

        #endregion

        private void OnFillSvgInputChecked(object sender, RoutedEventArgs e)
        {
            if (_svgPage == null)
            {
                return;
            }

            if (File.Exists(_svgFilePath) &&
                (fillSvgInput.IsChecked != null && fillSvgInput.IsChecked.Value))
            {
                _svgPage.LoadDocument(_svgFilePath);
            }
        }

        private void OnFillXamlOutputChecked(object sender, RoutedEventArgs e)
        {
            if (_xamlPage == null)
            {
                return;
            }

            if (File.Exists(_xamlFilePath) &&
                (fillXamlOutput.IsChecked != null && fillXamlOutput.IsChecked.Value))
            {
                _xamlPage.LoadDocument(_xamlFilePath);
            }
        }

        #endregion
    }
}
