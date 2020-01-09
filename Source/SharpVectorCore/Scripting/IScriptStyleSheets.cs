using System;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Scripting
{            
    /// <summary>
    /// IJsStyleSheet
    /// </summary>
    public interface IJsStyleSheet : IScriptableObject<IStyleSheet>
    {
        string type { get; }
        bool disabled { get; set; }
        IJsNode ownerNode { get; }
        IJsStyleSheet parentStyleSheet { get; }
        string href { get; }
        string title { get; }
        IJsMediaList media { get; }
    }

    /// <summary>
    /// IJsStyleSheetList
    /// </summary>
    public interface IJsStyleSheetList : IScriptableObject<IStyleSheetList>
    {
        ulong length { get; }

        IJsStyleSheet item(ulong index);
    }

    /// <summary>
    /// IJsMediaList
    /// </summary>
    public interface IJsMediaList : IScriptableObject<IMediaList>
    {
        string mediaText { get; set; }
        ulong length { get; }

        string item(ulong index);
        void deleteMedium(string oldMedium);
        void appendMedium(string newMedium);
    }

    /// <summary>
    /// IJsLinkStyle
    /// </summary>
    public interface IJsLinkStyle : IScriptableObject<ILinkStyle>
    {
        IJsStyleSheet sheet { get; }
    }

    /// <summary>
    /// IJsDocumentStyle
    /// </summary>
    public interface IJsDocumentStyle : IScriptableObject<IDocumentStyle>
    {
        IJsStyleSheetList styleSheets { get; }
    }    
}
