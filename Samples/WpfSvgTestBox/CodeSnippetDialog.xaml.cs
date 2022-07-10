using System;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Win32;
using IoPath = System.IO.Path;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Indentation;

using SharpVectors.Renderers;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for CodeSnippetDialog.xaml
    /// </summary>
    public partial class CodeSnippetDialog : Window
    {
        private const string AppTitle = "Svg Test Box";
        private const string AppErrorTitle = "Svg Test Box - Error";

        private bool _lastCompiled;
        private string _codeSnippet;

        private FoldingManager _foldingManager;
        private BraceFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        public CodeSnippetDialog()
        {
            InitializeComponent();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
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

            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new BraceFoldingStrategy();

            _searchPanel = SearchPanel.Install(textEditor);

            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();

            textEditor.TextChanged += OnEditorTextChanged;
        }

        private void OnEditorTextChanged(object sender, EventArgs e)
        {
            btnApplySnippet.IsEnabled = this.CompileSnippet();
        }

        public string CodeSnippet
        {
            get {
                return _codeSnippet;
            }
            set {
                _codeSnippet = value;
            }
        }

        /// <summary>
        /// Gets the dialog result of this dialog, based upon whether the user accepted the changes.
        /// </summary>
        //public new bool DialogResult { get; private set; } = false;

        private void UpdateFoldings()
        {
            if (textEditor == null)
            {
                return;
            }
            if (_foldingManager == null || _foldingStrategy == null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy = new BraceFoldingStrategy();
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);

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

        private bool CompileSnippet()
        {
            txtDebug.Clear();

            bool isSuccessful = false;

            txtDebug.AppendText("Compiling: Started..." + Environment.NewLine);
            txtDebug.AppendText(Environment.NewLine);
            string codeSnippet = textEditor.Text;
            if (string.IsNullOrWhiteSpace(codeSnippet))
            {
                txtDebug.AppendText("Error: Enter the code snippet to be compiled" + Environment.NewLine);
            }
            else
            {
                var compileResult = CodeSnippetKeyResolver.CompileSnippet(codeSnippet);
                if (compileResult.Item1)
                {
                    _codeSnippet = codeSnippet;
                    isSuccessful = true;
                }
                txtDebug.AppendText(compileResult.Item2 + Environment.NewLine);
            }
            txtDebug.AppendText("Compiling: Completed" + Environment.NewLine);

            _lastCompiled = isSuccessful;
            btnApplySnippet.IsEnabled = isSuccessful;

            return isSuccessful;
        }

        private void OnCompileInputClick(object sender, RoutedEventArgs e)
        {
            this.CompileSnippet();
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 4, GridUnitType.Pixel);

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            if (!string.IsNullOrWhiteSpace(_codeSnippet))
            {
                textEditor.Text = _codeSnippet;
            }

            textEditor.Focus();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {

        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 4, GridUnitType.Pixel);
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

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Title = "Select Code Snippet File";
            dlg.DefaultExt = "*.txt";
            dlg.Filter = "All Code Snippet Files (*.txt,*.cs)|*.txt;*.cs"
                                + "|Plain Text Files (*.txt)|*.txt"
                                + "|C# Files (*.cs)|*.cs";
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Load(dlg.FileName);
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save As";
            dlg.Filter = "Code Snippet Files|*.txt;*.cs";
            dlg.DefaultExt = ".txt";
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Save(dlg.FileName);
            }

        }
        /// <summary>
        /// Allows producing foldings from a document based on braces.
        /// </summary>
        private sealed class BraceFoldingStrategy
        {
            /// <summary>
            /// Gets/Sets the opening brace. The default value is '{'.
            /// </summary>
            public char OpeningBrace { get; set; }

            /// <summary>
            /// Gets/Sets the closing brace. The default value is '}'.
            /// </summary>
            public char ClosingBrace { get; set; }

            /// <summary>
            /// Creates a new BraceFoldingStrategy.
            /// </summary>
            public BraceFoldingStrategy()
            {
                this.OpeningBrace = '{';
                this.ClosingBrace = '}';
            }

            public void UpdateFoldings(FoldingManager manager, TextDocument document)
            {
                int firstErrorOffset;
                IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
                manager.UpdateFoldings(newFoldings, firstErrorOffset);
            }

            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                List<NewFolding> newFoldings = new List<NewFolding>();

                Stack<int> startOffsets = new Stack<int>();
                int lastNewLineOffset = 0;
                char openingBrace = this.OpeningBrace;
                char closingBrace = this.ClosingBrace;
                for (int i = 0; i < document.TextLength; i++)
                {
                    char c = document.GetCharAt(i);
                    if (c == openingBrace)
                    {
                        startOffsets.Push(i);
                    }
                    else if (c == closingBrace && startOffsets.Count > 0)
                    {
                        int startOffset = startOffsets.Pop();
                        // don't fold if opening and closing brace are on the same line
                        if (startOffset < lastNewLineOffset)
                        {
                            newFoldings.Add(new NewFolding(startOffset, i + 1));
                        }
                    }
                    else if (c == '\n' || c == '\r')
                    {
                        lastNewLineOffset = i + 1;
                    }
                }
                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }
        }
    }
}
