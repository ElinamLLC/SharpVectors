namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegCurvetoCubicSmoothRel interface corresponds to a "relative smooth cubic curveto" 
    /// (s) path data command.
	/// </summary>
	public interface ISvgPathSegCurvetoCubicSmoothRel : ISvgPathSeg
	{
		double X{get;set;}
		double Y{get;set;}
		double X2{get;set;}
		double Y2{get;set;}
	}
}