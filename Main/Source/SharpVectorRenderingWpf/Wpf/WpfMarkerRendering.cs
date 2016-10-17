using System;
using System.Diagnostics;

using System.Windows.Media;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    public enum SvgMarkerPosition 
    { 
        Start, 
        Mid, 
        End 
    }

    public sealed class WpfMarkerRendering : WpfRendering
    {
        #region Private Fields

        private Matrix _matrix;
        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfMarkerRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        // disable default rendering
        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            _matrix = Matrix.Identity;

            WpfDrawingContext context = renderer.Context;
            _drawGroup = new DrawingGroup();

            //string elementId = this.GetElementName();
            //if (!String.IsNullOrEmpty(elementId))
            //{
            //    _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);
            //}

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

            context.Pop();

            base.AfterRender(renderer);
        }

        public static Matrix GetTransformMatrix(SvgElement element)
        {
            ISvgTransformable transElm = element as ISvgTransformable;
            if (transElm == null)
                return Matrix.Identity;

            SvgTransformList svgTList = (SvgTransformList)transElm.Transform.AnimVal;
            //SvgTransform svgTransform = (SvgTransform)svgTList.Consolidate();
            SvgMatrix svgMatrix = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

            return new Matrix(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);
        }

        public void RenderMarker0(WpfDrawingRenderer renderer, WpfDrawingContext gr, 
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
        {
            //PathGeometry g;
            //g.GetPointAtFractionLength(

            ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;
            SvgMarkerElement markerElm = (SvgMarkerElement)_svgElement;

            SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
            int start;
            int len;

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

            for (int i = start; i < start + len; i++)
            {
                SvgPointF point = vertexPositions[i];

                Matrix m = GetTransformMatrix(_svgElement);

                //GraphicsContainer gc = gr.BeginContainer();

                this.BeforeRender(renderer);

                //gr.TranslateTransform(point.X, point.Y);

                //PAUL:
                //m.Translate(point.X, point.Y);

                if (markerElm.OrientType.AnimVal.Equals(SvgMarkerOrient.Angle))
                {
                    m.Rotate(markerElm.OrientAngle.AnimVal.Value);
                    //gr.RotateTransform((double)markerElm.OrientAngle.AnimVal.Value);
                }
                else
                {
                    double angle;

                    switch (markerPos)
                    {
                        case SvgMarkerPosition.Start:
                            angle = markerHostElm.GetStartAngle(i + 1);
                            break;
                        case SvgMarkerPosition.Mid:
                            //angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
                            angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
                            break;
                        default:
                            angle = markerHostElm.GetEndAngle(i);
                            break;
                    }
                    //gr.RotateTransform(angle);
                    m.Rotate(angle);
                }

                if (markerElm.MarkerUnits.AnimVal.Equals(SvgMarkerUnit.StrokeWidth))
                {
                    string propValue = refElement.GetPropertyValue("stroke-width");
                    if (propValue.Length == 0) 
                        propValue = "1";

                    SvgLength strokeWidthLength = new SvgLength("stroke-width", propValue, refElement, SvgLengthDirection.Viewport);
                    double strokeWidth = strokeWidthLength.Value;
                    //gr.ScaleTransform(strokeWidth, strokeWidth);
                    m.Scale(strokeWidth, strokeWidth);
                }

                SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)markerElm.PreserveAspectRatio.AnimVal;
                double[] translateAndScale = spar.FitToViewBox(
                    (SvgRect)markerElm.ViewBox.AnimVal, new SvgRect(0, 0, 
                        markerElm.MarkerWidth.AnimVal.Value, markerElm.MarkerHeight.AnimVal.Value));


                //PAUL:
                //m.Translate(-(double)markerElm.RefX.AnimVal.Value * translateAndScale[2], -(double)markerElm.RefY.AnimVal.Value * translateAndScale[3]);

                //PAUL:
                m.Scale(translateAndScale[2], translateAndScale[3]);
                m.Translate(point.X, point.Y); 

                //Matrix oldTransform = TransformMatrix;
                //TransformMatrix = m;
                //try
                //{
                //newTransform.Append(m);
                //TransformGroup tg = new TransformGroup();

                //renderer.Canvas.re

                //gr.TranslateTransform(
                //    -(double)markerElm.RefX.AnimVal.Value * translateAndScale[2],
                //    -(double)markerElm.RefY.AnimVal.Value * translateAndScale[3]
                //    );

                //gr.ScaleTransform(translateAndScale[2], translateAndScale[3]);

                renderer.RenderChildren(markerElm);
//                markerElm.RenderChildren(renderer);
                //}
                //finally
                //{
                //    TransformMatrix = oldTransform;
                //}
                //    //gr.EndContainer(gc);

                _matrix = m;
                this.Render(renderer);

                //gr.EndContainer(gc);

                this.AfterRender(renderer);
            }
        }

        public void RenderMarker(WpfDrawingRenderer renderer, WpfDrawingContext gr,
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
        {
            ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;
            SvgMarkerElement markerElm = (SvgMarkerElement)_svgElement;

            SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
            int start;
            int len;

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

            for (int i = start; i < start + len; i++)
            {
                SvgPointF point = vertexPositions[i];

                //GdiGraphicsContainer gc = gr.BeginContainer();

                this.BeforeRender(renderer);

                //Matrix matrix = Matrix.Identity;

                Matrix matrix = GetTransformMatrix(_svgElement);

                if (markerElm.OrientType.AnimVal.Equals(SvgMarkerOrient.Angle))
                {
                    matrix.Rotate(markerElm.OrientAngle.AnimVal.Value);
                }
                else
                {
                    double angle = 0;

                    switch (markerPos)
                    {
                        case SvgMarkerPosition.Start:
                            angle = markerHostElm.GetStartAngle(i + 1);
                            break;
                        case SvgMarkerPosition.Mid:
                            //angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
                            angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
                            break;
                        default:
                            angle = markerHostElm.GetEndAngle(i);
                            break;
                    }
                    matrix.Rotate(angle);
                }

                if (markerElm.MarkerUnits.AnimVal.Equals(SvgMarkerUnit.StrokeWidth))
                {
                    SvgLength strokeWidthLength = new SvgLength(refElement,
                        "stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
                    double strokeWidth = strokeWidthLength.Value;
                    matrix.Scale(strokeWidth, strokeWidth);
                }

                SvgPreserveAspectRatio spar =
                    (SvgPreserveAspectRatio)markerElm.PreserveAspectRatio.AnimVal;
                double[] translateAndScale = spar.FitToViewBox((SvgRect)markerElm.ViewBox.AnimVal,
                    new SvgRect(0, 0, markerElm.MarkerWidth.AnimVal.Value,
                        markerElm.MarkerHeight.AnimVal.Value));


                matrix.Translate(-markerElm.RefX.AnimVal.Value * translateAndScale[2],
                    -markerElm.RefY.AnimVal.Value * translateAndScale[3]);

                matrix.Scale(translateAndScale[2], translateAndScale[3]);

                matrix.Translate(point.X, point.Y);

                _matrix = matrix;
                this.Render(renderer);

                //Clip(gr);

                renderer.RenderChildren(markerElm);

                //gr.EndContainer(gc);

                this.AfterRender(renderer);
            }
        }

        public void RenderMarkerEx0(WpfDrawingRenderer renderer, WpfDrawingContext gr,
            SvgMarkerPosition markerPos, SvgStyleableElement refElement)
        {
            //ISharpMarkerHost markerHostElm = (ISharpMarkerHost)refElement;
            //SvgMarkerElement markerElm     = (SvgMarkerElement)element;

            //SvgPointF[] vertexPositions = markerHostElm.MarkerPositions;
            //int start;
            //int len;

            //// Choose which part of the position array to use
            //switch (markerPos)
            //{
            //    case SvgMarkerPosition.Start:
            //        start = 0;
            //        len   = 1;
            //        break;
            //    case SvgMarkerPosition.Mid:
            //        start = 1;
            //        len   = vertexPositions.Length - 2;
            //        break;
            //    default:
            //        // == MarkerPosition.End
            //        start = vertexPositions.Length - 1;
            //        len   = 1;
            //        break;
            //}

            //for (int i = start; i < start + len; i++)
            //{
            //    SvgPointF point = vertexPositions[i];

            //    GdiGraphicsContainer gc = gr.BeginContainer();

            //    gr.TranslateTransform(point.X, point.Y);

            //    if (markerElm.OrientType.AnimVal.Equals(SvgMarkerOrient.Angle))
            //    {
            //        gr.RotateTransform((float)markerElm.OrientAngle.AnimVal.Value);
            //    }
            //    else
            //    {
            //        float angle;

            //        switch (markerPos)
            //        {
            //            case SvgMarkerPosition.Start:
            //                angle = markerHostElm.GetStartAngle(i + 1);
            //                break;
            //            case SvgMarkerPosition.Mid:
            //                //angle = (markerHostElm.GetEndAngle(i) + markerHostElm.GetStartAngle(i + 1)) / 2;
            //                angle = SvgNumber.CalcAngleBisection(markerHostElm.GetEndAngle(i), markerHostElm.GetStartAngle(i + 1));
            //                break;
            //            default:
            //                angle = markerHostElm.GetEndAngle(i);
            //                break;
            //        }
            //        gr.RotateTransform(angle);
            //    }

            //    if (markerElm.MarkerUnits.AnimVal.Equals(SvgMarkerUnit.StrokeWidth))
            //    {
            //        SvgLength strokeWidthLength = new SvgLength(refElement,
            //            "stroke-width", SvgLengthSource.Css, SvgLengthDirection.Viewport, "1");
            //        float strokeWidth = (float)strokeWidthLength.Value;
            //        gr.ScaleTransform(strokeWidth, strokeWidth);
            //    }

            //    SvgPreserveAspectRatio spar =
            //        (SvgPreserveAspectRatio)markerElm.PreserveAspectRatio.AnimVal;
            //    float[] translateAndScale = spar.FitToViewBox((SvgRect)markerElm.ViewBox.AnimVal,
            //        new SvgRect(0, 0, markerElm.MarkerWidth.AnimVal.Value,
            //            markerElm.MarkerHeight.AnimVal.Value));


            //    gr.TranslateTransform(-(float)markerElm.RefX.AnimVal.Value * translateAndScale[2],
            //        -(float)markerElm.RefY.AnimVal.Value * translateAndScale[3]);

            //    gr.ScaleTransform(translateAndScale[2], translateAndScale[3]);

            //    Clip(gr);

            //    renderer.RenderChildren(markerElm);

            //    gr.EndContainer(gc);
            //}
        }

        #endregion
    }
}
