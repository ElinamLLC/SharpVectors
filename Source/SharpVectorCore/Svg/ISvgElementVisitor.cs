namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// A visitor pattern interface that visits all renderable SVG elements.
    /// </summary>
    /// <remarks>
    /// The implementation is defined are follows:
    /// <list type="bullet">
    /// <item>
    /// <term>Implements visitor (<see cref="ISvgElementVisitor"/>) object that implements an operation to be 
    /// performed on elements of SVG DOM structure.</term>
    /// </item>
    /// <item>
    /// <term>Implement the client that traverses the SVG DOM structure and call a dispatching operation 
    /// <see cref="ISvgElementVisitorTarget.Accept"/> on the SVG element — that dispatches (delegates) 
    /// the request to the accepted visitor object. 
    /// </term>
    /// </item>
    /// <item>
    /// <term>The visitor object then performs the operation on the element.</term>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Visitor_pattern">Wikipedia: Visitor pattern</seealso>
    public interface ISvgElementVisitor
    {
        /// <summary>Visits the circle shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgCircleElement"/> specifying the circle shape.</param>
        void Visit(ISvgCircleElement element);

        /// <summary>Visits the ellipse shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgEllipseElement"/> specifying the ellipse shape.</param>
        void Visit(ISvgEllipseElement element);

        /// <summary>Visits the line shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgLineElement"/> specifying the line shape.</param>
        void Visit(ISvgLineElement element);

        /// <summary>Visits the path shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgPathElement"/> specifying the path shape.</param>
        void Visit(ISvgPathElement element);

        /// <summary>Visits the polygon shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgPolygonElement"/> specifying the polygon shape.</param>
        void Visit(ISvgPolygonElement element);

        /// <summary>Visits the polyline shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgPolylineElement"/> specifying the polyline shape.</param>
        void Visit(ISvgPolylineElement element);

        /// <summary>Visits the rectangle shape element.</summary>
        /// <param name="element">An instance of <see cref="ISvgRectElement"/> specifying the rectangle shape.</param>
        void Visit(ISvgRectElement element);

        // Graphics referencing elements
        /// <summary>Visits the <c>image</c> graphics element.</summary>
        /// <param name="element">An instance of <see cref="ISvgImageElement"/> specifying the <c>image</c> element.</param>
        void Visit(ISvgImageElement element);

        /// <summary>Visits the use element.</summary>
        /// <param name="element">An instance of <see cref="ISvgUseElement"/> specifying the <c>use</c> element.</param>
        void Visit(ISvgUseElement element);

        /// <summary>Signal the beginning of a graphics container element.</summary>
        /// <param name="element">An instance of <see cref="ISvgElement"/> specifying the container element.</param>
        void BeginContainer(ISvgElement element);

        /// <summary>Visits the <c>link</c> element.</summary>
        /// <param name="element">An instance of <see cref="ISvgAElement"/> specifying the <c>link</c> element.</param>
        void Visit(ISvgAElement element);

        /// <summary>Visits the <c>group</c> element.</summary>
        /// <param name="element">An instance of <see cref="ISvgGElement"/> specifying the <c>group</c> element.</param>
        void Visit(ISvgGElement element);

        /// <summary>Visits the <c>svg</c> clipart element.</summary>
        /// <param name="element">An instance of <see cref="ISvgSvgElement"/> specifying the <c>svg</c> clipart element.</param>
        void Visit(ISvgSvgElement element);

        /// <summary>Visits the <c>switch</c> element.</summary>
        /// <param name="element">An instance of <see cref="ISvgSwitchElement"/> specifying the <c>switch</c> element.</param>
        void Visit(ISvgSwitchElement element);

        /// <summary>Visits the <c>symbol</c> element.</summary>
        /// <param name="element">An instance of <see cref="ISvgRectElement"/> specifying the <c>symbol</c> element.</param>
        void Visit(ISvgSymbolElement element);

        /// <summary>Signal the ending of a graphics container element.</summary>
        /// <param name="element">An instance of <see cref="ISvgElement"/> specifying the container element.</param>
        void EndContainer(ISvgElement element);

        /// <summary>Visits the <c>text</c> content element.</summary>
        /// <param name="element">An instance of <see cref="ISvgTextElement"/> specifying the <c>text</c> content element.</param>
        void Visit(ISvgTextElement element);

        /// <summary>Visits the <c>text-path</c> content element.</summary>
        /// <param name="element">An instance of <see cref="ISvgTextPathElement"/> specifying the <c>text-path</c> element.</param>
        void Visit(ISvgTextPathElement element);

        /// <summary>Visits the <c>text-span</c> content element.</summary>
        /// <param name="element">An instance of <see cref="ISvgTSpanElement"/> specifying the <c>text-span</c> content element.</param>
        void Visit(ISvgTSpanElement element);
    }
}
