namespace SharpVectors.Dom
{
    /// <summary>
    /// Visitor that visits all renderable elements
    /// </summary>
    public interface IElementVisitor
    {
        // Shape elements
        void Visit(Svg.ISvgCircleElement element);
        void Visit(Svg.ISvgEllipseElement element);
        void Visit(Svg.ISvgLineElement element);
        void Visit(Svg.ISvgPathElement element);
        void Visit(Svg.ISvgPolygonElement element);
        void Visit(Svg.ISvgPolylineElement element);
        void Visit(Svg.ISvgRectElement element);

        // Graphics referencing elements
        void Visit(Svg.ISvgImageElement element);
        void Visit(Svg.ISvgUseElement element);

        // Container elements
        void BeginContainer(Svg.ISvgElement element);
        void Visit(Svg.ISvgAElement element);
        void Visit(Svg.ISvgGElement element);
        void Visit(Svg.ISvgSvgElement element);
        void Visit(Svg.ISvgSwitchElement element);
        void Visit(Svg.ISvgSymbolElement element);
        void EndContainer(Svg.ISvgElement element);

        // Text content elements
        void Visit(Svg.ISvgTextElement element);
        void Visit(Svg.ISvgTextPathElement element);
        void Visit(Svg.ISvgTSpanElement element);
    }

    public interface IElementVisitorTarget
    {
        void Accept(IElementVisitor visitor);
    }
}
