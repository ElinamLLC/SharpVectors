﻿using System;
using System.Windows;
using System.ComponentModel;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string AppTitle      = "WpfSvgTestBox";
        public const string AppErrorTitle = "WpfSvgTestBox - Error";

        #region Private Fields

        private bool _isShown;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;
        private DebugPage _debugPage;
        private SettingsPage _settingsPage;
        private SvgResourcePage _resourcePage;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded  += OnWindowLoaded;
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
            _svgPage      = frameSvgInput.Content as SvgPage;
            _xamlPage     = frameXamlOutput.Content as XamlPage;
            _debugPage    = frameDebugging.Content as DebugPage;
            _settingsPage = frameSettings.Content as SettingsPage;
            _resourcePage = frameSvgResource.Content as SvgResourcePage;  

            if (_svgPage != null && _xamlPage != null)
            {
                _svgPage.XamlPage = _xamlPage;
            }

            if (_svgPage != null && _settingsPage != null)
            {
                _settingsPage.SvgPage = _svgPage;
            }

            if (_svgPage != null && _resourcePage != null)
            {
                _resourcePage.Window = this;
                _settingsPage.ResourcePage = _resourcePage;
            }

            if (_debugPage != null)
            {
                _debugPage.Startup();
            }

            tabSvgInput.IsSelected = true;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_debugPage != null)
            {
                _debugPage.Shutdown();
            }
        }

        #endregion
    }
}
