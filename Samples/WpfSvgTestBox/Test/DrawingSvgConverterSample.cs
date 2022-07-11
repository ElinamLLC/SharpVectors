using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

using DrawingSvgConverter = SharpVectors.Converters.FileSvgReader;

namespace SharpVectors.Test.Sample
{
    public class DrawingSvgConverterSample
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        public DrawingSvgConverterSample()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
        }

        public DrawingGroup Convert(string svgFile)
        {
            // Create the drawing converter
            var converter = new DrawingSvgConverter(_wpfSettings);

            // Perform the conversion to DrawingGroup
            return converter.Read(svgFile);
        }

        public DrawingGroup Convert(Uri svgFile)
        {
            // Create the drawing converter
            var converter = new DrawingSvgConverter(_wpfSettings);

            // Perform the conversion to DrawingGroup
            return converter.Read(svgFile);
        }
    }
}
