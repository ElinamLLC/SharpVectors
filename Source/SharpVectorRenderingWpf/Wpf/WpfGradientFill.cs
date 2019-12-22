using System;
using System.Xml;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfGradientFill : WpfFill
    {
        #region Private Fields

        private bool _isUserSpace;
        private SvgGradientElement _gradientElement;

        #endregion

        #region Constructors and Destructor

        public WpfGradientFill(SvgGradientElement gradientElement)
        {
            _isUserSpace     = false;
            _gradientElement = gradientElement;
        }

        #endregion

        #region Public Properties

        public override bool IsUserSpace
        {
            get {
                return _isUserSpace;
            }
        }

        public override WpfFillType FillType
        {
            get {
                return WpfFillType.Gradient;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context, Transform viewTransform)
        {
            SvgLinearGradientElement linearGrad = _gradientElement as SvgLinearGradientElement;
            if (linearGrad != null)
            {
                return GetLinearGradientBrush(linearGrad, viewTransform);
            }

            SvgRadialGradientElement radialGrad = _gradientElement as SvgRadialGradientElement;
            if (radialGrad != null)
            {
                return GetRadialGradientBrush(radialGrad);
            }

            return new SolidColorBrush(Colors.Black);
        }

        #endregion

        #region Private Methods

        private Brush GetLinearGradientBrush(SvgLinearGradientElement res, Transform viewBoxTransform = null)
        {
            GradientStopCollection gradientStops = GetGradientStops(res.Stops);
            if (gradientStops == null || gradientStops.Count == 0)
            {
                return null;
            }

            double x1 = res.X1.AnimVal.Value;
            double x2 = res.X2.AnimVal.Value;
            double y1 = res.Y1.AnimVal.Value;
            double y2 = res.Y2.AnimVal.Value;

            LinearGradientBrush brush = new LinearGradientBrush(gradientStops,
                new Point(x1, y1), new Point(x2, y2));

            SvgSpreadMethod spreadMethod = SvgSpreadMethod.Pad;
            if (res.SpreadMethod != null)
            {
                spreadMethod = (SvgSpreadMethod)res.SpreadMethod.AnimVal;

                if (spreadMethod != SvgSpreadMethod.None)
                {
                    brush.SpreadMethod = WpfConvert.ToSpreadMethod(spreadMethod);
                }
            }

            SvgUnitType mappingMode = SvgUnitType.ObjectBoundingBox;
            if (res.GradientUnits != null)
            {
                mappingMode = (SvgUnitType)res.GradientUnits.AnimVal;
                if (mappingMode == SvgUnitType.ObjectBoundingBox)
                {
                    brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                }
                else if (mappingMode == SvgUnitType.UserSpaceOnUse)
                {
                    brush.MappingMode = BrushMappingMode.Absolute;

                    _isUserSpace = true;
                }
            }

            string colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!string.IsNullOrWhiteSpace(colorInterpolation))
            {
                if (string.Equals(colorInterpolation, "linearRGB", StringComparison.OrdinalIgnoreCase))
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                }
                else
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
                }
            }

            MatrixTransform transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                if (viewBoxTransform != null && !viewBoxTransform.Value.IsIdentity)
                {
                    TransformGroup group = new TransformGroup();
                    group.Children.Add(viewBoxTransform);
                    group.Children.Add(transform);

                    brush.Transform = group;
                }
                else
                {
                    brush.Transform = transform;
                }
            }
            else
            {
                float fLeft   = (float)res.X1.AnimVal.Value;
                float fRight  = (float)res.X2.AnimVal.Value;
                float fTop    = (float)res.Y1.AnimVal.Value;
                float fBottom = (float)res.Y2.AnimVal.Value;

                if (mappingMode == SvgUnitType.ObjectBoundingBox)
                {
                    if (!fTop.Equals(fBottom) && !fLeft.Equals(fRight))
                    {
                        var drawingBrush = new DrawingBrush();
                        drawingBrush.Stretch = Stretch.Fill;
                        drawingBrush.Viewbox = new Rect(0, 0, 1, 1);
                        var DrawingRect = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0, 0, 1, 1)));
                        drawingBrush.Drawing = DrawingRect;
                        return drawingBrush;
                    }
                }

                if (fTop.Equals(fBottom))
                {
                }
                else
                {
                    if (fLeft.Equals(fRight))
                    {
                    }
                    else
                    {
                        if (fLeft < fRight)
                        {
                            if (viewBoxTransform != null && !viewBoxTransform.Value.IsIdentity)
                            {
                                brush.Transform = viewBoxTransform;
                            }
                        }
                        else
                        {
                            if (viewBoxTransform != null && !viewBoxTransform.Value.IsIdentity)
                            {
                                brush.Transform = viewBoxTransform;
                            }
                        }
                    }
                }
            }

            return brush;
        }

        private Brush GetRadialGradientBrush(SvgRadialGradientElement res)
        {
            var refElem = res.ReferencedElement;

            double centerX = res.Cx.AnimVal.Value;
            double centerY = res.Cy.AnimVal.Value;

            // 'fx', 'fy', and 'fr' define the start circle for the radial gradient.
            double focusX  = res.Fx.AnimVal.Value;
            double focusY  = res.Fy.AnimVal.Value;
            double radius  = res.R.AnimVal.Value;

            var lengthUnit = res.Cx.AnimVal.UnitType;
            // If attribute 'fx' is not specified, 'fx' will coincide with the presentational 
            // value of 'cx' for the element whether the value for 'cx' was inherited or not. 
            if (lengthUnit == SvgLengthType.Percentage)
            {
                if (!res.HasAttribute("fx") && (refElem == null || !refElem.HasAttribute("fx")))
                {
                    focusX = centerX;
                }
                else if (focusX.Equals(0.0))
                {
                    focusX = centerX;
                    if (focusX > 0 && focusX >= radius)
                    {
                        focusX = (centerX > radius) ? centerX - radius : focusX = radius;
                    }
                }
                else
                {
                    if (focusX > 0 && focusX >= radius)
                    {
                        focusX = (centerX > radius) ? centerX - radius : focusX = radius;
                    }
                }
            }

            lengthUnit = res.Cy.AnimVal.UnitType;
            // If attribute 'fy' is not specified, 'fy' will coincide with the presentational 
            // value of 'cy' for the element whether the value for 'cy' was inherited or not.
            if (lengthUnit == SvgLengthType.Percentage)
            {
                if (!res.HasAttribute("fy") && (refElem == null || !refElem.HasAttribute("fy")))
                {
                    focusY = centerY;
                }
                else if (focusY.Equals(0.0))
                {
                    focusY = centerY;
                    if (focusY > 0 && focusY >= radius)
                    {
                        focusY = (centerY > radius) ? centerY - radius : focusY = radius;
                    }
                }
                else
                {
                    if (focusY > 0 && focusY >= radius)
                    {
                        focusY = (centerY > radius) ? centerY - radius : focusY = radius;
                    }
                }
            }

            GradientStopCollection gradientStops = GetGradientStops(res.Stops);
            if (gradientStops == null || gradientStops.Count == 0)
            {
                return null;
            }

            RadialGradientBrush brush = new RadialGradientBrush(gradientStops);

            brush.RadiusX = radius;
            brush.RadiusY = radius;
            brush.Center  = new Point(centerX, centerY);
            brush.GradientOrigin = new Point(focusX, focusY);

            if (res.SpreadMethod != null)
            {
                SvgSpreadMethod spreadMethod = (SvgSpreadMethod)res.SpreadMethod.AnimVal;

                if (spreadMethod != SvgSpreadMethod.None)
                {
                    brush.SpreadMethod = WpfConvert.ToSpreadMethod(spreadMethod);
                }
            }
            if (res.GradientUnits != null)
            {
                SvgUnitType mappingMode = (SvgUnitType)res.GradientUnits.AnimVal;
                if (mappingMode == SvgUnitType.ObjectBoundingBox)
                {
                    brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                }
                else if (mappingMode == SvgUnitType.UserSpaceOnUse)
                {
                    brush.MappingMode = BrushMappingMode.Absolute;

                    if (res.Fx.AnimVal.UnitType == SvgLengthType.Percentage)
                    {
                        brush.GradientOrigin = brush.Center;
                    }

                    _isUserSpace = true;
                }
            }

            MatrixTransform transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                brush.Transform = transform;
            }

            string colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!string.IsNullOrWhiteSpace(colorInterpolation))
            {
                if (string.Equals(colorInterpolation, "linearRGB", StringComparison.OrdinalIgnoreCase))
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
                }
                else
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                }
            }

            return brush;
        }

        private MatrixTransform GetTransformMatrix(SvgGradientElement gradientElement)
        {
            SvgMatrix svgMatrix = 
                ((SvgTransformList)gradientElement.GradientTransform.AnimVal).TotalMatrix;

            MatrixTransform transformMatrix = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);

            return transformMatrix;
        }

        private GradientStopCollection GetGradientStops(XmlNodeList stops)
        {
            int itemCount = stops.Count;
            if (itemCount == 0)
            {
                return new GradientStopCollection();
            }
            GradientStopCollection gradientStops = new GradientStopCollection(itemCount);

            double lastOffset = 0;
            for (int i = 0; i < itemCount; i++)
            {
                SvgStopElement stop = (SvgStopElement)stops.Item(i);
                if (stop == null)
                {
                    continue;
                }
                string prop  = stop.GetAttribute("stop-color");
                string style = stop.GetAttribute("style");
                Color color  = Colors.Transparent; // no auto-inherited...
                if (!string.IsNullOrWhiteSpace(prop) || !string.IsNullOrWhiteSpace(style))
                {
                    WpfSvgColor svgColor = new WpfSvgColor(stop, "stop-color");
                    color = svgColor.Color;
                }
                else
                {
                    color = Colors.Black; // the default color...
                    double alpha = 255;
                    string opacity;

                    opacity = stop.GetAttribute("stop-opacity"); // no auto-inherit
                    if (opacity == "inherit") // if explicitly defined...
                    {
                        opacity = stop.GetPropertyValue("stop-opacity");
                    }
                    if (!string.IsNullOrWhiteSpace(opacity))
                    {
                        alpha *= SvgNumber.ParseNumber(opacity);
                    }

                    alpha = Math.Min(alpha, 255);
                    alpha = Math.Max(alpha, 0);

                    color = Color.FromArgb((byte)Convert.ToInt32(alpha), color.R, color.G, color.B);
                }

                double offset = (stop.Offset == null) ? 0 : stop.Offset.AnimVal;

                offset /= 100;
                offset = Math.Max(lastOffset, offset);

                gradientStops.Add(new GradientStop(color, offset));
                lastOffset = offset;
            }

            if (itemCount == 0)
            {
                gradientStops.Add(new GradientStop(Colors.Black, 0));
                gradientStops.Add(new GradientStop(Colors.Black, 1));
            }

            return gradientStops;
        }

        private Transform FitToViewbox(SvgRect viewBox, Rect rectToFit)
        {
            SvgPreserveAspectRatioType alignment = SvgPreserveAspectRatioType.XMidYMid;

            double[] transformArray = FitToViewBox(alignment, viewBox, 
                new SvgRect(rectToFit.X, rectToFit.Y, rectToFit.Width, rectToFit.Height));

            double translateX = transformArray[0];
            double translateY = transformArray[1];
            double scaleX     = transformArray[2];
            double scaleY     = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix = null;

            if (!translateX.Equals(0) || !translateY.Equals(0))
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if (!scaleX.Equals(1.0f) || !scaleY.Equals(1.0f))
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                if (translateMatrix.Value.IsIdentity && scaleMatrix.Value.IsIdentity)
                {
                    return null;
                }
                if (translateMatrix.Value.IsIdentity)
                {
                    return scaleMatrix;
                }
                if (scaleMatrix.Value.IsIdentity)
                {
                    return translateMatrix;
                }

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                return transformGroup;
            }

            if (translateMatrix != null)
            {
                return translateMatrix.Value.IsIdentity ? null : translateMatrix;
            }

            if (scaleMatrix != null)
            {
                return scaleMatrix.Value.IsIdentity ? null : scaleMatrix;
            }

            return null;
        }

        private double[] FitToViewBox(SvgPreserveAspectRatioType alignment, SvgRect viewBox, SvgRect rectToFit)
        {
            double translateX = 0;
            double translateY = 0;
            double scaleX = 1;
            double scaleY = 1;

            if (!viewBox.IsEmpty)
            {
                // calculate scale values for non-uniform scaling
                scaleX = rectToFit.Width / viewBox.Width;
                scaleY = rectToFit.Height / viewBox.Height;

                if (alignment != SvgPreserveAspectRatioType.None)
                {
                    // uniform scaling
                    scaleX = Math.Max(scaleX, scaleY);

                    scaleY = scaleX;

                    if (alignment == SvgPreserveAspectRatioType.XMidYMax ||
                      alignment == SvgPreserveAspectRatioType.XMidYMid ||
                      alignment == SvgPreserveAspectRatioType.XMidYMin)
                    {
                        // align to the Middle X
                        translateX = (rectToFit.X + rectToFit.Width / 2) - scaleX * (viewBox.X + viewBox.Width / 2);
                    }
                    else if (alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                      alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                      alignment == SvgPreserveAspectRatioType.XMaxYMin)
                    {
                        // align to the right X
                        translateX = (rectToFit.Width - viewBox.Width * scaleX);
                    }

                    if (alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                      alignment == SvgPreserveAspectRatioType.XMidYMid ||
                      alignment == SvgPreserveAspectRatioType.XMinYMid)
                    {
                        // align to the Middle Y
                        translateY = (rectToFit.Y + rectToFit.Height / 2) - scaleY * (viewBox.Y + viewBox.Height / 2);
                    }
                    else if (alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                      alignment == SvgPreserveAspectRatioType.XMidYMax ||
                      alignment == SvgPreserveAspectRatioType.XMinYMax)
                    {
                        // align to the bottom Y
                        translateY = (rectToFit.Height - viewBox.Height * scaleY);
                    }
                }
                else
                {
                    translateX = -viewBox.X * scaleX;
                    translateY = -viewBox.Y * scaleY;
                }
            }

            if (!SvgNumber.IsValid(translateX))
            {
                translateX = 0;
            }
            if (!SvgNumber.IsValid(translateY))
            {
                translateY = 0;
            }
            if (!SvgNumber.IsValid(scaleX))
            {
                scaleX = 1;
            }
            if (!SvgNumber.IsValid(scaleY))
            {
                scaleY = 1;
            }

            return new double[]{ translateX, translateY, scaleX, scaleY };
        }        

        #endregion
    }
}
