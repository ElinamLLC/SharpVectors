using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTransform.
	/// </summary>
    public sealed class SvgTransform : ISvgTransform
    {
        #region Private Fields

        private static readonly Regex _reSeparators = new Regex("[\\s\\,]+");

        private double _angle;
        private ISvgMatrix _matrix;
        private SvgTransformType _type;

        private double[] _values;

        #endregion

        #region Constructors and Destructor

		public SvgTransform()
		{
		}

        public SvgTransform(ISvgMatrix matrix)
        {
            _type   = SvgTransformType.Matrix;
            _matrix = matrix;
        }

		public SvgTransform(string str)
		{
			int start = str.IndexOf("(", StringComparison.OrdinalIgnoreCase);
			string type = str.Substring(0, start);
			string valuesList = (str.Substring(start+1, str.Length - start - 2)).Trim(); //JR added trim
			valuesList = _reSeparators.Replace(valuesList, ",");

			string[] valuesStr = valuesList.Split(new char[]{','});
			int len = valuesStr.GetLength(0);
            double[] values = new double[len];

            try
            {
                for (int i = 0; i < len; i++)
                {
                    values[i] = SvgNumber.Parse(valuesStr[i]);
                }
            }
            catch
            {
                values = SvgNumber.ParseDoubles(str);
                len = values.Length;
            }

            _values = values;

            switch (type.Trim())
            {
                case "translate":
                    switch (len)
                    {
                        case 1:
                            SetTranslate(values[0], 0);
                            break;
                        case 2:
                            SetTranslate(values[0], values[1]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in translate transform");
                    }
                    break;
                case "rotate":
                    switch (len)
                    {
                        case 1:
                            SetRotate(values[0]);
                            break;
                        case 3:
                            SetRotate(values[0], values[1], values[2]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in rotate transform");
                    }
                    break;
                case "scale":
                    switch (len)
                    {
                        case 1:
                            SetScale(values[0], values[0]);
                            break;
                        case 2:
                            SetScale(values[0], values[1]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in scale transform");
                    }
                    break;
                case "skewX":
                    if (len != 1)
                        throw new ApplicationException("Wrong number of arguments in skewX transform");
                    SetSkewX(values[0]);
                    break;
                case "skewY":
                    if (len != 1)
                        throw new ApplicationException("Wrong number of arguments in skewY transform");
                    SetSkewY(values[0]);
                    break;
                case "matrix":
                    if (len != 6)
                        throw new ApplicationException("Wrong number of arguments in matrix transform");
                    SetMatrix(new SvgMatrix(values[0], values[1], values[2], values[3], values[4], values[5]));
                    break;
                default:
                    _type = SvgTransformType.Unknown;
                    break;
            }
		}

        #endregion

        #region ISvgTransform Members

		public short Type
		{
			get { return (short)_type; }
		}

		public SvgTransformType TransformType
        {
			get { return _type; }
		}

		public ISvgMatrix Matrix
		{
			get { return _matrix; }
		}

        public double Angle
		{
            get { return _angle; }
		}

        public double[] InputValues
        {
            get { return _values; }
		}

		public void SetMatrix(ISvgMatrix matrix)
		{
			_type   = SvgTransformType.Matrix;
			_matrix = matrix;
		}

        public void SetTranslate(double tx, double ty)
		{
			_type   = SvgTransformType.Translate;
			_matrix = new SvgMatrix().Translate(tx, ty);
		}

        public void SetScale(double sx, double sy)
		{
			_type   = SvgTransformType.Scale;
			_matrix = new SvgMatrix().ScaleNonUniform(sx, sy);
		}

        public void SetRotate(double angle)
		{
			_type   = SvgTransformType.Rotate;
			_angle  = angle;
			_matrix = new SvgMatrix().Rotate(angle);
		}

        public void SetRotate(double angle, double cx, double cy)
		{
			_type   = SvgTransformType.Rotate;
			_angle  = angle;
			_matrix = new SvgMatrix().Translate(cx, cy).Rotate(angle).Translate(-cx,-cy);
		}

        public void SetSkewX(double angle)
		{
			_type   = SvgTransformType.SkewX;
			_angle  = angle;
			_matrix = new SvgMatrix().SkewX(angle);
		}

        public void SetSkewY(double angle)
		{
			_type   = SvgTransformType.SkewY;
			_angle  = angle;
			_matrix = new SvgMatrix().SkewY(angle);
		}

        #endregion
	}
}
