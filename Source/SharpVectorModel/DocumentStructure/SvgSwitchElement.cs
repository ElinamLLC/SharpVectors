using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgSwitchElement interface corresponds to the 'switch' element. 
	/// </summary>
    public sealed class SvgSwitchElement : SvgTransformableElement, ISvgSwitchElement
    {
        #region Private Fields

        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgSwitchElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
			_externalResourcesRequired = new SvgExternalResourcesRequired(this);
			_svgTests                  = new SvgTests(this);
		}

		#endregion

        #region ISvgElement Members

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

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);

            if (this.HasChildNodes)
            {
                visitor.BeginContainer(this);
                foreach (var item in this.ChildNodes)
                {
                    if (item is IElementVisitorTarget evt)
                    {
                        evt.Accept(visitor);
                    }
                }
                visitor.EndContainer(this);
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
