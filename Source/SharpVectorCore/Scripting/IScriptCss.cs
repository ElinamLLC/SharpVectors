using System;

using SharpVectors.Dom.Css;

namespace SharpVectors.Scripting
{     
    /// <summary>
    /// IJsCssRuleList
    /// </summary>
    public interface IJsCssRuleList : IScriptableObject<ICssRuleList>
    {
        ulong length { get; }

        IJsCssRule item(ulong index);
    }

    /// <summary>
    /// IJsCssRule
    /// </summary>
    public interface IJsCssRule : IScriptableObject<ICssRule>
    {
        ushort type { get; }
        string cssText { get; set; }
        IJsCssStyleSheet parentStyleSheet { get; }
        IJsCssRule parentRule { get; }
    }

    /// <summary>
    /// IJsCssStyleRule
    /// </summary>
    public interface IJsCssStyleRule : IJsCssRule
    {
        string selectorText { get; set; }
        IJsCssStyleDeclaration style { get; }
    }

    /// <summary>
    /// IJsCssMediaRule
    /// </summary>
    public interface IJsCssMediaRule : IJsCssRule
    {
        ulong insertRule(string rule, ulong index);
        void deleteRule(ulong index);
        IJsMediaList media { get; }
        IJsCssRuleList cssRules { get; }
    }

    /// <summary>
    /// IJsCssFontFaceRule
    /// </summary>
    public interface IJsCssFontFaceRule : IJsCssRule
    {
        IJsCssStyleDeclaration style { get; }
    }

    /// <summary>
    /// IJsCssPageRule
    /// </summary>
    public interface IJsCssPageRule : IJsCssRule
    {
        string selectorText { get; set; }
        IJsCssStyleDeclaration style { get; }
    }

    /// <summary>
    /// IJsCssImportRule
    /// </summary>
    public interface IJsCssImportRule : IJsCssRule
    {
        string href { get; }
        IJsMediaList media { get; }
        IJsCssStyleSheet styleSheet { get; }
    }

    /// <summary>
    /// IJsCssCharsetRule
    /// </summary>
    public interface IJsCssCharsetRule : IJsCssRule
    {
        string encoding { get; set; }
    }

    /// <summary>
    /// IJsCssUnknownRule
    /// </summary>
    public interface IJsCssUnknownRule : IJsCssRule
    {
    }

    /// <summary>
    /// IJsCssStyleDeclaration
    /// </summary>
    public interface IJsCssStyleDeclaration : IScriptableObject<ICssStyleDeclaration>
    {
        string cssText { get; set; }
        ulong length { get; }
        IJsCssRule parentRule { get; }

        string item(ulong index);
        string getPropertyValue(string propertyName);
        IJsCssValue getPropertyCSSValue(string propertyName);
        string removeProperty(string propertyName);
        string getPropertyPriority(string propertyName);
        void setProperty(string propertyName, string value, string priority);
    }

    /// <summary>
    /// IJsCssValue
    /// </summary>
    public interface IJsCssValue : IScriptableObject<ICssValue>
    {
        string cssText { get; set; }
        ushort cssValueType { get; }
    }

    /// <summary>
    /// IJsCssPrimitiveValue
    /// </summary>
    public interface IJsCssPrimitiveValue : IJsCssValue
    {
        ushort primitiveType { get; }

        void setFloatValue(ushort unitType, float floatValue);
        float getFloatValue(ushort unitType);
        void setStringValue(ushort stringType, string stringValue);
        string getStringValue();
        IJsCounter getCounterValue();
        IJsRect getRectValue();
        IJsRgbColor getRGBColorValue();
    }

    /// <summary>
    /// IJsCssValueList
    /// </summary>
    public interface IJsCssValueList : IJsCssValue
    {
        ulong length { get; }

        IJsCssValue item(ulong index);
    }

    /// <summary>
    /// IJsRgbColor
    /// </summary>
    public interface IJsRgbColor : IScriptableObject<ICssColor>
    {
        IJsCssPrimitiveValue red { get; }
        IJsCssPrimitiveValue green { get; }
        IJsCssPrimitiveValue blue { get; }
    }

    /// <summary>
    /// IJsRect
    /// </summary>
    public interface IJsRect : IScriptableObject<ICssRect>
    {
        IJsCssPrimitiveValue top { get; }
        IJsCssPrimitiveValue right { get; }
        IJsCssPrimitiveValue bottom { get; }
        IJsCssPrimitiveValue left { get; }
    }

    /// <summary>
    /// IJsCounter
    /// </summary>
    public interface IJsCounter : IScriptableObject<ICssCounter>
    {
        string identifier { get; }
        string listStyle { get; }
        string separator { get; }
    }

    /// <summary>
    /// IJsElementCssInlineStyle
    /// </summary>
    public interface IJsElementCssInlineStyle : IScriptableObject<IElementCssInlineStyle>
    {
        IJsCssStyleDeclaration style { get; }
    }

    ///// <summary>
    ///// IJsCss2Properties
    ///// </summary>
    //public interface IJsCss2Properties : IScriptableObject<ICss2Properties>
    //{
    //    string azimuth { get; set; }
    //    string background { get; set; }
    //    string backgroundAttachment { get; set; }
    //    string backgroundColor { get; set; }
    //    string backgroundImage { get; set; }
    //    string backgroundPosition { get; set; }
    //    string backgroundRepeat { get; set; }
    //    string border { get; set; }
    //    string borderCollapse { get; set; }
    //    string borderColor { get; set; }
    //    string borderSpacing { get; set; }
    //    string borderStyle { get; set; }
    //    string borderTop { get; set; }
    //    string borderRight { get; set; }
    //    string borderBottom { get; set; }
    //    string borderLeft { get; set; }
    //    string borderTopColor { get; set; }
    //    string borderRightColor { get; set; }
    //    string borderBottomColor { get; set; }
    //    string borderLeftColor { get; set; }
    //    string borderTopStyle { get; set; }
    //    string borderRightStyle { get; set; }
    //    string borderBottomStyle { get; set; }
    //    string borderLeftStyle { get; set; }
    //    string borderTopWidth { get; set; }
    //    string borderRightWidth { get; set; }
    //    string borderBottomWidth { get; set; }
    //    string borderLeftWidth { get; set; }
    //    string borderWidth { get; set; }
    //    string bottom { get; set; }
    //    string captionSide { get; set; }
    //    string clear { get; set; }
    //    string clip { get; set; }
    //    string color { get; set; }
    //    string content { get; set; }
    //    string counterIncrement { get; set; }
    //    string counterReset { get; set; }
    //    string cue { get; set; }
    //    string cueAfter { get; set; }
    //    string cueBefore { get; set; }
    //    string cursor { get; set; }
    //    string direction { get; set; }
    //    string display { get; set; }
    //    string elevation { get; set; }
    //    string emptyCells { get; set; }
    //    string cssFloat { get; set; }
    //    string font { get; set; }
    //    string fontFamily { get; set; }
    //    string fontSize { get; set; }
    //    string fontSizeAdjust { get; set; }
    //    string fontStretch { get; set; }
    //    string fontStyle { get; set; }
    //    string fontVariant { get; set; }
    //    string fontWeight { get; set; }
    //    string height { get; set; }
    //    string left { get; set; }
    //    string letterSpacing { get; set; }
    //    string lineHeight { get; set; }
    //    string listStyle { get; set; }
    //    string listStyleImage { get; set; }
    //    string listStylePosition { get; set; }
    //    string listStyleType { get; set; }
    //    string margin { get; set; }
    //    string marginTop { get; set; }
    //    string marginRight { get; set; }
    //    string marginBottom { get; set; }
    //    string marginLeft { get; set; }
    //    string markerOffset { get; set; }
    //    string marks { get; set; }
    //    string maxHeight { get; set; }
    //    string maxWidth { get; set; }
    //    string minHeight { get; set; }
    //    string minWidth { get; set; }
    //    string orphans { get; set; }
    //    string outline { get; set; }
    //    string outlineColor { get; set; }
    //    string outlineStyle { get; set; }
    //    string outlineWidth { get; set; }
    //    string overflow { get; set; }
    //    string padding { get; set; }
    //    string paddingTop { get; set; }
    //    string paddingRight { get; set; }
    //    string paddingBottom { get; set; }
    //    string paddingLeft { get; set; }
    //    string page { get; set; }
    //    string pageBreakAfter { get; set; }
    //    string pageBreakBefore { get; set; }
    //    string pageBreakInside { get; set; }
    //    string pause { get; set; }
    //    string pauseAfter { get; set; }
    //    string pauseBefore { get; set; }
    //    string pitch { get; set; }
    //    string pitchRange { get; set; }
    //    string playDuring { get; set; }
    //    string position { get; set; }
    //    string quotes { get; set; }
    //    string richness { get; set; }
    //    string right { get; set; }
    //    string size { get; set; }
    //    string speak { get; set; }
    //    string speakHeader { get; set; }
    //    string speakNumeral { get; set; }
    //    string speakPunctuation { get; set; }
    //    string speechRate { get; set; }
    //    string stress { get; set; }
    //    string tableLayout { get; set; }
    //    string textAlign { get; set; }
    //    string textDecoration { get; set; }
    //    string textIndent { get; set; }
    //    string textShadow { get; set; }
    //    string textTransform { get; set; }
    //    string top { get; set; }
    //    string unicodeBidi { get; set; }
    //    string verticalAlign { get; set; }
    //    string visibility { get; set; }
    //    string voiceFamily { get; set; }
    //    string volume { get; set; }
    //    string whiteSpace { get; set; }
    //    string widows { get; set; }
    //    string width { get; set; }
    //    string wordSpacing { get; set; }
    //    string zIndex { get; set; }
    //}

    /// <summary>
    /// IJsCssStyleSheet
    /// </summary>
    public interface IJsCssStyleSheet : IJsStyleSheet
    {
        IJsCssRule ownerRule { get; }
        IJsCssRuleList cssRules { get; }

        ulong insertRule(string rule, ulong index);
        void deleteRule(ulong index);
    }

    /// <summary>
    /// IJsViewCss
    /// </summary>
    public interface IJsViewCss : IJsAbstractView
    {
        IJsCssStyleDeclaration getComputedStyle(IJsElement elt, string pseudoElt);
    }

    /// <summary>
    /// IJsDocumentCss
    /// </summary>
    public interface IJsDocumentCss : IJsDocumentStyle
    {
        IJsCssStyleDeclaration getOverrideStyle(IJsElement elt, string pseudoElt);
    }

    /// <summary>
    /// IJsDomImplementationCss
    /// </summary>
    public interface IJsDomImplementationCss : IJsDomImplementation
    {
        IJsCssStyleSheet createCSSStyleSheet(string title, string media);
    }     
}
