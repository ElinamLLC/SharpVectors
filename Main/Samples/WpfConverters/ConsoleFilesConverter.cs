using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Security.AccessControl;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    public sealed class ConsoleFilesConverter : ConsoleConverter
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

        private IList<string> _sourceFiles;
        private DirectoryInfo _outputInfoDir;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private ConsoleWorker _worker;
        private ConsoleWriter _writer;

        #endregion

        #region Constructors and Destructor

        public ConsoleFilesConverter(IList<string> sourceFiles)
        {
            _sourceFiles = sourceFiles;

            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.CultureInfo = _wpfSettings.NeutralCultureInfo;

            _fileReader = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            _worker = new ConsoleWorker();
            //_worker.WorkerReportsProgress = true;
            //_worker.WorkerSupportsCancellation = true;

            _worker.DoWork += new DoWorkEventHandler(OnWorkerDoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(OnWorkerProgressChanged);

            _continueOnError = true;
        }

        #endregion

        #region Public Properties

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

        public override bool Convert(ConsoleWriter writer)
        {
            Debug.Assert(writer != null);

            Debug.Assert(_sourceFiles != null && _sourceFiles.Count != 0);
            if (_sourceFiles == null || _sourceFiles.Count == 0)
            {
                return false;
            }

            _writer = writer;

            _errorFiles = new List<string>();

            try
            {
                this.AppendLine(String.Empty);
                this.AppendLine("Converting files, please wait...");

                string outputDir = this.OutputDir;
                Debug.Assert(_sourceFiles != null && _sourceFiles.Count != 0);
                if (_sourceFiles == null || _sourceFiles.Count == 0)
                {
                    return false;
                }
                if (!String.IsNullOrEmpty(outputDir))
                {
                    _outputInfoDir = new DirectoryInfo(outputDir);
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

                return true;
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                builder.AppendLine();
                builder.AppendLine(ex.Message);

                this.AppendText(builder.ToString());

                return false;
            }
        }

        #endregion

        #region Private Event Handlers

        #region ConsoleWorker Methods

        private void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                this.AppendLine(e.UserState.ToString());
            }
        }

        private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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

                string outputDir = this.OutputDir;
                if (!String.IsNullOrEmpty(outputDir))
                {
                    builder.AppendLine("Output Directory: " + outputDir);
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
            ConsoleWorker worker = (ConsoleWorker)sender;

            ConverterOptions options   = this.Options;

            _wpfSettings.IncludeRuntime = options.IncludeRuntime;
            _wpfSettings.TextAsGeometry = options.TextAsGeometry;

            _fileReader.UseFrameXamlWriter = !options.UseCustomXamlWriter;

            if (options.GeneralWpf)
            {
                _fileReader.SaveXaml = options.SaveXaml;
                _fileReader.SaveZaml = options.SaveZaml;
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

            _writer.Write(text);
        }

        private void AppendLine(string text)
        {
            if (text == null)
            {
                return;
            }

            _writer.WriteLine(text);
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

                ConverterOptions options = this.Options;

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

                        if (drawing != null && options.GenerateImage)
                        {
                            _fileReader.SaveImage(svgFileName, target,
                                options.EncoderType);
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

        public override void Cancel()
        {
            if (_worker != null)
            {
                if (_worker.IsBusy)
                {
                    _worker.CancelAsync();

                    // Wait for the ConsoleWorker to finish the download.
                    while (_worker.IsBusy)
                    {
                        // Keep UI messages moving, so the form remains 
                        // responsive during the asynchronous operation.
                        MainApplication.DoEvents();
                    }
                }
            }
        }

        public override void Subscribe(IObserver observer)
        {
            _observer = observer;
        }

        #endregion
    }
}
