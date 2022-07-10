using System;
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
using System.Windows.Threading;

using Microsoft.Win32;
using IoPath = System.IO.Path;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Indentation;

namespace WpfSvgTestBox
{
    public enum XmlViewerType
    {
        Xml,
        Svg,
        Xaml
    }

    /// <summary>
    /// Interaction logic for XmlViewerDialog.xaml
    /// </summary>
    public partial class XmlViewerDialog : Window
    {
        private XmlViewerType _xmlType;
        private string _xmlText;
        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        private readonly SearchPanel _searchPanel;

        public XmlViewerDialog()
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

            _xmlType = XmlViewerType.Xml;
            textEditor.ShowLineNumbers = true;

            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            _searchPanel = SearchPanel.Install(textEditor);

            textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
        }

        public XmlViewerType XmlType
        {
            get {
                return _xmlType;
            }
            set {
                _xmlType = value;
            }
        }

        public string XmlText
        {
            get {
                return _xmlText;
            }
            set {
                _xmlText = value;
            }
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

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            if (!string.IsNullOrWhiteSpace(_xmlText))
            {
                textEditor.Text = _xmlText;
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
            dlg.Title = "Select File";
            dlg.DefaultExt = "*.xml";
            dlg.Filter = "All Files (*.xml,*.svg,*.xaml)|*.xml;*.svg;*.xaml"
                                + "|XML Files (*.xml)|*.xml"
                                + "|SVG Files (*.svg)|*.svg"
                                + "|XAML Files (*.xaml)|*.xaml";
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Load(dlg.FileName);
            }
        }

        private void OnSaveFileClick(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save As";
            if (_xmlType == XmlViewerType.Svg)
            {
                dlg.Filter = "SVG Files|*.svg;*.svgz";
                dlg.DefaultExt = ".svg";
            }
            else if (_xmlType == XmlViewerType.Xaml)
            {
                dlg.Filter = "XAML Files|*.xaml;*.txt";
                dlg.DefaultExt = ".xaml";
            }
            else if (_xmlType == XmlViewerType.Xml)
            {
                dlg.Filter = "XML Files|*.xml;*.txt";
                dlg.DefaultExt = ".xml";
            }
            if (dlg.ShowDialog() ?? false)
            {
                textEditor.Save(dlg.FileName);
            }

        }

    }
}
