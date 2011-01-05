using System;
using System.IO;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace FileSvgReaderSample
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
            FileSvgReader converter = new FileSvgReader(settings);
            // 4. Read the SVG file
            DrawingGroup drawing = converter.Read(svgTestFile);

            if (drawing != null)
            {
                svgImage.Source = new DrawingImage(drawing);

                using (StringWriter textWriter = new StringWriter())
                {
                    if (converter.Save(textWriter))
                    {
                        svgBox.Text = textWriter.ToString();
                    }
                }

            }
        }
    }
}
