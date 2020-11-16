namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// Hit-test options for text elements.
    /// </summary>
    public enum WpfHitTextLevel
    {
        /// <summary>
        /// Test only the bounds of the rendered text.
        /// </summary>
        Bounds = 0,
        /// <summary>
        /// Test the bounds and glyph geometris in the rendered text.
        /// </summary>
        Glyphs = 1
    }
}
