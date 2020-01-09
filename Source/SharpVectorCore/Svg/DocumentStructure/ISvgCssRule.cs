using System;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// SVG extends interface <see cref="ICssRule"/> with interface <see cref="ISvgCssRule"/> by adding an 
    /// <see cref="ISvgColorProfileRule"/> rule to allow for specification of ICC-based color.
    /// </summary>
    public interface ISvgCssRule : ICssRule
    {
    }
}
