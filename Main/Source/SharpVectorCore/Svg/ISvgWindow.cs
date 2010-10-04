using System;
using System.IO;
using System.Xml;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgWindow
    {
        IStyleSheet DefaultStyleSheet { get; }
        ISvgDocument Document { get; }
        long InnerHeight { get; }
        long InnerWidth { get; }
        string Source { get; set; }
        XmlDocumentFragment ParseXML(string source, XmlDocument document);
        string PrintNode(XmlNode node);
        void Alert(string message);
        ISvgRenderer Renderer { get; }
        DirectoryInfo WorkingDir { get; }
        /*void GetURL (string uri, EventListener callback);	*/
        /*void PostURL (string uri, string data, EventListener callback, string mimeType, string contentEncoding);*/
    }
}
