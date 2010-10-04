using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPatternElement : SvgStyleableElement, ISvgPatternElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration patternUnits;
        private ISvgAnimatedEnumeration patternContentUnits;
        private ISvgAnimatedTransformList patternTransform;
        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private SvgUriReference svgURIReference;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;
        private SvgFitToViewBox svgFitToViewBox;
        private SvgTests svgTests;

        #endregion

        #region Constructors and Destructor

        public SvgPatternElement(string prefix, string localname, string ns, SvgDocument doc)
			: base(prefix, localname, ns, doc)
		{
			svgURIReference              = new SvgUriReference(this);
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
			svgFitToViewBox              = new SvgFitToViewBox(this);
			svgTests                     = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public XmlNodeList Children
		{
			get
			{
				XmlNodeList children = SelectNodes("svg:*", OwnerDocument.NamespaceManager);
				if(children.Count > 0)
				{
					return children;
				}
				else
				{
					// check any eventually referenced gradient
					if(ReferencedElement == null)
					{
						// return an empty list
						return children;
					}
					else
					{
						return ReferencedElement.Children;
					}
				}
			}
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
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get
            {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

		#region ISvgPatternElement Members

		public ISvgAnimatedEnumeration PatternUnits
		{
			get
			{
				if(!HasAttribute("patternUnits") && ReferencedElement != null)
				{
					return ReferencedElement.PatternUnits;
				}
				else
				{
					if(patternUnits == null)
					{
						SvgUnitType type;
						switch(GetAttribute("patternUnits"))
						{
							case "userSpaceOnUse":
								type = SvgUnitType.UserSpaceOnUse;
								break;
							default:
								type = SvgUnitType.ObjectBoundingBox;
								break;
						}
						patternUnits = new SvgAnimatedEnumeration((ushort)type);
					}
					return patternUnits;
				}
			}
		}

		public ISvgAnimatedEnumeration PatternContentUnits
		{
			get
			{
				if(!HasAttribute("patternContentUnits") && ReferencedElement != null)
				{
					return ReferencedElement.PatternContentUnits;
				}
				else
				{
					if(patternContentUnits == null)
					{
						SvgUnitType type;
						switch(GetAttribute("patternContentUnits"))
						{
							case "objectBoundingBox":
								type = SvgUnitType.ObjectBoundingBox;
								break;
							default:
								type = SvgUnitType.UserSpaceOnUse;
								break;
						}
						patternContentUnits = new SvgAnimatedEnumeration((ushort)type);
					}
					return patternContentUnits;
				}
			}
		}

		public ISvgAnimatedTransformList PatternTransform
		{
			get
			{
				if(!HasAttribute("patternTransform") && ReferencedElement != null)
				{
					return ReferencedElement.PatternTransform;
				}
				else{
					if(patternTransform == null)
					{
						patternTransform = new SvgAnimatedTransformList(GetAttribute("patternTransform"));
					}
					return patternTransform;
				}
			}
		}

		public ISvgAnimatedLength X
		{
			get
			{
				if(!HasAttribute("x") && ReferencedElement != null)
				{
					return ReferencedElement.X;
				}
				else{
					if(x == null)
					{
						x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
					}
					return x;
				}
			}
		}

		public ISvgAnimatedLength Y
		{
			get
			{
				if(!HasAttribute("y") && ReferencedElement != null)
				{
					return ReferencedElement.Y;
				}
				else
				{
					if(y == null)
					{
						y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
					}
					return y;
				}
			}
		}

		public ISvgAnimatedLength Width
		{
			get
			{
				if(!HasAttribute("width") && ReferencedElement != null)
				{
					return ReferencedElement.Width;
				}
				else
				{
					if(width == null)
					{
						width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
					}
					return width;
				}
			}
		}

		public ISvgAnimatedLength Height
		{
			get
			{
				if(!HasAttribute("height") && ReferencedElement != null)
				{
					return ReferencedElement.Height;
				}
				else
				{
					if(height == null)
					{
						height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
					}
					return height;
				}
			}
		}

		#endregion

		#region ISvgURIReference Members

		public ISvgAnimatedString Href
		{
			get
			{
				return svgURIReference.Href;
			}
		}

		public SvgPatternElement ReferencedElement
		{
			get
			{
				return svgURIReference.ReferencedNode as SvgPatternElement;
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

		#region ISvgFitToViewBox Members

		public ISvgAnimatedRect ViewBox
		{
			get
			{
				return svgFitToViewBox.ViewBox;
			}
		}

		public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return svgFitToViewBox.PreserveAspectRatio;
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
