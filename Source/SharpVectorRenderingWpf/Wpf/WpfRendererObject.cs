using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public abstract class WpfRendererObject : DependencyObject, IDisposable
    {
        #region Private Fields

        private static readonly Regex _regExCaps = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Protected Fields

        protected bool _isDisposed; // To detect redundant calls
        protected WpfDrawingContext _context;

        // TODO-PAUL: Consider the possibility of providing this as option!
        protected bool _flattenClosedPath; // Experiental options
        protected double _flattenTolerance;
        protected ToleranceType _flattenToleranceType;

        #endregion

        #region Constructors and Destructor

        protected WpfRendererObject()
        {
            _flattenClosedPath    = false;
            _flattenTolerance     = 0.0022;
            _flattenToleranceType = ToleranceType.Relative;
        }

        protected WpfRendererObject(WpfDrawingContext context)
            : this()
        {
            _context = context;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~WpfRendererObject()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        public virtual WpfDrawingContext Context
        {
            get {
                return _context;
            }
        }

        public virtual bool IsInitialized
        {
            get {
                return (_context != null);
            }
        }

        #endregion

        #region Public Methods

        public static bool IsNullOrIdentity(Transform transform)
        {
            return (transform == null || transform.Value.IsIdentity);
        }

        public static bool SplitByCaps(string input, out string output)
        {
            output = input;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            output = _regExCaps.Replace(input, " ");

            return output.Length > input.Length;
        }

        public static string GetElementName(SvgElement element, WpfDrawingContext context = null)
        {
            if (element == null)
            {
                return string.Empty;
            }
            if (context != null && context.IDVisitor != null)
            {
                return context.IDVisitor.Visit(element, context);
            }
            string elementId = element.Id;
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return string.Empty;
            }
            if (string.Equals(elementId, "svgbar"))
            {
                elementId = "svgbar";
            }
            elementId = elementId.Trim();
            if (IsValidIdentifier(elementId))
            {
                return elementId;
            }

            return Regex.Replace(elementId, @"[^[0-9a-zA-Z]]*", "_");
        }

        public static string GetElementClassName(SvgElement element, WpfDrawingContext context = null)
        {
            if (element == null)
            {
                return string.Empty;
            }
            if (context != null && context.ClassVisitor != null)
            {
                return context.ClassVisitor.Visit(element, context);
            }

            string className = (element as ISvgStylable)?.ClassName?.BaseVal?.Trim();
            return string.IsNullOrWhiteSpace(className) ? string.Empty : className;
        }

        public static bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return false;
            }

            if (!IsIdentifierStart(identifier[0]))
            {
                return false;
            }

            for (int i = 1; i < identifier.Length; i++)
            {
                if (!IsIdentifierPart(identifier[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsIdentifierStart(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || char.IsLetter(c);
        }

        public static bool IsIdentifierPart(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                || c == '_' || (c >= '0' && c <= '9') || char.IsLetter(c);
        }

        public static Transform Combine(Transform first, Transform second,
            bool groupedFormat, bool checkEquality = true)
        {
            if (first == null && second == null)
            {
                return null;
            }
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }
            if (first.Value.IsIdentity)
            {
                return second;
            }
            if (second.Value.IsIdentity)
            {
                return first;
            }
            if (checkEquality && Matrix.Equals(first.Value, second.Value))
            {
                return first;
            }
            if (groupedFormat)
            {
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(first);
                transformGroup.Children.Add(second);

                return transformGroup;
            }
            return new MatrixTransform(Matrix.Multiply(first.Value, second.Value));
        }

        #endregion

        #region Geometry Methods

        public Geometry CreateGeometry(ISvgElement element, bool optimizePath)
        {
            if (element == null || element.RenderingHint != SvgRenderingHint.Shape)
            {
                return null;
            }

            try
            {
                string localName = element.LocalName;
                switch (localName)
                {
                    case "ellipse":
                        return CreateGeometry((SvgEllipseElement)element);
                    case "rect":
                        return CreateGeometry((SvgRectElement)element);
                    case "line":
                        return CreateGeometry((SvgLineElement)element);
                    case "path":
                        if (optimizePath)
                        {
                            return CreateGeometryEx((SvgPathElement)element);
                        }
                        return CreateGeometry((SvgPathElement)element);
                    case "circle":
                        return CreateGeometry((SvgCircleElement)element);
                    case "polyline":
                        return CreateGeometry((SvgPolylineElement)element);
                    case "polygon":
                        return CreateGeometry((SvgPolygonElement)element);
                }

            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        #region SvgEllipseElement Geometry

        public Geometry CreateGeometry(SvgEllipseElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _rx = Math.Round(element.Rx.AnimVal.Value, 4);
            double _ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (_rx <= 0 || _ry <= 0)
            {
                return null;
            }

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy),
                _rx, _ry);

            return geometry;
        }

        #endregion

        #region SvgRectElement Geometry

        public Geometry CreateGeometry(SvgRectElement element)
        {
            double dx = Math.Round(element.X.AnimVal.Value, 4);
            double dy = Math.Round(element.Y.AnimVal.Value, 4);
            double width = Math.Round(element.Width.AnimVal.Value, 4);
            double height = Math.Round(element.Height.AnimVal.Value, 4);
            double rx = Math.Round(element.Rx.AnimVal.Value, 4);
            double ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (width <= 0 || height <= 0)
            {
                return null;
            }
            if (rx <= 0 && ry > 0)
            {
                rx = ry;
            }
            else if (rx > 0 && ry <= 0)
            {
                ry = rx;
            }

            return new RectangleGeometry(new Rect(dx, dy, width, height), rx, ry);
        }

        #endregion

        #region SvgLineElement Geometry

        public Geometry CreateGeometry(SvgLineElement element)
        {
            return new LineGeometry(new Point(Math.Round(element.X1.AnimVal.Value, 4),
                Math.Round(element.Y1.AnimVal.Value, 4)),
                new Point(Math.Round(element.X2.AnimVal.Value, 4), Math.Round(element.Y2.AnimVal.Value, 4)));
        }

        #endregion

        #region SvgPathElement Geometry

        public Geometry CreateGeometryEx(SvgPathElement element)
        {
            PathGeometry geometry = new PathGeometry();

            string pathScript = element.PathScript;
            if (string.IsNullOrWhiteSpace(pathScript))
            {
                return geometry;
            }

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!string.IsNullOrWhiteSpace(clipRule) &&
                string.Equals(clipRule, "evenodd") || string.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            try
            {
                geometry.Figures = PathFigureCollection.Parse(pathScript);

                if (_flattenClosedPath && element.IsClosed &&
                    geometry.MayHaveCurves() == element.MayHaveCurves)
                {
                    int closedCount = 0;
                    foreach (var figure in geometry.Figures)
                    {
                        if (figure.IsClosed)
                        {
                            closedCount++;
                        }
                    }

                    if (geometry.Figures.Count == closedCount)
                    {
                        return geometry.GetFlattenedPathGeometry(_flattenTolerance, _flattenToleranceType);
                    }
                }

                return geometry;
            }
            catch (FormatException ex)
            {
                Trace.TraceError(ex.GetType().Name + ": " + ex.Message);
                //return null;
                return CreateGeometry(element);
            }
        }

        public Geometry CreateGeometry(SvgPathElement element)
        {
            PathGeometry geometry = new PathGeometry();

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!string.IsNullOrWhiteSpace(clipRule) &&
                string.Equals(clipRule, "evenodd") || string.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            SvgPointF initPoint = new SvgPointF(0, 0);
            SvgPointF lastPoint = new SvgPointF(0, 0);
            SvgPointF ptXY = new SvgPointF(0, 0);

            SvgPathSeg segment            = null;
            SvgPathSegMoveto pathMoveTo   = null;
            SvgPathSegLineto pathLineTo   = null;
            SvgPathSegCurveto pathCurveTo = null;
            SvgPathSegArc pathArc         = null;

            SvgPathSegList segments = element.PathSegList;
            int numSegs = segments.NumberOfItems;
            if (numSegs == 0)
            {
                return geometry;
            }

            PathFigure pathFigure = null;

            for (int i = 0; i < numSegs; i++)
            {
                segment = segments.GetItem(i);

                switch (segment.PathType)
                {
                    case SvgPathType.MoveTo: //if (DynamicCast.Cast(segment, out pathMoveTo))
                        pathMoveTo = (SvgPathSegMoveto)segment;
                        if (pathFigure != null)
                        {
                            pathFigure.IsClosed = false;
                            pathFigure.IsFilled = true;
                            geometry.Figures.Add(pathFigure);
                            pathFigure = null;
                        }

                        lastPoint = initPoint = pathMoveTo.AbsXY;

                        pathFigure = new PathFigure();
                        pathFigure.StartPoint = new Point(initPoint.ValueX, initPoint.ValueY);
                        break;

                    case SvgPathType.LineTo: //else if (DynamicCast.Cast(segment, out pathLineTo))
                        pathLineTo = (SvgPathSegLineto)segment;
                        ptXY = pathLineTo.AbsXY;
                        pathFigure.Segments.Add(new LineSegment(new Point(ptXY.ValueX, ptXY.ValueY), true));

                        lastPoint = ptXY;
                        break;

                    case SvgPathType.CurveTo: //else if (DynamicCast.Cast(segment, out pathCurveTo))
                        pathCurveTo = (SvgPathSegCurveto)segment;

                        SvgPointF xy = pathCurveTo.AbsXY;
                        SvgPointF x1y1 = pathCurveTo.CubicX1Y1;
                        SvgPointF x2y2 = pathCurveTo.CubicX2Y2;
                        pathFigure.Segments.Add(new BezierSegment(new Point(x1y1.ValueX, x1y1.ValueY),
                            new Point(x2y2.ValueX, x2y2.ValueY), new Point(xy.ValueX, xy.ValueY), true));

                        lastPoint = xy;
                        break;

                    case SvgPathType.ArcTo: //else if (DynamicCast.Cast(segment, out pathArc))
                        pathArc = (SvgPathSegArc)segment;
                        ptXY = pathArc.AbsXY;
                        if (lastPoint.Equals(ptXY))
                        {
                            // If the endpoints (x, y) and (x0, y0) are identical, then this
                            // is equivalent to omitting the elliptical arc segment entirely.
                        }
                        else if (pathArc.R1.Equals(0) || pathArc.R2.Equals(0))
                        {
                            // Ensure radii are valid
                            pathFigure.Segments.Add(new LineSegment(new Point(ptXY.ValueX, ptXY.ValueY), true));
                        }
                        else
                        {
                            CalculatedArcValues calcValues = pathArc.GetCalculatedArcValues();

                            pathFigure.Segments.Add(new ArcSegment(new Point(ptXY.ValueX, ptXY.ValueY),
                                new Size(pathArc.R1, pathArc.R2), pathArc.Angle, pathArc.LargeArcFlag,
                                pathArc.SweepFlag ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                true));
                        }

                        lastPoint = ptXY;
                        break;

                    case SvgPathType.Close://else if (segment is SvgPathSegClosePath)
                        if (pathFigure != null)
                        {
                            pathFigure.IsClosed = true;
                            pathFigure.IsFilled = true;
                            geometry.Figures.Add(pathFigure);
                            pathFigure = null;
                        }

                        lastPoint = initPoint;
                        break;
                }
            }

            if (pathFigure != null)
            {
                pathFigure.IsClosed = false;
                pathFigure.IsFilled = true;
                geometry.Figures.Add(pathFigure);
            }

            return geometry;
        }

        #endregion

        #region SvgCircleElement Geometry

        public Geometry CreateGeometry(SvgCircleElement element)
        {
            double _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            double _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            double _r = Math.Round(element.R.AnimVal.Value, 4);

            if (_r <= 0)
            {
                return null;
            }

            EllipseGeometry geometry = new EllipseGeometry(new Point(_cx, _cy), _r, _r);

            return geometry;
        }

        #endregion

        #region SvgPolylineElement Geometry

        public Geometry CreateGeometry(SvgPolylineElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0)
            {
                return null;
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

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!string.IsNullOrWhiteSpace(clipRule) &&
                string.Equals(clipRule, "evenodd") || string.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #region SvgPolygonElement Geometry

        public Geometry CreateGeometry(SvgPolygonElement element)
        {
            ISvgPointList list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0)
            {
                return null;
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

            string fillRule = element.GetPropertyValue("fill-rule");
            string clipRule = element.GetAttribute("clip-rule");
            if (!string.IsNullOrWhiteSpace(clipRule) &&
                string.Equals(clipRule, "evenodd") || string.Equals(clipRule, "nonzero"))
            {
                fillRule = clipRule;
            }
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #endregion

        #region IDisposable Members

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        #endregion
    }
}
