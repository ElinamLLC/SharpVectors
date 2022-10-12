namespace SharpVectors.Dom
{
    /// <summary>
    /// controls how external resources will be handled.
    /// </summary>
    public enum AccessExternalResourcesMode
    {
        /// <summary>
        /// allows acces of external resources.
        /// </summary>
        Allow,

        /// <summary>
        /// ignore acces of external resources. 
        /// </summary>
        Ignore,

        /// <summary>
        /// throw error on acces of a external resource.
        /// </summary>
        ThrowError,
    }
}
