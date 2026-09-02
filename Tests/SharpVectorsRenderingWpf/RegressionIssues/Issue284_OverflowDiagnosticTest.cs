using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SharpVectors.Rendering.Wpf.Tests.RegressionIssues
{
    [TestFixture]
    public class Issue284_OverflowDiagnosticTest
    {
        private const string TestFileName = "Issue284_Overflow.svg";
        private static readonly string _diagnosticOutputPath = Path.Combine(
            Path.GetTempPath(), "Issue284_Overflow_Diagnostic.txt");

        /// <summary>
        /// Diagnostic: Inspect the drawing structure to understand what's being rendered
        /// </summary>
        [Test]
        [Explicit("Diagnostic: inspect drawing structure for Issue284")]
        public void Issue284_Overflow_InspectDrawingStructure()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, $"Data/{TestFileName}");

            FileAssert.Exists(testSvgPath, $"Test SVG file not found: {testSvgPath}");

            var settings = new WpfDrawingSettings { IncludeRuntime = true };

            // Act: Render to drawing
            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // Inspect and write to file
            using (var sw = new StreamWriter(_diagnosticOutputPath, false))
            {
                sw.WriteLine("===== Issue284 Drawing Structure Analysis =====");
                sw.WriteLine($"Root Bounds: {drawing.Bounds}");
                sw.WriteLine($"Root Children Count: {drawing.Children.Count}");
                InspectDrawingGroup(drawing, 0, sw);
            }

            TestContext.WriteLine($"Diagnostic output written to: {_diagnosticOutputPath}");
            Debug.WriteLine($"Diagnostic output written to: {_diagnosticOutputPath}");
        }

        private void InspectDrawingGroup(DrawingGroup group, int level = 0, StreamWriter sw = null)
        {
            string indent = new string(' ', level * 2);

            if (sw != null)
            {
                sw.WriteLine($"{indent}DrawingGroup:");
                sw.WriteLine($"{indent}  Bounds: {group.Bounds}");
                sw.WriteLine($"{indent}  Children: {group.Children.Count}");
                if (group.ClipGeometry != null)
                {
                    sw.WriteLine($"{indent}  *** CLIP GEOMETRY FOUND ***: {group.ClipGeometry.Bounds}");
                    sw.WriteLine($"{indent}      Clip Path: {group.ClipGeometry.GetType().Name}");
                }
                if (group.OpacityMask != null)
                    sw.WriteLine($"{indent}  OpacityMask: present");
                if (group.Transform != null)
                    sw.WriteLine($"{indent}  Transform: {group.Transform.GetType().Name} => {group.Transform}");

                int childIndex = 0;
                foreach (var child in group.Children)
                {
                    if (child is DrawingGroup childGroup)
                    {
                        sw.WriteLine($"{indent}  [Child {childIndex}] DrawingGroup");
                        InspectDrawingGroup(childGroup, level + 1, sw);
                    }
                    else if (child is GeometryDrawing geomDrawing)
                    {
                        sw.WriteLine($"{indent}  [Child {childIndex}] GeometryDrawing:");
                        sw.WriteLine($"{indent}      Bounds: {geomDrawing.Geometry?.Bounds}");
                        sw.WriteLine($"{indent}      Brush: {geomDrawing.Brush?.GetType().Name}");
                        if (geomDrawing.Brush is SolidColorBrush scb)
                            sw.WriteLine($"{indent}      BrushColor: {scb.Color}");
                        sw.WriteLine($"{indent}      Pen: {geomDrawing.Pen?.GetType().Name}");
                        if (geomDrawing.Geometry != null)
                        {
                            var boundsStr = geomDrawing.Geometry.Bounds.ToString();
                            if (boundsStr.Contains("-"))
                                sw.WriteLine($"{indent}      *** NEGATIVE COORDINATES FOUND IN GEOMETRY ***");
                        }
                    }
                    else
                    {
                        sw.WriteLine($"{indent}  [Child {childIndex}] {child.GetType().Name}");
                    }
                    childIndex++;
                }
            }
            else
            {
                Debug.WriteLine($"{indent}DrawingGroup:");
                Debug.WriteLine($"{indent}  Bounds: {group.Bounds}");
                Debug.WriteLine($"{indent}  Children: {group.Children.Count}");
                if (group.ClipGeometry != null)
                {
                    Debug.WriteLine($"{indent}  *** CLIP GEOMETRY FOUND ***: {group.ClipGeometry.Bounds}");
                    Debug.WriteLine($"{indent}      Clip Path: {group.ClipGeometry.GetType().Name}");
                }
                if (group.OpacityMask != null)
                    Debug.WriteLine($"{indent}  OpacityMask: present");
                if (group.Transform != null)
                    Debug.WriteLine($"{indent}  Transform: {group.Transform.GetType().Name} => {group.Transform}");

                int childIndex = 0;
                foreach (var child in group.Children)
                {
                    if (child is DrawingGroup childGroup)
                    {
                        Debug.WriteLine($"{indent}  [Child {childIndex}] DrawingGroup");
                        InspectDrawingGroup(childGroup, level + 1);
                    }
                    else if (child is GeometryDrawing geomDrawing)
                    {
                        Debug.WriteLine($"{indent}  [Child {childIndex}] GeometryDrawing:");
                        Debug.WriteLine($"{indent}      Bounds: {geomDrawing.Geometry?.Bounds}");
                        Debug.WriteLine($"{indent}      Brush: {geomDrawing.Brush?.GetType().Name}");
                        if (geomDrawing.Brush is SolidColorBrush scb)
                            Debug.WriteLine($"{indent}      BrushColor: {scb.Color}");
                        Debug.WriteLine($"{indent}      Pen: {geomDrawing.Pen?.GetType().Name}");
                        if (geomDrawing.Geometry != null)
                        {
                            var boundsStr = geomDrawing.Geometry.Bounds.ToString();
                            if (boundsStr.Contains("-"))
                                Debug.WriteLine($"{indent}      *** NEGATIVE COORDINATES FOUND IN GEOMETRY ***");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"{indent}  [Child {childIndex}] {child.GetType().Name}");
                    }
                    childIndex++;
                }
            }
        }

        /// <summary>
        /// Diagnostic: Count all geometry drawings and their bounds
        /// </summary>
        [Test]
        [Explicit("Diagnostic: analyze geometry drawings")]
        public void Issue284_Overflow_AnalyzeGeometries()
        {
            // Arrange
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? AppContext.BaseDirectory;
            string testSvgPath = Path.Combine(baseDirectory, $"Data/{TestFileName}");

            var settings = new WpfDrawingSettings { IncludeRuntime = false };

            DrawingGroup drawing = null;
            using (var reader = new FileSvgReader(settings))
            {
                drawing = reader.Read(testSvgPath);
            }

            // Collect all geometry drawings
            var geometries = CollectGeometries(drawing).ToList();

            using (var sw = new StreamWriter(Path.Combine(Path.GetTempPath(), "Issue284_Overflow_Geometries.txt"), false))
            {
                sw.WriteLine("===== Geometry Analysis =====");
                sw.WriteLine($"Total geometry drawings found: {geometries.Count}");

                if (geometries.Count > 0)
                {
                    foreach (var i in Enumerable.Range(0, geometries.Count))
                    {
                        var geom = geometries[i];
                        sw.WriteLine($"\nGeometry {i}:");
                        sw.WriteLine($"  Type: {geom.Geometry?.GetType().Name}");
                        sw.WriteLine($"  Bounds: {geom.Geometry?.Bounds}");
                        sw.WriteLine($"  Brush: {geom.Brush?.GetType().Name}");
                        if (geom.Brush is SolidColorBrush scb)
                        {
                            sw.WriteLine($"    Color: {scb.Color}");
                            sw.WriteLine($"    Opacity: {scb.Opacity}");
                        }
                        sw.WriteLine($"  IsFilled: {geom.Brush != null}");
                        sw.WriteLine($"  IsStroked: {geom.Pen != null}");
                    }
                }
                else
                {
                    sw.WriteLine("WARNING: No geometry drawings found!");
                }
            }

            TestContext.WriteLine("Geometry analysis written");
        }

        private IEnumerable<GeometryDrawing> CollectGeometries(Drawing drawing)
        {
            if (drawing is GeometryDrawing geomDrawing)
            {
                yield return geomDrawing;
            }
            else if (drawing is DrawingGroup drawGroup)
            {
                foreach (var child in drawGroup.Children)
                {
                    foreach (var geom in CollectGeometries(child))
                    {
                        yield return geom;
                    }
                }
            }
        }
    }
}
