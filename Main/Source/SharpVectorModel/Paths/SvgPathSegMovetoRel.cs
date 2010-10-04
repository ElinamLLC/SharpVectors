// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgPathSegMovetoRel interface corresponds to an "relative moveto" (m) path data command. 
	/// </summary>
    public sealed class SvgPathSegMovetoRel : SvgPathSegMoveto, ISvgPathSegMovetoRel
	{
        public SvgPathSegMovetoRel(double x, double y)
            : base(SvgPathSegType.MoveToRel, x, y)
		{
		}

		public override SvgPointF AbsXY
		{
			get
			{
				SvgPathSeg prevSeg = PreviousSeg;
				SvgPointF prevPoint;
				if (prevSeg == null) 
                    prevPoint = new SvgPointF(0,0);
				else prevPoint = prevSeg.AbsXY;
				return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
			}
		}
	}
}
