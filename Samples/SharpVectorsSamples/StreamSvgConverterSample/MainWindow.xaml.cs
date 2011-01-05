using System;
using System.IO;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace StreamSvgConverterSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // 1. Create conversion options
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            // 2. Select a file to be converted
            string svgTestFile = "Test.svg";

            // 3. Create a file reader
            StreamSvgConverter converter = new StreamSvgConverter(settings);
            // 4. convert the SVG file
            MemoryStream memStream = new MemoryStream();

            if (converter.Convert(svgTestFile, memStream))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();
                // Set the image source.
                svgImage.Source = bitmap;
            }

            memStream.Close();
        }
    }
}
