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
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters.Utils;

namespace SharpVectors.Converters
{
    public sealed class ConsoleDirectoryConverter : ConsoleConverter
    {
        #region Private Fields

        private int _convertedCount;
        private bool _continueOnError;
        private bool _isOverwrite;
        private bool _isRecursive;
        private bool _includeHidden;
        private bool _includeSecurity;

        private bool _writerErrorOccurred;
        private bool _fallbackOnWriterError;

        private List<string> _errorFiles;

        /// <summary>
        /// Only one observer is expected!
        /// </summary>
        private IObserver _observer;

        private string _sourceDir;
        private DirectoryInfo _sourceInfoDir;
        private DirectoryInfo _outputInfoDir;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private ConsoleWorker _worker;

        private ConsoleWriter _writer;

        #endregion

        #region Constructors and Destructor

        public ConsoleDirectoryConverter(string sourceDir)
        {
            _sourceDir = sourceDir;

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

            _isOverwrite = true;
            _isRecursive = true;
            _continueOnError = true;
        }

        #endregion

        #region Public Propeties

        public string SourceDir
        {
            get
            {
                return _sourceDir;
            }
            set
            {
                _sourceDir = value;
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
        /// Gets or sets a value indicating whether the directory copying is
        /// recursive, that is includes the sub-directories.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the sub-directories are
        /// included in the directory copy; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool Recursive
        {
            get
            {
                return _isRecursive;
            }

            set
            {
                _isRecursive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an existing file is overwritten.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if existing file is overwritten;
        /// otherwise, it is <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public bool Overwrite
        {
            get
            {
                return _isOverwrite;
            }

            set
            {
                _isOverwrite = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the security settings of the
        /// copied file is retained.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the security settings of the
        /// file is also copied; otherwise, it is <see langword="false"/>. The
        /// default is <see langword="false"/>.
        /// </value>
        public bool IncludeSecurity
        {
            get
            {
                return _includeSecurity;
            }

            set
            {
                _includeSecurity = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the copy operation includes
        /// hidden directories and files.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if hidden directories and files
        /// are included in the copy operation; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool IncludeHidden
        {
            get
            {
                return _includeHidden;
            }

            set
            {
                _includeHidden = value;
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
            if (String.IsNullOrEmpty(_sourceDir) || !Directory.Exists(_sourceDir))
            {
                return false;
            }

            _writer = writer;

            _errorFiles = new List<string>();

            string outputDir = this.OutputDir;

            try
            {
                this.AppendLine(String.Empty);
                this.AppendLine("Converting files, please wait...");
                this.AppendLine("Input Directory: " + _sourceDir);

                Debug.Assert(_sourceDir != null && _sourceDir.Length != 0);
                if (String.IsNullOrEmpty(outputDir))
                {
                    outputDir = String.Copy(_sourceDir);
                }
                _sourceInfoDir = new DirectoryInfo(_sourceDir);
                _outputInfoDir = new DirectoryInfo(outputDir);

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
            string outputDir = this.OutputDir;

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

                if (!String.IsNullOrEmpty(outputDir))
                {
                    builder.AppendLine("Output Directory: " + outputDir);
                }
                else if (_outputInfoDir != null)
                {
                    builder.AppendLine("Output Directory: " + _outputInfoDir.FullName);
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

            ConverterOptions options = this.Options;

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

            this.ProcessConversion(e, _sourceInfoDir, _outputInfoDir);

            if (!e.Cancel)
            {
                e.Result = "Successful";
            }
        }

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

        private void ProcessConversion(DoWorkEventArgs e, DirectoryInfo source,
            DirectoryInfo target)
        {
            if (e.Cancel)
            {
                return;
            }

            if (_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // Convert the files in the specified directory...
            this.ConvertFiles(e, source, target);

            if (e.Cancel)
            {
                return;
            }

            if (_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (!_isRecursive)
            {
                return;
            }

            // If recursive, process any sub-directory...
            DirectoryInfo[] arrSourceInfo = source.GetDirectories();

            int dirCount = (arrSourceInfo == null) ? 0 : arrSourceInfo.Length;

            for (int i = 0; i < dirCount; i++)
            {
                DirectoryInfo sourceInfo = arrSourceInfo[i];
                FileAttributes fileAttr = sourceInfo.Attributes;
                if (!_includeHidden)
                {
                    if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                }

                if (e.Cancel)
                {
                    break;
                }

                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                DirectoryInfo targetInfo = null;
                if (_includeSecurity)
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name,
                        sourceInfo.GetAccessControl());
                }
                else
                {
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name);
                }
                targetInfo.Attributes = fileAttr;

                this.ProcessConversion(e, sourceInfo, targetInfo);
            }
        }

        private void ConvertFiles(DoWorkEventArgs e, DirectoryInfo source,
            DirectoryInfo target)
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

            ConverterOptions options = this.Options;

            IEnumerable<string> fileIterator = DirectoryUtils.FindFiles(
              source, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string svgFileName in fileIterator)
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
                        FileAttributes fileAttr = File.GetAttributes(svgFileName);
                        if (!_includeHidden)
                        {
                            if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                continue;
                            }
                        }

                        FileSecurity security = null;

                        if (_includeSecurity)
                        {
                            security = File.GetAccessControl(svgFileName);
                        }

                        if (_worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        DrawingGroup drawing = _fileReader.Read(svgFileName,
                            target);

                        if (drawing == null)
                        {
                            if (_continueOnError)
                            {
                                throw new InvalidOperationException(
                                    "The conversion failed due to unknown error.");
                            }
                        }

                        if (options.SaveXaml)
                        {
                            string xamlFile = _fileReader.XamlFile;
                            if (!String.IsNullOrEmpty(xamlFile) &&
                                File.Exists(xamlFile))
                            {
                                File.SetAttributes(xamlFile, fileAttr);

                                // if required to set the security or access control
                                if (_includeSecurity)
                                {
                                    File.SetAccessControl(xamlFile, security);
                                }
                            }
                        }
                        if (options.SaveZaml)
                        {
                            string zamlFile = _fileReader.ZamlFile;
                            if (!String.IsNullOrEmpty(zamlFile) &&
                                File.Exists(zamlFile))
                            {
                                File.SetAttributes(zamlFile, fileAttr);

                                // if required to set the security or access control
                                if (_includeSecurity)
                                {
                                    File.SetAccessControl(zamlFile, security);
                                }
                            }
                        }

                        if (drawing != null && options.GenerateImage)
                        {
                            _fileReader.SaveImage(svgFileName, target,
                                options.EncoderType);
                            string imageFile = _fileReader.ImageFile;
                            if (!String.IsNullOrEmpty(imageFile) &&
                                File.Exists(imageFile))
                            {
                                File.SetAttributes(imageFile, fileAttr);

                                // if required to set the security or access control
                                if (_includeSecurity)
                                {
                                    File.SetAccessControl(imageFile, security);
                                }
                            }
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
