using System;
using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAngle.
	/// </summary>
    public sealed class SvgAngle : ISvgAngle
	{
        #region Private Fields

        private bool _readOnly;
        private CssPrimitiveAngleValue _cssAngle;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Creates a SvgAngle value
        /// </summary>
        /// <param name="s">The string to parse for the angle value</param>
        /// <param name="defaultValue">The default value for the angle.</param>
        /// <param name="readOnly">Specifies if the value should be read-only</param>
        public SvgAngle(string baseVal, string defaultValue, bool readOnly)
        {
            baseVal = baseVal.Trim();
            if (baseVal.Length == 0)
                baseVal = defaultValue;

            baseVal = SvgNumber.ScientificToDec(baseVal);

            _cssAngle = new CssPrimitiveAngleValue(baseVal, readOnly);
            _readOnly = readOnly;
        }

        #endregion

        #region Implementation of ISvgAngle

        /// <summary>
        ///  Reset the value as a number with an associated unitType, thereby replacing the values 
        ///  for all of the attributes on the object.
        /// </summary>
        /// <param name="unitType">The unitType for the angle value (e.g., SvgAngleTypeDEG).</param>
        /// <param name="valueInSpecifiedUnits">The angle value</param>
        public void NewValueSpecifiedUnits(SvgAngleType unitType, double valueInSpecifiedUnits)
		{
			_cssAngle.SetFloatValue((CssPrimitiveType)unitType, valueInSpecifiedUnits);
		}

		/// <summary>
		///  Preserve the same underlying stored value, but reset the stored unit identifier to the given 
        ///  unitType. Object attributes unitType, valueAsSpecified and valueAsString might be modified 
        ///  as a result of this method.
		/// </summary>
		/// <param name="unitType">The unitType to switch to (e.g., SvgAngleTypeDEG).</param>
		public void ConvertToSpecifiedUnits(SvgAngleType unitType)
		{
			double newValue = _cssAngle.GetFloatValue((CssPrimitiveType)unitType);
			_cssAngle.SetFloatValue((CssPrimitiveType)unitType, newValue);
		}

		/// <summary>
		/// The type of the value as specified by one of the constants specified above
		/// </summary>
		public SvgAngleType UnitType
		{
			get
			{
				return (SvgAngleType)_cssAngle.PrimitiveType;
			}
		}
	
		/// <summary>
		///  The angle value as a floating point value, in degrees. Setting this attribute will cause 
        ///  valueInSpecifiedUnits and valueAsString to be updated automatically to reflect this setting.
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the 
        /// value of a readonly attribute.</exception>
		public double Value
		{
			get
			{
				return _cssAngle.GetFloatValue(CssPrimitiveType.Deg);
			}
			set
			{
				if (_readOnly)
                {
                    throw new DomException(DomExceptionType.NoModificationAllowedErr, "Cannot set readonly angle value");
                }

				CssPrimitiveType oldType = _cssAngle.PrimitiveType;
				_cssAngle.SetFloatValue(CssPrimitiveType.Deg, value);
                
				ConvertToSpecifiedUnits((SvgAngleType)oldType);
			}
		}

		/// <summary>
		///  The angle value as a floating point value, in the units expressed by unitType. Setting this 
        ///  attribute will cause value and valueAsString to be updated automatically to reflect this setting.
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the 
        /// value of a readonly attribute.</exception>
		public double ValueInSpecifiedUnits
		{
			get
			{
				return _cssAngle.GetFloatValue(_cssAngle.PrimitiveType);
			}
			set
			{
				if (_readOnly)
                {
                    throw new DomException(DomExceptionType.NoModificationAllowedErr, "Cannot set readonly angle value");
                }
                _cssAngle.SetFloatValue(_cssAngle.PrimitiveType, value);
			}
		}

		/// <summary>
		///  The angle value as a string value, in the units expressed by unitType. Setting this attribute 
        ///  will cause value and valueInSpecifiedUnits to be updated automatically to reflect this setting.
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the 
        /// value of a readonly attribute.</exception>
		public string ValueAsString
		{
			get
			{
				return _cssAngle.CssText;
			}
			set
			{
				if (_readOnly)
                {
                    throw new DomException(DomExceptionType.NoModificationAllowedErr, "Cannot set readonly angle value");
                }
                _cssAngle = new CssPrimitiveAngleValue(value, false);
			}

		}

		#endregion
	}
}
