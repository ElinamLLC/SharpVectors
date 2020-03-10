Imports System
Imports System.IO
Imports System.Net
Imports System.Windows.Controls.Primitives

Class MainWindow
    Public Sub New()
        InitializeComponent()

        ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

        AddHandler Me.Loaded, AddressOf OnWindowLoaded
    End Sub

    Private Sub OnWindowLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 6. Load Web SVG file (Stream)
        Try
            Dim webUrl As String = "https://upload.wikimedia.org/wikipedia/commons/d/dd/PoliceCar.svg"
            Dim requestInfo As HttpWebRequest = CType(WebRequest.Create(webUrl), HttpWebRequest)

            If requestInfo IsNot Nothing Then

                Using response As HttpWebResponse = CType(requestInfo.GetResponse(), HttpWebResponse)
                    ' Disposing the response will close the response stream.
                    Dim webStream = response.GetResponseStream()
                    ' The control will copy the stream to memory to avoid disposing issues
                    webSvgCanvas.StreamSource = webStream
                End Using
            End If

        Catch ex As Exception
            ReportError(ex)
            Return
        End Try

        ' 7. Load Resource SVG file (Stream)
        Try
            Dim uri As Uri = New Uri("/Resources/Test.svg", UriKind.Relative)
            Dim resourceInfo = Application.GetResourceStream(uri)

            If resourceInfo IsNot Nothing Then

                Using resourceStream = resourceInfo.Stream
                    resourceSvgCanvas.StreamSource = resourceStream
                End Using
            End If

        Catch ex As Exception
            ReportError(ex)
            Return
        End Try

        ' 8/9. Load/LoadAsync Methods
        Try
            ' Trigger the checked events...
            rbResource.RaiseEvent(New RoutedEventArgs(ToggleButton.CheckedEvent))
            rbResourceAsync.RaiseEvent(New RoutedEventArgs(ToggleButton.CheckedEvent))
        Catch ex As Exception
            ReportError(ex)
        End Try
    End Sub

    Private Sub OnLoadMethodChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Content-rendering may also fire this event
        If Not Me.IsLoaded Then
            Return
        End If

        Dim useAsync As Boolean = (chkUseAsync.IsChecked IsNot Nothing AndAlso chkUseAsync.IsChecked.Value)
        Dim useCopyStream As Boolean = True ' default option

        If rbUri.IsChecked IsNot Nothing AndAlso rbUri.IsChecked.Value Then
            chkUseCopyStream.IsEnabled = False
        Else
            ' For stream, enable the "Use Copy Stream" option
            chkUseCopyStream.IsEnabled = True
            useCopyStream = (chkUseCopyStream.IsChecked IsNot Nothing AndAlso chkUseCopyStream.IsChecked.Value)
        End If

        loadSvgCanvas.Unload(True) ' Will display message "Loading..."
        'loadSvgCanvas.Unload(True, "Loading, please wait..."); // Will display message "Loading, please wait"

        ' 8. Load using methods
        Try

            If rbResource.IsChecked IsNot Nothing AndAlso rbResource.IsChecked.Value Then
                Dim resourceUri As Uri = New Uri("pack://application:,,,/Resources/Test.svg")

                If rbUri.IsChecked IsNot Nothing AndAlso rbUri.IsChecked.Value Then
                    loadSvgCanvas.Load(resourceUri, useAsync)
                Else
                    Dim resourceInfo = Application.GetResourceStream(resourceUri)

                    If resourceInfo Is Nothing Then
                        ReportError("Error: The specified resource file cannot be loaded." _
                            & Environment.NewLine & resourceUri.ToString())
                        Return
                    End If

                    Using resourceStream = resourceInfo.Stream

                        If useCopyStream Then ' if true, let the control copy the stream...
                            loadSvgCanvas.Load(resourceStream, useCopyStream, useAsync)
                        Else ' otherwise, we send a cloned content of the stream...
                            Dim contentStream As MemoryStream = New MemoryStream()
                            ' Read the bytes in responseStream and copy them to content.
                            resourceStream.CopyTo(contentStream)

                            ' Not required, the control is handle this...
                            contentStream.Seek(0, SeekOrigin.Begin)

                            loadSvgCanvas.Load(contentStream, useCopyStream, useAsync)
                        End If
                    End Using
                End If
            Else
                Dim webPath As String = "https://upload.wikimedia.org/wikipedia/commons/4/4e/Valued_image_stock.svg"

                If rbUri.IsChecked IsNot Nothing AndAlso rbUri.IsChecked.Value Then
                    Dim webUri As Uri = New Uri(webPath)
                    loadSvgCanvas.Load(webUri, useAsync)
                Else
                    Dim requestWeb As HttpWebRequest = CType(WebRequest.Create(webPath), HttpWebRequest)

                    If requestWeb Is Nothing Then
                        ReportError("Error: The specified web file cannot be loaded." & Environment.NewLine & webPath)
                        Return
                    End If

                    Using responseWeb As HttpWebResponse = CType(requestWeb.GetResponse(), HttpWebResponse)
                        ' Get the data stream that is associated with the specified url.
                        Dim responseStream = responseWeb.GetResponseStream()

                        If useCopyStream Then ' if true, let the control copy/clone the stream...
                            loadSvgCanvas.Load(responseStream, useCopyStream, useAsync)
                        Else ' otherwise, we send a cloned content of the stream...
                            Dim contentStream As MemoryStream = New MemoryStream()
                            ' Read the bytes in responseStream and copy them to content.
                            responseStream.CopyTo(contentStream)

                            ' Not required, the control is handle this...
                            contentStream.Seek(0, SeekOrigin.Begin)

                            loadSvgCanvas.Load(contentStream, useCopyStream, useAsync)
                        End If
                    End Using
                End If
            End If

        Catch ex As Exception
            ReportError(ex)
        End Try
    End Sub

    ' This requires .NET 4.5 or above
    Private Async Sub OnLoadAsyncMethodChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Content-rendering may also fire this event
        If Not Me.IsLoaded Then
            Return
        End If

        Dim useCopyStream As Boolean = True ' default option

        If rbUriAsync.IsChecked IsNot Nothing AndAlso rbUriAsync.IsChecked.Value Then
            chkUseCopyStreamAsync.IsEnabled = False
        Else
            ' For stream, enable the "Use Copy Stream" option
            chkUseCopyStreamAsync.IsEnabled = True
            useCopyStream = (chkUseCopyStreamAsync.IsChecked IsNot Nothing AndAlso chkUseCopyStreamAsync.IsChecked.Value)
        End If

        Dim messageText = "Loading Web File..."

        If rbResourceAsync.IsChecked IsNot Nothing AndAlso rbResourceAsync.IsChecked.Value Then
            messageText = "Loading Resource File..."
        End If

        'loadSvgCanvasAsync.Unload(True); // Will display message "Loading..."
        loadSvgCanvasAsync.Unload(True, messageText) ' Will display customized message

        ' 9. LoadAsync using methods
        Try

            If rbResourceAsync.IsChecked IsNot Nothing AndAlso rbResourceAsync.IsChecked.Value Then
                Dim resourceUri As Uri = New Uri("pack://application:,,,/Resources/Test.svg")

                If rbUriAsync.IsChecked IsNot Nothing AndAlso rbUriAsync.IsChecked.Value Then
                    Await loadSvgCanvasAsync.LoadAsync(resourceUri)
                Else
                    Dim resourceInfo = Application.GetResourceStream(resourceUri)

                    If resourceInfo Is Nothing Then
                        ReportError("Error: The specified resource file cannot be loaded." _
                            & Environment.NewLine & resourceUri.ToString())
                        Return
                    End If

                    Using resourceStream = resourceInfo.Stream

                        If useCopyStream Then ' if true, let the control copy/clone the stream...
                            Await loadSvgCanvasAsync.LoadAsync(resourceStream)
                        Else ' otherwise, we send a cloned content of the stream...
                            Dim contentStream As MemoryStream = New MemoryStream()
                            ' Read the bytes in responseStream and copy them to content.
                            resourceStream.CopyTo(contentStream)

                            ' Not required, the control is handle this...
                            contentStream.Seek(0, SeekOrigin.Begin)

                            Await loadSvgCanvasAsync.LoadAsync(contentStream)
                        End If
                    End Using
                End If
            Else
                Dim webPath As String = "https://upload.wikimedia.org/wikipedia/commons/4/4e/Valued_image_stock.svg"

                If rbUriAsync.IsChecked IsNot Nothing AndAlso rbUriAsync.IsChecked.Value Then
                    Dim webUri As Uri = New Uri(webPath)
                    Await loadSvgCanvasAsync.LoadAsync(webUri)
                Else
                    Dim requestWeb As HttpWebRequest = CType(WebRequest.Create(webPath), HttpWebRequest)

                    If requestWeb Is Nothing Then
                        ReportError("Error: The specified web file cannot be loaded." & Environment.NewLine & webPath)
                        Return
                    End If

                    Using responseWeb As HttpWebResponse = CType(requestWeb.GetResponse(), HttpWebResponse)
                        ' Get the data stream that is associated with the specified url.
                        Dim responseStream = responseWeb.GetResponseStream()

                        If useCopyStream Then ' if true, let the control copy/clone the stream...
                            Await loadSvgCanvasAsync.LoadAsync(responseStream, useCopyStream)
                        Else
                            Dim contentStream As MemoryStream = New MemoryStream()
                            ' Read the bytes in responseStream and copy them to content.
                            responseStream.CopyTo(contentStream)

                            ' Not required, the control is handle this...
                            contentStream.Seek(0, SeekOrigin.Begin)
                            Await loadSvgCanvasAsync.LoadAsync(contentStream, useCopyStream)
                        End If
                    End Using
                End If
            End If

        Catch ex As Exception
            ReportError(ex)
        End Try
    End Sub

    Private Sub ReportError(ByVal ex As Exception)
        If ex Is Nothing Then
            Return
        End If

        Me.ReportError(ex.ToString())
    End Sub

    Private Sub ReportError(ByVal message As String)
        If String.IsNullOrWhiteSpace(message) Then
            Return
        End If

        MessageBox.Show(message, "SharpVector: SvgCanvas Sample - VB", MessageBoxButton.OK, MessageBoxImage.Error)
    End Sub

End Class
