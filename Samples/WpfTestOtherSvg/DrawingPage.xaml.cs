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

namespace WpfTestOtherSvg
{
    /// <summary>
    /// Interaction logic for DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Page
    {
        #region Public Fields

        public const string TemporalDirName = "_Drawings";

        public const int ImageWidth  = 350;
        public const int ImageHeight = 350;

        #endregion

        #region Private Fields

        private string _svgFilePath;
        private string _pngFilePath;

        private string _drawingDir;
        private DirectoryInfo _workingDir;
        private DirectoryInfo _directoryInfo;
        private MainWindow _mainWindow;

        #endregion

        #region Constructors and Destructor

        public DrawingPage()
        {
            InitializeComponent();

            string workDir = Path.Combine(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location), TemporalDirName);

            _workingDir = new DirectoryInfo(workDir);

            this.Loaded      += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Properties

        public string WorkingDrawingDir
        {
            get {
                return _drawingDir;
            }
            set {
                _drawingDir = value;

                if (!string.IsNullOrWhiteSpace(_drawingDir))
                {
                    _directoryInfo = new DirectoryInfo(_drawingDir);
                }
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

        public void PageSelected(bool isSelected)
        {
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

            DirectoryInfo workingDir = _workingDir;
            if (_directoryInfo != null)
            {
                workingDir = _directoryInfo;
            }

            this.UnloadDocument();

            var optionSettings = _mainWindow.OptionSettings;

            Size imageSize = new Size(0, 0);
            if (!LoadImage(pngFilePath, optionSettings, ref imageSize))
            {
                return false;
            }

            var wpfSettings = optionSettings.ConversionSettings;

            var _fileReader = new FileSvgReader(wpfSettings);
            _fileReader.SaveXaml = optionSettings.ShowOutputFile;
            _fileReader.SaveZaml = false;

            _svgFilePath = svgFilePath;

            string fileExt = Path.GetExtension(svgFilePath);

            if (string.Equals(fileExt, SvgConverter.SvgExt, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fileExt, SvgConverter.CompressedSvgExt, StringComparison.OrdinalIgnoreCase))
            {
                if (_fileReader != null)
                {
                    DrawingGroup drawing = _fileReader.Read(svgFilePath, workingDir);
                    if (drawing != null)
                    {
                        return LoadDrawing(imageSize, drawing);
                    }
                }
            }

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
                var pngDir = Path.GetDirectoryName(pngFilePath);
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

                svgImage.Width = double.NaN;
                svgImage.Height = double.NaN;
                svgImage.RenderSize = new Size(ImageWidth, ImageHeight);

                svgImage.Source = new DrawingImage(drawing);

//                if (((int)drawingBounds.Width < ImageWidth || (int)drawingBounds.Height < ImageHeight))
                {
                    svgImage.Width  = imageSize.Width;
                    svgImage.Height = imageSize.Height;
                }

                svgCanvas.Width = imageSize.Width + 10;
                svgCanvas.Height = imageSize.Height + 10;
            }
            catch (Exception ex)
            {
                svgImage.Source = null;
                Trace.TraceError(ex.ToString());
            }

            return true;
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

            _svgFilePath = null;
            _pngFilePath = null;
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
    }
}
