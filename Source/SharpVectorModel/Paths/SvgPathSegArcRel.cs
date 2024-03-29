using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgPathSegLinetoAbs.
	/// </summary>
    public sealed class SvgPathSegArcRel : SvgPathSegArc, ISvgPathSegArcRel
	{
        public SvgPathSegArcRel(double x, double y, double r1, double r2, double angle, 
            bool largeArcFlag, bool sweepFlag) 
            : base(SvgPathSegType.ArcRel, x, y, r1, r2, angle, largeArcFlag, sweepFlag)
		{
		}

        public override SvgPointF AbsXY
		{
			get
			{
				//if (_limits != null && _limits.Length == 2)
				//{
				//	return _limits[1];
				//}

				SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
				if (prevSeg == null)
                    prevPoint = new SvgPointF(0, 0);
				else prevPoint = 
                    prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
			}
		}
	}
}
