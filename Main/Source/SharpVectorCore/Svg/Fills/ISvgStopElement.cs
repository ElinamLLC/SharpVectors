namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgStopElement interface corresponds to the 'stop' element. 
	/// </summary>
	public interface ISvgStopElement : 
		ISvgElement,
		ISvgStylable 
	{
		ISvgAnimatedNumber Offset{get;}
	}
}
