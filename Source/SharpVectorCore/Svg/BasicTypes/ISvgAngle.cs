namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgAngle interface corresponds to the angle basic data 
	/// type. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ISvgAngle
	{
		/// <summary>
		/// The type of the value as specified by the SvgAngleType 
		/// enum class.
		/// </summary>
		SvgAngleType UnitType{get;}

		/// <summary>
		/// The angle value as a floating point value, in degrees. 
		/// Setting this attribute will cause valueInSpecifiedUnits 
		/// and valueAsString to be updated automatically to reflect 
		/// this setting.
		/// </summary>
		double Value{get;set;}

		/// <summary>
		/// The angle value as a floating point value, in the units 
		/// expressed by unitType. Setting this attribute will cause
		/// value and valueAsString to be updated automatically to 
		/// reflect this setting.
		/// </summary>
		double ValueInSpecifiedUnits{get;set;}

		/// <summary>
		/// The angle value as a string value, in the units expressed
		/// by unitType. Setting this attribute will cause value and 
		/// valueInSpecifiedUnits to be updated automatically to 
		/// reflect this setting.
		/// </summary>
		string ValueAsString{get;set;}

		/// <summary>
		/// Reset the value as a number with an associated unitType,
		/// thereby replacing the values for all of the attributes on 
		/// the object.
		/// </summary>
		/// <param name="unitType"> The unitType for the angle value (e.g., SvgAngleTypeDEG).</param>
		/// <param name="valueInSpecifiedUnits">The angle value.</param>
		void NewValueSpecifiedUnits(SvgAngleType unitType, double valueInSpecifiedUnits);
		
		/// <summary>
		/// Preserve the same underlying stored value, but reset the
		/// stored unit identifier to the given unitType. Object 
		/// attributes unitType, valueAsSpecified and valueAsString 
		/// might be modified as a result of this method.
		/// </summary>
		/// <param name="unitType">The unitType to switch to (e.g., SvgAngleTypeDEG).</param>
		void ConvertToSpecifiedUnits(SvgAngleType unitType);
	}
}
