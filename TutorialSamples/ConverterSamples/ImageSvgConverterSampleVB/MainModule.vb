Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Module MainModule

    Sub Main()
        ' 1. Create conversion options
        Dim settings As WpfDrawingSettings = New WpfDrawingSettings()
        settings.IncludeRuntime = False
        settings.TextAsGeometry = True

        ' 2. Select a file to be converted
        Dim svgTestFile As String = "Test.svg"

        ' 3. Create a file converter
        Dim converter As ImageSvgConverter = New ImageSvgConverter(settings)
        ' 4. Perform the conversion to image
        converter.EncoderType = ImageEncoderType.BmpBitmap
        converter.Convert(svgTestFile)

        converter.EncoderType = ImageEncoderType.GifBitmap
        converter.Convert(svgTestFile)

        converter.EncoderType = ImageEncoderType.JpegBitmap
        converter.Convert(svgTestFile)

        converter.EncoderType = ImageEncoderType.PngBitmap
        converter.Convert(svgTestFile)

        converter.EncoderType = ImageEncoderType.TiffBitmap
        converter.Convert(svgTestFile)

        converter.EncoderType = ImageEncoderType.WmpBitmap
        converter.Convert(svgTestFile)
    End Sub

End Module
