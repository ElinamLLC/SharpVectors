namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoHorizontalAbs interface corresponds to an 
    /// "absolute horizontal lineto" (H) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoHorizontalAbs : ISvgPathSeg
	{
		double X{get;set;}
	}
}