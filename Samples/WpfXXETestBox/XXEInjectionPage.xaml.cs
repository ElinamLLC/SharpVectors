using System;
using System.Collections.Generic;
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

using SharpVectors.Renderers.Wpf;

namespace WpfXXETestBox
{
    /// <summary>
    /// Interaction logic for XXEInjectionPage.xaml
    /// </summary>
    public partial class XXEInjectionPage : Page
    {
        private bool _isShown;

        private XXEInsidePage _insidePage;
        private XXEClassicPage _classicPage;
        private XXEOutsidePage _outsidePage;

        private WpfDrawingSettings _wpfSettings;

        public XXEInjectionPage()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
        }

        #region Public Properties

        #endregion

        public WpfDrawingSettings ConversionSettings
        {
            get {
                return _wpfSettings;
            }
            set {
                if (value != null)
                {
                    _wpfSettings = value;

                    if (_insidePage != null)
                    {
                        _insidePage.ConversionSettings = value;
                    }
                    if (_classicPage != null)
                    {
                        _classicPage.ConversionSettings = value;
                    }
                    if (_outsidePage != null)
                    {
                        _outsidePage.ConversionSettings = value;
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

            _insidePage = frameXXEInside.Content as XXEInsidePage;
            _classicPage = frameXXEClassic.Content as XXEClassicPage;
            _outsidePage = frameXXEOutside.Content as XXEOutsidePage;

            if (_insidePage != null)
            {
                //_insidePage.ToolbarVisibility = false;
                _insidePage.InitializePage();
            }

            if (_classicPage != null)
            {
                //_classicPage.ToolbarVisibility = false;
                _classicPage.InitializePage();
            }

            if (_outsidePage != null)
            {
                //_outsidePage.ToolbarVisibility = false;
                _outsidePage.InitializePage();
            }
        }

        public void OnContentCleared(EventArgs e)
        {
            if (_insidePage != null)
            {
                _insidePage.UnInitializePage();
            }

            if (_classicPage != null)
            {
                _classicPage.UnInitializePage();
            }

            if (_outsidePage != null)
            {
                _outsidePage.UnInitializePage();
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            _insidePage = frameXXEInside.Content as XXEInsidePage;
            _classicPage = frameXXEClassic.Content as XXEClassicPage;
            _outsidePage = frameXXEOutside.Content as XXEOutsidePage;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
        }

    }
}
