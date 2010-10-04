using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegMovetoAbs : SvgPathSegMoveto, ISvgPathSegMovetoAbs
	{
        public SvgPathSegMovetoAbs(double x, double y)
            : base(SvgPathSegType.MoveToAbs, x, y)
		{
		}

		public override SvgPointF AbsXY
		{
			get
			{
				return new SvgPointF(X, Y);
			}
		}
	}
}
