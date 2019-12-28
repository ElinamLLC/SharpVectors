namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///	The Filter Stitch Options
	/// </summary>
	public enum SvgFilterStitchOption : ushort
    {
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a new value of this type 
        /// or to attempt to switch an existing value to this type. 
        /// </summary>
		Unknown  = 0,
        /// <summary>
        /// Corresponds to value 'stitch'.
        /// </summary>
		Stitch   = 1,
        /// <summary>
        /// Corresponds to value 'noStitch'.
        /// </summary>
		NoStitch = 2
	}
}
