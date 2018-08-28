namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSeg interface is a base interface that corresponds to a single command within 
    /// a path data specification. 
	/// </summary>
	public interface ISvgPathSeg
	{
		SvgPathSegType PathSegType{get;}
		string PathSegTypeAsLetter{get;}
	}
}