Imports System.IO

Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Module MainModule

    Sub Main()
        ' 1. Create conversion options
        Dim settings As WpfDrawingSettings = New WpfDrawingSettings()
        settings.IncludeRuntime = False
        settings.TextAsGeometry = True

        ' 2. Specify the source and destination directories
        Dim svgDir As DirectoryInfo = New DirectoryInfo(Path.GetFullPath("Samples"))
        Dim xamlDir As DirectoryInfo = New DirectoryInfo(Path.GetFullPath("SamplesXaml"))

        ' 3. Create a file converter                       
        Dim converter As DirectorySvgConverter = New DirectorySvgConverter(settings)
        ' 4. Perform the conversion to XAML
        converter.Convert(svgDir, xamlDir)
    End Sub

End Module
