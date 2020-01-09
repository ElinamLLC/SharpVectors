using System;
using System.Xml;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Events;
using SharpVectors.Dom.Stylesheets;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Views;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgElement

    /// <summary>
    /// Implementation wrapper for IJsSvgElement
    /// </summary>
    public class JsSvgElement : JsElement, IJsSvgElement
    {
        public JsSvgElement(ISvgElement baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string id
        {
            get { return ((ISvgElement)_baseObject).Id; }
            set { ((ISvgElement)_baseObject).Id = value; }
        }

        public string xmlbase
        {
            get { return ((XmlElement)_baseObject).BaseURI; }
            set { ((XmlElement)_baseObject).SetAttribute("xml:base", value); }
        }

        public IJsSvgSvgElement ownerSVGElement
        {
            get {
                var wrappedValue = ((ISvgElement)_baseObject).OwnerSvgElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement viewportElement
        {
            get {
                var wrappedValue = ((ISvgElement)_baseObject).ViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgStylable

    /// <summary>
    /// Implementation wrapper for IJsSvgStylable
    /// </summary>
    public class JsSvgStylable : JsObject<ISvgStylable>, IJsSvgStylable
    {
        public JsSvgStylable(ISvgStylable baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = _baseObject.GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = _baseObject.ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = _baseObject.Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgLocatable

    /// <summary>
    /// Implementation wrapper for IJsSvgLocatable
    /// </summary>
    public class JsSvgLocatable : JsObject<ISvgLocatable>, IJsSvgLocatable
    {
        public JsSvgLocatable(ISvgLocatable baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = _baseObject.GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = _baseObject.GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = _baseObject.GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = _baseObject.GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = _baseObject.NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = _baseObject.FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgTransformable

    /// <summary>
    /// Implementation wrapper for IJsSvgTransformable
    /// </summary>
    public class JsSvgTransformable : JsSvgLocatable, IJsSvgTransformable
    {
        public JsSvgTransformable(ISvgTransformable baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgTests

    /// <summary>
    /// Implementation wrapper for IJsSvgTests
    /// </summary>
    public class JsSvgTests : JsObject<ISvgTests>, IJsSvgTests
    {
        public JsSvgTests(ISvgTests baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public bool hasExtension(string extension)
        {
            return _baseObject.HasExtension(extension);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = _baseObject.RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = _baseObject.RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = _baseObject.SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgLangSpace

    /// <summary>
    /// Implementation wrapper for IJsSvgLangSpace
    /// </summary>
    public class JsSvgLangSpace : JsObject<ISvgLangSpace>, IJsSvgLangSpace
    {
        public JsSvgLangSpace(ISvgLangSpace baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string xmllang
        {
            get { return _baseObject.XmlLang; }
            set { _baseObject.XmlLang = value; }
        }

        public string xmlspace
        {
            get { return _baseObject.XmlSpace; }
            set { _baseObject.XmlSpace = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgExternalResourcesRequired

    /// <summary>
    /// Implementation wrapper for IJsSvgExternalResourcesRequired
    /// </summary>
    public class JsSvgExternalResourcesRequired : JsObject<ISvgExternalResourcesRequired>, IJsSvgExternalResourcesRequired
    {
        public JsSvgExternalResourcesRequired(ISvgExternalResourcesRequired baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool externalResourcesRequired
        {
            get { return _baseObject.ExternalResourcesRequired.BaseVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgFitToViewBox

    /// <summary>
    /// Implementation wrapper for IJsSvgFitToViewBox
    /// </summary>
    public class JsSvgFitToViewBox : JsObject<ISvgFitToViewBox>, IJsSvgFitToViewBox
    {
        public JsSvgFitToViewBox(ISvgFitToViewBox baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedRect viewBox
        {
            get {
                var wrappedValue = _baseObject.ViewBox;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = _baseObject.PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgZoomAndPan

    /// <summary>
    /// Implementation wrapper for IJsSvgZoomAndPan
    /// </summary>
    public class JsSvgZoomAndPan : JsObject<ISvgZoomAndPan>, IJsSvgZoomAndPan
    {
        public JsSvgZoomAndPan(ISvgZoomAndPan baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ushort zoomAndPan
        {
            get { return (ushort)_baseObject.ZoomAndPan; }
            set { _baseObject.ZoomAndPan = (SvgZoomAndPanType)value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgViewSpec

    /// <summary>
    /// Implementation wrapper for IJsSvgViewSpec
    /// </summary>
    public class JsSvgViewSpec : JsSvgFitToViewBox, IJsSvgViewSpec
    {
        public JsSvgViewSpec(ISvgViewSpec baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ushort zoomAndPan
        {
            get { return (ushort)((ISvgZoomAndPan)this.BaseObject).ZoomAndPan; }
            set { ((ISvgZoomAndPan)this.BaseObject).ZoomAndPan = (SvgZoomAndPanType)value; }
        }

        public IJsSvgTransformList transform
        {
            get {
                var wrappedValue = ((ISvgViewSpec)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgTransformList>(wrappedValue, _engine) : null; }
        }

        public IJsSvgElement viewTarget
        {
            get {
                var wrappedValue = ((ISvgViewSpec)_baseObject).ViewTarget;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null; }
        }

        public string viewBoxString
        {
            get { return ((ISvgViewSpec)_baseObject).ViewBoxString; }
        }

        public string preserveAspectRatioString
        {
            get { return ((ISvgViewSpec)_baseObject).PreserveAspectRatioString; }
        }

        public string transformString
        {
            get { return ((ISvgViewSpec)_baseObject).TransformString; }
        }

        public string viewTargetString
        {
            get { return ((ISvgViewSpec)_baseObject).ViewTargetString; }
        }

        ISvgZoomAndPan IScriptableObject<ISvgZoomAndPan>.BaseObject
        {
            get {
                return (ISvgZoomAndPan)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgUriReference

    /// <summary>
    /// Implementation wrapper for IJsSvgUriReference
    /// </summary>
    public class JsSvgUriReference : JsObject<ISvgUriReference>, IJsSvgUriReference
    {
        public JsSvgUriReference(ISvgUriReference baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = _baseObject.Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgCssRule: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgCssRule
    /// </summary>
    public class JsSvgCssRule : JsCssRule, IJsSvgCssRule
    {
        public JsSvgCssRule(ICssRule baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsDocumentEvent

    /// <summary>
    /// Implementation wrapper for IJsSvgDocument
    /// </summary>
    public class JsSvgDocument : JsDocument, IJsSvgDocument
    {
        public JsSvgDocument(ISvgDocument baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsEvent createEvent(string eventType)
        {
            var wrappedValue = ((IDocumentEvent)_baseObject).CreateEvent(eventType);
            return (wrappedValue != null) ? CreateWrapper<IJsEvent>(wrappedValue, _engine) : null;
        }

        public string title
        {
            get { return ((ISvgDocument)_baseObject).Title; }
        }

        public string referrer
        {
            get { return ((ISvgDocument)_baseObject).Referrer; }
        }

        public string domain
        {
            get { return ((ISvgDocument)_baseObject).Domain; }
        }

        public string URL
        {
            get { return ((ISvgDocument)_baseObject).Url; }
        }

        public IJsSvgSvgElement rootElement
        {
            get {
                var wrappedValue = ((ISvgDocument)_baseObject).RootElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgSvgElement>(wrappedValue, _engine) : null;
            }
        }

        IDocumentEvent IScriptableObject<IDocumentEvent>.BaseObject
        {
            get {
                return (IDocumentEvent)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgSvgElement

    /// <summary>
    /// Implementation wrapper for IJsSvgSvgElement
    /// </summary>
    public class JsSvgSvgElement : JsSvgElement, IJsSvgSvgElement
    {
        public JsSvgSvgElement(ISvgSvgElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ulong suspendRedraw(ulong max_wait_milliseconds)
        {
            return (ulong)((ISvgSvgElement)_baseObject).SuspendRedraw((int)max_wait_milliseconds);
        }

        public void unsuspendRedraw(ulong suspend_handle_id)
        {
            ((ISvgSvgElement)_baseObject).UnsuspendRedraw((int)suspend_handle_id);
        }

        public void unsuspendRedrawAll()
        {
            ((ISvgSvgElement)_baseObject).UnsuspendRedrawAll();
        }

        public void forceRedraw()
        {
            ((ISvgSvgElement)_baseObject).ForceRedraw();
        }

        public void pauseAnimations()
        {
            ((ISvgSvgElement)_baseObject).PauseAnimations();
        }

        public void unpauseAnimations()
        {
            ((ISvgSvgElement)_baseObject).UnpauseAnimations();
        }

        public bool animationsPaused()
        {
            return ((ISvgSvgElement)_baseObject).AnimationsPaused();
        }

        public float getCurrentTime()
        {
            return ((ISvgSvgElement)_baseObject).CurrentTime;
        }

        public void setCurrentTime(float seconds)
        {
            ((ISvgSvgElement)_baseObject).CurrentTime = seconds;
        }

        public IJsNodeList getIntersectionList(IJsSvgRect rect, IJsSvgElement referenceElement)
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).GetIntersectionList(rect.BaseObject, 
                (ISvgElement)referenceElement.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsNodeList>(wrappedValue, _engine) : null;
        }

        public IJsNodeList getEnclosureList(IJsSvgRect rect, IJsSvgElement referenceElement)
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).GetEnclosureList(rect.BaseObject, 
                (ISvgElement)referenceElement.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsNodeList>(wrappedValue, _engine) : null;
        }

        public bool checkIntersection(IJsSvgElement element, IJsSvgRect rect)
        {
            return ((ISvgSvgElement)_baseObject).CheckIntersection((ISvgElement)element.BaseObject, rect.BaseObject);
        }

        public bool checkEnclosure(IJsSvgElement element, IJsSvgRect rect)
        {
            return ((ISvgSvgElement)_baseObject).CheckEnclosure((ISvgElement)element.BaseObject, rect.BaseObject);
        }

        public void deselectAll()
        {
            ((ISvgSvgElement)_baseObject).DeselectAll();
        }

        public IJsSvgNumber createSVGNumber()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgNumber();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength createSVGLength()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgLength();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgAngle createSVGAngle()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgAngle();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgAngle>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint createSVGPoint()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgPoint();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix createSVGMatrix()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgMatrix();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect createSVGRect()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgRect();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform createSVGTransform()
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgTransform();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform createSVGTransformFromMatrix(IJsSvgMatrix matrix)
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).CreateSvgTransformFromMatrix(matrix.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsElement getElementById(string elementId)
        {
            var wrappedValue = ((ISvgSvgElement)_baseObject).GetElementById(elementId);
            return (wrappedValue != null) ? CreateWrapper<IJsElement>(wrappedValue, _engine) : null;
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsEvent createEvent(string eventType)
        {
            var wrappedValue = ((IDocumentEvent)_baseObject).CreateEvent(eventType);
            return (wrappedValue != null) ? CreateWrapper<IJsEvent>(wrappedValue, _engine) : null;
        }

        public IJsCssStyleDeclaration getComputedStyle(IJsElement elt, string pseudoElt)
        {
            var wrappedValue = ((ICssView)_baseObject).GetComputedStyle((XmlElement)elt.BaseObject, pseudoElt);
            return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
        }

        public IJsCssStyleDeclaration getOverrideStyle(IJsElement elt, string pseudoElt)
        {
            var wrappedValue = ((IDocumentCss)_baseObject).GetOverrideStyle((XmlElement)elt.BaseObject, pseudoElt);
            return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public string contentScriptType
        {
            get { return ((ISvgSvgElement)_baseObject).ContentScriptType; }
            set { ((ISvgSvgElement)_baseObject).ContentScriptType = value; }
        }

        public string contentStyleType
        {
            get { return ((ISvgSvgElement)_baseObject).ContentStyleType; }
            set { ((ISvgSvgElement)_baseObject).ContentStyleType = value; }
        }

        public IJsSvgRect viewport
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).Viewport;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
            }
        }

        public float pixelUnitToMillimeterX
        {
            get { return ((ISvgSvgElement)_baseObject).PixelUnitToMillimeterX; }
        }

        public float pixelUnitToMillimeterY
        {
            get { return ((ISvgSvgElement)_baseObject).PixelUnitToMillimeterY; }
        }

        public float screenPixelToMillimeterX
        {
            get { return ((ISvgSvgElement)_baseObject).ScreenPixelToMillimeterX; }
        }

        public float screenPixelToMillimeterY
        {
            get { return ((ISvgSvgElement)_baseObject).ScreenPixelToMillimeterY; }
        }

        public bool useCurrentView
        {
            get { return ((ISvgSvgElement)_baseObject).UseCurrentView; }
            set { ((ISvgSvgElement)_baseObject).UseCurrentView = value; }
        }

        public IJsSvgViewSpec currentView
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).CurrentView;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgViewSpec>(wrappedValue, _engine) : null;
            }
        }

        public float currentScale
        {
            get { return ((ISvgSvgElement)_baseObject).CurrentScale; }
            set { ((ISvgSvgElement)_baseObject).CurrentScale = value; }
        }

        public IJsSvgPoint currentTranslate
        {
            get {
                var wrappedValue = ((ISvgSvgElement)_baseObject).CurrentTranslate;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedRect viewBox
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).ViewBox;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        public ushort zoomAndPan
        {
            get { return (ushort)((ISvgZoomAndPan)_baseObject).ZoomAndPan; }
            set { ((ISvgZoomAndPan)_baseObject).ZoomAndPan = (SvgZoomAndPanType)value; }
        }

        public IJsDocumentView document
        {
            get {
                var wrappedValue = ((IAbstractView)_baseObject).Document;
                return (wrappedValue != null) ? CreateWrapper<IJsDocumentView>(wrappedValue, _engine) : null;
            }
        }

        public IJsStyleSheetList styleSheets
        {
            get {
                var wrappedValue = ((IDocumentStyle)_baseObject).StyleSheets;
                return (wrappedValue != null) ? CreateWrapper<IJsStyleSheetList>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        ISvgFitToViewBox IScriptableObject<ISvgFitToViewBox>.BaseObject
        {
            get { return (ISvgFitToViewBox)this.BaseObject; }
        }
        ISvgZoomAndPan IScriptableObject<ISvgZoomAndPan>.BaseObject
        {
            get { return (ISvgZoomAndPan)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
        IDocumentEvent IScriptableObject<IDocumentEvent>.BaseObject
        {
            get { return (IDocumentEvent)this.BaseObject; }
        }
        IAbstractView IScriptableObject<IAbstractView>.BaseObject
        {
            get { return (IAbstractView)this.BaseObject; }
        }
        IDocumentStyle IScriptableObject<IDocumentStyle>.BaseObject
        {
            get { return (IDocumentStyle)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgGElement

    /// <summary>
    /// Implementation wrapper for IJsSvgGElement
    /// </summary>
    public class JsSvgGElement : JsSvgElement, IJsSvgGElement
    {
        public JsSvgGElement(ISvgGElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgDefsElement

    /// <summary>
    /// Implementation wrapper for IJsSvgDefsElement
    /// </summary>
    public class JsSvgDefsElement : JsSvgElement, IJsSvgDefsElement
    {
        public JsSvgDefsElement(ISvgDefsElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgDescElement

    /// <summary>
    /// Implementation wrapper for IJsSvgDescElement
    /// </summary>
    public class JsSvgDescElement : JsSvgElement, IJsSvgDescElement, IJsSvgLangSpace, IJsSvgStylable
    {
        public JsSvgDescElement(ISvgDescElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgTitleElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTitleElement
    /// </summary>
    public class JsSvgTitleElement : JsSvgElement, IJsSvgTitleElement, IJsSvgLangSpace, IJsSvgStylable
    {
        public JsSvgTitleElement(ISvgTitleElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgSymbolElement

    /// <summary>
    /// Implementation wrapper for IJsSvgSymbolElement
    /// </summary>
    public class JsSvgSymbolElement : JsSvgElement, IJsSvgSymbolElement
    {
        public JsSvgSymbolElement(ISvgSymbolElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedRect viewBox
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).ViewBox;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgFitToViewBox IScriptableObject<ISvgFitToViewBox>.BaseObject
        {
            get { return (ISvgFitToViewBox)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgUseElement

    /// <summary>
    /// Implementation wrapper for IJsSvgUseElement
    /// </summary>
    public class JsSvgUseElement : JsSvgElement, IJsSvgUseElement
    {
        public JsSvgUseElement(ISvgUseElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance instanceRoot
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).InstanceRoot;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance animatedInstanceRoot
        {
            get {
                var wrappedValue = ((ISvgUseElement)_baseObject).AnimatedInstanceRoot;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgImageElement

    /// <summary>
    /// Implementation wrapper for IJsSvgImageElement
    /// </summary>
    public class JsSvgImageElement : JsSvgElement, IJsSvgImageElement
    {
        public JsSvgImageElement(ISvgImageElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgImageElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgImageElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgImageElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgImageElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = ((ISvgImageElement)_baseObject).PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgSwitchElement

    /// <summary>
    /// Implementation wrapper for IJsSvgSwitchElement
    /// </summary>
    public class JsSvgSwitchElement : JsSvgElement, IJsSvgSwitchElement
    {
        public JsSvgSwitchElement(ISvgSwitchElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgStyleElement

    /// <summary>
    /// Implementation wrapper for IJsSvgStyleElement
    /// </summary>
    public class JsSvgStyleElement : JsSvgElement, IJsSvgStyleElement
    {
        public JsSvgStyleElement(ISvgStyleElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgStyleElement)_baseObject).XmlSpace;  }
            set { ((ISvgStyleElement)_baseObject).XmlSpace = value; }
        }

        public string type
        {
            get { return ((ISvgStyleElement)_baseObject).Type;  }
            set { ((ISvgStyleElement)_baseObject).Type = value; }
        }

        public string media
        {
            get { return ((ISvgStyleElement)_baseObject).Media;  }
            set { ((ISvgStyleElement)_baseObject).Media = value; }
        }

        public string title
        {
            get { return ((ISvgStyleElement)_baseObject).Title;  }
            set { ((ISvgStyleElement)_baseObject).Title = value; }
        }

        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPaint

    /// <summary>
    /// Implementation wrapper for IJsSvgPaint
    /// </summary>
    public class JsSvgPaint : JsSvgColor, IJsSvgPaint
    {
        public JsSvgPaint(ISvgPaint baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void setUri(string uri)
        {
            ((ISvgPaint)_baseObject).SetUri(uri);
        }

        public void setPaint(ushort paintType, string uri, string rgbColor, string iccColor)
        {
            ((ISvgPaint)_baseObject).SetPaint((SvgPaintType)paintType, uri, rgbColor, iccColor);
        }

        public ushort paintType
        {
            get { return (ushort)((ISvgPaint)_baseObject).PaintType; }
        }

        public string uri
        {
            get { return ((ISvgPaint)_baseObject).Uri; }
        }
    }

    #endregion

    #region Implementation - IJsSvgMarkerElement

    /// <summary>
    /// Implementation wrapper for IJsSvgMarkerElement
    /// </summary>
    public class JsSvgMarkerElement : JsSvgElement, IJsSvgMarkerElement
    {
        public JsSvgMarkerElement(ISvgMarkerElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void setOrientToAuto()
        {
            ((ISvgMarkerElement)_baseObject).SetOrientToAuto();
        }

        public void setOrientToAngle(IJsSvgAngle angle)
        {
            ((ISvgMarkerElement)_baseObject).SetOrientToAngle(angle.BaseObject);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedLength refX
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).RefX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength refY
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).RefY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration markerUnits
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).MarkerUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength markerWidth
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).MarkerWidth;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength markerHeight
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).MarkerHeight;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration orientType
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).OrientType;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedAngle orientAngle
        {
            get {
                var wrappedValue = ((ISvgMarkerElement)_baseObject).OrientAngle;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedAngle>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedRect viewBox
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).ViewBox;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgFitToViewBox IScriptableObject<ISvgFitToViewBox>.BaseObject
        {
            get { return (ISvgFitToViewBox)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgGradientElement

    /// <summary>
    /// Implementation wrapper for IJsSvgGradientElement
    /// </summary>
    public class JsSvgGradientElement : JsSvgElement, IJsSvgGradientElement
    {
        public JsSvgGradientElement(ISvgGradientElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedEnumeration gradientUnits
        {
            get {
                var wrappedValue = ((ISvgGradientElement)_baseObject).GradientUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList gradientTransform
        {
            get {
                var wrappedValue = ((ISvgGradientElement)_baseObject).GradientTransform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration spreadMethod
        {
            get {
                var wrappedValue = ((ISvgGradientElement)_baseObject).SpreadMethod;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgUnitTypes IScriptableObject<ISvgUnitTypes>.BaseObject
        {
            get { return (ISvgUnitTypes)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgLinearGradientElement

    /// <summary>
    /// Implementation wrapper for IJsSvgLinearGradientElement
    /// </summary>
    public class JsSvgLinearGradientElement : JsSvgGradientElement, IJsSvgLinearGradientElement
    {
        public JsSvgLinearGradientElement(ISvgLinearGradientElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedLength x1
        {
            get {
                var wrappedValue = ((ISvgLinearGradientElement)_baseObject).X1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y1
        {
            get {
                var wrappedValue = ((ISvgLinearGradientElement)_baseObject).Y1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x2
        {
            get {
                var wrappedValue = ((ISvgLinearGradientElement)_baseObject).X2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y2
        {
            get {
                var wrappedValue = ((ISvgLinearGradientElement)_baseObject).Y2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgRadialGradientElement

    /// <summary>
    /// Implementation wrapper for IJsSvgRadialGradientElement
    /// </summary>
    public class JsSvgRadialGradientElement : JsSvgGradientElement, IJsSvgRadialGradientElement
    {
        public JsSvgRadialGradientElement(ISvgRadialGradientElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedLength cx
        {
            get {
                var wrappedValue = ((ISvgRadialGradientElement)_baseObject).Cx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength cy
        {
            get {
                var wrappedValue = ((ISvgRadialGradientElement)_baseObject).Cy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength r
        {
            get {
                var wrappedValue = ((ISvgRadialGradientElement)_baseObject).R;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength fx
        {
            get {
                var wrappedValue = ((ISvgRadialGradientElement)_baseObject).Fx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength fy
        {
            get {
                var wrappedValue = ((ISvgRadialGradientElement)_baseObject).Fy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgStopElement

    /// <summary>
    /// Implementation wrapper for IJsSvgStopElement
    /// </summary>
    public class JsSvgStopElement : JsSvgElement, IJsSvgStopElement, IJsSvgStylable
    {
        public JsSvgStopElement(ISvgStopElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedNumber offset
        {
            get {
                var wrappedValue = ((ISvgStopElement)_baseObject).Offset;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPatternElement

    /// <summary>
    /// Implementation wrapper for IJsSvgPatternElement
    /// </summary>
    public class JsSvgPatternElement : JsSvgElement, IJsSvgPatternElement
    {
        public JsSvgPatternElement(ISvgPatternElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedEnumeration patternUnits
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).PatternUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration patternContentUnits
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).PatternContentUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList patternTransform
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).PatternTransform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgPatternElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedRect viewBox
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).ViewBox;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedPreserveAspectRatio preserveAspectRatio
        {
            get {
                var wrappedValue = ((ISvgFitToViewBox)_baseObject).PreserveAspectRatio;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgFitToViewBox IScriptableObject<ISvgFitToViewBox>.BaseObject
        {
            get { return (ISvgFitToViewBox)this.BaseObject; }
        }
        ISvgUnitTypes IScriptableObject<ISvgUnitTypes>.BaseObject
        {
            get { return (ISvgUnitTypes)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAElement
    /// </summary>
    public class JsSvgAElement : JsSvgElement, IJsSvgAElement
    {
        public JsSvgAElement(ISvgAElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedString target
        {
            get {
                var wrappedValue = ((ISvgAElement)_baseObject).Target;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgScriptElement

    /// <summary>
    /// Implementation wrapper for IJsSvgScriptElement
    /// </summary>
    public sealed class JsSvgScriptElement : JsSvgElement, IJsSvgScriptElement
    {
        public JsSvgScriptElement(ISvgScriptElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string type
        {
            get { return ((ISvgScriptElement)_baseObject).Type; }
            set { ((ISvgScriptElement)_baseObject).Type = value; }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
    }

    #endregion
}

