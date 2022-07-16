---
uid: topic_stream_converter
title: StreamSvgConverter
---

# Stream SVG Converter - StreamSvgConverter
The **[](xref:SharpVectors.Converters.StreamSvgConverter)** converter class takes SVG file as input and outputs XAML file.
The SVG input can also be one of the following
* @System.IO.Stream: A stream object providing access to the SVG input content.
* @System.IO.TextReader: A text reader file providing access to the SVG input content.
* @System.Xml.XmlReader: An XML reader providing access to the SVG input content.

> [!NOTE] 
> This strictly converts the input SVG source to @System.IO.Stream, which is created by the user.

## Sample Code
The following illustrate a simple case of using the stream SVG converter.

# [SVG to XAML C#](#tab/csharp)
```csharp
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

            // Perform the conversion to static image
            converter.Convert(svgFile, outputStream);
        }
    }
}
```

# [SVG to XAML VB.NET](#tab/vb)
```vb
Imports System.IO
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Imports SharpVectors.Dom.Svg
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf
Imports SharpVectors.Renderers.Utils

Namespace SharpVectors.Test.Sample
    Public Class StreamSvgConverterSample
        ' The drawing settings or options.
        Private _wpfSettings As WpfDrawingSettings

        Public Sub New()
            ' Initialize the options
            _wpfSettings = New WpfDrawingSettings()
            _wpfSettings.IncludeRuntime = False
            _wpfSettings.TextAsGeometry = False
        End Sub

        Public Sub Convert(ByVal svgFile As String, ByVal outputStream As Stream)
            ' Create an image converter
            Dim converter = New StreamSvgConverter(_wpfSettings)

            ' Perform the conversion XAML and save to the specified stream
            converter.Convert(svgFile, outputStream)
        End Sub
    End Class
End Namespace
```
