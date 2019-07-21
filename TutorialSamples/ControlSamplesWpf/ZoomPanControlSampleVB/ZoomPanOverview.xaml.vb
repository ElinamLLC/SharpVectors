Imports System

Imports SharpVectors.Runtime

Partial Public Class ZoomPanOverview
    Inherits UserControl

    Private _pageMode As ZoomPanPageMode
    Private _dragBorder As Border
    Private _sizingBorder As Border
    Private _sizingEvent As Boolean
    Private _viewportCanvas As Canvas
    Private _viewportBox As Viewbox
    Private _mouseHandlingMode As ZoomPanMouseHandlingMode = ZoomPanMouseHandlingMode.None
    Private _origContentMouseDownPoint As Point

    Public Sub New()
        InitializeComponent()
        Me.HorizontalContentAlignment = HorizontalAlignment.Center
        Me.VerticalContentAlignment = VerticalAlignment.Center
    End Sub

    Shared Sub New()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ZoomPanOverview), New FrameworkPropertyMetadata(GetType(ZoomPanOverview)))
    End Sub

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        _dragBorder = TryCast(Me.Template.FindName("PART_DraggingBorder", Me), Border)
        _sizingBorder = TryCast(Me.Template.FindName("PART_SizingBorder", Me), Border)
        _viewportCanvas = TryCast(Me.Template.FindName("PART_Content", Me), Canvas)
        _viewportBox = TryCast(Me.Template.FindName("PART_Viewbox", Me), Viewbox)

        If _viewportBox Is Nothing OrElse _viewportCanvas Is Nothing Then
            MessageBox.Show("The style/template of the ZoomPanOverview control is not defined in the resources.",
                            "ZoomPanOverview - Error", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub

    Public Property PageMode As ZoomPanPageMode
        Get
            Return _pageMode
        End Get
        Set(ByVal value As ZoomPanPageMode)
            _pageMode = value
        End Set
    End Property

    Protected Overrides Sub OnMouseDown(ByVal e As MouseButtonEventArgs)
        MyBase.OnMouseLeftButtonDown(e)

        Select Case _pageMode
            Case ZoomPanPageMode.Scrollable
                Dim scrollablePage = Me.GetScrollablePage()

                If scrollablePage IsNot Nothing Then
                    scrollablePage.SaveZoom()
                End If

            Case ZoomPanPageMode.Infinite
                Dim infinitePage = Me.GetInfinitePage()

                If infinitePage IsNot Nothing Then
                    infinitePage.SaveZoom()
                End If
        End Select

        _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning
        _origContentMouseDownPoint = e.GetPosition(_viewportCanvas)

        If (Keyboard.Modifiers And ModifierKeys.Shift) <> 0 Then
            _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming
            _dragBorder.Visibility = Visibility.Hidden
            _sizingBorder.Visibility = Visibility.Visible
            Canvas.SetLeft(_sizingBorder, _origContentMouseDownPoint.X)
            Canvas.SetTop(_sizingBorder, _origContentMouseDownPoint.Y)
            _sizingBorder.Width = 0
            _sizingBorder.Height = 0
        Else
            _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning
        End If

        If _mouseHandlingMode <> ZoomPanMouseHandlingMode.None Then
            _viewportCanvas.CaptureMouse()
            e.Handled = True
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseButtonEventArgs)
        MyBase.OnMouseLeftButtonUp(e)

        If _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming Then
            Dim zoomAndPanControl = GetZoomPanControl()
            Dim curContentPoint = e.GetPosition(_viewportCanvas)
            Dim rect = GetClip(curContentPoint, _origContentMouseDownPoint, New Point(0, 0),
                               New Point(_viewportCanvas.Width, _viewportCanvas.Height))
            zoomAndPanControl.AnimatedZoomTo(rect)
            _dragBorder.Visibility = Visibility.Visible
            _sizingBorder.Visibility = Visibility.Hidden
        End If

        _mouseHandlingMode = ZoomPanMouseHandlingMode.None
        _viewportCanvas.ReleaseMouseCapture()
        e.Handled = True
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)

        If _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning Then
            Dim curContentPoint = e.GetPosition(_viewportCanvas)
            Dim rectangleDragVector = curContentPoint - _origContentMouseDownPoint
            _origContentMouseDownPoint = Clamp(e.GetPosition(_viewportCanvas))
            Canvas.SetLeft(_dragBorder, Canvas.GetLeft(_dragBorder) + rectangleDragVector.X)
            Canvas.SetTop(_dragBorder, Canvas.GetTop(_dragBorder) + rectangleDragVector.Y)
        ElseIf _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming Then
            Dim curContentPoint = e.GetPosition(_viewportCanvas)
            Dim rect = GetClip(curContentPoint, _origContentMouseDownPoint, New Point(0, 0),
                               New Point(_viewportCanvas.Width, _viewportCanvas.Height))
            PositionBorderOnCanvas(_sizingBorder, rect)
        End If

        e.Handled = True
    End Sub

    Protected Overrides Sub OnMouseDoubleClick(ByVal e As MouseButtonEventArgs)
        MyBase.OnMouseDoubleClick(e)

        If (Keyboard.Modifiers And ModifierKeys.Shift) = 0 Then

            Select Case _pageMode
                Case ZoomPanPageMode.Scrollable
                    Dim scrollablePage = Me.GetScrollablePage()

                    If scrollablePage IsNot Nothing Then
                        scrollablePage.SaveZoom()
                    End If

                Case ZoomPanPageMode.Infinite
                    Dim infinitePage = Me.GetInfinitePage()

                    If infinitePage IsNot Nothing Then
                        infinitePage.SaveZoom()
                    End If
            End Select

            Dim zoomAndPanControl = GetZoomPanControl()
            zoomAndPanControl.AnimatedSnapTo(e.GetPosition(_viewportCanvas))
        End If
    End Sub

    Public Property Visual As FrameworkElement
        Get
            Return CType(GetValue(VisualProperty), FrameworkElement)
        End Get
        Set(ByVal value As FrameworkElement)
            SetValue(VisualProperty, value)
        End Set
    End Property

    Public Shared ReadOnly VisualProperty As DependencyProperty =
        DependencyProperty.Register("Visual", GetType(FrameworkElement), GetType(ZoomPanOverview),
                                    New FrameworkPropertyMetadata(Nothing, AddressOf OnVisualChanged))

    Private Shared Sub OnVisualChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim c = CType(d, ZoomPanOverview)
        c.SetBackground(TryCast(e.NewValue, FrameworkElement))
    End Sub

    Private Sub SetBackground(ByVal frameworkElement As FrameworkElement)
        If _viewportBox Is Nothing OrElse _viewportCanvas Is Nothing Then
            MessageBox.Show("The style/template of the ZoomPanOverview control is not defined in the resources.",
                            "ZoomPanOverview - Error", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Try
            frameworkElement = If(frameworkElement, TryCast((TryCast(DataContext, ContentControl))?.Content, FrameworkElement))

            If frameworkElement Is Nothing Then
                Return
            End If

            Dim visualBrush = New VisualBrush With {
            .Visual = frameworkElement,
            .ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
            .ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
            .AlignmentX = AlignmentX.Center,
            .AlignmentY = AlignmentY.Center,
            .TileMode = TileMode.None,
            .Stretch = Stretch.Uniform
        }

            If Not _sizingEvent Then
                AddHandler frameworkElement.SizeChanged,
                    Sub(s, e)
                        _viewportCanvas.Height = frameworkElement.ActualHeight
                        _viewportCanvas.Width = frameworkElement.ActualWidth
                        _viewportCanvas.Background = visualBrush
                    End Sub

                _viewportCanvas.Height = frameworkElement.ActualHeight
                _viewportCanvas.Width = frameworkElement.ActualWidth
                _viewportCanvas.Background = visualBrush
                _sizingEvent = True
            Else
                _viewportCanvas.Height = frameworkElement.ActualHeight
                _viewportCanvas.Width = frameworkElement.ActualWidth
                _viewportCanvas.Background = visualBrush
            End If

        Catch ex As Exception
            Trace.TraceError(ex.ToString())
        End Try
    End Sub

    Private Function GetScrollablePage() As ScrollableZoomPanPage
        Return TryCast(Me.DataContext, ScrollableZoomPanPage)
    End Function

    Private Function GetInfinitePage() As InfiniteZoomPanPage
        Return TryCast(Me.DataContext, InfiniteZoomPanPage)
    End Function

    Private Function GetZoomPanControl() As ZoomPanControl
        Dim zoomAndPanControl As ZoomPanControl = Nothing

        Select Case _pageMode
            Case ZoomPanPageMode.Scrollable
                zoomAndPanControl = (TryCast(Me.DataContext, ScrollableZoomPanPage))?.ZoomPanContent
            Case ZoomPanPageMode.Infinite
                zoomAndPanControl = (TryCast(Me.DataContext, InfiniteZoomPanPage))?.ZoomPanContent
        End Select

        If zoomAndPanControl Is Nothing Then Throw New NullReferenceException("DataContext is not of type ZoomPanControl")
        Return zoomAndPanControl
    End Function

    Private Shared Sub PositionBorderOnCanvas(ByVal border As Border, ByVal rect As Rect)
        Canvas.SetLeft(border, rect.Left)
        Canvas.SetTop(border, rect.Top)
        border.Width = rect.Width
        border.Height = rect.Height
    End Sub

    Private Shared Function Clamp(ByVal value As Point) As Point
        Return New Point(Math.Max(value.X, 0), Math.Max(value.Y, 0))
    End Function

    Private Shared Function Clamp(ByVal value As Point, ByVal topLeft As Point, ByVal bottomRight As Point) As Point
        Return New Point(Math.Max(Math.Min(value.X, bottomRight.X), topLeft.X),
                         Math.Max(Math.Min(value.Y, bottomRight.Y), topLeft.Y))
    End Function

    Private Shared Function GetClip(ByVal value1 As Point, ByVal value2 As Point,
                                    ByVal topLeft As Point, ByVal bottomRight As Point) As Rect
        Dim point1 = Clamp(value1, topLeft, bottomRight)
        Dim point2 = Clamp(value2, topLeft, bottomRight)
        Dim newTopLeft = New Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y))
        Dim size = New Size(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y))
        Return New Rect(newTopLeft, size)
    End Function
End Class
