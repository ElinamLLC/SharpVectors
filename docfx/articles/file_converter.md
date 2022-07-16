---
uid: topic_file_converter
title: FileSvgConverter
---

# File SVG Converter - FileSvgConverter
The **[](xref:SharpVectors.Converters.FileSvgConverter)** converter class takes SVG file as input and outputs XAML file.
The SVG input can also be one of the following
* @System.IO.Stream: A stream object providing access to the SVG input content.
* @System.IO.TextReader: A text reader file providing access to the SVG input content.
* @System.Xml.XmlReader: An XML reader providing access to the SVG input content.

> [!NOTE] 
> The output is always to a local file or a network file. It does not provide a backup of existing file.

## Sample Code
The following illustrate a simple case of using the file SVG converter.

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
    public class FileSvgConverterSample
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        public FileSvgConverterSample()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
        }

        public void Convert(string svgFile, string xamlFile)
        {
            // Create a file converter
            var converter = new FileSvgConverter(_wpfSettings);

            // Perform the conversion to XAML
            converter.Convert(svgFile, xamlFile);
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
    Public Class FileSvgConverterSample
        ' The drawing settings or options.
        Private _wpfSettings As WpfDrawingSettings

        Public Sub New()
            ' Initialize the options
            _wpfSettings = New WpfDrawingSettings()
            _wpfSettings.IncludeRuntime = False
            _wpfSettings.TextAsGeometry = False
        End Sub

        Public Sub Convert(ByVal svgFile As String, ByVal xamlFile As String)
            ' Create a file converter
            Dim converter = New FileSvgConverter(_wpfSettings)

            ' Perform the conversion to XAML
            converter.Convert(svgFile, xamlFile)
        End Sub
    End Class
End Namespace
```
