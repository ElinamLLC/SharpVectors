using System;   

namespace SharpVectors.Scripting
{
    /// <summary>
    /// IJsSvgWindow
    /// </summary>
    public interface IJsSvgWindow
    {
        long innerWidth { get; }
        long innerHeight { get; }
        IJsSvgDocument document { get; }
        IJsSvgDocument svgDocument { get; }
        IJsStyleSheet defaultStyleSheet { get; }

        string setTimeout(object scriptOrClosure, ulong delay);
        void clearTimeout(string token);
        string setInterval(object scriptOrClosure, ulong delay);
        void clearInterval(string token);
        void alert(string message);
        void setSrc(string newURL);
        string getSrc();
        string printNode(IJsNode node);
        IJsNode parseXML(string xml, IJsDocument owner);
        void registerEval(object closure);
    }    
}
