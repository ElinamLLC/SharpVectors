using System;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Wpf;

using Notification.Wpf;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool _isInitialising;
        private bool _isModified;
        private bool _isResourceModified;

        private SvgPage _svgPage;
        private SvgResourcePage _resourcePage;

        private WpfDrawingSettings _wpfSettings;
        private WpfDrawingSettings _conversionSettings;
        private WpfResourceSettings _resourceSettings;

        private NotificationManager _notifyIcon;

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

        public SvgResourcePage ResourcePage
        {
            get {
                return _resourcePage;
            }
            set {
                _resourcePage = value;
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isModified && _svgPage != null && _wpfSettings != null)
            {
                _svgPage.ConversionSettings = _wpfSettings;
            }
            if (_isResourceModified && _resourcePage != null)
            {
                if (_conversionSettings != null)
                {
                    _resourcePage.ConversionSettings = _conversionSettings;
                }
                if (_resourceSettings != null)
                {
                    _resourcePage.ResourceSettings = _resourceSettings;
                }
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
                    isValid = WpfResourceSettings.ValidateResourceNameFormat(nameFormat);
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
                    isValid = WpfResourceSettings.ValidateNameFormat(nameFormat, true);
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
            if (_svgPage == null)
            {
                return;
            }

            if (_notifyIcon == null)
            {
                _notifyIcon = new NotificationManager();
            }

            _isInitialising = true;

            if (_svgPage != null)
            {
                var wpfSettings = _svgPage.ConversionSettings;
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

            if (_resourcePage != null)
            {
                var conversionSettings = _resourcePage.ConversionSettings;
                if (conversionSettings != null)
                {
                    _conversionSettings = conversionSettings.Clone();

                    chkResourceTextAsGeometry.IsChecked        = _conversionSettings.TextAsGeometry;
                    chkResourceIncludeRuntime.IsChecked        = _conversionSettings.IncludeRuntime;

                    chkResourceIgnoreRootViewbox.IsChecked     = _conversionSettings.IgnoreRootViewbox;
                    chkResourceEnsureViewboxSize.IsChecked     = _conversionSettings.EnsureViewboxSize;
                    chkResourceEnsureViewboxPosition.IsChecked = _conversionSettings.EnsureViewboxPosition;

                    txtResourcePixelWidth.Text                 = _conversionSettings.PixelWidth.ToString();
                    txtResourcePixelHeight.Text                = _conversionSettings.PixelHeight.ToString();
                }

                var resourceSettings = _resourcePage.ResourceSettings;
                if (resourceSettings != null)
                {
                    _resourceSettings = resourceSettings.Clone();

                    chkResourceBindToColors.IsChecked     = _resourceSettings.BindToColors;
                    chkResourceBindToResources.IsChecked  = _resourceSettings.BindToResources;
                    chkResourceBindPenToBrushes.IsChecked = _resourceSettings.BindPenToBrushes;
                    txtResourcePenNameFormat.Text         = _resourceSettings.PenNameFormat;
                    txtResourceColorNameFormat.Text       = _resourceSettings.ColorNameFormat;
                    txtResourceBrushNameFormat.Text       = _resourceSettings.BrushNameFormat;
                    txtResourceNameFormat.Text            = _resourceSettings.ResourceNameFormat;
                    chkResourceFreeze.IsChecked           = _resourceSettings.ResourceFreeze;
                    chkResourceUseIndex.IsChecked         = _resourceSettings.UseResourceIndex;
                    cboResourceMode.SelectedIndex         = _resourceSettings.ResourceMode == WpfResourceMode.Drawing ? 0 : 1;
                    cboResourceAccess.SelectedIndex       = _resourceSettings.ResourceAccess == WpfResourceAccess.Dynamic ? 0 : 1;

                    cboIndentSpaces.SelectedValue         = _resourceSettings.IndentSpaces;
                    cboNumericPrecision.SelectedValue     = _resourceSettings.NumericPrecision;
                }
            }

            _isModified = false;
            _isResourceModified = false;

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

        private void OnConversionSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (_resourcePage == null || _conversionSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            _conversionSettings.TextAsGeometry        = chkResourceTextAsGeometry.IsChecked.Value;
            _conversionSettings.IncludeRuntime        = chkResourceIncludeRuntime.IsChecked.Value;

            _conversionSettings.IgnoreRootViewbox     = chkResourceIgnoreRootViewbox.IsChecked.Value;
            _conversionSettings.EnsureViewboxSize     = chkResourceEnsureViewboxSize.IsChecked.Value;
            _conversionSettings.EnsureViewboxPosition = chkResourceEnsureViewboxPosition.IsChecked.Value;

            _conversionSettings.PixelWidth  = GetNumberValue(txtResourcePixelWidth, 0);
            _conversionSettings.PixelHeight = GetNumberValue(txtResourcePixelHeight, 0);

            _isResourceModified = true;

            _isInitialising = false;
        }

        private void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
        }

        private void OnLostFocusHandler(object sender, RoutedEventArgs e)
        {
            TextBox ctrlSource = e.Source as TextBox;
            if (ctrlSource != null)
            {
                if (ctrlSource == txtResourcePenNameFormat || ctrlSource == txtResourceColorNameFormat
                    || ctrlSource == txtResourceBrushNameFormat || ctrlSource == txtResourceNameFormat)
                {
                    OnResourceSettingsChanged(ctrlSource, e);
                }
            }
        }

        private void OnResourceSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (_resourcePage == null || _resourceSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }
            if (sender == txtResourcePenNameFormat && txtResourcePenNameFormat.IsFocused)
            {
                return;
            }
            if (sender == txtResourceColorNameFormat && txtResourceColorNameFormat.IsFocused)
            {
                return;
            }
            if (sender == txtResourceBrushNameFormat && txtResourceBrushNameFormat.IsFocused)
            {
                return;
            }
            if (sender == txtResourceNameFormat && txtResourceNameFormat.IsFocused)
            {
                return;
            }

            _isInitialising = true;

            _resourceSettings.BindToColors       = chkResourceBindToColors.IsChecked.Value;
            _resourceSettings.BindToResources    = chkResourceBindToResources.IsChecked.Value;
            _resourceSettings.BindPenToBrushes   = chkResourceBindPenToBrushes.IsChecked.Value;
            _resourceSettings.PenNameFormat      = GetTextValue(txtResourcePenNameFormat, WpfDrawingResources.DefaultPenNameFormat);
            _resourceSettings.ColorNameFormat    = GetTextValue(txtResourceColorNameFormat, WpfDrawingResources.DefaultColorNameFormat);
            _resourceSettings.BrushNameFormat    = GetTextValue(txtResourceBrushNameFormat, WpfDrawingResources.DefaultBrushNameFormat);
            _resourceSettings.ResourceNameFormat = GetTextValue(txtResourceNameFormat, WpfDrawingResources.DefaultResourceNameFormat);
            _resourceSettings.ResourceFreeze     = chkResourceFreeze.IsChecked.Value;
            _resourceSettings.UseResourceIndex   = chkResourceUseIndex.IsChecked.Value;
            _resourceSettings.ResourceMode       = cboResourceMode.SelectedIndex == 0 ? WpfResourceMode.Drawing : WpfResourceMode.Image;
            _resourceSettings.ResourceAccess     = cboResourceAccess.SelectedIndex == 0 ? WpfResourceAccess.Dynamic : WpfResourceAccess.Static;

            _resourceSettings.IndentSpaces       = (int)cboIndentSpaces.SelectedValue;
            _resourceSettings.NumericPrecision   = (int)cboNumericPrecision.SelectedValue;

            _isResourceModified = true;

            _isInitialising = false;

            ValidateNameFormat(txtResourcePenNameFormat, "Pen Name Format", WpfDrawingResources.DefaultPenNameFormat, false);
            ValidateNameFormat(txtResourceColorNameFormat, "Color Name Format", WpfDrawingResources.DefaultColorNameFormat, false);
            ValidateNameFormat(txtResourceBrushNameFormat, "Brush Name Format", WpfDrawingResources.DefaultBrushNameFormat, false);
            ValidateNameFormat(txtResourceNameFormat, "Resource Name Format", WpfDrawingResources.DefaultResourceNameFormat, true);
        }
    }
}
