using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using SharpVectors.Runtime;

namespace WpfTestSvgControl
{
    /// <summary>
    /// Interaction logic for ZoomPanOverview.xaml
    /// </summary>
    public partial class ZoomPanOverview : UserControl
    {
        #region Private Fields

        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _dragBorder;

        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _sizingBorder;

        private bool _sizingEvent;

        /// <summary>
        /// The control for containing a zoom border
        /// </summary>
        private Canvas _viewportCanvas;

        private Viewbox _viewportBox;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private ZoomPanMouseHandlingMode _mouseHandlingMode = ZoomPanMouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;
        
        #endregion

        #region Constructors and Destructor

        public ZoomPanOverview()
        {
            InitializeComponent();

            this.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.VerticalContentAlignment   = VerticalAlignment.Center;
        }

        /// <summary>
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        static ZoomPanOverview()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomPanOverview), 
                new FrameworkPropertyMetadata(typeof(ZoomPanOverview)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dragBorder     = this.Template.FindName("PART_DraggingBorder", this) as Border;
            _sizingBorder   = this.Template.FindName("PART_SizingBorder", this) as Border;
            _viewportCanvas = this.Template.FindName("PART_Content", this) as Canvas;
            _viewportBox    = this.Template.FindName("PART_Viewbox", this) as Viewbox;
        }

        #endregion

        #region Mouse Event Handlers

        /// <summary>
        /// Event raised on mouse down in the ZoomPanControl.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var drawingPage = this.GetDrawingPage();
            if (drawingPage != null)
            {
                drawingPage.SaveZoom();
            }
            _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning;
            _origContentMouseDownPoint = e.GetPosition(_viewportCanvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode       = ZoomPanMouseHandlingMode.DragZooming;
                _dragBorder.Visibility   = Visibility.Hidden;
                _sizingBorder.Visibility = Visibility.Visible;
                Canvas.SetLeft(_sizingBorder, _origContentMouseDownPoint.X);
                Canvas.SetTop(_sizingBorder, _origContentMouseDownPoint.Y);
                _sizingBorder.Width  = 0;
                _sizingBorder.Height = 0;
            }
            else
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = ZoomPanMouseHandlingMode.Panning;
            }

            if (_mouseHandlingMode != ZoomPanMouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                _viewportCanvas.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomPanControl.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_mouseHandlingMode == ZoomPanMouseHandlingMode.DragZooming)
            {
                var zoomAndPanControl = GetZoomPanControl();
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = GetClip(curContentPoint, _origContentMouseDownPoint, new Point(0, 0),
                    new Point(_viewportCanvas.Width, _viewportCanvas.Height));
                zoomAndPanControl.AnimatedZoomTo(rect);
                _dragBorder.Visibility = Visibility.Visible;
                _sizingBorder.Visibility = Visibility.Hidden;
            }
            _mouseHandlingMode = ZoomPanMouseHandlingMode.None;
            _viewportCanvas.ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomPanControl.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_mouseHandlingMode == ZoomPanMouseHandlingMode.Panning)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rectangleDragVector = curContentPoint - _origContentMouseDownPoint;
                //
                // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
                //
                _origContentMouseDownPoint = Clamp(e.GetPosition(_viewportCanvas));
                Canvas.SetLeft(_dragBorder, Canvas.GetLeft(_dragBorder) + rectangleDragVector.X);
                Canvas.SetTop(_dragBorder, Canvas.GetTop(_dragBorder) + rectangleDragVector.Y);
            }
            else if (_mouseHandlingMode == ZoomPanMouseHandlingMode.DragZooming)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = GetClip(curContentPoint, _origContentMouseDownPoint, 
                    new Point(0, 0), new Point(_viewportCanvas.Width, _viewportCanvas.Height));

                PositionBorderOnCanvas(_sizingBorder, rect);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event raised with the double click command
        /// </summary>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                var drawingPage = this.GetDrawingPage();
                if (drawingPage != null)
                {
                    drawingPage.SaveZoom();
                }
                var zoomAndPanControl = GetZoomPanControl();
                zoomAndPanControl.AnimatedSnapTo(e.GetPosition(_viewportCanvas));
            }
        }

        #endregion

        #region Background--Visual Brush

        /// <summary>
        /// The X coordinate of the content focus, this is the point that we are focusing on when zooming.
        /// </summary>
        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register("Visual",
            typeof(FrameworkElement), typeof(ZoomPanOverview), new FrameworkPropertyMetadata(null, OnVisualChanged));

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomPanOverview)d;

            c.SetBackground(e.NewValue as FrameworkElement);
        }

        private void SetBackground(FrameworkElement frameworkElement)
        {
            try
            {
                frameworkElement = frameworkElement ?? (DataContext as ContentControl)?.Content as FrameworkElement;
                if (frameworkElement == null)
                {
                    return;
                }
                var visualBrush = new VisualBrush
                {
                    Visual        = frameworkElement,
                    ViewboxUnits  = BrushMappingMode.RelativeToBoundingBox,
                    ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                    AlignmentX    = AlignmentX.Center,
                    AlignmentY    = AlignmentY.Center,
                    TileMode      = TileMode.None,
                    Stretch       = Stretch.Uniform
                };

                if (!_sizingEvent)
                {
                    frameworkElement.SizeChanged += (s, e) =>
                    {
                        _viewportCanvas.Height = frameworkElement.ActualHeight;
                        _viewportCanvas.Width = frameworkElement.ActualWidth;
                        _viewportCanvas.Background = visualBrush;
                    };

                    _viewportCanvas.Height     = frameworkElement.ActualHeight;
                    _viewportCanvas.Width      = frameworkElement.ActualWidth;
                    _viewportCanvas.Background = visualBrush;

                    _sizingEvent = true;
                }
                else
                {
                    _viewportCanvas.Height     = frameworkElement.ActualHeight;
                    _viewportCanvas.Width      = frameworkElement.ActualWidth;
                    _viewportCanvas.Background = visualBrush;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        #region Private Methods

        private DrawingPage GetDrawingPage()
        {
            return this.DataContext as DrawingPage;
        }

        private ZoomPanControl GetZoomPanControl()
        {
            var zoomAndPanControl = (this.DataContext as DrawingPage)?.ZoomPanContent;
            if (zoomAndPanControl == null)
                throw new NullReferenceException("DataContext is not of type ZoomPanControl");
            return zoomAndPanControl;
        }

        /// <summary>
        /// Moves and sized a border on a Canvas according to a Rect
        /// </summary>
        /// <param name="border">Border to be moved and sized</param>
        /// <param name="rect">Rect that specifies the size and postion of the Border on the Canvas</param>
        private static void PositionBorderOnCanvas(Border border, Rect rect)
        {
            Canvas.SetLeft(border, rect.Left);
            Canvas.SetTop(border, rect.Top);
            border.Width = rect.Width;
            border.Height = rect.Height;
        }

        /// <summary>
        /// Limits the extent of a Point to the area where X and Y are at least 0
        /// </summary>
        /// <param name="value">Point to be clamped</param>
        /// <returns></returns>
        private static Point Clamp(Point value)
        {
            return new Point(Math.Max(value.X, 0), Math.Max(value.Y, 0));
        }

        /// <summary>
        /// Limits the extent of a Point to the area between two points
        /// </summary>
        /// <param name="value"></param>
        /// <param name="topLeft">Point specifiying the Top Left corner</param>
        /// <param name="bottomRight">Point specifiying the Bottom Right corner</param>
        /// <returns>The Point clamped by the Top Left and Bottom Right points</returns>
        private static Point Clamp(Point value, Point topLeft, Point bottomRight)
        {
            return new Point(Math.Max(Math.Min(value.X, bottomRight.X), topLeft.X),
                Math.Max(Math.Min(value.Y, bottomRight.Y), topLeft.Y));
        }

        /// <summary>
        /// Return a Rect that specificed by two points and clipped by a rectangle specified
        /// by two other points
        /// </summary>
        /// <param name="value1">First Point specifing the rectangle to be clipped</param>
        /// <param name="value2">Second Point specifing the rectangle to be clipped</param>
        /// <param name="topLeft">Point specifiying the Top Left corner of the clipping rectangle</param>
        /// <param name="bottomRight">Point specifiying the Bottom Right corner of the clipping rectangle</param>
        /// <returns>Rectangle specified by two points clipped by the other two points</returns>
        private static Rect GetClip(Point value1, Point value2, Point topLeft, Point bottomRight)
        {
            var point1 = Clamp(value1, topLeft, bottomRight);
            var point2 = Clamp(value2, topLeft, bottomRight);
            var newTopLeft = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            var size = new Size(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
            return new Rect(newTopLeft, size);
        }

        #endregion
    }
}
