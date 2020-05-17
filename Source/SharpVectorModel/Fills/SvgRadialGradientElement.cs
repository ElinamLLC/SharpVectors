using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgRadialGradientElement.
    /// </summary>
    public sealed class SvgRadialGradientElement : SvgGradientElement, ISvgRadialGradientElement
    {
        #region Private Fields

        private ISvgAnimatedLength _cx;
        private ISvgAnimatedLength _cy;
        private ISvgAnimatedLength _r;
        private ISvgAnimatedLength _fx;
        private ISvgAnimatedLength _fy;

        #endregion

        #region Constructors and Destructor

        public SvgRadialGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgRadialGradientElement Members

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

        public ISvgAnimatedLength Cx
        {
            get {
                if (!HasAttribute("cx") && ReferencedElement != null)
                {
                    return ReferencedElement.Cx;
                }
                if (_cx == null)
                {
                    _cx = new SvgAnimatedLength(this, "cx", SvgLengthDirection.Horizontal, "50%");
                }
                return _cx;
            }
        }

        public ISvgAnimatedLength Cy
        {
            get {
                if (!HasAttribute("cy") && ReferencedElement != null)
                {
                    return ReferencedElement.Cy;
                }
                if (_cy == null)
                {
                    _cy = new SvgAnimatedLength(this, "cy", SvgLengthDirection.Vertical, "50%");
                }
                return _cy;
            }
        }

        public ISvgAnimatedLength R
        {
            get {
                if (!HasAttribute("r") && ReferencedElement != null)
                {
                    return ReferencedElement.R;
                }
                if (_r == null)
                {
                    _r = new SvgAnimatedLength(this, "r", SvgLengthDirection.Viewport, "50%");
                }
                return _r;
            }
        }

        public ISvgAnimatedLength Fx
        {
            get {
                if (!HasAttribute("fx") && HasAttribute("fy"))
                {
                    return Fy;
                }
                if (!HasAttribute("fx") && ReferencedElement != null)
                {
                    return ReferencedElement.Fx;
                }
                if (_fx == null)
                {
                    _fx = new SvgAnimatedLength(this, "fx", SvgLengthDirection.Horizontal, "50%");
                }
                return _fx;
            }
        }

        public ISvgAnimatedLength Fy
        {
            get {
                if (!HasAttribute("fy") && HasAttribute("fx"))
                {
                    return Fx;
                }
                if (!HasAttribute("fy") && ReferencedElement != null)
                {
                    return ReferencedElement.Fy;
                }
                if (_fy == null)
                {
                    _fy = new SvgAnimatedLength(this, "fy", SvgLengthDirection.Vertical, "50%");
                }
                return _fy;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public new SvgRadialGradientElement ReferencedElement
        {
            get {
                return base.ReferencedElement as SvgRadialGradientElement;
            }
        }

        #endregion

        #region Update handling
        /*public override void OnAttributeChange(XmlNodeChangedAction action, XmlAttribute attribute)
		{
			base.OnAttributeChange(action, attribute);

			if(attribute.NamespaceURI.Length == 0)
			{
				switch(attribute.LocalName)
				{
					case "cx":
						cx = null;
						break;
					case "cy":
						cy = null;
						break;
					case "r":
						r = null;
						break;
					case "fx":
						fx = null;
						break;
					case "fy":
						fy = null;
						break;
				}
			}
		}*/
        #endregion
    }
}
