// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgClipPathElement : SvgTransformableElement, ISvgClipPathElement
    {
        #region Private Fields

        private SvgTests svgTests;
        private ISvgAnimatedEnumeration clipPathUnits;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region	Constructors and Destructor

        public SvgClipPathElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
			svgTests = new SvgTests(this);
		}

		#endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Clipping"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Clipping;
            }
        }

        #endregion

        #region ISvgClipPathElement Members

        public ISvgAnimatedEnumeration ClipPathUnits
		{
			get
			{
				if(clipPathUnits == null)
				{
					SvgUnitType clipPath = SvgUnitType.UserSpaceOnUse;
					if (GetAttribute("clipPathUnits") == "objectBoundingBox")
					{
						clipPath = SvgUnitType.ObjectBoundingBox;
					}

					clipPathUnits = new SvgAnimatedEnumeration((ushort)clipPath);
				}

				return clipPathUnits;
			}
		}

		#endregion

        #region ISvgExternalResourcesRequired Members
        
        public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return svgExternalResourcesRequired.ExternalResourcesRequired;
			}
		}

		#endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
		{
			get { return svgTests.RequiredFeatures; }
		}

		public ISvgStringList RequiredExtensions
		{
			get { return svgTests.RequiredExtensions; }
		}

		public ISvgStringList SystemLanguage
		{
			get { return svgTests.SystemLanguage; }
		}

		public bool HasExtension(string extension)
		{
			return svgTests.HasExtension(extension);
		}

        #endregion
	}
}
