using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	public class CssPrimitiveLengthValue : CssPrimitiveValue
	{
        #region Protected Fields

        private static readonly Regex _reLength = new Regex(CssValue.LengthPattern);

        protected const double Dpi = 96;
        //protected const double Dpi     = 90; // The common default value for this is 90, not 96
        protected const double CmPerIn = 2.54;

        #endregion

        #region Constructors

        public CssPrimitiveLengthValue(string number, string unit, bool readOnly) 
            : base(number+unit, readOnly)
		{
			SetUnitType(unit);
			SetFloatValue(number);
		}

		public CssPrimitiveLengthValue(string cssText, bool readOnly) 
            : base(cssText, readOnly)
		{
            SetCssText(cssText);
		}

		public CssPrimitiveLengthValue(double number, string unit, bool readOnly) 
            : base(number+unit, readOnly)
		{
			SetUnitType(unit);
			SetFloatValue(number);
		}

		protected CssPrimitiveLengthValue()
		{
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

        public override CssValue GetAbsoluteValue(string propertyName, XmlElement elm)
        {
            return new CssAbsPrimitiveLengthValue(this, propertyName, elm);
        }

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            double ret = double.NaN;
            switch (this.PrimitiveType)
            {
                case CssPrimitiveType.Number:
                case CssPrimitiveType.Px:
                case CssPrimitiveType.Cm:
                case CssPrimitiveType.Mm:
                case CssPrimitiveType.In:
                case CssPrimitiveType.Pt:
                case CssPrimitiveType.Pc:
                    switch (unitType)
                    {
                        case CssPrimitiveType.Px:
                            ret = GetPxLength();
                            break;
                        case CssPrimitiveType.Number:
                            ret = GetPxLength();
                            break;
                        case CssPrimitiveType.In:
                            ret = GetInchLength();
                            break;
                        case CssPrimitiveType.Cm:
                            ret = GetInchLength() * CmPerIn;
                            break;
                        case CssPrimitiveType.Mm:
                            ret = GetInchLength() * CmPerIn * 10;
                            break;
                        case CssPrimitiveType.Pt:
                            ret = GetInchLength() * 72;
                            break;
                        case CssPrimitiveType.Pc:
                            ret = GetInchLength() * 6;
                            break;
                    }
                    break;
                case CssPrimitiveType.Percentage:
                    if (unitType == CssPrimitiveType.Percentage) ret = _floatValue;
                    break;
                case CssPrimitiveType.Ems:
                    if (unitType == CssPrimitiveType.Ems) ret = _floatValue;
                    break;
                case CssPrimitiveType.Exs:
                    if (unitType == CssPrimitiveType.Exs) ret = _floatValue;
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
            SetCssText(cssText);
        }

        #endregion

        #region Private Methods

        private void SetCssText(string cssText)
        {
            Match match = _reLength.Match(cssText);
            if (match.Success)
            {
                SetUnitType(match.Groups["lengthUnit"].Value);
                SetFloatValue(match.Groups["lengthNumber"].Value);
            }
            else
            {
                throw new DomException(DomExceptionType.SyntaxErr, "Unrecognized length format: " + cssText);
            }
        }

        private void SetUnitType(string unit)
        {
            switch (unit)
            {
                case "cm":
                    SetPrimitiveType(CssPrimitiveType.Cm);
                    break;
                case "mm":
                    SetPrimitiveType(CssPrimitiveType.Mm);
                    break;
                case "px":
                    SetPrimitiveType(CssPrimitiveType.Px);
                    break;
                case "em":
                    SetPrimitiveType(CssPrimitiveType.Ems);
                    break;
                case "ex":
                    SetPrimitiveType(CssPrimitiveType.Exs);
                    break;
                case "pc":
                    SetPrimitiveType(CssPrimitiveType.Pc);
                    break;
                case "pt":
                    SetPrimitiveType(CssPrimitiveType.Pt);
                    break;
                case "in":
                    SetPrimitiveType(CssPrimitiveType.In);
                    break;
                case "%":
                    SetPrimitiveType(CssPrimitiveType.Percentage);
                    break;
                case "":
                    SetPrimitiveType(CssPrimitiveType.Number);
                    break;
                default:
                    throw new DomException(DomExceptionType.SyntaxErr, "Unknown length unit");
            }
        }

        // only for absolute values
        private double GetPxLength()
        {
            switch (PrimitiveType)
            {
                case CssPrimitiveType.In:
                    return _floatValue * Dpi;
                case CssPrimitiveType.Cm:
                    return _floatValue / CmPerIn * Dpi;
                case CssPrimitiveType.Mm:
                    return _floatValue / 10 / CmPerIn * Dpi;
                case CssPrimitiveType.Pt:
                    return _floatValue / 72 * Dpi;
                case CssPrimitiveType.Pc:
                    return _floatValue / 6 * Dpi;
                default:
                    return _floatValue;
            }
        }

        private double GetInchLength()
        {
            return GetPxLength() / Dpi;
        }

        #endregion
    }
}
