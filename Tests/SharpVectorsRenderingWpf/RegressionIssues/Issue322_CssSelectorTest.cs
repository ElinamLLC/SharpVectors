using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class Issue322_CssSelectorTest
    {
        [TestCase(".hyperlinks:not (:hover) { transition-delay: 1s; }", "", 0, 0, 0)]
        [TestCase(".hyperlinks:not (:hover) { fill: red; }", " class='hyperlinks'", 0, 0, 0)]
        [TestCase(".hyperlinks:not (:hover) { fill: red; } rect { fill: blue; }", " class='hyperlinks'", 0, 0, 255)]
        public void MalformedSelector_DoesNotPreventRectangleFill(string css, string attributes, byte red, byte green, byte blue)
        {
            string svg = "<svg xmlns='http://www.w3.org/2000/svg' width='10' height='10'><style>" + css +
                "</style><rect width='10' height='10'" + attributes + "/></svg>";
            DrawingGroup drawing;
            using (var reader = new FileSvgReader(new WpfDrawingSettings()))
            using (var input = new StringReader(svg))
            {
                drawing = reader.Read(input);
            }

            Assert.That(drawing, Is.Not.Null);
            var geometry = CollectGeometries(drawing).Single();
            Assert.That(geometry.Geometry.Bounds.Width, Is.EqualTo(10));
            Assert.That(geometry.Geometry.Bounds.Height, Is.EqualTo(10));
            Assert.That(geometry.Brush, Is.TypeOf<SolidColorBrush>());
            Assert.That(((SolidColorBrush)geometry.Brush).Color, Is.EqualTo(Color.FromRgb(red, green, blue)));
        }

        private static IEnumerable<GeometryDrawing> CollectGeometries(Drawing drawing)
        {
            if (drawing is GeometryDrawing geometry)
            {
                yield return geometry;
            }
            else if (drawing is DrawingGroup group)
            {
                foreach (Drawing child in group.Children)
                {
                    foreach (var childGeometry in CollectGeometries(child))
                    {
                        yield return childGeometry;
                    }
                }
            }
        }
    }
}
