using System;
using System.Windows;
using System.ComponentModel;

namespace WpfShapeSvgTestBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private bool _isShown;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnWindowLoaded;
            this.Closing += OnWindowClosing;
        }

        #endregion

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

            if (_svgPage != null)
            {
                _svgPage.InitializeDocument();
            }
        }

        #endregion

        #region Private Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the display pages...
            _svgPage  = frameSvgInput.Content as SvgPage;
            _xamlPage = frameXamlOutput.Content as XamlPage;

            if (_svgPage != null)
            {
                //_svgPage.XamlPage = _xamlPage;
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
        }

        #endregion
    }
}
