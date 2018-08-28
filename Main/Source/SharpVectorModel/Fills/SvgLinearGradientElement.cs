using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgLinearGradientElement : SvgGradientElement, ISvgLinearGradientElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x1;
        private ISvgAnimatedLength _y1;
        private ISvgAnimatedLength _x2;
        private ISvgAnimatedLength _y2;

        #endregion

        #region Constructors and Destructor

        public SvgLinearGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgLinearGradientElement Members

        public ISvgAnimatedLength X1
        {
            get {
                if (!HasAttribute("x1") && ReferencedElement != null)
                {
                    return ReferencedElement.X1;
                }
                if (_x1 == null)
                {
                    _x1 = new SvgAnimatedLength(this, "x1", SvgLengthDirection.Horizontal, "0%");
                }

                return _x1;
            }
        }

        public ISvgAnimatedLength Y1
        {
            get {
                if (!HasAttribute("y1") && ReferencedElement != null)
                {
                    return ReferencedElement.Y1;
                }
                if (_y1 == null)
                {
                    _y1 = new SvgAnimatedLength(this, "y1", SvgLengthDirection.Vertical, "0%");
                }

                return _y1;
            }
        }

        public ISvgAnimatedLength X2
        {
            get {
                if (!HasAttribute("x2") && ReferencedElement != null)
                {
                    return ReferencedElement.X2;
                }
                if (_x2 == null)
                {
                    _x2 = new SvgAnimatedLength(this, "x2", SvgLengthDirection.Horizontal, "100%");
                }

                return _x2;
            }
        }

        public ISvgAnimatedLength Y2
        {
            get {
                if (!HasAttribute("y2") && ReferencedElement != null)
                {
                    return ReferencedElement.Y2;
                }
                if (_y2 == null)
                {
                    _y2 = new SvgAnimatedLength(this, "y2", SvgLengthDirection.Vertical, "0%");
                }

                return _y2;
            }
        }
        #endregion

        #region ISvgURIReference Members

        public new SvgLinearGradientElement ReferencedElement
        {
            get {
                return base.ReferencedElement as SvgLinearGradientElement;
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
					case "x1":
						x1 = null;
						break;
					case "y1":
						y1 = null;
						break;
					case "x2":
						x2 = null;
						break;
					case "y2":
						y2 = null;
						break;
				}
			}
		}*/

        #endregion
    }
}
