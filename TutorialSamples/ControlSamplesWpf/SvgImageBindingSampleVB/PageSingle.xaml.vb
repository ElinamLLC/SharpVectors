Imports System.IO
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls

Class PageSingle

    Private Const SvgFileName As String = "Asian_Openbill.svg"
    Public Shared ReadOnly LocalFileNameProperty As DependencyProperty = DependencyProperty.Register("LocalFileName", GetType(String), GetType(PageSingle), New PropertyMetadata(SvgFileName))

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded, AddressOf OnPageLoaded
        AddHandler Me.SizeChanged, AddressOf OnPageSizeChanged
    End Sub

    Public Property LocalFileName As String
        Get
            Return CStr(GetValue(LocalFileNameProperty))
        End Get
        Set(ByVal value As String)
            SetValue(LocalFileNameProperty, value)
        End Set
    End Property

    Private Sub OnPageLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim workingDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim svgFilePath As String = Path.Combine(workingDir, SvgFileName)

        If File.Exists(svgFilePath) Then
            InputBox.Text = svgFilePath
        End If

        Dim rowTab As RowDefinition = rightGrid.RowDefinitions(2)
        rowTab.Height = New GridLength((Me.ActualHeight - 8) / 2, GridUnitType.Pixel)
    End Sub

    Private Sub OnPageSizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
        Dim rowTab As RowDefinition = rightGrid.RowDefinitions(2)
        rowTab.Height = New GridLength((Me.ActualHeight - 8) / 2, GridUnitType.Pixel)
    End Sub

End Class
