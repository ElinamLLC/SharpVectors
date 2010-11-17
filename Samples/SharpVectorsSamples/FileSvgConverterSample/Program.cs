using System;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace FileSvgConverterSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Create conversion options
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = false;
            settings.TextAsGeometry = true;

            // 2. Select a file to be converted
            string svgTestFile = "Test.svg";

            // 3. Create a file converter
            FileSvgConverter converter = new FileSvgConverter(settings);
            // 4. Perform the conversion to XAML
            converter.Convert(svgTestFile);
        }
    }
}
