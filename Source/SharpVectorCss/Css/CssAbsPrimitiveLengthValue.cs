using System;
using System.Xml;

namespace SharpVectors.Dom.Css
{
    public class CssAbsPrimitiveLengthValue : CssPrimitiveLengthValue
    {
        #region Private Fields

        private string _propertyName;
        private XmlElement _element;
        private CssPrimitiveValue _cssValue;

        #endregion

        #region Constructors and Destructor

        public CssAbsPrimitiveLengthValue(CssPrimitiveValue cssValue, string propertyName, XmlElement element)
        {
            _cssValue = cssValue;

            if (cssValue.PrimitiveType == CssPrimitiveType.Ident)
            {
                // this is primarily to deal with font sizes.
                float absSize;
                switch (cssValue.GetStringValue())
                {
                    case "xx-small":
                        absSize = 6F;
                        break;
                    case "x-small":
                        absSize = 7.5F;
                        break;
                    case "small":
                        absSize = 8.88F;
                        break;
                    case "large":
                        absSize = 12F;
                        break;
                    case "x-large":
                        absSize = 15F;
                        break;
                    case "xx-large":
                        absSize = 20F;
                        break;
                    case "larger":
                    case "smaller":
                        float parSize;
                        if (_parentElement != null)
                        {
                            CssStyleDeclaration csd = (CssStyleDeclaration)_ownerDocument.GetComputedStyle(_parentElement, string.Empty);
                            CssPrimitiveValue cssPrimValue = csd.GetPropertyCssValue("font-size") as CssPrimitiveValue;

                            // no default font-size set => use 10px
                            if (cssPrimValue == null)
                            {
                                parSize = 10;
                            }
                            else
                            {
                                parSize = (float)cssPrimValue.GetFloatValue(CssPrimitiveType.Px);
                            }
                        }
                        else
                        {
                            parSize = 10;
                        }
                        if (cssValue.GetStringValue() == "smaller") absSize = parSize / 1.2F;
                        else absSize = parSize * 1.2F;
                        break;
                    default:
                        absSize = 10F;
                        break;
                }
                SetFloatValue(absSize);
                SetPrimitiveType(CssPrimitiveType.Px);
            }
            else
            {
                SetFloatValue(cssValue.GetFloatValue(cssValue.PrimitiveType));
                SetPrimitiveType(cssValue.PrimitiveType);
            }
            _propertyName = propertyName;
            _element = element;
        }

        #endregion

        #region Public Properties

        public string PropertyName
        {
            get {
                return _propertyName;
            }
        }

        public override bool ReadOnly
        {
            get {
                return false;
            }
        }

        public override bool IsAbsolute
        {
            get {
                return true;
            }
        }

        public CssPrimitiveValue CssValue
        {
            get {
                return _cssValue;
            }
        }

        #endregion

        #region Private Properties

        private XmlElement _parentElement
        {
            get {
                return _element.ParentNode as XmlElement;
            }
        }

        private CssXmlDocument _ownerDocument
        {
            get {
                return _element.OwnerDocument as CssXmlDocument;
            }
        }

        #endregion

        #region Private Methods

        private double GetFontSize()
        {
            CssPrimitiveValue cssPrimValue;
            XmlElement elmToUse;
            if (_propertyName != null && _propertyName.Equals("font-size"))
            {
                elmToUse = _parentElement;
            }
            else
            {
                elmToUse = _element;
            }

            if (elmToUse == null)
            {
                return 10;
            }
            CssStyleDeclaration csd = (CssStyleDeclaration)_ownerDocument.GetComputedStyle(elmToUse, string.Empty);
            cssPrimValue = csd.GetPropertyCssValue("font-size") as CssPrimitiveValue;

            // no default font-size set => use 10px
            if (cssPrimValue == null)
            {
                return 10;
            }
            return cssPrimValue.GetFloatValue(CssPrimitiveType.Px);
        }

        private double GetPxLength()
        {
            //double floatValue = this. _cssValue.GetFloatValue(_cssValue.PrimitiveType);
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
                case CssPrimitiveType.Ems:
                    return _floatValue * GetFontSize();
                case CssPrimitiveType.Exs:
                    return _floatValue * GetFontSize() * 0.5F;
                default:
                    return _floatValue;
            }
        }

        private double GetInLength()
        {
            return GetPxLength() / Dpi;
        }

        #endregion

        #region Overrides of CssPrimitiveLengthValue

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            if (PrimitiveType == CssPrimitiveType.Percentage)
            {
                if (unitType == CssPrimitiveType.Percentage)
                {
                    return _floatValue;
                }
                throw new NotImplementedException("Can't get absolute values from percentages");
            }

            double ret;
            switch (unitType)
            {
                case CssPrimitiveType.Cm:
                    ret = GetInLength() * CmPerIn;
                    break;
                case CssPrimitiveType.Mm:
                    ret = GetInLength() * CmPerIn * 10;
                    break;
                case CssPrimitiveType.In:
                    ret = GetInLength();
                    break;
                case CssPrimitiveType.Pc:
                    ret = GetInLength() * 6;
                    break;
                case CssPrimitiveType.Pt:
                    ret = GetInLength() * 72;
                    break;
                case CssPrimitiveType.Ems:
                    ret = GetPxLength() / GetFontSize();
                    break;
                case CssPrimitiveType.Exs:
                    ret = GetPxLength() / (GetFontSize() * 0.5);
                    break;
                default:
                    ret = GetPxLength();
                    break;
            }
            return ret;
        }

        #endregion
    }
}
