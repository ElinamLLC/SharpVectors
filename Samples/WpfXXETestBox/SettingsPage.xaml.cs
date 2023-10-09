using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;

using Notification.Wpf;

namespace WpfXXETestBox
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool _isInitialising;
        private bool _isModified;

        private WpfDrawingSettings _wpfSettings;

        private NotificationManager _notifyIcon;

        private ResourceKeyResolver _defaultKeyResolver;
        private DictionaryKeyResolver _dictionaryKeyResolver;
        private CodeSnippetKeyResolver _codeSnippetKeyResolver;

        private MainWindow _mainWindow;

        public SettingsPage()
        {
            InitializeComponent();

            _defaultKeyResolver     = new ResourceKeyResolver();
            _dictionaryKeyResolver  = new DictionaryKeyResolver();
            _codeSnippetKeyResolver = new CodeSnippetKeyResolver("", "cs");

            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
        }

        public MainWindow Window
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isModified && _mainWindow != null && _wpfSettings != null)
            {
                _mainWindow.ConversionSettings = _wpfSettings;
            }

            if (_notifyIcon != null)
            {
                //_notifyIcon.Visible = false;
                _notifyIcon.Close();
            }
            _notifyIcon = null;
        }

        private bool ValidateNameFormat(TextBox textBox, string inputTitle, string defaultValue, bool isRootResource)
        {
            try
            {
                if (textBox.IsFocused)
                {
                    return true;
                }

                bool isValid = true;

                string nameFormat = GetTextValue(textBox, string.Empty);
                if (isRootResource)
                {
                    if (string.IsNullOrWhiteSpace(nameFormat))
                    {
                        return true;
                    }
                    isValid = ResourceKeyResolver.ValidateResourceNameFormat(nameFormat);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(nameFormat))
                    {
                        if (_notifyIcon != null)
                        {
                            _notifyIcon.Show(inputTitle, "The name format is invalid.",
                                NotificationType.Error, string.Empty, TimeSpan.FromSeconds(30));
                        }
                        return false;
                    }
                    isValid = ResourceKeyResolver.ValidateNameFormat(nameFormat, true);
                }

                if (!isValid)
                {
                    if (_notifyIcon != null)
                    {
                        _notifyIcon.Show(inputTitle, "The name format is invalid.",
                            NotificationType.Error, string.Empty, TimeSpan.FromSeconds(30));
                    }
                }
                return isValid;
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(inputTitle))
                {
                    inputTitle = MainWindow.AppTitle;
                }
                if (_notifyIcon != null)
                {
                    _notifyIcon.Show(inputTitle, "The name format is invalid.",
                        NotificationType.Error, string.Empty, TimeSpan.FromSeconds(30));
                }
                return false;
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null)
            {
                return;
            }

            if (_notifyIcon == null)
            {
                _notifyIcon = new NotificationManager();
            }

            _isInitialising = true;

            if (_mainWindow != null)
            {
                var wpfSettings = _mainWindow.ConversionSettings;
                if (wpfSettings != null)
                {
                    _wpfSettings = wpfSettings.Clone();

                    chkTextAsGeometry.IsChecked        = _wpfSettings.TextAsGeometry;
                    chkIncludeRuntime.IsChecked        = _wpfSettings.IncludeRuntime;

                    chkIgnoreRootViewbox.IsChecked     = _wpfSettings.IgnoreRootViewbox;
                    chkEnsureViewboxSize.IsChecked     = _wpfSettings.EnsureViewboxSize;
                    chkEnsureViewboxPosition.IsChecked = _wpfSettings.EnsureViewboxPosition;
                }
            }

            _isModified = false;

            _isInitialising = false;
        }

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null || _wpfSettings == null)
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

        private static string GetTextValue(TextBox textBox)
        {
            string textVal = textBox.Text;
            if (textVal == null)
            {
                return string.Empty;
            }
            return textVal.Trim();
        }

        private static string GetTextValue(TextBox textBox, string defaultValue)
        {
            string textVal = GetTextValue(textBox);
            if (textVal.Length == 0)
            {
                return defaultValue;
            }
            return textVal;
        }

        private static int GetNumberValue(TextBox textBox, int defaultValue)
        {
            string textVal = GetTextValue(textBox);
            if (textVal.Length == 0)
            {
                textBox.Text = String.Empty;
                return defaultValue;
            }

            if (int.TryParse(textVal, out int numberVal))
            {
                return numberVal;
            }
            textBox.Text = defaultValue.ToString();
            return defaultValue;
        }

        private void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
        }

        private void OnLostFocusHandler(object sender, RoutedEventArgs e)
        {
        }
    }
}
