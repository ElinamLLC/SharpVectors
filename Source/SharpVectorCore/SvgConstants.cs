using System;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Define SVG constants, such as tag names, attribute names and URI
    /// </summary>
    public static class SvgConstants
    {
        // ---------------------------------------------------------------------
        // SVG general
        // ---------------------------------------------------------------------
        public const string PublicId     = "-//W3C//DTD SVG 1.0//EN";
        public const string SystemId     = "http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd";
        public const string NamespaceUri = "http://www.w3.org/2000/svg";
        public const string Version      = "1.1";

        // ---------------------------------------------------------------------
        // Events type and attributes
        // ---------------------------------------------------------------------
        /// <summary>
        /// The event type for MouseEvent.
        /// </summary>
        public const string MouseEventsType = "MouseEvents";

        /// <summary>
        /// The event type for UIEvent.
        /// </summary>
        public const string UIEventsType    = "UIEvents";

        /// <summary>
        /// The event type for SVGEvent.
        /// </summary>
        public const string SvgEventsType   = "SVGEvents";

        /// <summary>
        /// The event type for KeyEvent.
        /// </summary>
        public const string KeyEventsType   = "KeyEvents";

        // ---------------------------------------------------------------------

        /// <summary>
        /// The event type for 'keydown' KeyEvent.
        /// </summary>
        public const string KeydownEventType     = "keydown";

        /// <summary>
        /// The event type for 'keypress' KeyEvent.
        /// </summary>
        public const string KeypressEventType    = "keypress";

        /// <summary>
        /// The event type for 'keyup' KeyEvent.
        /// </summary>
        public const string KeyupEventType       = "keyup";

        /// <summary>
        /// The event type for 'click' MouseEvent.
        /// </summary>
        public const string ClickEventType       = "click";

        /// <summary>
        /// The event type for 'mouseup' MouseEvent.
        /// </summary>
        public const string MouseupEventType     = "mouseup";

        /// <summary>
        /// The event type for 'mousedown' MouseEvent.
        /// </summary>
        public const string MousedownEventType   = "mousedown";

        /// <summary>
        /// The event type for 'mousemove' MouseEvent.
        /// </summary>
        public const string MousemoveEventType   = "mousemove";

        /// <summary>
        /// The event type for 'mouseout' MouseEvent.
        /// </summary>
        public const string MouseoutEventType    = "mouseout";

        /// <summary>
        /// The event type for 'mouseover' MouseEvent.
        /// </summary>
        public const string MouseoverEventType   = "mouseover";

        /// <summary>
        /// The event type for 'DOMFocusIn' UIEvent.
        /// </summary>
        public const string DomfocusinEventType  = "DOMFocusIn";

        /// <summary>
        /// The event type for 'DOMFocusOut' UIEvent.
        /// </summary>
        public const string DomfocusoutEventType = "DOMFocusOut";

        /// <summary>
        /// The event type for 'DOMActivate' UIEvent.
        /// </summary>
        public const string DomactivateEventType = "DOMActivate";

        /// <summary>
        /// The event type for 'SVGLoad' SVGEvent.
        /// </summary>
        public const string SvgLoadEventType     = "SVGLoad";

        /// <summary>
        /// The event type for 'SVGUnload' SVGEvent.
        /// </summary>
        public const string SvgUnloadEventType   = "SVGUnload";

        /// <summary>
        /// The event type for 'SVGAbort' SVGEvent.
        /// </summary>
        public const string SvgAbortEventType    = "SVGAbort";

        /// <summary>
        /// The event type for 'SVGError' SVGEvent.
        /// </summary>
        public const string SvgErrorEventType    = "SVGError";

        /// <summary>
        /// The event type for 'SVGResize' SVGEvent.
        /// </summary>
        public const string SvgResizeEventType   = "SVGResize";

        /// <summary>
        /// The event type for 'SVGScroll' SVGEvent.
        /// </summary>
        public const string SvgScrollEventType   = "SVGScroll";

        /// <summary>
        /// The event type for 'SVGZoom' SVGEvent.
        /// </summary>
        public const string SvgZoomEventType     = "SVGZoom";

        // ---------------------------------------------------------------------

        /// <summary>
        /// The 'onkeyup' attribute name of type KeyEvents.
        /// </summary>
        public const string OnKeyupAttribute     = "onkeyup";

        /// <summary>
        /// The 'onkeydown' attribute name of type KeyEvents.
        /// </summary>
        public const string OnKeydownAttribute   = "onkeydown";

        /// <summary>
        /// The 'onkeypress' attribute name of type KeyEvents.
        /// </summary>
        public const string OnKeypressAttribute  = "onkeypress";

        /// <summary>
        /// The 'onabort' attribute name of type SVGEvents.
        /// </summary>
        public const string OnAbortAttribute     = "onabort";

        /// <summary>
        /// The 'onabort' attribute name of type SVGEvents.
        /// </summary>
        public const string OnActivateAttribute  = "onactivate";

        /// <summary>
        /// The 'onbegin' attribute name of type SVGEvents.
        /// </summary>
        public const string OnBeginAttribute     = "onbegin";

        /// <summary>
        /// The 'onclick' attribute name of type MouseEvents.
        /// </summary>
        public const string OnClickAttribute     = "onclick";

        /// <summary>
        /// The 'onend' attribute name of type SVGEvents.
        /// </summary>
        public const string OnEndAttribute       = "onend";

        /// <summary>
        /// The 'onerror' attribute name of type SVGEvents.
        /// </summary>
        public const string OnErrorAttribute     = "onerror";

        /// <summary>
        /// The 'onfocusin' attribute name of type UIEvents.
        /// </summary>
        public const string OnFocusinAttribute   = "onfocusin";

        /// <summary>
        /// The 'onfocusout' attribute name of type UIEvents.
        /// </summary>
        public const string OnFocusoutAttribute  = "onfocusout";

        /// <summary>
        /// The 'onload' attribute name of type SVGEvents.
        /// </summary>
        public const string OnLoadAttribute      = "onload";

        /// <summary>
        /// The 'onmousedown' attribute name of type MouseEvents.
        /// </summary>
        public const string OnMousedownAttribute = "onmousedown";

        /// <summary>
        /// The 'onmousemove' attribute name of type MouseEvents.
        /// </summary>
        public const string OnMousemoveAttribute = "onmousemove";

        /// <summary>
        /// The 'onmouseout' attribute name of type MouseEvents.
        /// </summary>
        public const string OnMouseoutAttribute  = "onmouseout";

        /// <summary>
        /// The 'onmouseover' attribute name of type MouseEvents.
        /// </summary>
        public const string OnMouseoverAttribute = "onmouseover";

        /// <summary>
        /// The 'onmouseup' attribute name of type MouseEvents.
        /// </summary>
        public const string OnMouseupAttribute   = "onmouseup";

        /// <summary>
        /// The 'onrepeat' attribute name of type SVGEvents.
        /// </summary>
        public const string OnRepeatAttribute    = "onrepeat";

        /// <summary>
        /// The 'onresize' attribute name of type SVGEvents.
        /// </summary>
        public const string OnResizeAttribute    = "onresize";

        /// <summary>
        /// The 'onscroll' attribute name of type SVGEvents.
        /// </summary>
        public const string OnScrollAttribute    = "onscroll";

        /// <summary>
        /// The 'onunload' attribute name of type SVGEvents.
        /// </summary>
        public const string OnUnloadAttribute    = "onunload";

        /// <summary>
        /// The 'onzoom' attribute name of type SVGEvents.
        /// </summary>
        public const string OnZoomAttribute      = "onzoom";

        // ---------------------------------------------------------------------
        // SVG features
        // ---------------------------------------------------------------------
        // SVG 1.0 feature strings
        public const string FeatureOrgW3cSvg             = "org.w3c.svg";
        public const string FeatureOrgW3cSvgStatic       = "org.w3c.svg.static";
        public const string FeatureOrgW3cSvgAnimation    = "org.w3c.svg.animation";
        public const string FeatureOrgW3cSvgDynamic      = "org.w3c.svg.dynamic";
        public const string FeatureOrgW3cSvgAll          = "org.w3c.svg.all";
        public const string FeatureOrgW3cDomSvg          = "org.w3c.dom.svg";
        public const string FeatureOrgW3cDomSvgStatic    = "org.w3c.dom.svg.static";
        public const string FeatureOrgW3cDomSvgAnimation = "org.w3c.dom.svg.animation";
        public const string FeatureOrgW3cDomSvgDynamic   = "org.w3c.dom.svg.dynamic";
        public const string FeatureOrgW3cDomSvgAll       = "org.w3c.dom.svg.all";

        // SVG 1.1 feature strings
        public const string FeatureSvg                       = "http://www.w3.org/TR/SVG11/feature#SVG";
        public const string FeatureSvgDom                    = "http://www.w3.org/TR/SVG11/feature#SVGDOM";
        public const string FeatureSvgStatic                 = "http://www.w3.org/TR/SVG11/feature#SVG-static";
        public const string FeatureSvgDomStatic              = "http://www.w3.org/TR/SVG11/feature#SVGDOM-static";
        public const string FeatureSvgAnimation              = "http://www.w3.org/TR/SVG11/feature#SVG-animation";
        public const string FeatureSvgDomAnimation           = "http://www.w3.org/TR/SVG11/feature#SVGDOM-animation";
        public const string FeatureSvgDynamic                = "http://www.w3.org/TR/SVG11/feature#SVG-dynamic";
        public const string FeatureSvgDomDynamic             = "http://www.w3.org/TR/SVG11/feature#SVGDOM-dynamic";
        public const string FeatureCoreAttribute             = "http://www.w3.org/TR/SVG11/feature#CoreAttribute";
        public const string FeatureStructure                 = "http://www.w3.org/TR/SVG11/feature#Structure";
        public const string FeatureBasicStructure            = "http://www.w3.org/TR/SVG11/feature#BasicStructure";
        public const string FeatureContainerAttribute        = "http://www.w3.org/TR/SVG11/feature#ContainerAttribute";
        public const string FeatureConditionalProcessing     = "http://www.w3.org/TR/SVG11/feature#ConditionalProcessing";
        public const string FeatureImage                     = "http://www.w3.org/TR/SVG11/feature#Image";
        public const string FeatureStyle                     = "http://www.w3.org/TR/SVG11/feature#Style";
        public const string FeatureViewportAttribute         = "http://www.w3.org/TR/SVG11/feature#ViewportAttribute";
        public const string FeatureShape                     = "http://www.w3.org/TR/SVG11/feature#Shape";
        public const string FeatureText                      = "http://www.w3.org/TR/SVG11/feature#Text";
        public const string FeatureBasicText                 = "http://www.w3.org/TR/SVG11/feature#BasicText";
        public const string FeaturePaintAttribute            = "http://www.w3.org/TR/SVG11/feature#PaintAttribute";
        public const string FeatureBasicPaintAttribute       = "http://www.w3.org/TR/SVG11/feature#BasicPaintAttribute";
        public const string FeatureOpacityAttribute          = "http://www.w3.org/TR/SVG11/feature#OpacityAttribute";
        public const string FeatureGraphicsAttribute         = "http://www.w3.org/TR/SVG11/feature#GraphicsAttribute";
        public const string FeatureBasicGraphicsAttribute    = "http://www.w3.org/TR/SVG11/feature#BasicGraphicsAttribute";
        public const string FeatureMarker                    = "http://www.w3.org/TR/SVG11/feature#Marker";
        public const string FeatureColorProfile              = "http://www.w3.org/TR/SVG11/feature#ColorProfile";
        public const string FeatureGradient                  = "http://www.w3.org/TR/SVG11/feature#Gradient";
        public const string FeaturePattern                   = "http://www.w3.org/TR/SVG11/feature#Pattern";
        public const string FeatureClip                      = "http://www.w3.org/TR/SVG11/feature#Clip";
        public const string FeatureBasicClip                 = "http://www.w3.org/TR/SVG11/feature#BasicClip";
        public const string FeatureMask                      = "http://www.w3.org/TR/SVG11/feature#Mask";
        public const string FeatureFilter                    = "http://www.w3.org/TR/SVG11/feature#Filter";
        public const string FeatureBasicFilter               = "http://www.w3.org/TR/SVG11/feature#BasicFilter";
        public const string FeatureDocumentEventsAttribute   = "http://www.w3.org/TR/SVG11/feature#DocumentEventsAttribute";
        public const string FeatureGraphicalEventsAttribute  = "http://www.w3.org/TR/SVG11/feature#GraphicalEventsAttribute";
        public const string FeatureAnimationEventsAttribute  = "http://www.w3.org/TR/SVG11/feature#AnimationEventsAttribute";
        public const string FeatureCursor                    = "http://www.w3.org/TR/SVG11/feature#Cursor";
        public const string FeatureHyperlinking              = "http://www.w3.org/TR/SVG11/feature#Hyperlinking";
        public const string FeatureXlink                     = "http://www.w3.org/TR/SVG11/feature#Xlink";
        public const string FeatureExternalResourcesRequired = "http://www.w3.org/TR/SVG11/feature#ExternalResourcesRequired";
        public const string FeatureView                      = "http://www.w3.org/TR/SVG11/feature#View";
        public const string FeatureScript                    = "http://www.w3.org/TR/SVG11/feature#Script";
        public const string FeatureAnimation                 = "http://www.w3.org/TR/SVG11/feature#Animation";
        public const string FeatureFont                      = "http://www.w3.org/TR/SVG11/feature#Font";
        public const string FeatureBasicFont                 = "http://www.w3.org/TR/SVG11/feature#BasicFont";
        public const string FeatureExtensibility             = "http://www.w3.org/TR/SVG11/feature#Extensibility";

        // ---------------------------------------------------------------------
        // TODO SVG 1.2 feature strings
        // ---------------------------------------------------------------------

        // ---------------------------------------------------------------------
        // SVG tags
        // ---------------------------------------------------------------------
        public const string TagA                   = "a";
        public const string TagAltGlyph            = "altGlyph";
        public const string TagAltGlyphDef         = "altGlyphDef";
        public const string TagAltGlyphItem        = "altGlyphItem";
        public const string TagAnimate             = "animate";
        public const string TagAnimateColor        = "animateColor";
        public const string TagAnimateMotion       = "animateMotion";
        public const string TagAnimateTransform    = "animateTransform";
        public const string TagCircle              = "circle";
        public const string TagClipPath            = "clipPath";
        public const string TagColorProfile        = "color-profile";
        public const string TagCursor              = "cursor";
        public const string TagDefinitionSrc       = "definition-src";
        public const string TagDefs                = "defs";
        public const string TagDesc                = "desc";
        public const string TagEllipse             = "ellipse";
        public const string TagFeBlend             = "feBlend";
        public const string TagFeColorMatrix       = "feColorMatrix";
        public const string TagFeComponentTransfer = "feComponentTransfer";
        public const string TagFeComposite         = "feComposite";
        public const string TagFeConvolveMatrix    = "feConvolveMatrix";
        public const string TagFeDiffuseLighting   = "feDiffuseLighting";
        public const string TagFeDisplacementMap   = "feDisplacementMap";
        public const string TagFeDistantLight      = "feDistantLight";
        public const string TagFeFlood             = "feFlood";
        public const string TagFeFuncA             = "feFuncA";
        public const string TagFeFuncB             = "feFuncB";
        public const string TagFeFuncG             = "feFuncG";
        public const string TagFeFuncR             = "feFuncR";
        public const string TagFeGaussianBlur      = "feGaussianBlur";
        public const string TagFeImage             = "feImage";
        public const string TagFeMergeNode         = "feMergeNode";
        public const string TagFeMerge             = "feMerge";
        public const string TagFeMorphology        = "feMorphology";
        public const string TagFeOffset            = "feOffset";
        public const string TagFePointLight        = "fePointLight";
        public const string TagFeSpecularLighting  = "feSpecularLighting";
        public const string TagFeSpotLight         = "feSpotLight";
        public const string TagFeTile              = "feTile";
        public const string TagFeTurbulence        = "feTurbulence";
        public const string TagFilter              = "filter";
        public const string TagFont                = "font";
        public const string TagFontFace            = "font-face";
        public const string TagFontFaceFormat      = "font-face-format";
        public const string TagFontFaceName        = "font-face-name";
        public const string TagFontFaceSrc         = "font-face-src";
        public const string TagFontFaceUri         = "font-face-uri";
        public const string TagForeignObject       = "foreignObject";
        public const string TagG                   = "g";
        public const string TagGlyph               = "glyph";
        public const string TagGlyphRef            = "glyphRef";
        public const string TagHkern               = "hkern";
        public const string TagImage               = "image";
        public const string TagLine                = "line";
        public const string TagLinearGradient      = "linearGradient";
        public const string TagMarker              = "marker";
        public const string TagMask                = "mask";
        public const string TagMetadata            = "metadata";
        public const string TagMissingGlyph        = "missing-glyph";
        public const string TagMpath               = "mpath";
        public const string TagPath                = "path";
        public const string TagPattern             = "pattern";
        public const string TagPolygon             = "polygon";
        public const string TagPolyline            = "polyline";
        public const string TagRadialGradient      = "radialGradient";
        public const string TagRect                = "rect";
        public const string TagSet                 = "set";
        public const string TagScript              = "script";
        public const string TagStop                = "stop";
        public const string TagStyle               = "style";
        public const string TagSvg                 = "svg";
        public const string TagSwitch              = "switch";
        public const string TagSymbol              = "symbol";
        public const string TagTextArea            = "textArea";
        public const string TagTextPath            = "textPath";
        public const string TagText                = "text";
        public const string TagTitle               = "title";
        public const string TagTref                = "tref";
        public const string TagTspan               = "tspan";
        public const string TagUse                 = "use";
        public const string TagView                = "view";
        public const string TagVkern               = "vkern";

        // ---------------------------------------------------------------------
        // SVG attributes
        // ---------------------------------------------------------------------
        public const string AttrAccentHeight              = "accent-height";
        public const string AttrAccumulate                = "accumulate";
        public const string AttrAdditive                  = "additive";
        public const string AttrAmplitude                 = "amplitude";
        public const string AttrArabicForm                = "arabic-form";
        public const string AttrAscent                    = "ascent";
        public const string AttrAzimuth                   = "azimuth";
        public const string AttrAlphabetic                = "alphabetic";
        public const string AttrAttributeName             = "attributeName";
        public const string AttrAttributeType             = "attributeType";
        public const string AttrBaseFrequency             = "baseFrequency";
        public const string AttrBaseProfile               = "baseProfile";
        public const string AttrBegin                     = "begin";
        public const string AttrBbox                      = "bbox";
        public const string AttrBias                      = "bias";
        public const string AttrBy                        = "by";
        public const string AttrCalcMode                  = "calcMode";
        public const string AttrCapHeight                 = "cap-height";
        public const string AttrClass                     = "class";
        public const string AttrClipPath                  = CssConstants.PropClipPath;
        public const string AttrClipPathUnits             = "clipPathUnits";
        public const string AttrColorInterpolation        = CssConstants.PropColorInterpolation;
        public const string AttrColorRendering            = CssConstants.PropColorRendering;
        public const string AttrContentScriptType         = "contentScriptType";
        public const string AttrContentStyleType          = "contentStyleType";
        public const string AttrCx                        = "cx";
        public const string AttrCy                        = "cy";
        public const string AttrDescent                   = "descent";
        public const string AttrDiffuseConstant           = "diffuseConstant";
        public const string AttrDivisor                   = "divisor";
        public const string AttrDur                       = "dur";
        public const string AttrDx                        = "dx";
        public const string AttrDy                        = "dy";
        public const string AttrD                         = "d";
        public const string AttrEdgeMode                  = "edgeMode";
        public const string AttrElevation                 = "elevation";
        public const string AttrEnableBackground          = CssConstants.PropEnableBackground;
        public const string AttrEnd                       = "end";
        public const string AttrExponent                  = "exponent";
        public const string AttrExternalResourcesRequired = "externalResourcesRequired";
        public const string AttrFill                      = CssConstants.PropFill;
        public const string AttrFillOpacity               = CssConstants.PropFillOpacity;
        public const string AttrFillRule                  = CssConstants.PropFillRule;
        public const string AttrFilter                    = CssConstants.PropFilter;
        public const string AttrFilterRes                 = "filterRes";
        public const string AttrFilterUnits               = "filterUnits";
        public const string AttrFloodColor                = CssConstants.PropFloodColor;
        public const string AttrFloodOpacity              = CssConstants.PropFloodOpacity;
        public const string AttrFormat                    = "format";
        public const string AttrFontFamily                = CssConstants.PropFontFamily;
        public const string AttrFontSize                  = CssConstants.PropFontSize;
        public const string AttrFontStretch               = CssConstants.PropFontStretch;
        public const string AttrFontStyle                 = CssConstants.PropFontStyle;
        public const string AttrFontVariant               = CssConstants.PropFontVariant;
        public const string AttrFontWeight                = CssConstants.PropFontWeight;
        public const string AttrFrom                      = "from";
        public const string AttrFx                        = "fx";
        public const string AttrFy                        = "fy";
        public const string AttrG1                        = "g1";
        public const string AttrG2                        = "g2";
        public const string AttrGlyphName                 = "glyph-name";
        public const string AttrGlyphRef                  = "glyphRef";
        public const string AttrGradientTransform         = "gradientTransform";
        public const string AttrGradientUnits             = "gradientUnits";
        public const string AttrHanging                   = "hanging";
        public const string AttrHeight                    = "height";
        public const string AttrHorizAdvX                 = "horiz-adv-x";
        public const string AttrHorizOriginX              = "horiz-origin-x";
        public const string AttrHorizOriginY              = "horiz-origin-y";
        public const string AttrId                        = XmlConstants.IdAttribute;
        public const string AttrIdeographic               = "ideographic";
        public const string AttrImageRendering            = CssConstants.PropImageRendering;
        public const string AttrIn2                       = "in2";
        public const string AttrIntercept                 = "intercept";
        public const string AttrIn                        = "in";
        public const string AttrK                         = "k";
        public const string AttrK1                        = "k1";
        public const string AttrK2                        = "k2";
        public const string AttrK3                        = "k3";
        public const string AttrK4                        = "k4";
        public const string AttrKernelMatrix              = "kernelMatrix";
        public const string AttrKernelUnitLength          = "kernelUnitLength";
        public const string AttrKerning                   = CssConstants.PropKerning;
        public const string AttrKeyPoints                 = "keyPoints";
        public const string AttrKeySplines                = "keySplines";
        public const string AttrKeyTimes                  = "keyTimes";
        public const string AttrLang                      = "lang";
        public const string AttrLengthAdjust              = "lengthAdjust";
        public const string AttrLightingColor             = "lighting-color";
        public const string AttrLimitingConeAngle         = "limitingConeAngle";
        public const string AttrLocal                     = "local";
        public const string AttrMarkerHeight              = "markerHeight";
        public const string AttrMarkerUnits               = "markerUnits";
        public const string AttrMarkerWidth               = "markerWidth";
        public const string AttrMask                      = CssConstants.PropMask;
        public const string AttrMaskContentUnits          = "maskContentUnits";
        public const string AttrMaskUnits                 = "maskUnits";
        public const string AttrMathematical              = "mathematical";
        public const string AttrMax                       = "max";
        public const string AttrMedia                     = "media";
        public const string AttrMethod                    = "method";
        public const string AttrMin                       = "min";
        public const string AttrMode                      = "mode";
        public const string AttrName                      = "name";
        public const string AttrNumOctaves                = "numOctaves";
        public const string AttrOffset                    = "offset";
        public const string AttrOpacity                   = CssConstants.PropOpacity;
        public const string AttrOperator                  = "operator";
        public const string AttrOrder                     = "order";
        public const string AttrOrient                    = "orient";
        public const string AttrOrientation               = "orientation";
        public const string AttrOrigin                    = "origin";
        public const string AttrOverlinePosition          = "overline-position";
        public const string AttrOverlineThickness         = "overline-thickness";
        public const string AttrPanose_1                  = "panose-1";
        public const string AttrPath                      = "path";
        public const string AttrPathLength                = "pathLength";
        public const string AttrPatternContentUnits       = "patternContentUnits";
        public const string AttrPatternTransform          = "patternTransform";
        public const string AttrPatternUnits              = "patternUnits";
        public const string AttrPoints                    = "points";
        public const string AttrPointsAtX                 = "pointsAtX";
        public const string AttrPointsAtY                 = "pointsAtY";
        public const string AttrPointsAtZ                 = "pointsAtZ";
        public const string AttrPreserveAlpha             = "preserveAlpha";
        public const string AttrPreserveAspectRatio       = "preserveAspectRatio";
        public const string AttrPrimitiveUnits            = "primitiveUnits";
        public const string AttrRadius                    = "radius";
        public const string AttrRefX                      = "refX";
        public const string AttrRefY                      = "refY";
        public const string AttrRenderingIntent           = "rendering-intent";
        public const string AttrRepeatCount               = "repeatCount";
        public const string AttrRepeatDur                 = "repeatDur";
        public const string AttrRequiredFeatures          = "requiredFeatures";
        public const string AttrRequiredExtensions        = "requiredExtensions";
        public const string AttrResult                    = "result";
        public const string AttrRestart                   = "restart";
        public const string AttrRx                        = "rx";
        public const string AttrRy                        = "ry";
        public const string AttrR                         = "r";
        public const string AttrRotate                    = "rotate";
        public const string AttrScale                     = "scale";
        public const string AttrSeed                      = "seed";
        public const string AttrShapeRendering            = CssConstants.PropShapeRendering;
        public const string AttrSlope                     = "slope";
        public const string AttrSnapshotTime              = "snapshotTime";
        public const string AttrSpace                     = "space";
        public const string AttrSpacing                   = "spacing";
        public const string AttrSpecularConstant          = "specularConstant";
        public const string AttrSpecularExponent          = "specularExponent";
        public const string AttrSpreadMethod              = "spreadMethod";
        public const string AttrStartOffset               = "startOffset";
        public const string AttrStdDeviation              = "stdDeviation";
        public const string AttrStemh                     = "stemh";
        public const string AttrStemv                     = "stemv";
        public const string AttrStitchTiles               = "stitchTiles";
        public const string AttrStopColor                 = "stop-color";
        public const string AttrStopOpacity               = CssConstants.PropStopOpacity;
        public const string AttrStrikethroughPosition     = "strikethrough-position";
        public const string AttrStrikethroughThickness    = "strikethrough-thickness";
        public const string AttrString                    = "string";
        public const string AttrStroke                    = CssConstants.PropStroke;
        public const string AttrStrokeDasharray           = CssConstants.PropStrokeDasharray;
        public const string AttrStrokeDashoffset          = CssConstants.PropStrokeDashoffset;
        public const string AttrStrokeLinecap             = CssConstants.PropStrokeLinecap;
        public const string AttrStrokeLinejoin            = CssConstants.PropStrokeLinejoin;
        public const string AttrStrokeMiterlimit          = CssConstants.PropStrokeMiterlimit;
        public const string AttrStrokeOpacity             = CssConstants.PropStrokeOpacity;
        public const string AttrStrokeWidth               = CssConstants.PropStrokeWidth;
        public const string AttrStyle                     = "style";
        public const string AttrSurfaceScale              = "surfaceScale";
        public const string AttrSystemLanguage            = "systemLanguage";
        public const string AttrTableValues               = "tableValues";
        public const string AttrTarget                    = "target";
        public const string AttrTargetX                   = "targetX";
        public const string AttrTargetY                   = "targetY";
        public const string AttrTextAnchor                = CssConstants.PropTextAnchor;
        public const string AttrTextLength                = "textLength";
        public const string AttrTextRendering             = CssConstants.PropTextRendering;
        public const string AttrTitle                     = "title";
        public const string AttrTo                        = "to";
        public const string AttrTransform                 = "transform";
        public const string AttrType                      = "type";
        public const string AttrU1                        = "u1";
        public const string AttrU2                        = "u2";
        public const string AttrUnderlinePosition         = "underline-position";
        public const string AttrUnderlineThickness        = "underline-thickness";
        public const string AttrUnicode                   = "unicode";
        public const string AttrUnicodeRange              = "unicode-range";
        public const string AttrUnitsPerEm                = "units-per-em";
        public const string AttrVAlphabetic               = "v-alphabetic";
        public const string AttrVHanging                  = "v-hanging";
        public const string AttrVIdeographic              = "v-ideographic";
        public const string AttrVMathematical             = "v-mathematical";
        public const string AttrValues                    = "values";
        public const string AttrVersion                   = "version";
        public const string AttrVertAdvY                  = "vert-adv-y";
        public const string AttrVertOriginX               = "vert-origin-x";
        public const string AttrVertOriginY               = "vert-origin-y";
        public const string AttrViewBox                   = "viewBox";
        public const string AttrViewTarget                = "viewTarget";
        public const string AttrWidth                     = "width";
        public const string AttrWidths                    = "widths";
        public const string AttrX1                        = "x1";
        public const string AttrX2                        = "x2";
        public const string AttrX                         = "x";
        public const string AttrXChannelSelector          = "xChannelSelector";
        public const string AttrXHeight                   = "xHeight";
        public const string AttrY1                        = "y1";
        public const string AttrY2                        = "y2";
        public const string AttrY                         = "y";
        public const string AttrYChannelSelector          = "yChannelSelector";
        public const string AttrZ                         = "z";
        public const string AttrZoomAndPan                = "zoomAndPan";

        // ---------------------------------------------------------------------
        // SVG attribute value
        // ---------------------------------------------------------------------
        public const string ValWeight100            = "100";
        public const string ValWeight200            = "200";
        public const string ValWeight300            = "300";
        public const string ValWeight400            = "400";
        public const string ValWeight500            = "500";
        public const string ValWeight600            = "600";
        public const string ValWeight700            = "700";
        public const string ValWeight800            = "800";
        public const string ValWeight900            = "900";
        public const string ValAbsoluteColorimetric = "absolute-colorimetric";
        public const string ValAlign                = "align";
        public const string ValAll                  = "all";
        public const string ValArithmetic           = "arithmetic";
        public const string ValAtop                 = "atop";
        public const string ValAuto                 = "auto";
        public const string ValA                    = "A";
        public const string ValBackgroundAlpha      = "BackgroundAlpha";
        public const string ValBackgroundImage      = "BackgroundImage";
        public const string ValBevel                = "bevel";
        public const string ValBolder               = "bolder";
        public const string ValBold                 = "bold";
        public const string ValButt                 = "butt";
        public const string ValB                    = "B";
        public const string ValComposite            = "composite";
        public const string ValCrispEdges           = "crispEdges";
        public const string ValCrosshair            = "crosshair";
        public const string ValDarken               = "darken";
        public const string ValDefault              = "default";
        public const string ValDigitOne             = "1";
        public const string ValDilate               = "dilate";
        public const string ValDisable              = "disable";
        public const string ValDiscrete             = "discrete";
        public const string ValDuplicate            = "duplicate";
        public const string ValEnd                  = "end";
        public const string ValErode                = "erode";
        public const string ValEvenOdd              = "evenodd";
        public const string ValExact                = "exact";
        public const string ValEResize              = "e-resize";
        public const string ValFalse                = "false";
        public const string ValFillPaint            = "FillPaint";
        public const string ValFlood                = "flood";
        public const string ValFractalNoise         = "fractalNoise";
        public const string ValGamma                = "gamma";
        public const string ValGeometricPrecision   = "geometricPrecision";
        public const string ValG                    = "G";
        public const string ValHelp                 = "help";
        public const string ValHueRotate            = "hueRotate";
        public const string ValHundredPercent       = "100%";
        public const string ValH                    = "h";
        public const string ValIdentity             = "identity";
        public const string ValInitial              = "initial";
        public const string ValIn                   = "in";
        public const string ValIsolated             = "isolated";
        public const string ValItalic               = "italic";
        public const string ValLighten              = "lighten";
        public const string ValLighter              = "lighter";
        public const string ValLinearRgb            = "linearRGB";
        public const string ValLinear               = "linear";
        public const string ValLuminanceToAlpha     = "luminanceToAlpha";
        public const string ValMagnify              = "magnify";
        public const string ValMatrix               = "matrix";
        public const string ValMedial               = "medial";
        public const string ValMeet                 = "meet";
        public const string ValMiddle               = "middle";
        public const string ValMiter                = "miter";
        public const string ValMove                 = "move";
        public const string ValMultiply             = "multiply";
        public const string ValNew                  = "new";
        public const string ValNeResize             = "ne-resize";
        public const string ValNinety               = "90";
        public const string ValNone                 = "none";
        public const string ValNonZero              = "nonzero";
        public const string ValNormal               = "normal";
        public const string ValNoStitch             = "noStitch";
        public const string ValNwResize             = "nw-resize";
        public const string ValNResize              = "n-resize";
        public const string ValObjectBoundingBox    = "objectBoundingBox";
        public const string ValOblique              = "oblique";
        public const string ValOne                  = "1";
        public const string ValOpaque               = "1";
        public const string ValOptimizeLegibility   = "optimizeLegibility";
        public const string ValOptimizeQuality      = "optimizeQuality";
        public const string ValOptimizeSpeed        = "optimizeSpeed";
        public const string ValOut                  = "out";
        public const string ValOver                 = "over";
        public const string ValPaced                = "paced";
        public const string ValPad                  = "pad";
        public const string ValPerceptual           = "perceptual";
        public const string ValPointer              = "pointer";
        public const string ValPreserve             = "preserve";
        public const string ValReflect              = "reflect";
        public const string ValRelativeColorimetric = "relative-colorimetric";
        public const string ValRepeat               = "repeat";
        public const string ValRound                = "round";
        public const string ValR                    = "R";
        public const string ValSaturate             = "saturate";
        public const string ValSaturation           = "saturation";
        public const string ValScreen               = "screen";
        public const string ValSeResize             = "se-resize";
        public const string ValSlice                = "slice";
        public const string ValSourceAlpha          = "SourceAlpha";
        public const string ValSourceGraphic        = "SourceGraphic";
        public const string ValSpacingAndGlyphs     = "spacingAndGlyphs";
        public const string ValSpacing              = "spacing";
        public const string ValSquare               = "square";
        public const string ValSrgb                 = "sRGB";
        public const string ValStart                = "start";
        public const string ValStitch               = "stitch";
        public const string ValStretch              = "stretch";
        public const string ValStrokePaint          = "StrokePaint";
        public const string ValStrokeWidth          = "strokeWidth";
        public const string ValSwResize             = "sw-resize";
        public const string ValSResize              = "s-resize";
        public const string ValTable                = "table";
        public const string ValTerminal             = "terminal";
        public const string ValText                 = "text";
        public const string ValTranslate            = "translate";
        public const string ValTrue                 = "true";
        public const string ValTurbulence           = "turbulence";
        public const string ValUserSpaceOnUse       = "userSpaceOnUse";
        public const string ValV                    = "v";
        public const string ValWait                 = "wait";
        public const string ValWrap                 = "wrap";
        public const string ValWResize              = "w-resize";
        public const string ValXmaxYmax             = "xMaxYMax";
        public const string ValXmaxYmid             = "xMaxYMid";
        public const string ValXmaxYmin             = "xMaxYMin";
        public const string ValXmidYmax             = "xMidYMax";
        public const string ValXmidYmid             = "xMidYMid";
        public const string ValXmidYmin             = "xMidYMin";
        public const string ValXminYmax             = "xMinYMax";
        public const string ValXminYmid             = "xMinYMid";
        public const string ValXminYmin             = "xMinYMin";
        public const string ValXor                  = "xor";
        public const string ValZeroPercent          = "0%";
        public const string ValZero                 = "0";

        // ---------------------------------------------------------------------
        // Default values for attributes
        // ---------------------------------------------------------------------
        public const string DefCircleCx                             = "0";
        public const string DefCircleCy                             = "0";
        public const string DefClipPathClipPathUnits                = ValUserSpaceOnUse;
        public const string DefComponentTransferFunctionAmplitude   = "1";
        public const string DefComponentTransferFunctionExponent    = "1";
        public const string DefComponentTransferFunctionIntercept   = "0";
        public const string DefComponentTransferFunctionOffset      = "0";
        public const string DefComponentTransferFunctionSlope       = "1";
        public const string DefComponentTransferFunctionTableValues = "";
        public const string DefCursorX                              = "0";
        public const string DefCursorY                              = "0";
        public const string DefEllipseCx                            = "0";
        public const string DefEllipseCy                            = "0";
        public const string DefFeCompositeK1                        = "0";
        public const string DefFeCompositeK2                        = "0";
        public const string DefFeCompositeK3                        = "0";
        public const string DefFeCompositeK4                        = "0";
        public const string DefFeCompositeOperator                  = ValOver;
        public const string DefFeConvolveMatrixEdgeMode             = ValDuplicate;
        public const string DefFeDiffuseLightingDiffuseConstant     = "1";
        public const string DefFeDiffuseLightingSurfaceScale        = "1";
        public const string DefFeDisplacementMapScale               = "0";
        public const string DefFeDistantLightAzimuth                = "0";
        public const string DefFeDistantLightElevation              = "0";
        public const string DefFePointLightX                        = "0";
        public const string DefFePointLightY                        = "0";
        public const string DefFePointLightZ                        = "0";
        public const string DefFeSpecularLightingSpecularConstant   = "1";
        public const string DefFeSpecularLightingSpecularExponent   = "1";
        public const string DefFeSpecularLightingSurfaceScale       = "1";
        public const string DefFeSpotLightLimitingConeAngle         = "90";
        public const string DefFeSpotLightPointsAtX                 = "0";
        public const string DefFeSpotLightPointsAtY                 = "0";
        public const string DefFeSpotLightPointsAtZ                 = "0";
        public const string DefFeSpotLightSpecularExponent          = "1";
        public const string DefFeSpotLightX                         = "0";
        public const string DefFeSpotLightY                         = "0";
        public const string DefFeSpotLightZ                         = "0";
        public const string DefFeTurbulenceNumOctaves               = "1";
        public const string DefFeTurbulenceSeed                     = "0";
        public const string DefFilterFilterUnits                    = ValUserSpaceOnUse;
        public const string DefFilterHeight                         = "120%";
        public const string DefFilterPrimitiveX                     = "0%";
        public const string DefFilterPrimitiveY                     = "0%";
        public const string DefFilterPrimitiveWidth                 = "100%";
        public const string DefFilterPrimitiveHeight                = "100%";
        public const string DefFilterPrimitiveUnits                 = ValUserSpaceOnUse;
        public const string DefFilterWidth                          = "120%";
        public const string DefFilterX                              = "-10%";
        public const string DefFilterY                              = "-10%";
        public const string DefFontFaceFontStretch                  = ValNormal;
        public const string DefFontFaceFontStyle                    = ValAll;
        public const string DefFontFaceFontVariant                  = ValNormal;
        public const string DefFontFaceFontWeight                   = ValAll;
        public const string DefFontFacePanose_1                     = "0 0 0 0 0 0 0 0 0 0";
        public const string DefFontFaceSlope                        = "0";
        public const string DefFontFaceUnitsPerEm                   = "1000";
        public const string DefForeignObjectX                       = "0";
        public const string DefForeignObjectY                       = "0";
        public const string DefHorizOriginX                         = "0";
        public const string DefHorizOriginY                         = "0";
        public const string DefKernK                                = "0";
        public const string DefImageX                               = "0";
        public const string DefImageY                               = "0";
        public const string DefLineX1                               = "0";
        public const string DefLineX2                               = "0";
        public const string DefLineY1                               = "0";
        public const string DefLineY2                               = "0";
        public const string DefLinearGradientX1                     = "0%";
        public const string DefLinearGradientX2                     = "100%";
        public const string DefLinearGradientY1                     = "0%";
        public const string DefLinearGradientY2                     = "0%";
        public const string DefMarkerMarkerHeight                   = "3";
        public const string DefMarkerMarkerUnits                    = ValStrokeWidth;
        public const string DefMarkerMarkerWidth                    = "3";
        public const string DefMarkerOrient                         = "0";
        public const string DefMarkerRefX                           = "0";
        public const string DefMarkerRefY                           = "0";
        public const string DefMaskHeight                           = "120%";
        public const string DefMaskMaskUnits                        = ValUserSpaceOnUse;
        public const string DefMaskWidth                            = "120%";
        public const string DefMaskX                                = "-10%";
        public const string DefMaskY                                = "-10%";
        public const string DefPatternX                             = "0";
        public const string DefPatternY                             = "0";
        public const string DefPatternWidth                         = "0";
        public const string DefPatternHeight                        = "0";
        public const string DefRadialGradientCx                     = "50%";
        public const string DefRadialGradientCy                     = "50%";
        public const string DefRadialGradientR                      = "50%";
        public const string DefRectX                                = "0";
        public const string DefRectY                                = "0";
        public const string DefScriptTypeEcmascript                 = "text/ecmascript";
        public const string DefScriptTypeApplicationEcmascript      = "application/ecmascript";
        public const string DefScriptTypeJavascript                 = "text/javascript";
        public const string DefScriptTypeApplicationJavascript      = "application/javascript";
        public const string DefScriptType                           = DefScriptTypeEcmascript;
        public const string DefScriptTypeJava                       = "application/java-archive";
        public const string DefSvgX                                 = "0";
        public const string DefSvgY                                 = "0";
        public const string DefSvgHeight                            = "100%";
        public const string DefSvgWidth                             = "100%";
        public const string DefTextPathStartOffset                  = "0";
        public const string DefUseX                                 = "0";
        public const string DefUseY                                 = "0";
        public const string DefUseWidth                             = "100%";
        public const string DefUseHeight                            = "100%";

        // ---------------------------------------------------------------------
        // various constants in SVG attributes
        // ---------------------------------------------------------------------
        public const string TransformTranslate   = "translate";
        public const string TransformRotate      = "rotate";
        public const string TransformScale       = "scale";
        public const string TransformSkewx       = "skewX";
        public const string TransformSkewy       = "skewY";
        public const string TransformMatrix      = "matrix";

        public const string PathArc              = "A";
        public const string PathClose            = "Z";
        public const string PathCubicTo          = "C";
        public const string PathMove             = "M";
        public const string PathLineTo           = "L";
        public const string PathVerticalLineTo   = "V";
        public const string PathHorizontalLineTo = "H";
        public const string PathQuadTo           = "Q";
        public const string PathSmoothQuadTo     = "T";

        // ---------------------------------------------------------------------
        // Event constants
        // ---------------------------------------------------------------------
        public const string EventClick     = "click";
        public const string EventKeydown   = "keydown";
        public const string EventKeypress  = "keypress";
        public const string EventKeyup     = "keyup";
        public const string EventMousedown = "mousedown";
        public const string EventMousemove = "mousemove";
        public const string EventMouseout  = "mouseout";
        public const string EventMouseover = "mouseover";
        public const string EventMouseup   = "mouseup";
    }
}
