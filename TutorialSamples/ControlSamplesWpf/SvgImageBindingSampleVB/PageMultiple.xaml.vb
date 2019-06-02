Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation

Class PageMultiple

    Public Const IconZipFile As String = "svg-icons.zip"
    Public Const IconFolder As String = "Svg-Icons"

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded, AddressOf OnPageLoaded
        AddHandler Me.SizeChanged, AddressOf OnPageSizeChanged
    End Sub

    Private Sub OnPageLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim workingDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim iconsPath As String = Path.Combine(workingDir, PageMultiple.IconZipFile)

        If Not File.Exists(iconsPath) Then
            Return
        End If

        Dim iconsDir = New DirectoryInfo(Path.Combine(workingDir, PageMultiple.IconFolder))

        If Not iconsDir.Exists Then
            Return
        End If

        Dim iconFiles As FileInfo() = iconsDir.GetFiles("*.svg", SearchOption.TopDirectoryOnly)

        If iconFiles Is Nothing OrElse iconFiles.Length = 0 Then
            Return
        End If

        Dim sourceData As List(Of IconData) = New List(Of IconData)(iconFiles.Length)

        For Each iconFile In iconFiles
            sourceData.Add(New IconData(iconFile))
        Next

        sourceData.Add(New IconData())
        Me.DataContext = sourceData
    End Sub

    Private Sub OnPageSizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
    End Sub
End Class

Public Class IconData
    Private _uri As Uri
    Private _title As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal source As FileInfo)
        If source IsNot Nothing Then
            _title = Path.GetFileNameWithoutExtension(source.Name)
            _uri = New Uri(source.FullName)
        End If
    End Sub

    Public Property ImageTitle As String
        Get
            Return Me._title
        End Get
        Set(ByVal value As String)
            Me._title = value
        End Set
    End Property

    Public Property ImageUri As Uri
        Get
            Return Me._uri
        End Get
        Set(ByVal value As Uri)
            Me._uri = value
        End Set
    End Property
End Class


