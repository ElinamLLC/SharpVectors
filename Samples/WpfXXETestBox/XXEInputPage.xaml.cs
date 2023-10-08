using System;
using System.Linq;
using System.Text;
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

using SharpVectors.Renderers.Wpf;

namespace WpfXXETestBox
{
    /// <summary>
    /// Interaction logic for XXEInputPage.xaml
    /// </summary>
    public partial class XXEInputPage : Page
    {
        private bool _isShown;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;

        private WpfDrawingSettings _wpfSettings;

        public XXEInputPage()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
        }

        public WpfDrawingSettings ConversionSettings
        {
            get {
                return _wpfSettings;
            }
            set {
                if (value != null)
                {
                    _wpfSettings = value;

                    if (_svgPage != null)
                    {
                        _svgPage.ConversionSettings = value;
                    }
                }
            }
        }

        public void OnContentRendered(EventArgs e)
        {
            if (_isShown)
            {
                return;
            }
            _isShown = true;

            _svgPage = frameSvgInput.Content as SvgPage;
            _xamlPage = frameXamlOutput.Content as XamlPage;

            if (_xamlPage != null)
            {
                //_xamlPage.ToolbarVisibility = false;
            }

            if (_svgPage != null)
            {
                _svgPage.XamlPage = _xamlPage;

                //_svgPage.ToolbarVisibility = false;
                _svgPage.InitializePage();
            }
        }

        public void OnContentCleared(EventArgs e)
        {
            if (_svgPage != null)
            {
                _svgPage.XamlPage = null;
                _svgPage.UnInitializePage();
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _svgPage = frameSvgInput.Content as SvgPage;
            _xamlPage = frameXamlOutput.Content as XamlPage;

            if (_svgPage != null && _xamlPage != null)
            {
                _svgPage.XamlPage = _xamlPage;
            }

            tabSvgInput.IsSelected = true;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
