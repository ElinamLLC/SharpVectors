using System;
using System.IO;
using System.IO.Packaging;
using System.IO.Compression;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

using Microsoft.Win32;

namespace WpfTestSvgControl
{
    /// <summary>
    /// Interaction logic for XamlPage.xaml
    /// </summary>
    public partial class XamlPage : Page
    {
        #region Private Fields

        private string _currentFileName;

        private MainWindow _mainWindow;

        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        #endregion

        #region Constructors and Destructor

        public XamlPage()
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

        public void LoadDocument(string documentFileName)
        {
            if (textEditor == null || string.IsNullOrWhiteSpace(documentFileName))
            {
                return;
            }

            string fileExt = Path.GetExtension(documentFileName);
            if (string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
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
                        byte[] buffer = new byte[bufferSize];
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
                _foldingManager  = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new XmlFoldingStrategy();
            }

            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
        }

        public void UnloadDocument()
        {
            if (textEditor != null)
            {
                textEditor.Document.Text = string.Empty;
            }
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
                dlg.Title      = "Save As (.zaml is GZip compressed file used by SharpVectors)";
                dlg.Filter     = "SVG Files|*.xaml;*.zaml";
                dlg.DefaultExt = ".xaml";
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
            if (string.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase))
            {
                textEditor.Save(_currentFileName);
            }
            else if (string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream zamlDestFile = File.Create(_currentFileName))
                {
                    using (GZipStream zipStream = new GZipStream(zamlDestFile,
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

            textEditor.Select(textEditor.SelectionStart, 1);
            searchTextBox.Focus();

            this.OnSearchTextClick(sender, e);
        } 

        private void OnHighlightingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        #endregion

        #region Printing Methods

        private Block ConvertTextDocumentToBlock()
        {
            TextDocument document = textEditor.Document;
            IHighlighter highlighter =
                textEditor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;

            return DocumentPrinter.ConvertTextDocumentToBlock(document, highlighter);

            //Paragraph p = new Paragraph();
            //foreach (DocumentLine line in document.Lines)
            //{
            //    int lineNumber = line.LineNumber;
            //    HighlightedInlineBuilder inlineBuilder = new HighlightedInlineBuilder(document.GetText(line));
            //    if (highlighter != null)
            //    {
            //        HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
            //        int lineStartOffset = line.Offset;
            //        foreach (HighlightedSection section in highlightedLine.Sections)
            //            inlineBuilder.SetHighlighting(section.Offset - lineStartOffset, section.Length, section.Color);
            //    }
            //    p.Inlines.AddRange(inlineBuilder.CreateRuns());
            //    p.Inlines.Add(new LineBreak());
            //}

            //return p;
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

            MemoryStream xpsStream  = new MemoryStream();
            Package package         = Package.Open(xpsStream, FileMode.Create, FileAccess.ReadWrite);
            string packageUriString = "memorystream://data.xps";
            PackageStore.AddPackage(new Uri(packageUriString), package);

            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Normal, packageUriString);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            DocumentPaginator paginator = ((IDocumentPaginatorSource)printSource).DocumentPaginator;
            paginator.PageSize = pageSize;
            paginator.ComputePageCount();

            writer.Write(paginator);

            // Reapply the old settings.
            printSource.PageHeight  = pageHeight;
            printSource.PageWidth   = pageWidth;
            printSource.PagePadding = pagePadding;
            printSource.ColumnGap   = columnGap;
            printSource.ColumnWidth = columnWidth;

            PrintPreviewWindow printPreview = new PrintPreviewWindow();
            printPreview.Width  = this.ActualWidth;
            printPreview.Height = this.ActualHeight;
            printPreview.Owner  = Application.Current.MainWindow;

            printPreview.LoadDocument(xpsDocument, package, packageUriString);

            printPreview.Show();
        }

        #endregion
    }
}
