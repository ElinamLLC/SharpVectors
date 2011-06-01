using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Security.AccessControl;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for FileListConverterOutput.xaml
    /// </summary>
    public partial class FileListConverterOutput : Page, IObservable
    {
        #region Private Fields

        private int _convertedCount;
        private bool _continueOnError;

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private List<string> _errorFiles;

        /// <summary>
        /// Only one observer is expected!
        /// </summary>
        private IObserver _observer;
        private ConverterOptions _options;

        private IList<string> _sourceFiles;
        private string _outputDir;
        private DirectoryInfo _outputInfoDir;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private BackgroundWorker _worker;

        #endregion

        #region Constructors and Destructor

        public FileListConverterOutput()
        {
            InitializeComponent();

            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.CultureInfo = _wpfSettings.NeutralCultureInfo;

            _fileReader = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;

            _worker.DoWork += new DoWorkEventHandler(OnWorkerDoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(OnWorkerProgressChanged);

            _continueOnError = true;
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
            }
        }

        public IList<string> SourceFiles
        {
            get
            {
                return _sourceFiles;
            }
            set
            {
                _sourceFiles = value;
            }
        }

        public string OutputDir
        {
            get
            {
                return _outputDir;
            }
            set
            {
                _outputDir = value;
            }
        }

        public bool ContinueOnError
        {
            get
            {
                return _continueOnError;
            }
            set
            {
                _continueOnError = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a writer error occurred when
        /// using the custom XAML writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if an error occurred when using
        /// the custom XAML writer; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool WriterErrorOccurred
        {
            get
            {
                return _writerErrorOccurred;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to fall back and use
        /// the .NET Framework XAML writer when an error occurred in using the
        /// custom writer.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the converter falls back to using
        /// the system XAML writer when an error occurred in using the custom
        /// writer; otherwise, it is <see langword="false"/>. If <see langword="false"/>,
        /// an exception, which occurred in using the custom writer will be
        /// thrown. The default is <see langword="false"/>. 
        /// </value>
        public bool FallbackOnWriterError
        {
            get
            {
                return _fallbackOnWriterError;
            }
            set
            {
                _fallbackOnWriterError = value;
            }
        }

        #endregion

        #region Public Methods

        public void Convert()
        {
            txtOutput.Clear();

            btnCancel.IsEnabled = false;

            _errorFiles = new List<string>();

            try
            {
                this.AppendLine("Converting files, please wait...");

                Debug.Assert(_sourceFiles != null && _sourceFiles.Count != 0);
                if (!String.IsNullOrEmpty(_outputDir))
                {
                    _outputInfoDir = new DirectoryInfo(_outputDir);
                    if (!_outputInfoDir.Exists)
                    {
                        _outputInfoDir.Create();
                    }
                }

                this.AppendLine("Input Files:");
                for (int i = 0; i < _sourceFiles.Count; i++)
                {
                    this.AppendLine(_sourceFiles[i]);
                }
                this.AppendLine(String.Empty);

                _worker.RunWorkerAsync();

                if (_observer != null)
                {
                    _observer.OnStarted(this);
                }

                btnCancel.IsEnabled = true;
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                builder.AppendLine();
                builder.AppendLine(ex.Message);

                this.AppendText(builder.ToString());
            }
        }

        #endregion

        #region Private Event Handlers

        #region Page Methods

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Cursor startCursor = this.Cursor;

            try
            {
                this.Cursor = Cursors.Wait;

                this.Cancel();
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                builder.AppendLine();
                builder.AppendLine(ex.Message);

                this.AppendText(builder.ToString());
            }
            finally
            {
                this.Cursor = startCursor;
            }
        }

        #endregion

        #region BackgroundWorker Methods

        private void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                this.AppendLine(e.UserState.ToString());
            }
        }

        private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCancel.IsEnabled = false;

            StringBuilder builder = new StringBuilder();
            if (e.Error != null)
            {
                Exception ex = e.Error;

                if (ex != null)
                {
                    builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                    builder.AppendLine();
                    builder.AppendLine(ex.Message);
                    builder.AppendLine(ex.ToString());
                }
                else
                {
                    builder.AppendFormat("Error: Unknown");
                }

                if (_observer != null)
                {
                    _observer.OnCompleted(this, false);
                }
            }
            else if (e.Cancelled)
            {
                builder.AppendLine("Result: Cancelled");

                if (_observer != null)
                {
                    _observer.OnCompleted(this, false);
                }
            }
            else if (e.Result != null)
            {
                string resultText = e.Result.ToString();
                bool isSuccessful = !String.IsNullOrEmpty(resultText) &&
                    String.Equals(resultText, "Successful", StringComparison.OrdinalIgnoreCase);

                if (_errorFiles == null || _errorFiles.Count == 0)
                {
                    builder.AppendLine("Total number of files converted: " + _convertedCount);
                }
                else
                {
                    builder.AppendLine("Total number of files successful converted: " + _convertedCount);
                    builder.AppendLine("Total number of files failed: " + _errorFiles.Count);
                }
                if (!String.IsNullOrEmpty(resultText))
                {
                    builder.AppendLine("Result: " + resultText);
                }

                if (!String.IsNullOrEmpty(_outputDir))
                {
                    builder.AppendLine("Output Directory: " + _outputDir);
                }

                if (_observer != null)
                {
                    _observer.OnCompleted(this, isSuccessful);
                }
            }

            this.AppendLine(builder.ToString());
        }

        private void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            _wpfSettings.IncludeRuntime = _options.IncludeRuntime;
            _wpfSettings.TextAsGeometry = _options.TextAsGeometry;

            _fileReader.UseFrameXamlWriter = !_options.UseCustomXamlWriter;

            if (_options.GeneralWpf)
            {
                _fileReader.SaveXaml = _options.SaveXaml;
                _fileReader.SaveZaml = _options.SaveZaml;
            }
            else
            {
                _fileReader.SaveXaml = false;
                _fileReader.SaveZaml = false;
            }

            this.ConvertFiles(e, _outputInfoDir);

            if (!e.Cancel)
            {
                e.Result = "Successful";
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void AppendText(string text)
        {
            if (text == null)
            {
                return;
            }

            txtOutput.AppendText(text);
        }

        private void AppendLine(string text)
        {
            if (text == null)
            {
                return;
            }

            txtOutput.AppendText(text + Environment.NewLine);
        }

        private void ConvertFiles(DoWorkEventArgs e, DirectoryInfo target)
        {
            _fileReader.FallbackOnWriterError = _fallbackOnWriterError;

            if (e.Cancel)
            {
                return;
            }

            if (_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            DirectoryInfo outputDir = target;

            foreach (string svgFileName in _sourceFiles)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                string fileExt = Path.GetExtension(svgFileName);
                if (String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (_worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        if (target == null)
                        {
                            outputDir = new DirectoryInfo(
                                Path.GetDirectoryName(svgFileName));
                        }

                        DrawingGroup drawing = _fileReader.Read(svgFileName,
                            outputDir);

                        if (drawing == null)
                        {
                            if (_continueOnError)
                            {
                                throw new InvalidOperationException(
                                    "The conversion failed due to unknown error.");
                            }
                        }

                        if (drawing != null && _options.GenerateImage)
                        {
                            _fileReader.SaveImage(svgFileName, target,
                                _options.EncoderType);
                        }

                        if (drawing != null)
                        {
                            _convertedCount++;
                        }

                        if (_fileReader.WriterErrorOccurred)
                        {
                            _writerErrorOccurred = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorFiles.Add(svgFileName);

                        if (_continueOnError)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("Error converting: " + svgFileName);
                            builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                            builder.AppendLine();
                            builder.AppendLine(ex.Message);
                            builder.AppendLine(ex.ToString());

                            _worker.ReportProgress(0, builder.ToString());
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        #endregion

        #region IObservable Members

        public void Cancel()
        {
            btnCancel.IsEnabled = false;

            if (_worker != null)
            {
                if (_worker.IsBusy)
                {
                    _worker.CancelAsync();

                    // Wait for the BackgroundWorker to finish the download.
                    while (_worker.IsBusy)
                    {
                        // Keep UI messages moving, so the form remains 
                        // responsive during the asynchronous operation.
                        MainApplication.DoEvents();   
                    }
                }
            }
        }

        public void Subscribe(IObserver observer)
        {
            _observer = observer;
        }

        #endregion
    }
}
