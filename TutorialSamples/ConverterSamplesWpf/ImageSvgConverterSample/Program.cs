using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace ImageSvgConverterSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Create conversion options
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            // 2. Select a file to be converted
            string svgTestFile = "Test.svg";

            // 3. Create a file converter
            ImageSvgConverter converter = new ImageSvgConverter(settings);
            // 4. Perform the conversion to image  
            converter.EncoderType = ImageEncoderType.BmpBitmap;
            converter.Convert(svgTestFile);

            converter.EncoderType = ImageEncoderType.GifBitmap;
            converter.Convert(svgTestFile);

            converter.EncoderType = ImageEncoderType.JpegBitmap;
            converter.Convert(svgTestFile);

            converter.EncoderType = ImageEncoderType.PngBitmap;
            converter.Convert(svgTestFile);

            converter.EncoderType = ImageEncoderType.TiffBitmap;
            converter.Convert(svgTestFile);

            converter.EncoderType = ImageEncoderType.WmpBitmap;
            converter.Convert(svgTestFile);
        }
    }
}
