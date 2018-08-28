using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgSymbolElement : SvgStyleableElement, ISvgSymbolElement
    {
        #region Private Fields

        private SvgFitToViewBox _fitToViewBox;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgSymbolElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
			_externalResourcesRequired = new SvgExternalResourcesRequired(this);
			_fitToViewBox              = new SvgFitToViewBox(this);
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
                XmlNode parentNode = this.ParentNode;
                if (parentNode != null && string.Equals(parentNode.LocalName, 
                    "use", StringComparison.Ordinal))
                {
                    return true;
                }

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

		#region ISvgExternalResourcesRequired Members

		public ISvgAnimatedBoolean ExternalResourcesRequired
		{
			get
			{
				return _externalResourcesRequired.ExternalResourcesRequired;
			}
		}

		#endregion

		#region ISvgFitToViewBox Members

		public ISvgAnimatedRect ViewBox
		{
			get
			{
				return _fitToViewBox.ViewBox;
			}
		}

		public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return _fitToViewBox.PreserveAspectRatio;
			}
		}

        #endregion   
	}
}
