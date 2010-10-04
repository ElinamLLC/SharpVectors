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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

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
    /// Interaction logic for DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Page
    {
        #region Private Fields

        private string _drawingDir;
        private DirectoryInfo _directoryInfo;

        private FileSvgReader _fileReader;

        #endregion

        #region Constructors and Destructor

        public DrawingPage()
        {
            InitializeComponent();

            _fileReader          = new FileSvgReader();
            _fileReader.SaveXaml = false;
            _fileReader.SaveZaml = false;

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

        public void LoadDocument(string svgFilePath, string pngFilePath)
        {
            this.UnloadDocument();

            BitmapImage bitmap = null;
            try
            {
                bitmap = new BitmapImage(new Uri(pngFilePath));
                pngResult.Source = bitmap;
            }
            catch
            {
                throw;
            }

            try
            {
                DrawingGroup drawing = _fileReader.Read(svgFilePath, _directoryInfo);

                //if (bitmap != null)
                //{
                //    drawing.ClipGeometry = new RectangleGeometry(
                //        new Rect(0, 0, bitmap.Width, bitmap.Height));
                //}

                //svgDrawing.Source = new DrawingImage(drawing);

                svgDrawing.UnloadDiagrams();

                svgDrawing.RenderDiagrams(drawing);

                if (bitmap != null)
                {
                    //SvgDrawingCanvas drawCanvas = svgDrawing.DrawingCanvas;
                    viewBox.Width = bitmap.Width;
                    viewBox.Height = bitmap.Height;
                    //svgDrawing.Width = bitmap.Width;
                    //svgDrawing.Height = bitmap.Height;

                    //SvgZoomableCanvas zoomableCanvas = svgDrawing.ZoomableCanvas;
                    ////zoomableCanvas.Width = bitmap.Width;
                    ////zoomableCanvas.Height = bitmap.Height;

                    //zoomableCanvas.FitWindow(new Size(bitmap.Width, bitmap.Height));
                }
            }
            catch
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();

                throw;
            }
        }

        public void UnloadDocument()
        {
            if (svgDrawing != null)
            {
                //svgDrawing.Source = null;
                svgDrawing.UnloadDiagrams();
            }
            if (pngResult != null)
            {
                pngResult.Source = null;
            }
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
            rowTab.Height = new GridLength(this.ActualHeight / 2, GridUnitType.Pixel);
        }

        #endregion
    }
}
