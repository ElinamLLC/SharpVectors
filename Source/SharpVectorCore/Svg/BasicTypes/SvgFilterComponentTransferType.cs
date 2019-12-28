namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///	The Filter Component Transfer Types
	/// </summary>
	public enum SvgFilterTransferType : ushort
    {
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a new value 
        /// of this type or to attempt to switch an existing value to this type. 
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Corresponds to value 'identity'. 
        /// </summary>
        Identity = 1,
        /// <summary>
        /// Corresponds to value 'table'. 
        /// </summary>
        Table = 2,
        /// <summary>
        /// Corresponds to value 'discrete'. 
        /// </summary>
        Discrete = 3,
        /// <summary>
        /// Corresponds to value 'linear'. 
        /// </summary>
        Linear = 4,
        /// <summary>
        /// Corresponds to value 'gamma'. 
        /// </summary>
        Gamma = 5
    }
}
