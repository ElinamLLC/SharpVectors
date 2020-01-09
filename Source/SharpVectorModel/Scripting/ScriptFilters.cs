using System;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgClipPathElement

    /// <summary>
    /// Implementation wrapper for IJsSvgClipPathElement
    /// </summary>
    public sealed class JsSvgClipPathElement : JsSvgElement, IJsSvgClipPathElement
    {
        public JsSvgClipPathElement(ISvgClipPathElement baseObject, ISvgScriptEngine engine) 
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

        public IJsSvgAnimatedEnumeration clipPathUnits
        {
            get {
                var wrappedValue = ((ISvgClipPathElement)_baseObject).ClipPathUnits;
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
            get {
                return (ISvgTests)this.BaseObject;
            }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get {
                return (ISvgLangSpace)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get {
                return (ISvgLocatable)this.BaseObject;
            }
        }
        ISvgUnitTypes IScriptableObject<ISvgUnitTypes>.BaseObject
        {
            get {
                return (ISvgUnitTypes)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgMaskElement

    /// <summary>
    /// Implementation wrapper for IJsSvgMaskElement
    /// </summary>
    public sealed class JsSvgMaskElement : JsSvgElement, IJsSvgMaskElement
    {
        public JsSvgMaskElement(ISvgMaskElement baseObject, ISvgScriptEngine engine) 
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

        public IJsSvgAnimatedEnumeration maskUnits
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).MaskUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration maskContentUnits
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).MaskContentUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgMaskElement)_baseObject).Height;
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

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get {
                return (ISvgTests)this.BaseObject;
            }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get {
                return (ISvgLangSpace)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
        ISvgUnitTypes IScriptableObject<ISvgUnitTypes>.BaseObject
        {
            get {
                return (ISvgUnitTypes)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFilterElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFilterElement
    /// </summary>
    public sealed class JsSvgFilterElement : JsSvgElement, IJsSvgFilterElement
    {
        public JsSvgFilterElement(ISvgFilterElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void setFilterRes(ulong filterResX, ulong filterResY)
        {
            ((ISvgFilterElement)_baseObject).SetFilterRes(filterResX, filterResY);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedEnumeration filterUnits
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).FilterUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration primitiveUnits
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).PrimitiveUnits;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger filterResX
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).FilterResX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger filterResY
        {
            get {
                var wrappedValue = ((ISvgFilterElement)_baseObject).FilterResY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
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

        ISvgUriReference IScriptableObject<ISvgUriReference>.BaseObject
        {
            get {
                return (ISvgUriReference)this.BaseObject;
            }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get {
                return (ISvgLangSpace)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
        ISvgUnitTypes IScriptableObject<ISvgUnitTypes>.BaseObject
        {
            get {
                return (ISvgUnitTypes)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFilterPrimitiveStandardAttributes

    /// <summary>
    /// Implementation wrapper for IJsSvgFilterPrimitiveStandardAttributes
    /// </summary>
    public sealed class JsSvgFilterPrimitiveStandardAttributes : JsSvgStylable, IJsSvgFilterPrimitiveStandardAttributes
    {
        public JsSvgFilterPrimitiveStandardAttributes(ISvgFilterPrimitiveStandardAttributes baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEBlendElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEBlendElement
    /// </summary>
    public sealed class JsSvgFEBlendElement : JsSvgElement, IJsSvgFEBlendElement
    {
        public JsSvgFEBlendElement(ISvgFEBlendElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEBlendElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString in2
        {
            get {
                var wrappedValue = ((ISvgFEBlendElement)_baseObject).In2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration mode
        {
            get {
                var wrappedValue = ((ISvgFEBlendElement)_baseObject).Mode;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEColorMatrixElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEColorMatrixElement
    /// </summary>
    public sealed class JsSvgFEColorMatrixElement : JsSvgElement, IJsSvgFEColorMatrixElement
    {
        public JsSvgFEColorMatrixElement(ISvgFEColorMatrixElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEColorMatrixElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration type
        {
            get {
                var wrappedValue = ((ISvgFEColorMatrixElement)_baseObject).Type;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumberList values
        {
            get {
                var wrappedValue = ((ISvgFEColorMatrixElement)_baseObject).Values;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumberList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEComponentTransferElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEComponentTransferElement
    /// </summary>
    public class JsSvgFEComponentTransferElement : JsSvgElement, IJsSvgFEComponentTransferElement
    {
        public JsSvgFEComponentTransferElement(ISvgFEComponentTransferElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEComponentTransferElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgComponentTransferFunctionElement

    /// <summary>
    /// Implementation wrapper for IJsSvgComponentTransferFunctionElement
    /// </summary>
    public class JsSvgComponentTransferFunctionElement : JsSvgElement, IJsSvgComponentTransferFunctionElement
    {
        public JsSvgComponentTransferFunctionElement(ISvgComponentTransferFunctionElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedEnumeration type
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Type;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumberList tableValues
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).TableValues;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumberList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber slope
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Slope;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber intercept
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Intercept;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber amplitude
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Amplitude;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber exponent
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Exponent;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber offset
        {
            get {
                var wrappedValue = ((ISvgComponentTransferFunctionElement)_baseObject).Offset;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEFuncRElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEFuncRElement
    /// </summary>
    public sealed class JsSvgFEFuncRElement : JsSvgComponentTransferFunctionElement, IJsSvgFEFuncRElement
    {
        public JsSvgFEFuncRElement(ISvgFEFuncRElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFEFuncGElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEFuncGElement
    /// </summary>
    public sealed class JsSvgFEFuncGElement : JsSvgComponentTransferFunctionElement, IJsSvgFEFuncGElement
    {
        public JsSvgFEFuncGElement(ISvgFEFuncGElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFEFuncBElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEFuncBElement
    /// </summary>
    public sealed class JsSvgFEFuncBElement : JsSvgComponentTransferFunctionElement, IJsSvgFEFuncBElement
    {
        public JsSvgFEFuncBElement(ISvgFEFuncBElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFEFuncAElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEFuncAElement
    /// </summary>
    public sealed class JsSvgFEFuncAElement : JsSvgComponentTransferFunctionElement, IJsSvgFEFuncAElement
    {
        public JsSvgFEFuncAElement(ISvgFEFuncAElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgFECompositeElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFECompositeElement
    /// </summary>
    public sealed class JsSvgFECompositeElement : JsSvgElement, IJsSvgFECompositeElement
    {
        public JsSvgFECompositeElement(ISvgFECompositeElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString in2
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).In2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration operator_
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).Operator;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber k1
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).K1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber k2
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).K2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber k3
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).K3;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber k4
        {
            get {
                var wrappedValue = ((ISvgFECompositeElement)_baseObject).K4;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEConvolveMatrixElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEConvolveMatrixElement
    /// </summary>
    public sealed class JsSvgFEConvolveMatrixElement : JsSvgElement, IJsSvgFEConvolveMatrixElement
    {
        public JsSvgFEConvolveMatrixElement(ISvgFEConvolveMatrixElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedInteger orderX
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).OrderX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger orderY
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).OrderY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumberList kernelMatrix
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).KernelMatrix;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumberList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber divisor
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).Divisor;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber bias
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).Bias;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger targetX
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).TargetX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger targetY
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).TargetY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration edgeMode
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).EdgeMode;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength kernelUnitLengthX
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).KernelUnitLengthX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength kernelUnitLengthY
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).KernelUnitLengthY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedBoolean preserveAlpha
        {
            get {
                var wrappedValue = ((ISvgFEConvolveMatrixElement)_baseObject).PreserveAlpha;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedBoolean>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEDiffuseLightingElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEDiffuseLightingElement
    /// </summary>
    public sealed class JsSvgFEDiffuseLightingElement : JsSvgElement, IJsSvgFEDiffuseLightingElement
    {
        public JsSvgFEDiffuseLightingElement(ISvgFEDiffuseLightingElement baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEDiffuseLightingElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber surfaceScale
        {
            get {
                var wrappedValue = ((ISvgFEDiffuseLightingElement)_baseObject).SurfaceScale;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber diffuseConstant
        {
            get {
                var wrappedValue = ((ISvgFEDiffuseLightingElement)_baseObject).DiffuseConstant;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEDistantLightElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEDistantLightElement
    /// </summary>
    public sealed class JsSvgFEDistantLightElement : JsSvgElement, IJsSvgFEDistantLightElement
    {
        public JsSvgFEDistantLightElement(ISvgFEDistantLightElement baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedNumber azimuth
        {
            get {
                var wrappedValue = ((ISvgFEDistantLightElement)_baseObject).Azimuth;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber elevation
        {
            get {
                var wrappedValue = ((ISvgFEDistantLightElement)_baseObject).Elevation;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEPointLightElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEPointLightElement
    /// </summary>
    public sealed class JsSvgFEPointLightElement : JsSvgElement, IJsSvgFEPointLightElement
    {
        public JsSvgFEPointLightElement(ISvgFEPointLightElement baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedNumber x
        {
            get {
                var wrappedValue = ((ISvgFEPointLightElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber y
        {
            get {
                var wrappedValue = ((ISvgFEPointLightElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber z
        {
            get {
                var wrappedValue = ((ISvgFEPointLightElement)_baseObject).Z;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFESpotLightElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFESpotLightElement
    /// </summary>
    public sealed class JsSvgFESpotLightElement : JsSvgElement, IJsSvgFESpotLightElement
    {
        public JsSvgFESpotLightElement(ISvgFESpotLightElement baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedNumber x
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber y
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber z
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).Z;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber pointsAtX
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).PointsAtX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber pointsAtY
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).PointsAtY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber pointsAtZ
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).PointsAtZ;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber specularExponent
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).SpecularExponent;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber limitingConeAngle
        {
            get {
                var wrappedValue = ((ISvgFESpotLightElement)_baseObject).LimitingConeAngle;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEDisplacementMapElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEDisplacementMapElement
    /// </summary>
    public sealed class JsSvgFEDisplacementMapElement : JsSvgElement, IJsSvgFEDisplacementMapElement
    {
        public JsSvgFEDisplacementMapElement(ISvgFEDisplacementMapElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEDisplacementMapElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString in2
        {
            get {
                var wrappedValue = ((ISvgFEDisplacementMapElement)_baseObject).In2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber scale
        {
            get {
                var wrappedValue = ((ISvgFEDisplacementMapElement)_baseObject).Scale;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration xChannelSelector
        {
            get {
                var wrappedValue = ((ISvgFEDisplacementMapElement)_baseObject).XChannelSelector;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration yChannelSelector
        {
            get {
                var wrappedValue = ((ISvgFEDisplacementMapElement)_baseObject).YChannelSelector;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEFloodElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEFloodElement
    /// </summary>
    public sealed class JsSvgFEFloodElement : JsSvgElement, IJsSvgFEFloodElement
    {
        public JsSvgFEFloodElement(ISvgFEFloodElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEFloodElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEGaussianBlurElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEGaussianBlurElement
    /// </summary>
    public sealed class JsSvgFEGaussianBlurElement : JsSvgElement, IJsSvgFEGaussianBlurElement
    {
        public JsSvgFEGaussianBlurElement(ISvgFEGaussianBlurElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void setStdDeviation(float stdDeviationX, float stdDeviationY)
        {
            ((ISvgFEGaussianBlurElement)_baseObject).SetStdDeviation(stdDeviationX, stdDeviationY);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEGaussianBlurElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber stdDeviationX
        {
            get {
                var wrappedValue = ((ISvgFEGaussianBlurElement)_baseObject).StdDeviationX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber stdDeviationY
        {
            get {
                var wrappedValue = ((ISvgFEGaussianBlurElement)_baseObject).StdDeviationY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEImageElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEImageElement
    /// </summary>
    public sealed class JsSvgFEImageElement : JsSvgElement, IJsSvgFEImageElement
    {
        public JsSvgFEImageElement(ISvgFEImageElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString href
        {
            get {
                var wrappedValue = ((ISvgUriReference)_baseObject).Href;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
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

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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
            get {
                return (ISvgUriReference)this.BaseObject;
            }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get {
                return (ISvgLangSpace)this.BaseObject;
            }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get {
                return (ISvgExternalResourcesRequired)this.BaseObject;
            }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEMergeElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEMergeElement
    /// </summary>
    public sealed class JsSvgFEMergeElement : JsSvgElement, IJsSvgFEMergeElement
    {
        public JsSvgFEMergeElement(ISvgFEMergeElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEMergeNodeElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEMergeNodeElement
    /// </summary>
    public sealed class JsSvgFEMergeNodeElement : JsSvgElement, IJsSvgFEMergeNodeElement
    {
        public JsSvgFEMergeNodeElement(ISvgFEMergeNodeElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEMergeNodeElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null; }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEMorphologyElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEMorphologyElement
    /// </summary>
    public sealed class JsSvgFEMorphologyElement : JsSvgElement, IJsSvgFEMorphologyElement
    {
        public JsSvgFEMorphologyElement(ISvgFEMorphologyElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEMorphologyElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration operator_
        {
            get {
                var wrappedValue = ((ISvgFEMorphologyElement)_baseObject).Operator;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength radiusX
        {
            get {
                var wrappedValue = ((ISvgFEMorphologyElement)_baseObject).RadiusX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength radiusY
        {
            get {
                var wrappedValue = ((ISvgFEMorphologyElement)_baseObject).RadiusY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFEOffsetElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFEOffsetElement
    /// </summary>
    public sealed class JsSvgFEOffsetElement : JsSvgElement, IJsSvgFEOffsetElement
    {
        public JsSvgFEOffsetElement(ISvgFEOffsetElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFEOffsetElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber dx
        {
            get {
                var wrappedValue = ((ISvgFEOffsetElement)_baseObject).Dx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber dy
        {
            get {
                var wrappedValue = ((ISvgFEOffsetElement)_baseObject).Dy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFESpecularLightingElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFESpecularLightingElement
    /// </summary>
    public sealed class JsSvgFESpecularLightingElement : JsSvgElement, IJsSvgFESpecularLightingElement
    {
        public JsSvgFESpecularLightingElement(ISvgFESpecularLightingElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFESpecularLightingElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber surfaceScale
        {
            get {
                var wrappedValue = ((ISvgFESpecularLightingElement)_baseObject).SurfaceScale;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber specularConstant
        {
            get {
                var wrappedValue = ((ISvgFESpecularLightingElement)_baseObject).SpecularConstant;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber specularExponent
        {
            get {
                var wrappedValue = ((ISvgFESpecularLightingElement)_baseObject).SpecularExponent;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFETileElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFETileElement
    /// </summary>
    public sealed class JsSvgFETileElement : JsSvgElement, IJsSvgFETileElement
    {
        public JsSvgFETileElement(ISvgFETileElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedString in1
        {
            get {
                var wrappedValue = ((ISvgFETileElement)_baseObject).In1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgFETurbulenceElement

    /// <summary>
    /// Implementation wrapper for IJsSvgFETurbulenceElement
    /// </summary>
    public sealed class JsSvgFETurbulenceElement : JsSvgElement, IJsSvgFETurbulenceElement
    {
        public JsSvgFETurbulenceElement(ISvgFETurbulenceElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgAnimatedNumber baseFrequencyX
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).BaseFrequencyX;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber baseFrequencyY
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).BaseFrequencyY;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedInteger numOctaves
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).NumOctaves;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedInteger>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedNumber seed
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).Seed;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration stitchTiles
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).StitchTiles;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedEnumeration type
        {
            get {
                var wrappedValue = ((ISvgFETurbulenceElement)_baseObject).Type;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedEnumeration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedString result
        {
            get {
                var wrappedValue = ((ISvgFilterPrimitiveStandardAttributes)_baseObject).Result;
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

        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get {
                return (ISvgStylable)this.BaseObject;
            }
        }
    }

    #endregion
}
