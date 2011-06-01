using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        private bool _isInitialising;
        private ConverterOptions _options;

        public OptionsPage()
        {
            InitializeComponent();

            // Reset the dimensions...
            this.Width  = Double.NaN;
            this.Height = Double.NaN;

            this.Loaded += new RoutedEventHandler(OnOptionsPageLoaded);
        }

        public ConverterOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
            }
        }

        private void OnOptionsPageLoaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(_options != null);
            if (_options == null)
            {
                _options = new ConverterOptions();
            }

            _isInitialising = true;

            chkTextAsGeometry.IsChecked = _options.TextAsGeometry;
            chkIncludeRuntime.IsChecked = _options.IncludeRuntime;
            
            chkXaml.IsChecked       = _options.GeneralWpf;
            panelXaml.IsEnabled     = _options.GeneralWpf;
            chkSameXaml.IsChecked   = _options.SaveXaml;
            chkSameZaml.IsChecked   = _options.SaveZaml;
            chkXamlWriter.IsChecked = _options.UseCustomXamlWriter;

            chkImage.IsChecked      = _options.GenerateImage;
            panelImage.IsEnabled    = _options.GenerateImage;
            cboImages.SelectedIndex = (int)_options.EncoderType;

            _isInitialising = false;
        }

        private void OnOptionChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            if (chkImage == sender)
            {
                if (panelImage != null)
                {
                    panelImage.IsEnabled = chkImage.IsChecked.Value;
                }    
            }
            if (chkXaml == sender)
            {
                if (panelXaml != null)
                {
                    panelXaml.IsEnabled = chkXaml.IsChecked.Value;
                }
            }    
                                      
            _options.TextAsGeometry      = chkTextAsGeometry.IsChecked.Value;
            _options.IncludeRuntime      = chkIncludeRuntime.IsChecked.Value;

            _options.GeneralWpf          = chkXaml.IsChecked.Value;
            //_options.GeneralWpf = panelXaml.IsEnabled;
            _options.SaveXaml            = chkSameXaml.IsChecked.Value;
            _options.SaveZaml            = chkSameZaml.IsChecked.Value;
            _options.UseCustomXamlWriter = chkXamlWriter.IsChecked.Value;

            _options.GenerateImage       = chkImage.IsChecked.Value;
            //_options.GenerateImage = panelImage.IsEnabled;
            if (cboImages.SelectedIndex >= 0)
            {
                _options.EncoderType = (ImageEncoderType)cboImages.SelectedIndex;
            }

            _isInitialising = false;
        }
    }
}
