using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Win32;

using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for FileListConverterPage.xaml
    /// </summary>
    public partial class FileListConverterPage : Page, IObservable, IObserver
    {
        #region Private Fields

        private delegate void ConvertHandler();

        private bool _isConverting;
        private bool _isConversionError;
        /// <summary>
        /// Only one observer is expected!
        /// </summary>
        private Brush _titleBkDefault;
        private IObserver _observer;
        private ConverterOptions _options;

        private FileList _listItems;

        private FileListConverterOutput _converterOutput;

        private ConverterCommandLines _commandLines;

        private Frame _parentFrame;

        #endregion

        #region Constructors and Destructor

        public FileListConverterPage()
        {
            InitializeComponent();

            // Reset the dimensions...
            this.Width = Double.NaN;
            this.Height = Double.NaN;

            this.Loaded += new RoutedEventHandler(OnPageLoaded);

            if (_titleBkDefault == null &&
                (statusTitle != null && statusTitle.IsInitialized))
            {
                _titleBkDefault = statusTitle.Background;
            }

            _listItems = new FileList();

            lstSourceFile.ItemsSource = _listItems;

            _listItems.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSourceUpdated);
        }

        #endregion

        #region Public Properties

        public ConverterOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;

                if (_options != null)
                {
                    _options.PropertyChanged += new PropertyChangedEventHandler(OnOptionsPropertyChanged);
                }
            }
        }

        public Frame ParentFrame
        {
            get
            {
                return _parentFrame;
            }
            set
            {
                _parentFrame = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_titleBkDefault == null)
            {
                _titleBkDefault = statusTitle.Background;
            }
        }

        #endregion

        #region Private Event Handlers

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            MainApplication theApp = (MainApplication)Application.Current;
            Debug.Assert(theApp != null);
            if (theApp != null && _commandLines == null)
            {
                _commandLines = theApp.CommandLines;
                if (_commandLines != null && !_commandLines.IsEmpty)
                {
                    IList<string> sourceFiles = _commandLines.SourceFiles;
                    if (sourceFiles != null && sourceFiles.Count != 0)
                    {
                        // this will remove the watermark...
                        lstSourceFile.Focus();

                        for (int i = 0; i < sourceFiles.Count; i++)
                        {
                            _listItems.Add(sourceFiles[i]);
                        }
                    }
                    txtOutputDir.Text = _commandLines.OutputDir;
                    chkContinueOnError.IsChecked = _commandLines.ContinueOnError;
                }
            }

            Debug.Assert(_options != null);

            if (!_isConversionError)
            {
                this.UpdateStatus();
            }
        }

        private void OnDirTextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateStatus();
        }

        private void OnSourceFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data is DataObject && ((DataObject)e.Data).ContainsFileDropList())
            {
                foreach (string filePath in ((DataObject)e.Data).GetFileDropList())
                {
                    _listItems.Add(filePath);
                }
            }
        }

        private void OnSourceFilePreviewDragEnter(object sender, DragEventArgs e)
        {
            bool dropPossible = e.Data != null && ((DataObject)e.Data).ContainsFileDropList();
            if (dropPossible)
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void OnSourceFilePreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            // this will remove the watermark...
            lstSourceFile.Focus();
        }

        private void OnSourceAddClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter      = "SVG Files|*.svg;*.svgz"; ;
            dlg.FilterIndex = 1;

            bool? isSelected = dlg.ShowDialog();

            if (isSelected != null && isSelected.Value)
            {
                // this will remove the watermark...
                lstSourceFile.Focus();

                foreach (string filePath in dlg.FileNames)
                {
                    _listItems.Add(filePath);
                }
            }
        }

        private void OnSourceRemoveClick(object sender, RoutedEventArgs e)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            int selIndex = lstSourceFile.SelectedIndex;
            if (selIndex < 0)
            {
                return;
            }

            _listItems.RemoveAt(selIndex);
        }

        private void OnSourceClearClick(object sender, RoutedEventArgs e)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Clear();
        }

        private void OnSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listItems != null && _listItems.Count > 0)
            {
                btnClearSourceFile.IsEnabled = true;
                btnRemoveSourceFile.IsEnabled = lstSourceFile.SelectedIndex >= 0;
            }
            else
            {
                btnClearSourceFile.IsEnabled = false;
                btnRemoveSourceFile.IsEnabled = false;
            }
        }

        private void OnSourceUpdated(object sender, EventArgs e)
        {
            if (_listItems != null && _listItems.Count > 0)
            {
                btnClearSourceFile.IsEnabled = true;
                btnRemoveSourceFile.IsEnabled = lstSourceFile.SelectedIndex >= 0;
            }
            else
            {
                btnClearSourceFile.IsEnabled = false;
                btnRemoveSourceFile.IsEnabled = false;
            }

            txtFileCount.Text = _listItems.Count.ToString();

            this.UpdateStatus();
        }

        private void OnOutputDirClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "Select the output directory for the converted files.";
            string sourceFile = _listItems.LastDirectory;
            if (!String.IsNullOrEmpty(sourceFile) &&
                File.Exists(sourceFile))
            {
                dlg.SelectedPath = Path.GetDirectoryName(sourceFile);
            }
            else
            {
                dlg.SelectedPath = Environment.CurrentDirectory;
            }

            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // this will remove the watermark...
                txtOutputDir.Focus();
                txtOutputDir.Text = dlg.SelectedPath;
            }
        }

        private void OnConvertClick(object sender, RoutedEventArgs e)
        {
            Debug.Assert(_parentFrame != null);
            if (_parentFrame == null)
            {
                return;
            }

            _isConverting        = true;
            _isConversionError   = false;
            btnConvert.IsEnabled = false;

            if (_converterOutput == null)
            {
                _converterOutput = new FileListConverterOutput();
            }
            _converterOutput.Options = _options;
            _converterOutput.Subscribe(this);

            if (chkContinueOnError.IsChecked != null)
            {
                _converterOutput.ContinueOnError = chkContinueOnError.IsChecked.Value;
            }
            _converterOutput.SourceFiles = _listItems.FileItems;
            _converterOutput.OutputDir   = txtOutputDir.Text;

            _parentFrame.Content = _converterOutput;

            //_converterOutput.Convert();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new ConvertHandler(_converterOutput.Convert));
        }

        private void OnOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _isConversionError = false;
        }

        #endregion

        #region Private Methods

        private void UpdateStatus()
        {
            if (_isConverting)
            {
                this.UpdateStatus("Converting",
                    "The conversion process is currently running, please wait...", false);
                return;
            }

            bool isValid = false;

            if (_options.IsValid)
            {
                string outputDir = txtOutputDir.Text.Trim();
                bool isReadOnlyOutputDir = _listItems.HasReadOnlyMedia;
                if (!String.IsNullOrEmpty(outputDir))
                {
                    try
                    {
                        string rootDir = Path.GetPathRoot(outputDir);
                        if (!String.IsNullOrEmpty(rootDir))
                        {
                            DriveInfo drive = new DriveInfo(rootDir);
                            if (!drive.IsReady || drive.DriveType == DriveType.CDRom
                                || drive.DriveType == DriveType.Unknown)
                            {
                                isReadOnlyOutputDir = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                if (_listItems == null || _listItems.Count == 0)
                {
                    this.UpdateStatus("Conversion: Not Ready",
                        "Select the input SVG files for conversion.", false);
                }
                else
                {
                    if (isReadOnlyOutputDir)
                    {
                        this.UpdateStatus("Error: Output Directory",
                            "The output directory is either invalid or read-only. Please select a different output directory.", true);
                    }
                    else
                    {
                        bool isReadOnlySource = false;
                        try
                        {
                            string rootDir = Path.GetPathRoot(outputDir);
                            if (!String.IsNullOrEmpty(rootDir))
                            {
                                DriveInfo drive = new DriveInfo(rootDir);
                                if (!drive.IsReady || drive.DriveType == DriveType.CDRom
                                    || drive.DriveType == DriveType.Unknown)
                                {
                                    isReadOnlySource = true;
                                }
                            }
                        }
                        catch
                        {
                        }
                        if (isReadOnlySource && String.IsNullOrEmpty(outputDir))
                        {
                            this.UpdateStatus("Required: Output Directory",
                                "For the read-only source directory, an output directory is required and must be specified.", true);
                        }
                        else
                        {
                            this.UpdateStatus("Conversion: Ready",
                                "Click the Convert button to convert the input files.", false);

                            isValid = true;
                        }
                    }
                }
            }
            else
            {
                this.UpdateStatus("Error: Options", _options.Message, true);
            }

            btnConvert.IsEnabled = isValid;
        }

        private void UpdateStatus(string title, string text, bool isError)
        {
            if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(text))
            {
                return;
            }

            statusTitle.Background = isError ? Brushes.Red : _titleBkDefault;
            statusTitle.Foreground = isError ? Brushes.White : Brushes.Black;

            statusTitle.Text = title;
            statusText.Text = text;
        }

        #endregion

        #region IObservable Members

        public void Cancel()
        {
            if (_converterOutput != null)
            {
                _converterOutput.Cancel();
            }
        }

        public void Subscribe(IObserver observer)
        {
            _observer = observer;
        }

        #endregion

        #region IObserver Members

        public void OnStarted(IObservable sender)
        {
            _isConverting = true;

            progressBar.Visibility = Visibility.Visible;

            this.UpdateStatus();

            if (_observer != null)
            {
                _observer.OnStarted(this);
            }
        }

        public void OnCompleted(IObservable sender, bool isSuccessful)
        {
            _isConverting = false;

            progressBar.Visibility = Visibility.Hidden;

            this.UpdateStatus();

            if (_observer != null)
            {
                _observer.OnCompleted(this, isSuccessful);
            }

            _isConversionError = isSuccessful ? false : true;

            if (isSuccessful)
            {
                this.UpdateStatus("Conversion: Successful",
                    "The conversion of the specified file is completed successfully.", false);
            }
            else
            {
                this.UpdateStatus("Conversion: Failed",
                    "The conversion of the specified file failed, see the output for further information.", true);
            }
        }

        #endregion

        #region FileList Class

        public sealed class FileList : ObservableCollection<ListBoxItem>
        {
            #region Private Fields

            private List<string> _listItems;

            #endregion

            #region Constructors and Destructor

            public FileList()
                : base()
            {
                _listItems = new List<string>();
            }

            #endregion

            #region Public Properties

            public IList<string> FileItems
            {
                get
                {
                    return _listItems;
                }
            }

            public string LastDirectory
            {
                get
                {
                    if (_listItems.Count != 0)
                    {
                        return Path.GetDirectoryName(
                            _listItems[_listItems.Count - 1]);
                    }

                    return String.Empty;
                }
            }

            public bool HasReadOnlyMedia
            {
                get
                {
                    bool isReadOnlyOutputDir = false;
                    try
                    {
                        for (int i = 0; i < _listItems.Count; i++)
                        {
                            string rootDir = Path.GetPathRoot(_listItems[i]);
                            if (!String.IsNullOrEmpty(rootDir))
                            {
                                DriveInfo drive = new DriveInfo(rootDir);
                                if (!drive.IsReady || drive.DriveType == DriveType.CDRom
                                    || drive.DriveType == DriveType.Unknown)
                                {
                                    isReadOnlyOutputDir = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    return isReadOnlyOutputDir;
                }
            }

            #endregion

            #region Public Methods

            public void Add(string filePath)
            {
                if (String.IsNullOrEmpty(filePath))
                {
                    return;
                }
                string fileExt = Path.GetExtension(filePath);
                if (String.IsNullOrEmpty(fileExt))
                {
                    return;
                }
                if (String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                    ||String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    ListBoxItem listItem = new ListBoxItem();
                    listItem.Content = filePath;
                    this.Add(listItem);

                    _listItems.Add(filePath);
                }
            }

            #endregion

            #region Protected Methods

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);

                _listItems.RemoveAt(index);
            }

            protected override void ClearItems()
            {
                base.ClearItems();

                if (_listItems != null)
                {
                    _listItems.Clear();
                }
            }

            #endregion
        }

        #endregion
    }
}
