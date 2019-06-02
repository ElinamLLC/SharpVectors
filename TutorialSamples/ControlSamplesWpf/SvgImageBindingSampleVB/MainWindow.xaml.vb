Imports System
Imports System.IO
Imports System.Reflection
Imports System.IO.Compression

Class MainWindow
    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded, AddressOf OnWindowLoaded
    End Sub

    Private Sub OnWindowLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)

        ' ICons credit: https://github.com/icons8/flat-color-icons
        Dim workingDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim iconsPath As String = Path.Combine(workingDir, PageMultiple.IconZipFile)

        If Not File.Exists(iconsPath) Then
            Return
        End If

        Dim iconsDir = New DirectoryInfo(Path.Combine(workingDir, PageMultiple.IconFolder))

        If Not iconsDir.Exists Then
            iconsDir.Create()
            ZipFile.ExtractToDirectory(iconsPath, workingDir)
        End If
    End Sub

End Class
