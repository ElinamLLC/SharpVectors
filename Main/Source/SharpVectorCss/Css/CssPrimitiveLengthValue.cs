using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	public class CssPrimitiveLengthValue : CssPrimitiveValue
	{
		//protected const double Dpi = 96;
        protected const double Dpi = 90; // The common default value for this is 90, not 96
		protected const double CmPerIn = 2.54;

		#region Constructors
		public CssPrimitiveLengthValue(string number, string unit, bool readOnly) : base(number+unit, readOnly)
		{
			_setType(unit);
			SetFloatValue(number);
		}

		public CssPrimitiveLengthValue(string cssText, bool readOnly) : base(cssText, readOnly)
		{
			OnSetCssText(cssText);
		}

		public CssPrimitiveLengthValue(double number, string unit, bool readOnly) : base(number+unit, readOnly)
		{
			_setType(unit);
			SetFloatValue(number);
		}

		protected CssPrimitiveLengthValue() : base()
		{
		}
		#endregion

		public override CssValue GetAbsoluteValue(string propertyName, XmlElement elm)
		{
			return new CssAbsPrimitiveLengthValue(this, propertyName, elm);
		}

		protected override void OnSetCssText(string cssText)
		{
			Regex re = new Regex(CssValue.LengthPattern);
			Match match = re.Match(cssText);
			if(match.Success)
			{
				_setType(match.Groups["lengthUnit"].Value);
				SetFloatValue(match.Groups["lengthNumber"].Value);
			}
			else
			{
				throw new DomException(DomExceptionType.SyntaxErr, "Unrecognized length format: " + cssText);
			}
		}
		private void _setType(string unit)
		{
			switch(unit)
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
		private double _getPxLength()
		{
			switch(PrimitiveType)
			{
				case CssPrimitiveType.In:
					return floatValue * Dpi;
				case CssPrimitiveType.Cm:
					return floatValue / CmPerIn * Dpi;
				case CssPrimitiveType.Mm:
					return floatValue / 10 / CmPerIn * Dpi;
				case CssPrimitiveType.Pt:
					return floatValue / 72 * Dpi;
				case CssPrimitiveType.Pc:
					return floatValue / 6 * Dpi;
				default:
					return floatValue;
			}
		}

		private double _getInLength()
		{
			return _getPxLength() / Dpi;
		}

		public override double GetFloatValue(CssPrimitiveType unitType)
		{
			double ret = Double.NaN;
			switch(PrimitiveType)
			{
				case CssPrimitiveType.Number:
				case CssPrimitiveType.Px:
				case CssPrimitiveType.Cm:
				case CssPrimitiveType.Mm:
				case CssPrimitiveType.In:
				case CssPrimitiveType.Pt:
				case CssPrimitiveType.Pc:
					if(unitType == CssPrimitiveType.Px) ret = _getPxLength();
					else if(unitType == CssPrimitiveType.Number) ret = _getPxLength();
					else if(unitType == CssPrimitiveType.In) ret = _getInLength();
					else if(unitType == CssPrimitiveType.Cm) ret = _getInLength() * CmPerIn;
					else if(unitType == CssPrimitiveType.Mm) ret = _getInLength() * CmPerIn * 10;
					else if(unitType == CssPrimitiveType.Pt) ret = _getInLength() * 72;
					else if(unitType == CssPrimitiveType.Pc) ret = _getInLength() * 6;
					break;
				case CssPrimitiveType.Percentage:
					if(unitType == CssPrimitiveType.Percentage) ret = floatValue;
					break;
				case CssPrimitiveType.Ems:
					if(unitType == CssPrimitiveType.Ems) ret = floatValue;
					break;
				case CssPrimitiveType.Exs:
					if(unitType == CssPrimitiveType.Exs) ret = floatValue;
					break;
			}
			if(Double.IsNaN(ret))
			{
				throw new DomException(DomExceptionType.InvalidAccessErr);
			}
			else
			{
				return ret;
			}
		}

		public override string CssText
		{
			get
			{
				return GetFloatValue(PrimitiveType).ToString(CssNumber.Format) + PrimitiveTypeAsString;
			}
		}
	}
}
