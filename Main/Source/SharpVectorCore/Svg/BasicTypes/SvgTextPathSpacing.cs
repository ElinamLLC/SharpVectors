using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Indicates how the user agent should determine the spacing between glyphs 
    /// that are to be rendered along a path.
    /// </summary>
    public enum SvgTextPathSpacing
    {
        /// <summary>
        /// A value of exact indicates that the glyphs should be rendered exactly 
        /// according to the spacing rules as specified in "Text on a path layout rules".
        /// </summary>
        Exact = 0,

        /// <summary>
        /// A value of auto indicates that the user agent should use text-on-a-path 
        /// layout algorithms to adjust the spacing between glyphs in order to achieve 
        /// visually appealing results.
        /// </summary>
        Auto = 1
    }
}
