namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///	The Filter Turbulence Types
	/// </summary>
	public enum SvgFilterTurbulenceType : ushort
    {
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a new value of this 
        /// type or to attempt to switch an existing value to this type. 
        /// </summary>
		Unknown      = 0,
        /// <summary>
        /// Corresponds to value 'fractalNoise'. 
        /// </summary>
		FractalNoise = 1,
        /// <summary>
        /// Corresponds to value 'turbulence'. 
        /// </summary>
		Turbulence   = 2
	}
}
