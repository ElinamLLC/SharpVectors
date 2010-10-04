using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string StartedConversion   = "Started SVG to XAML/ZAML Conversion.";
        private const string CompletedConversion = "Completed SVG to XAML/ZAML Conversion Successfully.";

        private BackgroundWorker _worker;
        private delegate void ProcessConversionHandler(TextBlock textBlock);

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(OnWindowLoaded);
            _worker = new BackgroundWorker();

            _worker.DoWork += new DoWorkEventHandler(OnWorkerDoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(OnWorkerProgressChanged);            
        }

        private void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.IsEnabled = true;

            if (e.Result != null)
            {
                string resultText = e.Result.ToString();
                if (String.Equals(resultText, CompletedConversion, StringComparison.OrdinalIgnoreCase))
                {
                    txtMessage.Text = resultText;
                    txtMessage.Background = Brushes.LightBlue;
                }
                else
                {
                    txtMessage.Text = resultText;
                    txtMessage.Background = Brushes.LightPink;
                }
            }
            else
            {
                txtMessage.Text = CompletedConversion;                
            }
        }

        private void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            string sourceDir =
                @"";

            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDir);

            string destDir =
                @"";

            DirectoryInfo destInfo = new DirectoryInfo(destDir);

            DirectorySvgConverter converter = new DirectorySvgConverter();

            try
            {
                converter.SaveXaml = false;

                converter.Convert(sourceInfo, destInfo);

                e.Result = CompletedConversion;
            }
            catch (Exception ex)
            {
                //textBlock.Text = ex.Message;
                //textBlock.Text = converter.ErrorFile;

                e.Result = converter.ErrorFile;

                MessageBox.Show(ex.ToString());
            }                                  
        }

        private void OnStartConversion(object sender, RoutedEventArgs e)
        {
            txtMessage.Text       = StartedConversion;
            txtMessage.Background = Brushes.LightBlue;

            //ProcessConversionHandler handler =
            //    new ProcessConversionHandler(ProcessConversion);
            //this.Dispatcher.BeginInvoke(handler, DispatcherPriority.Background, txtMessage);
            _worker.RunWorkerAsync();

            btnStart.IsEnabled = false;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private static void ProcessConversion(TextBlock textBlock)
        {
        }
    }
}
