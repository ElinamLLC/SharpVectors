using System;
using System.Threading;

using System.Windows;
using System.Windows.Threading;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for MainApplication.xaml
    /// </summary>
    public partial class MainApplication : Application
    {
        private ConverterCommandLines _commandLines;

        public MainApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnDomainUnhandledException);
            this.DispatcherUnhandledException          += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(OnApplicationUnhandledException);
        }

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

        public void InitializeComponent(bool mainWindow)
        {
            if (mainWindow)
            {
                this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            }
            else
            {
                this.StartupUri = new Uri("ConverterWindow.xaml", UriKind.Relative);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {             
            base.OnExit(e);
        }        

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);
        }

        private void OnApplicationUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                try
                {
                    //WiringErrorWindow errorDlg = new WiringErrorWindow();
                    //errorDlg.Owner = this.MainWindow;
                    //errorDlg.Initialize(e.Exception);

                    //errorDlg.ShowDialog();
                }
                catch
                {
                }
            }

            e.Handled = true;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject == null)
            {
                return;
            }

            try
            {
                //WiringErrorWindow errorDlg = new WiringErrorWindow();
                //errorDlg.Owner = this.MainWindow;
                //errorDlg.Initialize(e.ExceptionObject);

                //errorDlg.ShowDialog();
            }
            catch
            {
            }
        }

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                var f = arg as DispatcherFrame;
                f.Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
        }
    }
}
