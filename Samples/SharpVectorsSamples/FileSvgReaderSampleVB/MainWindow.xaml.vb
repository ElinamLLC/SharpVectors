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
        Dim converter As FileSvgReader = New FileSvgReader(settings)
        ' 4. Read the SVG file
        Dim drawing As DrawingGroup = converter.Read(svgTestFile)

        If (drawing IsNot Nothing) Then
            svgImage.Source = New DrawingImage(drawing)
        End If
    End Sub

End Class
