using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Test.Sample
{
    public class StreamSvgConverterSample
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        public StreamSvgConverterSample()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
        }

        public void Convert(string svgFile, Stream outputStream)
        {
            // Create an image converter
            var converter = new StreamSvgConverter(_wpfSettings);

            // Perform the conversion XAML and save to the specified stream
            converter.Convert(svgFile, outputStream);
        }
    }
}
