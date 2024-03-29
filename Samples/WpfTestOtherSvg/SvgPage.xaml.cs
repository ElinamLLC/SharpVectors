﻿using System;
using System.IO;
using System.IO.Packaging;
using System.IO.Compression;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

using Microsoft.Win32;

namespace WpfTestOtherSvg
{
    /// <summary>
    /// Interaction logic for SvgPage.xaml
    /// </summary>
    public partial class SvgPage : Page
    {
        #region Private Fields

        private string _currentFileName;

        private MainWindow _mainWindow;

        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        #endregion

        #region Constructors and Destructor

        public SvgPage()
        {
            InitializeComponent();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            TextEditorOptions options = textEditor.Options;
            if (options != null)
            {
                //options.AllowScrollBelowDocument = true;
                options.EnableHyperlinks           = true;
                options.EnableEmailHyperlinks      = true;
                options.EnableVirtualSpace         = false;
                options.HighlightCurrentLine       = true;
                //options.ShowSpaces               = true;
                //options.ShowTabs                 = true;
                //options.ShowEndOfLine            = true;              
            }

            textEditor.ShowLineNumbers = true;

            _foldingManager  = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            textEditor.CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Print, OnPrint, OnCanExecuteTextEditorCommand));
            textEditor.CommandBindings.Add(new CommandBinding(
                ApplicationCommands.PrintPreview, OnPrintPreview, OnCanExecuteTextEditorCommand));

            _searchPanel = SearchPanel.Install(textEditor);
        }

        #endregion

        #region Public Properties

        public MainWindow MainWindow
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        #endregion

        #region Public Methods

        public bool LoadDocument(string documentFileName)
        {   
            if (textEditor == null || string.IsNullOrWhiteSpace(documentFileName))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_currentFileName) && File.Exists(_currentFileName))
            {
                // Prevent reloading the same file, just in case we are editing...
                if (string.Equals(documentFileName, _currentFileName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            string fileExt = Path.GetExtension(documentFileName);
            if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream fileStream = File.OpenRead(documentFileName))
                {
                    using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        // Text Editor does not work with this stream, so we read the data to memory stream...
                        MemoryStream memoryStream = new MemoryStream();
                        // Use this method is used to read all bytes from a stream.
                        int totalCount = 0;
                        int bufferSize = 512;
                        byte[] buffer  = new byte[bufferSize];
                        while (true)
                        {
                            int bytesRead = zipStream.Read(buffer, 0, bufferSize);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            else
                            {
                                memoryStream.Write(buffer, 0, bytesRead);
                            }
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
                textEditor.Load(documentFileName);
            }

            if (_foldingManager == null || _foldingStrategy == null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new XmlFoldingStrategy();
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);

            _currentFileName = documentFileName;

            txtFileName.Text = _currentFileName;

            return true;
        }

        public void UnloadDocument()
        {
            if (textEditor != null)
            {
                textEditor.Document.Text = string.Empty;
            }
            _currentFileName = null;

            txtFileName.Text = string.Empty;
        }

        public void PageSelected(bool isSelected)
        {
            if (isSelected)
            {
                if (textEditor.TextArea.IsKeyboardFocusWithin)
                {   
                    Keyboard.Focus(textEditor.TextArea);
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                _currentFileName = dlg.FileName;
                textEditor.Load(_currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(
                    Path.GetExtension(_currentFileName));
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            if (_currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title      = "Save As";
                dlg.Filter     = "SVG Files|*.svg;*.svgz";
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

        private void OnLocateFile(object sender, RoutedEventArgs e)
        {
            var filePath = txtFileName.Text;
            if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
            {
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", @"/select, " + filePath);
        }

        #region Printing Methods

        private Block ConvertTextDocumentToBlock()
        {
            TextDocument document = textEditor.Document;
            IHighlighter highlighter =
                textEditor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;

            return DocumentPrinter.ConvertTextDocumentToBlock(document, highlighter);
        }

        private FlowDocument CreateFlowDocumentForEditor()
        {
            FlowDocument doc = new FlowDocument(ConvertTextDocumentToBlock());
            doc.FontFamily = textEditor.FontFamily;
            doc.FontSize   = textEditor.FontSize;
            return doc;
        }

        // CanExecuteRoutedEventHandler that only returns true if
        // the source is a control.
        private void OnCanExecuteTextEditorCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var target = e.Source as TextEditor;

            if (target != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OnPrint(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PageRangeSelection   = PageRangeSelection.AllPages;
            printDialog.UserPageRangeEnabled = true;
            bool? dialogResult = printDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value == false)
            {
                return;
            }

            FlowDocument printSource = this.CreateFlowDocumentForEditor();

            // Save all the existing settings.                                
            double pageHeight     = printSource.PageHeight;
            double pageWidth      = printSource.PageWidth;
            Thickness pagePadding = printSource.PagePadding;
            double columnGap      = printSource.ColumnGap;
            double columnWidth    = printSource.ColumnWidth;

            // Make the FlowDocument page match the printed page.
            printSource.PageHeight  = printDialog.PrintableAreaHeight;
            printSource.PageWidth   = printDialog.PrintableAreaWidth;
            printSource.PagePadding = new Thickness(20);
            printSource.ColumnGap   = Double.NaN;
            printSource.ColumnWidth = printDialog.PrintableAreaWidth;

            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            DocumentPaginator paginator = ((IDocumentPaginatorSource)printSource).DocumentPaginator;
            paginator.PageSize = pageSize;
            paginator.ComputePageCount();

            printDialog.PrintDocument(paginator, "Svg Contents");

            // Reapply the old settings.
            printSource.PageHeight  = pageHeight;
            printSource.PageWidth   = pageWidth;
            printSource.PagePadding = pagePadding;
            printSource.ColumnGap   = columnGap;
            printSource.ColumnWidth = columnWidth;
        }

        private void OnPrintPreview(object sender, ExecutedRoutedEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}
