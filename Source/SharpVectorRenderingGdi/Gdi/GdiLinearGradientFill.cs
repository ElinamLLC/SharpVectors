using System;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiLinearGradientFill : GdiGradientFill
    {
        #region Private Fields


        #endregion

        #region Constructors and Destructor

        public GdiLinearGradientFill(SvgLinearGradientElement gradientElement)
            : base(gradientElement)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsUserSpace
        {
            get {
                return false;
            }
        }

        public override GdiFillType FillType
        {
            get {
                return GdiFillType.LinearGradient;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(RectangleF bounds, float opacity = 1)
        {
            return this.GetBrush((SvgLinearGradientElement)_gradientElement, bounds);
        }

        #endregion

        #region Private Methods

        private LinearGradientBrush GetBrush(SvgLinearGradientElement res, RectangleF bounds)
        {
            //LinearGradientBrush brush = new LinearGradientBrush(new RectangleF(fEffectiveLeft - 1,
            //    fEffectiveTop - 1, fEffectiveWidth + 2, fEffectiveHeight + 2),
            //    Color.White, Color.White, mode);

            XmlNodeList stops = res.Stops;

            ColorBlend cb = new ColorBlend();

            //List<Color> adjcolors = new List<Color>();
            //List<float> adjpositions = new List<float>();
            //GetColorsAndPositions(stops, adjpositions, adjcolors);
            var gradientStops = this.GetGradientStops(stops);
            if (gradientStops == null || gradientStops.Count == 0)
            {
                return null;
            }

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
                    fEffectiveLeft  = bounds.Left;
                }
                else
                {
                    fEffectiveLeft  = bounds.Left + fLeft * (bounds.Width);
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

            int stopCount = gradientStops.Count;
            var colors    = new Color[stopCount];
            var positions = new float[stopCount];
            for (int i = 0; i < stopCount; i++)
            {
                var gradientStop = gradientStops[i];
                colors[i] = gradientStop.Color;
                positions[i] = gradientStop.Offset;
                Trace.WriteLine(string.Format("color,position = {0}, {1}", colors[i], positions[i]));
            }
            //LinearGradientBrush brush = new LinearGradientBrush(new PointF(fLeft, fTop),
            //    new PointF(fRight, fBottom), gradientStops[0].Color, gradientStops[stopCount - 1].Color);
            LinearGradientBrush brush = new LinearGradientBrush(new RectangleF(fEffectiveLeft - 1,
                fEffectiveTop - 1, fEffectiveWidth + 2, fEffectiveHeight + 2),
                Color.Transparent, Color.Transparent, mode);

            if (res.GradientUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox) && !bForceUserSpaceOnUse)
            {
                if (res.SpreadMethod.AnimVal.Equals((ushort)SvgSpreadMethod.Pad))
                {
                    for (int i = 0; i < stopCount; i++)
                    {
                        if (fLeft == fRight)
                            positions[i] = fTop + positions[i] * (fBottom - fTop);
                        else
                            positions[i] = fLeft + positions[i] * (fRight - fLeft);
                    }

                    // this code corrects the values again... fix
                    int nSize = colors.Length;

                    if (positions[0] > 0.0)
                        ++nSize;

                    if (positions[colors.Length - 1] < 1)
                        ++nSize;

                    Color[] readjcolors    = new Color[nSize];
                    float[] readjpositions = new float[nSize];

                    if (positions[0] > 0.0)
                    {
                        positions.CopyTo(readjpositions, 1);
                        colors.CopyTo(readjcolors, 1);

                        readjcolors[0] = readjcolors[1];
                        readjpositions[0] = 0;
                    }
                    else
                    {
                        positions.CopyTo(readjpositions, 0);
                        colors.CopyTo(readjcolors, 0);
                    }

                    if (positions[colors.Length - 1] < 1)
                    {
                        readjcolors[nSize - 1] = readjcolors[nSize - 2];
                        readjpositions[nSize - 1] = 1;
                    }

                    cb.Colors    = readjcolors;
                    cb.Positions = readjpositions;
                }
                else
                {
                    cb.Colors    = colors;
                    cb.Positions = positions;
                }
            }
            else
            {
                //TODO
                if (positions.Length == 2 && positions[0] < 1)
                {
                    positions[1] = 1.0f;
                }
                cb.Colors    = colors;
                cb.Positions = positions;
            }

            if (colors != null && positions != null)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    Trace.WriteLine(string.Format("color,position = {0}, {1}", colors[i], positions[i]));
                }
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

            Matrix OriginalMatrix = brush.Transform.Clone();
            OriginalMatrix.Multiply(GetTransformMatrix(res));
            brush.Transform = OriginalMatrix;

            if (string.Equals(res.GetPropertyValue("color-interpolation"), 
                CssConstants.ValLinearRgb, StringComparison.OrdinalIgnoreCase))
            {
                brush.GammaCorrection = true;
            }
            else
            {
                brush.GammaCorrection = false;
            }

            return brush;
        }

        #endregion
    }
}
