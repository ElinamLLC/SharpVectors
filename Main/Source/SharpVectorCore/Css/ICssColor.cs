// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>	

using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The RGBColor interface is used to represent any RGB color 
	/// value. This interface reflects the values in the underlying
	/// style property. Hence, modifications made to the 
	/// CSSPrimitiveValue objects modify the style property. 
	/// </summary>
	public interface ICssColor
	{
		/// <summary>
		/// This attribute is used for the red value of the RGB color
		/// </summary>
		ICssPrimitiveValue Red {get;}

		/// <summary>
		/// This attribute is used for the green value of the RGB color.
		/// </summary>
		ICssPrimitiveValue Green {get;}

		/// <summary>
		/// This attribute is used for the blue value of the RGB color
		/// </summary>
		ICssPrimitiveValue Blue {get;}
	}
}
