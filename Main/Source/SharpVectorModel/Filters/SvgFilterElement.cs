using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgFilterElement.
	/// </summary>
    public sealed class SvgFilterElement : SvgStyleableElement, ISvgFilterElement
	{
		#region Constructors
		internal SvgFilterElement(string prefix, string localname, string ns, SvgDocument doc) : base(prefix, localname, ns, doc)
		{
			svgURIReference = new SvgUriReference(this);
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
		}
		#endregion

		#region ISvgFilterElement Interface

		#region ISvgFilterElement Properties

		public ISvgAnimatedEnumeration FilterUnits
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedEnumeration PrimitiveUnits
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedLength X
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedLength Y
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedLength Width
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedLength Height
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedInteger FilterResX
		{
			get
			{
				return null;
			}
		}

		public ISvgAnimatedInteger FilterResY
		{
			get
			{
				return null;
			}
		}
		
		#endregion

		#region ISvgFilterElement Methods
		
		public void SetFilterRes(ulong filterResX,ulong filterResY)
		{

		}

		#endregion
		#endregion

		#region Implementation of ISvgURIReference
		private SvgUriReference svgURIReference;
		public ISvgAnimatedString Href
		{
			get
			{
				return svgURIReference.Href;
			}
		}
		#endregion

		#region Implementation of ISvgExternalResourcesRequired
		private SvgExternalResourcesRequired svgExternalResourcesRequired;
		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return svgExternalResourcesRequired.ExternalResourcesRequired;
			}
		}
		#endregion

	}
}
