namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// A host target of the element visitor. Each renderable SVG element implements this interface.
    /// </summary>
    public interface ISvgElementVisitorTarget
    {
        /// <summary>
        /// This dispatches the rendering request to the accepted <see cref="ISvgElementVisitor"/> object.
        /// </summary>
        /// <param name="visitor">An implementation of the <see cref="ISvgElementVisitor"/> interface, to which the
        /// rendering operation is delegated.</param>
        void Accept(ISvgElementVisitor visitor);
    }
}
