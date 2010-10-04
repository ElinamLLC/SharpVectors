using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

using IoPath = System.IO.Path;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private const string SvgTestSuite = "SvgTestSuite.xml";

        private bool   _isTreeModified;

        private bool   _isTreeChangedPending;

        private bool   _isSuiteAvailable;
        private bool   _leftSplitterChanging;
        private bool   _isBottomSplitterChanging;

        private string _suitePath;
        private string _svgPath;
        private string _pngPath;

        private string _testFileName;

        private string _drawingDir;

        private SvgPage     _svgPage;
        private XamlPage    _xamlPage;
        private DrawingPage _drawingPage;
        
        private BitmapImage _folderClose;
        private BitmapImage _folderOpen;

        #endregion

        #region Constructors and Destructor

        public MainWindow()
        {
            InitializeComponent();

            leftExpander.Expanded    += new RoutedEventHandler(OnLeftExpanderExpanded);
            leftExpander.Collapsed   += new RoutedEventHandler(OnLeftExpanderCollapsed);
            leftSplitter.MouseMove   += new MouseEventHandler(OnLeftSplitterMove);  

            bottomExpander.Expanded  += new RoutedEventHandler(OnBottomExpanderExpanded);
            bottomExpander.Collapsed += new RoutedEventHandler(OnBottomExpanderCollapsed);
            bottomSplitter.MouseMove += new MouseEventHandler(OnBottomSplitterMove);

            this.Loaded  += new RoutedEventHandler(OnWindowLoaded);
            this.Closing += new CancelEventHandler(OnWindowClosing);

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

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double width  = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            this.Width  = Math.Min(1600, width) * 0.85;
            this.Height = height * 0.90;

            this.Left = (width  - this.Width) / 2.0;
            this.Top  = (height - this.Height) / 2.0;

            this.WindowStartupLocation = WindowStartupLocation.Manual;

            ColumnDefinition colExpander = mainGrid.ColumnDefinitions[0];
            colExpander.Width = new GridLength(280, GridUnitType.Pixel); 

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
            _svgPage     = frameSvgInput.Content   as SvgPage;
            _xamlPage    = frameXamlOutput.Content as XamlPage;
            _drawingPage = frameDrawing.Content    as DrawingPage;

            if (_drawingPage != null)
            {
                _drawingPage.XamlDrawingDir = _drawingDir;
            }

            string currentDir = IoPath.GetFullPath("FullTestSuite"); 

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
            if (!_isTreeModified || String.IsNullOrEmpty(_testFileName) ||
                !File.Exists(_testFileName))
            {
                return;
            }

            string backupFile = IoPath.ChangeExtension(_testFileName, ".bak");
            try
            {
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
                File.Move(_testFileName, backupFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Svg Test Suite - Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent      = true;
                settings.IndentChars = "    ";
                settings.Encoding    = Encoding.UTF8;

                using (XmlWriter writer = XmlWriter.Create(_testFileName, settings))
                {
                    this.SaveTreeView(writer);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(backupFile))    
                {
                    File.Move(backupFile, _testFileName);
                }

                MessageBox.Show(ex.ToString(), "Svg Test Suite - Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void OnSvgSuitePathTextChanged(object sender, TextChangedEventArgs e)
        {
            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (String.IsNullOrEmpty(selectePath) || !Directory.Exists(selectePath))
            {
                return;
            }

            //if (!File.Exists(IoPath.Combine(selectePath, "index.html")))
            //{
            //    return;
            //}
            if (!Directory.Exists(IoPath.Combine(selectePath, "svg")))
            {
                return;
            }
            if (!Directory.Exists(IoPath.Combine(selectePath, "png")))
            {
                return;
            }
            //if (!Directory.Exists(IoPath.Combine(selectePath, "svggen")))
            //{
            //    return;
            //}

            this.InitializePath(selectePath);
        }

        private void EnableTestPanel(bool isEnabled)
        {
            testInfoPanel.IsEnabled = isEnabled;
            if (!isEnabled)
            {
                stateComboBox.SelectedIndex = -1;
                testComment.Text            = String.Empty;

                testFilePath.Text           = String.Empty;
                testDescrition.Text         = String.Empty;

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
            TestInfo testInfo = treeItem.Tag as TestInfo;
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

            testInfo.State   = (TestState)selIndex;
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
            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                EnableTestPanel(false);
                return;
            }

            TestInfo testItem = selItem.Tag as TestInfo;
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
                    _svgPage.LoadDocument(svgFilePath);
                }
            }
            catch (Exception ex)
            {
                _isTreeChangedPending = false;
                this.Cursor = Cursors.Arrow;

                MessageBox.Show(ex.ToString(), "Svg Test Suite - Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            try
            {
                if (_drawingPage != null)
                {
                    _drawingPage.LoadDocument(svgFilePath, pngFilePath);
                }

                if (_xamlPage != null && !String.IsNullOrEmpty(_drawingDir))
                {
                    string xamlFilePath = IoPath.Combine(_drawingDir,
                        IoPath.ChangeExtension(testItem.FileName, ".xaml"));

                    if (File.Exists(xamlFilePath))
                    {
                        _xamlPage.LoadDocument(xamlFilePath);

                        // Delete the file after loading it...
                        File.Delete(xamlFilePath);
                    }
                }

                testFilePath.Text = svgFilePath;

                testDescrition.Text = testItem.Description;

                stateComboBox.SelectedIndex = (int)testItem.State;
                testComment.Text            = testItem.Comment;

                _isTreeChangedPending = false;
                testApply.IsEnabled   = false;
                EnableTestPanel(true);
            }
            catch (Exception ex)
            {
                //EnableTestPanel(false);
                _isTreeChangedPending = false;

                MessageBox.Show(ex.ToString(), "Svg Test Suite - Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }  
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
                    TestInfo testInfo = treeItem.Tag as TestInfo;
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
            rowExpander.Width = new GridLength(280, GridUnitType.Pixel);
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
            rowExpander.Height = new GridLength(200, GridUnitType.Pixel);
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

        private void InitializePath(string suitePath)
        {   
            if (String.IsNullOrEmpty(suitePath) || !Directory.Exists(suitePath))
            {
                return;
            }

            //string indexFilePath = IoPath.Combine(suitePath, "index.html");
            //if (!File.Exists(indexFilePath))
            //{
            //    return;
            //}

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

            string fileName = IoPath.GetFullPath(SvgTestSuite);
            if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false;
                settings.IgnoreComments   = true;
                settings.IgnoreProcessingInstructions = true;

                using (XmlReader reader = XmlReader.Create(fileName, settings))
                {
                    LoadTreeView(reader);
                }

                leftExpander.IsExpanded = true;

                _testFileName = fileName;
            }   
        }

        private void LoadTreeView(XmlReader reader)
        {
            treeView.BeginInit();
            treeView.Items.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    String.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                {
                    // <category label="Animations">
                    string category = reader.GetAttribute("label");
                    if (!String.IsNullOrEmpty(category))
                    {
                        TextBlock headerText = new TextBlock();
                        headerText.Text   = category;
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

                        LoadTreeViewCategory(reader, categoryItem);
                    }
                }
            }

            treeView.EndInit();
        }

        private void LoadTreeViewCategory(XmlReader reader, TreeViewItem categoryItem)
        {
            int itemCount = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "test", StringComparison.OrdinalIgnoreCase))
                    {
                        TestInfo testInfo = new TestInfo(reader);
                        if (!testInfo.IsEmpty)
                        {
                            TextBlock headerText = new TextBlock();
                            headerText.Text   = String.Format("({0:D3}) - {1}", itemCount, testInfo.Title);
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
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {   
                    if (String.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
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
                    writer.WriteStartElement("category");

                    writer.WriteAttributeString("label", headerText.Text);

                    ItemCollection treeItems = categoryItem.Items;
                    for (int j = 0; j < treeItems.Count; j++)
                    {
                        TreeViewItem treeItem = treeItems[j] as TreeViewItem;
                        if (treeItem != null)
                        {
                            TestInfo testInfo = treeItem.Tag as TestInfo;
                            if (testInfo != null)
                            {
                                testInfo.WriteXml(writer);
                            }
                        }
                    }

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        #endregion

        #region TestInfo Class

        private enum TestState
        {
            Unknown = 0,
            Failure = 1,
            Success = 2,
            Partial = 3
        }

        [Serializable]
        private sealed class TestInfo : IXmlSerializable
        {
            #region Private Fields

            private string _fileName;
            private string _title;
            private string _comment;
            private string _description;
            private TestState _state;

            #endregion

            #region Constructors and Destructor

            public TestInfo()
            {
                _title       = String.Empty;
                _fileName    = String.Empty;
                _description = String.Empty;
                _comment     = String.Empty;
                _state       = TestState.Unknown;
            }

            public TestInfo(XmlReader reader)
                : this()
            {
                this.ReadXml(reader);
            }

            public TestInfo(string fileName, string title, string state, 
                string comment, string description)
                : this()
            {
                this.Initialize(fileName, title, state, comment, description);
            }

            #endregion

            #region Public Fields

            public bool IsEmpty
            {
                get
                {
                    return (String.IsNullOrEmpty(_fileName));
                }
            }

            public TestState State
            {
                get { return _state; }
                set { _state = value; }
            }

            public Brush StateBrush
            {
                get
                {
                    switch (_state)
                    {
                        case TestState.Unknown:
                            return Brushes.LightGray;
                        case TestState.Failure:
                            return Brushes.Red;
                        case TestState.Success:
                            return Brushes.Green;
                        case TestState.Partial:
                            return Brushes.Yellow;
                    }

                    return Brushes.LightGray;
                }
            }

            public string Comment
            {
                get { return _comment; }
                set { _comment = value; }
            }

            public string FileName
            {
                get { return _fileName; }
                set { _fileName = value; }
            }

            public string Title
            {
                get { return _title; }
                set 
                {
                    if (value == null)
                    {
                        value = String.Empty;
                    }
                    _title = value; 
                }
            }

            public string Description
            {
                get 
                { 
                    return _description; 
                }
                set 
                {
                    if (value == null)
                    {
                        value = String.Empty;
                    }
                    _description = value; 
                }
            }

            #endregion

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                if (reader == null || reader.NodeType != XmlNodeType.Element)
                {
                    return;
                }
                if (!String.Equals(reader.Name, "test", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // <test source="*.svg" title="" state="partial" comment="" description="" />
                string source = reader.GetAttribute("source");
                if (!String.IsNullOrEmpty(source))
                {
                    string title = reader.GetAttribute("title");
                    if (String.IsNullOrEmpty(title))
                    {
                        title = source.Replace(".svg", String.Empty);
                    }
                    string state = reader.GetAttribute("state");
                    string comment = reader.GetAttribute("comment");
                    string description = reader.GetAttribute("description");

                    this.Initialize(source, title, state, comment, description);
                }
            }

            public void WriteXml(XmlWriter writer)
            {
                if (writer == null)
                {
                    return;
                }

                // <test source="*.svg" title="" state="partial" comment="" description="" />
                writer.WriteStartElement("test");
                writer.WriteAttributeString("source", _fileName);
                writer.WriteAttributeString("title", _title);
                writer.WriteAttributeString("state", _state.ToString().ToLowerInvariant());
                writer.WriteAttributeString("comment", _comment);
                writer.WriteAttributeString("description", _description);
                writer.WriteEndElement();
            }

            #endregion

            #region Private Methods

            private void Initialize(string fileName, string title, string state, 
                string comment, string description)
            {
                if (description == null)
                {
                    description = String.Empty;
                }

                _fileName = fileName;
                _title = title;
                _comment = comment;
                _description = description.Trim();
                if (!String.IsNullOrEmpty(_description))
                {
                    _description = _description.Replace("\n", " ");
                    _description = _description.Replace("  ", String.Empty);
                }

                if (!String.IsNullOrEmpty(state))
                {
                    if (state.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                    {
                        _state = TestState.Unknown;
                    }
                    else if (state.Equals("failure", StringComparison.OrdinalIgnoreCase))
                    {
                        _state = TestState.Failure;
                    }
                    else if (state.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        _state = TestState.Success;
                    }
                    else if (state.Equals("partial", StringComparison.OrdinalIgnoreCase))
                    {
                        _state = TestState.Partial;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
