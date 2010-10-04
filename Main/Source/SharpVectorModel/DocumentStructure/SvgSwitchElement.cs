using System;
using System.Xml;

using SharpVectors.Text;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// The SvgSwitchElement interface corresponds to the 'switch' element. 
	/// </summary>
    public sealed class SvgSwitchElement : SvgTransformableElement, ISvgSwitchElement
    {
        #region Private Fields

        private SvgTests svgTests;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgSwitchElement(string prefix, string localname, string ns, SvgDocument doc) 
            : base(prefix, localname, ns, doc) 
		{
			svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
			svgTests = new SvgTests(this);
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
