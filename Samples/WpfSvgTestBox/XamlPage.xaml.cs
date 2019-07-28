using System;
using System.IO;
using System.IO.Compression;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

using Microsoft.Win32;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for XamlPage.xaml
    /// </summary>
    public partial class XamlPage : Page
    {
        #region Private Fields

        private string _currentFileName;

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
                options.EnableHyperlinks = true;
                options.EnableEmailHyperlinks = true;
                options.EnableVirtualSpace = false;
                options.HighlightCurrentLine = true;
                //options.ShowSpaces               = true;
                //options.ShowTabs                 = true;
                //options.ShowEndOfLine            = true;
            }
            textEditor.ShowLineNumbers = true;

            textEditor.IsReadOnly = true;

            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            _searchPanel = SearchPanel.Install(textEditor);

            this.Loaded += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Methods

        public bool LoadDocument(string documentFilePath)
        {
            this.UnloadDocument();

            if (textEditor == null || string.IsNullOrWhiteSpace(documentFilePath))
            {
                return false;
            }

            string fileExt = Path.GetExtension(documentFilePath);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return false;
            }
            if (!string.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
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

            if (_foldingManager == null || _foldingStrategy == null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new XmlFoldingStrategy();
            }

            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);

            return true;
        }

        public void UnloadDocument()
        {
            if (textEditor != null)
            {
                textEditor.Document.Text = string.Empty;
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
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(_currentFileName));
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            if (_currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title      = "Save As (.zaml is GZip compressed file used by SharpVectors)";
                dlg.Filter     = "XAML Files|*.xaml;*.zaml";
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

            this.OnSearchTextClick(sender, e);
        }

        private void OnHighlightingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        #endregion
    }
}
