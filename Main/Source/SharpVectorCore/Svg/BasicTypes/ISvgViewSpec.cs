namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The interface corresponds to an Svg View Specification. 
	/// </summary>
	/// <developer></developer>
	/// <completed>0</completed>
	public interface ISvgViewSpec : ISvgZoomAndPan, ISvgFitToViewBox
	{
		ISvgTransformList Transform{get;}
		ISvgElement ViewTarget{get;}
		string ViewBoxString{get;}
		string PreserveAspectRatioString{get;}
		string TransformString{get;}
		string ViewTargetString{get;}
	}
}
