using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfGradientFill : WpfFill
    {
        #region Private Fields

        private SvgGradientElement _gradientElement;

        #endregion

        #region Constructors and Destructor

        public WpfGradientFill(SvgGradientElement gradientElement)
        {
            _gradientElement = gradientElement;
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(WpfDrawingContext context)
        {
            if (_gradientElement is SvgLinearGradientElement)
            {
                return GetLinearGradientBrush((SvgLinearGradientElement)_gradientElement);
            }
            else if (_gradientElement is SvgRadialGradientElement)
            {
                return GetRadialGradientBrush((SvgRadialGradientElement)_gradientElement);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        #endregion

        #region Private Methods

        private LinearGradientBrush GetLinearGradientBrush(SvgLinearGradientElement res)
        {
            double x1 = res.X1.AnimVal.Value;
            double x2 = res.X2.AnimVal.Value;
            double y1 = res.Y1.AnimVal.Value;
            double y2 = res.Y2.AnimVal.Value;

            GradientStopCollection gradientStops = GetGradientStops(res.Stops);

            LinearGradientBrush brush = new LinearGradientBrush(gradientStops,
                new Point(x1, y1), new Point(x2, y2));

            SvgSpreadMethod spreadMethod = SvgSpreadMethod.None;
            if (res.SpreadMethod != null)
            {
                spreadMethod = (SvgSpreadMethod)res.SpreadMethod.AnimVal;

                if (spreadMethod != SvgSpreadMethod.None)
                {
                    brush.SpreadMethod = WpfConverter.ToSpreadMethod(spreadMethod);
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
                }
            }

            MatrixTransform transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                brush.Transform = transform;
            }
            //else
            //{
            //    float fLeft = (float)res.X1.AnimVal.Value;
            //    float fRight = (float)res.X2.AnimVal.Value;
            //    float fTop = (float)res.Y1.AnimVal.Value;
            //    float fBottom = (float)res.Y2.AnimVal.Value;

            //    if (fTop == fBottom)
            //    {
            //        //mode = LinearGradientMode.Horizontal;
            //    }
            //    else
            //    {
            //        if (fLeft == fRight)
            //        {
            //            //mode = LinearGradientMode.Vertical;
            //        }
            //        else
            //        {
            //            if (fLeft < fRight)
            //            {
            //                //mode = LinearGradientMode.ForwardDiagonal;
            //                brush.Transform = new RotateTransform(45, 0, 0);
            //                //brush.EndPoint = new Point(x1, y1 + 1);
            //            }
            //            else
            //            {
            //                //mode = LinearGradientMode.BackwardDiagonal;
            //                brush.Transform = new RotateTransform(-45);
            //            }
            //        }
            //    }
            //}

            string colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!String.IsNullOrEmpty(colorInterpolation))
            {
                if (colorInterpolation == "linearRGB")
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                }
                else
                {
                    brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
                }
            }

            return brush;
        }

        private RadialGradientBrush GetRadialGradientBrush(SvgRadialGradientElement res)
        {
            double centerX = res.Cx.AnimVal.Value;
            double centerY = res.Cy.AnimVal.Value;
            double focusX  = res.Fx.AnimVal.Value;
            double focusY  = res.Fy.AnimVal.Value;
            double radius  = res.R.AnimVal.Value;

            GradientStopCollection gradientStops = GetGradientStops(res.Stops);

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
                    brush.SpreadMethod = WpfConverter.ToSpreadMethod(spreadMethod);
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
                }
            }

            MatrixTransform transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                brush.Transform = transform;
            }

            string colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!String.IsNullOrEmpty(colorInterpolation))
            {
                if (colorInterpolation == "linearRGB")
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
            GradientStopCollection gradientStops = new GradientStopCollection(itemCount);

            double lastOffset = 0;
            for (int i = 0; i < itemCount; i++)
            {
                SvgStopElement stop = (SvgStopElement)stops.Item(i);
                string prop = stop.GetPropertyValue("stop-color");
                WpfSvgColor svgColor = new WpfSvgColor(stop, "stop-color");

                double offset = stop.Offset.AnimVal;

                offset /= 100;
                offset = Math.Max(lastOffset, offset);

                gradientStops.Add(new GradientStop(svgColor.Color, offset));
                lastOffset = offset;
            }

            if (itemCount == 0)
            {
                gradientStops.Add(new GradientStop(Colors.Black, 0));
                gradientStops.Add(new GradientStop(Colors.Black, 1));
            }

            return gradientStops;
        }

        #endregion
    }
}
