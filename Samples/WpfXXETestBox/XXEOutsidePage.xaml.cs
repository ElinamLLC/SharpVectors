﻿using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;

using Microsoft.Win32;
using IoPath = System.IO.Path;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Indentation;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace WpfXXETestBox
{
    /// <summary>
    /// Interaction logic for XXEOutsidePage.xaml
    /// </summary>
    public partial class XXEOutsidePage : Page
    {
        #region Private Fields

        private const string OutsidePage  = "xxe.svg";
        private const string TestFileName = "xxe-test.svg";
        private const string BackFileName = "xxe-test.bak";

        private const string XmlFileName       = "xxe.xml";
        private const string OtherFileName     = "xxe-other.xml";
        private const string BackOtherFileName = "xxe-other.bak";

        private const string AppTitle = "Svg Test Box";
        private const string AppErrorTitle = "Svg Test Box - Error";

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private DrawingGroup _currentDrawing;

        private string _currentFileName;

        private DirectoryInfo _directoryInfo;

        private string _svgFilePath;
        private string _xmlFilePath;

        private string _testFilePath;
        private string _backFilePath;

        private string _otherFilePath;
        private string _otherBackFilePath;

        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        private EmbeddedImageSerializerVisitor _embeddedImageVisitor;
        private IList<EmbeddedImageSerializerArgs> _embeddedImages;

        private bool documentInitialized;
        private bool toolbarVisibility;

        #endregion

        #region Constructors and Destructor

        public XXEOutsidePage()
        {
            InitializeComponent();

            toolbarVisibility = true;

            string workingDir = IoPath.GetFullPath("..\\");

            _svgFilePath = IoPath.Combine(workingDir, OutsidePage);
            _testFilePath = IoPath.Combine(workingDir, TestFileName);
            _backFilePath = IoPath.Combine(workingDir, BackFileName);

            _xmlFilePath = IoPath.Combine(workingDir, XmlFileName);
            _otherFilePath = IoPath.Combine(workingDir, OtherFileName);
            _otherBackFilePath = IoPath.Combine(workingDir, BackOtherFileName);

            _directoryInfo = new DirectoryInfo(workingDir);

            _wpfSettings = new WpfDrawingSettings();

            _embeddedImages = new List<EmbeddedImageSerializerArgs>();

            _embeddedImageVisitor = new EmbeddedImageSerializerVisitor(true);
            _wpfSettings.Visitors.ImageVisitor = _embeddedImageVisitor;

            _embeddedImageVisitor.ImageCreated += OnEmbeddedImageCreated;

            _fileReader = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            otherEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            TextEditorOptions options = textEditor.Options;
            if (options != null)
            {
                options.EnableHyperlinks = true;
                options.EnableEmailHyperlinks = true;
                options.EnableVirtualSpace = false;
                options.HighlightCurrentLine = true;
            }
            options = otherEditor.Options;
            if (options != null)
            {
                options.EnableHyperlinks = true;
                options.EnableEmailHyperlinks = true;
                options.EnableVirtualSpace = false;
                options.HighlightCurrentLine = false;
            }

            textEditor.ShowLineNumbers = true;
            otherEditor.ShowLineNumbers = true;

            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            _searchPanel = SearchPanel.Install(textEditor);

            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            this.Loaded += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Properties

        public bool ToolbarVisibility
        {
            get {
                return toolbarVisibility;
            }
            set {
                toolbarVisibility = value;

                if (toolBar != null)
                {
                    toolBar.Visibility = toolbarVisibility ? Visibility.Visible : Visibility.Collapsed;
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
                    _wpfSettings = value;
                    _wpfSettings.Visitors.ImageVisitor = _embeddedImageVisitor;

                    // Recreated the conveter
                    _fileReader = new FileSvgReader(_wpfSettings);
                    _fileReader.SaveXaml = false;
                    _fileReader.SaveZaml = false;
                }
            }
        }

        #endregion

        #region Public Methods

        public bool InitializePage()
        {
            if (documentInitialized)
            {
                return documentInitialized;
            }
            if (toolBar != null)
            {
                toolBar.Visibility = toolbarVisibility ? Visibility.Visible : Visibility.Collapsed;
            }
            this.UnInitializePage();

            if (!string.IsNullOrWhiteSpace(_svgFilePath) && File.Exists(_svgFilePath))
            {
                File.Copy(_svgFilePath, _testFilePath);
            }
            if (!string.IsNullOrWhiteSpace(_xmlFilePath) && File.Exists(_xmlFilePath))
            {
                File.Copy(_xmlFilePath, _otherFilePath);
            }

            if (!string.IsNullOrWhiteSpace(_testFilePath) && File.Exists(_testFilePath))
            {
                documentInitialized = this.LoadDocument(_testFilePath);
            }

            return documentInitialized;
        }

        public void UnInitializePage()
        {
            if (!string.IsNullOrWhiteSpace(_testFilePath) && File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            if (!string.IsNullOrWhiteSpace(_backFilePath) && File.Exists(_backFilePath))
            {
                File.Delete(_backFilePath);
            }

            if (!string.IsNullOrWhiteSpace(_otherFilePath) && File.Exists(_otherFilePath))
            {
                File.Delete(_otherFilePath);
            }
            if (!string.IsNullOrWhiteSpace(_otherBackFilePath) && File.Exists(_otherBackFilePath))
            {
                File.Delete(_otherBackFilePath);
            }
            documentInitialized = false;
        }

        public bool LoadDocument(string documentFilePath)
        {
            try
            {
                this.UnloadDocument();

                if (textEditor == null || string.IsNullOrWhiteSpace(documentFilePath)
                    || File.Exists(documentFilePath) == false)
                {
                    return false;
                }
                string fileExt = IoPath.GetExtension(documentFilePath);
                if (string.IsNullOrWhiteSpace(fileExt))
                {
                    return false;
                }
                if (!string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream fileStream = File.OpenRead(documentFilePath))
                    {
                        using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            // Text Editor does not work with this stream, so we read the data to memory stream...
                            MemoryStream memoryStream = new MemoryStream();
                            // Use this method is used to read all bytes from a stream.
                            int totalCount = 0;
                            int bufferSize = 512;
                            byte[] buffer = new byte[bufferSize];
                            while (true)
                            {
                                int bytesRead = zipStream.Read(buffer, 0, bufferSize);
                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                memoryStream.Write(buffer, 0, bytesRead);
                                totalCount += bytesRead;
                            }

                            if (totalCount > 0)
                            {
                                memoryStream.Position = 0;
                            }

                            textEditor.Load(memoryStream);

                            memoryStream.Close();
                        }
                    }
                }
                else
                {
                    textEditor.Load(documentFilePath);
                }

                if (otherEditor != null && File.Exists(_otherFilePath))
                {
                    otherEditor.Load(_otherFilePath);
                }

                this.UpdateFoldings();
            }
            catch (Exception ex)
            {
                this.ReportError(ex);

                return false;
            }

            if (string.Equals(documentFilePath, _testFilePath, StringComparison.OrdinalIgnoreCase))
            {
                return this.ConvertDocument();
            }

            if (this.SaveDocument() && File.Exists(_testFilePath))
            {
                return this.ConvertDocument(documentFilePath);
            }

            return true;
        }

        public void UnloadDocument(bool clearText = false)
        {
            if (clearText)
            {
                if (textEditor != null)
                {
                    textEditor.Document.Text = string.Empty;
                }
            }
            if (svgDrawing != null)
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();
            }
        }

        #endregion

        #region Private Methods

        private void OnEmbeddedImageCreated(object sender, EmbeddedImageSerializerArgs args)
        {
            if (args == null)
            {
                return;
            }
            if (_embeddedImages == null)
            {
                _embeddedImages = new List<EmbeddedImageSerializerArgs>();
            }
            _embeddedImages.Add(args);
        }

        private void UpdateFoldings()
        {
            if (_foldingManager == null || _foldingStrategy == null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new XmlFoldingStrategy();
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
        }

        private bool SaveDocument()
        {
            try
            {
                string inputText = textEditor.Document.Text;
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    return false;
                }

                textEditor.Save(_backFilePath);

                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    XmlResolver = null
                };

                using (var textReader = new StreamReader(_backFilePath))
                {
                    using (var reader = XmlReader.Create(textReader, settings))
                    {
                        var document = new XmlDocument();
                        document.Load(reader);
                    }
                }

                File.Move(_backFilePath, _testFilePath);

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex);

                return false;
            }
        }

        private bool ConvertDocument(string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
            {
                filePath = _testFilePath;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
                {
                    return false;
                }

                //TODO
                _fileReader.DrawingSettings.OptimizePath = true;

                DrawingGroup drawing = _fileReader.Read(filePath, _directoryInfo);
                if (drawing == null)
                {
                    return false;
                }
                _currentDrawing = drawing;

                svgDrawing.UnloadDiagrams();

                svgDrawing.RenderDiagrams(drawing);

                zoomPanControl.InvalidateMeasure();

                Rect bounds = drawing.Bounds;

                //zoomPanControl.AnimatedScaleToFit();
                //Rect rect = new Rect(0, 0,
                //    mainFrame.RenderSize.Width, mainFrame.RenderSize.Height);
                //Rect rect = new Rect(0, 0,
                //    bounds.Width, bounds.Height);
                if (bounds.IsEmpty)
                {
                    bounds = new Rect(0, 0, viewerFrame.ActualWidth, viewerFrame.ActualHeight);
                }
                zoomPanControl.AnimatedZoomTo(bounds);

                return true;
            }
            catch (Exception ex)
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();

                this.ReportError(ex);

                return false;
            }
        }

        private void ReportInfo(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceInformation(message);

            MessageBox.Show(message, AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReportError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceError(message);

            MessageBox.Show(message, AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ReportError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            Trace.TraceError(ex.ToString());

            MessageBox.Show(ex.ToString(), AppErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region Private Event Handlers

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Title = "Select An SVG File";
            dlg.DefaultExt = "*.svg";
            dlg.Filter = "All SVG Files (*.svg,*.svgz)|*.svg;*.svgz"
                                + "|Svg Uncompressed Files (*.svg)|*.svg"
                                + "|SVG Compressed Files (*.svgz)|*.svgz";
            if (dlg.ShowDialog() ?? false)
            {
                _currentFileName = dlg.FileName;

                if (!string.IsNullOrWhiteSpace(_testFilePath))
                {
                    if (File.Exists(_testFilePath))
                    {
                        File.Delete(_testFilePath);
                    }
                }
                if (!string.IsNullOrWhiteSpace(_backFilePath))
                {
                    if (File.Exists(_backFilePath))
                    {
                        File.Delete(_backFilePath);
                    }
                }

                this.LoadDocument(_currentFileName);
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            if (_currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "Save As";
                dlg.Filter = "SVG Files|*.svg;*.svgz";
                dlg.DefaultExt = ".svg";
                if (dlg.ShowDialog() ?? false)
                {
                    _currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }

            string fileExt = Path.GetExtension(_currentFileName);
            if (string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                textEditor.Save(_currentFileName);
            }
            else if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream svgzDestFile = File.Create(_currentFileName))
                {
                    using (GZipStream zipStream = new GZipStream(svgzDestFile,
                        CompressionMode.Compress, true))
                    {
                        textEditor.Save(zipStream);
                    }
                }
            }
        }

        //private void OnPasteText(object sender, DataObjectPastingEventArgs args)
        //{
        //    string clipboard = args.DataObject.GetData(typeof(string)) as string;
        //}

        private void OnFormatInputClick(object sender, RoutedEventArgs e)
        {
            if (textEditor == null)
            {
                return;
            }
            string inputText = textEditor.Document.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.UTF8);
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(inputText);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                textEditor.Document.Text = sReader.ReadToEnd();
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();
        }

        private void OnSearchTextClick(object sender, RoutedEventArgs e)
        {
            if (_searchPanel == null)
            {
                return;
            }

            string searchText = searchTextBox.Text;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                _searchPanel.SearchPattern = searchText;
            }

            _searchPanel.Open();
            _searchPanel.Reactivate();
        }

        private void OnSearchTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            // your event handler here
            e.Handled = true;

            this.OnSearchTextClick(sender, e);
        }

        private void OnHighlightingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            textEditor.Focus();
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }

        private void OnConvertInputClick(object sender, RoutedEventArgs e)
        {
            this.UnloadDocument();

            try
            {
                if (!string.IsNullOrWhiteSpace(_testFilePath))
                {
                    if (File.Exists(_testFilePath))
                    {
                        File.Delete(_testFilePath);
                    }
                }
                if (!string.IsNullOrWhiteSpace(_backFilePath))
                {
                    if (File.Exists(_backFilePath))
                    {
                        File.Delete(_backFilePath);
                    }
                }

                if (this.SaveDocument() && File.Exists(_testFilePath))
                {
                    this.ConvertDocument();
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex);
            }
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

        private void OnDragDrop(object sender, DragEventArgs de)
        {
            string fileName = string.Empty;
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

            if (!string.IsNullOrEmpty(fileName))
            {
            }
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }
            string fileExt = IoPath.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return;
            }
            if (!string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;

                if (!string.IsNullOrWhiteSpace(_testFilePath))
                {
                    if (File.Exists(_testFilePath))
                    {
                        File.Delete(_testFilePath);
                    }
                }
                if (!string.IsNullOrWhiteSpace(_backFilePath))
                {
                    if (File.Exists(_backFilePath))
                    {
                        File.Delete(_backFilePath);
                    }
                }

                this.LoadDocument(fileName);
            }
            catch (Exception ex)
            {
                this.ReportError(ex);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;
            }
        }

        #endregion

        #region Private Zoom Panel Handlers

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            svgDrawing.Focus();
            Keyboard.Focus(svgDrawing);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomPanControl);
            origContentMouseDownPoint = e.GetPosition(svgDrawing);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn();
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut();
                    }
                }

                zoomPanControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(svgDrawing);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomPanControl.ContentOffsetX -= dragOffset.X;
                zoomPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void OnZoomPanMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        private void ZoomOut()
        {
            zoomPanControl.ContentScale -= 0.1;
        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        private void ZoomIn()
        {
            zoomPanControl.ContentScale += 0.1;
        }

        #endregion
    }
}
