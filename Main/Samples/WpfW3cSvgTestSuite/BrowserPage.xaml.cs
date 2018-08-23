using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using IoPath = System.IO.Path;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Net;
using SharpVectors.Xml;
using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

using SharpVectors.Converters;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

using SharpVectors.Runtime;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for BrowserPage.xaml
    /// </summary>
    public partial class BrowserPage : Page, ITestPage
    {
        #region Private Fields

        private const int ImageWidth = 480;
        private const int ImageHeight = 360;

        private int _viewBoxWidth;
        private int _viewBoxHeight;

        private string _drawingDir;
        private DirectoryInfo _directoryInfo;

        private FileSvgReader _fileReader;
        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public BrowserPage()
        {
            InitializeComponent();

            _viewBoxWidth        = 0;
            _viewBoxHeight       = 0;
            _wpfSettings         = new WpfDrawingSettings();

            _fileReader          = new FileSvgReader(_wpfSettings);
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

            this.Loaded      += OnPageLoaded;
            this.SizeChanged += OnPageSizeChanged;
        }

        #endregion

        #region Public Properties

        public string XamlDrawingDir
        {
            get {
                return _drawingDir;
            }
            set {
                _drawingDir = value;

                if (!string.IsNullOrWhiteSpace(_drawingDir))
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

        public bool LoadDocument(string svgFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            this.UnloadDocument();

            if (extraInfo == null)
            {
                return false;
            }
            DrawingGroup drawing = extraInfo as DrawingGroup;
            if (drawing == null)
            {
                return false;
            }

            try
            {
                webBrowserBox.Navigate(svgFilePath);
            }
            catch
            {
                throw;
            }

            try
            {

                Rect drawingBounds = drawing.Bounds;

                //if (bitmap != null)
                //{
                //    drawing.ClipGeometry = new RectangleGeometry(
                //        new Rect(0, 0, bitmap.Width, bitmap.Height));
                //}

                //svgDrawing.Source = new DrawingImage(drawing);

                if (_viewBoxWidth == 0 || _viewBoxHeight == 0)
                {
                    _viewBoxWidth = (int)svgDrawing.ActualHeight;
                    _viewBoxHeight = (int)svgDrawing.ActualHeight;
                }

                svgDrawing.UnloadDiagrams();
                viewBox.Width = double.NaN;
                viewBox.Height = double.NaN;
                viewBox.RenderSize = new Size(_viewBoxWidth, _viewBoxHeight);

                svgDrawing.RenderDiagrams(drawing);

                //if (bitmap != null && ((int)drawingBounds.Width < ImageWidth || (int)drawingBounds.Height < ImageHeight))
                //{
                //    //SvgDrawingCanvas drawCanvas = svgDrawing.DrawingCanvas;
                //    viewBox.Width = bitmap.Width;
                //    viewBox.Height = bitmap.Height;
                //    //svgDrawing.Width = bitmap.Width;
                //    //svgDrawing.Height = bitmap.Height;

                //    //SvgZoomableCanvas zoomableCanvas = svgDrawing.ZoomableCanvas;
                //    ////zoomableCanvas.Width = bitmap.Width;
                //    ////zoomableCanvas.Height = bitmap.Height;

                //    //zoomableCanvas.FitWindow(new Size(bitmap.Width, bitmap.Height));
                //}
            }
            catch
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();

                throw;
            }

            return true;
        }

        public void UnloadDocument()
        {
            if (svgDrawing != null)
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();
            }

            webBrowserBox.Navigate("about:blank");
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

        #endregion
    }
}
