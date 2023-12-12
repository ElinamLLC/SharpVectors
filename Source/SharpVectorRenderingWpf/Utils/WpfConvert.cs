using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Utils
{
    public static class WpfConvert
    {
        // Approximating a 1/4 circle with a Bezier curve               
        private const double ArcAsBezier = 0.5522847498307933984;

        /// <summary>
        /// A WPF <see cref="Color"/> representation of the <c>RgbColor</c>.
        /// </summary>
        public static Color? ToColor(ICssColor color)
        {
            if (color == null)
            {
                return null;
            }
            if (color.IsSystemColor)
            {
                string colorName = color.Name;

                switch (colorName.ToLowerInvariant())
                {
                    case "activeborder"       : return SystemColors.ActiveBorderColor;
                    case "activecaption"      : return SystemColors.ActiveCaptionColor;
                    case "appworkspace"       : return SystemColors.AppWorkspaceColor;
                    case "background"         : return SystemColors.DesktopColor;
                    case "buttonface"         : return SystemColors.ControlColor;
                    case "buttonhighlight"    : return SystemColors.ControlLightLightColor;
                    case "buttonshadow"       : return SystemColors.ControlDarkColor;
                    case "buttontext"         : return SystemColors.ControlTextColor;
                    case "captiontext"        : return SystemColors.ActiveCaptionTextColor;
                    case "graytext"           : return SystemColors.GrayTextColor;
                    case "highlight"          : return SystemColors.HighlightColor;
                    case "highlighttext"      : return SystemColors.HighlightTextColor;
                    case "inactiveborder"     : return SystemColors.InactiveBorderColor;
                    case "inactivecaption"    : return SystemColors.InactiveCaptionColor;
                    case "inactivecaptiontext": return SystemColors.InactiveCaptionTextColor;
                    case "infobackground"     : return SystemColors.InfoColor;
                    case "infotext"           : return SystemColors.InfoTextColor;
                    case "menu"               : return SystemColors.MenuColor;
                    case "menutext"           : return SystemColors.MenuTextColor;
                    case "scrollbar"          : return SystemColors.ScrollBarColor;
                    case "threeddarkshadow"   : return SystemColors.ControlDarkDarkColor;
                    case "threedface"         : return SystemColors.ControlColor;
                    case "threedhighlight"    : return SystemColors.ControlLightColor;
                    case "threedlightshadow"  : return SystemColors.ControlLightLightColor;
                    case "window"             : return SystemColors.WindowColor;
                    case "windowframe"        : return SystemColors.WindowFrameColor;
                    case "windowtext"         : return SystemColors.WindowTextColor;
                }

                return (Color)ColorConverter.ConvertFromString(colorName);
            }

            if (color.Red == null || color.Green == null || color.Blue == null)
            {
                return null;
            }

            double dRed   = color.Red.GetFloatValue(CssPrimitiveType.Number);
            double dGreen = color.Green.GetFloatValue(CssPrimitiveType.Number);
            double dBlue  = color.Blue.GetFloatValue(CssPrimitiveType.Number);

            if (dRed < 0 || double.IsNaN(dRed) || double.IsInfinity(dRed))
            {
                return null;
            }
            if (dGreen < 0 || double.IsNaN(dGreen) || double.IsInfinity(dGreen))
            {
                return null;
            }
            if (dBlue < 0 || double.IsNaN(dBlue) || double.IsInfinity(dBlue))
            {
                return null;
            }
            if (color.HasAlpha)
            {
                double dAlpha = color.Alpha.GetFloatValue(color.Alpha.PrimitiveType == CssPrimitiveType.Percentage ? CssPrimitiveType.Percentage : CssPrimitiveType.Number);
                if (!double.IsNaN(dAlpha) && !double.IsInfinity(dAlpha))
                {
                    return Color.FromArgb(Convert.ToByte(dAlpha), Convert.ToByte(dRed), 
                        Convert.ToByte(dGreen), Convert.ToByte(dBlue));
                }
            }

            return Color.FromRgb(Convert.ToByte(dRed), Convert.ToByte(dGreen), Convert.ToByte(dBlue));
        }

        public static Rect ToRect(ICssRect rect)
        {
            if (rect == null)
            {
                return Rect.Empty;
            }

            double x      = rect.Left.GetFloatValue(CssPrimitiveType.Px);
            double y      = rect.Top.GetFloatValue(CssPrimitiveType.Px);
            double width  = rect.Right.GetFloatValue(CssPrimitiveType.Px) - x;
            double height = rect.Bottom.GetFloatValue(CssPrimitiveType.Px) - y;

            return new Rect(x, y, width, height);
        }

        public static bool Equals(Size size1, Size size2)
        {
            var width1 = Math.Round(size1.Width, 4);
            var width2 = Math.Round(size2.Width, 4);
            if (!width1.Equals(width2))
            {
                return false;
            }
            var height1 = Math.Round(size1.Height, 4);
            var height2 = Math.Round(size2.Height, 4);
            if (!height1.Equals(height2))
            {
                return false;
            }
            return true;
        }

        public static PathGeometry ToPath(SvgRectF rect)
        {
            return ToPath(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
        }

        public static PathGeometry ToPath(Rect rect)
        {
            PathGeometry geometry = new PathGeometry();
            geometry.FillRule = FillRule.EvenOdd;
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = rect.TopLeft;
            pathFigure.Segments.Add(new LineSegment(rect.TopRight, true));
            pathFigure.Segments.Add(new LineSegment(rect.BottomRight, true));
            pathFigure.Segments.Add(new LineSegment(rect.BottomLeft, true));

            pathFigure.IsClosed = true;
            geometry.Figures.Add(pathFigure);

            return geometry;
        }

        public static PathGeometry ToPath(RectangleGeometry rect)
        {
            return new PathGeometry(GetFigures(rect), FillRule.EvenOdd, null);
        }

        public static PathGeometry ToPath(EllipseGeometry ellipse)
        {
            return new PathGeometry(GetFigures(ellipse), FillRule.EvenOdd, null);
        }

        public static PathGeometry ToPath(LineGeometry line)
        {
            return new PathGeometry(GetFigures(line), FillRule.EvenOdd, null);
        }

        private static bool IsRounded(double radiusX, double radiusY)
        {
            return (radiusX != 0.0) && (radiusY != 0.0);
        }

        private static Point[] GetPointList(Rect rect, double radiusX, double radiusY, bool isRounded)
        {
            int pointCount = 0;
            if (!rect.IsEmpty)
            {
                // rounded or squared
                pointCount = isRounded ? 17 : 5;
            }
            Point[] points = new Point[pointCount];

            GetPointList(points, rect, radiusX, radiusY, isRounded);

            return points;
        }

        private static void GetPointList(Point[] points, Rect rect, double radiusX, double radiusY, bool isRounded)
        {
            if (isRounded)
            {
                radiusX = Math.Min(rect.Width * (1.0 / 2.0), Math.Abs(radiusX));
                radiusY = Math.Min(rect.Height * (1.0 / 2.0), Math.Abs(radiusY));

                double bezierX = ((1.0 - ArcAsBezier) * radiusX);
                double bezierY = ((1.0 - ArcAsBezier) * radiusY);

                points[1].X = points[0].X = points[15].X = points[14].X = rect.X;
                points[2].X = points[13].X = rect.X + bezierX;
                points[3].X = points[12].X = rect.X + radiusX;
                points[4].X = points[11].X = rect.Right - radiusX;
                points[5].X = points[10].X = rect.Right - bezierX;
                points[6].X = points[7].X = points[8].X = points[9].X = rect.Right;

                points[2].Y = points[3].Y = points[4].Y = points[5].Y = rect.Y;
                points[1].Y = points[6].Y = rect.Y + bezierY;
                points[0].Y = points[7].Y = rect.Y + radiusY;
                points[15].Y = points[8].Y = rect.Bottom - radiusY;
                points[14].Y = points[9].Y = rect.Bottom - bezierY;
                points[13].Y = points[12].Y = points[11].Y = points[10].Y = rect.Bottom;

                points[16] = points[0];
            }
            else
            {
                points[0].X = points[3].X = points[4].X = rect.X;
                points[1].X = points[2].X = rect.Right;

                points[0].Y = points[1].Y = points[4].Y = rect.Y;
                points[2].Y = points[3].Y = rect.Bottom;
            }
        }

        private static void GetPointList(Point[] points, Point center, double radiusX, double radiusY)
        {
            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);

            // Set the X coordinates
            double mid = radiusX * ArcAsBezier;

            points[0].X = points[1].X = points[11].X = points[12].X = center.X + radiusX;
            points[2].X = points[10].X = center.X + mid;
            points[3].X = points[9].X = center.X;
            points[4].X = points[8].X = center.X - mid;
            points[5].X = points[6].X = points[7].X = center.X - radiusX;

            // Set the Y coordinates
            mid = radiusY * ArcAsBezier;

            points[2].Y = points[3].Y = points[4].Y = center.Y + radiusY;
            points[1].Y = points[5].Y = center.Y + mid;
            points[0].Y = points[6].Y = points[12].Y = center.Y;
            points[7].Y = points[11].Y = center.Y - mid;
            points[8].Y = points[9].Y = points[10].Y = center.Y - radiusY;
        }

        private static PathFigureCollection GetFigures(RectangleGeometry geometry)
        {
            if (geometry.IsEmpty())
            {
                return null;
            }
            Transform transform = geometry.Transform;
            Matrix matrix = transform == null ? Matrix.Identity : transform.Value;

            double radiusX = geometry.RadiusX;
            double radiusY = geometry.RadiusY;
            Rect rect = geometry.Rect;

            var pathFigures = new PathFigureCollection();
            if (IsRounded(radiusX, radiusY))
            {
                Point[] points = GetPointList(rect, radiusX, radiusY, true);
                if (!matrix.IsIdentity)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] *= matrix;
                    }
                }

                var pathSegments = new PathSegment[] {
                    new BezierSegment(points[1], points[2], points[3], true),
                    new LineSegment(points[4], true),
                    new BezierSegment(points[5], points[6], points[7], true),
                    new LineSegment(points[8], true),
                    new BezierSegment(points[9], points[10], points[11], true),
                    new LineSegment(points[12], true),
                    new BezierSegment(points[13], points[14], points[15], true)
                };
                foreach (var pathSegment in pathSegments)
                {
                    pathSegment.IsSmoothJoin = true;
                }
                pathFigures.Add(new PathFigure(points[0], pathSegments, true));
            }
            else
            {
                pathFigures.Add(new PathFigure(rect.TopLeft * matrix, 
                    new PathSegment[] {
                        new PolyLineSegment(
                        new Point[] {
                            rect.TopRight * matrix,
                            rect.BottomRight * matrix,
                            rect.BottomLeft * matrix
                        },
                        true)},
                        true    // closed
                    )
                );
            }

            return pathFigures;
        }

        private static PathFigureCollection GetFigures(EllipseGeometry geometry)
        {
            if (geometry.IsEmpty())
            {
                return null;
            }
            Transform transform = geometry.Transform;
            Matrix matrix = transform == null ? Matrix.Identity : transform.Value;

            Point[] points = new Point[13];
            GetPointList(points, geometry.Center, geometry.RadiusX, geometry.RadiusY);

            if (!matrix.IsIdentity)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] *= matrix;
                }
            }

            PathFigureCollection pathFigures = new PathFigureCollection();
            var pathSegments = new PathSegment[] {
                new BezierSegment(points[1], points[2], points[3], true),
                new BezierSegment(points[4], points[5], points[6], true),
                new BezierSegment(points[7], points[8], points[9], true),
                new BezierSegment(points[10], points[11], points[12], true)
            };
            foreach (var pathSegment in pathSegments)
            {
                pathSegment.IsSmoothJoin = true;
            }
            pathFigures.Add(new PathFigure(points[0], pathSegments, true));

            return pathFigures;
        }

        private static PathFigureCollection GetFigures(LineGeometry geometry)
        {
            // This is lossy for consistency with other GetPathFigureCollection() implementations
            // however this limitation doesn't otherwise need to exist for LineGeometry.

            Point startPoint = geometry.StartPoint;
            Point endPoint = geometry.EndPoint;

            Transform transform = geometry.Transform;
            Matrix matrix = transform == null ? Matrix.Identity : transform.Value;

            if (!matrix.IsIdentity)
            {
                startPoint *= matrix;
                endPoint *= matrix;
            }

            PathFigureCollection pathFigures = new PathFigureCollection();
            pathFigures.Add(new PathFigure(startPoint,
                new PathSegment[] { new LineSegment(endPoint, true) }, false));

            return pathFigures;
        }

        /// <summary>
        /// This converts the specified <see cref="Rect"/> structure to a 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="rect">The <see cref="Rect"/> structure to convert.</param>
        /// <returns>
        /// The <see cref="SvgRectF"/> structure that is converted from the 
        /// specified <see cref="Rect"/> structure.
        /// </returns>
        public static Rect ToRect(SvgRectF rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rect ToRect(ISvgRect rect)
        {
            if (rect == null)
            {
                return Rect.Empty;
            }

            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static GradientSpreadMethod ToSpreadMethod(SvgSpreadMethod sm)
        {
            switch (sm)
            {
                case SvgSpreadMethod.Pad:
                    return GradientSpreadMethod.Pad;
                case SvgSpreadMethod.Reflect:
                    return GradientSpreadMethod.Reflect;
                case SvgSpreadMethod.Repeat:
                    return GradientSpreadMethod.Repeat;
            }

            return GradientSpreadMethod.Pad;
        }

        public static double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            bool isAlreadyFlattened = true;

            foreach (PathSegment pathSegment in pathFigure.Segments)
            {
                if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
                {
                    isAlreadyFlattened = false;
                    break;
                }
            }

            var pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();

            double length = 0;
            Point pt1 = pathFigureFlattened.StartPoint;

            foreach (PathSegment pathSegment in pathFigureFlattened.Segments)
            {
                if (pathSegment is LineSegment)
                {
                    Point pt2 = (pathSegment as LineSegment).Point;
                    length += (pt2 - pt1).Length;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment)
                {
                    PointCollection pointCollection = (pathSegment as PolyLineSegment).Points;
                    foreach (Point pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        pt1 = pt2;
                    }
                }
            }

            return length;
        }


        public static Transform GetTransform(SvgElement svgElement, bool _combineTransforms = true)
        {
            ISvgTransformable transElm = svgElement as ISvgTransformable;
            if (transElm == null)
            {
                return null;
            }

            Transform transformMatrix = null;

            SvgTransformList transformList = (SvgTransformList)transElm.Transform.AnimVal;
            if (transformList.NumberOfItems != 0 && _combineTransforms == false)
            {
                List<Transform> transforms = new List<Transform>();

                for (uint i = 0; i < transformList.NumberOfItems; i++)
                {
                    ISvgTransform transform = transformList.GetItem(i);
                    double[] values = transform.InputValues;
                    switch (transform.TransformType)
                    {
                        case SvgTransformType.Translate:
                            if (values.Length == 1)
                            {
                                transforms.Add(new TranslateTransform(values[0], 0));
                            }
                            else if (values.Length == 2)
                            {
                                transforms.Add(new TranslateTransform(values[0], values[1]));
                            }
                            break;
                        case SvgTransformType.Rotate:
                            if (values.Length == 1)
                            {
                                transforms.Add(new RotateTransform(values[0]));
                            }
                            else if (values.Length == 3)
                            {
                                transforms.Add(new RotateTransform(values[0], values[1], values[2]));
                            }
                            break;
                        case SvgTransformType.Scale:
                            if (values.Length == 1)
                            {
                                transforms.Add(new ScaleTransform(values[0], values[0]));
                            }
                            else if (values.Length == 2)
                            {
                                transforms.Add(new ScaleTransform(values[0], values[1]));
                            }
                            break;
                        case SvgTransformType.SkewX:
                            if (values.Length == 1)
                            {
                                transforms.Add(new SkewTransform(values[0], 0));
                            }
                            break;
                        case SvgTransformType.SkewY:
                            if (values.Length == 1)
                            {
                                transforms.Add(new SkewTransform(0, values[0]));
                            }
                            break;
                        case SvgTransformType.Matrix:
                            if (values.Length == 6)
                            {
                                transforms.Add(new MatrixTransform(values[0],
                                    values[1], values[2], values[3], values[4], values[5]));
                            }
                            break;
                    }
                }

                if (transforms.Count == 1)
                {
                    transformMatrix = transforms[0];

                }
                else if (transforms.Count > 1)
                {
                    transforms.Reverse();

                    TransformGroup transformGroup = new TransformGroup();
                    transformGroup.Children = new TransformCollection(transforms);
                    transformMatrix = transformGroup;
                }

                return transformMatrix;
            }
            SvgMatrix svgMatrix = transformList.TotalMatrix;

            if (svgMatrix.IsIdentity)
            {
                return transformMatrix;
            }

            transformMatrix = new MatrixTransform(Math.Round(svgMatrix.A, 6), Math.Round(svgMatrix.B, 6),
                Math.Round(svgMatrix.C, 6), Math.Round(svgMatrix.D, 6),
                Math.Round(svgMatrix.E, 6), Math.Round(svgMatrix.F, 6));

            return transformMatrix;
        }

    }
}
