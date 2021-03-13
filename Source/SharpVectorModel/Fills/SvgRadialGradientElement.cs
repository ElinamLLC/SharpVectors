using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The implementation of the <c>radialGradient</c> element or the <see cref="ISvgRadialGradientElement"/> interface.
    /// </summary>
    /// <remarks>
    /// Radial Gradient fx/fy values should only be inherited from a referenced element if that element is explicitly 
    /// defining them, otherwise they should follow the cy special case behavior. Additionally, because xlink references 
    /// can inherit to an arbitrary level, we should walk up the tree looking for explicitly defined fx/fy values to 
    /// inherit before falling back to the cx/cy definitions.
    /// </remarks>
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
                var refElem = this.ReferencedElement;
                if (!HasAttribute("cx") && refElem != null)
                {
                    return refElem.Cx;
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
                var refElem = this.ReferencedElement;
                if (!HasAttribute("cy") && refElem != null)
                {
                    return refElem.Cy;
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
                var refElem = this.ReferencedElement;
                if (!HasAttribute("r") && refElem != null)
                {
                    return refElem.R;
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
                var refElem = this.ReferencedElement;
                if (!HasAttribute("fx") && refElem != null)
                {
                    if (!refElem.HasAttribute("fx"))
                    {
                        var nextRef = refElem.ReferencedElement;
                        while (nextRef != null)
                        {
                            if (nextRef.HasAttribute("fx"))
                            {
                                return nextRef.Fx;
                            }
                            nextRef = nextRef.ReferencedElement;
                        }

                        if (HasAttribute("cx"))
                        {
                            return Cx;
                        }
                    }
                    return refElem.Fx;
                }
                if (!HasAttribute("fx") && HasAttribute("cx"))
                {
                    return Cx;
                }
                if (!HasAttribute("fx") && HasAttribute("fy"))
                {
                    return Fy;
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
                var refElem = this.ReferencedElement;
                if (!HasAttribute("fy") && refElem != null)
                {
                    if (!refElem.HasAttribute("fy"))
                    {
                        var nextRef = refElem.ReferencedElement;
                        while (nextRef != null)
                        {
                            if (nextRef.HasAttribute("fy"))
                            {
                                return nextRef.Fy;
                            }
                            nextRef = nextRef.ReferencedElement;
                        }

                        if (HasAttribute("cy"))
                        {
                            return Cy;
                        }
                    }
                    return refElem.Fy;
                }
                if (!HasAttribute("fy") && HasAttribute("cy"))
                {
                    return Cy;
                }
                if (!HasAttribute("fy") && HasAttribute("fx"))
                {
                    return Fx;
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
