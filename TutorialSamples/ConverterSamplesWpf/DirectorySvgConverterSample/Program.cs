using System;
using System.IO;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace DirectorySvgConverterSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Create conversion options
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            // 2. Specify the source and destination directories
            DirectoryInfo svgDir = new DirectoryInfo(
                Path.GetFullPath("Samples"));
            DirectoryInfo xamlDir = new DirectoryInfo(
                Path.GetFullPath("SamplesXaml"));

            // 3. Create a directory converter
            DirectorySvgConverter converter = 
                new DirectorySvgConverter(settings);
            // 4. Perform the conversion to XAML
            converter.Convert(svgDir, xamlDir);
        }
    }
}
