using System.Windows;

namespace SharpVectors.Renderers.Texts
{
    // A more complete enum to represent the most common CSS dominant-baseline values.
    public enum DominantBaseline
    {
        /// <summary>
        /// The dominant baseline is not set.
        /// </summary>
        Auto,
        /// <summary>
        /// The dominant baseline is the alphabetic baseline.
        /// </summary>
        Alphabetic, // Default
        /// <summary>
        /// 
        /// </summary>
        Middle,
        Central,
        Hanging,
        Ideographic,
        Mathematical,
        TextBeforeEdge,  // Corresponds to CSS text-top
        TextAfterEdge    // Corresponds to CSS text-bottom
    }

    public struct WpfTextStringFormat
    {
        public FlowDirection Direction;
        public TextTrimming Trimming;
        //public TextAlignment Alignment;
        public WpfTextAnchor Anchor;

        public DominantBaseline Dominant;

        public WpfTextStringFormat(FlowDirection direction, TextTrimming trimming,
            WpfTextAnchor anchor, DominantBaseline dominant)
        {
            this.Direction = direction;
            this.Trimming  = trimming;
            this.Anchor    = anchor;
            this.Dominant  = dominant;
         }

        public TextAlignment Alignment
        {
            get {
                if (Anchor == WpfTextAnchor.Middle)
                {
                    return TextAlignment.Center;
                }
                if (Anchor == WpfTextAnchor.End)
                {
                    return TextAlignment.Right;
                }

                return TextAlignment.Left;
            }
        }

        public static WpfTextStringFormat Default
        {
            get {
                return new WpfTextStringFormat(FlowDirection.LeftToRight,
                    TextTrimming.None, WpfTextAnchor.Start, DominantBaseline.Alphabetic);
            }
        }
    }
}
