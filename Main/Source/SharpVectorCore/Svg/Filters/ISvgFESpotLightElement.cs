using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFESpotLightElement:
		ISvgElement
	{
		ISvgAnimatedNumber X{get;}
		ISvgAnimatedNumber Y{get;}
		ISvgAnimatedNumber Z{get;}
		ISvgAnimatedNumber PointsAtX{get;}
		ISvgAnimatedNumber PointsAtY{get;}
		ISvgAnimatedNumber PointsAtZ{get;}
		ISvgAnimatedNumber SpecularExponent{get;}
		ISvgAnimatedNumber LimitingConeAngle{get;}
	}
}
