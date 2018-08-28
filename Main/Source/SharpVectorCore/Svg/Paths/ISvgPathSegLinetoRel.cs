namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegLinetoRel interface corresponds to an "relative lineto" (l) path data command. 
	/// </summary>
	public interface ISvgPathSegLinetoRel : ISvgPathSeg
	{
        double X { get; set; }
        double Y { get; set; }
	}
}