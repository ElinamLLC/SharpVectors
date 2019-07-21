Imports System
Imports System.IO
Imports System.Globalization
Imports Microsoft.Win32
Imports System.Windows.Markup
Imports System.Windows.Threading
Imports SharpVectors.Runtime
Imports SharpVectors.Converters
Imports SharpVectors.Renderers.Wpf

Partial Public Class ScrollableZoomPanPage
    Inherits Page

    Private Const ZoomChange As Double = 0.1
    Private _isLoadingDrawing As Boolean
    Private _svgFilePath As String
    Private _fileReader As FileSvgReader
    Private _wpfSettings As WpfDrawingSettings
    Private _mouseHandlingMode As ZoomPanMouseHandlingMode
    Private _origZoomAndPanControlMouseDownPoint As Point
    Private _origContentMouseDownPoint As Point
    Private _mouseButtonDown As MouseButton
    Private _prevZoomRect As Rect
    Private _prevZoomScale As Double
    Private _prevZoomRectSet As Boolean
    Private _nextZoomRect As Rect
    Private _nextZoomScale As Double
    Private _nextZoomRectSet As Boolean
    Private _mainWindow As MainWindow
    Private _dispatcherTimer As DispatcherTimer
    Private _wndOverview As ZoomPanOverviewWindow

    Public Sub New()
        InitializeComponent()

        _wpfSettings = New WpfDrawingSettings()
        _wpfSettings.CultureInfo = _wpfSettings.NeutralCultureInfo
        _fileReader = New FileSvgReader(_wpfSettings)
        _fileReader.SaveXaml = False
        _fileReader.SaveZaml = False

        _mouseHandlingMode = ZoomPanMouseHandlingMode.None

        AddHandler Me.Loaded, AddressOf OnPageLoaded
        AddHandler Me.Unloaded, AddressOf OnPageUnloaded
        AddHandler Me.SizeChanged, AddressOf OnPageSizeChanged
    End Sub

    Public ReadOnly Property ZoomPanContent As ZoomPanControl
        Get
            Return zoomPanControl
        End Get
    End Property

    Public ReadOnly Property Viewer As SvgDrawingCanvas
        Get
            Return svgViewer
        End Get
    End Property

    Public Property MainWindow As MainWindow
        Get
            Return _mainWindow
        End Get
        Set(ByVal value As MainWindow)
            _mainWindow = value
        End Set
    End Property

    Public Sub SaveZoom()
        If zoomPanControl IsNot Nothing Then
            SavePrevZoomRect()
            ClearNextZoomRect()
        End If
    End Sub

    Private Sub OnPageLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If String.IsNullOrWhiteSpace(_svgFilePath) OrElse Not File.Exists(_svgFilePath) Then
            zoomPanControl.ContentScale = 1.0

            If zoomPanControl IsNot Nothing Then
                zoomPanControl.IsMouseWheelScrollingEnabled = True
            End If

            If String.IsNullOrWhiteSpace(_svgFilePath) Then
                Me.UnloadDocument()
            End If
        End If

        Try

            If _dispatcherTimer Is Nothing Then
                _dispatcherTimer = New DispatcherTimer()
                AddHandler _dispatcherTimer.Tick, AddressOf OnUpdateUITick
                _dispatcherTimer.Interval = New TimeSpan(0, 0, 1)
            End If

        Catch ex As Exception
            Trace.TraceError(ex.ToString())
        End Try

        If zoomPanControl IsNot Nothing AndAlso zoomPanControl.ScrollOwner Is Nothing Then

            If canvasScroller IsNot Nothing Then
                zoomPanControl.ScrollOwner = canvasScroller
            End If
        End If

        If _dispatcherTimer IsNot Nothing Then
            _dispatcherTimer.Start()
        End If

        If _mainWindow IsNot Nothing Then
            _wndOverview = New ZoomPanOverviewWindow()
            _wndOverview.Title = "Overview - Scrollable"
            _wndOverview.PageMode = ZoomPanPageMode.Scrollable
            _wndOverview.Owner = _mainWindow
            _wndOverview.DataContext = Me
            _wndOverview.Left = _mainWindow.Left + _mainWindow.ActualWidth
            _wndOverview.Top = _mainWindow.Top + _mainWindow.ActualHeight - _wndOverview.Height
            _wndOverview.Show()
        End If
    End Sub

    Private Sub OnPageSizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
        If zoomPanControl IsNot Nothing AndAlso svgViewer IsNot Nothing Then
            svgViewer.InvalidateMeasure()
            svgViewer.UpdateLayout()
            Dim bounds As Rect = svgViewer.Bounds

            If bounds.IsEmpty Then
                bounds = New Rect(0, 0, svgViewer.ActualWidth, svgViewer.ActualHeight)
            End If

            zoomPanControl.AnimatedZoomTo(Me.FitZoomValue)
            CommandManager.InvalidateRequerySuggested()
        End If
    End Sub

    Private Sub OnPageUnloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If _dispatcherTimer IsNot Nothing Then
            _dispatcherTimer.[Stop]()
        End If

        If _wndOverview IsNot Nothing Then
            _wndOverview.Close()
            _wndOverview = Nothing
        End If
    End Sub

    Private Async Sub OnOpenFileClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim dlg As OpenFileDialog = New OpenFileDialog()
        dlg.Multiselect = False
        dlg.Title = "Select An SVG File"
        dlg.DefaultExt = "*.svg"
        dlg.Filter = "All SVG Files (*.svg,*.svgz)|*.svg;*.svgz" &
            "|Svg Uncompressed Files (*.svg)|*.svg" & "|SVG Compressed Files (*.svgz)|*.svgz"
        Dim isSelected As Boolean? = dlg.ShowDialog()

        If isSelected IsNot Nothing AndAlso isSelected.Value Then
            Await Me.LoadDocumentAsync(dlg.FileName)
        End If
    End Sub

    Private Sub OnUpdateUITick(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
    End Sub

    Private Sub OnZoomPanMouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        zoomPanControl.Focus()
        Keyboard.Focus(zoomPanControl)
        _mouseButtonDown = e.ChangedButton
        _origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomPanControl)
        _origContentMouseDownPoint = e.GetPosition(svgViewer)

        If (Keyboard.Modifiers And ModifierKeys.Shift) <> 0 _
            AndAlso (e.ChangedButton = MouseButton.Left OrElse e.ChangedButton = MouseButton.Right) Then
            _mouseHandlingMode = ZoomPanMouseHandlingMode.Zooming
        ElseIf _mouseButtonDown = MouseButton.Left Then
            _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning
        End If

        If _mouseHandlingMode <> ZoomPanMouseHandlingMode.None Then
            zoomPanControl.CaptureMouse()
            e.Handled = True
        End If
    End Sub

    Private Sub OnZoomPanMouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        If _mouseHandlingMode <> ZoomPanMouseHandlingMode.None Then

            If _mouseHandlingMode = ZoomPanMouseHandlingMode.Zooming Then

                If _mouseButtonDown = MouseButton.Left Then
                    ZoomIn(_origContentMouseDownPoint)
                ElseIf _mouseButtonDown = MouseButton.Right Then
                    ZoomOut(_origContentMouseDownPoint)
                End If
            ElseIf _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming Then
                ApplyDragZoomRect()
            End If

            zoomPanControl.ReleaseMouseCapture()
            _mouseHandlingMode = ZoomPanMouseHandlingMode.None
            e.Handled = True
        End If
    End Sub

    Private Sub OnZoomPanMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
        If _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning Then
            Dim curContentMousePoint As Point = e.GetPosition(svgViewer)
            Dim dragOffset As Vector = curContentMousePoint - _origContentMouseDownPoint
            zoomPanControl.ContentOffsetX -= dragOffset.X
            zoomPanControl.ContentOffsetY -= dragOffset.Y
            e.Handled = True
        ElseIf _mouseHandlingMode = ZoomPanMouseHandlingMode.Zooming Then
            Dim curZoomAndPanControlMousePoint As Point = e.GetPosition(zoomPanControl)
            Dim dragOffset As Vector = curZoomAndPanControlMousePoint - _origZoomAndPanControlMouseDownPoint
            Dim dragThreshold As Double = 10

            If _mouseButtonDown = MouseButton.Left AndAlso (Math.Abs(dragOffset.X) > dragThreshold _
                OrElse Math.Abs(dragOffset.Y) > dragThreshold) Then
                _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming

                Dim curContentMousePoint As Point = e.GetPosition(svgViewer)
                InitDragZoomRect(_origContentMouseDownPoint, curContentMousePoint)
            End If

            e.Handled = True
        ElseIf _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming Then
            Dim curContentMousePoint As Point = e.GetPosition(svgViewer)
            SetDragZoomRect(_origContentMouseDownPoint, curContentMousePoint)
            e.Handled = True
        End If
    End Sub

    Private Sub OnZoomPanMouseWheel(ByVal sender As Object, ByVal e As MouseWheelEventArgs)
        e.Handled = True
        Dim curContentMousePoint As Point = e.GetPosition(svgViewer)
        Me.Zoom(curContentMousePoint, e.Delta)

        If svgViewer.IsKeyboardFocusWithin Then
            Keyboard.Focus(zoomPanControl)
        End If
    End Sub

    Private Sub OnZoomPanMouseDoubleClick(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        If (Keyboard.Modifiers And ModifierKeys.Shift) = 0 Then
            SavePrevZoomRect()
            zoomPanControl.AnimatedSnapTo(e.GetPosition(svgViewer))
            ClearNextZoomRect()
            e.Handled = True
        End If
    End Sub

    Private Sub OnPanMode(ByVal sender As Object, ByVal e As RoutedEventArgs)
    End Sub

    Private Sub OnCanPanMode(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = False
    End Sub

    Private Sub OnZoomReset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SavePrevZoomRect()
        zoomPanControl.AnimatedZoomTo(1.0)
        ClearNextZoomRect()
        CommandManager.InvalidateRequerySuggested()
    End Sub

    Private Sub OnCanZoomReset(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If zoomPanControl Is Nothing Then
            e.CanExecute = False
            Return
        End If

        e.CanExecute = Not zoomPanControl.ContentScale.Equals(1.0)
    End Sub

    Private Sub OnZoomFit(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SavePrevZoomRect()
        zoomPanControl.AnimatedScaleToFit()
        zoomPanControl.AnimatedZoomTo(Me.FitZoomValue)
        ClearNextZoomRect()
        CommandManager.InvalidateRequerySuggested()
    End Sub

    Private Sub OnCanZoomFit(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If zoomPanControl Is Nothing Then
            e.CanExecute = False
            Return
        End If

        Dim scaleX As Double = zoomPanControl.ContentViewportWidth / svgViewer.ActualWidth
        Dim scaleY As Double = zoomPanControl.ContentViewportHeight / svgViewer.ActualHeight
        Dim fitValue As Double = zoomPanControl.ContentScale * Math.Min(scaleX, scaleY)
        e.CanExecute = Not IsWithinOnePercent(zoomPanControl.ContentScale, fitValue) _
            AndAlso fitValue >= zoomPanControl.MinContentScale
    End Sub

    Private Sub OnZoomIn(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SavePrevZoomRect()
        ZoomIn(New Point(zoomPanControl.ContentZoomFocusX, zoomPanControl.ContentZoomFocusY))
        ClearNextZoomRect()
    End Sub

    Private Sub OnCanZoomIn(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If zoomPanControl Is Nothing Then
            e.CanExecute = False
            Return
        End If

        e.CanExecute = zoomPanControl.ContentScale < zoomPanControl.MaxContentScale
    End Sub

    Private Sub OnZoomOut(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SavePrevZoomRect()
        ZoomOut(New Point(zoomPanControl.ContentZoomFocusX, zoomPanControl.ContentZoomFocusY))
        ClearNextZoomRect()
    End Sub

    Private Sub OnCanZoomOut(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If zoomPanControl Is Nothing Then
            e.CanExecute = False
            Return
        End If

        e.CanExecute = zoomPanControl.ContentScale > zoomPanControl.MinContentScale
    End Sub

    Private Sub OnUndoZoom(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        UndoZoom()
    End Sub

    Private Sub OnCanUndoZoom(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = _prevZoomRectSet
    End Sub

    Private Sub OnRedoZoom(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        RedoZoom()
    End Sub

    Private Sub OnCanRedoZoom(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = _nextZoomRectSet
    End Sub

    Private Sub UndoZoom()
        SaveNextZoomRect()
        zoomPanControl.AnimatedZoomTo(_prevZoomScale, _prevZoomRect)
        ClearPrevZoomRect()
    End Sub

    Private Sub RedoZoom()
        SavePrevZoomRect()
        zoomPanControl.AnimatedZoomTo(_nextZoomScale, _nextZoomRect)
        ClearNextZoomRect()
    End Sub

    Private Sub Zoom(ByVal contentZoomCenter As Point, ByVal wheelMouseDelta As Integer)
        SavePrevZoomRect()
        Dim zoomFactor = zoomPanControl.ContentScale + ZoomChange * wheelMouseDelta / (120 * 3)
        zoomPanControl.ZoomAboutPoint(zoomFactor, contentZoomCenter)
        ClearNextZoomRect()
    End Sub

    Private Sub ZoomOut(ByVal contentZoomCenter As Point)
        SavePrevZoomRect()
        zoomPanControl.ZoomAboutPoint(zoomPanControl.ContentScale - ZoomChange, contentZoomCenter)
        ClearNextZoomRect()
    End Sub

    Private Sub ZoomIn(ByVal contentZoomCenter As Point)
        SavePrevZoomRect()
        zoomPanControl.ZoomAboutPoint(zoomPanControl.ContentScale + ZoomChange, contentZoomCenter)
        ClearNextZoomRect()
    End Sub

    Private Sub InitDragZoomRect(ByVal pt1 As Point, ByVal pt2 As Point)
        SetDragZoomRect(pt1, pt2)
        dragZoomCanvas.Visibility = Visibility.Visible
        dragZoomBorder.Opacity = 0.5
    End Sub

    Private Sub SetDragZoomRect(ByVal pt1 As Point, ByVal pt2 As Point)
        Dim x, y, width, height As Double

        If pt2.X < pt1.X Then
            x = pt2.X
            width = pt1.X - pt2.X
        Else
            x = pt1.X
            width = pt2.X - pt1.X
        End If

        If pt2.Y < pt1.Y Then
            y = pt2.Y
            height = pt1.Y - pt2.Y
        Else
            y = pt1.Y
            height = pt2.Y - pt1.Y
        End If

        Canvas.SetLeft(dragZoomBorder, x)
        Canvas.SetTop(dragZoomBorder, y)
        dragZoomBorder.Width = width
        dragZoomBorder.Height = height
    End Sub

    Private Sub ApplyDragZoomRect()
        SavePrevZoomRect()
        Dim contentX As Double = Canvas.GetLeft(dragZoomBorder)
        Dim contentY As Double = Canvas.GetTop(dragZoomBorder)
        Dim contentWidth As Double = dragZoomBorder.Width
        Dim contentHeight As Double = dragZoomBorder.Height
        zoomPanControl.AnimatedZoomTo(New Rect(contentX, contentY, contentWidth, contentHeight))
        FadeOutDragZoomRect()
        ClearNextZoomRect()
    End Sub

    Private Sub FadeOutDragZoomRect()
        ZoomPanAnimationHelper.StartAnimation(dragZoomBorder, OpacityProperty, 0.0, ZoomChange,
                                              Function(ByVal sender As Object, ByVal e As EventArgs) dragZoomCanvas.Visibility = Visibility.Collapsed)
    End Sub

    Private Sub SavePrevZoomRect()
        _prevZoomRect = New Rect(zoomPanControl.ContentOffsetX, zoomPanControl.ContentOffsetY,
                                 zoomPanControl.ContentViewportWidth, zoomPanControl.ContentViewportHeight)
        _prevZoomScale = zoomPanControl.ContentScale
        _prevZoomRectSet = True
    End Sub

    Private Sub SaveNextZoomRect()
        _nextZoomRect = New Rect(zoomPanControl.ContentOffsetX, zoomPanControl.ContentOffsetY,
                                 zoomPanControl.ContentViewportWidth, zoomPanControl.ContentViewportHeight)
        _nextZoomScale = zoomPanControl.ContentScale
        _nextZoomRectSet = True
    End Sub

    Private Sub ClearPrevZoomRect()
        _prevZoomRectSet = False
    End Sub

    Private Sub ClearNextZoomRect()
        _nextZoomRectSet = False
    End Sub

    Public ReadOnly Property FitZoomValue As Double
        Get

            If zoomPanControl Is Nothing Then
                Return 1
            End If

            Dim content = zoomPanControl.ContentElement
            Return FitZoom(ActualWidth, ActualHeight, content?.ActualWidth, content?.ActualHeight)
        End Get
    End Property

    Private Shared Function IsWithinOnePercent(ByVal value As Double, ByVal testValue As Double) As Boolean
        Return Math.Abs(value - testValue) < 0.01 * testValue
    End Function

    Private Shared Function FitZoom(ByVal actualWidth As Double,
                                    ByVal actualHeight As Double, ByVal contentWidth As Double?,
                                    ByVal contentHeight As Double?) As Double
        If Not contentWidth.HasValue OrElse Not contentHeight.HasValue Then Return 1
        Return Math.Min(actualWidth / contentWidth.Value, actualHeight / contentHeight.Value)
    End Function

    Private Function LoadDocumentAsync(ByVal svgFilePath As String) As Task(Of Boolean)
        If _isLoadingDrawing OrElse String.IsNullOrWhiteSpace(svgFilePath) OrElse Not File.Exists(svgFilePath) Then
            Return Task.FromResult(Of Boolean)(False)
        End If

        Dim fileExt As String = Path.GetExtension(svgFilePath)

        If Not (String.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) _
            OrElse String.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase)) Then
            _svgFilePath = Nothing
            Return Task.FromResult(Of Boolean)(False)
        End If

        If _fileReader Is Nothing Then
            _fileReader = New FileSvgReader(_wpfSettings)
            _fileReader.SaveXaml = False
            _fileReader.SaveZaml = False
        End If

        _isLoadingDrawing = True
        Me.UnloadDocument(True)
        _svgFilePath = svgFilePath
        Dim drawingStream As MemoryStream = New MemoryStream()
        Dim context = TaskScheduler.FromCurrentSynchronizationContext()
        Return Task(Of Boolean).Factory.StartNew(
            Function()
                Dim saveXaml = _fileReader.SaveXaml
                Dim drawing As DrawingGroup = _fileReader.Read(svgFilePath)
                _fileReader.SaveXaml = saveXaml

                If drawing IsNot Nothing Then
                    XamlWriter.Save(drawing, drawingStream)
                    drawingStream.Seek(0, SeekOrigin.Begin)
                    Return True
                End If

                _svgFilePath = Nothing
                Return False
            End Function).ContinueWith(
            Function(t)
                Try

                    If Not t.Result Then
                        _isLoadingDrawing = False
                        _svgFilePath = Nothing
                        Return False
                    End If

                    If drawingStream.Length <> 0 Then
                        Dim drawing As DrawingGroup = CType(XamlReader.Load(drawingStream), DrawingGroup)
                        svgViewer.UnloadDiagrams()
                        svgViewer.RenderDiagrams(drawing)
                        svgViewer.InvalidateMeasure()
                        svgViewer.UpdateLayout()
                        Dim bounds As Rect = svgViewer.Bounds

                        If bounds.IsEmpty Then
                            bounds = New Rect(0, 0, svgViewer.ActualWidth, svgViewer.ActualHeight)
                        End If

                        zoomPanControl.AnimatedZoomTo(bounds)
                        CommandManager.InvalidateRequerySuggested()
                    End If

                    _isLoadingDrawing = False
                    Return True
                Catch
                    _isLoadingDrawing = False
                    Throw
                End Try
            End Function, context)
    End Function

    Private Sub UnloadDocument(ByVal Optional displayMessage As Boolean = False)
        _svgFilePath = Nothing

        If svgViewer IsNot Nothing Then
            svgViewer.UnloadDiagrams()

            If displayMessage Then
                Dim drawing = Me.DrawText("Loading...")
                svgViewer.RenderDiagrams(drawing)
                Dim bounds As Rect = svgViewer.Bounds

                If bounds.IsEmpty Then
                    bounds = drawing.Bounds
                End If

                zoomPanControl.ZoomTo(bounds)
                Return
            End If
        End If

        Dim drawRect = Me.DrawRect()
        svgViewer.RenderDiagrams(drawRect)
        zoomPanControl.ZoomTo(drawRect.Bounds)
        ClearPrevZoomRect()
        ClearNextZoomRect()
    End Sub

    Private Function SaveDocument(ByVal fileName As String) As Boolean
        If String.IsNullOrWhiteSpace(fileName) Then
            Return False
        End If

        If _fileReader Is Nothing OrElse _fileReader.Drawing Is Nothing Then
            Return False
        End If

        Return _fileReader.Save(fileName, True, False)
    End Function

    Private Function DrawRect() As DrawingGroup
        Dim drawingGroup As DrawingGroup = New DrawingGroup()

        Using drawingContext As DrawingContext = drawingGroup.Open()
            drawingContext.DrawRectangle(Brushes.White, Nothing, New Rect(0, 0, 280, 300))
        End Using

        Return drawingGroup
    End Function

    Private Function DrawText(ByVal textString As String) As DrawingGroup
        Dim drawingGroup As DrawingGroup = New DrawingGroup()
        drawingGroup.Opacity = 0.8

        Using drawingContext As DrawingContext = drawingGroup.Open()
            Dim formattedText = New FormattedText(textString, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                                  New Typeface(New FontFamily("Tahoma"), FontStyles.Normal,
                                                               FontWeights.Normal, FontStretches.Normal), 72, Brushes.Black)
            Dim textGeometry As Geometry = formattedText.BuildGeometry(New Point(20, 0))
            drawingContext.DrawRoundedRectangle(Brushes.Transparent, Nothing,
                                                New Rect(New Size(formattedText.Width + 50, formattedText.Height + 5)), 5.0, 5.0)
            drawingContext.DrawGeometry(Nothing, New Pen(Brushes.DarkGray, 1.5), textGeometry)
        End Using

        Return drawingGroup
    End Function
End Class
