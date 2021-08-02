using System;
using System.IO;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Runtime;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

using WebViews.Core;
using WebViews.Core.EventArguments;

namespace WpfTestOtherSvg
{
    /// <summary>
    /// Interaction logic for TestsPage.xaml
    /// </summary>
    public partial class TestsPage : Page
    {
        #region Public Fields

//        public const string TemporalDirName = "_Drawings";

        public const int ImageWidth  = 350;
        public const int ImageHeight = 350;

        #endregion

        #region Private Fields

        private string _svgFilePath;
        private string _pngFilePath;

        private DrawingGroup _drawing;
        private MainWindow _mainWindow;

        #endregion

        #region Constructors and Destructor

        public TestsPage()
        {
            InitializeComponent();

            this.Loaded      += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Properties

        public MainWindow MainWindow
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        #endregion

        #region Public Methods

        public void PageSelected(bool isSelected)
        {
        }

        public bool LoadXaml()
        {
            if (_mainWindow == null || string.IsNullOrWhiteSpace(_svgFilePath) 
                || !File.Exists(_svgFilePath) || _drawing == null)
            {
                return false;
            }

            this.UnloadDocument();

            var optionSettings = _mainWindow.OptionSettings;

            var wpfSettings = optionSettings.ConversionSettings;

            var fileReader = new FileSvgReader(wpfSettings);
            fileReader.SaveXaml = false;
            fileReader.SaveZaml = false;
            fileReader.Drawing  = _drawing;

            MemoryStream xamlStream = new MemoryStream();
            if (fileReader.Save(xamlStream))
            {
                _mainWindow.XamlPage.LoadDocument(xamlStream);
                return true;
            }

            return false;
        }

        public bool LoadTests(string svgFilePath, string pngFilePath)
        {
            if (string.IsNullOrWhiteSpace(svgFilePath) || !File.Exists(svgFilePath))
            {
                return false;
            }
            if (_mainWindow == null)
            {
                return false;
            }

            this.UnloadDocument();

            var optionSettings = _mainWindow.OptionSettings;

            Size imageSize = new Size(0, 0);
            if (!LoadImage(pngFilePath, optionSettings, ref imageSize))
            {
                return false;
            }

            try
            {
                if (webView != null)
                {
                    webView.Url = svgFilePath;

                    if (imageSize.Width >= ImageWidth || imageSize.Height >= ImageHeight)
                    {
                        webView.Width  = ImageWidth;
                        webView.Height = ImageHeight;
                    }
                    else
                    {
                        webView.Width  = imageSize.Width + 10;
                        webView.Height = imageSize.Height + 10;
                    }

                    if (webView.IsCreated)
                    {
                        webView.InvalidateVisual();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            var wpfSettings = optionSettings.ConversionSettings;

            var fileReader = new FileSvgReader(wpfSettings);
            fileReader.SaveXaml = false;
            fileReader.SaveZaml = false;

            _svgFilePath = svgFilePath;

            string fileExt = Path.GetExtension(svgFilePath);

            if (string.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase))
            {
                if (fileReader != null)
                {
                    _drawing = fileReader.Read(svgFilePath);
                    if (_drawing != null)
                    {
                        if (LoadDrawing(imageSize, _drawing))
                        {
                            if (optionSettings.ShowOutputFile && _mainWindow.XamlPage != null)
                            {
                                MemoryStream xamlStream = new MemoryStream();
                                if (fileReader.Save(xamlStream))
                                {
                                    _mainWindow.XamlPage.LoadDocument(xamlStream);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            _drawing     = null;
            _svgFilePath = null;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>A PNG file starts with an 8-byte signature[10] (refer to hex editor image on the right):</para>
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Portable_Network_Graphics#File_header">Portable Network Graphics</see>
        /// </para>
        /// </remarks>
        private static bool IsPng(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return stream.ReadByte() == 137; // or 89 (HEX)
            }
        }

        private bool LoadImage(string pngFilePath, OptionSettings options, ref Size imageSize)
        {
            if (string.IsNullOrWhiteSpace(pngFilePath) || !File.Exists(pngFilePath))
            {
                return false;
            }

            FileInfo pathInfo = new FileInfo(pngFilePath);
            if (pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                pngFilePath = options.EmptyImageFile;
            }

            if (!IsPng(pngFilePath))
            {
                var pngDir    = Path.GetDirectoryName(pngFilePath);
                var textLines = File.ReadAllLines(pngFilePath);
                if (textLines == null || textLines.Length == 0)
                {
                    return false;
                }
                var linkedFileName = string.Empty;
                foreach (var textLine in textLines)
                {
                    if (!string.IsNullOrWhiteSpace(textLine))
                    {
                        linkedFileName = textLine;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(linkedFileName))
                {
                    return false;
                }

                var currentDir = Environment.CurrentDirectory;
                Environment.CurrentDirectory = pngDir;
                var linkedPngFilePath = Path.GetFullPath(linkedFileName.Replace("/", "\\"));
                Environment.CurrentDirectory = currentDir;

                if (!File.Exists(linkedPngFilePath))
                {
                    return false;
                }
                pngFilePath = linkedPngFilePath;
            }

            _pngFilePath = pngFilePath;

            BitmapSource bitmap = null;
            try
            {
                bitmap = new BitmapImage(new Uri(pngFilePath));
                if ((int)bitmap.DpiY != 96) // Some of the images were not created in right DPI
                {
                    double dpi = 96;
                    int width  = bitmap.PixelWidth;
                    int height = bitmap.PixelHeight;

                    int bitsPerPixel = (int)((bitmap.Format.BitsPerPixel + 7) / 8);
                    int stride       = width * bitsPerPixel;
                    int dataLength   = stride * height;
                    byte[] pixelData = new byte[dataLength];
                    bitmap.CopyPixels(pixelData, stride, 0);

                    if (bitsPerPixel == 4)
                    {
                        bitmap = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Bgra32, null, pixelData, stride);
                    }
                    else
                    {
                        bitmap = BitmapSource.Create(width, height, dpi, dpi, bitmap.Format, bitmap.Palette, pixelData, stride);
                    }
                }

                pngImage.Source     = bitmap;
                pngImage.RenderSize = new Size(ImageWidth, ImageHeight);

                if (bitmap.Width >= ImageWidth || bitmap.Height >= ImageHeight)
                {
                    pngImage.Width  = ImageWidth;
                    pngImage.Height = ImageHeight;

                    pngCanvas.Width  = ImageWidth + 10;
                    pngCanvas.Height = ImageHeight + 10;

                    imageSize = new Size(ImageWidth, ImageHeight);
                }
                else
                {
                    pngCanvas.Width  = bitmap.Width  + 10;
                    pngCanvas.Height = bitmap.Height + 10;

                    imageSize = new Size(bitmap.Width, bitmap.Height);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Png: " + pngFilePath);
                Trace.TraceError(ex.ToString());
            }

            return true;
        }

        private bool LoadDrawing(Size imageSize, DrawingGroup drawing)
        {
            if (drawing == null)
            {
                return false;
            }

            try
            {
                Rect drawingBounds = drawing.Bounds;

                svgImage.Width      = double.NaN;
                svgImage.Height     = double.NaN;
                svgImage.RenderSize = new Size(ImageWidth, ImageHeight);

                svgImage.Source = new DrawingImage(drawing);

//                if (((int)drawingBounds.Width < ImageWidth || (int)drawingBounds.Height < ImageHeight))
                {
                    svgImage.Width  = imageSize.Width;
                    svgImage.Height = imageSize.Height;
                }

                svgCanvas.Width  = imageSize.Width + 10;
                svgCanvas.Height = imageSize.Height + 10;

                return true;
            }
            catch (Exception ex)
            {
                svgImage.Source = null;
                Trace.TraceError(ex.ToString());

                return false;
            }
        }

        public void UnloadDocument()
        {
            if (svgImage != null)
            {
                svgImage.Source = null;
            }
            if (pngImage != null)
            {
                pngImage.Source = null;
            }

            _drawing     = null;
            _svgFilePath = null;
            _pngFilePath = null;

            try
            {
                if (webView != null && webView.IsCreated)
                {
                    webView.Url = "about:blank";
                    webView.InvalidateVisual();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (webView != null)
            {
                //webView.NavigationStart                           += OnWebViewNavigationStart;
                //webView.ContentLoading                            += OnWebViewContentLoading;
                //webView.SourceChanged                             += OnWebViewSourceChanged;
                //webView.HistoryChanged                            += OnWebViewHistoryChanged;
                //webView.NavigationCompleted                       += OnWebViewNavigationCompleted;
                //webView.WebResourceRequested                      += OnWebViewWebResourceRequested;
                //webView.AcceleratorKeyPressed                     += OnWebViewAcceleratorKeyPressed;
                //webView.WebViewGotFocus                           += OnWebViewWebViewGotFocus;
                //webView.WebViewLostFocus                          += OnWebViewWebViewLostFocus;
                //webView.MoveFocusRequested                        += OnWebViewMoveFocusRequested;
                //webView.ZoomFactorChanged                         += OnWebViewZoomFactorChanged;
                //webView.DocumentTitleChanged                      += OnWebViewDocumentTitleChanged;
                //webView.ContainsFullScreenElementChanged          += OnWebViewContainsFullScreenElementChanged;
                //webView.PermissionRequested                       += OnWebViewPermissionRequested;
                //webView.FrameNavigationCompleted                  += OnWebViewFrameNavigationCompleted;
                //webView.FrameNavigationStarting                   += OnWebViewFrameNavigationStarting;
                //webView.WebMessageReceived                        += OnWebViewWebMessageReceived;
                //webView.ScriptToExecuteOnDocumentCreatedCompleted += OnWebViewScriptToExecuteOnDocumentCreatedCompleted;
                //webView.DOMContentLoaded                          += OnWebViewDOMContentLoaded;
                //webView.WebResourceResponseReceived               += OnWebViewWebResourceResponseReceived;
                //webView.WebViewCreated                            += OnWebViewWebViewCreated;
                //webView.BeforeWebViewDestroy                      += OnWebViewBeforeWebViewDestroy;


                //webView.BeforeWebViewDestroy += OnBeforeWebViewDestroy;

//                this.webView.BackColor = System.Drawing.SystemColors.Window;
                this.webView.DefaultBackgroundColor = Colors.White;
                this.webView.DefaultContextMenusEnabled = false;
                this.webView.DefaultScriptDialogsEnabled = false;
                this.webView.DevToolsEnabled = false;
//                this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
                this.webView.EnableMonitoring = false;
                this.webView.HtmlContent = null;
//                this.webView.IsCreated = false;
                this.webView.IsScriptEnabled = false;
                this.webView.IsStatusBarEnabled = false;
                this.webView.IsWebMessageEnabled = false;
                this.webView.IsZoomControlEnabled = false;
//                this.webView.Location = new System.Drawing.Point(0, 0);
//                this.webView.Name = "webView1";
                this.webView.RemoteObjectsAllowed = false;
            }
        }

        #endregion

        #region Private Event Handlers

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RowDefinition rowTab = rightGrid.RowDefinitions[2];
            rowTab.Height = new GridLength((this.ActualHeight - 8) / 2, GridUnitType.Pixel);
        }

        private void OnSaveTests(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.SaveTests();
            }
        }

        private void OnApplySuccess(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.ApplyState(SvgTestState.Success);
            }
        }

        private void OnApplyFailure(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.ApplyState(SvgTestState.Failure);
            }
        }

        private void OnApplyPartial(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.ApplyState(SvgTestState.Partial);
            }
        }

        private void OnApplyUnknown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mainWindow != null)
            {
                _mainWindow.ApplyState(SvgTestState.Unknown);
            }
        }

        #endregion

        #region Private Browser Event Handlers

        private void OnBeforeWebViewDestroy(object sender, EventArgs e)
        {
            Trace.TraceInformation("OnBeforeWebViewDestroy");
        }

        private void OnWebViewNavigationStart(object sender, NavigationStartingEventArgs e)
        {
            Trace.TraceInformation("OnWebViewNavigationStart: " + e.Uri);
            //MessageBox.Show(e.Uri);
        }

        private void OnWebViewContentLoading(object sender, ContentLoadingEventArgs e)
        {
            Trace.TraceInformation("OnWebViewContentLoading: " + e.NavigationId);
            //MessageBox.Show("Naviagation ID=>" + e.NavigationId);
        }

        private void OnWebViewSourceChanged(object sender, WebSourceChangedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewSourceChanged: " + e.IsNewDocument);
            //MessageBox.Show("SourceChanged=>" + e.IsNewDocument);

        }

        private void OnWebViewHistoryChanged(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewHistoryChanged: ");
        }

        private void OnWebViewNavigationCompleted(object sender, NavigationCompletedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewNavigationCompleted: " + e.GetErrorText());
            Trace.TraceInformation("Source: " + webView.Source);

            if (e.IsSuccess == true || e.WebErrorStatus == ErrorStatus.Unknown)
            {
                Trace.TraceInformation("Source: " + webView.BrowserVersion + " => " + webView.DocumentTitle);
            }
            else
            {
                Trace.TraceInformation("Source: " + webView.BrowserVersion + "=> Error=" + e.GetErrorText() + "->" + e.WebErrorStatus);

                if (e.WebErrorStatus == ErrorStatus.ConnectionAbborted)
                {
                    webView.Reload();
                }
            }
        }

        private void OnWebViewAcceleratorKeyPressed(object sender, AcceleratorKeyPressedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewAcceleratorKeyPressed");
            //MessageBox.Show(this, "OnWebViewAcceleratorKeyPressed");
            e.Handled = false;
        }

        private void OnWebViewDocumentTitleChanged(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewDocumentTitleChanged");
            //MessageBox.Show(this, "OnWebViewDocumentTitleChanged");
        }

        private void OnWebViewContainsFullScreenElementChanged(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewContainsFullScreenElementChanged");
            // MessageBox.Show(this, "OnWebViewContainsFullScreenElementChanged");
        }

        private void OnWebViewWebViewGotFocus(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebViewGotFocus");
            //MessageBox.Show(this, "OnWebViewWebViewGotFocus");
        }

        private void OnWebViewFrameNavigationStarting(object sender, NavigationStartingEventArgs e)
        {
            Trace.TraceInformation("OnWebViewFrameNavigationStarting");
            //MessageBox.Show(this, "OnWebViewFrameNavigationStarting");
        }

        private void OnWebViewWebViewLostFocus(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebViewLostFocus");
            //MessageBox.Show(this, "OnWebViewWebViewLostFocus");
        }

        private void OnWebViewMoveFocusRequested(object sender, MoveFocusRequestedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewMoveFocusRequested");
            //MessageBox.Show(this, "OnWebViewMoveFocusRequested");
        }

        private void OnWebViewPermissionRequested(object sender, PermissionRequestedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewPermissionRequested");
            //MessageBox.Show(this, "OnWebViewPermissionRequested");
        }

        private void OnWebViewWebMessageReceived(object sender, WebMessageReceivedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebMessageReceived");
        }

        private void OnWebViewWebResourceRequested(object sender, WebResourceRequestedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebResourceRequested");
            Debug.Print(e.Request.Uri);
            Debug.Print(e.Request.Method);
        }

        private void OnWebViewZoomFactorChanged(object sender, WebView2EventArgs e)
        {
            Trace.TraceInformation("OnWebViewZoomFactorChanged");
            //MessageBox.Show(this, "OnWebViewZoomFactorChanged");
        }

        private void OnWebViewWebViewCreated(object sender, EventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebViewCreated");
            //this._TestObject.Name = "hallo Welt";
            //webView.AddRemoteObject("testObject", this._TestObject);
            //string value = File.ReadAllText("index.html");
            //webView.NavigateToString(value);
            //this.txtUrl.AutoCompleteCustomSource.Add(webView.MonitoringUrl);
        }

        private void OnWebViewScriptToExecuteOnDocumentCreatedCompleted(object sender, AddScriptToExecuteOnDocumentCreatedCompletedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewScriptToExecuteOnDocumentCreatedCompleted");
            Debug.Print(e.Id);
        }

        private void OnWebViewFrameNavigationCompleted(object sender, NavigationCompletedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewFrameNavigationCompleted");
            //MessageBox.Show(this, "OnWebViewFrameNavigationCompleted");
        }

        private void OnWebViewBeforeWebViewDestroy(object sender, EventArgs e)
        {
            Trace.TraceInformation("OnWebViewBeforeWebViewDestroy");
            //webView.RemoveRemoteObject("{60A417CA-F1AB-4307-801B-F96003F8938B} Host Object Helper");
            //webView.RemoveRemoteObject("testObject");
        }

        private void OnWebViewDOMContentLoaded(object sender, DOMContentLoadedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewDOMContentLoaded");
            Debug.Print(e.NavigationId.ToString());
        }

        private void OnWebViewWebResourceResponseReceived(object sender, WebResourceResponseReceivedEventArgs e)
        {
            Trace.TraceInformation("OnWebViewWebResourceResponseReceived");
            Debug.Print(e.Request.Uri);
            Debug.Print(e.Response.StatusCode.ToString());
            Debug.Print(e.Response.ReasonPhrase);
            var it = e.Response.Headers.GetIterator();

            if (it.HasCurrent)
                Debug.Print(it.Current.Name + "," + it.Current.Value);
            while (it.MoveNext())
            {
                Debug.Print(it.Current.Name + "," + it.Current.Value);
            }
        }

        #endregion
    }
}
