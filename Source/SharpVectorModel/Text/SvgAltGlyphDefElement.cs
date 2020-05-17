using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This implements the <see cref="ISvgAltGlyphDefElement"/> interface corresponds to the 'altGlyphDef' element.
    /// </summary>
    /// <remarks>
    /// *Content model:* Either
    /// <list type="bullet">
    /// <item><term>one or more 'glyphRef' elements, or</term>
    /// <description>In the simplest case, an 'altGlyphDef' contains one or more 'glyphRef' elements. 
    /// Each 'glyphRef' element references a single glyph within a particular font.
    /// </description>
    /// </item>
    /// <item><term>one or more 'altGlyphItem' elements.</term>
    /// <description>In the more complex case, an 'altGlyphDef' contains one or more 'altGlyphItem' elements. 
    /// Each 'altGlyphItem' represents a candidate set of substitute glyphs. Each 'altGlyphItem' contains 
    /// one or more 'glyphRef' elements. Each 'glyphRef' element references a single glyph within a particular font. 
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class SvgAltGlyphDefElement : SvgElement, ISvgAltGlyphDefElement
    {
        #region Private Fields

        private bool? _isSimple;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgAltGlyphDefElement"/> class with the specified parameters.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localname"></param>
        /// <param name="ns"></param>
        /// <param name="doc"></param>
        public SvgAltGlyphDefElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgAltGlyphDefElement Members

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
        /// Gets a value indicating whether this is a simple content model.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this element defines a simple content model that contains only 'glyphRef'
        /// elements, otherwise returns <see langword="false"/>.
        /// </value>
        public bool IsSimple
        {
            get {
                if (_isSimple == null) // Ensure _isSimple is not null
                {
                    this.Initialize();
                }
                return _isSimple.Value;
            }
        }

        /// <summary>
        /// Gets the first child element with the name 'glyphRef'.
        /// </summary>
        /// <value>
        /// The first <see cref="SvgGlyphRefElement"/> if matched. It returns a <see langword="null"/> if there is no match.
        /// </value>
        public SvgGlyphRefElement GlyphRef
        {
            get {
                return this["glyphRef"] as SvgGlyphRefElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISvgGlyphRefElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgGlyphRefElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgGlyphRefElement"/> specifying the 'glyphRef' element of the specifiied name or ID.
        /// <para>
        /// This will always return <see langword="null"/>, if the <see cref="IsSimple"/> is <see langword="false"/>.
        /// </para>
        /// </returns>
        public ISvgGlyphRefElement GetGlyphRef(string name)
        {
            if (_isSimple == null) // Ensure _isSimple is not null
            {
                this.Initialize();
            }

            if (this.HasChildNodes && _isSimple.Value)
            {
                var xpath = string.Format("./glyphRef[@id='{0}']", name);
                return this.SelectSingleNode(xpath) as SvgGlyphRefElement;
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="ISvgAltGlyphItemElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgAltGlyphItemElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgAltGlyphItemElement"/> specifying the 'altGlyphItem' element of the specifiied name or ID.
        /// <para>
        /// This will always return <see langword="null"/>, if the <see cref="IsSimple"/> is <see langword="true"/>.
        /// </para>
        /// </returns>
        public ISvgAltGlyphItemElement GetGlyphItem(string name)
        {
            if (_isSimple == null) // Ensure _isSimple is not null
            {
                this.Initialize();
            }

            if (this.HasChildNodes && !_isSimple.Value)
            {
                var xpath = string.Format("./altGlyphItem[@id='{0}']", name);
                return this.SelectSingleNode(xpath) as SvgAltGlyphItemElement;
            }
            return null;
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            _isSimple = true; // Assume it is simple content model

            if (this.HasChildNodes)
            {
                // altGlyphItem element will contain glyphRef, we rather check the presence of altGlyphItem
                var altGlyphItem = this["altGlyphItem"] as SvgAltGlyphItemElement;
                _isSimple = (altGlyphItem == null);
            }
        }

        #endregion
    }
}
