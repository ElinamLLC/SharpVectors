using System;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// Represents the SVG paint-order CSS property value.
    /// Specifies the order in which fill, stroke, and markers are rendered on an element.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The paint-order property allows you to change the rendering order of fill, stroke, and markers.
    /// By default (Normal), fill is drawn first, then stroke (which appears on top), then markers.
    /// </para>
    /// <para>
    /// This is particularly useful for:
    /// - Icons with visible outlines/strokes
    /// - Text with colored outlines
    /// - Graphics where the stroke should be the primary visual
    /// </para>
    /// <para>
    /// Reference: https://www.w3.org/TR/svg2/painting.html#PaintOrder
    /// </para>
    /// </remarks>
    public enum WpfPaintOrder
    {
        /// <summary>
        /// Default: fill, stroke, markers
        /// Fill is drawn first, stroke appears on top (WPF default behavior)
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Stroke first: stroke, fill, markers
        /// Stroke is drawn first, fill appears on top
        /// This creates the effect of an outline with a colored interior
        /// </summary>
        Stroke = 1,

        /// <summary>
        /// Fill explicit: fill, stroke, markers
        /// Explicit specification of fill-first order (same as Normal)
        /// </summary>
        Fill = 2,

        /// <summary>
        /// Markers first: markers, stroke, fill
        /// Markers are drawn first (for future enhanced marker support)
        /// </summary>
        Markers = 3,
    }

    /// <summary>
    /// Helper class for parsing and handling the paint-order CSS property
    /// </summary>
    public static class WpfPaintOrderHelper
    {
        /// <summary>
        /// Parse a paint-order CSS value string to the corresponding enum value
        /// </summary>
        /// <param name="paintOrderValue">
        /// The CSS paint-order value to parse.
        /// Can be null, empty, whitespace, or one of: "normal", "stroke", "fill", "markers"
        /// Can also be space-separated keywords like "stroke fill", "markers stroke fill", etc.
        /// </param>
        /// <returns>
        /// The corresponding WpfPaintOrder enum value.
        /// Returns Normal if the value is null, empty, or unrecognized.
        /// Parsing is case-insensitive.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Phase 2 Implementation (Enhanced): Now supports multi-keyword paint-order values.
        /// The first keyword in the sequence determines the rendering behavior:
        /// - "stroke fill" → interpreted as Stroke (stroke-first order)
        /// - "fill stroke" → interpreted as Fill (fill-first order)
        /// - "markers stroke fill" → interpreted as Markers (markers-first order, Phase 3+)
        /// </para>
        /// <para>
        /// Subsequent keywords are reserved for future phases that may support
        /// more granular multi-order rendering control.
        /// </para>
        /// </remarks>
        public static WpfPaintOrder Parse(string paintOrderValue)
        {
            if (string.IsNullOrWhiteSpace(paintOrderValue))
            {
                return WpfPaintOrder.Normal;
            }

            paintOrderValue = paintOrderValue.Trim().ToLowerInvariant();

            // Phase 2: Support multi-keyword values by parsing the first keyword
            // Split by whitespace to handle "stroke fill", "markers stroke fill", etc.
            var keywords = paintOrderValue.Split(new[] { ' ', '\t', '\n', '\r' }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (keywords.Length == 0)
            {
                return WpfPaintOrder.Normal;
            }

            // Parse the first keyword to determine the paint order
            string firstKeyword = keywords[0];

            switch (firstKeyword)
            {
                case "stroke":
                    return WpfPaintOrder.Stroke;

                case "fill":
                    return WpfPaintOrder.Fill;

                case "markers":
                    return WpfPaintOrder.Markers;

                case "normal":
                    return WpfPaintOrder.Normal;

                default:
                    // Unrecognized keywords default to Normal
                    return WpfPaintOrder.Normal;
            }
        }
    }
}
