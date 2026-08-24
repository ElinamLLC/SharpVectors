using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This implements a value converter that enables the conversion of SVG files to 
    /// <see cref="BitmapImage"/> for use with <see cref="System.Windows.Controls.Image.Source"/> binding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This converter rasterizes SVG files to bitmap images at a fixed DPI (96 DPI by default),
    /// making them suitable for static icons and UI elements. The resulting bitmap is cached
    /// after the initial conversion.
    /// </para>
    /// <para>
    /// The SVG source file can be:
    /// <list type="bullet">
    /// <item>
    /// <description>From the web (HTTP/HTTPS URLs)</description>
    /// </item>
    /// <item>
    /// <description>From the local computer (relative or absolute paths)</description>
    /// </item>
    /// <item>
    /// <description>From embedded resources (pack:// URIs)</description>
    /// </item>
    /// <item>
    /// <description>Compressed SVG files (.svgz)</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// XAML Usage:
    /// <code>
    /// &lt;Window.Resources&gt;
    ///     &lt;converters:SvgToBitmapValueConverter x:Key="SvgToBitmapConverter" /&gt;
    /// &lt;/Window.Resources&gt;
    /// &lt;Image Source="{Binding SvgPath, Converter={StaticResource SvgToBitmapConverter}}" /&gt;
    /// </code>
    /// </para>
    /// </remarks>
    [MarkupExtensionReturnType(typeof(BitmapImage))]
    public sealed class SvgToBitmapValueConverter : SvgImageBase, IValueConverter
    {
        #region Private Fields

        private readonly UriTypeConverter _uriConverter;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgToBitmapValueConverter"/> 
        /// class with the default parameters.
        /// </summary>
        public SvgToBitmapValueConverter()
        {
            _uriConverter = new UriTypeConverter();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the value for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// This returns <see langword="this"/> to allow use as a converter in XAML.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var uriContext = serviceProvider?.GetService(typeof(IUriContext)) as IUriContext;
            if (uriContext != null)
            {
                this.BaseUri = uriContext.BaseUri;
            }

            return this;
        }

        /// <summary>
        /// Converts an SVG file path or URI to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">
        /// The SVG source file path or URI. Can be a string or Uri.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property (should be <see cref="BitmapImage"/>).
        /// </param>
        /// <param name="parameter">
        /// An optional converter parameter specifying an alternative SVG source path.
        /// If provided, this takes precedence over <paramref name="value"/>.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter (not used).
        /// </param>
        /// <returns>
        /// A <see cref="BitmapImage"/> if the conversion is successful; otherwise, 
        /// <see langword="null"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Uri inputUri = null;

                // Prefer parameter over value
                if (parameter != null)
                {
                    inputUri = ConvertToUri(parameter.ToString());
                }
                else if (value != null)
                {
                    inputUri = _uriConverter.ConvertFrom(value) as Uri;
                    if (inputUri == null)
                    {
                        inputUri = ConvertToUri(value.ToString());
                    }
                    else if (!inputUri.IsAbsoluteUri)
                    {
                        inputUri = ConvertToUri(value.ToString());
                    }
                }

                if (inputUri == null)
                {
                    return null;
                }

                Uri baseUri = this.BaseUri ?? new Uri("pack://application:,,,/");
                var svgSource = inputUri.IsAbsoluteUri ? inputUri : new Uri(baseUri, inputUri);
                return this.GetBitmap(svgSource);
            }
            catch
            {
                // Silently fail - return null on any exception
                // This matches the behavior of SvgImageConverterExtension
            }

            return null;
        }

        /// <summary>
        /// Not implemented for this converter.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets or sets the base URI for resolving relative paths.
        /// </summary>
        private Uri BaseUri { get; set; }

        /// <summary>
        /// Converts a string to a URI, handling pack:// and file paths.
        /// </summary>
        private Uri ConvertToUri(string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return null;
            }

            // Try direct URI creation
            if (Uri.TryCreate(inputPath, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                return uri;
            }

            // Try as pack URI
            if (!inputPath.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
            {
                string packUri = "pack://application:,,,/" + inputPath.TrimStart('/');
                if (Uri.TryCreate(packUri, UriKind.Absolute, out Uri result))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the SVG source file to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="svgSource">
        /// A <see cref="Uri"/> specifying the source of the SVG resource.
        /// </param>
        /// <returns>
        /// A <see cref="BitmapImage"/> of the converted SVG resource, or 
        /// <see langword="null"/> if conversion fails.
        /// </returns>
        private BitmapImage GetBitmap(Uri svgSource)
        {
            DrawingGroup drawGroup = this.GetDrawing(svgSource);
            if (drawGroup == null)
            {
                return null;
            }

            try
            {
                // Get the bounds of the drawing
                Rect drawingBounds = drawGroup.Bounds;
                if (drawingBounds.IsEmpty || drawingBounds.Width <= 0 || drawingBounds.Height <= 0)
                {
                    // Default to a reasonable size if bounds are not available
                    int pixelWidth = 96;
                    int pixelHeight = 96;

                    return this.RasterizeBitmap(drawGroup, pixelWidth, pixelHeight);
                }

                // Use DPI-aware sizing (96 DPI)
                int width = Math.Max(1, (int)Math.Ceiling(drawingBounds.Width));
                int height = Math.Max(1, (int)Math.Ceiling(drawingBounds.Height));

                return this.RasterizeBitmap(drawGroup, width, height);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rasterizes a DrawingGroup to a BitmapImage at the specified dimensions.
        /// </summary>
        /// <param name="drawingGroup">The drawing group to rasterize.</param>
        /// <param name="pixelWidth">The width in pixels.</param>
        /// <param name="pixelHeight">The height in pixels.</param>
        /// <returns>A BitmapImage, or null if rasterization fails.</returns>
        private BitmapImage RasterizeBitmap(DrawingGroup drawingGroup, int pixelWidth, int pixelHeight)
        {
            const double dpi = 96.0; // Standard WPF DPI

            // Create a DrawingVisual to render the drawing
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(drawingGroup);
            }

            // Render to a bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);

            // Optionally freeze to improve performance
            if (renderBitmap.CanFreeze)
            {
                renderBitmap.Freeze();
            }

            // Convert to BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            MemoryStream memoryStream = new MemoryStream();
            encoder.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.None;

            try
            {
                bitmapImage.EndInit();
            }
            catch
            {
                memoryStream?.Dispose();
                return null;
            }

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        #endregion
    }
}
