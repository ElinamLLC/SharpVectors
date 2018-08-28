Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Class MainWindow

    Private Sub OnWindowLoaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        ' 1. Create conversion options
        Dim settings As WpfDrawingSettings = New WpfDrawingSettings()
        settings.IncludeRuntime = True
        settings.TextAsGeometry = False

        ' 2. Select a file to be converted
        Dim svgTestFile As String = "Test.svg"

        ' 3. Create a file converter
        Dim converter As StreamSvgConverter = New StreamSvgConverter(settings)
        ' 4. Convert the SVG file
        Dim memStream As MemoryStream = New MemoryStream()

        If converter.Convert(svgTestFile, memStream) Then
            Dim bitmap As New BitmapImage()
            bitmap.BeginInit()
            bitmap.CacheOption = BitmapCacheOption.OnLoad
            bitmap.StreamSource = memStream
            bitmap.EndInit()
            ' Set the image source.
            svgImage.Source = bitmap
        End If

        memStream.Close()

    End Sub

End Class
