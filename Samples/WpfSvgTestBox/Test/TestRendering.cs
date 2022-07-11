using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpVectors.Dom.Svg;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Test.Sample
{
    public class TestXamlOutput
    {
        // The drawing settings or options.
        private WpfDrawingSettings _wpfSettings;

        // The browser window object abstraction.
        private WpfSvgWindow _wpfWindow;

        // The main rendering controller.
        private WpfDrawingRenderer _wpfRenderer;

        public TestXamlOutput()
        {
            // Initialize the options
            _wpfSettings = new WpfDrawingSettings();
            // Set the options to the rendering controller
            _wpfRenderer = new WpfDrawingRenderer(_wpfSettings);
            // Create a drawing area of 500x500 pixels and set the rending object to it.
            _wpfWindow   = new WpfSvgWindow(500, 500, _wpfRenderer);
        }

        public string SvgToXaml(string svgFile)
        {
            // Create an instance of the rendered document
            var drawingDocument = new WpfDrawingDocument();

            // Signal the start of the rendering process, the drawing context is created.
            _wpfRenderer.BeginRender(drawingDocument);

            // Open the specified SVG file for drawin, the SVG DOM is created as SvgDocument
            _wpfWindow.LoadDocument(svgFile, _wpfSettings);

            // Get the created SVG document
            var svgDocument = _wpfWindow.Document as SvgDocument;

            // Render the SVG document
            _wpfRenderer.Render(svgDocument);

            // Retrived the rendered drawing, and do something with it
            DrawingGroup drawing = _wpfRenderer.Drawing;

            // Signal an end to the rendering process
            _wpfRenderer.EndRender();

            // Create an instance of the XAML writer
            var xamlWriter = new XmlXamlWriter();

            return xamlWriter.Save(drawing);
        }
    }
}
