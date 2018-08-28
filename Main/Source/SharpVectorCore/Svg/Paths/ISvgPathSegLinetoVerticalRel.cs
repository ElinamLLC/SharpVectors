namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoVerticalRel interface corresponds to a "relative vertical lineto" (v) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoVerticalRel : ISvgPathSeg
	{
        double Y { get; set; }
	}
}