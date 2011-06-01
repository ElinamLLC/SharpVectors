using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

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
    /// Interaction logic for DirectoryConverterPage.xaml
    /// </summary>
    public partial class DirectoryConverterPage : Page, IObservable, IObserver
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

        private DirectoryConverterOutput _converterOutput;

        private ConverterCommandLines _commandLines;

        private Frame _parentFrame;

        #endregion

        #region Constructors and Destructor

        public DirectoryConverterPage()
        {
            InitializeComponent();

            // Reset the dimensions...
            this.Width  = Double.NaN;
            this.Height = Double.NaN;

            if (_titleBkDefault == null && 
                (statusTitle != null && statusTitle.IsInitialized))
            {
                _titleBkDefault = statusTitle.Background;
            }

            this.Loaded += new RoutedEventHandler(OnPageLoaded);
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
                    // this will remove the watermark...
                    txtSourceDir.Focus();

                    string sourceDir = _commandLines.SourceDir;
                    if (!String.IsNullOrEmpty(sourceDir) && Directory.Exists(sourceDir))
                    {
                        txtSourceDir.Text = sourceDir;
                    }
                    txtOutputDir.Text = _commandLines.OutputDir;
                    chkRecursive.IsChecked = _commandLines.Recursive;
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

        private void OnSourceDirClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "Select the source directory of the SVG files.";
            string sourceDir = txtSourceDir.Text.Trim();
            if (!String.IsNullOrEmpty(sourceDir) &&
                Directory.Exists(sourceDir))
            {
                dlg.SelectedPath = sourceDir;
            }
            else
            {
                dlg.SelectedPath = Environment.CurrentDirectory;
            }

            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // this will remove the watermark...
                txtSourceDir.Focus();
                txtSourceDir.Text = dlg.SelectedPath;
            }
        }

        private void OnOutputDirClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description         = "Select the output directory for the converted file.";
            string sourceDir = txtSourceDir.Text.Trim();
            if (!String.IsNullOrEmpty(sourceDir) &&
                Directory.Exists(sourceDir))
            {
                dlg.SelectedPath = sourceDir;
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
                _converterOutput = new DirectoryConverterOutput();
            }
            _converterOutput.Options = _options;
            if (chkRecursive.IsChecked != null)
            {
                _converterOutput.Recursive = chkRecursive.IsChecked.Value;
            }
            if (chkContinueOnError.IsChecked != null)
            {
                _converterOutput.ContinueOnError = chkContinueOnError.IsChecked.Value;
            }
            _converterOutput.Subscribe(this);

            _converterOutput.SourceDir = txtSourceDir.Text;
            _converterOutput.OutputDir = txtOutputDir.Text;

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
                string sourceDir = txtSourceDir.Text.Trim();
                string outputDir = txtOutputDir.Text.Trim();
                bool isReadOnlyOutputDir = false;
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
                if (String.IsNullOrEmpty(sourceDir))
                {
                    this.UpdateStatus("Conversion: Not Ready",
                        "Select an input directory of SVG files for conversion.", false);
                }
                else if (Directory.Exists(sourceDir))
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
                                "Click the Convert button to convert the SVG files in the source directory.", false);

                            isValid = true;
                        }
                    }
                }
                else
                {
                    this.UpdateStatus("Error: Source Directory",
                        "The specified source directory is either invalid or does not exists.",
                        true);
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
                    "The conversion of the specified directory is completed successfully.", false);
            }
            else
            {
                this.UpdateStatus("Conversion: Failed",
                    "The conversion of the specified directory failed, see the output for further information.", true);
            }
        }

        #endregion
    }
}
