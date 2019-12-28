namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgScriptElement interface corresponds to the 'script' element. 
    /// </summary>
    /// <remarks>
    /// <para>A 'script' element is equivalent to the 'script' element in HTML and thus is the place for scripts.</para>
    /// <para>Any functions defined within any 'script' element have a 'global' scope across the entire current document.</para>
    /// </remarks>
    public interface ISvgScriptElement : ISvgElement, ISvgUriReference, ISvgExternalResourcesRequired
    {
        /// <summary>
        /// Gets or sets a value corresponding to the attribute 'type' on the given 'script' element.
        /// </summary>
        /// <value>A string specifying the script type.</value>
        string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value corresponding to the attribute 'crossorigin' on the given 'script' element.
        /// </summary>
        /// <value>An enumeration specifying the CORS (Cross-Origin Resource Sharing) setting attributes. 
        /// Possible values are <c>anonymous</c>, <c>use-credentials</c> and empty string.</value>
        /// <remarks>This is introduced in <c>SVG 2</c>.</remarks>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/CORS_settings_attributes"/>
        string CrossOrigin
        {
            get;
            set;
        }
    }
}
