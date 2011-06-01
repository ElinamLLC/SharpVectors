using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Interaction logic for HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page
    {
        public HelpPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(OnHelpPageLoaded);

            // Reset the dimensions...
            this.Width  = Double.NaN;
            this.Height = Double.NaN;
        }

        private void OnHelpPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "SharpVectors.Converters.ConverterHelp.xaml");
                if (stream == null)
                {
                    return;
                }
                FlowDocument flowDocument = (FlowDocument)XamlReader.Load(stream);
                helpViewer.Document = flowDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SVG-WPF Converter", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
