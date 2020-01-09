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
    #region Implementation - IJsGetSvgDocument: TODO

    /// <summary>
    /// Implementation wrapper for IJsGetSvgDocument
    /// </summary>
    public sealed class JsGetSvgDocument : JsObject<IGetSvgDocument>, IJsGetSvgDocument
    {
        public JsGetSvgDocument(IGetSvgDocument baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgDocument getSVGDocument()
        {
            throw new NotImplementedException();
            //var wrappedValue = ((IGetSvgDocument)baseObject).GetSvgDocument();
            //return (wrappedValue != null) ? CreateWrapper<IJsSvgDocument>(wrappedValue, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsSvgCursorElement: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgCursorElement
    /// </summary>
    public sealed class JsSvgCursorElement : JsSvgElement, IJsSvgCursorElement
    {
        public JsSvgCursorElement(ISvgCursorElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgCursorElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgCursorElement)_baseObject).Y;
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

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get {
                return (ISvgUriReference)this.BaseObject;
            }
        }
        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get {
                return (ISvgTests)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgViewElement

    /// <summary>
    /// Implementation wrapper for IJsSvgViewElement
    /// </summary>
    public sealed class JsSvgViewElement : JsSvgElement, IJsSvgViewElement
    {
        public JsSvgViewElement(ISvgViewElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgStringList viewTarget
        {
            get {
                var wrappedValue = ((ISvgViewElement)_baseObject).ViewTarget;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
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

        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        ISvgFitToViewBox IScriptableObject<ISvgFitToViewBox>.BaseObject
        {
            get {
                return (ISvgFitToViewBox)this.BaseObject;
            }
        }
        ISvgZoomAndPan IScriptableObject<ISvgZoomAndPan>.BaseObject
        {
            get {
                return (ISvgZoomAndPan)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgEvent

    /// <summary>
    /// Implementation wrapper for IJsSvgEvent
    /// </summary>
    public sealed class JsSvgEvent : JsEvent, IJsSvgEvent
    {
        public JsSvgEvent(ISvgEvent baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }
    }

    #endregion

    #region Implementation - IJsSvgZoomEvent: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgZoomEvent
    /// </summary>
    public sealed class JsSvgZoomEvent : JsUiEvent, IJsSvgZoomEvent
    {
        public JsSvgZoomEvent(ISvgZoomEvent baseObject, ISvgScriptEngine engine) : base(baseObject, engine)
        {
        }

        public IJsSvgRect zoomRectScreen
        {
            get {
                var wrappedValue = ((ISvgZoomEvent)_baseObject).ZoomRectScreen;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
            }
        }

        public float previousScale
        {
            get { return (float)((ISvgZoomEvent)_baseObject).PreviousScale;  }
        }

        public IJsSvgPoint previousTranslate
        {
            get {
                var wrappedValue = ((ISvgZoomEvent)_baseObject).PreviousTranslate;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
            }
        }

        public float newScale
        {
            get { return (float)((ISvgZoomEvent)_baseObject).NewScale;  }
        }

        public IJsSvgPoint newTranslate
        {
            get {
                var wrappedValue = ((ISvgZoomEvent)_baseObject).NewTranslate;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgDefinitionSrcElement: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgDefinitionSrcElement
    /// </summary>
    public sealed class JsSvgDefinitionSrcElement : JsSvgElement, IJsSvgDefinitionSrcElement
    {
        public JsSvgDefinitionSrcElement(ISvgDefinitionSrcElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgMetadataElement

    /// <summary>
    /// Implementation wrapper for IJsSvgMetadataElement
    /// </summary>
    public sealed class JsSvgMetadataElement : JsSvgElement, IJsSvgMetadataElement
    {
        public JsSvgMetadataElement(ISvgMetadataElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgForeignObjectElement

    /// <summary>
    /// Implementation wrapper for IJsSvgForeignObjectElement
    /// </summary>
    public sealed class JsSvgForeignObjectElement : JsSvgElement, IJsSvgForeignObjectElement
    {
        public JsSvgForeignObjectElement(ISvgForeignObjectElement baseObject, ISvgScriptEngine engine) 
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
                var wrappedValue = ((ISvgForeignObjectElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgForeignObjectElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgForeignObjectElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgForeignObjectElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
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

    #region Implementation - IJsSvgElementInstance

    /// <summary>
    /// Implementation wrapper for IJsSvgElementInstance
    /// </summary>
    public sealed class JsSvgElementInstance : JsEventTarget, IJsSvgElementInstance
    {
        public JsSvgElementInstance(ISvgElementInstance baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgElement correspondingElement
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).CorrespondingElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgUseElement correspondingUseElement
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).CorrespondingUseElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgUseElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance parentNode
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).ParentNode;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstanceList childNodes
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).ChildNodes;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstanceList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance firstChild
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).FirstChild;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance lastChild
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).LastChild;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance previousSibling
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).PreviousSibling;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElementInstance nextSibling
        {
            get {
                var wrappedValue = ((ISvgElementInstance)_baseObject).NextSibling;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgElementInstanceList

    /// <summary>
    /// Implementation wrapper for IJsSvgElementInstanceList
    /// </summary>
    public sealed class JsSvgElementInstanceList : JsObject<ISvgElementInstanceList>, IJsSvgElementInstanceList
    {
        public JsSvgElementInstanceList(ISvgElementInstanceList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

        public IJsSvgElementInstance item(ulong index)
        {
            var wrappedValue = _baseObject.Item(index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgElementInstance>(wrappedValue, _engine) : null;
        }

        public ulong length
        {
            get { return _baseObject.Length; }
        }
    }

    #endregion

    #region Implementation - IJsSvgColorProfileElement

    /// <summary>
    /// Implementation wrapper for IJsSvgColorProfileElement
    /// </summary>
    public sealed class JsSvgColorProfileElement : JsSvgElement, IJsSvgColorProfileElement
    {
        public JsSvgColorProfileElement(ISvgColorProfileElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string local
        {
            get { return ((ISvgColorProfileElement)_baseObject).Local;  }
            set { ((ISvgColorProfileElement)_baseObject).Local = value; }
        }

        public string name
        {
            get { return ((ISvgColorProfileElement)_baseObject).Name;  }
            set { ((ISvgColorProfileElement)_baseObject).Name = value; }
        }

        public ushort renderingIntent
        {
            get { return ((ISvgColorProfileElement)_baseObject).RenderingIntent;  }
            set { ((ISvgColorProfileElement)_baseObject).RenderingIntent = value; }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgRenderingIntent IScriptableObject<ISvgRenderingIntent>.BaseObject
        {
            get { return (ISvgRenderingIntent)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgColorProfileRule

    /// <summary>
    /// Implementation wrapper for IJsSvgColorProfileRule
    /// </summary>
    public sealed class JsSvgColorProfileRule : JsSvgCssRule, IJsSvgColorProfileRule
    {
        public JsSvgColorProfileRule(ISvgColorProfileRule baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string src
        {
            get { return ((ISvgColorProfileRule)_baseObject).Src;  }
            set { ((ISvgColorProfileRule)_baseObject).Src = value; }
        }

        public string name
        {
            get { return ((ISvgColorProfileRule)_baseObject).Name;  }
            set { ((ISvgColorProfileRule)_baseObject).Name = value; }
        }

        public ushort renderingIntent
        {
            get { return ((ISvgColorProfileRule)_baseObject).RenderingIntent;  }
            set { ((ISvgColorProfileRule)_baseObject).RenderingIntent = value; }
        }

        ISvgRenderingIntent IScriptableObject<ISvgRenderingIntent>.BaseObject
        {
            get { return (ISvgRenderingIntent)this.BaseObject; }
        }
    }

    #endregion
}
