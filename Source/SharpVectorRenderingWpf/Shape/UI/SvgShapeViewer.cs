using SharpVectors.Renderers.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpVectors.Renderers.Wpf.Shape
{
    /// <summary>
    /// Component that visualizes svg document contents.
    /// </summary>
    public class SvgShapeViewer : Canvas
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(string), typeof(SvgShapeViewer), new PropertyMetadata(OnSourcePropertyChanged));

        public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register("ItemStyle",
            typeof(Style), typeof(SvgShapeViewer), new PropertyMetadata(OnItemStylePropertyChanged));

        public SvgShapeViewer()
        {
            _wpfRenderer = new WpfShapeRenderer();
            _wpfRenderer.Canvas = this;
            _wpfWindow = new WpfSvgWindow(640, 480, _wpfRenderer);
            ClipToBounds = true;
            Bounds = Rect.Empty;
        }

        private WpfSvgWindow _wpfWindow;
        private WpfShapeRenderer _wpfRenderer;

        /// <summary>
        /// Gets or sets svg document source.
        /// </summary>
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
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
            get => (Style)GetValue(ItemStyleProperty);
            set => SetValue(ItemStyleProperty, value);
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
            return new Size(rect.Width, rect.Height);
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as SvgShapeViewer).Load(e.NewValue as string);
        }

        private static void OnItemStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as SvgShapeViewer)._wpfRenderer.ItemStyle = e.NewValue as Style;
        }

        private void Load(string uri)
        {
            UnloadDiagrams();

            if (string.IsNullOrEmpty(uri))
                return;

            try
            {
                _wpfWindow.LoadDocument(uri);
                _wpfRenderer.Render(_wpfWindow.Document);
                this.Bounds = CalculateBounds(_wpfWindow);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                UnloadDiagrams();
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