namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgAnimatedPoints interface supports elements which have a 
	/// 'points' attribute which holds a list of coordinate values and 
	/// which support the ability to animate that attribute. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>90</completed>
	public interface ISvgAnimatedPoints
	{
		ISvgPointList Points{get;}
		ISvgPointList AnimatedPoints{get;}
	}
}
