using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for ConverterWindow.xaml
    /// </summary>
    public partial class ConverterWindow : Window, IObserver
    {
        #region Private Fields

        private delegate void ConvertHandler();

        private bool _isConverting;

        private ConverterOptions _options;

        private FileListConverterOutput _converterOutput;

        #endregion

        #region Constructors and Destructor

        public ConverterWindow()
        {
            InitializeComponent();

            this.MinWidth  = 640;
            this.MinHeight = 340;

            this.Width     = 640;
            this.Height    = 340;

            _options = new ConverterOptions();

            this.Loaded   += new RoutedEventHandler(OnWindowLoaded);
            this.Unloaded += new RoutedEventHandler(OnWindowUnloaded);

            this.Closing  += new CancelEventHandler(OnWindowClosing);
            this.ContentRendered += new EventHandler(OnWindowContentRendered);
        }

        #endregion

        #region Private Event Handlers

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            if (_options == null || !_options.IsValid)
            {
                return;
            }

            MainApplication theApp = (MainApplication)Application.Current;
            Debug.Assert(theApp != null);
            if (theApp == null)
            {
                return;
            }
            ConverterCommandLines commandLines = theApp.CommandLines;
            Debug.Assert(commandLines != null);
            if (commandLines == null || commandLines.IsEmpty)
            {
                return;
            }
            IList<string> sourceFiles = commandLines.SourceFiles;
            if (sourceFiles == null || sourceFiles.Count == 0)
            {
                string sourceFile = commandLines.SourceFile;
                if (String.IsNullOrEmpty(sourceFile) || 
                    !File.Exists(sourceFile))
                {
                    return;
                }
                sourceFiles = new List<string>();
                sourceFiles.Add(sourceFile);
            }

            _isConverting = true;

            if (_converterOutput == null)
            {
                _converterOutput = new FileListConverterOutput();
            }

            _options.Update(commandLines);

            _converterOutput.Options = _options;
            _converterOutput.Subscribe(this);

            _converterOutput.ContinueOnError = commandLines.ContinueOnError;
            _converterOutput.SourceFiles = sourceFiles;
            _converterOutput.OutputDir  = commandLines.OutputDir;

            frameConverter.Content = _converterOutput;

            //_converterOutput.Convert();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new ConvertHandler(_converterOutput.Convert));
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (_isConverting)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Conversion process is running on the background.");
                    builder.AppendLine("Do you want to stop the conversion process and close this application?");
                    MessageBoxResult boxResult = MessageBox.Show(builder.ToString(), this.Title,
                        MessageBoxButton.YesNo, MessageBoxImage.Warning,
                        MessageBoxResult.No);

                    if (boxResult == MessageBoxResult.No)
                    {
                        e.Cancel = false;
                        return;
                    }

                    if (_converterOutput != null)
                    {
                        _converterOutput.Cancel();
                    }
                }
            }
            catch
            {
            }
        }

        private void OnClickClosed(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region IObserver Members

        public void OnStarted(IObservable sender)
        {
            progressBar.Visibility = Visibility.Visible;
        }

        public void OnCompleted(IObservable sender, bool isSuccessful)
        {
            progressBar.Visibility = Visibility.Hidden;

            _isConverting = false;
        }

        #endregion
    }
}
