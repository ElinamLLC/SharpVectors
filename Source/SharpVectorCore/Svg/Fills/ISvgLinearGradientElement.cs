namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgLinearGradientElement interface corresponds to the 'linearGradient' element. 
	/// </summary>
	public interface ISvgLinearGradientElement : ISvgGradientElement 
	{ 
		ISvgAnimatedLength X1{get;}
		ISvgAnimatedLength Y1{get;}
		ISvgAnimatedLength X2{get;}
		ISvgAnimatedLength Y2{get;}
	}
}
