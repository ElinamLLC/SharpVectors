---
uid: topic_drawing_converter
title: FileSvgReader
---

# Drawing SVG Converter - FileSvgReader
The **[](xref:SharpVectors.Converters.FileSvgReader)** converter class takes SVG file as input and outputs [DrawingGroup](xref:System.Windows.Media.DrawingGroup) object.
The SVG input can also be one of the following
* @System.Uri: A representation of a resource available to your application locally, on the intranet or internet
* @System.IO.Stream: A stream object providing access to the SVG input content.
* @System.IO.TextReader: A text reader file providing access to the SVG input content.
* @System.Xml.XmlReader: An XML reader providing access to the SVG input content.

> [!NOTE] 
> * With the @System.Uri as input, **[](xref:SharpVectors.Converters.FileSvgReader)** provides a wider access to SVG files.
> * It can optionally save the output to XAML file, which is odd as is its naming. This was the first and initial SVG to XAML handler. 
> * User demands gave rise to the multiple SVG converters forcing this object to be considered as a converter for simplicity.
> * This converter will be renamed `DrawingSvgConverter` in SharpVectors 2.x and stripped of some of its shared features (a breaking change).

## Sample Code

# [SVG to XAML C#](#tab/csharp)
```csharp
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
```

# [SVG to XAML VB.NET](#tab/vb)
```vb
Imports System
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Imports SharpVectors.Dom.Svg
Imports SharpVectors.Renderers.Wpf
Imports SharpVectors.Renderers.Utils

Imports DrawingSvgConverter = SharpVectors.Converters.FileSvgReader

Namespace SharpVectors.Test.Sample
    Public Class DrawingSvgConverterSample
        ' The drawing settings or options.
        Private _wpfSettings As WpfDrawingSettings

        Public Sub New()
            ' Initialize the options
            _wpfSettings = New WpfDrawingSettings()
            _wpfSettings.IncludeRuntime = False
            _wpfSettings.TextAsGeometry = False
        End Sub

        Public Function Convert(ByVal svgFile As String) As DrawingGroup
            ' Create the drawing converter
            Dim converter = New SharpVectors.Converters.FileSvgReader(_wpfSettings)

            ' Perform the conversion to DrawingGroup
            Return converter.Read(svgFile)
        End Function

        Public Function Convert(ByVal svgFile As Uri) As DrawingGroup
            ' Create the drawing converter
            Dim converter = New SharpVectors.Converters.FileSvgReader(_wpfSettings)

            ' Perform the conversion to DrawingGroup
            Return converter.Read(svgFile)
        End Function
    End Class
End Namespace
```
