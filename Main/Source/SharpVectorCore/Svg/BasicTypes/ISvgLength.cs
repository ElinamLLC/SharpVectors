// <developer>niklas@protocol7.com</developer>
// <completed>95</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgLength interface corresponds to the length basic data type. 
	/// </summary>
	public interface ISvgLength
	{
		 SvgLengthType UnitType{get;}
		 double Value{get;set;}
		 double ValueInSpecifiedUnits{get;set;}
		 string ValueAsString{get;set;}
		 
		 void NewValueSpecifiedUnits(SvgLengthType unitType, double valueInSpecifiedUnits);
		 void ConvertToSpecifiedUnits(SvgLengthType unitType);
	}
}
