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
    public class DirectorySvgConverterSample
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        public DirectorySvgConverterSample()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
        }

        public void Convert(DirectoryInfo svgDir, DirectoryInfo xamlDir)
        {
            // Create an directory converter
            var converter = new DirectorySvgConverter(_wpfSettings);
            converter.Recursive = true;

            // Converts SVG files in the source directory, save the output to the specified directory
            converter.Convert(svgDir, xamlDir);
        }
    }
}
