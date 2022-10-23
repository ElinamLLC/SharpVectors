using System;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using SharpVectors.Runtime.Utils;

using IoPath = System.IO.Path;

namespace SharpVectors.Runtime
{
    /// <summary>
    /// This is the main drawing canvas for the rendered SVG diagrams.
    /// </summary>
    public class SvgDrawingCanvas : Canvas
    {
        #region Private Fields

        private const string DefaultTitle = "SharpVectors";

        private string _appTitle;
        private bool _drawForInteractivity;

        private Rect _bounds;

        private Transform _displayTransform;

        private double _offsetX;
        private double _offsetY;

        private ToolTip _tooltip;
        private TextBlock _tooltipText;

        private DrawingGroup _wholeDrawing;
        private DrawingGroup _linksDrawing;
        private DrawingGroup _mainDrawing;

        private Drawing _hitVisual;

        private DrawingVisual _hostVisual;

        private SvgInteractiveModes _interactiveMode;

        // Create a collection of child visual objects.
        private List<Drawing> _drawObjects;

        // Create a collection of child visual link objects.
        private List<Drawing> _linkObjects;

        private SvgAnimationLayer _animationCanvas;

        private event EventHandler<SvgAlertArgs> _svgAlerts;
        private event EventHandler<SvgErrorArgs> _svgErrors;

        #endregion

        #region Constructors and Destructor

        static SvgDrawingCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgDrawingCanvas),
                new FrameworkPropertyMetadata(typeof(SvgDrawingCanvas)));
        }

        public SvgDrawingCanvas()
        {
            _drawForInteractivity       = true;

            _appTitle                   = DefaultTitle;

            _drawObjects                = new List<Drawing>();
            _linkObjects                = new List<Drawing>();

            _displayTransform           = Transform.Identity;

            // Create a tooltip and set its position.
            _tooltip                    = new ToolTip();
            _tooltip.Placement          = PlacementMode.MousePoint;
            _tooltip.PlacementRectangle = new Rect(50, 0, 0, 0);
            _tooltip.HorizontalOffset   = 20;
            _tooltip.VerticalOffset     = 20;

            _tooltipText                = new TextBlock();
            _tooltipText.Text           = string.Empty;
            _tooltipText.Margin         = new Thickness(6, 0, 0, 0);

            //Create BulletDecorator and set it as the tooltip content.
            Ellipse bullet              = new Ellipse();
            bullet.Height               = 10;
            bullet.Width                = 10;
            bullet.Fill                 = Brushes.LightCyan;

            BulletDecorator decorator   = new BulletDecorator();
            decorator.Bullet            = bullet;
            decorator.Margin            = new Thickness(0, 0, 10, 0);
            decorator.Child             = _tooltipText;

            _tooltip.Content            = decorator;
            _tooltip.IsOpen             = false;
            _tooltip.Visibility         = Visibility.Hidden;

            //Finally, set tooltip on this canvas
            this.ToolTip                = _tooltip;
            this.Background             = Brushes.Transparent;

            _animationCanvas            = new SvgAnimationLayer(this);

            this.SnapsToDevicePixels    = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the application title, which is used to display the alert and error messages not handled
        /// by the user.
        /// </summary>
        /// <value>A string containg the application title. This cannot be <see langword="null"/> or empty. 
        /// The default is <c>SharpVectors</c>.</value>
        [DefaultValue(DefaultTitle)]
        [Description("The title of the application, used in displaying error and alert messages.")]
        public string AppTitle
        {
            get {
                return _appTitle;
            }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _appTitle = value;
                }
            }
        }

        /// <summary>
        /// Gets a value specifying whether the viewer control is in design-mode.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the viewer control is in design-mode, otherwise; it is <see langword="false"/>.
        /// </value>
        public bool DesignMode
        {
            get {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return true;
                }
                return false;
            }
        }

        public SvgAnimationLayer AnimationCanvas
        {
            get {
                return _animationCanvas;
            }
        }

        public Transform DisplayTransform
        {
            get {
                return _displayTransform;
            }
        }

        public Rect Bounds
        {
            get {
                return _bounds;
            }
        }

        public Point DisplayOffset
        {
            get {
                return new Point(_offsetX, _offsetY);
            }
        }

        public IList<Drawing> DrawObjects
        {
            get {
                return _drawObjects;
            }
        }

        public IList<Drawing> LinkObjects
        {
            get {
                return _linkObjects;
            }
        }

        public DrawingVisual HostVisual
        {
            get {
                return _hostVisual;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the interactive mode, which controls the level of information attached
        /// to the generated drawing.
        /// </summary>
        /// <value>An enumeration of the type <see cref="SvgInteractiveModes"/> specifying the interactive mode.
        /// The default is <see cref="SvgInteractiveModes.None"/>; no interactivity and may change in the future.</value>
        public SvgInteractiveModes InteractiveMode 
        {
            get {
                return _interactiveMode;
            }
            set {
                _interactiveMode = value;
            }
        }

        #endregion

        #region Public Events

        public event EventHandler<SvgAlertArgs> Alert
        {
            add { _svgAlerts += value; }
            remove { _svgAlerts -= value; }
        }

        public event EventHandler<SvgErrorArgs> Error
        {
            add { _svgErrors += value; }
            remove { _svgErrors -= value; }
        }

        #endregion

        #region Protected Properties

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get {
                if (_hostVisual != null)
                {
                    return 1;
                }
                return 0;
            }
        }

        #endregion

        #region Public Methods

        public void LoadDiagrams(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return;
            }

            this.UnloadDiagrams();

            string fileExt = IoPath.GetExtension(fileName);

            Cursor curCursor = this.Cursor;
            this.Cursor = Cursors.Wait;
            try
            {
                object xamlObject = null;

                if (string.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase))
                {
                    using (XmlReader xmlReader = XmlReader.Create(new StreamReader(fileName)))
                    {
                        xamlObject = XamlReader.Load(xmlReader);
                    }
                }
                else if (string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream fileStream = File.OpenRead(fileName))
                    {
                        using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            xamlObject = XamlReader.Load(zipStream);
                        }
                    }
                }

                if (xamlObject is SvgImageNameScope)
                {
                    SvgImageNameScope imageDrawing = (SvgImageNameScope)xamlObject;
                    RenderDiagrams(imageDrawing);
                }
                else if (xamlObject is DrawingGroup)
                {
                    DrawingGroup groupDrawing = (DrawingGroup)xamlObject;
                    RenderDiagrams(groupDrawing);
                }
            }
            finally
            {
                this.Cursor = curCursor;
            }
        }

        public void LoadDiagrams(DrawingGroup whole, DrawingGroup links, DrawingGroup main)
        {
            if (whole == null)
            {
                return;
            }

            this.UnloadDiagrams();

            this.Draw(whole, main);

            _wholeDrawing = whole;
            _linksDrawing = links;
            _mainDrawing = main;

            this.InvalidateMeasure();
        }

        public void UnloadDiagrams()
        {
            _offsetX = 0;
            _offsetY = 0;
            _bounds = new Rect(0, 0, 1, 1);

            _wholeDrawing = null;

            _displayTransform = Transform.Identity;

            this.ClearVisuals();
            this.ClearDrawings();

            this.InvalidateMeasure();
            //            this.InvalidateVisual();
            this.UpdateLayout();

            //            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            //            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }

        public void RenderDiagrams(SvgImageNameScope image)
        {
            DrawingImage drawingImage = image.Source as DrawingImage;
            if (drawingImage == null)
            {
                return;
            }

            DrawingGroup renderedGroup = drawingImage.Drawing as DrawingGroup;

            if (renderedGroup != null)
            {
                this.RenderDiagrams(renderedGroup);
            }
        }

        public void RenderDiagrams(DrawingGroup renderedGroup)
        {
            DrawingCollection drawings = renderedGroup.Children;
            int linkIndex = -1;
            int drawIndex = -1;
            for (int i = 0; i < drawings.Count; i++)
            {
                Drawing drawing = drawings[i];
                //string drawingName = SvgObject.GetName(drawing);
                string drawingName = SvgLink.GetKey(drawing);
                if (!string.IsNullOrWhiteSpace(drawingName) && string.Equals(drawingName, SvgObject.DrawLayer))
                {
                    drawIndex = i;
                }
                else if (!string.IsNullOrWhiteSpace(drawingName) &&
                    string.Equals(drawingName, SvgObject.LinksLayer))
                {
                    linkIndex = i;
                }
            }

            DrawingGroup mainGroups = null;
            if (drawIndex >= 0)
            {
                mainGroups = drawings[drawIndex] as DrawingGroup;
            }
            DrawingGroup linkGroups = null;
            if (linkIndex >= 0)
            {
                linkGroups = drawings[linkIndex] as DrawingGroup;
            }

            this.LoadDiagrams(renderedGroup, linkGroups, mainGroups);

            if (linkGroups != null)
            {
                _animationCanvas.LoadDiagrams(linkGroups, renderedGroup);
            }

            _bounds = _wholeDrawing.Bounds;

            this.InvalidateMeasure();
            this.InvalidateVisual();
        }

        #endregion

        #region Protected Methods

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (_hostVisual == null)
            {
                return null;
            }

            return _hostVisual;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_wholeDrawing != null)
            {
                Rect rectBounds = _wholeDrawing.Bounds;
                if (!rectBounds.IsEmpty)
                {
                    // Return the new size
                    double diaWidth = rectBounds.Width;
                    double diaHeight = rectBounds.Height;

                    _bounds = rectBounds;
                    return new Size(diaWidth, diaHeight);
                }
            }
            else
            {
                double dWidth  = this.Width;
                double dHeight = this.Height;
                if ((!Double.IsNaN(dWidth) && !Double.IsInfinity(dWidth)) &&
                    (!Double.IsNaN(dHeight) && !Double.IsInfinity(dHeight)))
                {
                    return new Size(dWidth, dHeight);
                }
            }

            var sizeCtrl = base.MeasureOverride(constraint);
            if ((!Double.IsNaN(sizeCtrl.Width) && !Double.IsInfinity(sizeCtrl.Width)) &&
                (!Double.IsNaN(sizeCtrl.Height) && !Double.IsInfinity(sizeCtrl.Height)))
            {
                if (sizeCtrl.Width != 0 && sizeCtrl.Height != 0)
                {
                    return sizeCtrl;
                }
            }

            return new Size(120, 120);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (_animationCanvas != null && _animationCanvas.HandleMouseDown(e))
            {
                return;
            }

            Point pt = e.GetPosition(this);

            Drawing visual = HitTest(pt);
            if (visual == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

//                this.Cursor = Cursors.Arrow;
                return;
            }

            string itemName = SvgObject.GetName(visual);
            if (itemName == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return;
            }
            //Brush brush = null;
            //if (_visualBrushes.ContainsKey(itemName))
            //{
            //    brush = _visualBrushes[itemName];
            //}
            //if (brush == null)
            //{
            //    if (_tooltip != null)
            //    {
            //        _tooltip.IsOpen = false;
            //        _tooltip.Visibility = Visibility.Hidden;
            //    }

            //    return;
            //}

            //if (e.ChangedButton == MouseButton.Left)
            //{
            //    string brushName = SvgObject.GetName(visual);
            //    if (!string.IsNullOrWhiteSpace(brushName))
            //    {
            //        SvgLinkAction linkAction = SvgLink.GetLinkAction(visual);
            //        if (linkAction == SvgLinkAction.LinkHtml ||
            //            linkAction == SvgLinkAction.LinkPage)
            //        {
            //            _animator.Start(brushName, brush);
            //        }
            //    }
            //}
            //else if (e.ChangedButton == MouseButton.Right)
            //{
            //    _animator.Stop();
            //}
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_animationCanvas != null && _animationCanvas.HandleMouseMove(e))
            {
                return;
            }

            // Retrieve the coordinates of the mouse button event.
            Point pt = e.GetPosition(this);

            Drawing hitVisual = HitTest(pt);

            //string itemName = null;

            if (hitVisual == null)
            {
//                this.Cursor = Cursors.Arrow;

                if (_hitVisual != null)
                {
                    //itemName = SvgObject.GetName(_hitVisual);
                    //if (itemName == null)
                    //{
                    //    _hitVisual = null;
                    //    return;
                    //}
                    //if (_visualBrushes.ContainsKey(itemName))
                    //{
                    //    Brush brush = _visualBrushes[itemName];
                    //    brush.Opacity = 0;
                    //}
                    _hitVisual = null;
                }

                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return;
            }
            else
            {
//                this.Cursor = Cursors.Hand;

                if (hitVisual == _hitVisual)
                {
                    return;
                }

                if (_hitVisual != null)
                {
                    //itemName = SvgObject.GetName(_hitVisual);
                    //if (itemName == null)
                    //{
                    //    _hitVisual = null;
                    //    return;
                    //}
                    //if (_visualBrushes.ContainsKey(itemName))
                    //{
                    //    Brush brush = _visualBrushes[itemName];
                    //    brush.Opacity = 0;
                    //}
                    _hitVisual = null;
                }

                //itemName = SvgObject.GetName(hitVisual);
                //if (itemName == null)
                //{
                //    return;
                //}
                //if (_visualBrushes.ContainsKey(itemName))
                //{
                //    Brush brush = _visualBrushes[itemName];
                //    brush.Opacity = 0.5;
                //}
                _hitVisual = hitVisual;

                string tooltipText = SvgObject.GetTitle(_hitVisual);
                Rect rectBounds = _hitVisual.Bounds;
                //Drawing drawing = _hitVisual.GetValue(FrameworkElement.TagProperty) as Drawing;
                //if (drawing != null)
                //{
                //    rectBounds  = drawing.Bounds; 
                //    tooltipText = SvgObject.GetTitle(drawing);
                //}

                if (_tooltip != null && !string.IsNullOrWhiteSpace(tooltipText))
                {
                    _tooltip.PlacementRectangle = rectBounds;

                    _tooltipText.Text = tooltipText;

                    if (_tooltip.Visibility == Visibility.Hidden)
                    {
                        _tooltip.Visibility = Visibility.Visible;
                    }

                    _tooltip.IsOpen = true;
                }
                else
                {
                    if (_tooltip != null)
                    {
                        _tooltip.IsOpen = false;
                        _tooltip.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_animationCanvas != null && _animationCanvas.HandleMouseLeave())
            {
                return;
            }

            if (_tooltip != null)
            {
                _tooltip.IsOpen = false;
                _tooltip.Visibility = Visibility.Hidden;
            }

            if (_hitVisual == null)
            {
                return;
            }

            string itemName = SvgObject.GetName(_hitVisual);
            if (itemName == null)
            {
                _hitVisual = null;
                return;
            }
            //if (_visualBrushes.ContainsKey(itemName))
            //{
            //    Brush brush = _visualBrushes[itemName];
            //    brush.Opacity = 0;
            //}
            _hitVisual = null;

//            this.Cursor = Cursors.Arrow;
        }

        protected virtual void OnHandleAlert(string message)
        {
            if (this.DesignMode)
            {
                return;
            }
            var alertArgs = new SvgAlertArgs(message);
            _svgAlerts?.Invoke(this, alertArgs);
            if (!alertArgs.Handled)
            {
                MessageBox.Show(alertArgs.Message, _appTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected virtual void OnHandleError(string message, Exception exception)
        {
            if (this.DesignMode)
            {
                return;
            }
            var errorArgs = new SvgErrorArgs(message, exception);
            _svgErrors?.Invoke(this, errorArgs);
            if (!errorArgs.Handled)
            {
                throw new SvgErrorException(errorArgs);
            }
        }

        #endregion

        #region Private Methods

        #region Visuals Methods

        private void AddVisual(DrawingVisual visual)
        {
            if (visual == null)
            {
                return;
            }

            _hostVisual = visual;

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        private void RemoveVisual(DrawingVisual visual)
        {
            if (visual == null)
            {
                return;
            }

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        private void ClearVisuals()
        {
            if (_hostVisual != null)
            {
                base.RemoveVisualChild(_hostVisual);
                base.RemoveLogicalChild(_hostVisual);

                _hostVisual = null;
            }
        }

        private void AddDrawing(Drawing visual)
        {
            if (visual == null)
            {
                return;
            }

            _drawObjects.Add(visual);
        }

        private void RemoveDrawing(Drawing visual)
        {
            if (visual == null)
            {
                return;
            }

            _drawObjects.Remove(visual);

            if (_linkObjects != null && _linkObjects.Count != 0)
            {
                _linkObjects.Remove(visual);
            }
        }

        private void ClearDrawings()
        {
            if (_drawObjects != null && _drawObjects.Count != 0)
            {
                _drawObjects.Clear();
            }

            if (_linkObjects != null && _linkObjects.Count != 0)
            {
                _linkObjects.Clear();
            }
        }

        #endregion

        #region Draw Methods

        private void Draw(DrawingGroup group, DrawingGroup main)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            _offsetX = 0;
            _offsetY = 0;
            _displayTransform = Transform.Identity;

            TranslateTransform offsetTransform = null;
            Rect rectBounds = group.Bounds;
            if (!rectBounds.IsEmpty)
            {
                // Return the new size
                //double diaWidth = rectBounds.Width;
                if (rectBounds.X > 0)
                {
                    //diaWidth += rectBounds.X;
                    _offsetX = rectBounds.X;
                }
                //double diaHeight = rectBounds.Height;
                if (rectBounds.Y > 0)
                {
                    //diaHeight += rectBounds.Y;
                    _offsetY = rectBounds.Y;
                }

                _bounds = rectBounds;

                if (!_offsetX.Equals(0) || !_offsetY.Equals(0))
                {
                    offsetTransform = new TranslateTransform(-_offsetX, -_offsetY);
                    _displayTransform = new TranslateTransform(_offsetX, _offsetY); // the inverse...
                }
            }

            //Canvas.SetTop(this, -_offsetX);
            //Canvas.SetLeft(this, -_offsetY);

            if (offsetTransform != null)
            {
                drawingContext.PushTransform(offsetTransform);
            }

            drawingContext.DrawDrawing(group);

            if (offsetTransform != null)
            {
                drawingContext.Pop();
            }

//            drawingVisual.Opacity = group.Opacity;

            //Transform transform = group.Transform;
            //if (transform == null)
            //{
            //    transform = offsetTransform;
            //}
            //if (offsetTransform != null)
            //{
            //    drawingVisual.Transform = offsetTransform;
            //}
            Geometry clipGeometry = group.ClipGeometry;
            if (clipGeometry != null)
            {
                drawingVisual.Clip = clipGeometry;
            }

            drawingContext.Close();

            this.AddVisual(drawingVisual);

            if (_drawForInteractivity)
            {
                if (main == null)
                {
                    main = group;
                }

                this.EnumerateDrawings(main);
            }
        }

        //private void Draw(DrawingGroup group)
        //{
        //    DrawingVisual drawingVisual = new DrawingVisual();

        //    DrawingContext drawingContext = drawingVisual.RenderOpen();

        //    if (_offsetTransform != null)
        //    {
        //        drawingContext.PushTransform(_offsetTransform);
        //    }

        //    drawingContext.DrawDrawing(group);

        //    if (_offsetTransform != null)
        //    {
        //        drawingContext.Pop();
        //    }

        //    drawingContext.DrawDrawing(group);
        //    drawingVisual.Opacity = group.Opacity;

        //    Transform transform = group.Transform;
        //    if (transform != null)
        //    {
        //        drawingVisual.Transform = transform;
        //    }
        //    Geometry clipGeometry = group.ClipGeometry;
        //    if (clipGeometry != null)
        //    {
        //        drawingVisual.Clip = clipGeometry;
        //    }

        //    drawingContext.Close();

        //    this.AddVisual(drawingVisual);

        //    if (_drawForInteractivity)
        //    {
        //        this.EnumerateDrawings(group);
        //    }
        //}

        private void EnumerateDrawings(DrawingGroup group)
        {
            if (group == null || group == _linksDrawing)
            {
                return;
            }

            DrawingCollection drawings = group.Children;
            for (int i = 0; i < drawings.Count; i++)
            {
                Drawing drawing = drawings[i];
                DrawingGroup childGroup = drawing as DrawingGroup;
                if (childGroup != null)
                {
                    SvgObjectType objectType = SvgObject.GetType(childGroup);
                    if (objectType == SvgObjectType.Link)
                    {
                        InsertLinkDrawing(childGroup);
                    }
                    else if (objectType == SvgObjectType.Text)
                    {
                        InsertTextDrawing(childGroup);
                    }
                    else
                    {
                        EnumerateDrawings(childGroup);
                    }
                }
                else
                {
                    InsertDrawing(drawing);
                }
            }
        }

        private void InsertTextDrawing(DrawingGroup group)
        {
            this.AddDrawing(group);
        }

        private void InsertLinkDrawing(DrawingGroup group)
        {
            this.AddDrawing(group);

            if (_linkObjects != null)
            {
                _linkObjects.Add(group);
            }
        }

        private void InsertDrawing(Drawing drawing)
        {
            this.AddDrawing(drawing);
        }

        #endregion

        #region HitTest Methods

        private Drawing HitTest(Point pt)
        {
            if (_linkObjects == null)
            {
                return null;
            }

            Point ptDisplay = _displayTransform.Transform(pt);

            DrawingGroup groupDrawing = null;
            GlyphRunDrawing glyRunDrawing = null;
            GeometryDrawing geometryDrawing = null;

            Drawing foundDrawing = null;

            //for (int i = 0; i < _linkObjects.Count; i++)
            for (int i = _linkObjects.Count - 1; i >= 0; i--)
            {
                Drawing drawing = _linkObjects[i];
                if (TryCast.Cast(drawing, out geometryDrawing))
                {
                    if (HitTestDrawing(geometryDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    if (HitTestDrawing(groupDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                    else if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text &&
                        groupDrawing.Bounds.Contains(ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
            }

            //if (_testHit != null)
            //{
            //    if (_testHitBrush != null)
            //    {
            //        _testHit.Brush = _testHitBrush;
            //    }
            //    else if (_testHitPen != null && _testHitBrushPen != null)
            //    {
            //        _testHit.Pen.Brush = _testHitBrushPen;
            //    }

            //    _testHit = null;
            //    _testHitPen = null;
            //    _testHitBrush = null;
            //}
            //if (_testHitGroup != null)
            //{
            //    _testHitGroup.BitmapEffect = null;
            //    _testHitGroup = null;
            //}

            //_testHit = foundDrawing as GeometryDrawing;
            //if (_testHit != null)
            //{
            //    _testHitBrush = _testHit.Brush;
            //    _testHitPen = _testHit.Pen;

            //    // Create and animate a Brush to set the button's Background.
            //    SolidColorBrush animationBrush = new SolidColorBrush();
            //    animationBrush.Color = Colors.Blue;

            //    ColorAnimation colorAnimation = new ColorAnimation();
            //    colorAnimation.From           = Colors.Blue;
            //    colorAnimation.To             = Colors.Red;
            //    colorAnimation.Duration       = new Duration(TimeSpan.FromMilliseconds(1000));
            //    colorAnimation.AutoReverse    = true;
            //    colorAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //    if (_testHitBrush != null)
            //    {
            //        _testHit.Brush = animationBrush;
            //    }
            //    else if (_testHitPen != null)
            //    {
            //        _testHitBrushPen  = _testHitPen.Brush;
            //        _testHitPen.Brush = animationBrush;
            //    }

            //    // Apply the animation to the brush's Color property.
            //    //animationBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            //}
            //else
            //{
            //    _testHitGroup = foundDrawing as DrawingGroup;
            //    if (_testHitGroup != null)
            //    {
            //        //// Create a blur effect.
            //        //BlurBitmapEffect blurEffect = new BlurBitmapEffect();
            //        //blurEffect.Radius = 3.0;

            //        //// Apply it to the drawing group.
            //        //_testHitGroup.BitmapEffect = blurEffect;

            //        // Initialize a new OuterGlowBitmapEffect that will be applied
            //        // to the TextBox.
            //        OuterGlowBitmapEffect glowEffect = new OuterGlowBitmapEffect();

            //        // Set the size of the glow to 30 pixels.
            //        glowEffect.GlowSize = 3;

            //        // Set the color of the glow to blue.
            //        Color glowColor = new Color();
            //        glowColor.ScA = 1;
            //        glowColor.ScB = 0;
            //        glowColor.ScG = 0;
            //        glowColor.ScR = 1;
            //        glowEffect.GlowColor = glowColor;

            //        // Set the noise of the effect to the maximum possible (range 0-1).
            //        glowEffect.Noise = 0;

            //        // Set the Opacity of the effect to 75%. Note that the same effect
            //        // could be done by setting the ScA property of the Color to 0.75.
            //        glowEffect.Opacity = 0.5;

            //        // Apply the bitmap effect to the TextBox.
            //        _testHitGroup.BitmapEffect = glowEffect;
            //    }
            //}   

            return foundDrawing;
        }

        private bool HitTestDrawing(GlyphRunDrawing drawing, Point pt)
        {
            if (drawing != null && drawing.Bounds.Contains(pt))
            {
                return true;
            }

            return false;
        }

        private bool HitTestDrawing(GeometryDrawing drawing, Point pt)
        {
            Pen pen = drawing.Pen;
            Brush brush = drawing.Brush;
            if (pen != null && brush == null)
            {
                if (drawing.Geometry.StrokeContains(pen, pt))
                {
                    return true;
                }
                else
                {
                    Geometry geometry = drawing.Geometry;

                    EllipseGeometry ellipse = null;
                    RectangleGeometry rectangle = null;
                    PathGeometry path = null;
                    if (TryCast.Cast(geometry, out ellipse))
                    {
                        if (ellipse.FillContains(pt))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(geometry, out rectangle))
                    {
                        if (rectangle.FillContains(pt))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(geometry, out path))
                    {
                        PathFigureCollection pathFigures = path.Figures;
                        int itemCount = pathFigures.Count;
                        if (itemCount == 1)
                        {
                            if (pathFigures[0].IsClosed && path.FillContains(pt))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            for (int f = 0; f < itemCount; f++)
                            {
                                PathFigure pathFigure = pathFigures[f];
                                if (pathFigure.IsClosed)
                                {
                                    PathFigureCollection testFigures = new PathFigureCollection();
                                    testFigures.Add(pathFigure);

                                    PathGeometry testPath = new PathGeometry();
                                    testPath.Figures = testFigures;

                                    if (testPath.FillContains(pt))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (brush != null && drawing.Geometry.FillContains(pt))
            {
                return true;
            }

            return false;
        }

        private bool HitTestDrawing(DrawingGroup group, Point pt)
        {
            if (group.Bounds.Contains(pt))
            {
                DrawingGroup groupDrawing = null;
                GlyphRunDrawing glyRunDrawing = null;
                GeometryDrawing geometryDrawing = null;
                DrawingCollection drawings = group.Children;

                for (int i = 0; i < drawings.Count; i++)
                {
                    Drawing drawing = drawings[i];
                    if (TryCast.Cast(drawing, out geometryDrawing))
                    {
                        if (HitTestDrawing(geometryDrawing, pt))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out groupDrawing))
                    {
                        if (HitTestDrawing(groupDrawing, pt))
                        {
                            return true;
                        }
                        SvgObjectType objectType = SvgObject.GetType(groupDrawing);
                        if (objectType == SvgObjectType.Text && groupDrawing.Bounds.Contains(pt))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out glyRunDrawing))
                    {
                        if (HitTestDrawing(glyRunDrawing, pt))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        private static bool IsValidBounds(Rect rectBounds)
        {
            if (rectBounds.IsEmpty || double.IsNaN(rectBounds.Width) || double.IsNaN(rectBounds.Height)
                || double.IsInfinity(rectBounds.Width) || double.IsInfinity(rectBounds.Height))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
