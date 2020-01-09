using System;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgStyleElement : ISvgElement, ISvgLangSpace
    {
        string Type { get; set; }
        string Media { get; set; }
        string Title { get; set; }
    }
}
