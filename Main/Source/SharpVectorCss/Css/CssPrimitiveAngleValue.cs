using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    public sealed class CssPrimitiveAngleValue : CssPrimitiveValue
	{
		#region Constructors
		public CssPrimitiveAngleValue(string number, string unit, bool readOnly) : base(number+unit, readOnly)
		{
			_setType(unit);
			SetFloatValue(number);
		}

		public CssPrimitiveAngleValue(string cssText, bool readOnly) : base(cssText, readOnly)
		{
			OnSetCssText(cssText);
		}

		public CssPrimitiveAngleValue(double number, string unit, bool readOnly) : base(number+unit, readOnly)
		{
			_setType(unit);
			SetFloatValue(number);
		}
		#endregion

		protected override void OnSetCssText(string cssText)
		{
			Regex re = new Regex(CssValue.AnglePattern);
			Match match = re.Match(cssText);
			if(match.Success)
			{
				_setType(match.Groups["angleUnit"].Value);
				SetFloatValue(match.Groups["angleNumber"].Value);
			}
			else
			{
				throw new DomException(DomExceptionType.SyntaxErr, "Unrecognized angle format: " + cssText);
			}
		}
		private void _setType(string unit)
		{
			switch(unit)
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
				case "grad":
					SetPrimitiveType(CssPrimitiveType.Grad);
					break;
				default:
					throw new DomException(DomExceptionType.SyntaxErr, "Unknown angle unit");
			}
		}

		// only for absolute values
		private double _getDegAngle()
		{
			double ret;
			switch(PrimitiveType)
			{
				case CssPrimitiveType.Rad:
					ret = floatValue * 180 / Math.PI;
					break;
				case CssPrimitiveType.Grad:
					ret = floatValue *90 / 100;
					break;
				default:
					ret = floatValue;
					break;
			}
			ret %= 360;
			while(ret<0) ret += 360;

			return ret;
		}

		public override double GetFloatValue(CssPrimitiveType unitType)
		{
			double ret = Double.NaN;
			switch(unitType)
			{
				case CssPrimitiveType.Number:
				case CssPrimitiveType.Deg:
					ret = _getDegAngle();
					break;
				case CssPrimitiveType.Rad:
					ret = _getDegAngle() * Math.PI / 180;
					break;
				case CssPrimitiveType.Grad:
					ret = _getDegAngle() * 100 / 90;
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
