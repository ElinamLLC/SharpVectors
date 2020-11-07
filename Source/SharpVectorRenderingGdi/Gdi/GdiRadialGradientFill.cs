using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiRadialGradientFill : GdiGradientFill
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public GdiRadialGradientFill(SvgRadialGradientElement gradientElement)
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
                return GdiFillType.RadialGradient;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(RectangleF bounds, float opacity = 1)
        {
            return this.GetBrush((SvgRadialGradientElement)_gradientElement, bounds);
        }

        public Region GetRadialRegion(RectangleF bounds)
        {
            SvgRadialGradientElement res = _gradientElement as SvgRadialGradientElement;

            if (_gradientElement == null)
            {
                return null;
            }

            float fCenterX = (float)res.Cx.AnimVal.Value;
            float fCenterY = (float)res.Cy.AnimVal.Value;
            float fFocusX  = (float)res.Fx.AnimVal.Value;
            float fFocusY  = (float)res.Fy.AnimVal.Value;
            float fRadius  = (float)res.R.AnimVal.Value;

            float fEffectiveCX      = fCenterX;
            float fEffectiveCY      = fCenterY;
            float fEffectiveFX      = fFocusX;
            float fEffectiveFY      = fFocusY;
            float fEffectiveRadiusX = fRadius;
            float fEffectiveRadiusY = fRadius;

            if (res.GradientUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
            {
                fEffectiveCX      = bounds.Left + fCenterX * (bounds.Width);
                fEffectiveCY      = bounds.Top + fCenterY * (bounds.Height);
                fEffectiveFX      = bounds.Left + fFocusX * (bounds.Width);
                fEffectiveFY      = bounds.Top + fFocusY * (bounds.Height);
                fEffectiveRadiusX = fRadius * bounds.Width;
                fEffectiveRadiusY = fRadius * bounds.Height;
            }

            GraphicsPath gp2 = new GraphicsPath();
            gp2.AddEllipse(fEffectiveCX - fEffectiveRadiusX, fEffectiveCY - fEffectiveRadiusY, 
                2 * fEffectiveRadiusX, 2 * fEffectiveRadiusY);

            return new Region(gp2);
        }

        #endregion

        #region Private Methods

        private PathGradientBrush GetBrush(SvgRadialGradientElement res, RectangleF bounds)
        {
            float fCenterX = (float)res.Cx.AnimVal.Value;
            float fCenterY = (float)res.Cy.AnimVal.Value;
            float fFocusX  = (float)res.Fx.AnimVal.Value;
            float fFocusY  = (float)res.Fy.AnimVal.Value;
            float fRadius  = (float)res.R.AnimVal.Value;

            float fEffectiveCX      = fCenterX;
            float fEffectiveCY      = fCenterY;
            float fEffectiveFX      = fFocusX;
            float fEffectiveFY      = fFocusY;
            float fEffectiveRadiusX = fRadius;
            float fEffectiveRadiusY = fRadius;

            if (res.GradientUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
            {
                fEffectiveCX      = bounds.Left + fCenterX * (bounds.Width);
                fEffectiveCY      = bounds.Top + fCenterY * (bounds.Height);
                fEffectiveFX      = bounds.Left + fFocusX * (bounds.Width);
                fEffectiveFY      = bounds.Top + fFocusY * (bounds.Height);
                fEffectiveRadiusX = fRadius * bounds.Width;
                fEffectiveRadiusY = fRadius * bounds.Height;
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(fEffectiveCX - fEffectiveRadiusX,
                fEffectiveCY - fEffectiveRadiusY, 2 * fEffectiveRadiusX, 2 * fEffectiveRadiusY);

            PathGradientBrush brush = new PathGradientBrush(gp);

            brush.CenterPoint = new PointF(fEffectiveFX, fEffectiveFY);

            XmlNodeList stops = res.Stops;

            //List<Color> adjcolors = new List<Color>();
            //List<float> adjpositions = new List<float>();
            //GetColorsAndPositions(stops, adjpositions, adjcolors);
            //// Need to invert the colors for some bizarre reason
            //adjcolors.Reverse();
            //adjpositions.Reverse();
            //for (int i = 0; i < adjpositions.Count; i++)
            //{
            //    adjpositions[i] = 1 - adjpositions[i];
            //}
            //cb.Colors    = adjcolors.ToArray();
            //cb.Positions = adjpositions.ToArray();

            var gradientStops = this.GetGradientStops(stops);
            gradientStops.Reverse();
            for (int i = 0; i < gradientStops.Count; i++)
            {
                gradientStops[i].Offset = 1 - gradientStops[i].Offset;
            }

            int stopCount = gradientStops.Count;

            ColorBlend cb = new ColorBlend(stopCount);

            var colors    = new Color[stopCount];
            var positions = new float[stopCount];
            for (int i = 0; i < stopCount; i++)
            {
                var gradientStop = gradientStops[i];
                colors[i]    = gradientStop.Color;
                positions[i] = gradientStop.Offset;
            }

            cb.Colors    = colors;
            cb.Positions = positions;

            brush.InterpolationColors = cb;

            //			ISvgTransformable transElm = (ISvgTransformable)res;
            //			SvgTransformList svgTList = (SvgTransformList)transElm.transform.AnimVal;
            //			brush.Transform = svgTList.matrix.matrix;

            //if (res.GetPropertyValue("color-interpolation") == CssConstants.ValLinearRgb)
            //{
            //    //GdipSetPathGradientGammaCorrection(brush, true);
            //}
            //else
            //{
            //    //GdipSetPathGradientGammaCorrection(brush, false);
            //}

            ///*
            //       * How to do brush.GammaCorrection = true on a PathGradientBrush? / nikgus
            //       * */

            return brush;
        }

        #endregion
    }
}
