// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgAnimatedPathData interface supports elements which have a 'd' attribute which holds Svg path data, and supports the ability to animate that attribute. 
	/// </summary>
	public interface ISvgAnimatedPathData
	{
		ISvgPathSegList PathSegList{get;}
		ISvgPathSegList NormalizedPathSegList{get;}
		ISvgPathSegList AnimatedPathSegList{get;}
		ISvgPathSegList AnimatedNormalizedPathSegList{get;}
	}
}
