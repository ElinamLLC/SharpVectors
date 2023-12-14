---
uid: topic_image_converter
title: ImageSvgConverter
---

# File SVG Converter - ImageSvgConverter
The **[](xref:SharpVectors.Converters.ImageSvgConverter)** converter class takes SVG file as input and outputs XAML file.
The SVG input can also be one of the following
* @System.IO.Stream: A stream object providing access to the SVG input content.
* @System.IO.TextReader: A text reader file providing access to the SVG input content.
* @System.Xml.XmlReader: An XML reader providing access to the SVG input content.

> [!NOTE] 
> The output is always to a local file or a network file. It does not provide a backup of existing file.

## Sample Code
The following illustrate a simple case of using the static image SVG converter.

# [SVG to XAML C#](#tab/csharp)
```csharp
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Test.Sample
{
    public class ImageSvgConverterSample
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        public ImageSvgConverterSample()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
        }

        public void Convert(string svgFile, string pngFile)
        {
            // Create an image converter
            var converter = new ImageSvgConverter(_wpfSettings);

            // Perform the conversion to static image
            converter.Convert(svgFile, pngFile);
        }
    }
}
```

# [SVG to XAML VB.NET](#tab/vb)
```vb
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Imports SharpVectors.Dom.Svg
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf
Imports SharpVectors.Renderers.Utils

Namespace SharpVectors.Test.Sample
    Public Class ImageSvgConverterSample
        ' The drawing settings or options.
        Private _wpfSettings As WpfDrawingSettings

        Public Sub New()
            ' Initialize the options
            _wpfSettings = New WpfDrawingSettings()
            _wpfSettings.IncludeRuntime = False
            _wpfSettings.TextAsGeometry = False
        End Sub

        Public Sub Convert(ByVal svgFile As String, ByVal pngFile As String)
            ' Create an image converter
            Dim converter = New ImageSvgConverter(_wpfSettings)

            ' Perform the conversion to static image
            converter.Convert(svgFile, pngFile)
        End Sub
    End Class
End Namespace
```
