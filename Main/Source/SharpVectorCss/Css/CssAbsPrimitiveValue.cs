using System;
using System.Xml;

namespace SharpVectors.Dom.Css
{
	public class CssAbsPrimitiveValue : CssPrimitiveValue
	{
        private string            _propertyName;
        private XmlElement        _element;
        private CssPrimitiveValue _cssValue;

		public CssAbsPrimitiveValue(CssPrimitiveValue cssValue, string propertyName, 
            XmlElement element)
		{
			_cssValue     = cssValue;
			_propertyName = propertyName;
			_element      = element;
		}

		public override string CssText
		{
			get
			{
				return _cssValue.CssText;
			}
		}

		public override double GetFloatValue(CssPrimitiveType unitType)
		{
			return _cssValue.GetFloatValue(unitType);
		}

		public override string GetStringValue()
		{
			switch(PrimitiveType)
			{
				case CssPrimitiveType.Attr:
					return _element.GetAttribute(_cssValue.GetStringValue(), String.Empty);
				default:
					return _cssValue.GetStringValue();
			}
		}

		public override ICssRect GetRectValue()
		{
			return _cssValue.GetRectValue();
		}

		public override ICssColor GetRgbColorValue()
		{
			return _cssValue.GetRgbColorValue();
		}

		public override CssPrimitiveType PrimitiveType
		{
			get
			{
				return _cssValue.PrimitiveType;
			}
		}
	}
}
