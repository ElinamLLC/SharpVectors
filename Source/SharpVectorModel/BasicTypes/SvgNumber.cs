using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgNumber : ISvgNumber
    {
        #region Private Fields

        private static readonly string numberPattern = @"(?<number>(\+|-)?\d*\.?\d+((e|E)(\+|-)?\d+)?)";
        private static readonly Regex reNumber = new Regex("^" + numberPattern + "$");

        private static Regex reUnit = new Regex("[a-z]+$");
        private static Regex DoubleRegex = new Regex(@"(\+|-)?((\.[0-9]+)|([0-9]+(\.[0-9]*)?))([eE](\+|-)?[0-9]+)?", RegexOptions.Compiled);

        private double _value;

        #endregion

        #region Constructors and Destructor

        public SvgNumber(float val)
        {
            _value = val;
        }

        public SvgNumber(string str)
        {
            _value = ParseNumber(str);
        }

        #endregion

        #region Public Static Properties

        public static NumberFormatInfo Format
        {
            get {
                return CssNumber.Format;
            }
        }

        #endregion

        #region Public Static Methods

        public static string ScientificToDec(string sc)
        {
            if (sc.IndexOfAny(new char[] { 'e', 'E' }) > -1)
            {
                sc = sc.Trim();
                // remove the unit
                Match match = reUnit.Match(sc);
                string value = sc.Substring(0, sc.Length - match.Length);

                var numberValue = decimal.Parse(value,
                    NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                // <number> values in conforming SVG Tiny 1.2 content must have no more 
                // than 4 decimal digits in the fractional part of their decimal expansion
                var roundedValue = decimal.Round(numberValue, 4, MidpointRounding.AwayFromZero);

                return roundedValue.ToString(Format) + match.Value;
            }
            return sc;
        }

        public static double Parse(string str)
        {
            try
            {
                return double.Parse(str, SvgNumber.Format);
            }
            catch (Exception e)
            {
                throw new DomException(DomExceptionType.SyntaxErr,
                    "Input string was not in a correct format: " + str, e);
            }
        }

        public static double ParseNumber(string str)
        {
            str = DoubleRegex.Match(str).Value;
            return double.Parse(str, SvgNumber.Format);
        }

        public static double ParseDouble(string str)
        {
            str = DoubleRegex.Match(str).Value;
            return double.Parse(str, SvgNumber.Format);
        }

        public static double[] ParseDoubles(string str)
        {
            List<double> valueList = new List<double>();
            foreach (Match m in DoubleRegex.Matches(str))
            {
                if (!string.IsNullOrEmpty(m.Value))
                    valueList.Add(SvgNumber.ParseDouble(m.Value));
            }
            return valueList.ToArray();
        }

        public static double CalcAngleDiff(double a1, double a2)
        {
            while (a1 < 0) a1 += 360;
            a1 %= 360;

            while (a2 < 0) a2 += 360;
            a2 %= 360;

            double diff = (a1 - a2);

            while (diff < 0)
                diff += 360;
            diff %= 360;

            return diff;
        }

        public static double CalcAngleBisection(double a1, double a2)
        {
            double diff = CalcAngleDiff(a1, a2);
            double bisect = a1 - diff / 2F;

            while (bisect < 0)
                bisect += 360;

            bisect %= 360;
            return bisect;
        }

        public static bool IsValid(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return false;
            }
            return true;
        }

        public static bool IsValid(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region ISvgNumber Nembers

        public double Value
        {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        #endregion
    }
}
