using System;
using System.IO;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using SharpVectors.Runtime;
using SharpVectors.Renderers;
using SharpVectors.Converters;

namespace WpfTestSvgSample
{
    /// <summary>
    /// Interaction logic for DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Page
    {
        #region Private Fields

        private string _drawingDir;
        private DirectoryInfo _directoryInfo;

        private FileSvgReader _fileReader;

        private DirectoryInfo _workingDir;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        #endregion

        #region Constructors and Destructor

        public DrawingPage()
        {
            InitializeComponent();

            _fileReader          = new FileSvgReader();
            _fileReader.SaveXaml = true;
            _fileReader.SaveZaml = false;

            mouseHandlingMode = MouseHandlingMode.None;

            string workDir = Path.Combine(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), "XamlDrawings");

            _workingDir = new DirectoryInfo(workDir);

            this.Loaded += new RoutedEventHandler(OnPageLoaded);
        }

        #endregion      

        #region Public Properties

        public string XamlDrawingDir
        {
            get 
            { 
                return _drawingDir; 
            }
            set 
            { 
                _drawingDir = value; 

                if (!String.IsNullOrEmpty(_drawingDir))
                {
                    _directoryInfo = new DirectoryInfo(_drawingDir);

                    if (_fileReader != null)
                    {
                        _fileReader.SaveXaml = Directory.Exists(_drawingDir);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public bool LoadDocument(string svgFilePath)
        {
            if (String.IsNullOrEmpty(svgFilePath) || !File.Exists(svgFilePath))
            {
                return false;
            }

            DirectoryInfo workingDir = _workingDir;
            if (_directoryInfo != null)
            {
                workingDir = _directoryInfo;
            }

           //double currentZoom = zoomSlider.Value;

            svgViewer.UnloadDiagrams();

            //zoomSlider.Value = 1.0;

            string fileExt = Path.GetExtension(svgFilePath);

            if (String.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                if (_fileReader != null)
                {
                    _fileReader.SaveXaml = true;
                    _fileReader.SaveZaml = false;

                    DrawingGroup drawing = _fileReader.Read(svgFilePath, workingDir);
                    if (drawing != null)
                    {
                        svgViewer.RenderDiagrams(drawing);

                        //zoomSlider.Value = currentZoom;

                        Rect bounds = svgViewer.Bounds;

                        //Rect rect = new Rect(0, 0,
                        //    mainFrame.RenderSize.Width, mainFrame.RenderSize.Height);
                        //Rect rect = new Rect(0, 0,
                        //    bounds.Width, bounds.Height);
                        if (bounds.IsEmpty)
                        {
                            bounds = new Rect(0, 0, 
                                canvasScroller.ActualWidth, canvasScroller.ActualHeight);
                        }
                        zoomPanControl.AnimatedZoomTo(bounds);

                        return true;
                    }
                }
            }
            else if (String.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
            {
                svgViewer.LoadDiagrams(svgFilePath);

                //zoomSlider.Value = currentZoom;

                svgViewer.InvalidateMeasure();

                return true;
            }

            return false;
        }

        public void UnloadDocument()
        {
            if (svgViewer != null)
            {
                svgViewer.UnloadDiagrams();
            }
        }

        public void PageSelected(bool isSelected)
        {   
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        #endregion

        #region Private Event Handlers

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 100;

            if (zoomPanControl != null)
            {
                zoomPanControl.IsMouseWheelScrollingEnabled = true;
            }
        }

        private void OnZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (zoomPanControl != null)
            {
                zoomPanControl.AnimatedZoomTo(zoomSlider.Value / 100.0);
            }
        }

        private void OnZoomInClick(object sender, RoutedEventArgs e)
        {
            this.ZoomIn();
        }

        private void OnZoomOutClick(object sender, RoutedEventArgs e)
        {
            this.ZoomOut();
        }

        private void OnResetZoom(object sender, RoutedEventArgs e)
        {
            if (zoomPanControl == null)
            {
                return;
            }

            zoomPanControl.ContentScale = 1.0;
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomFitClick(object sender, RoutedEventArgs e)
        {
            if (svgViewer == null || zoomPanControl == null)
            {
                return;
            }

            Rect bounds = svgViewer.Bounds;

            //Rect rect = new Rect(0, 0,
            //    mainFrame.RenderSize.Width, mainFrame.RenderSize.Height);
            //Rect rect = new Rect(0, 0,
            //    bounds.Width, bounds.Height);
            if (bounds.IsEmpty)
            {
                bounds = new Rect(0, 0,
                    canvasScroller.ActualWidth, canvasScroller.ActualHeight);
            }
            zoomPanControl.AnimatedZoomTo(bounds);
        }

        private void OnPanClick(object sender, RoutedEventArgs e)
        {
            //if (drawScrollView == null)
            //{
            //    return;
            //}

            //drawScrollView.ZoomableCanvas.IsPanning = 
            //    (tbbPanning.IsChecked != null && tbbPanning.IsChecked.Value);
        }

        #region Private Zoom Panel Handlers

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            svgViewer.Focus();
            Keyboard.Focus(svgViewer);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomPanControl);
            origContentMouseDownPoint = e.GetPosition(svgViewer);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
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
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn();
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut();
                    }
                }

                zoomPanControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void OnZoomPanMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(svgViewer);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomPanControl.ContentOffsetX -= dragOffset.X;
                zoomPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void OnZoomPanMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomFit(object sender, RoutedEventArgs e)
        {
            if (svgViewer == null || zoomPanControl == null)
            {
                return;
            }

            Rect bounds = svgViewer.Bounds;

            //Rect rect = new Rect(0, 0,
            //    mainFrame.RenderSize.Width, mainFrame.RenderSize.Height);
            //Rect rect = new Rect(0, 0,
            //    bounds.Width, bounds.Height);
            if (bounds.IsEmpty)
            {
                bounds = new Rect(0, 0,
                    canvasScroller.ActualWidth, canvasScroller.ActualHeight);
            }
            zoomPanControl.AnimatedZoomTo(bounds);
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        private void ZoomOut()
        {
            if (zoomPanControl == null)
            {
                return;
            }

            zoomPanControl.ContentScale -= 0.1;
        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        private void ZoomIn()
        {
            if (zoomPanControl == null)
            {
                return;
            }

            zoomPanControl.ContentScale += 0.1;
        }

        #endregion

        #endregion
    }
}
