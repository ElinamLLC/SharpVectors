using System;
using System.Diagnostics;
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

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool _isInitialising;
        private bool _isModified;

        private SvgPage _svgPage;

        private WpfDrawingSettings _wpfSettings;

        public SettingsPage()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
        }

        public SvgPage SvgPage
        {
            get {
                return _svgPage;
            }
            set {
                _svgPage = value;
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isModified && _svgPage != null && _wpfSettings != null)
            {
                _svgPage.ConversionSettings = _wpfSettings;
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_svgPage == null)
            {
                return;
            }
            var wpfSettings = _svgPage.ConversionSettings;
            if (wpfSettings != null)
            {
                _wpfSettings = wpfSettings.Clone();
            }
            if (_wpfSettings == null)
            {
                return;
            }

            _isInitialising = true;

            chkTextAsGeometry.IsChecked        = _wpfSettings.TextAsGeometry;
            chkIncludeRuntime.IsChecked        = _wpfSettings.IncludeRuntime;

            chkIgnoreRootViewbox.IsChecked     = _wpfSettings.IgnoreRootViewbox;
            chkEnsureViewboxSize.IsChecked     = _wpfSettings.EnsureViewboxSize;
            chkEnsureViewboxPosition.IsChecked = _wpfSettings.EnsureViewboxPosition;

            _isModified = false;

            _isInitialising = false;
        }

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (_svgPage == null || _wpfSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            _wpfSettings.TextAsGeometry        = chkTextAsGeometry.IsChecked.Value;
            _wpfSettings.IncludeRuntime        = chkIncludeRuntime.IsChecked.Value;

            _wpfSettings.IgnoreRootViewbox     = chkIgnoreRootViewbox.IsChecked.Value;
            _wpfSettings.EnsureViewboxSize     = chkEnsureViewboxSize.IsChecked.Value;
            _wpfSettings.EnsureViewboxPosition = chkEnsureViewboxPosition.IsChecked.Value;

            _isModified = true;

            _isInitialising = false;
        }

    }
}
