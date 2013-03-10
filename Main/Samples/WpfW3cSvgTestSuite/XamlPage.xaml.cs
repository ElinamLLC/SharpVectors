using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;  

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Search;

using Microsoft.Win32;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for XamlPage.xaml
    /// </summary>
    public partial class XamlPage : Page
    {
        private string currentFileName;

        private FoldingManager     _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private SearchInputHandler _searchHandler;

        public XamlPage()
        {
            InitializeComponent();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            TextEditorOptions options = textEditor.Options;
            if (options != null)
            {
                //options.AllowScrollBelowDocument = true;
                options.EnableHyperlinks = true;
                options.EnableEmailHyperlinks = true;
                //options.ShowSpaces = true;
                //options.ShowTabs = true;
                //options.ShowEndOfLine = true;              
            }
            textEditor.ShowLineNumbers = true;

            _foldingManager  = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            _searchHandler = new SearchInputHandler(textEditor.TextArea);
            textEditor.TextArea.DefaultInputHandler.NestedInputHandlers.Add(_searchHandler);
        }

        public void LoadDocument(string documentFileName)
        {
            if (textEditor == null || String.IsNullOrEmpty(documentFileName))
            {
                return;
            }

            this.UnloadDocument();

            string fileExt = Path.GetExtension(documentFileName);
            if (String.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream fileStream = File.OpenRead(documentFileName))
                {
                    using (GZipStream zipStream =
                        new GZipStream(fileStream, CompressionMode.Decompress))
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
                textEditor.Document.Text = String.Empty;
            }
        }

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        private void OnSearchTextClick(object sender, RoutedEventArgs e)
        {
            string searchText = searchTextBox.Text;

            if (String.IsNullOrEmpty(searchText))
            {
                return;
            }

            _searchHandler.SearchPattern = searchText;
            _searchHandler.FindNext();
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
            //if (textEditor.SyntaxHighlighting == null)
            //{
            //    _foldingStrategy = null;
            //}
            //else
            //{
            //    switch (textEditor.SyntaxHighlighting.Name)
            //    {
            //        case "XML":
            //            _foldingStrategy = new XmlFoldingStrategy();
            //            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
            //            break;
            //        case "C#":
            //        case "C++":
            //        case "PHP":
            //        case "Java":
            //            textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(textEditor.Options);
            //            _foldingStrategy = new BraceFoldingStrategy();
            //            break;
            //        default:
            //            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
            //            _foldingStrategy = null;
            //            break;
            //    }
            //}
            //if (_foldingStrategy != null)
            //{
            //    if (_foldingManager == null)
            //        _foldingManager = FoldingManager.Install(textEditor.TextArea);
            //    _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            //}
            //else
            //{
            //    if (_foldingManager != null)
            //    {
            //        FoldingManager.Uninstall(_foldingManager);
            //        _foldingManager = null;
            //    }
            //}
        }
    }
}
