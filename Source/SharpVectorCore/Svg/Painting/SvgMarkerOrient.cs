namespace SharpVectors.Dom.Svg
{
	public enum SvgMarkerOrient
	{
        /// <summary>
        /// The marker orientation is not one of predefined types. It is invalid to attempt 
        /// to define a new value of this type or to attempt to switch an existing value to this type.
        /// </summary>
        Unknown,
		/// <summary>
		/// Attribute orient has value 'auto'.
		/// </summary>
		Auto,
		/// <summary>
		/// Attribute orient has an angle value.
		/// </summary>
		Angle,
        /// <summary>
        /// Attribute orient has value 'auto-start-reverse'.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If placed by marker-start, the marker is oriented 180Åã different from the orientation 
        /// that would be used if 'auto' where specified. For all other markers, 'auto-start-reverse' 
        /// means the same as 'auto'.
        /// </para>
        /// <para>
        /// This allows a single arrowhead marker to be defined that can be used for both the start 
        /// and end of a path, i.e. which points outwards from both ends.
        /// </para>
        /// </remarks>
        AutoStartReverse,
	}
}
