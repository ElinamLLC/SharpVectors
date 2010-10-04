using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// </summary>
	/// <developer>don@donxml.com</developer>
	/// <completed>100</completed>
	public interface ISvgFEPointLightElement:
		ISvgElement
	{
		ISvgAnimatedNumber X{get;}
		ISvgAnimatedNumber Y{get;}
		ISvgAnimatedNumber Z{get;}
	}
}
