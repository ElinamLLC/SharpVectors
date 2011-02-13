using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
	public enum SvgSpreadMethod
    {
		Pad     = 0, 
		Reflect = 1, 
		Repeat  = 2,
        None    = 3,
	}

	public abstract class SvgGradientElement : SvgStyleableElement, ISvgGradientElement
    {  
        private ISvgAnimatedEnumeration gradientUnits;
        private ISvgAnimatedEnumeration spreadMethod;
        private ISvgAnimatedTransformList gradientTransform;

		protected SvgGradientElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
			svgURIReference              = new SvgUriReference(this);
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
		}

		#region ISvgGradientElement Members

		public ISvgAnimatedEnumeration GradientUnits
		{
			get
			{
				if (!HasAttribute("gradientUnits") && ReferencedElement != null)
				{
					return ReferencedElement.GradientUnits;
				}
				else
				{
					if(gradientUnits == null)
					{
						SvgUnitType gradUnit;
						switch(GetAttribute("gradientUnits"))
						{
							case "userSpaceOnUse":
								gradUnit = SvgUnitType.UserSpaceOnUse;
								break;
							default:
								gradUnit = SvgUnitType.ObjectBoundingBox;
								break;
						}

						gradientUnits = new SvgAnimatedEnumeration((ushort)gradUnit);
					}
					return gradientUnits;
				}
			}
		}

		public ISvgAnimatedTransformList GradientTransform
		{
			get
			{
				if (!HasAttribute("gradientTransform") && ReferencedElement != null)
				{
					return ReferencedElement.GradientTransform;
				}
				else
				{
					if (gradientTransform == null)
					{
						gradientTransform = new SvgAnimatedTransformList(GetAttribute("gradientTransform"));
					}

					return gradientTransform;
				}
			}
		}

		public ISvgAnimatedEnumeration SpreadMethod
		{
			get
			{
				if (!HasAttribute("spreadMethod") && ReferencedElement != null)
				{
					return ReferencedElement.SpreadMethod;
				}
				else
				{
					if (spreadMethod == null)
					{
						SvgSpreadMethod spreadMeth;
						switch (GetAttribute("spreadMethod"))
						{
                            case "pad":
                                spreadMeth = SvgSpreadMethod.Pad;
                                break;
							case "reflect":
								spreadMeth = SvgSpreadMethod.Reflect;
								break;
							case "repeat":
								spreadMeth = SvgSpreadMethod.Repeat;
								break;
							default:
								spreadMeth = SvgSpreadMethod.None;
								break;
						}

						spreadMethod = new SvgAnimatedEnumeration((ushort)spreadMeth);
					}

					return spreadMethod;
				}
			}
		}

		#endregion

		#region ISvgURIReference Members

		private SvgUriReference svgURIReference;
		public ISvgAnimatedString Href
		{
			get
			{
				return svgURIReference.Href;
			}
		}

		public SvgGradientElement ReferencedElement
		{
			get
			{
				return svgURIReference.ReferencedNode as SvgGradientElement;
			}
		}

		#endregion

		#region ISvgExternalResourcesRequired Members

		private SvgExternalResourcesRequired svgExternalResourcesRequired;
		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return svgExternalResourcesRequired.ExternalResourcesRequired;
			}
		}

		#endregion

		#region Public Properties

		public XmlNodeList Stops
		{
			get
			{
				XmlNodeList stops = SelectNodes("svg:stop", OwnerDocument.NamespaceManager);
				if(stops.Count > 0)
				{
					return stops;
				}
				else
				{
					// check any eventually referenced gradient
					if(ReferencedElement == null)
					{
						// return an empty list
						return stops;
					}
					else
					{
						return ReferencedElement.Stops;
					}
				}
			}
		}

		#endregion

		#region Update handling

		public override void HandleAttributeChange(XmlAttribute attribute)
		{
			if (attribute.NamespaceURI.Length == 0)
			{
				switch (attribute.LocalName)
				{
					case "gradientUnits":
						gradientUnits = null;
						break;
					case "gradientTransform":
						gradientTransform = null;
						break;
					case "spreadMethod":
						spreadMethod = null;
						break;
				}
			}

            base.HandleAttributeChange(attribute);
		}

		#endregion
	}
}
