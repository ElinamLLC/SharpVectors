using System;

namespace SharpVectors.Dom.Svg
{
	public interface ISvgAnimatedAngle
	{
		ISvgAngle BaseVal { get; }
		ISvgAngle AnimVal { get; }
	}
}