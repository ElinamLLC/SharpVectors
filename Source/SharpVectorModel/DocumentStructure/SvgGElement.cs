using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgGElement interface corresponds to the 'g' element.
    /// </summary>
    public sealed class SvgGElement : SvgTransformableElement, ISvgGElement
    {
        #region Private Fields

        private SvgTests _svgTests;

        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors

        public SvgGElement(string prefix, string localname, string ns, SvgDocument doc)
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

        #region Implementation of ISvgExternalResourcesRequired

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

        #region Implementation of ISvgTests

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
