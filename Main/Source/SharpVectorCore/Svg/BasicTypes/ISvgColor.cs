// <developer>niklas@protocol7.com</developer>
// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgColor interface corresponds to color value definition
	/// for properties 'stop-color', 'flood-color' and 
	/// 'lighting-color' and is a base class for interface SvgPaint.
	/// It incorporates Svg's extended notion of color, which 
	/// incorporates ICC-based color specifications. 
	/// 
	/// Interface SVGColor does not correspond to the color basic 
	/// data type. For the color basic data type, the applicable 
	/// DOM interfaces are defined in CSS; in particular, see 
	/// the ICssRgbColor interface. 
	/// </summary>
	public interface ISvgColor : ICssValue 
	{
		SvgColorType ColorType { get; }
		ICssColor RgbColor { get; }
		ISvgIccColor IccColor { get; }

		void SetRgbColor(string rgbColor);
		void SetRgbColorIccColor(string rgbColor, string iccColor);
		void SetColor(SvgColorType colorType, string rgbColor, string iccColor);
	}
}
