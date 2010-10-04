using System;
using System.Xml;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgExternalResourcesRequired
	{
		public SvgExternalResourcesRequired(SvgElement ownerElement)
		{
			this.ownerElement = ownerElement;

			this.ownerElement.attributeChangeHandler += new NodeChangeHandler(AttributeChange);
		}
		private SvgElement ownerElement;


		private void AttributeChange(Object src, XmlNodeChangedEventArgs args)
		{
			XmlAttribute attribute = src as XmlAttribute;

			if(attribute.LocalName == "externalResourcesRequired")
			{
				externalResourcesRequired = null;
			}
		}

		private ISvgAnimatedBoolean externalResourcesRequired;
		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				if(externalResourcesRequired == null)
				{
					externalResourcesRequired = new SvgAnimatedBoolean(ownerElement.GetAttribute("externalResourcesRequired"), false);
				}
				return externalResourcesRequired;
			}
		}
	
	}
}
