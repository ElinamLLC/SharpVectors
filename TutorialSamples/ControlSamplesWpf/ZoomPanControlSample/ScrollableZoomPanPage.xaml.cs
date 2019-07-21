using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Win32;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpVectors.Runtime;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace ZoomPanControlSample
{
    /// <summary>
    /// Interaction logic for ScrollableZoomPanPage.xaml
    /// </summary>
    public partial class ScrollableZoomPanPage : Page
    {
        #region Private Fields

        private const double ZoomChange = 0.1;

        private bool _isLoadingDrawing;

        private string _svgFilePath;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private ZoomPanMouseHandlingMode _mouseHandlingMode;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point _origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton _mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect _prevZoomRect;

        /// <summary>
        /// Save the previous content scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double _prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool _prevZoomRectSet;

        /// <summary>
        /// Saves the next zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect _nextZoomRect;

        /// <summary>
        /// Save the next content scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double _nextZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool _nextZoomRectSet;

        private MainWindow _mainWindow;

        private DispatcherTimer _dispatcherTimer;

        private ZoomPanOverviewWindow _wndOverview;

        #endregion

        #region Constructors and Destructor

        public ScrollableZoomPanPage()
        {
            InitializeComponent();

            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.CultureInfo = _wpfSettings.NeutralCultureInfo;

            _fileReader = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            _mouseHandlingMode = ZoomPanMouseHandlingMode.None;

            this.Loaded      += OnPageLoaded;
            this.Unloaded    += OnPageUnloaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Properties

        //
        // Definitions for dependency properties.
        //
        /// <summary>
        /// This allows the same property name to be used for direct and indirect access to the ZoomPanelControl control.
        /// </summary>
        public ZoomPanControl ZoomPanContent
        {
            get {
                return zoomPanControl;
            }
        }

        /// <summary>
        /// This allows the same property name to be used for direct and indirect access to the SVG Canvas control.
        /// </summary>
        public SvgDrawingCanvas Viewer
        {
            get {
                return svgViewer;
            }
        }

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

        public void SaveZoom()
        {
            if (zoomPanControl != null)
            {
                SavePrevZoomRect();

                ClearNextZoomRect();
            }
        }

        #endregion

        #region Private Event Handlers (Page)

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_svgFilePath) || !File.Exists(_svgFilePath))
            {
                zoomPanControl.ContentScale = 1.0;

                if (zoomPanControl != null)
                {
                    zoomPanControl.IsMouseWheelScrollingEnabled = true;
                }

                if (string.IsNullOrWhiteSpace(_svgFilePath))
                {
                    this.UnloadDocument();
                }
            }

            try
            {
                //  DispatcherTimer setup
                if (_dispatcherTimer == null)
                {
                    _dispatcherTimer = new DispatcherTimer();
                    _dispatcherTimer.Tick += OnUpdateUITick;
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            if (zoomPanControl != null && zoomPanControl.ScrollOwner == null)
            {
                if (canvasScroller != null)
                {
                    zoomPanControl.ScrollOwner = canvasScroller;
                }
            }

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Start();
            }

            if (_mainWindow != null)
            {
                _wndOverview = new ZoomPanOverviewWindow();
                _wndOverview.Title       = "Overview - Scrollable";
                _wndOverview.PageMode    = ZoomPanPageMode.Scrollable;
                _wndOverview.Owner       = _mainWindow;
                _wndOverview.DataContext = this;
                _wndOverview.Left        = _mainWindow.Left + _mainWindow.ActualWidth;
                _wndOverview.Top         = _mainWindow.Top + _mainWindow.ActualHeight - _wndOverview.Height;

                _wndOverview.Show();
            }
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (zoomPanControl != null && svgViewer != null)
            {
                svgViewer.InvalidateMeasure();
                svgViewer.UpdateLayout();

                Rect bounds = svgViewer.Bounds;

                if (bounds.IsEmpty)
                {
                    bounds = new Rect(0, 0, svgViewer.ActualWidth, svgViewer.ActualHeight);
                }

                //zoomPanControl.AnimatedZoomTo(bounds);
                zoomPanControl.AnimatedZoomTo(this.FitZoomValue);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }
            if (_wndOverview != null)
            {
                _wndOverview.Close();
                _wndOverview = null;
            }
        }

        private async void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Title = "Select An SVG File";
            dlg.DefaultExt = "*.svg";
            dlg.Filter = "All SVG Files (*.svg,*.svgz)|*.svg;*.svgz"
                + "|Svg Uncompressed Files (*.svg)|*.svg"
                + "|SVG Compressed Files (*.svgz)|*.svgz";

            bool? isSelected = dlg.ShowDialog();

            if (isSelected != null && isSelected.Value)
            {
                await this.LoadDocumentAsync(dlg.FileName);
            }
        }

        /// <summary>
        /// Updates the current seconds display and calls InvalidateRequerySuggested on the 
        /// CommandManager to force the Command to raise the CanExecuteChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateUITick(object sender, EventArgs e)
        {
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region Private Zoom Panel Handlers

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            zoomPanControl.Focus();
            Keyboard.Focus(zoomPanControl);

            _mouseButtonDown = e.ChangedButton;
            _origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomPanControl);
            _origContentMouseDownPoint = e.GetPosition(svgViewer);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode = ZoomPanMouseHandlingMode.Zooming;
            }
            else if (_mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning;
            }

            if (_mouseHandlingMode != ZoomPanMouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                zoomPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseHandlingMode != ZoomPanMouseHandlingMode.None)
            {
                if (_mouseHandlingMode == ZoomPanMouseHandlingMode.Zooming)
                {
                    if (_mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(_origContentMouseDownPoint);
                    }
                    else if (_mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(_origContentMouseDownPoint);
                    }
                }
                else if (_mouseHandlingMode == ZoomPanMouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect();
                }

                zoomPanControl.ReleaseMouseCapture();
                _mouseHandlingMode = ZoomPanMouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseHandlingMode == ZoomPanMouseHandlingMode.Panning)
            {
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                Point curContentMousePoint = e.GetPosition(svgViewer);
                Vector dragOffset = curContentMousePoint - _origContentMouseDownPoint;

                zoomPanControl.ContentOffsetX -= dragOffset.X;
                zoomPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (_mouseHandlingMode == ZoomPanMouseHandlingMode.Zooming)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - _origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (_mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    _mouseHandlingMode = ZoomPanMouseHandlingMode.DragZooming;

                    Point curContentMousePoint = e.GetPosition(svgViewer);
                    InitDragZoomRect(_origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (_mouseHandlingMode == ZoomPanMouseHandlingMode.DragZooming)
            {
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                Point curContentMousePoint = e.GetPosition(svgViewer);
                SetDragZoomRect(_origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void OnZoomPanMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            Point curContentMousePoint = e.GetPosition(svgViewer);
            this.Zoom(curContentMousePoint, e.Delta);

            if (svgViewer.IsKeyboardFocusWithin)
            {
                Keyboard.Focus(zoomPanControl);
            }
        }

        /// <summary>
        /// Event raised by double-left click
        /// </summary>
        private void OnZoomPanMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                SavePrevZoomRect();

                zoomPanControl.AnimatedSnapTo(e.GetPosition(svgViewer));

                ClearNextZoomRect();

                e.Handled = true;
            }
        }

        /// <summary>
        /// The 'Pan' command (bound to the plus key) was executed.
        /// </summary>
        private void OnPanMode(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Determines whether the 'Pan' command can be executed.
        /// </summary>
        private void OnCanPanMode(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        /// <summary>
        /// The 'ZoomReset' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomReset(object sender, RoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomPanControl.AnimatedZoomTo(1.0);

            ClearNextZoomRect();
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Determines whether the 'ZoomReset' command can be executed.
        /// </summary>
        private void OnCanZoomReset(object sender, CanExecuteRoutedEventArgs e)
        {
            if (zoomPanControl == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = !zoomPanControl.ContentScale.Equals(1.0);
        }

        /// <summary>
        /// The 'ZoomFit/Fill' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomFit(object sender, RoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomPanControl.AnimatedScaleToFit();
            zoomPanControl.AnimatedZoomTo(this.FitZoomValue);

            ClearNextZoomRect();
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Determines whether the 'ZoomFit' command can be executed.
        /// </summary>
        private void OnCanZoomFit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (zoomPanControl == null)
            {
                e.CanExecute = false;
                return;
            }

            double scaleX   = zoomPanControl.ContentViewportWidth / svgViewer.ActualWidth;
            double scaleY   = zoomPanControl.ContentViewportHeight / svgViewer.ActualHeight;
            double fitValue = zoomPanControl.ContentScale * Math.Min(scaleX, scaleY);

            // double fitValue = this.FitZoomValue;

            e.CanExecute = !IsWithinOnePercent(zoomPanControl.ContentScale, fitValue)
                && fitValue >= zoomPanControl.MinContentScale;
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            SavePrevZoomRect();

            ZoomIn(new Point(zoomPanControl.ContentZoomFocusX, zoomPanControl.ContentZoomFocusY));

            ClearNextZoomRect();
        }

        /// <summary>
        /// Determines whether the 'ZoomIn' command can be executed.
        /// </summary>
        private void OnCanZoomIn(object sender, CanExecuteRoutedEventArgs e)
        {
            if (zoomPanControl == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = zoomPanControl.ContentScale < zoomPanControl.MaxContentScale;
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            SavePrevZoomRect();

            ZoomOut(new Point(zoomPanControl.ContentZoomFocusX, zoomPanControl.ContentZoomFocusY));

            ClearNextZoomRect();
        }

        /// <summary>
        /// Determines whether the 'UndoZoom' command can be executed.
        /// </summary>
        private void OnCanZoomOut(object sender, CanExecuteRoutedEventArgs e)
        {
            if (zoomPanControl == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = zoomPanControl.ContentScale > zoomPanControl.MinContentScale;
        }

        /// <summary>
        /// The 'UndoZoom' command was executed.
        /// </summary>
        private void OnUndoZoom(object sender, ExecutedRoutedEventArgs e)
        {
            UndoZoom();
        }

        /// <summary>
        /// Determines whether the 'UndoZoom' command can be executed.
        /// </summary>
        private void OnCanUndoZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _prevZoomRectSet;
        }

        /// <summary>
        /// The 'RedoZoom' command was executed.
        /// </summary>
        private void OnRedoZoom(object sender, ExecutedRoutedEventArgs e)
        {
            RedoZoom();
        }

        /// <summary>
        /// Determines whether the 'RedoZoom' command can be executed.
        /// </summary>
        private void OnCanRedoZoom(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _nextZoomRectSet;
        }

        /// <summary>
        /// Jump back to the previous zoom level.
        /// </summary>
        private void UndoZoom()
        {
            SaveNextZoomRect();

            zoomPanControl.AnimatedZoomTo(_prevZoomScale, _prevZoomRect);

            ClearPrevZoomRect();
        }

        /// <summary>
        /// Jump back to the next zoom level.
        /// </summary>
        private void RedoZoom()
        {
            SavePrevZoomRect();

            zoomPanControl.AnimatedZoomTo(_nextZoomScale, _nextZoomRect);

            ClearNextZoomRect();
        }

        private void Zoom(Point contentZoomCenter, int wheelMouseDelta)
        {
            SavePrevZoomRect();

            // Found the division by 3 gives a little smoothing effect
            var zoomFactor = zoomPanControl.ContentScale + ZoomChange * wheelMouseDelta / (120 * 3);

            zoomPanControl.ZoomAboutPoint(zoomFactor, contentZoomCenter);

            ClearNextZoomRect();
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            SavePrevZoomRect();

            zoomPanControl.ZoomAboutPoint(zoomPanControl.ContentScale - ZoomChange, contentZoomCenter);

            ClearNextZoomRect();
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            SavePrevZoomRect();

            zoomPanControl.ZoomAboutPoint(zoomPanControl.ContentScale + ZoomChange, contentZoomCenter);

            ClearNextZoomRect();
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            zoomPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();

            ClearNextZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            ZoomPanAnimationHelper.StartAnimation(dragZoomBorder, OpacityProperty, 0.0, ZoomChange,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            _prevZoomRect = new Rect(zoomPanControl.ContentOffsetX, zoomPanControl.ContentOffsetY,
                zoomPanControl.ContentViewportWidth, zoomPanControl.ContentViewportHeight);
            _prevZoomScale = zoomPanControl.ContentScale;
            _prevZoomRectSet = true;
        }

        //
        // Record the next zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SaveNextZoomRect()
        {
            _nextZoomRect = new Rect(zoomPanControl.ContentOffsetX, zoomPanControl.ContentOffsetY,
                zoomPanControl.ContentViewportWidth, zoomPanControl.ContentViewportHeight);
            _nextZoomScale = zoomPanControl.ContentScale;
            _nextZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            _prevZoomRectSet = false;
        }

        /// <summary>
        /// Clear the memory of the next zoom level.
        /// </summary>
        private void ClearNextZoomRect()
        {
            _nextZoomRectSet = false;
        }

        public double FitZoomValue
        {
            get {
                if (zoomPanControl == null)
                {
                    return 1;
                }

                var content = zoomPanControl.ContentElement;

                return FitZoom(ActualWidth, ActualHeight, content?.ActualWidth, content?.ActualHeight);
            }
        }

        private static bool IsWithinOnePercent(double value, double testValue)
        {
            return Math.Abs(value - testValue) < .01 * testValue;
        }

        private static double FitZoom(double actualWidth, double actualHeight, double? contentWidth, double? contentHeight)
        {
            if (!contentWidth.HasValue || !contentHeight.HasValue) return 1;
            return Math.Min(actualWidth / contentWidth.Value, actualHeight / contentHeight.Value);
        }

        #endregion

        #region Private Methods

        private Task<bool> LoadDocumentAsync(string svgFilePath)
        {
            if (_isLoadingDrawing || string.IsNullOrWhiteSpace(svgFilePath) || !File.Exists(svgFilePath))
            {
                return Task.FromResult<bool>(false);
            }

            string fileExt = Path.GetExtension(svgFilePath);

            if (!(string.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase)))
            {
                _svgFilePath = null;
                return Task.FromResult<bool>(false);
            }

            if (_fileReader == null)
            {
                _fileReader = new FileSvgReader(_wpfSettings);
                _fileReader.SaveXaml = false;
                _fileReader.SaveZaml = false;
            }

            _isLoadingDrawing = true;

            this.UnloadDocument(true);

            _svgFilePath = svgFilePath;

            MemoryStream drawingStream = new MemoryStream();

            // Get the UI thread's context
            var context = TaskScheduler.FromCurrentSynchronizationContext();

            return Task<bool>.Factory.StartNew(() =>
            {
                var saveXaml = _fileReader.SaveXaml;
                DrawingGroup drawing = _fileReader.Read(svgFilePath);
                _fileReader.SaveXaml = saveXaml;
                if (drawing != null)
                {
                    XamlWriter.Save(drawing, drawingStream);
                    drawingStream.Seek(0, SeekOrigin.Begin);

                    return true;
                }
                _svgFilePath = null;
                return false;
            }).ContinueWith((t) => {
                try
                {
                    if (!t.Result)
                    {
                        _isLoadingDrawing = false;
                        _svgFilePath = null;
                        return false;
                    }
                    if (drawingStream.Length != 0)
                    {
                        DrawingGroup drawing = (DrawingGroup)XamlReader.Load(drawingStream);

                        svgViewer.UnloadDiagrams();
                        svgViewer.RenderDiagrams(drawing);

                        svgViewer.InvalidateMeasure();
                        svgViewer.UpdateLayout();

                        Rect bounds = svgViewer.Bounds;

                        if (bounds.IsEmpty)
                        {
                            bounds = new Rect(0, 0, svgViewer.ActualWidth, svgViewer.ActualHeight);
                        }

                        zoomPanControl.AnimatedZoomTo(bounds);
                        CommandManager.InvalidateRequerySuggested();
                    }

                    _isLoadingDrawing = false;

                    return true;
                }
                catch
                {
                    _isLoadingDrawing = false;
                    throw;
                }
            }, context);
        }

        private void UnloadDocument(bool displayMessage = false)
        {
            _svgFilePath = null;

            if (svgViewer != null)
            {
                svgViewer.UnloadDiagrams();

                if (displayMessage)
                {
                    var drawing = this.DrawText("Loading...");

                    svgViewer.RenderDiagrams(drawing);

                    Rect bounds = svgViewer.Bounds;
                    if (bounds.IsEmpty)
                    {
                        bounds = drawing.Bounds;
                    }

                    zoomPanControl.ZoomTo(bounds);
                    return;
                }
            }

            var drawRect = this.DrawRect();
            svgViewer.RenderDiagrams(drawRect);

            zoomPanControl.ZoomTo(drawRect.Bounds);
            ClearPrevZoomRect();
            ClearNextZoomRect();
        }

        private bool SaveDocument(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            if (_fileReader == null || _fileReader.Drawing == null)
            {
                return false;
            }

            return _fileReader.Save(fileName, true, false);
        }

        private DrawingGroup DrawRect()
        {
            // Create a new DrawingGroup of the control.
            DrawingGroup drawingGroup = new DrawingGroup();

            // Open the DrawingGroup in order to access the DrawingContext.
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, 280, 300));
            }
            // Return the updated DrawingGroup content to be used by the control.
            return drawingGroup;
        }

        // Convert the text string to a geometry and draw it to the control's DrawingContext.
        private DrawingGroup DrawText(string textString)
        {
            // Create a new DrawingGroup of the control.
            DrawingGroup drawingGroup = new DrawingGroup();

            drawingGroup.Opacity = 0.8;

            // Open the DrawingGroup in order to access the DrawingContext.
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                // Create the formatted text based on the properties set.
                var formattedText = new FormattedText(textString,
                    CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Tahoma"), FontStyles.Normal,
                    FontWeights.Normal, FontStretches.Normal), 72, Brushes.Black);

                // Build the geometry object that represents the text.
                Geometry textGeometry = formattedText.BuildGeometry(new Point(20, 0));

                drawingContext.DrawRoundedRectangle(Brushes.Transparent, null,
                    new Rect(new Size(formattedText.Width + 50, formattedText.Height + 5)), 5.0, 5.0);

                // Draw the outline based on the properties that are set.
                drawingContext.DrawGeometry(null, new Pen(Brushes.DarkGray, 1.5), textGeometry);
            }

            // Return the updated DrawingGroup content to be used by the control.
            return drawingGroup;
        }

        #endregion
    }
}
