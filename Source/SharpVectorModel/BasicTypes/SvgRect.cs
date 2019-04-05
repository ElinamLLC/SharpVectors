using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Rectangles are defined as consisting of a (x,y) coordinate pair identifying a minimum X value, 
    /// a minimum Y value, and a width and height, which are usually constrained to be non-negative. 
	/// </summary>
    public sealed class SvgRect : ISvgRect
	{
        #region Static Fields

        public static readonly SvgRect Empty = new SvgRect(0, 0, 0, 0);
        
        #endregion

        #region Private Fields

        private double _x;
        private double _y;
        private double _width;
        private double _height;
		
        #endregion

		#region Constructors

        public SvgRect(double x, double y, double width, double height)
        {
            _x      = x;
            _y      = y;
            _width  = width;
            _height = height;
        }

		public SvgRect(string str)
		{
			string replacedStr = Regex.Replace(str, @"(\s|,)+", ",");
			string[] tokens = replacedStr.Split(new char[]{','});
			if (tokens.Length == 2)
			{
				_x      = 0;
				_y      = 0;
				_width  = SvgNumber.ParseNumber(tokens[0]);
				_height = SvgNumber.ParseNumber(tokens[1]);
			}
            else if (tokens.Length == 4)
            {
                _x      = SvgNumber.ParseNumber(tokens[0]);
                _y      = SvgNumber.ParseNumber(tokens[1]);
                _width  = SvgNumber.ParseNumber(tokens[2]);
                _height = SvgNumber.ParseNumber(tokens[3]);
            }
			else
			{
				throw new SvgException(SvgExceptionType.SvgInvalidValueErr, 
                    "Invalid SvgRect value: " + str);
			}
		}

        #endregion

		#region Public Properties

		public bool IsEmpty
		{
			get
			{
				return (_width <= 0 || _height <= 0);
			}
		}

		#endregion

        #region Public Methods

        public override string ToString()
		{
            CultureInfo culture = CultureInfo.InvariantCulture;

            return ("{X=" + _x.ToString(culture) + ",Y=" + _y.ToString(culture) 
                + ",Width=" + _width.ToString(culture) + ",Height=" + _height.ToString(culture) + "}");
        }

        #endregion

        #region ISvgRect Interface

        public double X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public double Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public double Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		#endregion
	}
}
