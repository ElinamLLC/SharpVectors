namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgPreserveAspectRatio interface corresponds to the preserveAspectRatio attribute, 
    /// which is available for some of Svg's elements. 
    /// </summary>
    public interface ISvgPreserveAspectRatio
    {
        /// <summary>
        /// Gets a value indicating if this a default aspect ratio or it is defined by the target element.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if it is the default aspect ratio; otherwise it is <see langword="false"/>.
        /// </value>
        bool IsDefaultAlign { get; }

        /// <summary>
        /// The type of the alignment value as specified by one of the constants specified above.
        /// </summary>
        SvgPreserveAspectRatioType Align { get; set; }

        /// <summary>
        /// The type of the meet-or-slice value as specified by one of the constants specified above.
        /// </summary>
        SvgMeetOrSlice MeetOrSlice { get; set; }
    }
}