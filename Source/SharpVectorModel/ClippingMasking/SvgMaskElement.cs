using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgMaskElement : SvgStyleableElement, ISvgMaskElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private ISvgAnimatedEnumeration _maskUnits;
        private ISvgAnimatedEnumeration _maskContentUnits;

        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgMaskElement(string prefix, string localname, string ns, SvgDocument doc)
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
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Masking"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Masking;
            }
        }

        #endregion

        #region ISvgMaskElement Members

        public ISvgAnimatedEnumeration MaskUnits
        {
            get {
                if (_maskUnits == null)
                {
                    SvgUnitType mask = SvgUnitType.ObjectBoundingBox;
                    if (string.Equals(this.GetAttribute("maskUnits"), "userSpaceOnUse", StringComparison.Ordinal))
                        mask = SvgUnitType.UserSpaceOnUse;

                    _maskUnits = new SvgAnimatedEnumeration((ushort)mask);
                }
                return _maskUnits;
            }
        }

        public ISvgAnimatedEnumeration MaskContentUnits
        {
            get {
                if (_maskContentUnits == null)
                {
                    SvgUnitType maskContent = SvgUnitType.UserSpaceOnUse;
                    if (string.Equals(this.GetAttribute("maskContentUnits"), "objectBoundingBox", StringComparison.Ordinal))
                        maskContent = SvgUnitType.ObjectBoundingBox;
                    _maskContentUnits = new SvgAnimatedEnumeration((ushort)maskContent);
                }
                return _maskContentUnits;
            }
        }

        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "-10%");
                }
                return _x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "-10%");
                }
                return _y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Viewport, "120%");
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Viewport, "120%");
                }
                return _height;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
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
