using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveAngleValue : CssPrimitiveValue
	{
        #region Private Fields

        private static readonly Regex _reAngle = new Regex(CssValue.AnglePattern);

        #endregion

        #region Constructors

        public CssPrimitiveAngleValue(string number, string unit, bool readOnly) 
            : base(number+unit, readOnly)
		{
			SetUnitType(unit);
			SetFloatValue(number);
		}

		public CssPrimitiveAngleValue(string cssText, bool readOnly) 
            : base(cssText, readOnly)
		{
			OnSetCssText(cssText);
		}

		public CssPrimitiveAngleValue(double number, string unit, bool readOnly) 
            : base(number+unit, readOnly)
		{
			SetUnitType(unit);
			SetFloatValue(number);
		}

        #endregion

        #region Public Properties

        public override string CssText
        {
            get {
                return GetFloatValue(PrimitiveType).ToString(CssNumber.Format) + PrimitiveTypeAsString;
            }
        }

        #endregion

        #region Public Methods

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            double ret = double.NaN;
            switch (unitType)
            {
                case CssPrimitiveType.Number:
                case CssPrimitiveType.Deg:
                    ret = GetDegAngle();
                    break;
                case CssPrimitiveType.Rad:
                    ret = GetDegAngle() * Math.PI / 180;
                    break;
                case CssPrimitiveType.Grad:
                    ret = GetDegAngle() * 100 / 90;
                    break;
            }

            if (double.IsNaN(ret))
            {
                throw new DomException(DomExceptionType.InvalidAccessErr);
            }
            return ret;
        }

        #endregion

        #region Protected Methods

        protected override void OnSetCssText(string cssText)
        {
            Match match = _reAngle.Match(cssText);
            if (match.Success)
            {
                SetUnitType(match.Groups["angleUnit"].Value);
                SetFloatValue(match.Groups["angleNumber"].Value);
            }
            else
            {
                throw new DomException(DomExceptionType.SyntaxErr, "Unrecognized angle format: " + cssText);
            }
        }

        #endregion

        #region Private Methods

        private void SetUnitType(string unit)
        {
            switch (unit)
            {
                case "":
                    SetPrimitiveType(CssPrimitiveType.Number);
                    break;
                case "deg":
                    SetPrimitiveType(CssPrimitiveType.Deg);
                    break;
                case "rad":
                    SetPrimitiveType(CssPrimitiveType.Rad);
                    break;
                case "g":
                case "grad":
                case "gon":
                    SetPrimitiveType(CssPrimitiveType.Grad);
                    break;
                default:
                    throw new DomException(DomExceptionType.SyntaxErr, "Unknown angle unit");
            }
        }

        // only for absolute values
        private double GetDegAngle()
        {
            double ret;
            switch (PrimitiveType)
            {
                case CssPrimitiveType.Rad:
                    ret = _floatValue * 180 / Math.PI;
                    break;
                case CssPrimitiveType.Grad:
                    ret = _floatValue * 90 / 100;
                    break;
                default:
                    ret = _floatValue;
                    break;
            }
            ret %= 360;
            while (ret < 0) ret += 360;

            return ret;
        }

        #endregion
    }
}
