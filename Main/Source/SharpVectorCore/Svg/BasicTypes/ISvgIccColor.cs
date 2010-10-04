// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISvgIccColor
	{
		/// <summary>
		/// The list of color values that define this ICC color. 
		/// Each color value is an arbitrary floating point number. 
		/// </summary>
		ISvgNumberList Colors
		{
			get;
		}
		
		/// <summary>
		/// The name of the color profile, which is the first 
		/// parameter of an ICC color specification.  Inheriting
		/// class should throw exception on setting a read only value 
		/// </summary>
		string ColorProfile
		{
			get;
			set;
		}
	}
}
