namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegCurvetoCubicRel interface corresponds to a "relative cubic Bezier curveto" 
    /// (c) path data command. 
	/// </summary>
	public interface ISvgPathSegCurvetoQuadraticRel : ISvgPathSeg
	{
		double X{get;set;}
		double Y{get;set;}
		double X1{get;set;}
		double Y1{get;set;}
	}
}