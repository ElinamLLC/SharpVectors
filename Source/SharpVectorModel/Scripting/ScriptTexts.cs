using System;

using SharpVectors.Dom.Events;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgTextContentElement: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgTextContentElement
    /// </summary>
    public class JsSvgTextContentElement : JsSvgElement, IJsSvgTextContentElement
    {
        public JsSvgTextContentElement(ISvgTextContentElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public long getNumberOfChars()
        {
            return ((ISvgTextContentElement)_baseObject).GetNumberOfChars();
        }

        public float getComputedTextLength()
        {
            return ((ISvgTextContentElement)_baseObject).GetComputedTextLength();
        }

        public float getSubStringLength(ulong charnum, ulong nchars)
        {
            return ((ISvgTextContentElement)_baseObject).GetSubStringLength((long)charnum, (long)nchars);
        }

        public IJsSvgPoint getStartPositionOfChar(ulong charnum)
        {
            var wrappedValue = ((ISvgTextContentElement)_baseObject).GetStartPositionOfChar((long)charnum);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint getEndPositionOfChar(ulong charnum)
        {
            var wrappedValue = ((ISvgTextContentElement)_baseObject).GetEndPositionOfChar((long)charnum);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getExtentOfChar(ulong charnum)
        {
            var wrappedValue = ((ISvgTextContentElement)_baseObject).GetExtentOfChar((long)charnum);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public float getRotationOfChar(ulong charnum)
        {
            return ((ISvgTextContentElement)_baseObject).GetRotationOfChar((long)charnum);
        }

        public long getCharNumAtPosition(IJsSvgPoint point)
        {
            return ((ISvgTextContentElement)_baseObject).GetCharNumAtPosition(point.BaseObject);
        }

        public void selectSubString(ulong charnum, ulong nchars)
        {
            ((ISvgTextContentElement)_baseObject).SelectSubString((long)charnum, (long)nchars);
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

        public IJsSvgAnimatedLength textLength
        {
            get {
                var wrappedValue = ((ISvgTextContentElement)_baseObject).TextLength;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration lengthAdjust
        {
            get {
                var wrappedValue = ((ISvgTextContentElement)_baseObject).LengthAdjust;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
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
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgTextPositioningElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTextPositioningElement
    /// </summary>
    public class JsSvgTextPositioningElement : JsSvgTextContentElement, IJsSvgTextPositioningElement
    {
        public JsSvgTextPositioningElement(ISvgTextPositioningElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

        public IJsSvgAnimatedLengthList x
        {
            get {
                var wrappedValue = ((ISvgTextPositioningElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLengthList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLengthList y
        {
            get {
                var wrappedValue = ((ISvgTextPositioningElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLengthList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLengthList dx
        {
            get {
                var wrappedValue = ((ISvgTextPositioningElement)_baseObject).Dx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLengthList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLengthList dy
        {
            get {
                var wrappedValue = ((ISvgTextPositioningElement)_baseObject).Dy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLengthList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumberList rotate
        {
            get {
                var wrappedValue = ((ISvgTextPositioningElement)_baseObject).Rotate;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumberList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgTextElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTextElement
    /// </summary>
    public sealed class JsSvgTextElement : JsSvgTextPositioningElement, IJsSvgTextElement
    {
        public JsSvgTextElement(ISvgTextElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
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

        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgTSpanElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTSpanElement
    /// </summary>
    public sealed class JsSvgTSpanElement : JsSvgTextPositioningElement, IJsSvgTSpanElement
    {
        public JsSvgTSpanElement(ISvgTSpanElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgTRefElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTRefElement
    /// </summary>
    public sealed class JsSvgTRefElement : JsSvgTextPositioningElement, IJsSvgTRefElement, IJsSvgUriReference
    {
        public JsSvgTRefElement(ISvgTRefElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
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
    }

    #endregion

    #region Implementation - IJsSvgTextPathElement

    /// <summary>
    /// Implementation wrapper for IJsSvgTextPathElement
    /// </summary>
    public sealed class JsSvgTextPathElement : JsSvgTextContentElement, IJsSvgTextPathElement, IJsSvgUriReference
    {
        public JsSvgTextPathElement(ISvgTextPathElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedLength startOffset
        {
            get {
                var wrappedValue = ((ISvgTextPathElement)_baseObject).StartOffset;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration method
        {
            get {
                var wrappedValue = ((ISvgTextPathElement)_baseObject).Method;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration spacing
        {
            get {
                var wrappedValue = ((ISvgTextPathElement)_baseObject).Spacing;
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

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAltGlyphElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAltGlyphElement
    /// </summary>
    public sealed class JsSvgAltGlyphElement : JsSvgTextPositioningElement, IJsSvgAltGlyphElement, IJsSvgUriReference
    {
        public JsSvgAltGlyphElement(ISvgAltGlyphElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string glyphRef
        {
            get { return ((ISvgAltGlyphElement)_baseObject).GlyphRef;  }
            set { ((ISvgAltGlyphElement)_baseObject).GlyphRef = value; }
        }

        public string format
        {
            get { return ((ISvgAltGlyphElement)_baseObject).Format;  }
            set { ((ISvgAltGlyphElement)_baseObject).Format = value; }
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
    }

    #endregion

    #region Implementation - IJsSvgAltGlyphDefElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAltGlyphDefElement
    /// </summary>
    public sealed class JsSvgAltGlyphDefElement : JsSvgElement, IJsSvgAltGlyphDefElement
    {
        public JsSvgAltGlyphDefElement(ISvgAltGlyphDefElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgAltGlyphItemElement

    /// <summary>
    /// Implementation wrapper for IJsSvgAltGlyphItemElement
    /// </summary>
    public sealed class JsSvgAltGlyphItemElement : JsSvgElement, IJsSvgAltGlyphItemElement
    {
        public JsSvgAltGlyphItemElement(ISvgAltGlyphItemElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgGlyphRefElement

    /// <summary>
    /// Implementation wrapper for IJsSvgGlyphRefElement
    /// </summary>
    public sealed class JsSvgGlyphRefElement : JsSvgElement, IJsSvgGlyphRefElement
    {
        public JsSvgGlyphRefElement(ISvgGlyphRefElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public string glyphRef
        {
            get { return ((ISvgGlyphRefElement)_baseObject).GlyphRef;  }
            set { ((ISvgGlyphRefElement)_baseObject).GlyphRef = value; }
        }

        public string format
        {
            get { return ((ISvgGlyphRefElement)_baseObject).Format;  }
            set { ((ISvgGlyphRefElement)_baseObject).Format = value; }
        }

        public float x
        {
            get { return ((ISvgGlyphRefElement)_baseObject).X;  }
            set { ((ISvgGlyphRefElement)_baseObject).X = value; }
        }

        public float y
        {
            get { return ((ISvgGlyphRefElement)_baseObject).Y;  }
            set { ((ISvgGlyphRefElement)_baseObject).Y = value; }
        }

        public float dx
        {
            get { return ((ISvgGlyphRefElement)_baseObject).Dx;  }
            set { ((ISvgGlyphRefElement)_baseObject).Dx = value; }
        }

        public float dy
        {
            get { return ((ISvgGlyphRefElement)_baseObject).Dy;  }
            set { ((ISvgGlyphRefElement)_baseObject).Dy = value; }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
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

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get { return (ISvgUriReference)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgFontElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontElement
    /// </summary>
    public sealed class JsSvgFontElement : JsSvgElement, IJsSvgFontElement
    {
        public JsSvgFontElement(ISvgFontElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
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

        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgGlyphElement

    /// <summary>
    /// Implementation wrapper for IJsSvgGlyphElement
    /// </summary>
    public sealed class JsSvgGlyphElement : JsSvgElement, IJsSvgGlyphElement
    {
        public JsSvgGlyphElement(ISvgGlyphElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
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

    #region Implementation - IJsSvgMissingGlyphElement

    /// <summary>
    /// Implementation wrapper for IJsSvgMissingGlyphElement
    /// </summary>
    public sealed class JsSvgMissingGlyphElement : JsSvgElement, IJsSvgMissingGlyphElement, IJsSvgStylable
    {
        public JsSvgMissingGlyphElement(ISvgMissingGlyphElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
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

    #region Implementation - IJsSvgHKernElement

    /// <summary>
    /// Implementation wrapper for IJsSvgHKernElement
    /// </summary>
    public sealed class JsSvgHKernElement : JsSvgElement, IJsSvgHKernElement
    {
        public JsSvgHKernElement(ISvgHKernElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgVKernElement

    /// <summary>
    /// Implementation wrapper for IJsSvgVKernElement
    /// </summary>
    public sealed class JsSvgVKernElement : JsSvgElement, IJsSvgVKernElement
    {
        public JsSvgVKernElement(ISvgVKernElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFontFaceElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontFaceElement
    /// </summary>
    public sealed class JsSvgFontFaceElement : JsSvgElement, IJsSvgFontFaceElement
    {
        public JsSvgFontFaceElement(ISvgFontFaceElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFontFaceSrcElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontFaceSrcElement
    /// </summary>
    public sealed class JsSvgFontFaceSrcElement : JsSvgElement, IJsSvgFontFaceSrcElement
    {
        public JsSvgFontFaceSrcElement(ISvgFontFaceSrcElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFontFaceUriElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontFaceUriElement
    /// </summary>
    public sealed class JsSvgFontFaceUriElement : JsSvgElement, IJsSvgFontFaceUriElement
    {
        public JsSvgFontFaceUriElement(ISvgFontFaceUriElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFontFaceFormatElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontFaceFormatElement
    /// </summary>
    public sealed class JsSvgFontFaceFormatElement : JsSvgElement, IJsSvgFontFaceFormatElement
    {
        public JsSvgFontFaceFormatElement(ISvgFontFaceFormatElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFontFaceNameElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFontFaceNameElement
    /// </summary>
    public sealed class JsSvgFontFaceNameElement : JsSvgElement, IJsSvgFontFaceNameElement
    {
        public JsSvgFontFaceNameElement(ISvgFontFaceNameElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion
}
