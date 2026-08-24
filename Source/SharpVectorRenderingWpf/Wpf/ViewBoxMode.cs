using System;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// Specifies how the viewbox dimensions should be determined when rendering SVG.
    /// </summary>
    [Serializable]
    public enum ViewBoxMode
    {
        /// <summary>
        /// Uses the explicitly declared viewBox and width/height attributes from the SVG root element.
        /// This is the strict SVG specification approach.
        /// This was the default behavior before version 1.8.4.
        /// </summary>
        Strict,

        /// <summary>
        /// Uses the computed union of all rendered element bounds.
        /// This provides the true visual footprint of the rendered content.
        /// This is the default behavior starting from version 1.8.4.
        /// </summary>
        ComputedUnion
    }
}
