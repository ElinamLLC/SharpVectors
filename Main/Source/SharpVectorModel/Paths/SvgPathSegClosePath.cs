using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgPathSegClosePath.
	/// </summary>
	public sealed class SvgPathSegClosePath : SvgPathSeg, ISvgPathSegClosePath
	{
		internal SvgPathSegClosePath() : base(SvgPathSegType.ClosePath)
		{
		}

        public override SvgPointF AbsXY
		{
			get
			{
				SvgPathSeg item = this;
			
				while(item != null && !(item is SvgPathSegMoveto))
				{
					item = item.PreviousSeg;
				}

				if(item == null)
				{
                    return new SvgPointF(0, 0);
				}
				else
				{
					return item.AbsXY;
				}
			}
		}

        public override double StartAngle
		{
			get
			{
				SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
				if(prevSeg == null)
				{
                    prevPoint = new SvgPointF(0, 0);
				}
				else
				{
					prevPoint = prevSeg.AbsXY;
				}
                SvgPointF curPoint = AbsXY;

                double dx = curPoint.X - prevPoint.X;
                double dy = curPoint.Y - prevPoint.Y;

                double a = (Math.Atan2(dy, dx) * 180 / Math.PI);
				a += 270;
				a %= 360;
				return a;
			}
		}

        public override double EndAngle
		{
			get
			{
                double a = StartAngle;
				a += 180;
				a %= 360;
				return a;
			}
		}

		public override string PathText
		{
			get
			{
				return "z";
			}
		}
	}
}
