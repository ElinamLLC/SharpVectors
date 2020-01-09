using System;
using System.Dynamic;
using System.Reflection;using System.Collections.Generic;
using System.Runtime.CompilerServices;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Scripting
{
    public sealed class JsObjectTypeDictionary
    {
        private Dictionary<string, Type> _objectTypes;

        public JsObjectTypeDictionary()
        {
            _objectTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count
        {
            get {
                return _objectTypes.Count;
            }
        }

        public Type this[string key]
        {            get {
                if (_objectTypes.ContainsKey(key))
                {
                    return _objectTypes[key];
                }
                return null;
            }            set {
                _objectTypes[key] = value;
            }        }

        public object CreateInstance(string key, object[] args, BindingFlags flags)        {            if (!_objectTypes.ContainsKey(key))
            {
                return null;
            }            Type type = _objectTypes[key];
            return type.Assembly.CreateInstance(type.FullName, false, flags, null, args, null, new object[0]);        }        public object CreateInstance(string key, object[] args)        {            return this.CreateInstance(key, args, BindingFlags.Default);        }
    }    public sealed class JsObjectReferenceCache : ISvgScriptReferenceCache
    {
        private readonly ConditionalWeakTable<object, IScriptableObject> _jsCachedTypes;

        public JsObjectReferenceCache()
        {
            _jsCachedTypes = new ConditionalWeakTable<object, IScriptableObject>();
        }

        public void Add(object key, IScriptableObject value)
        {
            _jsCachedTypes.Add(key, value);
        }

        public bool Remove(object key)
        {
            return _jsCachedTypes.Remove(key);
        }

        public bool TryGetValue(object key, out IScriptableObject value)
        {
            return _jsCachedTypes.TryGetValue(key, out value);
        }
    }    public abstract class JsObject : DynamicObject
    {
        protected bool _isDisposed; // To detect redundant calls

        protected readonly static object[] _wrapperArgs = new object[2];

        // The table of key types to wrappable type values
        private static JsObjectTypeDictionary _jsMappedTypes;

        protected JsObject()
        {
        }

        protected abstract object WrappedObject { get; }

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        /// <summary>
        /// Creates a new scriptable wrapper for the specified object. Use this function instead
        /// of calling the JsObject constructor.
        /// </summary>
        /// <param name="wrappableObject">The object to wrap. It's type will be used as a key
        /// in a lookup for creating the correct wrappable type.</param>
        /// <returns>A new scriptable object that has been created with a type corresponding
        /// to the wrappableObject's type.</returns>
        public static J CreateWrapper<J>(object wrappableObject, ISvgScriptEngine scriptEngine) 
            where J : class, IScriptableObject
        {
            // return null if we get null
            if (wrappableObject == null || scriptEngine == null)
            {
                return null;
            }
            if (scriptEngine.ReferenceCache == null)
            {
                scriptEngine.ReferenceCache = new JsObjectReferenceCache();
            }

            var jsRefCache = scriptEngine.ReferenceCache;

            // Check that the static table is built
            if (_jsMappedTypes == null || _jsMappedTypes.Count == 0)
            {
                InitializeWrapperTypes();
            }

            // Do we already have a wrapper for this object?
            J jsObj = null;
            IScriptableObject jsObjBase = null;            
            if (jsRefCache.TryGetValue(wrappableObject, out jsObjBase) && jsObjBase != null)
            {
                jsObj = jsObjBase as J;
                if (jsObj != null)
                    return jsObj;
            }

            // Return a new instance
            try
            {
                _wrapperArgs[0] = wrappableObject;
                _wrapperArgs[1] = scriptEngine;
                // Normal
                try
                {
                    jsObjBase = (IScriptableObject)_jsMappedTypes.CreateInstance(wrappableObject.GetType().Name, _wrapperArgs);
                    jsRefCache.Add(wrappableObject, jsObjBase);
                    return jsObjBase as J;
                }
                catch (Exception)
                {
                    // Try the ancestor
                    jsObjBase = (IScriptableObject)_jsMappedTypes.CreateInstance(wrappableObject.GetType().BaseType.Name, _wrapperArgs);
                    jsRefCache.Add(wrappableObject, jsObjBase);
                    return jsObjBase as J;
                }
            }
            catch (Exception e)
            {
                throw new SvgException(SvgExceptionType.SvgWrongTypeErr,
                    "Could not create wrappable type for " + wrappableObject.GetType().FullName, e);
            }

        }

        /// <summary>
        /// Removes the wrapper and base object key value pair. This function is called by the JsObject destructor.
        /// </summary>
        /// <param name="wrappableObject">The key object that was wrapped, to be removed.</param>
        public static void RemoveWrapper(object wrappableObject, ISvgScriptEngine scriptEngine)
        {
            // return null if we get null
            if (wrappableObject == null || scriptEngine == null)
            {
                return;
            }
            if (scriptEngine.ReferenceCache == null)
            {
                scriptEngine.ReferenceCache = new JsObjectReferenceCache();
            }

            var jsRefCache = scriptEngine.ReferenceCache;

            // Remove it
            jsRefCache.Remove(wrappableObject.GetType().Name);
        }

        /// <summary>
        /// Builds the wrapper type table for static lookups
        /// </summary>
        private static void InitializeWrapperTypes()
        {
            _jsMappedTypes = new JsObjectTypeDictionary();

            // CSS Types
            _jsMappedTypes[typeof(CssRuleList).Name] = typeof(JsCssRuleList);
            _jsMappedTypes[typeof(CssRule).Name] = typeof(JsCssRule);
            _jsMappedTypes[typeof(CssStyleRule).Name] = typeof(JsCssStyleRule);
            _jsMappedTypes[typeof(CssMediaRule).Name] = typeof(JsCssMediaRule);
            _jsMappedTypes[typeof(CssFontFaceRule).Name] = typeof(JsCssFontFaceRule);
            _jsMappedTypes[typeof(CssPageRule).Name] = typeof(JsCssPageRule);
            _jsMappedTypes[typeof(CssImportRule).Name] = typeof(JsCssImportRule);
            _jsMappedTypes[typeof(CssCharsetRule).Name] = typeof(JsCssCharsetRule);
            _jsMappedTypes[typeof(CssUnknownRule).Name] = typeof(JsCssUnknownRule);
            _jsMappedTypes[typeof(CssStyleDeclaration).Name] = typeof(JsCssStyleDeclaration);
            _jsMappedTypes[typeof(CssValue).Name] = typeof(JsCssValue);
            _jsMappedTypes[typeof(CssPrimitiveValue).Name] = typeof(JsCssPrimitiveValue);
            //TODO _jsMappedTypes[typeof(CssValueList).Name] = typeof(JsCssValueList);
            _jsMappedTypes[typeof(ICssColor).Name] = typeof(JsRgbColor);
            _jsMappedTypes[typeof(ICssRect).Name] = typeof(JsRect);
            //TODO _jsMappedTypes[typeof(Counter).Name] = typeof(JsCounter);
            //TODO _jsMappedTypes[typeof(ElementCssInlineStyle).Name] = typeof(JsElementCssInlineStyle);
            //TODO _jsMappedTypes[typeof(Css2Properties).Name] = typeof(JsCss2Properties);
            _jsMappedTypes[typeof(CssStyleSheet).Name] = typeof(JsCssStyleSheet);
            //TODO _jsMappedTypes[typeof(ViewCss).Name] = typeof(JsViewCss);
            //TODO _jsMappedTypes[typeof(DocumentCss).Name] = typeof(JsDocumentCss);
            //TODO _jsMappedTypes[typeof(DomImplementationCss).Name] = typeof(JsDomImplementationCss);

            // DOM Types
            _jsMappedTypes[typeof(Dom.DomImplementation).Name] = typeof(JsDomImplementation);
            _jsMappedTypes[typeof(System.Xml.XmlNode).Name] = typeof(JsNode);
            _jsMappedTypes[typeof(Dom.NodeListAdapter).Name] = typeof(JsNodeList);
            _jsMappedTypes[typeof(System.Xml.XmlNodeList).Name] = typeof(JsNodeList);
            _jsMappedTypes[typeof(Dom.NodeListAdapter).Name] = typeof(JsNodeList);
            _jsMappedTypes[typeof(System.Xml.XmlNamedNodeMap).Name] = typeof(JsNamedNodeMap);
            _jsMappedTypes[typeof(System.Xml.XmlCDataSection).Name] = typeof(JsCharacterData);
            _jsMappedTypes[typeof(Dom.Attribute).Name] = typeof(JsAttr);
            _jsMappedTypes[typeof(Dom.Element).Name] = typeof(JsElement);
            _jsMappedTypes[typeof(Dom.Text).Name] = typeof(JsText);
            _jsMappedTypes[typeof(Dom.Comment).Name] = typeof(JsComment);
            _jsMappedTypes[typeof(Dom.CDataSection).Name] = typeof(JsCDataSection);
            _jsMappedTypes[typeof(Dom.DocumentType).Name] = typeof(JsDocumentType);
            _jsMappedTypes[typeof(System.Xml.XmlNotation).Name] = typeof(JsNotation);
            _jsMappedTypes[typeof(System.Xml.XmlEntity).Name] = typeof(JsEntity);
            _jsMappedTypes[typeof(Dom.EntityReference).Name] = typeof(JsEntityReference);
            _jsMappedTypes[typeof(Dom.ProcessingInstruction).Name] = typeof(JsProcessingInstruction);
            _jsMappedTypes[typeof(Dom.DocumentFragment).Name] = typeof(JsDocumentFragment);
            _jsMappedTypes[typeof(Dom.Document).Name] = typeof(JsDocument);

            // Events Types
            _jsMappedTypes[typeof(EventTarget).Name] = typeof(JsEventTarget);
            //TODO _jsMappedTypes[typeof(EventListener).Name] = typeof(JsEventListener);
            _jsMappedTypes[typeof(Event).Name] = typeof(JsEvent);
            //TODO, I think this is handled under Document: _jsMappedTypes[typeof(DocumentEvent).Name] = typeof(JsDocumentEvent);
            _jsMappedTypes[typeof(UiEvent).Name] = typeof(JsUiEvent);
            _jsMappedTypes[typeof(MouseEvent).Name] = typeof(JsMouseEvent);
            _jsMappedTypes[typeof(MutationEvent).Name] = typeof(JsMutationEvent);
            _jsMappedTypes[typeof(IUiEvent).Name] = typeof(JsUiEvent);
            _jsMappedTypes[typeof(IMouseEvent).Name] = typeof(JsMouseEvent);
            _jsMappedTypes[typeof(IMutationEvent).Name] = typeof(JsMutationEvent);

            // SMIL Types
            //TODO _jsMappedTypes[typeof(ElementTimeControl).Name] = typeof(JsElementTimeControl);
            //TODO _jsMappedTypes[typeof(TimeEvent).Name] = typeof(JsTimeEvent);

            // StyleSheets Types
            _jsMappedTypes[typeof(StyleSheet).Name] = typeof(JsStyleSheet);
            _jsMappedTypes[typeof(StyleSheetList).Name] = typeof(JsStyleSheetList);
            _jsMappedTypes[typeof(MediaList).Name] = typeof(JsMediaList);
            //TODO _jsMappedTypes[typeof(LinkStyle).Name] = typeof(JsLinkStyle);
            //TODO _jsMappedTypes[typeof(DocumentStyle).Name] = typeof(JsDocumentStyle);

            // SVG Types
            _jsMappedTypes[typeof(SvgElement).Name] = typeof(JsSvgElement);
            _jsMappedTypes[typeof(SvgAnimatedBoolean).Name] = typeof(JsSvgAnimatedBoolean);
            _jsMappedTypes[typeof(SvgAnimatedString).Name] = typeof(JsSvgAnimatedString);
            _jsMappedTypes[typeof(SvgStringList).Name] = typeof(JsSvgStringList);
            _jsMappedTypes[typeof(SvgAnimatedEnumeration).Name] = typeof(JsSvgAnimatedEnumeration);
            _jsMappedTypes[typeof(SvgAnimatedInteger).Name] = typeof(JsSvgAnimatedInteger);
            _jsMappedTypes[typeof(SvgNumber).Name] = typeof(JsSvgNumber);
            _jsMappedTypes[typeof(SvgAnimatedNumber).Name] = typeof(JsSvgAnimatedNumber);
            _jsMappedTypes[typeof(SvgNumberList).Name] = typeof(JsSvgNumberList);
            _jsMappedTypes[typeof(SvgAnimatedNumberList).Name] = typeof(JsSvgAnimatedNumberList);
            _jsMappedTypes[typeof(SvgLength).Name] = typeof(JsSvgLength);
            _jsMappedTypes[typeof(SvgAnimatedLength).Name] = typeof(JsSvgAnimatedLength);
            _jsMappedTypes[typeof(SvgLengthList).Name] = typeof(JsSvgLengthList);
            _jsMappedTypes[typeof(SvgAnimatedLengthList).Name] = typeof(JsSvgAnimatedLengthList);
            _jsMappedTypes[typeof(SvgAngle).Name] = typeof(JsSvgAngle);
            _jsMappedTypes[typeof(SvgAnimatedAngle).Name] = typeof(JsSvgAnimatedAngle);
            _jsMappedTypes[typeof(SvgColor).Name] = typeof(JsSvgColor);
            //TODO _jsMappedTypes[typeof(SvgIccColor).Name] = typeof(JsSvgIccColor);
            _jsMappedTypes[typeof(SvgRect).Name] = typeof(JsSvgRect);
            _jsMappedTypes[typeof(SvgAnimatedRect).Name] = typeof(JsSvgAnimatedRect);
            //No Type information _jsMappedTypes[typeof(SvgUnitTypes).Name] = typeof(JsSvgUnitTypes);
            _jsMappedTypes[typeof(SvgStyleableElement).Name] = typeof(JsSvgStylable);
            //TODO _jsMappedTypes[typeof(SvgLocatable).Name] = typeof(JsSvgLocatable);
            _jsMappedTypes[typeof(SvgTransformableElement).Name] = typeof(JsSvgTransformable);
            _jsMappedTypes[typeof(SvgTests).Name] = typeof(JsSvgTests);
            //TODO _jsMappedTypes[typeof(SvgLangSpace).Name] = typeof(JsSvgLangSpace);
            _jsMappedTypes[typeof(SvgExternalResourcesRequired).Name] = typeof(JsSvgExternalResourcesRequired);
            _jsMappedTypes[typeof(SvgFitToViewBox).Name] = typeof(JsSvgFitToViewBox);
            _jsMappedTypes[typeof(SvgZoomAndPan).Name] = typeof(JsSvgZoomAndPan);
            _jsMappedTypes[typeof(SvgViewSpec).Name] = typeof(JsSvgViewSpec);
            _jsMappedTypes[typeof(SvgUriReference).Name] = typeof(JsSvgUriReference);
            //No Type information _jsMappedTypes[typeof(SvgCssRule).Name] = typeof(JsSvgCssRule);
            //No Type information _jsMappedTypes[typeof(SvgRenderingIntent).Name] = typeof(JsSvgRenderingIntent);
            _jsMappedTypes[typeof(SvgDocument).Name] = typeof(JsSvgDocument);
            _jsMappedTypes[typeof(SvgSvgElement).Name] = typeof(JsSvgSvgElement);
            _jsMappedTypes[typeof(SvgGElement).Name] = typeof(JsSvgGElement);
            _jsMappedTypes[typeof(SvgDefsElement).Name] = typeof(JsSvgDefsElement);
            _jsMappedTypes[typeof(SvgDescElement).Name] = typeof(JsSvgDescElement);
            _jsMappedTypes[typeof(SvgTitleElement).Name] = typeof(JsSvgTitleElement);
            _jsMappedTypes[typeof(SvgSymbolElement).Name] = typeof(JsSvgSymbolElement);
            _jsMappedTypes[typeof(SvgUseElement).Name] = typeof(JsSvgUseElement);
            _jsMappedTypes[typeof(SvgElementInstance).Name] = typeof(JsSvgElementInstance);
            _jsMappedTypes[typeof(SvgElementInstanceList).Name] = typeof(JsSvgElementInstanceList);
            _jsMappedTypes[typeof(SvgImageElement).Name] = typeof(JsSvgImageElement);
            _jsMappedTypes[typeof(SvgSwitchElement).Name] = typeof(JsSvgSwitchElement);
            //TODO _jsMappedTypes[typeof(GetSvgDocument).Name] = typeof(JsGetSvgDocument);
            _jsMappedTypes[typeof(SvgStyleElement).Name] = typeof(JsSvgStyleElement);
            _jsMappedTypes[typeof(SvgPoint).Name] = typeof(JsSvgPoint);
            _jsMappedTypes[typeof(SvgPointList).Name] = typeof(JsSvgPointList);
            _jsMappedTypes[typeof(SvgMatrix).Name] = typeof(JsSvgMatrix);
            _jsMappedTypes[typeof(SvgTransform).Name] = typeof(JsSvgTransform);
            _jsMappedTypes[typeof(SvgTransformList).Name] = typeof(JsSvgTransformList);
            _jsMappedTypes[typeof(SvgAnimatedTransformList).Name] = typeof(JsSvgAnimatedTransformList);
            _jsMappedTypes[typeof(SvgPreserveAspectRatio).Name] = typeof(JsSvgPreserveAspectRatio);
            _jsMappedTypes[typeof(SvgAnimatedPreserveAspectRatio).Name] = typeof(JsSvgAnimatedPreserveAspectRatio);
            _jsMappedTypes[typeof(SvgPathSeg).Name] = typeof(JsSvgPathSeg);
            _jsMappedTypes[typeof(SvgPathSegClosePath).Name] = typeof(JsSvgPathSegClosePath);
            _jsMappedTypes[typeof(SvgPathSegMovetoAbs).Name] = typeof(JsSvgPathSegMovetoAbs);
            _jsMappedTypes[typeof(SvgPathSegMovetoRel).Name] = typeof(JsSvgPathSegMovetoRel);
            _jsMappedTypes[typeof(SvgPathSegLinetoAbs).Name] = typeof(JsSvgPathSegLinetoAbs);
            _jsMappedTypes[typeof(SvgPathSegLinetoRel).Name] = typeof(JsSvgPathSegLinetoRel);
            _jsMappedTypes[typeof(SvgPathSegCurvetoCubicAbs).Name] = typeof(JsSvgPathSegCurvetoCubicAbs);
            _jsMappedTypes[typeof(SvgPathSegCurvetoCubicRel).Name] = typeof(JsSvgPathSegCurvetoCubicRel);
            _jsMappedTypes[typeof(SvgPathSegCurvetoQuadraticAbs).Name] = typeof(JsSvgPathSegCurvetoQuadraticAbs);
            _jsMappedTypes[typeof(SvgPathSegCurvetoQuadraticRel).Name] = typeof(JsSvgPathSegCurvetoQuadraticRel);
            _jsMappedTypes[typeof(SvgPathSegArcAbs).Name] = typeof(JsSvgPathSegArcAbs);
            _jsMappedTypes[typeof(SvgPathSegArcRel).Name] = typeof(JsSvgPathSegArcRel);
            _jsMappedTypes[typeof(SvgPathSegLinetoHorizontalAbs).Name] = typeof(JsSvgPathSegLinetoHorizontalAbs);
            _jsMappedTypes[typeof(SvgPathSegLinetoHorizontalRel).Name] = typeof(JsSvgPathSegLinetoHorizontalRel);
            _jsMappedTypes[typeof(SvgPathSegLinetoVerticalAbs).Name] = typeof(JsSvgPathSegLinetoVerticalAbs);
            _jsMappedTypes[typeof(SvgPathSegLinetoVerticalRel).Name] = typeof(JsSvgPathSegLinetoVerticalRel);
            _jsMappedTypes[typeof(SvgPathSegCurvetoCubicSmoothAbs).Name] = typeof(JsSvgPathSegCurvetoCubicSmoothAbs);
            _jsMappedTypes[typeof(SvgPathSegCurvetoCubicSmoothRel).Name] = typeof(JsSvgPathSegCurvetoCubicSmoothRel);
            _jsMappedTypes[typeof(SvgPathSegCurvetoQuadraticSmoothAbs).Name] = typeof(JsSvgPathSegCurvetoQuadraticSmoothAbs);
            _jsMappedTypes[typeof(SvgPathSegCurvetoQuadraticSmoothRel).Name] = typeof(JsSvgPathSegCurvetoQuadraticSmoothRel);
            _jsMappedTypes[typeof(SvgPathSegList).Name] = typeof(JsSvgPathSegList);
            //TODO _jsMappedTypes[typeof(SvgAnimatedPathData).Name] = typeof(JsSvgAnimatedPathData);
            _jsMappedTypes[typeof(SvgPathElement).Name] = typeof(JsSvgPathElement);
            _jsMappedTypes[typeof(SvgRectElement).Name] = typeof(JsSvgRectElement);
            _jsMappedTypes[typeof(SvgCircleElement).Name] = typeof(JsSvgCircleElement);
            _jsMappedTypes[typeof(SvgEllipseElement).Name] = typeof(JsSvgEllipseElement);
            _jsMappedTypes[typeof(SvgLineElement).Name] = typeof(JsSvgLineElement);
            //TODO _jsMappedTypes[typeof(SvgAnimatedPoints).Name] = typeof(JsSvgAnimatedPoints);
            _jsMappedTypes[typeof(SvgPolylineElement).Name] = typeof(JsSvgPolylineElement);
            _jsMappedTypes[typeof(SvgPolygonElement).Name] = typeof(JsSvgPolygonElement);
            _jsMappedTypes[typeof(SvgTextContentElement).Name] = typeof(JsSvgTextContentElement);
            _jsMappedTypes[typeof(SvgTextPositioningElement).Name] = typeof(JsSvgTextPositioningElement);
            _jsMappedTypes[typeof(SvgTextElement).Name] = typeof(JsSvgTextElement);
            _jsMappedTypes[typeof(SvgTSpanElement).Name] = typeof(JsSvgTSpanElement);
            _jsMappedTypes[typeof(SvgTRefElement).Name] = typeof(JsSvgTRefElement);
            _jsMappedTypes[typeof(SvgTextPathElement).Name] = typeof(JsSvgTextPathElement);
            _jsMappedTypes[typeof(SvgAltGlyphElement).Name] = typeof(JsSvgAltGlyphElement);
            _jsMappedTypes[typeof(SvgAltGlyphDefElement).Name] = typeof(JsSvgAltGlyphDefElement);
            _jsMappedTypes[typeof(SvgAltGlyphItemElement).Name] = typeof(JsSvgAltGlyphItemElement);
            _jsMappedTypes[typeof(SvgGlyphRefElement).Name] = typeof(JsSvgGlyphRefElement);
            _jsMappedTypes[typeof(SvgPaint).Name] = typeof(JsSvgPaint);
            _jsMappedTypes[typeof(SvgMarkerElement).Name] = typeof(JsSvgMarkerElement);
            //TODO _jsMappedTypes[typeof(SvgColorProfileElement).Name] = typeof(JsSvgColorProfileElement);
            //TODO _jsMappedTypes[typeof(SvgColorProfileRule).Name] = typeof(JsSvgColorProfileRule);
            _jsMappedTypes[typeof(SvgGradientElement).Name] = typeof(JsSvgGradientElement);
            _jsMappedTypes[typeof(SvgLinearGradientElement).Name] = typeof(JsSvgLinearGradientElement);
            _jsMappedTypes[typeof(SvgRadialGradientElement).Name] = typeof(JsSvgRadialGradientElement);
            _jsMappedTypes[typeof(SvgStopElement).Name] = typeof(JsSvgStopElement);
            _jsMappedTypes[typeof(SvgPatternElement).Name] = typeof(JsSvgPatternElement);
            _jsMappedTypes[typeof(SvgClipPathElement).Name] = typeof(JsSvgClipPathElement);
            _jsMappedTypes[typeof(SvgMaskElement).Name] = typeof(JsSvgMaskElement);
            _jsMappedTypes[typeof(SvgFilterElement).Name] = typeof(JsSvgFilterElement);
            _jsMappedTypes[typeof(SvgFilterPrimitiveStandardAttributes).Name] = typeof(JsSvgFilterPrimitiveStandardAttributes);
            _jsMappedTypes[typeof(SvgFEBlendElement).Name] = typeof(JsSvgFEBlendElement);
            _jsMappedTypes[typeof(SvgFEColorMatrixElement).Name] = typeof(JsSvgFEColorMatrixElement);
            _jsMappedTypes[typeof(SvgFEComponentTransferElement).Name] = typeof(JsSvgFEComponentTransferElement);
            _jsMappedTypes[typeof(SvgComponentTransferFunctionElement).Name] = typeof(JsSvgComponentTransferFunctionElement);
            _jsMappedTypes[typeof(SvgFEFuncRElement).Name] = typeof(JsSvgFEFuncRElement);
            _jsMappedTypes[typeof(SvgFEFuncGElement).Name] = typeof(JsSvgFEFuncGElement);
            _jsMappedTypes[typeof(SvgFEFuncBElement).Name] = typeof(JsSvgFEFuncBElement);
            _jsMappedTypes[typeof(SvgFEFuncAElement).Name] = typeof(JsSvgFEFuncAElement);
            _jsMappedTypes[typeof(SvgFECompositeElement).Name] = typeof(JsSvgFECompositeElement);
            _jsMappedTypes[typeof(SvgFEConvolveMatrixElement).Name] = typeof(JsSvgFEConvolveMatrixElement);
            _jsMappedTypes[typeof(SvgFEDiffuseLightingElement).Name] = typeof(JsSvgFEDiffuseLightingElement);
            _jsMappedTypes[typeof(SvgFEDistantLightElement).Name] = typeof(JsSvgFEDistantLightElement);
            _jsMappedTypes[typeof(SvgFEPointLightElement).Name] = typeof(JsSvgFEPointLightElement);
            _jsMappedTypes[typeof(SvgFESpotLightElement).Name] = typeof(JsSvgFESpotLightElement);
            _jsMappedTypes[typeof(SvgFEDisplacementMapElement).Name] = typeof(JsSvgFEDisplacementMapElement);
            _jsMappedTypes[typeof(SvgFEFloodElement).Name] = typeof(JsSvgFEFloodElement);
            _jsMappedTypes[typeof(SvgFEGaussianBlurElement).Name] = typeof(JsSvgFEGaussianBlurElement);
            _jsMappedTypes[typeof(SvgFEImageElement).Name] = typeof(JsSvgFEImageElement);
            _jsMappedTypes[typeof(SvgFEMergeElement).Name] = typeof(JsSvgFEMergeElement);
            _jsMappedTypes[typeof(SvgFEMergeNodeElement).Name] = typeof(JsSvgFEMergeNodeElement);
            _jsMappedTypes[typeof(SvgFEMorphologyElement).Name] = typeof(JsSvgFEMorphologyElement);
            _jsMappedTypes[typeof(SvgFEOffsetElement).Name] = typeof(JsSvgFEOffsetElement);
            _jsMappedTypes[typeof(SvgFESpecularLightingElement).Name] = typeof(JsSvgFESpecularLightingElement);
            _jsMappedTypes[typeof(SvgFETileElement).Name] = typeof(JsSvgFETileElement);
            _jsMappedTypes[typeof(SvgFETurbulenceElement).Name] = typeof(JsSvgFETurbulenceElement);
            //TODO _jsMappedTypes[typeof(SvgCursorElement).Name] = typeof(JsSvgCursorElement);
            _jsMappedTypes[typeof(SvgAElement).Name] = typeof(JsSvgAElement);
            //TODO _jsMappedTypes[typeof(SvgViewElement).Name] = typeof(JsSvgViewElement);
            _jsMappedTypes[typeof(SvgScriptElement).Name] = typeof(JsSvgScriptElement);
            //TODO _jsMappedTypes[typeof(SvgEvent).Name] = typeof(JsSvgEvent);
            //TODO _jsMappedTypes[typeof(SvgZoomEvent).Name] = typeof(JsSvgZoomEvent);
            _jsMappedTypes[typeof(SvgAnimationElement).Name] = typeof(JsSvgAnimationElement);
            _jsMappedTypes[typeof(SvgAnimateElement).Name] = typeof(JsSvgAnimateElement);
            _jsMappedTypes[typeof(SvgAnimateSetElement).Name] = typeof(JsSvgSetElement);
            _jsMappedTypes[typeof(SvgAnimateMotionElement).Name] = typeof(JsSvgAnimateMotionElement);
            _jsMappedTypes[typeof(SvgAnimateMPathElement).Name] = typeof(JsSvgMPathElement);
            _jsMappedTypes[typeof(SvgAnimateColorElement).Name] = typeof(JsSvgAnimateColorElement);
            _jsMappedTypes[typeof(SvgAnimateTransformElement).Name] = typeof(JsSvgAnimateTransformElement);
            _jsMappedTypes[typeof(SvgFontElement).Name] = typeof(JsSvgFontElement);
            _jsMappedTypes[typeof(SvgGlyphElement).Name] = typeof(JsSvgGlyphElement);
            _jsMappedTypes[typeof(SvgMissingGlyphElement).Name] = typeof(JsSvgMissingGlyphElement);
            _jsMappedTypes[typeof(SvgHKernElement).Name] = typeof(JsSvgHKernElement);
            _jsMappedTypes[typeof(SvgVKernElement).Name] = typeof(JsSvgVKernElement);
            _jsMappedTypes[typeof(SvgFontFaceElement).Name] = typeof(JsSvgFontFaceElement);
            _jsMappedTypes[typeof(SvgFontFaceSrcElement).Name] = typeof(JsSvgFontFaceSrcElement);
            _jsMappedTypes[typeof(SvgFontFaceUriElement).Name] = typeof(JsSvgFontFaceUriElement);
            _jsMappedTypes[typeof(SvgFontFaceFormatElement).Name] = typeof(JsSvgFontFaceFormatElement);
            _jsMappedTypes[typeof(SvgFontFaceNameElement).Name] = typeof(JsSvgFontFaceNameElement);
            //TODO _jsMappedTypes[typeof(SvgDefinitionSrcElement).Name] = typeof(JsSvgDefinitionSrcElement);
            _jsMappedTypes[typeof(SvgMetadataElement).Name] = typeof(JsSvgMetadataElement);
            //TODO _jsMappedTypes[typeof(SvgForeignObjectElement).Name] = typeof(JsSvgForeignObjectElement);

            // Views Types
            //TODO _jsMappedTypes[typeof(AbstractView).Name] = typeof(JsAbstractView);
            //TODO _jsMappedTypes[typeof(DocumentView).Name] = typeof(JsDocumentView);

            // Window Types
            _jsMappedTypes[typeof(SvgWindow).Name] = typeof(JsSvgWindow);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is JsObject && this.WrappedObject == ((JsObject)obj).WrappedObject;
        }

        public override int GetHashCode()
        {
            if (this.WrappedObject == null)
                return 0;

            return this.WrappedObject.GetHashCode();
        }

        public static bool operator ==(JsObject x, JsObject y)
        {
            if ((object)x == null || (object)y == null)
                return false;

            return x.WrappedObject == y.WrappedObject;
        }

        public static bool operator !=(JsObject x, JsObject y)
        {
            return !(x == y);
        }

    }
    /// <summary>
    /// Base class for all wrappers
    /// </summary>
    public abstract class JsObject<T> : JsObject, IDisposable where T : class
    {
        protected T _baseObject;
        protected ISvgScriptEngine _engine;

        /// <summary>
        /// Base constructor, if used you must assign the baseObject after the call.
        /// This constructor should only be called internally. Higher order classes should construct
        /// new wrappers using the CreateWrapper function to allow for type lookups.
        /// </summary>
        protected JsObject()
        {
        }

        /// <summary>
        /// Base constructor, accepts the baseObject that will be wrapped with all inherited calls. 
        /// This constructor should only be called internally. Higher order classes should construct
        /// new wrappers using the CreateWrapper function to allow for type lookups.
        /// </summary>
        /// <param name="baseObject">The object to wrap with a scriptable object.</param>
        protected JsObject(T baseObject, ISvgScriptEngine engine)
        {
            _engine     = engine;
            _baseObject = baseObject;
        }

        ~JsObject()
        {
            this.Dispose(false);
        }

        public T BaseObject
        {
            get {
                return _baseObject;
            }
        }

        protected override object WrappedObject
        {
            get {
                return _baseObject;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // We need to remove the item from the global hash table
            if (_baseObject != null)
            {
                RemoveWrapper(_baseObject, _engine);
            }

            _baseObject = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
