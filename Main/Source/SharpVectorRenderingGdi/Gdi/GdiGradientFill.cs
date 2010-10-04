using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Summary description for PaintServer.
    /// </summary>
    public sealed class GdiGradientFill : GdiFill
    {
        #region Private Fields

        private SvgGradientElement _gradientElement;

        #endregion

        #region Constructors and Destructor

        public GdiGradientFill(SvgGradientElement gradientElement)
        {
            _gradientElement = gradientElement;
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(RectangleF bounds)
        {
            SvgLinearGradientElement linearGradient = _gradientElement as SvgLinearGradientElement;
            if (linearGradient != null)
            {
                return GetLinearGradientBrush(linearGradient, bounds);
            }

            SvgRadialGradientElement radialGradient = _gradientElement as SvgRadialGradientElement;
            if (radialGradient != null)
            {
                return GetRadialGradientBrush(radialGradient, bounds);
            }

            return new SolidBrush(Color.Black);
        }

        public Region GetRadialGradientRegion(RectangleF bounds)
        {
            SvgRadialGradientElement res = _gradientElement as SvgRadialGradientElement;

            if (_gradientElement == null)
            {
                return null;
            }

            float fCenterX = (float)res.Cx.AnimVal.Value;
            float fCenterY = (float)res.Cy.AnimVal.Value;
            float fFocusX = (float)res.Fx.AnimVal.Value;
            float fFocusY = (float)res.Fy.AnimVal.Value;
            float fRadius = (float)res.R.AnimVal.Value;

            float fEffectiveCX = fCenterX;
            float fEffectiveCY = fCenterY;
            float fEffectiveFX = fFocusX;
            float fEffectiveFY = fFocusY;
            float fEffectiveRadiusX = fRadius;
            float fEffectiveRadiusY = fRadius;

            if (res.GradientUnits.AnimVal.Equals(SvgUnitType.ObjectBoundingBox))
            {
                fEffectiveCX = bounds.Left + fCenterX * (bounds.Width);
                fEffectiveCY = bounds.Top + fCenterY * (bounds.Height);
                fEffectiveFX = bounds.Left + fFocusX * (bounds.Width);
                fEffectiveFY = bounds.Top + fFocusY * (bounds.Height);
                fEffectiveRadiusX = fRadius * bounds.Width;
                fEffectiveRadiusY = fRadius * bounds.Height;
            }

            GraphicsPath gp2 = new GraphicsPath();
            gp2.AddEllipse(fEffectiveCX - fEffectiveRadiusX, fEffectiveCY - fEffectiveRadiusY, 2 * fEffectiveRadiusX, 2 * fEffectiveRadiusY);

            return new Region(gp2);
        }

        #endregion

        #region Private Methods

        private List<Color> GetColors(XmlNodeList stops)
        {
            List<Color> colors = new List<Color>(stops.Count);
            for (int i = 0; i < stops.Count; i++)
            {
                SvgStopElement stop = (SvgStopElement)stops.Item(i);
                string prop = stop.GetPropertyValue("stop-color");
                GdiSvgColor svgColor = new GdiSvgColor(stop, "stop-color");

                colors.Add(svgColor.Color);
            }

            return colors;
        }

        private List<float> GetPositions(XmlNodeList stops)
        {
            List<float> positions = new List<float>(stops.Count);
            float lastPos = 0;
            for (int i = 0; i < stops.Count; i++)
            {
                SvgStopElement stop = (SvgStopElement)stops.Item(i);
                float pos = (float)stop.Offset.AnimVal;

                pos /= 100;
                pos = Math.Max(lastPos, pos);

                positions.Add(pos);
                lastPos = pos;
            }

            return positions;
        }

        private void GetCorrectPositions(List<float> positions, List<Color> colors)
        {
            if (positions.Count > 0)
            {
                float firstPos = positions[0];
                if (firstPos > 0F)
                {
                    positions.Insert(0, 0F);
                    colors.Insert(0, colors[0]);
                }
                float lastPos = positions[positions.Count - 1];
                if (lastPos < 1F)
                {
                    positions.Add(1F);
                    colors.Add(colors[colors.Count - 1]);
                }
            }
        }

        private void GetColorsAndPositions(XmlNodeList stops, List<float> positions, List<Color> colors)
        {
            List<Color> alColors    = GetColors(stops);
            List<float> alPositions = GetPositions(stops);

            if (alPositions.Count > 0)
            {
                GetCorrectPositions(alPositions, alColors);

                colors.AddRange(alColors);
                positions.AddRange(alPositions);
                //colors = alColors.ToArray();
                //positions = alPositions.ToArray();
            }
            else
            {
                //colors = new Color[2];
                //colors[0] = Color.Black;
                //colors[1] = Color.Black;

                colors.Add(Color.Black);
                colors.Add(Color.Black);

                //positions = new float[2];
                //positions[0] = 0;
                //positions[1] = 1;

                positions.Add(0);
                positions.Add(1);
            }
        }

        private LinearGradientBrush GetLinearGradientBrush(SvgLinearGradientElement res, RectangleF bounds)
        {
            float fLeft   = (float)res.X1.AnimVal.Value;
            float fRight  = (float)res.X2.AnimVal.Value;
            float fTop    = (float)res.Y1.AnimVal.Value;
            float fBottom = (float)res.Y2.AnimVal.Value;

            bool bForceUserSpaceOnUse = (fLeft > 1 || fRight > 1 || fTop > 1 || fBottom > 1);

            float fEffectiveLeft   = fLeft;
            float fEffectiveRight  = fRight;
            float fEffectiveTop    = fTop;
            float fEffectiveBottom = fBottom;

            if (res.GradientUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox) && !bForceUserSpaceOnUse)
            {
                if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Pad))
                {
                    fEffectiveRight = bounds.Right;
                    fEffectiveLeft = bounds.Left;
                }
                else
                {
                    fEffectiveLeft = bounds.Left + fLeft * (bounds.Width);
                    fEffectiveRight = bounds.Left + fRight * (bounds.Width);
                }

                fEffectiveTop    = bounds.Top + fTop * (bounds.Height);
                fEffectiveBottom = bounds.Top + fBottom * (bounds.Height);
            }

            LinearGradientMode mode = LinearGradientMode.Horizontal;

            if (fTop == fBottom)
            {
                mode = LinearGradientMode.Horizontal;
            }
            else
            {
                if (fLeft == fRight)
                {
                    mode = LinearGradientMode.Vertical;
                }
                else
                {
                    if (fLeft < fRight)
                        mode = LinearGradientMode.ForwardDiagonal;
                    else
                        mode = LinearGradientMode.BackwardDiagonal;
                }
            }

            float fEffectiveWidth = fEffectiveRight - fEffectiveLeft;

            if (fEffectiveWidth <= 0)
                fEffectiveWidth = bounds.Width;

            float fEffectiveHeight = fEffectiveBottom - fEffectiveTop;

            if (fEffectiveHeight <= 0)
                fEffectiveHeight = bounds.Height;

            LinearGradientBrush brush = new LinearGradientBrush(new RectangleF(fEffectiveLeft - 1, 
                fEffectiveTop - 1, fEffectiveWidth + 2, fEffectiveHeight + 2), 
                Color.White, Color.White, mode);

            XmlNodeList stops = res.Stops;

            ColorBlend cb = new ColorBlend();

            List<Color> adjcolors    = new List<Color>();
            List<float> adjpositions = new List<float>();
            GetColorsAndPositions(stops, adjpositions, adjcolors);

            if (res.GradientUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox) && !bForceUserSpaceOnUse)
            {
                if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Pad))
                {
                    for (int i = 0; i < adjpositions.Count; i++)
                    {
                        if (fLeft == fRight)
                            adjpositions[i] = fTop + adjpositions[i] * (fBottom - fTop);
                        else
                            adjpositions[i] = fLeft + adjpositions[i] * (fRight - fLeft);
                    }

                    // this code corrects the values again... fix
                    int nSize = adjcolors.Count;

                    if (adjpositions[0] > 0.0)
                        ++nSize;

                    if (adjpositions[adjcolors.Count - 1] < 1)
                        ++nSize;

                    Color[] readjcolors    = new Color[nSize];
                    float[] readjpositions = new float[nSize];

                    if (adjpositions[0] > 0.0)
                    {
                        adjpositions.CopyTo(readjpositions, 1);
                        adjcolors.CopyTo(readjcolors, 1);

                        readjcolors[0]    = readjcolors[1];
                        readjpositions[0] = 0;
                    }
                    else
                    {
                        adjpositions.CopyTo(readjpositions, 0);
                        adjcolors.CopyTo(readjcolors, 0);
                    }

                    if (adjpositions[adjcolors.Count - 1] < 1)
                    {
                        readjcolors[nSize - 1]    = readjcolors[nSize - 2];
                        readjpositions[nSize - 1] = 1;
                    }

                    cb.Colors    = readjcolors;
                    cb.Positions = readjpositions;
                }
                else
                {
                    cb.Colors    = adjcolors.ToArray();
                    cb.Positions = adjpositions.ToArray();
                }
            }
            else
            {
                cb.Colors    = adjcolors.ToArray();
                cb.Positions = adjpositions.ToArray();
            }

            brush.InterpolationColors = cb;

            if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Reflect))
            {
                brush.WrapMode = WrapMode.TileFlipXY;
            }
            else if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Repeat))
            {
                brush.WrapMode = WrapMode.Tile;
            }
            else if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Pad))
            {
                brush.WrapMode = WrapMode.Tile;
            }

            brush.Transform = GetTransformMatrix(res);

            if (res.GetPropertyValue("color-interpolation") == "linearRGB")
            {
                brush.GammaCorrection = true;
            }
            else
            {
                brush.GammaCorrection = false;
            }

            return brush;
        }

        private Matrix GetTransformMatrix(SvgGradientElement gradientElement)
        {
            SvgMatrix svgMatrix = ((SvgTransformList)gradientElement.GradientTransform.AnimVal).TotalMatrix;

            Matrix transformMatrix = new Matrix((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);

            return transformMatrix;
        }

        private PathGradientBrush GetRadialGradientBrush(SvgRadialGradientElement res, RectangleF bounds)
        {
            float fCenterX = (float)res.Cx.AnimVal.Value;
            float fCenterY = (float)res.Cy.AnimVal.Value;
            float fFocusX  = (float)res.Fx.AnimVal.Value;
            float fFocusY  = (float)res.Fy.AnimVal.Value;
            float fRadius  = (float)res.R.AnimVal.Value;

            float fEffectiveCX = fCenterX;
            float fEffectiveCY = fCenterY;
            float fEffectiveFX = fFocusX;
            float fEffectiveFY = fFocusY;
            float fEffectiveRadiusX = fRadius;
            float fEffectiveRadiusY = fRadius;

            if (res.GradientUnits.AnimVal.Equals(SvgUnitType.ObjectBoundingBox))
            {
                fEffectiveCX = bounds.Left + fCenterX * (bounds.Width);
                fEffectiveCY = bounds.Top + fCenterY * (bounds.Height);
                fEffectiveFX = bounds.Left + fFocusX * (bounds.Width);
                fEffectiveFY = bounds.Top + fFocusY * (bounds.Height);
                fEffectiveRadiusX = fRadius * bounds.Width;
                fEffectiveRadiusY = fRadius * bounds.Height;
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(fEffectiveCX - fEffectiveRadiusX, 
                fEffectiveCY - fEffectiveRadiusY, 2 * fEffectiveRadiusX, 2 * fEffectiveRadiusY);

            PathGradientBrush brush = new PathGradientBrush(gp);

            brush.CenterPoint = new PointF(fEffectiveFX, fEffectiveFY);

            XmlNodeList stops = res.Stops;

            ColorBlend cb = new ColorBlend();

            List<Color> adjcolors    = new List<Color>();
            List<float> adjpositions = new List<float>();
            GetColorsAndPositions(stops, adjpositions, adjcolors);

            // Need to invert the colors for some bizarre reason
            adjcolors.Reverse();
            adjpositions.Reverse();
            for (int i = 0; i < adjpositions.Count; i++)
            {
                adjpositions[i] = 1 - adjpositions[i];

            }

            cb.Colors    = adjcolors.ToArray();
            cb.Positions = adjpositions.ToArray();

            brush.InterpolationColors = cb;

            //			ISvgTransformable transElm = (ISvgTransformable)res;
            //			SvgTransformList svgTList = (SvgTransformList)transElm.transform.AnimVal;
            //			brush.Transform = svgTList.matrix.matrix;

            if (res.GetPropertyValue("color-interpolation") == "linearRGB")
            {
                //GdipSetPathGradientGammaCorrection(brush, true);
            }
            else
            {
                //GdipSetPathGradientGammaCorrection(brush, false);
            }

            /*
                   * How to do brush.GammaCorrection = true on a PathGradientBrush? / nikgus
                   * */

            return brush;
        }

        #endregion
    }
}
