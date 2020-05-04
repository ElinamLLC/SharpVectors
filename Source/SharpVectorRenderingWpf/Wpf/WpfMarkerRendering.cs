using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfMarkerRendering : WpfRendering
    {
        #region Private Fields

        private Matrix _matrix;
        private DrawingGroup _drawGroup;
        private SvgMarkerElement _markerElement;
        private SvgStyleableElement _hostElement;

        private PathGeometry _hostGeometry;
        private PathFigureCollection _pathFigures;

        #endregion

        #region Constructors and Destructor

        public WpfMarkerRendering(SvgElement element)
            : base(element)
        {
            _markerElement = element as SvgMarkerElement;
        }

        #endregion

        #region Public Methods

        // disable default rendering
        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            if (_hostElement != null && _paintContext != null)
            {
                _paintContext.TargetId = _hostElement.UniqueId;
            }

            _matrix = Matrix.Identity;

            WpfDrawingContext context = renderer.Context;

            //SetQuality(context);
            //SetTransform(context);
            //SetClip(_context);
            //SetMask(_context);

            _drawGroup = new DrawingGroup();

            string sVisibility = _markerElement.GetPropertyValue("visibility");
            string sDisplay = _markerElement.GetPropertyValue("display");
            if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            {
                // A 'marker' element with 'display' set to 'none' on that element or any 
                // ancestor is rendered when referenced by another element.

                // _drawGroup.Opacity = 0;
            }

            string elementId = this.GetElementName();
            if (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
            {
                SvgObject.SetName(_drawGroup, elementId);

                context.RegisterId(elementId);

                if (context.IncludeRuntime)
                {
                    SvgObject.SetId(_drawGroup, elementId);
                }
            }

            string elementClass = this.GetElementClass();
            if (!string.IsNullOrWhiteSpace(elementClass) && context.IncludeRuntime)
            {
                SvgObject.SetClass(_drawGroup, elementClass);
            }

            //Transform markerTransform = this.Transform;
            //if (markerTransform != null && !markerTransform.Value.IsIdentity)
            //{
            //    _drawGroup.Transform = markerTransform;
            //}
            //else
            //{
            //    markerTransform = null; // render any identity transform useless...
            //}
            Geometry markerClip = this.ClipGeometry;
            if (markerClip != null && !markerClip.IsEmpty())
            {
                _drawGroup.ClipGeometry = markerClip;
            }
            else
            {
                markerClip = null; // render any empty geometry useless...
            }
            Brush markerMask = this.Masking;
            if (markerMask != null)
            {
                _drawGroup.OpacityMask = markerMask;
            }

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            currentGroup.Children.Add(_drawGroup);
            context.Push(_drawGroup);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            if (_drawGroup != null)
            {
                if (!_matrix.IsIdentity)
                {
                    _drawGroup.Transform = new MatrixTransform(_matrix);
                }

                Geometry clipGeom = this.ClipGeometry;
                if (clipGeom != null)
                {
                    _drawGroup.ClipGeometry = clipGeom;
                }

                //Transform transform = this.Transform;
                //if (transform != null)
                //{
                //    _drawGroup.Transform = transform;
                //}
            }

            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            Debug.Assert(_drawGroup != null);

            WpfDrawingContext context = renderer.Context;

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }
            ISvgAnimatedEnumeration markerUnits = _markerElement.MarkerUnits;
            if (markerUnits.AnimVal.Equals((ushort)SvgMarkerUnit.StrokeWidth))
            {
                var comparer = StringComparison.OrdinalIgnoreCase;
                string overflowAttr = _markerElement.GetAttribute("overflow");
                if (string.IsNullOrWhiteSpace(overflowAttr) 
                    || overflowAttr.Equals("scroll", comparer) || overflowAttr.Equals("hidden", comparer))
                {
                    Geometry markerClip = this.ClipGeometry;
                    if (markerClip == null || markerClip.IsEmpty())
                    {
                        SvgRect clipRect = (SvgRect)_markerElement.ViewBox.AnimVal;
                        if (clipRect != null && !clipRect.IsEmpty)
                        {
                            _drawGroup.ClipGeometry = new RectangleGeometry(
                                new Rect(clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height));
                        }
                        else if (_markerElement.IsSizeDefined)
                        {
                            _drawGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0, 
                                _markerElement.MarkerWidth.AnimVal.Value, _markerElement.MarkerHeight.AnimVal.Value));
                        }
                        else if (_hostElement != null)
                        {
                            // Special cases for zero-length 'path' and 'line' segments.
                            var isLineSegment = false;
                            if (_hostGeometry != null)
                            {
                                var bounds = _hostGeometry.Bounds;
                                if (string.Equals(_hostElement.LocalName, "line", StringComparison.Ordinal))
                                {
                                    isLineSegment = true;
                                }
                                else if (string.Equals(_hostElement.LocalName, "rect", StringComparison.Ordinal))
                                {
                                    isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
                                }
                                else if (string.Equals(_hostElement.LocalName, "path", StringComparison.Ordinal))
                                {
                                    isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
                                }
                            }
                            else
                            {
                                if (string.Equals(_hostElement.LocalName, "line", StringComparison.Ordinal))
                                {
                                    isLineSegment = true;
                                }
                            }
                            if (isLineSegment)
                            {
                                bool isZeroWidthLine = false;
                                if (_pathFigures != null)
                                {
                                    if (_pathFigures.Count == 0)
                                    {
                                        isZeroWidthLine = true;
                                    }
                                    else
                                    {
                                        var pathWidth = 0.0d;
                                        foreach (PathFigure pathFigure in _pathFigures)
                                        {
                                            pathWidth += WpfConvert.GetPathFigureLength(pathFigure); 
                                        }
                                        isZeroWidthLine = pathWidth.Equals(0.0d);
                                    }
                                }

                                if (isZeroWidthLine)
                                {
                                    _drawGroup.ClipGeometry = new RectangleGeometry(new Rect(0, 0,
                                        _markerElement.MarkerWidth.AnimVal.Value, _markerElement.MarkerHeight.AnimVal.Value));
                                }
                            }
                        }
                    }
                }
            }

            context.Pop();

            base.AfterRender(renderer);
        }

        public void RenderMarker(WpfDrawingRenderer renderer, WpfDrawingContext gr,
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
        {
            _hostElement = refElement;
            if (_hostElement != null)
            {
                var paintContext = gr.GetPaintContext(_hostElement.UniqueId);
                if (paintContext != null)
                {
                    PathGeometry pathGeometry = paintContext.Tag as PathGeometry;

                    if (pathGeometry != null)
                    {
                        _pathFigures = pathGeometry.Figures;

                        _hostGeometry = pathGeometry;
                    }
                    else
                    {
                        var hostGeometry = paintContext.Tag as Geometry;
                        if (hostGeometry != null)
                        {
                            _hostGeometry = hostGeometry.GetFlattenedPathGeometry();
                            _pathFigures  = _hostGeometry.Figures;
                        }
                    }
                }
            }

            ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;

            SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
            if (vertexPositions == null)
            {
                return;
            }

            bool mayHaveCurves = markerHostElm.MayHaveCurves;

            int start = 0;
            int len   = 0;

            // Choose which part of the position array to use
            switch (markerPos)
            {
                case SvgMarkerPosition.Start:
                    start = 0;
                    len = 1;
                    break;
                case SvgMarkerPosition.Mid:
                    start = 1;
                    len = vertexPositions.Length - 2;
                    break;
                default:
                    // == MarkerPosition.End
                    start = vertexPositions.Length - 1;
                    len = 1;
                    break;
            }
            int end = start + len;

            TransformGroup transform = new TransformGroup();

            for (int i = start; i < end; i++)
            {
                SvgPointF point = vertexPositions[i];

                this.BeforeRender(renderer);

                Matrix matrix = GetTransformMatrix(_svgElement, transform);

                ISvgAnimatedEnumeration orientType = _markerElement.OrientType;

                if (orientType.AnimVal.Equals((ushort)SvgMarkerOrient.Angle))
                {
                    double scaleValue = _markerElement.OrientAngle.AnimVal.Value;
                    if (!scaleValue.Equals(0))
                    {
                        matrix.Rotate(scaleValue);
                        transform.Children.Add(new RotateTransform(scaleValue));
                    }
                }
                else
                {
                    bool isAutoReverse = orientType.AnimVal.Equals((ushort)SvgMarkerOrient.AutoStartReverse);

                    double angle = 0;
                    switch (markerPos)
                    {
                        case SvgMarkerPosition.Start:
                            angle = markerHostElm.GetStartAngle(i);
                            //angle = markerHostElm.GetStartAngle(i + 1);
                            if (vertexPositions.Length >= 2)
                            {
                                SvgPointF pMarkerPoint1 = vertexPositions[start];
                                SvgPointF pMarkerPoint2 = vertexPositions[end];
                                float xDiff = pMarkerPoint2.X - pMarkerPoint1.X;
                                float yDiff = pMarkerPoint2.Y - pMarkerPoint1.Y;
                                double angleMarker = (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
                                if (!angleMarker.Equals(angle))
                                {
                                    angle = angleMarker;
                                }
                            }
                            if (mayHaveCurves)
                            {
                                angle = this.GetAngleAt(start, angle, markerPos, markerHostElm);
                            }

                            // A value of 'auto-start-reverse' means the same as 'auto' except that for a 
                            // marker placed by 'marker-start', the orientation is 180° different from 
                            // the orientation as determined by 'auto'.
                            if (isAutoReverse)
                            {
                                angle += 180;
                            }
                            break;
                        case SvgMarkerPosition.Mid:
                            //angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
                            angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
                            if (mayHaveCurves)
                            {
                                angle = this.GetAngleAt(i, angle, markerPos, markerHostElm);
                            }

                            break;
                        default:
                            angle = markerHostElm.GetEndAngle(i - 1);
                            //double angle2 = markerHostElm.GetEndAngle(i);
                            if (vertexPositions.Length >= 2)
                            {
                                SvgPointF pMarkerPoint1 = vertexPositions[start - 1];
                                SvgPointF pMarkerPoint2 = vertexPositions[start];
                                float xDiff = pMarkerPoint2.X - pMarkerPoint1.X;
                                float yDiff = pMarkerPoint2.Y - pMarkerPoint1.Y;
                                double angleMarker = (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
                                if (!angleMarker.Equals(angle))
                                {
                                    angle = angleMarker;
                                }
                            }
                            if (mayHaveCurves)
                            {
                                angle = this.GetAngleAt(start - 1, angle, markerPos, markerHostElm);
                            }
                            break;
                    }
                    matrix.Rotate(angle);
                    transform.Children.Add(new RotateTransform(angle));
                }

                // 'viewBox' and 'preserveAspectRatio' attributes
                // viewBox -> viewport(0, 0, markerWidth, markerHeight)
                var spar = (SvgPreserveAspectRatio)_markerElement.PreserveAspectRatio.AnimVal;
                double[] translateAndScale = spar.FitToViewBox((SvgRect)_markerElement.ViewBox.AnimVal,
                    new SvgRect(0, 0, _markerElement.MarkerWidth.AnimVal.Value, _markerElement.MarkerHeight.AnimVal.Value));

                // Warning at this time, refX and refY are relative to the painted element's coordinate system. 
                // We need to move the reference point to the marker's coordinate system
                double refX = _markerElement.RefX.AnimVal.Value;
                double refY = _markerElement.RefY.AnimVal.Value;

                if (!(refX.Equals(0) && refY.Equals(0)))
                {
                    var ptRef = matrix.Transform(new Point(refX, refY));

                    refX = ptRef.X;
                    refY = ptRef.Y;

                    matrix.Translate(-refX, -refY);
                    transform.Children.Add(new TranslateTransform(-refX, -refY));
                }

                //matrix.Translate(-markerElm.RefX.AnimVal.Value * translateAndScale[2],
                //    -markerElm.RefY.AnimVal.Value * translateAndScale[3]);
                //transform.Children.Add(new TranslateTransform(-markerElm.RefX.AnimVal.Value * translateAndScale[2],
                //    -markerElm.RefY.AnimVal.Value * translateAndScale[3]));

                // compute an additional transform for 'strokeWidth' coordinate system
                ISvgAnimatedEnumeration markerUnits = _markerElement.MarkerUnits;
                if (markerUnits.AnimVal.Equals((ushort)SvgMarkerUnit.StrokeWidth))
                {
                    SvgLength strokeWidthLength = new SvgLength(refElement,
                        "stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
                    double strokeWidth = strokeWidthLength.Value;
                    if (!strokeWidth.Equals(1))
                    {
                        matrix.Scale(strokeWidth, strokeWidth);
                        transform.Children.Add(new ScaleTransform(strokeWidth, strokeWidth));
                    }
                }

                if (!(translateAndScale[2].Equals(1) && translateAndScale[3].Equals(1)))
                {
                    matrix.Scale(translateAndScale[2], translateAndScale[3]);
                    transform.Children.Add(new ScaleTransform(translateAndScale[2], translateAndScale[3]));
                }

                matrix.Translate(point.X, point.Y);
                transform.Children.Add(new TranslateTransform(point.X, point.Y));

                _matrix = matrix;

                this.Transform = transform;

                this.Render(renderer);

                renderer.RenderChildren(_markerElement);

                this.AfterRender(renderer);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _matrix        = Matrix.Identity;
            _drawGroup     = null;
            _hostElement   = null;
            _pathFigures   = null;
            _hostGeometry  = null;

            _markerElement = element as SvgMarkerElement;
        }

        #endregion

        #region Private Methods

        private static Matrix GetTransformMatrix(SvgElement element)
        {
            ISvgTransformable transElm = element as ISvgTransformable;
            if (transElm == null)
                return Matrix.Identity;

            SvgTransformList svgTList = (SvgTransformList)transElm.Transform.AnimVal;
            SvgTransform svgTransform = (SvgTransform)svgTList.Consolidate();
            SvgMatrix svgMatrix       = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

            return new Matrix(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);
        }

        private static Matrix GetTransformMatrix(SvgElement element, TransformGroup transform)
        {
            ISvgTransformable transElm = element as ISvgTransformable;
            if (transElm == null)
                return Matrix.Identity;

            SvgTransformList svgTList = (SvgTransformList)transElm.Transform.AnimVal;
            SvgTransform svgTransform = (SvgTransform)svgTList.Consolidate();
            SvgMatrix svgMatrix       = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

            var matrix = new Matrix(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);

            if (!matrix.IsIdentity)
            {
                transform.Children.Add(new MatrixTransform(matrix));
            }

            return matrix;
        }

        private double GetAngleAt(int index, double angle, SvgMarkerPosition position, ISharpMarkerHost markerHost)
        {
            if (markerHost == null || _pathFigures == null || _pathFigures.Count == 0)
            {
                return angle;
            }
            var marker = markerHost.GetMarker(index + 1);

            PathGeometry pathFlattened = null;
            if (_pathFigures.Count != 1)
            {
                if (index < _pathFigures.Count)
                {
                    PathGeometry path = new PathGeometry(new PathFigure[] { _pathFigures[index] });
                    pathFlattened = path.GetFlattenedPathGeometry();
                }
            }
            else
            {
                PathFigure figureAt = _pathFigures[0];

                if (figureAt.Segments.Count == 1)
                {
                    PathGeometry path = new PathGeometry(new PathFigure[] { figureAt });
                    pathFlattened = path.GetFlattenedPathGeometry();
                }
                else
                {
                    if (marker.IsCurve == false)
                    {
                        return angle;
                    }

                    if (index < figureAt.Segments.Count)
                    {
                        var pathSegment = figureAt.Segments[index];

                        Point startPoint = new Point(0, 0);
                        if (marker != null)
                        {
                            var pathSet = marker.Segment;

                            if (pathSet != null && pathSet.Limits != null && pathSet.Limits.Length == 2)
                            {
                                SvgPointF point = pathSet.Limits[0];
                                startPoint = new Point(point.ValueX, point.ValueY);
                            }
                        }

                        PathFigure targetFigure = new PathFigure(startPoint, new PathSegment[] { pathSegment }, false);
                        PathGeometry path = new PathGeometry(new PathFigure[] { targetFigure });
                        pathFlattened = path.GetFlattenedPathGeometry();
                    }
                }
            }

            if (pathFlattened != null)
            {
                double progress = 1;
                switch (position)
                {
                    case SvgMarkerPosition.End:
                        progress = 1;
                        break;
                    case SvgMarkerPosition.Start:
                        progress = 0;
                        break;
                    case SvgMarkerPosition.Mid:
                        progress = 0;
                        break;
                }
                Point locationAt;
                Point tagentAt;

                pathFlattened.GetPointAtFractionLength(progress, out locationAt, out tagentAt);

                return Math.Atan2(tagentAt.Y, tagentAt.X) * 180 / Math.PI;
            }

            return angle;
        }

        #endregion
    }
}
