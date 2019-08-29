using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgTextContentElement interface is inherited by various text-related interfaces, such as 
    /// <see cref="ISvgTextElement"/>, <see cref="ISvgTSpanElement"/>, <see cref="ISvgTRefElement"/>, 
    /// <see cref="ISvgAltGlyphElement"/> and <see cref="ISvgTextPathElement"/>. 
    /// </summary>
    public interface ISvgTextContentElement : ISvgElement, ISvgTests, ISvgLangSpace,
                ISvgExternalResourcesRequired, ISvgStylable, IEventTarget
    {
        ISvgAnimatedLength TextLength { get; }
        ISvgAnimatedEnumeration LengthAdjust { get; }

        long GetNumberOfChars();
        float GetComputedTextLength();
        float GetSubStringLength(long charnum, long nchars);
        ISvgPoint GetStartPositionOfChar(long charnum);
        ISvgPoint GetEndPositionOfChar(long charnum);
        ISvgRect GetExtentOfChar(long charnum);
        float GetRotationOfChar(long charnum);
        long GetCharNumAtPosition(ISvgPoint point);
        void SelectSubString(long charnum, long nchars);
    }
}
