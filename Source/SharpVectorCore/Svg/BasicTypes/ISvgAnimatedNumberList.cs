namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes which take a list of numbers and 
	/// which can be animated.
	/// </summary>
	public interface ISvgAnimatedNumberList
	{
		/// <summary>
		/// The base value of the given attribute before applying any animations
		/// </summary>
		ISvgNumberList BaseVal { get; }

		/// <summary>
		/// If the given attribute or property is being animated, then this attribute contains the current animated
        /// value of the attribute or property, and both the object itself and its contents are readonly. If the given 
        /// attribute or property is not currently being animated, then this attribute contains the same value as 'BaseVal'.
		/// </summary>
		ISvgNumberList AnimVal { get; }

        // Extensions to the interface
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ISvgAnimatedNumberList"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ISvgAnimatedNumberList"/>.</value>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        ISvgAnimatedNumber this[uint index] { get; }
    }
}