using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfSvgTestBox
{
    internal sealed class KeyValue
    {
        private string _fileName;
        private string _resourceKey;

        public KeyValue()
        {
        }

        public KeyValue(string fileName, string resourceKey)
        {
            _fileName    = fileName;
            _resourceKey = resourceKey;
        }

        public string FileName
        {
            get {
                return _fileName;
            }
            set {
                _fileName = value;
            }
        }

        public string ResourceKey
        {
            get {
                return _resourceKey;
            }
            set {
                _resourceKey = value;
            }
        }
    }

    /// <summary>
    /// Interaction logic for DictonaryKeyDialog.xaml
    /// </summary>
    public partial class DictonaryKeyDialog : Window
    {
        private string _svgDir;

        private IDictionary<string, string> _keyDictionary;

        private ObservableCollection<KeyValue> _keyCollection;

        public DictonaryKeyDialog()
        {
            InitializeComponent();

            _keyCollection = new ObservableCollection<KeyValue>();
        }

        public string SvgDir
        {
            get {
                return _svgDir;
            }
            set {
                _svgDir = value;
            }
        }

        public IDictionary<string, string> KeyDictionary
        {
            get {
                return _keyDictionary;
            }
            set {
                _keyDictionary = value;
            }
        }

        /// <summary>
        /// Gets the dialog result of this dialog, based upon whether the user accepted the changes.
        /// </summary>
        //public new bool DialogResult { get; private set; } = false;

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            btnApplySnippet.IsEnabled = false;

            if (_keyDictionary != null && _keyDictionary.Count != 0)
            {
                txtSvgSource.Text = _svgDir;
                this.UpdateGrid();
            }
            else if (!string.IsNullOrWhiteSpace(_svgDir) && Directory.Exists(_svgDir))
            {
                this.UpdateGrid(_svgDir);
            }
            dataGrid.ItemsSource = _keyCollection;
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

        private void OnBrowseClicked(object sender, RoutedEventArgs e)
        {
            string svgDir = txtSvgSource.Text;
            if (string.IsNullOrWhiteSpace(svgDir) || Directory.Exists(svgDir) == false)
            {
                string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                    "Select the SVG files directory ", null);
                if (selectedDirectory != null)
                {
                    svgDir = selectedDirectory;
                }
                else
                {
                    return;
                }
            }

            this.UpdateGrid(svgDir);
        }

        private void UpdateGrid(string svgDir)
        {
            _svgDir = svgDir;
            txtSvgSource.Text = svgDir;
            if (string.IsNullOrWhiteSpace(svgDir) || Directory.Exists(svgDir) == false)
            {
                return;
            }

            var keyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            int resourceNumber = 1;
            foreach (var svgFilePath in Directory.EnumerateFiles(svgDir, "*.svg"))
            {
                // Eliminate any compressed file, not supported in this test...
                if (svgFilePath.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(svgFilePath);
                string resourceName = SvgTestResourceKeyResolver.ToUpperCamelCase(fileName.ToUpper());

                keyDictionary.Add(fileName, string.Format("Icon{0:D2}_{1}", resourceNumber, resourceName));
                resourceNumber++;
            }

            _keyDictionary = keyDictionary;
            this.UpdateGrid();
        }

        private void UpdateGrid()
        {
            btnApplySnippet.IsEnabled = false;
            _keyCollection.Clear();

            if (_keyDictionary == null || _keyDictionary.Count == 0)
            {
                return;
            }

            foreach (var keyValue in _keyDictionary)
            {
                _keyCollection.Add(new KeyValue(keyValue.Key, keyValue.Value));
            }

            btnApplySnippet.IsEnabled = true;
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            var keyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var keyValue in _keyCollection)
            {
                keyDictionary.Add(keyValue.FileName, keyValue.ResourceKey);
            }
            _keyDictionary = keyDictionary;

            this.DialogResult = true;
            this.Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
