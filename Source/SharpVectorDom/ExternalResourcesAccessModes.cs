namespace SharpVectors.Dom
{
    /// <summary>
    /// This provides possible options to controls how external resources will be handled.
    /// </summary>
    public enum ExternalResourcesAccessModes
    {
        /// <summary>
        /// An option that allows access of external resources.
        /// </summary>
        Allow,

        /// <summary>
        /// An option that ignores access of external resources. 
        /// </summary>
        Ignore,

        /// <summary>
        /// An option that throws error on access of a external resource.
        /// </summary>
        ThrowError
    }
}
