namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of type SvgLengthList which can be 
	/// animated.
	/// </summary>
	public interface ISvgAnimatedLengthList
	{
		/// <summary>
		/// The base value of the given attribute before applying any animations.
		/// </summary>
		ISvgLengthList BaseVal { get; }

		/// <summary>
		/// If the given attribute or property is being animated, contains the current animated value of the 
        /// attribute or property, and both the object itself and its contents are readonly. If the given 
        /// attribute or property is not currently being animated, contains the same value as 'BaseVal'.
		/// </summary>
		ISvgLengthList AnimVal { get; }

        // Extensions to the interface
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ISvgAnimatedLengthList"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ISvgAnimatedLengthList"/>.</value>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        ISvgAnimatedLength this[uint index] { get; }
    }
}
