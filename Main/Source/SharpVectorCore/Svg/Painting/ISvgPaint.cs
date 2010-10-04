using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPaint interface corresponds to basic type paint and represents the values of properties 'fill' and 'stroke'. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>50</completed>
	public interface ISvgPaint : ISvgColor
	{
		SvgPaintType PaintType{get;}
		string Uri{get;}

		void SetUri ( string uri );
		void SetPaint ( SvgPaintType paintType, string uri, string rgbColor, string iccColor );
	}
}
