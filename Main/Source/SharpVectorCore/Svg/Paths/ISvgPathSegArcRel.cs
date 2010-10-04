// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegArcRel interface corresponds to a "relative arcto" (a) path data command. 
	/// </summary>
	public interface ISvgPathSegArcRel : ISvgPathSeg 
	{
		double X{get;set;}
		double Y{get;set;}
		double R1{get;set;}
		double R2{get;set;}
		double Angle{get;set;}
		bool LargeArcFlag{get;set;}
		bool SweepFlag{get;set;}
	}
}