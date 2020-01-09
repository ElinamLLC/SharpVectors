using System;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The <see cref="ISvgForeignObjectElement"/> interface provides access to the properties of <c>'foreignObject'</c> 
    /// elements, as well as methods to manipulate them.
    /// </summary>
    public interface ISvgForeignObjectElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable, IEventTarget
    {
        ISvgAnimatedLength X { get; }
        ISvgAnimatedLength Y { get; }
        ISvgAnimatedLength Width { get; }
        ISvgAnimatedLength Height { get; }
    }
}
