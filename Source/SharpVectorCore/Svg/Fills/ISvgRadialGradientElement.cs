namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The ISvgRadialGradientElement interface corresponds to the 'radialGradient' element. 
	/// </summary>
	public interface ISvgRadialGradientElement : ISvgGradientElement 
	{ 
		ISvgAnimatedLength Cx{get;}
		ISvgAnimatedLength Cy{get;}
		ISvgAnimatedLength R{get;}
		ISvgAnimatedLength Fx{get;}
		ISvgAnimatedLength Fy{get;}
	}
}
