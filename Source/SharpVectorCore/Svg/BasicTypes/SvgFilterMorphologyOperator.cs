namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///	The Morphology Operator
	/// </summary>
	public enum SvgFilterMorphologyOperator : ushort
	{
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a new value of 
        /// this type or to attempt to switch an existing value to this type. 
        /// </summary>
		Unknown = 0,
        /// <summary>
        /// Corresponds to value 'erode'. 
        /// </summary>
		Erode = 1,
        /// <summary>
        /// Corresponds to value 'dilate'. 
        /// </summary>
		Dilate = 2
	}
}
