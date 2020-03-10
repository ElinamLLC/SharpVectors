using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SvgCanvasSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            this.Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // 6. Load Web SVG file (Stream)
            try
            {
                string webUrl = "https://upload.wikimedia.org/wikipedia/commons/d/dd/PoliceCar.svg";

                HttpWebRequest requestInfo = (HttpWebRequest)WebRequest.Create(webUrl);
                if (requestInfo != null)
                {
                    using (HttpWebResponse response = (HttpWebResponse)requestInfo.GetResponse())
                    {
                        // Disposing the response will close the response stream.
                        var webStream = response.GetResponseStream();
                        // The control will copy the stream to memory to avoid disposing issues
                        webSvgCanvas.StreamSource = webStream;
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
                return;
            }

            // 7. Load Resource SVG file (Stream)
            try
            {
                Uri uri = new Uri("/Resources/Test.svg", UriKind.Relative);
                var resourceInfo = Application.GetResourceStream(uri);

                if (resourceInfo != null)
                {
                    using (var resourceStream = resourceInfo.Stream)
                    {
                        resourceSvgCanvas.StreamSource = resourceStream;
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
                return;
            }

            // 8/9. Load/LoadAsync Methods
            try
            {
                // Trigger the checked events...
                rbResource.RaiseEvent(new RoutedEventArgs(ToggleButton.CheckedEvent));
                rbResourceAsync.RaiseEvent(new RoutedEventArgs(ToggleButton.CheckedEvent));
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void OnLoadMethodChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) // Content-rendering may also fire this event
            {
                return;
            }

            bool useAsync      = (chkUseAsync.IsChecked != null && chkUseAsync.IsChecked.Value);
            bool useCopyStream = true; // default option
            if (rbUri.IsChecked != null && rbUri.IsChecked.Value)
            {
                chkUseCopyStream.IsEnabled = false;
            }
            else
            {
                // For stream, enable the "Use Copy Stream" option
                chkUseCopyStream.IsEnabled = true;
                useCopyStream = (chkUseCopyStream.IsChecked != null && chkUseCopyStream.IsChecked.Value);
            }

            loadSvgCanvas.Unload(true); // Will display message "Loading..."
            //loadSvgCanvas.Unload(true, "Loading, please wait..."); // Will display message "Loading, please wait"

            // 8. Load using methods
            try
            {
                if (rbResource.IsChecked != null && rbResource.IsChecked.Value)
                {
                    Uri resourceUri = new Uri("pack://application:,,,/Resources/Test.svg");

                    if (rbUri.IsChecked != null && rbUri.IsChecked.Value)
                    {
                        loadSvgCanvas.Load(resourceUri, useAsync);
                    }
                    else
                    {
                        var resourceInfo = Application.GetResourceStream(resourceUri);
                        if (resourceInfo == null)
                        {
                            ReportError("Error: The specified resource file cannot be loaded."
                                + Environment.NewLine + resourceUri);
                            return;
                        }

                        using (var resourceStream = resourceInfo.Stream)
                        {
                            if (useCopyStream) // if true, let the control copy the stream...
                            {
                                loadSvgCanvas.Load(resourceStream, useCopyStream, useAsync);
                            }
                            else // otherwise, we send a cloned content of the stream...
                            {
                                MemoryStream contentStream = new MemoryStream();
                                // Read the bytes in responseStream and copy them to content.
                                resourceStream.CopyTo(contentStream);

                                // Not required, the control is handle this...
                                contentStream.Seek(0, SeekOrigin.Begin); 

                                loadSvgCanvas.Load(contentStream, useCopyStream, useAsync);
                            }
                        }
                    }
                }
                else
                {
                    string webPath = "https://upload.wikimedia.org/wikipedia/commons/4/4e/Valued_image_stock.svg";

                    if (rbUri.IsChecked != null && rbUri.IsChecked.Value)
                    {
                        Uri webUri = new Uri(webPath);
                        loadSvgCanvas.Load(webUri, useAsync);
                    }
                    else
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(webPath);
                        if (webRequest == null)
                        {
                            ReportError("Error: The specified web file cannot be loaded."
                                + Environment.NewLine + webPath);
                            return;
                        }

                        using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            // Get the data stream that is associated with the specified url.
                            var responseStream = webResponse.GetResponseStream();

                            if (useCopyStream) // if true, let the control copy/clone the stream...
                            {
                                loadSvgCanvas.Load(responseStream, useCopyStream, useAsync);
                            }
                            else // otherwise, we send a cloned content of the stream...
                            {
                                MemoryStream contentStream = new MemoryStream();
                                // Read the bytes in responseStream and copy them to content.
                                responseStream.CopyTo(contentStream);

                                // Not required, the control is handle this...
                                contentStream.Seek(0, SeekOrigin.Begin);

                                loadSvgCanvas.Load(contentStream, useCopyStream, useAsync);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        // This requires .NET 4.5 or above
        private async void OnLoadAsyncMethodChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) // Content-rendering may also fire this event
            {
                return;
            }

            bool useCopyStream = true; // default option
            if (rbUriAsync.IsChecked != null && rbUriAsync.IsChecked.Value)
            {
                chkUseCopyStreamAsync.IsEnabled = false;
            }
            else
            {
                // For stream, enable the "Use Copy Stream" option
                chkUseCopyStreamAsync.IsEnabled = true;
                useCopyStream = (chkUseCopyStreamAsync.IsChecked != null && chkUseCopyStreamAsync.IsChecked.Value);
            }

            var messageText = "Loading Web File...";
            if (rbResourceAsync.IsChecked != null && rbResourceAsync.IsChecked.Value)
            {
                messageText = "Loading Resource File...";
            }

            //loadSvgCanvasAsync.Unload(true); // Will display message "Loading..."
            loadSvgCanvasAsync.Unload(true, messageText); // Will display customized message

            // 9. LoadAsync using methods
            try
            {
                if (rbResourceAsync.IsChecked != null && rbResourceAsync.IsChecked.Value)
                {
                    Uri resourceUri = new Uri("pack://application:,,,/Resources/Test.svg");

                    if (rbUriAsync.IsChecked != null && rbUriAsync.IsChecked.Value)
                    {
                        await loadSvgCanvasAsync.LoadAsync(resourceUri);
                    }
                    else
                    {
                        var resourceInfo = Application.GetResourceStream(resourceUri);
                        if (resourceInfo == null)
                        {
                            ReportError("Error: The specified resource file cannot be loaded."
                                + Environment.NewLine + resourceUri);
                            return;
                        }

                        using (var resourceStream = resourceInfo.Stream)
                        {
                            if (useCopyStream) // if true, let the control copy/clone the stream...
                            {
                                await loadSvgCanvasAsync.LoadAsync(resourceStream);
                            }
                            else // otherwise, we send a cloned content of the stream...
                            {
                                MemoryStream contentStream = new MemoryStream();
                                // Read the bytes in responseStream and copy them to content.
                                resourceStream.CopyTo(contentStream);

                                // Not required, the control is handle this...
                                contentStream.Seek(0, SeekOrigin.Begin);

                                await loadSvgCanvasAsync.LoadAsync(contentStream);
                            }
                        }

                    }
                }
                else
                {
                    string webPath = "https://upload.wikimedia.org/wikipedia/commons/4/4e/Valued_image_stock.svg";

                    if (rbUriAsync.IsChecked != null && rbUriAsync.IsChecked.Value)
                    {
                        Uri webUri = new Uri(webPath);
                        await loadSvgCanvasAsync.LoadAsync(webUri);
                    }
                    else
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(webPath);
                        if (webRequest == null)
                        {
                            ReportError("Error: The specified web file cannot be loaded."
                                + Environment.NewLine + webPath);
                            return;
                        }

                        using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            // Get the data stream that is associated with the specified url.
                            var responseStream = webResponse.GetResponseStream();

                            if (useCopyStream) // if true, let the control copy/clone the stream...
                            {
                                await loadSvgCanvasAsync.LoadAsync(responseStream, useCopyStream);
                            }
                            else // otherwise, we send a cloned content of the stream...
                            {
                                MemoryStream contentStream = new MemoryStream();
                                // Read the bytes in responseStream and copy them to content.
                                responseStream.CopyTo(contentStream);

                                // Not required, the control is handle this...
                                contentStream.Seek(0, SeekOrigin.Begin);

                                await loadSvgCanvasAsync.LoadAsync(contentStream, useCopyStream);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ReportError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            this.ReportError(ex.ToString());
        }

        private void ReportError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            MessageBox.Show(message, "SharpVector: SvgCanvas Sample - C#", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
