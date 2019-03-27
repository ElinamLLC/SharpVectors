using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgExternalResourcesRequired
	{
		private SvgElement _ownerElement;
		private ISvgAnimatedBoolean _externalResourcesRequired;

		public SvgExternalResourcesRequired(SvgElement ownerElement)
		{
			_ownerElement = ownerElement;
			_ownerElement.attributeChangeHandler += OnAttributeChange;
		}

		private void OnAttributeChange(object src, XmlNodeChangedEventArgs args)
		{
			XmlAttribute attribute = src as XmlAttribute;

			if (attribute != null && string.Equals(attribute.LocalName, 
                "externalResourcesRequired", StringComparison.OrdinalIgnoreCase))
			{
				_externalResourcesRequired = null;
			}
		}

		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				if (_externalResourcesRequired == null)
				{
					_externalResourcesRequired = new SvgAnimatedBoolean(
                        _ownerElement.GetAttribute("externalResourcesRequired"), false);
				}
				return _externalResourcesRequired;
			}
		}
	
	}
}
