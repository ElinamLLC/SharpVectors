using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    public sealed class ConsoleFileConverter : ConsoleConverter
    {
        #region Private Fields

        private string _imageFile;
        private string _xamlFile;
        private string _zamlFile;

        private IObserver _observer;

        private DrawingGroup _drawing;

        private string _sourceFile;
        private DirectoryInfo _outputInfoDir;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        private ConsoleWriter _writer;

        private ConsoleWorker _worker;

        #endregion

        #region Constructors and Destructor

        public ConsoleFileConverter(string sourceFile)
        {
            _sourceFile = sourceFile;

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
        }

        #endregion

        #region Public Propeties

        public string SourceFile
        {
            get
            {
                return _sourceFile;
            }
        }

        #endregion

        #region Public Methods

        public override bool Convert(ConsoleWriter writer)
        {
            Debug.Assert(writer != null);

            Debug.Assert(_sourceFile != null && _sourceFile.Length != 0);
            if (String.IsNullOrEmpty(_sourceFile) || !File.Exists(_sourceFile))
            {
                return false;
            }

            _writer = writer;

            try
            {
                this.AppendLine(String.Empty);
                this.AppendLine("Converting file, please wait...");
                this.AppendLine("Input File: " + _sourceFile);

                string _outputDir = this.OutputDir;
                if (String.IsNullOrEmpty(_outputDir))
                {
                    _outputDir = Path.GetDirectoryName(_sourceFile);
                }
                _outputInfoDir = new DirectoryInfo(_outputDir);

                //this.OnConvert();

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

        #region Private Methods

        #region ConsoleWorker Methods

        private void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_drawing != null)
            {
                if (!_drawing.IsFrozen)
                {
                    _drawing.Freeze();
                }
            }

            bool isSuccessful = false;

            StringBuilder builder = new StringBuilder();
            if (e.Error != null || _drawing == null)
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

                isSuccessful = false;
            }
            else if (e.Cancelled)
            {
                builder.AppendLine("Result: Cancelled");

                isSuccessful = false;
            }
            else if (e.Result != null)
            {
                string resultText = e.Result.ToString();
                if (!String.IsNullOrEmpty(resultText))
                {
                    builder.AppendLine("Result: " + resultText);
                }

                builder.AppendLine("Output Files:");
                if (_xamlFile != null)
                {
                    builder.AppendLine(_xamlFile);
                }
                if (_zamlFile != null)
                {
                    builder.AppendLine(_zamlFile);
                }
                if (_imageFile != null)
                {
                    builder.AppendLine(_imageFile);
                }

                isSuccessful = String.Equals(resultText, "Successful",
                        StringComparison.OrdinalIgnoreCase);
            }

            this.AppendLine(builder.ToString());
            if (_observer != null)
            {
                _observer.OnCompleted(this, isSuccessful);
            }
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

            _drawing = _fileReader.Read(_sourceFile, _outputInfoDir);

            if (_drawing == null)
            {
                e.Result = "Failed";
                return;
            }

            if (options.GenerateImage)
            {
                _fileReader.SaveImage(_sourceFile, _outputInfoDir,
                    options.EncoderType);

                _imageFile = _fileReader.ImageFile;
            }
            _xamlFile = _fileReader.XamlFile;
            _zamlFile = _fileReader.ZamlFile;

            if (_drawing.CanFreeze)
            {
                _drawing.Freeze();
            }

            e.Result = "Successful";
        }

        #endregion

        #region Other Methods

        private void OnSyncConvert()
        {
            StringBuilder builder = new StringBuilder();
            try
            {
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

                Drawing drawing = _fileReader.Read(_sourceFile, _outputInfoDir);

                if (drawing == null)
                {
                    this.AppendLine("Result: Conversion Failed.");
                }
                else
                {
                    string _imageFile = null;
                    if (options.GenerateImage)
                    {
                        _fileReader.SaveImage(_sourceFile, _outputInfoDir,
                            options.EncoderType);

                        _imageFile = _fileReader.ImageFile;
                    }
                    string _xamlFile = _fileReader.XamlFile;
                    string _zamlFile = _fileReader.ZamlFile;

                    builder.AppendLine("Result: Conversion is Successful.");

                    builder.AppendLine("Output Files:");
                    if (_xamlFile != null)
                    {
                        builder.AppendLine(_xamlFile);
                    }
                    if (_zamlFile != null)
                    {
                        builder.AppendLine(_zamlFile);
                    }
                    if (_imageFile != null)
                    {
                        builder.AppendLine(_imageFile);
                    }
                }  
            }
            catch (Exception ex)
            {
                builder.AppendFormat("Error: Exception ({0})", ex.GetType());
                builder.AppendLine();
                builder.AppendLine(ex.Message);
                builder.AppendLine(ex.ToString());
            }

            this.AppendLine(builder.ToString());
        }

        private void AppendText(string text)
        {
            if (text == null)
            {
                return;
            }

            _writer.WriteLine(text);
        }

        private void AppendLine(string text)
        {
            if (text == null)
            {
                return;
            }

            _writer.WriteLine(text);
        }

        #endregion

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
