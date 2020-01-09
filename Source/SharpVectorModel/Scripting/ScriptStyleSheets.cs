using System;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsStyleSheet

    /// <summary>
    /// Implementation wrapper for IJsStyleSheet
    /// </summary>
    public class JsStyleSheet : JsObject<IStyleSheet>, IJsStyleSheet
    {
        public JsStyleSheet(IStyleSheet baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string type
        {
            get { return _baseObject.Type; }
        }

        public bool disabled
        {
            get { return _baseObject.Disabled; }
            set { _baseObject.Disabled = value; }
        }

        public IJsNode ownerNode
        {
            get {
                var result = _baseObject.OwnerNode;
                return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
            }
        }

        public IJsStyleSheet parentStyleSheet
        {
            get {
                var result = _baseObject.ParentStyleSheet;
                return (result != null) ? CreateWrapper<IJsStyleSheet>(result, _engine) : null;
            }
        }

        public string href
        {
            get { return _baseObject.Href; }
        }

        public string title
        {
            get { return _baseObject.Title; }
        }

        public IJsMediaList media
        {
            get {
                var result = _baseObject.Media;
                return (result != null) ? CreateWrapper<IJsMediaList>(result, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsStyleSheetList

    /// <summary>
    /// Implementation wrapper for IJsStyleSheetList
    /// </summary>
    public class JsStyleSheetList : JsObject<IStyleSheetList>, IJsStyleSheetList
    {
        public JsStyleSheetList(IStyleSheetList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsStyleSheet item(ulong index)
        {
            var result = _baseObject[index];
            return (result != null) ? CreateWrapper<IJsStyleSheet>(result, _engine) : null;
        }

        public ulong length
        {
            get { return _baseObject.Length; }
        }
    }

    #endregion

    #region Implementation - IJsMediaList

    /// <summary>
    /// Implementation wrapper for IJsMediaList
    /// </summary>
    public class JsMediaList : JsObject<IMediaList>, IJsMediaList
    {
        public JsMediaList(IMediaList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string item(ulong index)
        {
            return ((IMediaList)_baseObject)[index];
        }

        public void deleteMedium(string oldMedium)
        {
            ((IMediaList)_baseObject).DeleteMedium(oldMedium);
        }

        public void appendMedium(string newMedium)
        {
            ((IMediaList)_baseObject).AppendMedium(newMedium);
        }

        public string mediaText
        {
            get { return ((IMediaList)_baseObject).MediaText; }
            set { ((IMediaList)_baseObject).MediaText = value; }
        }

        public ulong length
        {
            get { return ((IMediaList)_baseObject).Length; }
        }
    }

    #endregion

    #region Implementation - IJsLinkStyle

    /// <summary>
    /// Implementation wrapper for IJsLinkStyle
    /// </summary>
    public class JsLinkStyle : JsObject<ILinkStyle>, IJsLinkStyle
    {
        public JsLinkStyle(ILinkStyle baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsStyleSheet sheet
        {
            get { 
                var result = _baseObject.Sheet; 
                return (result != null) ? CreateWrapper<IJsStyleSheet>(result, _engine) : null; 
            }
        }
    }

    #endregion

    #region Implementation - IJsDocumentStyle

    /// <summary>
    /// Implementation wrapper for IJsDocumentStyle
    /// </summary>
    public class JsDocumentStyle : JsObject<IDocumentStyle>, IJsDocumentStyle
    {
        public JsDocumentStyle(IDocumentStyle baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsStyleSheetList styleSheets
        {
            get { 
                var result = _baseObject.StyleSheets; 
                return (result != null) ? CreateWrapper<IJsStyleSheetList>(result, _engine) : null; 
            }
        }
    }   

    #endregion
}

