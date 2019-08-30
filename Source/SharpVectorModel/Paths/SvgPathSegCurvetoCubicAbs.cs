using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegCurvetoCubicAbs : SvgPathSegCurvetoCubic, ISvgPathSegCurvetoCubicAbs
    {
        #region Private Fields

        private double _x;
        private double _y;
        private double _x1;
        private double _y1;
        private double _x2;
        private double _y2;

        #endregion

        #region Constructors

        public SvgPathSegCurvetoCubicAbs(double x, double y, double x1, double y1, double x2, double y2)
            : base(SvgPathSegType.CurveToCubicAbs)
        {
            _x  = x;
            _y  = y;
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
        }

        #endregion

        #region ISvgPathSegCurvtoCubicAbs Members

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        public double Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }

        public double X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }

        public double Y2
        {
            get { return _y2; }
            set { _y2 = value; }
        }

        #endregion

        #region Public Methods

        public override SvgPointF AbsXY
        {
            get {
                return new SvgPointF(X, Y);
            }
        }
        public override SvgPointF CubicX1Y1
        {
            get {
                return new SvgPointF(X1, Y1);
            }
        }
        public override SvgPointF CubicX2Y2
        {
            get {
                return new SvgPointF(X2, Y2);
            }
        }

        public override string PathText
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X1);
                sb.Append(",");
                sb.Append(Y1);
                sb.Append(",");
                sb.Append(X2);
                sb.Append(",");
                sb.Append(Y2);
                sb.Append(",");
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        #endregion
    }
}
