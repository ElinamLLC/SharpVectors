namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoVerticalAbs interface corresponds to an "absolute vertical lineto" (V) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoVerticalAbs : ISvgPathSeg
	{
        double Y { get; set; }
	}
}