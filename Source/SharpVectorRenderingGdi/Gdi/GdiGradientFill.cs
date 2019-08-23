using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Summary description for PaintServer.
    /// </summary>
    public abstract class GdiGradientFill : GdiFill
    {
        #region Private Fields

        protected SvgGradientElement _gradientElement;

        #endregion

        #region Constructors and Destructor

        protected GdiGradientFill(SvgGradientElement gradientElement)
        {
            _gradientElement = gradientElement;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected List<GdiGradientStop> GetGradientStops(XmlNodeList stops)
        {
            int itemCount = stops.Count;
            if (itemCount == 0)
            {
                return new List<GdiGradientStop>();
            }
            var gradientStops = new List<GdiGradientStop>(itemCount);

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
                Color color  = Color.Transparent; // no auto-inherited...
                if (!string.IsNullOrWhiteSpace(prop) || !string.IsNullOrWhiteSpace(style))
                {
                    GdiSvgColor svgColor = new GdiSvgColor(stop, "stop-color");
                    color = svgColor.Color;
                }
                else
                {
                    color = Color.Black; // the default color...
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

                gradientStops.Add(new GdiGradientStop(color, (float)offset));
                lastOffset = offset;
            }

            if (itemCount == 0)
            {
                gradientStops.Add(new GdiGradientStop(Color.Black, 0));
                gradientStops.Add(new GdiGradientStop(Color.Black, 1));
            }

            return gradientStops;
        }

        //protected List<Color> GetColors(XmlNodeList stops)
        //{
        //    List<Color> colors = new List<Color>(stops.Count);
        //    for (int i = 0; i < stops.Count; i++)
        //    {
        //        SvgStopElement stop = (SvgStopElement)stops.Item(i);
        //        string prop = stop.GetPropertyValue("stop-color");
        //        GdiSvgColor svgColor = new GdiSvgColor(stop, "stop-color");

        //        colors.Add(svgColor.Color);
        //    }

        //    return colors;
        //}

        //protected List<float> GetPositions(XmlNodeList stops)
        //{
        //    List<float> positions = new List<float>(stops.Count);
        //    float lastPos = 0;
        //    for (int i = 0; i < stops.Count; i++)
        //    {
        //        SvgStopElement stop = (SvgStopElement)stops.Item(i);
        //        float pos = (float)stop.Offset.AnimVal;

        //        pos /= 100;
        //        pos = Math.Max(lastPos, pos);

        //        positions.Add(pos);
        //        lastPos = pos;
        //    }

        //    return positions;
        //}

        //protected void GetCorrectPositions(List<float> positions, List<Color> colors)
        //{
        //    if (positions.Count > 0)
        //    {
        //        float firstPos = positions[0];
        //        if (firstPos > 0F)
        //        {
        //            positions.Insert(0, 0F);
        //            colors.Insert(0, colors[0]);
        //        }
        //        float lastPos = positions[positions.Count - 1];
        //        if (lastPos < 1F)
        //        {
        //            positions.Add(1F);
        //            colors.Add(colors[colors.Count - 1]);
        //        }
        //    }
        //}

        //protected void GetColorsAndPositions(XmlNodeList stops, List<float> positions, List<Color> colors)
        //{
        //    List<Color> alColors    = GetColors(stops);
        //    List<float> alPositions = GetPositions(stops);

        //    if (alPositions.Count > 0)
        //    {
        //        GetCorrectPositions(alPositions, alColors);

        //        colors.AddRange(alColors);
        //        positions.AddRange(alPositions);
        //        //colors = alColors.ToArray();
        //        //positions = alPositions.ToArray();
        //    }
        //    else
        //    {
        //        //colors = new Color[2];
        //        //colors[0] = Color.Black;
        //        //colors[1] = Color.Black;

        //        colors.Add(Color.Black);
        //        colors.Add(Color.Black);

        //        //positions = new float[2];
        //        //positions[0] = 0;
        //        //positions[1] = 1;

        //        positions.Add(0);
        //        positions.Add(1);
        //    }
        //}

        protected Matrix GetTransformMatrix(SvgGradientElement gradientElement)
        {
            SvgMatrix svgMatrix = ((SvgTransformList)gradientElement.GradientTransform.AnimVal).TotalMatrix;

            Matrix transformMatrix = new Matrix((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);

            return transformMatrix;
        }

        #endregion
    }
}
