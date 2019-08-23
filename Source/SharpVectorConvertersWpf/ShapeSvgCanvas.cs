using System;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Utils;
using SharpVectors.Converters.Shapes;

namespace SharpVectors.Converters
{
    /// <summary>
    /// Component that visualizes svg document contents.
    /// </summary>
    public class ShapeSvgCanvas : Canvas
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(string), typeof(ShapeSvgCanvas), new PropertyMetadata(OnSourcePropertyChanged));

        public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register("ItemStyle",
            typeof(Style), typeof(ShapeSvgCanvas), new PropertyMetadata(OnItemStylePropertyChanged));

        private WpfSvgWindow _wpfWindow;
        private WpfShapeRenderer _wpfRenderer;

        public ShapeSvgCanvas()
        {
            _wpfRenderer        = new WpfShapeRenderer();
            _wpfRenderer.Canvas = this;
            _wpfWindow          = new WpfSvgWindow(640, 480, _wpfRenderer);
            this.ClipToBounds   = true;
            this.Bounds         = Rect.Empty;
        }

        public Canvas Drawing
        {
            get {
                if (this.Children.Count != 0)
                {
                    return this.Children[0] as Canvas;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets svg document source.
        /// </summary>
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets an item style that will be applied to all svg elements.
        /// </summary>
        /// <remarks>
        /// This style will be used as a BasedOn style for all shapes created from
        /// svg elements.
        /// </remarks>
        public Style ItemStyle
        {
            get { return (Style)GetValue(ItemStyleProperty); }
            set { SetValue(ItemStyleProperty, value); }
        }

        /// <summary>
        /// Gets the bounding box of the svg document.
        /// </summary>
        public Rect Bounds { get; private set; }

        /// <summary>
        /// Unloads loaded svg.
        /// </summary>
        public void UnloadDiagrams()
        {
            this.Children.Clear();
            this.Bounds = Rect.Empty;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var baseSize = base.MeasureOverride(constraint);
            if (_wpfWindow.Document?.RootElement == null)
            {
                return baseSize;
            }

            var rect = this.Bounds;
            if (!rect.IsEmpty)
            {
                return new Size(rect.Width, rect.Height);
            }

            return baseSize;
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ShapeSvgCanvas).Load(e.NewValue as string);
        }

        private static void OnItemStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ShapeSvgCanvas)._wpfRenderer.ItemStyle = e.NewValue as Style;
        }

        public bool RenderDiagrams(string sourceUri)
        {
            return this.Load(sourceUri);
        }

        private bool Load(string uri)
        {
            UnloadDiagrams();

            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }

            try
            {
                _wpfWindow.LoadDocument(uri, null);
                _wpfRenderer.Render(_wpfWindow.Document);
                this.Bounds = CalculateBounds(_wpfWindow);

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                UnloadDiagrams();

                return false;
            }
        }

        private static Rect CalculateBounds(WpfSvgWindow window)
        {
            if (window.Document?.RootElement == null)
                return Rect.Empty;

            var rect = window.Document.RootElement.GetBBox();
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}