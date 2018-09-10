using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isShown;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
            this.Closing += OnWindowClosing;
        }

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_isShown)
            {
                return;
            }
            _isShown = true;

            Console.WriteLine("OnContentRendered");

            if (_svgPage != null)
            {
                _svgPage.InitializeDocument();
            }
        }

        #endregion

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the display pages...
            _svgPage  = frameSvgInput.Content as SvgPage;
            _xamlPage = frameXamlOutput.Content as XamlPage;

            if (_svgPage != null)
            {
                _svgPage.XamlPage = _xamlPage;
            }

            Console.WriteLine("OnWindowLoaded");
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
        }

    }
}
