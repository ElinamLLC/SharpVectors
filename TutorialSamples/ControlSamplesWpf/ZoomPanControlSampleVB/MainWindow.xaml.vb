Imports System

Partial Public Class MainWindow
    Inherits Window

    Private _scrollablePage As ScrollableZoomPanPage
    Private _infinitePage As InfiniteZoomPanPage

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded, AddressOf OnWindowLoaded
    End Sub

    Private Sub OnWindowLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        helpViewer.Document = TryCast(Application.LoadComponent(New Uri("QuickHelpPage.xaml", UriKind.Relative)), FlowDocument)
        _scrollablePage = TryCast(frameScrollable.Content, ScrollableZoomPanPage)
        _infinitePage = TryCast(frameInfinite.Content, InfiniteZoomPanPage)

        If _scrollablePage IsNot Nothing Then
            _scrollablePage.MainWindow = Me
        End If

        If _infinitePage IsNot Nothing Then
            _infinitePage.MainWindow = Me
        End If
    End Sub
End Class
