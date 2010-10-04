using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public struct CalculatedArcValues
    {
        public double CorrRx;
        public double CorrRy;
        public double Cx;
        public double Cy;
        public double AngleStart;
        public double AngleExtent;

        public CalculatedArcValues(double rx, double ry, double cx, double cy,
            double angleStart, double angleExtent)
        {
            this.CorrRx      = rx;
            this.CorrRy      = ry;
            this.Cx          = cx;
            this.Cy          = cy;
            this.AngleStart  = angleStart;
            this.AngleExtent = angleExtent;
        }
    }

    public abstract class SvgPathSegArc : SvgPathSeg
    {
        #region Private Fields

        private bool  largeArcFlag;
        private bool  sweepFlag;
        private double x;
        private double y;
        private double r1;
        private double r2;
        private double angle;

        #endregion

        #region Constructors and Destructor

        protected SvgPathSegArc(SvgPathSegType type, double x, double y, double r1, double r2,
            double angle, bool largeArcFlag, bool sweepFlag)
            : base(type)
        {
            this.x            = x;
            this.y            = y;
            this.r1           = r1;
            this.r2           = r2;
            this.angle        = angle;
            this.largeArcFlag = largeArcFlag;
            this.sweepFlag    = sweepFlag;
        }

        #endregion

        #region Public Properties

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double R1
        {
            get { return r1; }
            set { r1 = value; }
        }

        public double R2
        {
            get { return r2; }
            set { r2 = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public bool LargeArcFlag
        {
            get { return largeArcFlag; }
            set { largeArcFlag = value; }
        }

        public bool SweepFlag
        {
            get { return sweepFlag; }
            set { sweepFlag = value; }
        }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle
        {
            get
            {
                double a = GetAngle(false);
                a += 270;
                a += 360;
                a = a % 360;
                return a;
            }
        }

        public override double EndAngle
        {
            get
            {
                double a = GetAngle(true);
                a += 90;
                a += 360;
                a = a % 360;

                return a;
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(R1);
                sb.Append(",");
                sb.Append(R2);
                sb.Append(",");
                sb.Append(Angle);
                sb.Append(",");

                if (LargeArcFlag)
                    sb.Append("1");
                else
                    sb.Append("0");

                sb.Append(",");

                if (SweepFlag)
                    sb.Append("1");
                else
                    sb.Append("0");

                sb.Append(",");
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        #endregion

        #region Public Methods

        public CalculatedArcValues GetCalculatedArcValues()
        {
            CalculatedArcValues calcVal = new CalculatedArcValues();

            /*
             *	This algorithm is taken from the Batik source. All cudos to the Batik crew.
             */

            SvgPointF startPoint = PreviousSeg.AbsXY;
            SvgPointF endPoint = AbsXY;

            double x0 = startPoint.X;
            double y0 = startPoint.Y;

            double x = endPoint.X;
            double y = endPoint.Y;

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;

            // Convert angle from degrees to radians
            double radAngle = Angle * Math.PI / 180;
            double cosAngle = Math.Cos(radAngle);
            double sinAngle = Math.Sin(radAngle);

            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough

            double rx = Math.Abs(R1);
            double ry = Math.Abs(R2);

            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;

            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (LargeArcFlag == SweepFlag) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);

            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1); // rx;
            double uy = (y1 - cy1); // ry;
            double vx = (-x1 - cx1); // rx;
            double vy = (-y1 - cy1); // ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = sign * Math.Acos(p / n);
            angleStart = angleStart * 180 / Math.PI;

            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = sign * Math.Acos(p / n);
            angleExtent = angleExtent * 180 / Math.PI;

            if (!sweepFlag && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (sweepFlag && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            angleExtent %= 360f;
            angleStart %= 360f;

            calcVal.CorrRx = rx;
            calcVal.CorrRy = ry;
            calcVal.Cx = cx;
            calcVal.Cy = cy;
            calcVal.AngleStart = angleStart;
            calcVal.AngleExtent = angleExtent;

            return calcVal;
        }

        #endregion

        #region Private Methods

        private double GetAngle(bool addExtent)
        {
            CalculatedArcValues calcValues = GetCalculatedArcValues();

            double radAngle = calcValues.AngleStart;
            if (addExtent)
            {
                radAngle += calcValues.AngleExtent;
            }

            radAngle *= (Math.PI / 180);
            double cosAngle = Math.Cos(radAngle);
            double sinAngle = Math.Sin(radAngle);

            double denom = Math.Sqrt(
                calcValues.CorrRy * calcValues.CorrRy * cosAngle * cosAngle +
                calcValues.CorrRx * calcValues.CorrRx * sinAngle * sinAngle);

            double xt = -calcValues.CorrRx * sinAngle / denom;
            double yt = calcValues.CorrRy * cosAngle / denom;

            double a = (Math.Atan2(yt, xt) * 180 / Math.PI);
            a += Angle;
            return a;
        }

        #endregion
    }
}
