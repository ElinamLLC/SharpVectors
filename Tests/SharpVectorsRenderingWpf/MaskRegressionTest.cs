using System;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests
{
    [TestFixture]
    public class MaskRegressionTest
    {
        private WpfDrawingSettings GetSettings()
        {
            var settings = new WpfDrawingSettings();
            settings.TextAsGeometry = true;
            return settings;
        }

        [Test]
        public void Issue288_Gear_ShouldRenderWithMasks()
        {
            var reader = new FileSvgReader(GetSettings());
            var drawing = reader.Read("Data/Issue288_gear-svgrepo-com.svg");

            // The drawing should not be empty
            Assert.IsNotNull(drawing, "Drawing should not be null");
            Assert.IsFalse(drawing.Bounds.IsEmpty, "Drawing bounds should not be empty");
            Assert.Greater(drawing.Bounds.Width, 0, "Drawing width should be greater than 0");
            Assert.Greater(drawing.Bounds.Height, 0, "Drawing height should be greater than 0");

            // Log the structure
            Console.WriteLine($"Drawing bounds: {drawing.Bounds}");
            Console.WriteLine($"Drawing children: {drawing.Children.Count}");

            // The mask should result in some rendered content
            Assert.Greater(drawing.Children.Count, 0, "Drawing should have child elements");
        }

        [Test]
        public void Issue288_Heart_ShouldRenderWithMasks()
        {
            var reader = new FileSvgReader(GetSettings());
            var drawing = reader.Read("Data/Issue288_heart-svgrepo-com.svg");

            Assert.IsNotNull(drawing, "Drawing should not be null");
            Assert.IsFalse(drawing.Bounds.IsEmpty, "Drawing bounds should not be empty");
            Assert.Greater(drawing.Bounds.Width, 0, "Drawing width should be greater than 0");
            Assert.Greater(drawing.Bounds.Height, 0, "Drawing height should be greater than 0");

            Console.WriteLine($"Drawing bounds: {drawing.Bounds}");
            Console.WriteLine($"Drawing children: {drawing.Children.Count}");

            Assert.Greater(drawing.Children.Count, 0, "Drawing should have child elements");
        }

        [Test]
        public void Issue288_Mail_ShouldRenderWithMasks()
        {
            var reader = new FileSvgReader(GetSettings());
            var drawing = reader.Read("Data/Issue288_mail-svgrepo-com.svg");

            Assert.IsNotNull(drawing, "Drawing should not be null");
            Assert.IsFalse(drawing.Bounds.IsEmpty, "Drawing bounds should not be empty");

            Console.WriteLine($"Drawing bounds: {drawing.Bounds}");
            Console.WriteLine($"Drawing children: {drawing.Children.Count}");

            Assert.Greater(drawing.Children.Count, 0, "Drawing should have child elements");
        }

        [Test]
        public void Issue288_Image_ShouldRenderWithMasks()
        {
            var reader = new FileSvgReader(GetSettings());
            var drawing = reader.Read("Data/Issue288_image-svgrepo-com.svg");

            Assert.IsNotNull(drawing, "Drawing should not be null");
            Assert.IsFalse(drawing.Bounds.IsEmpty, "Drawing bounds should not be empty");

            Console.WriteLine($"Drawing bounds: {drawing.Bounds}");
            Console.WriteLine($"Drawing children: {drawing.Children.Count}");
        }

        [Test]
        public void Issue288_Wrench_ShouldRenderWithMasks()
        {
            var reader = new FileSvgReader(GetSettings());
            var drawing = reader.Read("Data/Issue288_wrench-svgrepo-com.svg");

            Assert.IsNotNull(drawing, "Drawing should not be null");
            Assert.IsFalse(drawing.Bounds.IsEmpty, "Drawing bounds should not be empty");

            Console.WriteLine($"Drawing bounds: {drawing.Bounds}");
            Console.WriteLine($"Drawing children: {drawing.Children.Count}");
        }
    }
}
