namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Used for attributes of basic type <c>integer</c> which can be animated. 
    /// </summary>
    public interface ISvgAnimatedInteger
    {
        /// <summary>
        /// The base value of the given attribute before applying any animations. 
        /// </summary>
        long BaseVal { get; set; }

        /// <summary>
        /// If the given attribute or property is being animated, contains the current animated value of the 
        /// attribute or property. If the given attribute or property is not currently being animated, contains 
        /// the same value as <see cref="BaseVal"/>. 
        /// </summary>
        long AnimVal { get; }
    }
}

