using System;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Scripting
{        
    /// <summary>
    /// IJsSvgElement
    /// </summary>
    public interface IJsSvgElement : IJsElement
    {
        string id { get; set; }
        string xmlbase { get; set; }
        IJsSvgSvgElement ownerSVGElement { get; }
        IJsSvgElement viewportElement { get; }
    }

    /// <summary>
    /// IJsSvgAnimatedBoolean
    /// </summary>
    public interface IJsSvgAnimatedBoolean : IScriptableObject<ISvgAnimatedBoolean>
    {
        bool baseVal { get; set; }
        bool animVal { get; }
    }

    /// <summary>
    /// IJsSvgAnimatedString
    /// </summary>
    public interface IJsSvgAnimatedString : IScriptableObject<ISvgAnimatedString>
    {
        string baseVal { get; set; }
        string animVal { get; }
    }

    /// <summary>
    /// IJsSvgStringList
    /// </summary>
    public interface IJsSvgStringList : IScriptableObject<ISvgStringList>
    {
        ulong numberOfItems { get; }

        void clear();
        string initialize(string newItem);
        string getItem(ulong index);
        string insertItemBefore(string newItem, ulong index);
        string replaceItem(string newItem, ulong index);
        string removeItem(ulong index);
        string appendItem(string newItem);
    }

    /// <summary>
    /// IJsSvgAnimatedEnumeration
    /// </summary>
    public interface IJsSvgAnimatedEnumeration : IScriptableObject<ISvgAnimatedEnumeration>
    {
        ushort baseVal { get; set; }
        ushort animVal { get; }
    }

    /// <summary>
    /// IJsSvgAnimatedInteger
    /// </summary>
    public interface IJsSvgAnimatedInteger : IScriptableObject<ISvgAnimatedInteger>
    {
        long baseVal { get; set; }
        long animVal { get; }
    }

    /// <summary>
    /// IJsSvgNumber
    /// </summary>
    public interface IJsSvgNumber : IScriptableObject<ISvgNumber>
    {
        float value { get; set; }
    }

    /// <summary>
    /// IJsSvgAnimatedNumber
    /// </summary>
    public interface IJsSvgAnimatedNumber : IScriptableObject<ISvgAnimatedNumber>
    {
        float baseVal { get; set; }
        float animVal { get; }
    }

    /// <summary>
    /// IJsSvgNumberList
    /// </summary>
    public interface IJsSvgNumberList : IScriptableObject<ISvgNumberList>
    {
        ulong numberOfItems { get; }

        void clear();
        IJsSvgNumber initialize(IJsSvgNumber newItem);
        IJsSvgNumber getItem(ulong index);
        IJsSvgNumber insertItemBefore(IJsSvgNumber newItem, ulong index);
        IJsSvgNumber replaceItem(IJsSvgNumber newItem, ulong index);
        IJsSvgNumber removeItem(ulong index);
        IJsSvgNumber appendItem(IJsSvgNumber newItem);
    }

    /// <summary>
    /// IJsSvgAnimatedNumberList
    /// </summary>
    public interface IJsSvgAnimatedNumberList : IScriptableObject<ISvgAnimatedNumberList>
    {
        IJsSvgNumberList baseVal { get; }
        IJsSvgNumberList animVal { get; }
    }

    /// <summary>
    /// IJsSvgLength
    /// </summary>
    public interface IJsSvgLength : IScriptableObject<ISvgLength>
    {
        ushort unitType { get; }
        float value { get; set; }
        float valueInSpecifiedUnits { get; set; }
        string valueAsString { get; set; }

        void newValueSpecifiedUnits(ushort unitType, float valueInSpecifiedUnits);
        void convertToSpecifiedUnits(ushort unitType);
    }

    /// <summary>
    /// IJsSvgAnimatedLength
    /// </summary>
    public interface IJsSvgAnimatedLength : IScriptableObject<ISvgAnimatedLength>
    {
        IJsSvgLength baseVal { get; }
        IJsSvgLength animVal { get; }
    }

    /// <summary>
    /// IJsSvgLengthList
    /// </summary>
    public interface IJsSvgLengthList : IScriptableObject<ISvgLengthList>
    {
        ulong numberOfItems { get; }

        void clear();
        IJsSvgLength initialize(IJsSvgLength newItem);
        IJsSvgLength getItem(ulong index);
        IJsSvgLength insertItemBefore(IJsSvgLength newItem, ulong index);
        IJsSvgLength replaceItem(IJsSvgLength newItem, ulong index);
        IJsSvgLength removeItem(ulong index);
        IJsSvgLength appendItem(IJsSvgLength newItem);
    }

    /// <summary>
    /// IJsSvgAnimatedLengthList
    /// </summary>
    public interface IJsSvgAnimatedLengthList : IScriptableObject<ISvgAnimatedLengthList>
    {
        IJsSvgLengthList baseVal { get; }
        IJsSvgLengthList animVal { get; }
    }

    /// <summary>
    /// IJsSvgAngle
    /// </summary>
    public interface IJsSvgAngle : IScriptableObject<ISvgAngle>
    {
        ushort unitType { get; }
        float value { get; set; }
        float valueInSpecifiedUnits { get; set; }
        string valueAsString { get; set; }

        void newValueSpecifiedUnits(ushort unitType, float valueInSpecifiedUnits);
        void convertToSpecifiedUnits(ushort unitType);
    }

    /// <summary>
    /// IJsSvgAnimatedAngle
    /// </summary>
    public interface IJsSvgAnimatedAngle : IScriptableObject<ISvgAnimatedAngle>
    {
        IJsSvgAngle baseVal { get; }
        IJsSvgAngle animVal { get; }
    }

    /// <summary>
    /// IJsSvgColor
    /// </summary>
    public interface IJsSvgColor : IJsCssValue
    {
        ushort colorType { get; }
        IJsRgbColor rgbColor { get; }
        IJsSvgIccColor iccColor { get; }

        void setRGBColor(string rgbColor);
        void setRGBColorICCColor(string rgbColor, string iccColor);
        void setColor(ushort colorType, string rgbColor, string iccColor);
    }

    /// <summary>
    /// IJsSvgIccColor
    /// </summary>
    public interface IJsSvgIccColor : IScriptableObject<ISvgIccColor>
    {
        string colorProfile { get; set; }
        IJsSvgNumberList colors { get; }
    }

    /// <summary>
    /// IJsSvgRect
    /// </summary>
    public interface IJsSvgRect : IScriptableObject<ISvgRect>
    {
        float x { get; set; }
        float y { get; set; }
        float width { get; set; }
        float height { get; set; }
    }

    /// <summary>
    /// IJsSvgAnimatedRect
    /// </summary>
    public interface IJsSvgAnimatedRect : IScriptableObject<ISvgAnimatedRect>
    {
        IJsSvgRect baseVal { get; }
        IJsSvgRect animVal { get; }
    }

    /// <summary>
    /// IJsSvgUnitTypes
    /// </summary>
    public interface IJsSvgUnitTypes : IScriptableObject<ISvgUnitTypes>
    {
    }

    /// <summary>
    /// IJsSvgStylable
    /// </summary>
    public interface IJsSvgStylable : IScriptableObject<ISvgStylable>
    {
        IJsSvgAnimatedString className { get; }
        IJsCssStyleDeclaration style { get; }

        IJsCssValue getPresentationAttribute(string name);
    }

    /// <summary>
    /// IJsSvgLocatable
    /// </summary>
    public interface IJsSvgLocatable : IScriptableObject<ISvgLocatable>
    {
        IJsSvgElement nearestViewportElement { get; }
        IJsSvgElement farthestViewportElement { get; }

        IJsSvgRect getBBox();
        IJsSvgMatrix getCTM();
        IJsSvgMatrix getScreenCTM();
        IJsSvgMatrix getTransformToElement(IJsSvgElement element);
    }

    /// <summary>
    /// IJsSvgTransformable
    /// </summary>
    public interface IJsSvgTransformable : IJsSvgLocatable
    {
        IJsSvgAnimatedTransformList transform { get; }
    }

    /// <summary>
    /// IJsSvgTests
    /// </summary>
    public interface IJsSvgTests : IScriptableObject<ISvgTests>
    {
        bool hasExtension(string extension);
        IJsSvgStringList requiredFeatures { get; }
        IJsSvgStringList requiredExtensions { get; }
        IJsSvgStringList systemLanguage { get; }
    }

    /// <summary>
    /// IJsSvgLangSpace
    /// </summary>
    public interface IJsSvgLangSpace : IScriptableObject<ISvgLangSpace>
    {
        string xmllang { get; set; }
        string xmlspace { get; set; }
    }

    /// <summary>
    /// IJsSvgExternalResourcesRequired
    /// </summary>
    public interface IJsSvgExternalResourcesRequired : IScriptableObject<ISvgExternalResourcesRequired>
    {
        bool externalResourcesRequired { get; }
    }

    /// <summary>
    /// IJsSvgFitToViewBox
    /// </summary>
    public interface IJsSvgFitToViewBox : IScriptableObject<ISvgFitToViewBox>
    {
        IJsSvgAnimatedRect viewBox { get; }
        IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio { get; }
    }

    /// <summary>
    /// IJsSvgZoomAndPan
    /// </summary>
    public interface IJsSvgZoomAndPan : IScriptableObject<ISvgZoomAndPan>
    {
        ushort zoomAndPan { get; set; }
    }

    /// <summary>
    /// IJsSvgViewSpec
    /// </summary>
    public interface IJsSvgViewSpec : IScriptableObject<ISvgFitToViewBox>, IJsSvgFitToViewBox, IJsSvgZoomAndPan
    {
        IJsSvgTransformList transform { get; }
        IJsSvgElement viewTarget { get; }
        string viewBoxString { get; }
        string preserveAspectRatioString { get; }
        string transformString { get; }
        string viewTargetString { get; }
    }

    /// <summary>
    /// IJsSvgUriReference
    /// </summary>
    public interface IJsSvgUriReference : IScriptableObject<ISvgUriReference>
    {
        IJsSvgAnimatedString href { get; }
    }

    /// <summary>
    /// IJsSvgCssRule
    /// </summary>
    public interface IJsSvgCssRule : IJsCssRule
    {
    }

    /// <summary>
    /// IJsSvgRenderingIntent
    /// </summary>
    public interface IJsSvgRenderingIntent : IScriptableObject<ISvgRenderingIntent>
    {
    }

    /// <summary>
    /// IJsSvgDocument
    /// </summary>
    public interface IJsSvgDocument : IJsDocument, IJsDocumentEvent
    {
        string title { get; }
        string referrer { get; }
        string domain { get; }
        string URL { get; }
        IJsSvgSvgElement rootElement { get; }
    }

    /// <summary>
    /// IJsSvgSvgElement
    /// </summary>
    public interface IJsSvgSvgElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgLocatable, IJsSvgFitToViewBox, IJsSvgZoomAndPan, 
        IJsEventTarget, IJsDocumentEvent, IJsViewCss, IJsDocumentCss
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        string contentScriptType { get; set; }
        string contentStyleType { get; set; }
        IJsSvgRect viewport { get; }
        float pixelUnitToMillimeterX { get; }
        float pixelUnitToMillimeterY { get; }
        float screenPixelToMillimeterX { get; }
        float screenPixelToMillimeterY { get; }
        bool useCurrentView { get; set; }
        IJsSvgViewSpec currentView { get; }
        float currentScale { get; set; }
        IJsSvgPoint currentTranslate { get; }

        ulong suspendRedraw(ulong max_wait_milliseconds);
        void unsuspendRedraw(ulong suspend_handle_id);
        void unsuspendRedrawAll();
        void forceRedraw();
        void pauseAnimations();
        void unpauseAnimations();
        bool animationsPaused();
        float getCurrentTime();
        void setCurrentTime(float seconds);
        IJsNodeList getIntersectionList(IJsSvgRect rect, IJsSvgElement referenceElement);
        IJsNodeList getEnclosureList(IJsSvgRect rect, IJsSvgElement referenceElement);
        bool checkIntersection(IJsSvgElement element, IJsSvgRect rect);
        bool checkEnclosure(IJsSvgElement element, IJsSvgRect rect);
        void deselectAll();
        IJsSvgNumber createSVGNumber();
        IJsSvgLength createSVGLength();
        IJsSvgAngle createSVGAngle();
        IJsSvgPoint createSVGPoint();
        IJsSvgMatrix createSVGMatrix();
        IJsSvgRect createSVGRect();
        IJsSvgTransform createSVGTransform();
        IJsSvgTransform createSVGTransformFromMatrix(IJsSvgMatrix matrix);
        IJsElement getElementById(string elementId);
    }

    /// <summary>
    /// IJsSvgGElement
    /// </summary>
    public interface IJsSvgGElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgTransformable, IJsEventTarget
    {
    }

    /// <summary>
    /// IJsSvgDefsElement
    /// </summary>
    public interface IJsSvgDefsElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgTransformable, IJsEventTarget
    {
    }

    /// <summary>
    /// IJsSvgDescElement
    /// </summary>
    public interface IJsSvgDescElement : IJsSvgElement, IJsSvgLangSpace, IJsSvgStylable
    {
    }

    /// <summary>
    /// IJsSvgTitleElement
    /// </summary>
    public interface IJsSvgTitleElement : IJsSvgElement, IJsSvgLangSpace, IJsSvgStylable
    {
    }

    /// <summary>
    /// IJsSvgSymbolElement
    /// </summary>
    public interface IJsSvgSymbolElement : IJsSvgElement, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, 
        IJsSvgStylable, IJsSvgFitToViewBox, IJsEventTarget
    {
    }

    /// <summary>
    /// IJsSvgUseElement
    /// </summary>
    public interface IJsSvgUseElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgTests, IJsSvgLangSpace, IJsSvgExternalResourcesRequired, 
        IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        IJsSvgElementInstance instanceRoot { get; }
        IJsSvgElementInstance animatedInstanceRoot { get; }
    }

    /// <summary>
    /// IJsSvgElementInstance
    /// </summary>
    public interface IJsSvgElementInstance : IJsEventTarget
    {
        IJsSvgElement correspondingElement { get; }
        IJsSvgUseElement correspondingUseElement { get; }
        IJsSvgElementInstance parentNode { get; }
        IJsSvgElementInstanceList childNodes { get; }
        IJsSvgElementInstance firstChild { get; }
        IJsSvgElementInstance lastChild { get; }
        IJsSvgElementInstance previousSibling { get; }
        IJsSvgElementInstance nextSibling { get; }
    }

    /// <summary>
    /// IJsSvgElementInstanceList
    /// </summary>
    public interface IJsSvgElementInstanceList : IScriptableObject<ISvgElementInstanceList>
    {
        IJsSvgElementInstance item(ulong index);
        ulong length { get; }
    }

    /// <summary>
    /// IJsSvgImageElement
    /// </summary>
    public interface IJsSvgImageElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgTests, IJsSvgLangSpace, IJsSvgExternalResourcesRequired, 
        IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio { get; }
    }

    /// <summary>
    /// IJsSvgSwitchElement
    /// </summary>
    public interface IJsSvgSwitchElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgTransformable, IJsEventTarget
    {
    }

    /// <summary>
    /// IJsGetSvgDocument
    /// </summary>
    public interface IJsGetSvgDocument : IScriptableObject<IGetSvgDocument>
    {
        IJsSvgDocument getSVGDocument();
    }

    /// <summary>
    /// IJsSvgStyleElement
    /// </summary>
    public interface IJsSvgStyleElement : IJsSvgElement, IJsSvgLangSpace
    {
        string type { get; set; }
        string media { get; set; }
        string title { get; set; }
    }

    /// <summary>
    /// IJsSvgPoint
    /// </summary>
    public interface IJsSvgPoint : IScriptableObject<ISvgPoint>
    {
        IJsSvgPoint matrixTransform(IJsSvgMatrix matrix);
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPointList
    /// </summary>
    public interface IJsSvgPointList : IScriptableObject<ISvgPointList>
    {
        ulong numberOfItems { get; }

        void clear();
        IJsSvgPoint initialize(IJsSvgPoint newItem);
        IJsSvgPoint getItem(ulong index);
        IJsSvgPoint insertItemBefore(IJsSvgPoint newItem, ulong index);
        IJsSvgPoint replaceItem(IJsSvgPoint newItem, ulong index);
        IJsSvgPoint removeItem(ulong index);
        IJsSvgPoint appendItem(IJsSvgPoint newItem);
    }

    /// <summary>
    /// IJsSvgMatrix
    /// </summary>
    public interface IJsSvgMatrix : IScriptableObject<ISvgMatrix>
    {
        float a { get; set; }
        float b { get; set; }
        float c { get; set; }
        float d { get; set; }
        float e { get; set; }
        float f { get; set; }

        IJsSvgMatrix multiply(IJsSvgMatrix secondMatrix);
        IJsSvgMatrix inverse();
        IJsSvgMatrix translate(float x, float y);
        IJsSvgMatrix scale(float scaleFactor);
        IJsSvgMatrix scaleNonUniform(float scaleFactorX, float scaleFactorY);
        IJsSvgMatrix rotate(float angle);
        IJsSvgMatrix rotateFromVector(float x, float y);
        IJsSvgMatrix flipX();
        IJsSvgMatrix flipY();
        IJsSvgMatrix skewX(float angle);
        IJsSvgMatrix skewY(float angle);
    }

    /// <summary>
    /// IJsSvgTransform
    /// </summary>
    public interface IJsSvgTransform : IScriptableObject<ISvgTransform>
    {
        ushort type { get; }
        IJsSvgMatrix matrix { get; }
        float angle { get; }

        void setMatrix(IJsSvgMatrix matrix);
        void setTranslate(float tx, float ty);
        void setScale(float sx, float sy);
        void setRotate(float angle, float cx, float cy);
        void setSkewX(float angle);
        void setSkewY(float angle);
    }

    /// <summary>
    /// IJsSvgTransformList
    /// </summary>
    public interface IJsSvgTransformList : IScriptableObject<ISvgTransformList>
    {
        ulong numberOfItems { get; }

        void clear();
        IJsSvgTransform initialize(IJsSvgTransform newItem);
        IJsSvgTransform getItem(ulong index);
        IJsSvgTransform insertItemBefore(IJsSvgTransform newItem, ulong index);
        IJsSvgTransform replaceItem(IJsSvgTransform newItem, ulong index);
        IJsSvgTransform removeItem(ulong index);
        IJsSvgTransform appendItem(IJsSvgTransform newItem);
        IJsSvgTransform createSVGTransformFromMatrix(IJsSvgMatrix matrix);
        IJsSvgTransform consolidate();
    }

    /// <summary>
    /// IJsSvgAnimatedTransformList
    /// </summary>
    public interface IJsSvgAnimatedTransformList : IScriptableObject<ISvgAnimatedTransformList>
    {
        IJsSvgTransformList baseVal { get; }
        IJsSvgTransformList animVal { get; }
    }

    /// <summary>
    /// IJsSvgPreserveAspectRatio
    /// </summary>
    public interface IJsSvgPreserveAspectRatio : IScriptableObject<ISvgPreserveAspectRatio>
    {
        ushort align { get; set; }
        ushort meetOrSlice { get; set; }
    }

    /// <summary>
    /// IJsSvgAnimatedPreserveAspectRatio
    /// </summary>
    public interface IJsSvgAnimatedPreserveAspectRatio : IScriptableObject<ISvgAnimatedPreserveAspectRatio>
    {
        IJsSvgPreserveAspectRatio baseVal { get; }
        IJsSvgPreserveAspectRatio animVal { get; }
    }

    /// <summary>
    /// IJsSvgPathSeg
    /// </summary>
    public interface IJsSvgPathSeg : IScriptableObject
    {
        ushort pathSegType { get; }
        string pathSegTypeAsLetter { get; }

        object BasePathSeg { get; }
    }

    /// <summary>
    /// IJsSvgPathSeg
    /// </summary>
    public interface IJsSvgPathSeg<T> : IScriptableObject<T>, IJsSvgPathSeg
        where T : class
    {
    }

    /// <summary>
    /// IJsSvgPathSegClosePath
    /// </summary>
    public interface IJsSvgPathSegClosePath : IJsSvgPathSeg<ISvgPathSegClosePath>
    {
    }

    /// <summary>
    /// IJsSvgPathSegMovetoAbs
    /// </summary>
    public interface IJsSvgPathSegMovetoAbs : IJsSvgPathSeg<ISvgPathSegMovetoAbs>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegMovetoRel
    /// </summary>
    public interface IJsSvgPathSegMovetoRel : IJsSvgPathSeg<ISvgPathSegMovetoRel>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoAbs
    /// </summary>
    public interface IJsSvgPathSegLinetoAbs : IJsSvgPathSeg<ISvgPathSegLinetoAbs>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoRel
    /// </summary>
    public interface IJsSvgPathSegLinetoRel : IJsSvgPathSeg<ISvgPathSegLinetoRel>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoCubicAbs
    /// </summary>
    public interface IJsSvgPathSegCurvetoCubicAbs : IJsSvgPathSeg<ISvgPathSegCurvetoCubicAbs>
    {
        float x { get; set; }
        float y { get; set; }
        float x1 { get; set; }
        float y1 { get; set; }
        float x2 { get; set; }
        float y2 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoCubicRel
    /// </summary>
    public interface IJsSvgPathSegCurvetoCubicRel : IJsSvgPathSeg<ISvgPathSegCurvetoCubicRel>
    {
        float x { get; set; }
        float y { get; set; }
        float x1 { get; set; }
        float y1 { get; set; }
        float x2 { get; set; }
        float y2 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoQuadraticAbs
    /// </summary>
    public interface IJsSvgPathSegCurvetoQuadraticAbs : IJsSvgPathSeg<ISvgPathSegCurvetoQuadraticAbs>
    {
        float x { get; set; }
        float y { get; set; }
        float x1 { get; set; }
        float y1 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoQuadraticRel
    /// </summary>
    public interface IJsSvgPathSegCurvetoQuadraticRel : IJsSvgPathSeg<ISvgPathSegCurvetoQuadraticRel>
    {
        float x { get; set; }
        float y { get; set; }
        float x1 { get; set; }
        float y1 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegArcAbs
    /// </summary>
    public interface IJsSvgPathSegArcAbs : IJsSvgPathSeg<ISvgPathSegArcAbs>
    {
        float x { get; set; }
        float y { get; set; }
        float r1 { get; set; }
        float r2 { get; set; }
        float angle { get; set; }
        bool largeArcFlag { get; set; }
        bool sweepFlag { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegArcRel
    /// </summary>
    public interface IJsSvgPathSegArcRel : IJsSvgPathSeg<ISvgPathSegArcRel>
    {
        float x { get; set; }
        float y { get; set; }
        float r1 { get; set; }
        float r2 { get; set; }
        float angle { get; set; }
        bool largeArcFlag { get; set; }
        bool sweepFlag { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoHorizontalAbs
    /// </summary>
    public interface IJsSvgPathSegLinetoHorizontalAbs : IJsSvgPathSeg<ISvgPathSegLinetoHorizontalAbs>
    {
        float x { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoHorizontalRel
    /// </summary>
    public interface IJsSvgPathSegLinetoHorizontalRel : IJsSvgPathSeg<ISvgPathSegLinetoHorizontalRel>
    {
        float x { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoVerticalAbs
    /// </summary>
    public interface IJsSvgPathSegLinetoVerticalAbs : IJsSvgPathSeg<ISvgPathSegLinetoVerticalAbs>
    {
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegLinetoVerticalRel
    /// </summary>
    public interface IJsSvgPathSegLinetoVerticalRel : IJsSvgPathSeg<ISvgPathSegLinetoVerticalRel>
    {
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoCubicSmoothAbs
    /// </summary>
    public interface IJsSvgPathSegCurvetoCubicSmoothAbs : IJsSvgPathSeg<ISvgPathSegCurvetoCubicSmoothAbs>
    {
        float x { get; set; }
        float y { get; set; }
        float x2 { get; set; }
        float y2 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoCubicSmoothRel
    /// </summary>
    public interface IJsSvgPathSegCurvetoCubicSmoothRel : IJsSvgPathSeg<ISvgPathSegCurvetoCubicSmoothRel>
    {
        float x { get; set; }
        float y { get; set; }
        float x2 { get; set; }
        float y2 { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoQuadraticSmoothAbs
    /// </summary>
    public interface IJsSvgPathSegCurvetoQuadraticSmoothAbs : IJsSvgPathSeg<ISvgPathSegCurvetoQuadraticSmoothAbs>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegCurvetoQuadraticSmoothRel
    /// </summary>
    public interface IJsSvgPathSegCurvetoQuadraticSmoothRel : IJsSvgPathSeg<ISvgPathSegCurvetoQuadraticSmoothRel>
    {
        float x { get; set; }
        float y { get; set; }
    }

    /// <summary>
    /// IJsSvgPathSegList
    /// </summary>
    public interface IJsSvgPathSegList : IScriptableObject<ISvgPathSegList>
    {
        ulong numberOfItems { get; }

        void clear();
        IJsSvgPathSeg initialize(IJsSvgPathSeg newItem);
        IJsSvgPathSeg getItem(ulong index);
        IJsSvgPathSeg insertItemBefore(IJsSvgPathSeg newItem, ulong index);
        IJsSvgPathSeg replaceItem(IJsSvgPathSeg newItem, ulong index);
        IJsSvgPathSeg removeItem(ulong index);
        IJsSvgPathSeg appendItem(IJsSvgPathSeg newItem);
    }

    /// <summary>
    /// IJsSvgAnimatedPathData
    /// </summary>
    public interface IJsSvgAnimatedPathData : IScriptableObject<ISvgAnimatedPathData>
    {
        IJsSvgPathSegList pathSegList { get; }
        IJsSvgPathSegList normalizedPathSegList { get; }
        IJsSvgPathSegList animatedPathSegList { get; }
        IJsSvgPathSegList animatedNormalizedPathSegList { get; }
    }

    /// <summary>
    /// IJsSvgPathElement
    /// </summary>
    public interface IJsSvgPathElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, IJsSvgStylable, 
        IJsSvgExternalResourcesRequired, IJsSvgTransformable, IJsEventTarget, IJsSvgAnimatedPathData
    {
        IJsSvgAnimatedNumber pathLength { get; }

        float getTotalLength();
        IJsSvgPoint getPointAtLength(float distance);
        ulong getPathSegAtLength(float distance);
        IJsSvgPathSegClosePath createSVGPathSegClosePath();
        IJsSvgPathSegMovetoAbs createSVGPathSegMovetoAbs(float x, float y);
        IJsSvgPathSegMovetoRel createSVGPathSegMovetoRel(float x, float y);
        IJsSvgPathSegLinetoAbs createSVGPathSegLinetoAbs(float x, float y);
        IJsSvgPathSegLinetoRel createSVGPathSegLinetoRel(float x, float y);
        IJsSvgPathSegCurvetoCubicAbs createSVGPathSegCurvetoCubicAbs(float x, float y, float x1, float y1, float x2, float y2);
        IJsSvgPathSegCurvetoCubicRel createSVGPathSegCurvetoCubicRel(float x, float y, float x1, float y1, float x2, float y2);
        IJsSvgPathSegCurvetoQuadraticAbs createSVGPathSegCurvetoQuadraticAbs(float x, float y, float x1, float y1);
        IJsSvgPathSegCurvetoQuadraticRel createSVGPathSegCurvetoQuadraticRel(float x, float y, float x1, float y1);
        IJsSvgPathSegArcAbs createSVGPathSegArcAbs(float x, float y, float r1, float r2, 
            float angle, bool largeArcFlag, bool sweepFlag);
        IJsSvgPathSegArcRel createSVGPathSegArcRel(float x, float y, float r1, float r2, 
            float angle, bool largeArcFlag, bool sweepFlag);
        IJsSvgPathSegLinetoHorizontalAbs createSVGPathSegLinetoHorizontalAbs(float x);
        IJsSvgPathSegLinetoHorizontalRel createSVGPathSegLinetoHorizontalRel(float x);
        IJsSvgPathSegLinetoVerticalAbs createSVGPathSegLinetoVerticalAbs(float y);
        IJsSvgPathSegLinetoVerticalRel createSVGPathSegLinetoVerticalRel(float y);
        IJsSvgPathSegCurvetoCubicSmoothAbs createSVGPathSegCurvetoCubicSmoothAbs(float x, float y, float x2, float y2);
        IJsSvgPathSegCurvetoCubicSmoothRel createSVGPathSegCurvetoCubicSmoothRel(float x, float y, float x2, float y2);
        IJsSvgPathSegCurvetoQuadraticSmoothAbs createSVGPathSegCurvetoQuadraticSmoothAbs(float x, float y);
        IJsSvgPathSegCurvetoQuadraticSmoothRel createSVGPathSegCurvetoQuadraticSmoothRel(float x, float y);
    }

    /// <summary>
    /// IJsSvgRectElement
    /// </summary>
    public interface IJsSvgRectElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        IJsSvgAnimatedLength rx { get; }
        IJsSvgAnimatedLength ry { get; }
    }

    /// <summary>
    /// IJsSvgCircleElement
    /// </summary>
    public interface IJsSvgCircleElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength cx { get; }
        IJsSvgAnimatedLength cy { get; }
        IJsSvgAnimatedLength r { get; }
    }

    /// <summary>
    /// IJsSvgEllipseElement
    /// </summary>
    public interface IJsSvgEllipseElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength cx { get; }
        IJsSvgAnimatedLength cy { get; }
        IJsSvgAnimatedLength rx { get; }
        IJsSvgAnimatedLength ry { get; }
    }

    /// <summary>
    /// IJsSvgLineElement
    /// </summary>
    public interface IJsSvgLineElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength x1 { get; }
        IJsSvgAnimatedLength y1 { get; }
        IJsSvgAnimatedLength x2 { get; }
        IJsSvgAnimatedLength y2 { get; }
    }

    /// <summary>
    /// IJsSvgAnimatedPoints
    /// </summary>
    public interface IJsSvgAnimatedPoints : IScriptableObject<ISvgAnimatedPoints>
    {
        IJsSvgPointList points { get; }
        IJsSvgPointList animatedPoints { get; }
    }

    /// <summary>
    /// IJsSvgPolylineElement
    /// </summary>
    public interface IJsSvgPolylineElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, IJsSvgStylable, 
        IJsSvgExternalResourcesRequired, IJsSvgTransformable, IJsEventTarget, IJsSvgAnimatedPoints
    {
    }

    /// <summary>
    /// IJsSvgPolygonElement
    /// </summary>
    public interface IJsSvgPolygonElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgTransformable, IJsEventTarget, IJsSvgAnimatedPoints
    {
    }

    /// <summary>
    /// IJsSvgTextContentElement
    /// </summary>
    public interface IJsSvgTextContentElement : IJsSvgElement, IJsSvgTests, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsEventTarget
    {
        IJsSvgAnimatedLength textLength { get; }
        IJsSvgAnimatedEnumeration lengthAdjust { get; }

        long getNumberOfChars();
        float getComputedTextLength();
        float getSubStringLength(ulong charnum, ulong nchars);
        IJsSvgPoint getStartPositionOfChar(ulong charnum);
        IJsSvgPoint getEndPositionOfChar(ulong charnum);
        IJsSvgRect getExtentOfChar(ulong charnum);
        float getRotationOfChar(ulong charnum);
        long getCharNumAtPosition(IJsSvgPoint point);
        void selectSubString(ulong charnum, ulong nchars);
    }

    /// <summary>
    /// IJsSvgTextPositioningElement
    /// </summary>
    public interface IJsSvgTextPositioningElement : IJsSvgTextContentElement
    {
        IJsSvgAnimatedLengthList x { get; }
        IJsSvgAnimatedLengthList y { get; }
        IJsSvgAnimatedLengthList dx { get; }
        IJsSvgAnimatedLengthList dy { get; }
        IJsSvgAnimatedNumberList rotate { get; }
    }

    /// <summary>
    /// IJsSvgTextElement
    /// </summary>
    public interface IJsSvgTextElement : IJsSvgTextPositioningElement, IJsSvgTransformable
    {
    }

    /// <summary>
    /// IJsSvgTSpanElement
    /// </summary>
    public interface IJsSvgTSpanElement : IJsSvgTextPositioningElement
    {
    }

    /// <summary>
    /// IJsSvgTRefElement
    /// </summary>
    public interface IJsSvgTRefElement : IJsSvgTextPositioningElement, IJsSvgUriReference
    {
    }

    /// <summary>
    /// IJsSvgTextPathElement
    /// </summary>
    public interface IJsSvgTextPathElement : IJsSvgTextContentElement, IJsSvgUriReference
    {
        IJsSvgAnimatedLength startOffset { get; }
        IJsSvgAnimatedEnumeration method { get; }
        IJsSvgAnimatedEnumeration spacing { get; }
    }

    /// <summary>
    /// IJsSvgAltGlyphElement
    /// </summary>
    public interface IJsSvgAltGlyphElement : IJsSvgTextPositioningElement, IJsSvgUriReference
    {
        string glyphRef { get; set; }
        string format { get; set; }
    }

    /// <summary>
    /// IJsSvgAltGlyphDefElement
    /// </summary>
    public interface IJsSvgAltGlyphDefElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgAltGlyphItemElement
    /// </summary>
    public interface IJsSvgAltGlyphItemElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgGlyphRefElement
    /// </summary>
    public interface IJsSvgGlyphRefElement : IJsSvgElement, IJsSvgUriReference, IJsSvgStylable
    {
        string glyphRef { get; set; }
        string format { get; set; }
        float x { get; set; }
        float y { get; set; }
        float dx { get; set; }
        float dy { get; set; }
    }

    /// <summary>
    /// IJsSvgPaint
    /// </summary>
    public interface IJsSvgPaint : IJsSvgColor
    {
        ushort paintType { get; }
        string uri { get; }

        void setUri(string uri);
        void setPaint(ushort paintType, string uri, string rgbColor, string iccColor);
    }

    /// <summary>
    /// IJsSvgMarkerElement
    /// </summary>
    public interface IJsSvgMarkerElement : IJsSvgElement, IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgFitToViewBox
    {
        IJsSvgAnimatedLength refX { get; }
        IJsSvgAnimatedLength refY { get; }
        IJsSvgAnimatedEnumeration markerUnits { get; }
        IJsSvgAnimatedLength markerWidth { get; }
        IJsSvgAnimatedLength markerHeight { get; }
        IJsSvgAnimatedEnumeration orientType { get; }
        IJsSvgAnimatedAngle orientAngle { get; }

        void setOrientToAuto();
        void setOrientToAngle(IJsSvgAngle angle);
    }

    /// <summary>
    /// IJsSvgColorProfileElement
    /// </summary>
    public interface IJsSvgColorProfileElement : IJsSvgElement, IJsSvgUriReference, IJsSvgRenderingIntent
    {
        string local { get; set; }
        string name { get; set; }
        ushort renderingIntent { get; set; }
    }

    /// <summary>
    /// IJsSvgColorProfileRule
    /// </summary>
    public interface IJsSvgColorProfileRule : IJsSvgCssRule, IJsSvgRenderingIntent
    {
        string src { get; set; }
        string name { get; set; }
        ushort renderingIntent { get; set; }
    }

    /// <summary>
    /// IJsSvgGradientElement
    /// </summary>
    public interface IJsSvgGradientElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgUnitTypes
    {
        IJsSvgAnimatedEnumeration gradientUnits { get; }
        IJsSvgAnimatedTransformList gradientTransform { get; }
        IJsSvgAnimatedEnumeration spreadMethod { get; }
    }

    /// <summary>
    /// IJsSvgLinearGradientElement
    /// </summary>
    public interface IJsSvgLinearGradientElement : IJsSvgGradientElement
    {
        IJsSvgAnimatedLength x1 { get; }
        IJsSvgAnimatedLength y1 { get; }
        IJsSvgAnimatedLength x2 { get; }
        IJsSvgAnimatedLength y2 { get; }
    }

    /// <summary>
    /// IJsSvgRadialGradientElement
    /// </summary>
    public interface IJsSvgRadialGradientElement : IJsSvgGradientElement
    {
        IJsSvgAnimatedLength cx { get; }
        IJsSvgAnimatedLength cy { get; }
        IJsSvgAnimatedLength r { get; }
        IJsSvgAnimatedLength fx { get; }
        IJsSvgAnimatedLength fy { get; }
    }

    /// <summary>
    /// IJsSvgStopElement
    /// </summary>
    public interface IJsSvgStopElement : IJsSvgElement, IJsSvgStylable
    {
        IJsSvgAnimatedNumber offset { get; }
    }

    /// <summary>
    /// IJsSvgPatternElement
    /// </summary>
    public interface IJsSvgPatternElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgTests, IJsSvgLangSpace, IJsSvgExternalResourcesRequired, 
        IJsSvgStylable, IJsSvgFitToViewBox, IJsSvgUnitTypes
    {
        IJsSvgAnimatedEnumeration patternUnits { get; }
        IJsSvgAnimatedEnumeration patternContentUnits { get; }
        IJsSvgAnimatedTransformList patternTransform { get; }
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
    }

    /// <summary>
    /// IJsSvgClipPathElement
    /// </summary>
    public interface IJsSvgClipPathElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, 
        IJsSvgTransformable, IJsSvgUnitTypes
    {
        IJsSvgAnimatedEnumeration clipPathUnits { get; }
    }

    /// <summary>
    /// IJsSvgMaskElement
    /// </summary>
    public interface IJsSvgMaskElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgUnitTypes
    {
        IJsSvgAnimatedEnumeration maskUnits { get; }
        IJsSvgAnimatedEnumeration maskContentUnits { get; }
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
    }

    /// <summary>
    /// IJsSvgFilterElement
    /// </summary>
    public interface IJsSvgFilterElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgUnitTypes
    {
        IJsSvgAnimatedEnumeration filterUnits { get; }
        IJsSvgAnimatedEnumeration primitiveUnits { get; }
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        IJsSvgAnimatedInteger filterResX { get; }
        IJsSvgAnimatedInteger filterResY { get; }

        void setFilterRes(ulong filterResX, ulong filterResY);
    }

    /// <summary>
    /// IJsSvgFilterPrimitiveStandardAttributes
    /// </summary>
    public interface IJsSvgFilterPrimitiveStandardAttributes : IJsSvgStylable
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
        IJsSvgAnimatedString result { get; }
    }

    /// <summary>
    /// IJsSvgFEBlendElement
    /// </summary>
    public interface IJsSvgFEBlendElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedString in2 { get; }
        IJsSvgAnimatedEnumeration mode { get; }
    }

    /// <summary>
    /// IJsSvgFEColorMatrixElement
    /// </summary>
    public interface IJsSvgFEColorMatrixElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedEnumeration type { get; }
        IJsSvgAnimatedNumberList values { get; }
    }

    /// <summary>
    /// IJsSvgFEComponentTransferElement
    /// </summary>
    public interface IJsSvgFEComponentTransferElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
    }

    /// <summary>
    /// IJsSvgComponentTransferFunctionElement
    /// </summary>
    public interface IJsSvgComponentTransferFunctionElement : IJsSvgElement
    {
        IJsSvgAnimatedEnumeration type { get; }
        IJsSvgAnimatedNumberList tableValues { get; }
        IJsSvgAnimatedNumber slope { get; }
        IJsSvgAnimatedNumber intercept { get; }
        IJsSvgAnimatedNumber amplitude { get; }
        IJsSvgAnimatedNumber exponent { get; }
        IJsSvgAnimatedNumber offset { get; }
    }

    /// <summary>
    /// IJsSvgFEFuncRElement
    /// </summary>
    public interface IJsSvgFEFuncRElement : IJsSvgComponentTransferFunctionElement
    {
    }

    /// <summary>
    /// IJsSvgFEFuncGElement
    /// </summary>
    public interface IJsSvgFEFuncGElement : IJsSvgComponentTransferFunctionElement
    {
    }

    /// <summary>
    /// IJsSvgFEFuncBElement
    /// </summary>
    public interface IJsSvgFEFuncBElement : IJsSvgComponentTransferFunctionElement
    {
    }

    /// <summary>
    /// IJsSvgFEFuncAElement
    /// </summary>
    public interface IJsSvgFEFuncAElement : IJsSvgComponentTransferFunctionElement
    {
    }

    /// <summary>
    /// IJsSvgFECompositeElement
    /// </summary>
    public interface IJsSvgFECompositeElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedString in2 { get; }
        IJsSvgAnimatedEnumeration operator_ { get; }
        IJsSvgAnimatedNumber k1 { get; }
        IJsSvgAnimatedNumber k2 { get; }
        IJsSvgAnimatedNumber k3 { get; }
        IJsSvgAnimatedNumber k4 { get; }
    }

    /// <summary>
    /// IJsSvgFEConvolveMatrixElement
    /// </summary>
    public interface IJsSvgFEConvolveMatrixElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedInteger orderX { get; }
        IJsSvgAnimatedInteger orderY { get; }
        IJsSvgAnimatedNumberList kernelMatrix { get; }
        IJsSvgAnimatedNumber divisor { get; }
        IJsSvgAnimatedNumber bias { get; }
        IJsSvgAnimatedInteger targetX { get; }
        IJsSvgAnimatedInteger targetY { get; }
        IJsSvgAnimatedEnumeration edgeMode { get; }
        IJsSvgAnimatedLength kernelUnitLengthX { get; }
        IJsSvgAnimatedLength kernelUnitLengthY { get; }
        IJsSvgAnimatedBoolean preserveAlpha { get; }
    }

    /// <summary>
    /// IJsSvgFEDiffuseLightingElement
    /// </summary>
    public interface IJsSvgFEDiffuseLightingElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedNumber surfaceScale { get; }
        IJsSvgAnimatedNumber diffuseConstant { get; }
    }

    /// <summary>
    /// IJsSvgFEDistantLightElement
    /// </summary>
    public interface IJsSvgFEDistantLightElement : IJsSvgElement
    {
        IJsSvgAnimatedNumber azimuth { get; }
        IJsSvgAnimatedNumber elevation { get; }
    }

    /// <summary>
    /// IJsSvgFEPointLightElement
    /// </summary>
    public interface IJsSvgFEPointLightElement : IJsSvgElement
    {
        IJsSvgAnimatedNumber x { get; }
        IJsSvgAnimatedNumber y { get; }
        IJsSvgAnimatedNumber z { get; }
    }

    /// <summary>
    /// IJsSvgFESpotLightElement
    /// </summary>
    public interface IJsSvgFESpotLightElement : IJsSvgElement
    {
        IJsSvgAnimatedNumber x { get; }
        IJsSvgAnimatedNumber y { get; }
        IJsSvgAnimatedNumber z { get; }
        IJsSvgAnimatedNumber pointsAtX { get; }
        IJsSvgAnimatedNumber pointsAtY { get; }
        IJsSvgAnimatedNumber pointsAtZ { get; }
        IJsSvgAnimatedNumber specularExponent { get; }
        IJsSvgAnimatedNumber limitingConeAngle { get; }
    }

    /// <summary>
    /// IJsSvgFEDisplacementMapElement
    /// </summary>
    public interface IJsSvgFEDisplacementMapElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedString in2 { get; }
        IJsSvgAnimatedNumber scale { get; }
        IJsSvgAnimatedEnumeration xChannelSelector { get; }
        IJsSvgAnimatedEnumeration yChannelSelector { get; }
    }

    /// <summary>
    /// IJsSvgFEFloodElement
    /// </summary>
    public interface IJsSvgFEFloodElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
    }

    /// <summary>
    /// IJsSvgFEGaussianBlurElement
    /// </summary>
    public interface IJsSvgFEGaussianBlurElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedNumber stdDeviationX { get; }
        IJsSvgAnimatedNumber stdDeviationY { get; }

        void setStdDeviation(float stdDeviationX, float stdDeviationY);
    }

    /// <summary>
    /// IJsSvgFEImageElement
    /// </summary>
    public interface IJsSvgFEImageElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgFilterPrimitiveStandardAttributes
    {
    }

    /// <summary>
    /// IJsSvgFEMergeElement
    /// </summary>
    public interface IJsSvgFEMergeElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
    }

    /// <summary>
    /// IJsSvgFEMergeNodeElement
    /// </summary>
    public interface IJsSvgFEMergeNodeElement : IJsSvgElement
    {
        IJsSvgAnimatedString in1 { get; }
    }

    /// <summary>
    /// IJsSvgFEMorphologyElement
    /// </summary>
    public interface IJsSvgFEMorphologyElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedEnumeration operator_ { get; }
        IJsSvgAnimatedLength radiusX { get; }
        IJsSvgAnimatedLength radiusY { get; }
    }

    /// <summary>
    /// IJsSvgFEOffsetElement
    /// </summary>
    public interface IJsSvgFEOffsetElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedNumber dx { get; }
        IJsSvgAnimatedNumber dy { get; }
    }

    /// <summary>
    /// IJsSvgFESpecularLightingElement
    /// </summary>
    public interface IJsSvgFESpecularLightingElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
        IJsSvgAnimatedNumber surfaceScale { get; }
        IJsSvgAnimatedNumber specularConstant { get; }
        IJsSvgAnimatedNumber specularExponent { get; }
    }

    /// <summary>
    /// IJsSvgFETileElement
    /// </summary>
    public interface IJsSvgFETileElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedString in1 { get; }
    }

    /// <summary>
    /// IJsSvgFETurbulenceElement
    /// </summary>
    public interface IJsSvgFETurbulenceElement : IJsSvgElement, IJsSvgFilterPrimitiveStandardAttributes
    {
        IJsSvgAnimatedNumber baseFrequencyX { get; }
        IJsSvgAnimatedNumber baseFrequencyY { get; }
        IJsSvgAnimatedInteger numOctaves { get; }
        IJsSvgAnimatedNumber seed { get; }
        IJsSvgAnimatedEnumeration stitchTiles { get; }
        IJsSvgAnimatedEnumeration type { get; }
    }

    /// <summary>
    /// IJsSvgCursorElement
    /// </summary>
    public interface IJsSvgCursorElement : IJsSvgElement, IJsSvgUriReference, 
        IJsSvgTests, IJsSvgExternalResourcesRequired
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
    }

    /// <summary>
    /// IJsSvgAElement
    /// </summary>
    public interface IJsSvgAElement : IJsSvgElement, IJsSvgUriReference, IJsSvgTests, 
        IJsSvgLangSpace, IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedString target { get; }
    }

    /// <summary>
    /// IJsSvgViewElement
    /// </summary>
    public interface IJsSvgViewElement : IJsSvgElement, IJsSvgExternalResourcesRequired, 
        IJsSvgFitToViewBox, IJsSvgZoomAndPan
    {
        IJsSvgStringList viewTarget { get; }
    }

    /// <summary>
    /// IJsSvgScriptElement
    /// </summary>
    public interface IJsSvgScriptElement : IJsSvgElement, IJsSvgUriReference, IJsSvgExternalResourcesRequired
    {
        string type { get; set; }
    }

    /// <summary>
    /// IJsSvgEvent
    /// </summary>
    public interface IJsSvgEvent : IJsEvent
    {
    }

    /// <summary>
    /// IJsSvgZoomEvent
    /// </summary>
    public interface IJsSvgZoomEvent : IJsUiEvent
    {
        IJsSvgRect zoomRectScreen { get; }
        float previousScale { get; }
        IJsSvgPoint previousTranslate { get; }
        float newScale { get; }
        IJsSvgPoint newTranslate { get; }
    }

    /// <summary>
    /// IJsSvgAnimationElement
    /// </summary>
    public interface IJsSvgAnimationElement : IJsSvgElement, IJsSvgTests, 
        IJsSvgExternalResourcesRequired, IJsElementTimeControl, IJsEventTarget
    {
        float getStartTime();
        float getCurrentTime();
        float getSimpleDuration();
        IJsSvgElement targetElement { get; }
    }

    /// <summary>
    /// IJsSvgAnimateElement
    /// </summary>
    public interface IJsSvgAnimateElement : IJsSvgAnimationElement
    {
    }

    /// <summary>
    /// IJsSvgSetElement
    /// </summary>
    public interface IJsSvgSetElement : IJsSvgAnimationElement
    {
    }

    /// <summary>
    /// IJsSvgAnimateMotionElement
    /// </summary>
    public interface IJsSvgAnimateMotionElement : IJsSvgAnimationElement
    {
    }

    /// <summary>
    /// IJsSvgMPathElement
    /// </summary>
    public interface IJsSvgMPathElement : IJsSvgElement, IJsSvgUriReference, IJsSvgExternalResourcesRequired
    {
    }

    /// <summary>
    /// IJsSvgAnimateColorElement
    /// </summary>
    public interface IJsSvgAnimateColorElement : IJsSvgAnimationElement
    {
    }

    /// <summary>
    /// IJsSvgAnimateTransformElement
    /// </summary>
    public interface IJsSvgAnimateTransformElement : IJsSvgAnimationElement
    {
    }

    /// <summary>
    /// IJsSvgFontElement
    /// </summary>
    public interface IJsSvgFontElement : IJsSvgElement, IJsSvgExternalResourcesRequired, IJsSvgStylable
    {
    }

    /// <summary>
    /// IJsSvgGlyphElement
    /// </summary>
    public interface IJsSvgGlyphElement : IJsSvgElement, IJsSvgStylable
    {
    }

    /// <summary>
    /// IJsSvgMissingGlyphElement
    /// </summary>
    public interface IJsSvgMissingGlyphElement : IJsSvgElement, IJsSvgStylable
    {
    }

    /// <summary>
    /// IJsSvgHKernElement
    /// </summary>
    public interface IJsSvgHKernElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgVKernElement
    /// </summary>
    public interface IJsSvgVKernElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgFontFaceElement
    /// </summary>
    public interface IJsSvgFontFaceElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgFontFaceSrcElement
    /// </summary>
    public interface IJsSvgFontFaceSrcElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgFontFaceUriElement
    /// </summary>
    public interface IJsSvgFontFaceUriElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgFontFaceFormatElement
    /// </summary>
    public interface IJsSvgFontFaceFormatElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgFontFaceNameElement
    /// </summary>
    public interface IJsSvgFontFaceNameElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgDefinitionSrcElement
    /// </summary>
    public interface IJsSvgDefinitionSrcElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgMetadataElement
    /// </summary>
    public interface IJsSvgMetadataElement : IJsSvgElement
    {
    }

    /// <summary>
    /// IJsSvgForeignObjectElement
    /// </summary>
    public interface IJsSvgForeignObjectElement : IJsSvgElement, IJsSvgTests,  IJsSvgLangSpace, 
        IJsSvgExternalResourcesRequired, IJsSvgStylable, IJsSvgTransformable, IJsEventTarget
    {
        IJsSvgAnimatedLength x { get; }
        IJsSvgAnimatedLength y { get; }
        IJsSvgAnimatedLength width { get; }
        IJsSvgAnimatedLength height { get; }
    }  
}
