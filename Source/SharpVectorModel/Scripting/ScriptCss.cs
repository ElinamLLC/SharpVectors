using System;
using System.Xml;

using SharpVectors.Dom.Css;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsCssRuleList

    /// <summary>
    /// Implementation wrapper for IJsCssRuleList
    /// </summary>
    public sealed class JsCssRuleList : JsObject<ICssRuleList>, IJsCssRuleList
    {
        public JsCssRuleList(ICssRuleList baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssRule item(ulong index)
        {
            var result = _baseObject[index];
            return (result != null) ? CreateWrapper<IJsCssRule>(result, _engine) : null;
        }

        public ulong length
        {
            get { return _baseObject.Length; }
        }
    }

    #endregion

    #region Implementation - IJsCssRule

    /// <summary>
    /// Implementation wrapper for IJsCssRule
    /// </summary>
    public class JsCssRule : JsObject<ICssRule>, IJsCssRule
    {
        public JsCssRule(ICssRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public ushort type
        {
            get { return (ushort)_baseObject.Type; }
        }

        public string cssText
        {
            get { return _baseObject.CssText; }
            set { _baseObject.CssText = value; }
        }

        public IJsCssStyleSheet parentStyleSheet
        {
            get { 
                var result = _baseObject.ParentStyleSheet; 
                return (result != null) ? CreateWrapper<IJsCssStyleSheet>(result, _engine) : null; 
            }
        }

        public IJsCssRule parentRule
        {
            get { 
                var result = _baseObject.ParentRule; 
                return (result != null) ? CreateWrapper<IJsCssRule>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsCssStyleRule

    /// <summary>
    /// Implementation wrapper for IJsCssStyleRule
    /// </summary>
    public sealed class JsCssStyleRule : JsCssRule, IJsCssStyleRule
    {
        public JsCssStyleRule(ICssStyleRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string selectorText
        {
            get { return ((ICssStyleRule)_baseObject).SelectorText; }
            set { ((ICssStyleRule)_baseObject).SelectorText = value; }
        }

        public IJsCssStyleDeclaration style
        {
            get { 
                var result = ((ICssStyleRule)_baseObject).Style; 
                return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsCssMediaRule

    /// <summary>
    /// Implementation wrapper for IJsCssMediaRule
    /// </summary>
    public sealed class JsCssMediaRule : JsCssRule, IJsCssMediaRule
    {
        public JsCssMediaRule(ICssMediaRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public ulong insertRule(string rule, ulong index)
        {
            return ((ICssMediaRule)_baseObject).InsertRule(rule, index);
        }

        public void deleteRule(ulong index)
        {
            ((ICssMediaRule)_baseObject).DeleteRule(index);
        }

        public IJsMediaList media
        {
            get { 
                var result = ((ICssMediaRule)_baseObject).Media; 
                return (result != null) ? CreateWrapper<IJsMediaList>(result, _engine) : null; 
            }
        }

        public IJsCssRuleList cssRules
        {
            get { 
                var result = ((ICssMediaRule)_baseObject).CssRules; 
                return (result != null) ? CreateWrapper<IJsCssRuleList>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsCssFontFaceRule

    /// <summary>
    /// Implementation wrapper for IJsCssFontFaceRule
    /// </summary>
    public sealed class JsCssFontFaceRule : JsCssRule, IJsCssFontFaceRule
    {
        public JsCssFontFaceRule(ICssFontFaceRule baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssStyleDeclaration style
        {
            get { 
                var result = ((ICssFontFaceRule)_baseObject).Style; 
                return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsCssPageRule

    /// <summary>
    /// Implementation wrapper for IJsCssPageRule
    /// </summary>
    public sealed class JsCssPageRule : JsCssRule, IJsCssPageRule
    {
        public JsCssPageRule(ICssPageRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string selectorText
        {
            get { return ((ICssPageRule)_baseObject).SelectorText; }
            set { ((ICssPageRule)_baseObject).SelectorText = value; }
        }

        public IJsCssStyleDeclaration style
        {
            get { 
                var result = ((ICssPageRule)_baseObject).Style; 
                return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsCssImportRule

    /// <summary>
    /// Implementation wrapper for IJsCssImportRule
    /// </summary>
    public sealed class JsCssImportRule : JsCssRule, IJsCssImportRule
    {
        public JsCssImportRule(ICssImportRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string href
        {
            get { return ((ICssImportRule)_baseObject).Href; }
        }

        public IJsMediaList media
        {
            get {
                var result = ((ICssImportRule)_baseObject).Media;
                return (result != null) ? CreateWrapper<IJsMediaList>(result, _engine) : null;
            }
        }

        public IJsCssStyleSheet styleSheet
        {
            get {
                var result = ((ICssImportRule)_baseObject).StyleSheet;
                return (result != null) ? CreateWrapper<IJsCssStyleSheet>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsCssCharsetRule

    /// <summary>
    /// Implementation wrapper for IJsCssCharsetRule
    /// </summary>
    public sealed class JsCssCharsetRule : JsCssRule, IJsCssCharsetRule
    {
        public JsCssCharsetRule(ICssCharsetRule baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string encoding
        {
            get { return ((ICssCharsetRule)_baseObject).Encoding; }
            set { ((ICssCharsetRule)_baseObject).Encoding = value; }
        }
    }

    #endregion

    #region Implementation - IJsCssUnknownRule

    /// <summary>
    /// Implementation wrapper for IJsCssUnknownRule
    /// </summary>
    public sealed class JsCssUnknownRule : JsCssRule, IJsCssUnknownRule
    {
        public JsCssUnknownRule(ICssUnknownRule baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsCssStyleDeclaration

    /// <summary>
    /// Implementation wrapper for IJsCssStyleDeclaration
    /// </summary>
    public sealed class JsCssStyleDeclaration : JsObject<ICssStyleDeclaration>, IJsCssStyleDeclaration
    {
        public JsCssStyleDeclaration(ICssStyleDeclaration baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string getPropertyValue(string propertyName)
        {
            return _baseObject.GetPropertyValue(propertyName);
        }

        public IJsCssValue getPropertyCSSValue(string propertyName)
        {
            var result = _baseObject.GetPropertyCssValue(propertyName);
            return (result != null) ? CreateWrapper<IJsCssValue>(result, _engine) : null;
        }

        public string removeProperty(string propertyName)
        {
            return _baseObject.RemoveProperty(propertyName);
        }

        public string getPropertyPriority(string propertyName)
        {
            return _baseObject.GetPropertyPriority(propertyName);
        }

        public void setProperty(string propertyName, string value, string priority)
        {
            _baseObject.SetProperty(propertyName, value, priority);
        }

        public string item(ulong index)
        {
            return _baseObject[index];
        }

        public string cssText
        {
            get { return _baseObject.CssText; }
            set { _baseObject.CssText = value; }
        }

        public ulong length
        {
            get { return _baseObject.Length; }
        }

        public IJsCssRule parentRule
        {
            get {
                var result = _baseObject.ParentRule;
                return (result != null) ? CreateWrapper<IJsCssRule>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsCssValue

    /// <summary>
    /// Implementation wrapper for IJsCssValue
    /// </summary>
    public class JsCssValue : JsObject<ICssValue>, IJsCssValue
    {
        public JsCssValue(ICssValue baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string cssText
        {
            get { return _baseObject.CssText; }
            set { _baseObject.CssText = value; }
        }

        public ushort cssValueType
        {
            get { return (ushort)_baseObject.CssValueType; }
        }
    }

    #endregion

    #region Implementation - IJsCssPrimitiveValue

    /// <summary>
    /// Implementation wrapper for IJsCssPrimitiveValue
    /// </summary>
    public sealed class JsCssPrimitiveValue : JsCssValue, IJsCssPrimitiveValue
    {
        public JsCssPrimitiveValue(ICssPrimitiveValue baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void setFloatValue(ushort unitType, float floatValue)
        {
            ((ICssPrimitiveValue)_baseObject).SetFloatValue((CssPrimitiveType)unitType, floatValue);
        }

        public float getFloatValue(ushort unitType)
        {
            return (float)((ICssPrimitiveValue)_baseObject).GetFloatValue((CssPrimitiveType)unitType);
        }

        public void setStringValue(ushort stringType, string stringValue)
        {
            ((ICssPrimitiveValue)_baseObject).SetStringValue((CssPrimitiveType)stringType, stringValue);
        }

        public string getStringValue()
        {
            return ((ICssPrimitiveValue)_baseObject).GetStringValue();
        }

        public IJsCounter getCounterValue()
        {
            var result = ((ICssPrimitiveValue)_baseObject).GetCounterValue();
            return (result != null) ? CreateWrapper<IJsCounter>(result, _engine) : null;
        }

        public IJsRect getRectValue()
        {
            var result = ((ICssPrimitiveValue)_baseObject).GetRectValue();
            return (result != null) ? CreateWrapper<IJsRect>(result, _engine) : null;
        }

        public IJsRgbColor getRGBColorValue()
        {
            var result = ((ICssPrimitiveValue)_baseObject).GetRgbColorValue();
            return (result != null) ? CreateWrapper<IJsRgbColor>(result, _engine) : null;
        }

        public ushort primitiveType
        {
            get { return (ushort)((ICssPrimitiveValue)_baseObject).PrimitiveType; }
        }
    }

    #endregion

    #region Implementation - IJsCssValueList

    /// <summary>
    /// Implementation wrapper for IJsCssValueList
    /// </summary>
    public sealed class JsCssValueList : JsCssValue, IJsCssValueList
    {
        public JsCssValueList(ICssValueList baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssValue item(ulong index)
        {
            var result = ((ICssValueList)_baseObject)[index];
            return (result != null) ? CreateWrapper<IJsCssValue>(result, _engine) : null;
        }

        public ulong length
        {
            get { return ((ICssValueList)_baseObject).Length; }
        }
    }

    #endregion

    #region Implementation - IJsRgbColor

    /// <summary>
    /// Implementation wrapper for IJsRgbColor
    /// </summary>
    public sealed class JsRgbColor : JsObject<ICssColor>, IJsRgbColor
    {
        public JsRgbColor(ICssColor baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssPrimitiveValue red
        {
            get {
                var result = _baseObject.Red;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }

        public IJsCssPrimitiveValue green
        {
            get {
                var result = _baseObject.Green;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }

        public IJsCssPrimitiveValue blue
        {
            get {
                var result = _baseObject.Blue;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsRect

    /// <summary>
    /// Implementation wrapper for IJsRect
    /// </summary>
    public sealed class JsRect : JsObject<ICssRect>, IJsRect
    {
        public JsRect(ICssRect baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssPrimitiveValue top
        {
            get {
                var result = _baseObject.Top;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }

        public IJsCssPrimitiveValue right
        {
            get {
                var result = _baseObject.Right;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }

        public IJsCssPrimitiveValue bottom
        {
            get {
                var result = _baseObject.Bottom;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }

        public IJsCssPrimitiveValue left
        {
            get {
                var result = _baseObject.Left;
                return (result != null) ? CreateWrapper<IJsCssPrimitiveValue>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsCounter

    /// <summary>
    /// Implementation wrapper for IJsCounter
    /// </summary>
    public sealed class JsCounter : JsObject<ICssCounter>, IJsCounter
    {
        public JsCounter(ICssCounter baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string identifier
        {
            get { return _baseObject.Identifier; }
        }

        public string listStyle
        {
            get { return _baseObject.ListStyle; }
        }

        public string separator
        {
            get { return _baseObject.Separator; }
        }
    }

    #endregion

    #region Implementation - IJsElementCssInlineStyle

    /// <summary>
    /// Implementation wrapper for IJsElementCssInlineStyle
    /// </summary>
    public sealed class JsElementCssInlineStyle : JsObject<IElementCssInlineStyle>, IJsElementCssInlineStyle
    {
        public JsElementCssInlineStyle(IElementCssInlineStyle baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssStyleDeclaration style
        {
            get { var result = _baseObject.Style;
                return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsCssStyleSheet

    /// <summary>
    /// Implementation wrapper for IJsCssStyleSheet
    /// </summary>
    public class JsCssStyleSheet : JsStyleSheet, IJsCssStyleSheet
    {
        public JsCssStyleSheet(ICssStyleSheet baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public ulong insertRule(string rule, ulong index)
        {
            return ((ICssStyleSheet)_baseObject).InsertRule(rule, index);
        }

        public void deleteRule(ulong index)
        {
            ((ICssStyleSheet)_baseObject).DeleteRule(index);
        }

        public IJsCssRule ownerRule
        {
            get {
                var result = ((ICssStyleSheet)_baseObject).OwnerRule;
                return (result != null) ? CreateWrapper<IJsCssRule>(result, _engine) : null;
            }
        }

        public IJsCssRuleList cssRules
        {
            get {
                var result = ((ICssStyleSheet)_baseObject).CssRules;
                return (result != null) ? CreateWrapper<IJsCssRuleList>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsViewCss

    /// <summary>
    /// Implementation wrapper for IJsViewCss
    /// </summary>
    public sealed class JsViewCss : JsAbstractView, IJsViewCss
    {
        public JsViewCss(ICssView baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssStyleDeclaration getComputedStyle(IJsElement elt, string pseudoElt)
        {
            var result = ((ICssView)_baseObject).GetComputedStyle((XmlElement)elt.BaseObject, pseudoElt);
            return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsDocumentCss

    /// <summary>
    /// Implementation wrapper for IJsDocumentCss
    /// </summary>
    public sealed class JsDocumentCss : JsDocumentStyle, IJsDocumentCss
    {
        public JsDocumentCss(IDocumentCss baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsCssStyleDeclaration getOverrideStyle(IJsElement elt, string pseudoElt)
        {
            var result = ((IDocumentCss)_baseObject).GetOverrideStyle((XmlElement)elt.BaseObject, pseudoElt);
            return (result != null) ? CreateWrapper<IJsCssStyleDeclaration>(result, _engine) : null;
        }
    }

    #endregion

    #region Implementation - IJsDomImplementationCss

    /// <summary>
    /// Implementation wrapper for IJsDomImplementationCss
    /// </summary>
    public sealed class JsDomImplementationCss : JsDomImplementation, IJsDomImplementationCss
    {
        public JsDomImplementationCss(IDomImplementationCss baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssStyleSheet createCSSStyleSheet(string title, string media)
        {
            var result = ((IDomImplementationCss)_baseObject).CreateCssStyleSheet(title, media);
            return (result != null) ? CreateWrapper<IJsCssStyleSheet>(result, _engine) : null;
        }
    }   

    #endregion
}

