using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgClipPathElement : SvgTransformableElement, ISvgClipPathElement
    {
        #region Private Fields

        private SvgTests _svgTests;
        private ISvgAnimatedEnumeration _clipPathUnits;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region	Constructors and Destructor

        public SvgClipPathElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
			_externalResourcesRequired = new SvgExternalResourcesRequired(this);
			_svgTests = new SvgTests(this);
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
				if (_clipPathUnits == null)
				{
					SvgUnitType clipPath = SvgUnitType.UserSpaceOnUse;
					if (GetAttribute("clipPathUnits") == "objectBoundingBox")
					{
						clipPath = SvgUnitType.ObjectBoundingBox;
					}

					_clipPathUnits = new SvgAnimatedEnumeration((ushort)clipPath);
				}

				return _clipPathUnits;
			}
		}

		#endregion

        #region ISvgExternalResourcesRequired Members
        
        public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return _externalResourcesRequired.ExternalResourcesRequired;
			}
		}

		#endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
		{
			get { return _svgTests.RequiredFeatures; }
		}

		public ISvgStringList RequiredExtensions
		{
			get { return _svgTests.RequiredExtensions; }
		}

		public ISvgStringList SystemLanguage
		{
			get { return _svgTests.SystemLanguage; }
		}

		public bool HasExtension(string extension)
		{
			return _svgTests.HasExtension(extension);
		}

        #endregion
	}
}
