using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Converters.Shapes
{
    using static SharpVectors.Converters.Shapes.WpfShapeHelper;

    public class ShapeRenderingVisitor : IElementVisitor
    {
        private readonly WpfShapeRenderer renderer;
        private Canvas currentCanvas;

        public ShapeRenderingVisitor(WpfShapeRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void BeginContainer()
        {
            if (currentCanvas == null)
            {
                currentCanvas = this.renderer.Canvas;
            }

            var newCanvas = new Canvas();
            currentCanvas.Children.Add(newCanvas);
            currentCanvas = newCanvas;
        }

        public void EndContainer()
        {
            currentCanvas = currentCanvas?.Parent as Canvas;
        }

        public void BeginContainer(ISvgElement element)
        {
            if (element is ISvgSvgElement)
            {
                if (currentCanvas == null)
                {
                    currentCanvas = this.renderer.Canvas;
                }
            }
            else if (element is ISvgGElement)
            {
                var newCanvas = new Canvas();
                currentCanvas.Children.Add(newCanvas);
                currentCanvas = newCanvas;
            }
        }

        public void EndContainer(ISvgElement element)
        {
            currentCanvas = currentCanvas?.Parent as Canvas;
        }

        public void Visit(ISvgCircleElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _r = Math.Round(element.R.AnimVal.Value, 4);

            if (_r <= 0)
            {
                return;
            }

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy), _r, _r);
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgEllipseElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _rx = Math.Round(element.Rx.AnimVal.Value, 4);
            double _ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (_rx <= 0 || _ry <= 0)
            {
                return;
            }

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy),
                _rx, _ry);
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgLineElement element)
        {
            double x1 = element.X1.AnimVal.Value;
            double y1 = element.Y1.AnimVal.Value;
            double x2 = element.X2.AnimVal.Value;
            double y2 = element.Y2.AnimVal.Value;

            var geometry = new LineGeometry(new Point(x1, y1), new Point(x2, y2));
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgPathElement element)
        {
            SvgPathElement pe = element as SvgPathElement;
            if (pe == null || string.IsNullOrEmpty(pe.PathScript))
                return;

            var geometry = new PathGeometry();

            FillRule fillRule;
            if (TryGetFillRule(pe, out fillRule))
                geometry.FillRule = fillRule;

            try
            {
                geometry.Figures = PathFigureCollection.Parse(pe.PathScript);
            }
            catch
            {
                return;
            }

            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgPolygonElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            SvgPolygonElement pe = element as SvgPolygonElement;
            if (nElems == 0 || pe == null)
            {
                return;
            }

            PointCollection points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                ISvgPoint point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }

            PolyLineSegment polyline = new PolyLineSegment();
            polyline.Points = points;

            PathFigure polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed = true;
            polylineFigure.IsFilled = true;

            polylineFigure.Segments.Add(polyline);

            PathGeometry geometry = new PathGeometry();

            FillRule fillRule;
            if (TryGetFillRule(pe, out fillRule))
                geometry.FillRule = fillRule;

            geometry.Figures.Add(polylineFigure);
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgPolylineElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            SvgPolylineElement pe = element as SvgPolylineElement;
            if (nElems == 0 || pe == null)
            {
                return;
            }

            PointCollection points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                ISvgPoint point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }
            PolyLineSegment polyline = new PolyLineSegment();
            polyline.Points = points;

            PathFigure polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed = false;
            polylineFigure.IsFilled = true;

            polylineFigure.Segments.Add(polyline);

            PathGeometry geometry = new PathGeometry();

            FillRule fillRule;
            if (TryGetFillRule(pe, out fillRule))
                geometry.FillRule = fillRule;

            geometry.Figures.Add(polylineFigure);
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgRectElement element)
        {
            double dx = element.X.AnimVal.Value;
            double dy = element.Y.AnimVal.Value;
            double width = element.Width.AnimVal.Value;
            double height = element.Height.AnimVal.Value;
            double rx = element.Rx.AnimVal.Value;
            double ry = element.Ry.AnimVal.Value;

            if (width <= 0 || height <= 0)
            {
                return;
            }
            if (rx <= 0 && ry > 0)
            {
                rx = ry;
            }
            else if (rx > 0 && ry <= 0)
            {
                ry = rx;
            }

            var geometry = new RectangleGeometry(new Rect(dx, dy, width, height), rx, ry);
            var shape = WrapGeometry(geometry, element);
            DisplayShape(shape, element);
        }

        public void Visit(ISvgImageElement element)
        {
        }

        public void Visit(ISvgUseElement element)
        {
            SvgUseElement useElement = (SvgUseElement)element;
            SvgDocument document = useElement.OwnerDocument;
            XmlElement refEl = useElement.ReferencedElement;
            if (refEl == null)
                return;
            XmlElement refElParent = (XmlElement)refEl.ParentNode;

            bool isImported = false;
            // For the external node, the documents are different, and we may not be
            // able to insert this node, so we first import it...
            if (useElement.OwnerDocument != refEl.OwnerDocument)
            {
                var importedNode = useElement.OwnerDocument.ImportNode(refEl, true) as XmlElement;

                if (importedNode != null)
                {
                    var importedSvgElement = importedNode as SvgElement;
                    if (importedSvgElement != null)
                    {
                        importedSvgElement.Imported = true;
                        importedSvgElement.ImportNode = refEl as SvgElement;
                        importedSvgElement.ImportDocument = refEl.OwnerDocument as SvgDocument;
                    }

                    refEl = importedNode;
                    isImported = true;
                }
            }
            useElement.OwnerDocument.Static = true;
            useElement.CopyToReferencedElement(refEl);
            if (!isImported) // if imported, we do not need to remove it...
            {
                refElParent.RemoveChild(refEl);
            }
            useElement.AppendChild(refEl);

            // Now, render the use element...
            var refElement = useElement?.FirstChild;
            IElementVisitorTarget evt = refElement as IElementVisitorTarget;
            if (evt != null)
            {
                evt.Accept(this);
            }

            useElement.RemoveChild(refEl);
            useElement.RestoreReferencedElement(refEl);
            if (!isImported)
            {
                refElParent.AppendChild(refEl);
            }
            useElement.OwnerDocument.Static = false;
        }

        public void Visit(ISvgAElement element)
        {
        }

        public void Visit(ISvgGElement element)
        {
        }

        public void Visit(ISvgSvgElement element)
        {
        }

        public void Visit(ISvgSwitchElement element)
        {
        }

        public void Visit(ISvgSymbolElement element)
        {
        }

        public void Visit(ISvgTextElement element)
        {
            Point position = GetCurrentTextPosition(element as SvgTextPositioningElement, new Point(0, 0));
            Size spanSize;
            foreach (var child in element.ChildNodes)
            {
                Geometry geometry;
                Path shape;
                spanSize = new Size(0, 0);
                SvgTSpanElement tspan;
                Dom.Text simpleText;
                if (TryCast.Cast(child, out tspan))
                {
                    geometry = ConstructTextGeometry(tspan, tspan.InnerText, position, out spanSize);
                    shape = WrapGeometry(geometry, tspan);
                    shape.IsHitTestVisible = false;
                    DisplayShape(shape, tspan);
                }
                else if (TryCast.Cast(child, out simpleText))
                {
                    geometry = ConstructTextGeometry(element as SvgTextBaseElement, 
                        simpleText.InnerText, position, out spanSize);
                    shape = WrapGeometry(geometry, element);
                    shape.IsHitTestVisible = false;
                    DisplayShape(shape, element);
                }

                position.Offset(spanSize.Width, 0);
            }
        }

        public void Visit(ISvgTextPathElement element)
        {
        }

        public void Visit(ISvgTSpanElement element)
        {
        }

        private Path WrapGeometry(Geometry geometry, ISvgElement element)
        {
            Path path = new Path();
            Transform transform;
            if (TryGetTransform(element as ISvgTransformable, out transform))
                geometry.Transform = transform;
            path.Data = geometry;
            return path;
        }

        private void DisplayShape(Path shape, ISvgElement element, bool applyStyle = true)
        {
            if (currentCanvas == null)
                return;

            if (applyStyle)
            {
                var style = CreateStyle(shape, element as SvgStyleableElement);
                if (style != null)
                    shape.Style = style;
            }
            Geometry geom = shape.Data;
            if (geom != null && geom.CanFreeze && !geom.IsFrozen)
                geom.Freeze();
            currentCanvas.Children.Add(shape);
        }

        private Style CreateStyle(Path shape, SvgStyleableElement element)
        {
            if (element == null)
                return null;

            Style style = new Style();
            style.BasedOn = this.renderer.ItemStyle;

            style.Setters.Add(new Setter(UIElement.SnapsToDevicePixelsProperty, true));

            Rect shapeBounds = Rect.Empty;
            Matrix shapeTransform = Matrix.Identity;
            if (shape.Data != null)
            {
                if (shape.Data.Transform?.Value.IsIdentity == true)
                {
                    shapeBounds = shape.Data.Bounds;
                }
                else
                {
                    var transform = shape.Data.Transform;
                    shape.Data.Transform = null;
                    shapeBounds = shape.Data.Bounds;
                    shape.Data.Transform = transform;
                    shapeTransform = transform.Value;
                }
            }

            // Stroke
            Brush stroke;
            if (TryGetBrush(element, "stroke", shapeBounds, shapeTransform, out stroke))
                style.Setters.Add(new Setter(Shape.StrokeProperty, stroke));

            double strokeWidth;
            if (TryGetStrokeWidth(element, out strokeWidth))
                style.Setters.Add(new Setter(Shape.StrokeThicknessProperty, strokeWidth));

            double miterLimit;
            if (TryGetMiterLimit(element, strokeWidth, out miterLimit))
                style.Setters.Add(new Setter(Shape.StrokeMiterLimitProperty, miterLimit));

            PenLineJoin lineJoin;
            if (TryGetLineJoin(element, out lineJoin))
                style.Setters.Add(new Setter(Shape.StrokeLineJoinProperty, lineJoin));

            PenLineCap lineCap;
            if (TryGetLineCap(element, out lineCap))
            {
                style.Setters.Add(new Setter(Shape.StrokeStartLineCapProperty, lineCap));
                style.Setters.Add(new Setter(Shape.StrokeEndLineCapProperty, lineCap));
            }

            DoubleCollection dashArray;
            if (TryGetDashArray(element, strokeWidth, out dashArray))
                style.Setters.Add(new Setter(Shape.StrokeDashArrayProperty, dashArray));

            double dashOffset;
            if (TryGetDashOffset(element, out dashOffset))
                style.Setters.Add(new Setter(Shape.StrokeDashOffsetProperty, dashOffset));

            // Fill
            Brush fill;
            if (TryGetBrush(element, "fill", shapeBounds, shapeTransform, out fill))
                style.Setters.Add(new Setter(Shape.FillProperty, fill));

            return style;
        }
    }
}
