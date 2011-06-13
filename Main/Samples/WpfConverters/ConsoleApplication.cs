using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Converters
{
    public sealed class ConsoleApplication : IObserver, IObservable
    {
        #region Private Fields

        private bool _isConverting;
        private bool _isConversionError;

        private bool _consoleSuccess;
        private bool _startedInConsole;

        private IObserver _observer;

        private Process _process;

        private ConsoleWriter    _writer;
        private ConsoleProgress  _progressBar;
        private ConsoleConverter _converterOutput;
        private ConverterOptions _options;
        private ConverterCommandLines _commandLines;

        #endregion

        #region Constructors and Destructor

        public ConsoleApplication(Process process)
        {
            _process = process;               
            _options = new ConverterOptions();
        }

        #endregion

        #region Public Properties

        public ConverterCommandLines CommandLines
        {
            get
            {
                return _commandLines;
            }
            set
            {
                _commandLines = value;
            }
        }

        #endregion

        #region Public Methods

        public void InitializeComponent(bool startedInConsole, bool isQuiet)
        {
            _startedInConsole = startedInConsole;

            _writer = new ConsoleWriter(isQuiet, ConsoleWriterVerbosity.Normal);

            _progressBar = new ConsoleProgress(isQuiet, _writer);
            this.Subscribe(_progressBar);
        }

        public int Run()
        {
            Debug.Assert(_writer != null);
            Debug.Assert(_process != null);
            Debug.Assert(_commandLines != null);

            _isConverting = false;
            _isConversionError = false;

            bool? isControlling = null;
            try
            {
                if (!_writer.IsQuiet)
                {
                    // If not quiet, we will display the progress information
                    // to the console window, so try creating or attaching to 
                    // existing one...
                    _consoleSuccess = this.CreateConsole();
                    if (!_consoleSuccess)
                    {
                        return 1;
                    }

                    // Turn off the default system behavior when CTRL+C is pressed. When 
                    // Console.TreatControlCAsInput is false, CTRL+C is treated as an
                    // interrupt instead of as input.
                    isControlling = Console.TreatControlCAsInput;

                    Console.TreatControlCAsInput = false;

                    Console.CancelKeyPress += new ConsoleCancelEventHandler(OnConsoleCancelKeyPress);
                }

                _options.Update(_commandLines);

                _converterOutput = this.CreateConverter();
                if (_converterOutput == null)
                {
                    return 1;
                }

                // The convert method will simply start the background
                // conversion thread and start processing. It will return
                // immediately...
                bool startedConversion = _converterOutput.Convert(_writer);
                if (!startedConversion)
                {
                    return 1;
                }

                _writer.WriteLine("Press Control + C keys to cancel the conversion.");
                while (_isConverting)
                {
                    // just wait...
                }

                return 0;
            }
            catch (Exception ex)
            {     
             	if (_writer != null)
                {
                    string message = ex.Message;
                    if (String.IsNullOrEmpty(message))
                    {
                        _writer.WriteErrorLine(ex.ToString());
                    }
                    else
                    {
                        _writer.WriteErrorLine(message);
                    }
                }

                return 1;
            }
            finally
            {
                if (isControlling != null && isControlling.HasValue)
                {
                    Console.TreatControlCAsInput = isControlling.Value;

                    Console.CancelKeyPress -= new ConsoleCancelEventHandler(OnConsoleCancelKeyPress);
                }

                if (_consoleSuccess)
                {
                    this.DestroyConsole();

                    _consoleSuccess = false;
                }
            }
        }

        public int Help()
        {
            Debug.Assert(_writer != null);
            Debug.Assert(_process != null);
            Debug.Assert(_commandLines != null);

            _isConverting      = false;
            _isConversionError = false;

            try
            {
                if (!_writer.IsQuiet)
                {
                    // If not quiet, we will display the progress information
                    // to the console window, so try creating or attaching to 
                    // existing one...
                    _consoleSuccess = this.CreateConsole();
                    if (!_consoleSuccess)
                    {
                        return 1;
                    }
                }

                if (_consoleSuccess)
                {
                    string usageText = _commandLines.Usage;
                    if (!String.IsNullOrEmpty(usageText))
                    {
                        _writer.WriteLine(usageText);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {     
             	if (_writer != null)
                {
                    string message = ex.Message;
                    if (String.IsNullOrEmpty(message))
                    {
                        _writer.WriteErrorLine(ex.ToString());
                    }
                    else
                    {
                        _writer.WriteErrorLine(message);
                    }
                }

                return 1;
            }
            finally
            {
                if (_consoleSuccess)
                {
                    this.DestroyConsole();

                    _consoleSuccess = false;
                }
            }
        }

        #endregion

        #region Private Methods

        private ConsoleConverter CreateConverter()
        {
            Debug.Assert(_commandLines != null);
            if (_commandLines == null)
            {
                return null;
            }

            string outputDir  = _commandLines.OutputDir;

            string sourceFile = _commandLines.SourceFile;
            if (!String.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
            {
                ConsoleFileConverter fileConverter = 
                    new ConsoleFileConverter(sourceFile);

                fileConverter.Options   = _options;
                fileConverter.OutputDir = outputDir;

                fileConverter.Subscribe(this);

                return fileConverter;
            }

            string sourceDir = _commandLines.SourceDir;
            if (!String.IsNullOrEmpty(sourceDir) && Directory.Exists(sourceDir))
            {
                ConsoleDirectoryConverter dirConverter = 
                    new ConsoleDirectoryConverter(sourceDir);

                dirConverter.Options   = _options;
                dirConverter.OutputDir = outputDir;

                dirConverter.Recursive       = _commandLines.Recursive;
                dirConverter.ContinueOnError = _commandLines.ContinueOnError;

                dirConverter.Subscribe(this);

                return dirConverter;
            }

            IList<string> sourceFiles = _commandLines.SourceFiles;
            if (sourceFiles != null && sourceFiles.Count != 0)
            {
                ConsoleFilesConverter filesConverter = 
                    new ConsoleFilesConverter(sourceFiles);

                filesConverter.Options = _options;
                filesConverter.OutputDir = outputDir;

                filesConverter.ContinueOnError = _commandLines.ContinueOnError;

                filesConverter.Subscribe(this);

                return filesConverter;
            }

            return null;
        }

        private bool CreateConsole()
        {
            try
            {
                bool consoleSuccess = false;
                // If the uppermost window a cmd process...
                if (_startedInConsole && _process != null)
                {
                    consoleSuccess = ConverterWindowsAPI.AttachConsole(_process.Id);
                    if (!consoleSuccess)
                    {
                        consoleSuccess = ConverterWindowsAPI.AllocConsole();
                        if (consoleSuccess)
                        {
                            Console.Title = "SVG-WPF Converter";
                        }
                    }
                }
                else
                {
                    consoleSuccess = ConverterWindowsAPI.AllocConsole();
                    if (consoleSuccess)
                    {
                        Console.Title = "SVG-WPF Converter";
                    }
                }

                return consoleSuccess;
            }
            catch
            {
                return false;
            }
        }

        public void DestroyConsole()
        {   
            try
            {
                this.AppendLine(String.Empty);
                this.AppendLine("Press the Enter key to continue...");

                if (_commandLines.BeepOnEnd)
                {
                    if (_isConversionError)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Console.Beep(4000, 50);
                            Console.Beep(1000, 25);
                            Thread.Sleep(30);
                        }
                    }
                    else
                    {
                        Console.Beep(2000, 150);
                        Console.Beep(2000, 150);
                        Console.Beep(2000, 150);
                        Console.Beep(1500, 300);
                    }  
                }

                ConverterWindowsAPI.FreeConsole();
            }
            catch
            {
            	
            }
        }

        private void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_isConverting)
            {
                this.Cancel();
            }

            e.Cancel = !_isConverting;
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

            if (_observer != null)
            {
                _observer.OnStarted(this);
            }
        }

        public void OnCompleted(IObservable sender, bool isSuccessful)
        {
            if (_observer != null)
            {
                _observer.OnCompleted(this, isSuccessful);

                Thread.Sleep(100); // wait a bit...
            }

            if (isSuccessful)
            {
                this.AppendLines("Conversion: Successful",
                    "The conversion is completed successfully.", false);
            }
            else
            {
                this.AppendLines("Conversion: Failed",
                    "The conversion failed, see the output for further information.", true);
            }

            _isConversionError = isSuccessful ? false : true;

            _isConverting = false;
        }

        private IntPtr GetWindow()
        {
            if (_process != null && _process.MainWindowHandle != IntPtr.Zero)
            {
                return _process.MainWindowHandle;
            }

            return ConverterWindowsAPI.GetConsoleWindow();
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

        private void AppendLines(string title, string text, bool isError)
        {
            if (text == null)
            {
                return;
            }

            _writer.WriteLine(text);
        }

        #endregion
    }
}
