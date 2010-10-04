﻿using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Indicates the method by which text should be rendered along the path.
    /// </summary>
    public enum SvgTextPathMethod
    {
        /// <summary>
        /// A value of align indicates that the glyphs should be rendered using 
        /// simple 2x3 transformations such that there is no stretching/warping 
        /// of the glyphs. Typically, supplemental rotation, scaling and translation 
        /// transformations are done for each glyph to be rendered. As a result, 
        /// with align, fonts where the glyphs are designed to be connected (e.g., 
        /// cursive fonts), the connections may not align properly when text is 
        /// rendered along a path.
        /// </summary>
        Align   = 0,

        /// <summary>
        /// A value of stretch indicates that the glyph outlines will be converted 
        /// into paths, and then all end points and control points will be adjusted 
        /// to be along the perpendicular vectors from the path, thereby stretching 
        /// and possibly warping the glyphs. With this approach, connected glyphs, 
        /// such as in cursive scripts, will maintain their connections.
        /// </summary>
        Stretch = 1
    }
}
