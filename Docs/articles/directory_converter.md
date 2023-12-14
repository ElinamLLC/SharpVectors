---
uid: topic_directory_converter
title: DirectorySvgConverter
---

# Directory SVG Converter - DirectorySvgConverter
The **[](xref:SharpVectors.Converters.DirectorySvgConverter)** converter class converts a directory (and optionally the subdirectories) of SVG 
input files to directory of XAML or static image files. It uses
* **[](xref:SharpVectors.Converters.FileSvgReader)**: If the requested output is XAML file, or
* **[](xref:SharpVectors.Converters.ImageSvgConverter)**: If the requested output is static image.

## Sample Code
The following codes illustrate how to recursively convert a directory of SVG to a directory of XAML files (the default), maintaining the order of the
subdirectories.

# [SVG to XAML C#](#tab/csharp)
```csharp
using System;
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
    Public Class DirectorySvgConverterSample
        ' The drawing settings or options.
        Private _wpfSettings As WpfDrawingSettings

        Public Sub New()
            ' Initialize the options
            _wpfSettings = New WpfDrawingSettings()
            _wpfSettings.IncludeRuntime = False
            _wpfSettings.TextAsGeometry = False
        End Sub

        Public Sub Convert(ByVal svgDir As DirectoryInfo, ByVal xamlDir As DirectoryInfo)
            ' Create an directory converter
            Dim converter = New DirectorySvgConverter(_wpfSettings)
            converter.Recursive = True

            ' Converts SVG files in the source directory, save the output to the specified directory
            converter.Convert(svgDir, xamlDir)
        End Sub
    End Class
End Namespace
```
