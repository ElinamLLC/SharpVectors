Imports System

Partial Public Class ZoomPanOverviewWindow
    Inherits Window

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded, AddressOf OnWindowLoaded
    End Sub

    Public Property PageMode As ZoomPanPageMode
        Get
            If zoomOverview IsNot Nothing Then
                Return zoomOverview.PageMode
            End If

            Return ZoomPanPageMode.None
        End Get
        Set(ByVal value As ZoomPanPageMode)
            If zoomOverview IsNot Nothing Then
                zoomOverview.PageMode = value
            End If
        End Set
    End Property

    Private Sub OnWindowLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' This assumes the target page is stored in the DataContext, 
        ' before the page is displayed...
        If Me.DataContext Is Nothing OrElse zoomOverview Is Nothing Then
            Return
        End If

        ' Pass the required information to the overview control through binding...
        ' 1. The target page, so that the ZoomPanControl install can be accessed...
        Dim pageBinding = New Binding()
        pageBinding.Source = Me.DataContext
        zoomOverview.SetBinding(DataContextProperty, pageBinding)

        ' 2. The target page's viewer property, so that the SvgDrawingCanvas instance can be accessed...
        Dim drawingBinding = New Binding("Viewer")
        drawingBinding.Source = Me.DataContext
        zoomOverview.SetBinding(ZoomPanOverview.VisualProperty, drawingBinding)
    End Sub
End Class
