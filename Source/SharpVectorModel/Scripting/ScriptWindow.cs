using System;
using System.Xml;

using SharpVectors.Dom;
using SharpVectors.Dom.Css;
using SharpVectors.Dom.Events;
using SharpVectors.Dom.Stylesheets;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Views;
using SharpVectors.Scripting;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgWindow

    /// <summary>
    /// Implementation wrapper for IJsSvgWindow
    /// </summary>
    public class JsSvgWindow : JsObject<ISvgWindow>, IJsSvgWindow
    {
        public JsSvgWindow(ISvgWindow baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public string setTimeout(object scriptOrClosure, ulong delay)
        {
            //return ScriptTimerMonitor.CreateMonitor((VsaScriptEngine)Engine, 
            //    (ISvgWindow)_baseObject, scriptOrClosure, delay, false);
            return string.Empty;
        }

        public void clearTimeout(string token)
        {
            //ScriptTimerMonitor.ClearMonitor(token);
        }

        public string setInterval(object scriptOrClosure, ulong delay)
        {
            //return ScriptTimerMonitor.CreateMonitor((VsaScriptEngine)Engine, 
            //    (ISvgWindow)_baseObject, scriptOrClosure, delay, true);
            return string.Empty;
        }

        public void clearInterval(string token)
        {
            //ScriptTimerMonitor.ClearMonitor(token);
        }

        public void alert(string message)
        {
            _baseObject.Alert(message);
        }

        public void setSrc(string newURL)
        {
            _baseObject.Source = newURL;
        }

        public string getSrc()
        {
            return _baseObject.Source;
        }

        public string printNode(IJsNode node)
        {
            return _baseObject.PrintNode((XmlNode)node.BaseObject);
        }

        public IJsNode parseXML(string xml, IJsDocument owner)
        {
            object result = _baseObject.ParseXML(xml, (XmlDocument)owner.BaseObject);
            return (result != null) ? CreateWrapper<IJsNode>(result, _engine) : null;
        }

        public void registerEval(object scriptFunction)
        {
            //JScriptEngine js = ((JScriptEngine)Engine);
            //js.EvaluateFunction = (ScriptFunction)scriptFunction;
        }

        public IJsSvgDocument document
        {
            get {
                object result = _baseObject.Document;
                return (result != null) ? CreateWrapper<IJsSvgDocument>(result, _engine) : null;
            }
        }

        public IJsSvgDocument svgDocument
        {
            get {
                object result = _baseObject.Document;
                return (result != null) ? CreateWrapper<IJsSvgDocument>(result, _engine) : null;
            }
        }

        public IJsStyleSheet defaultStyleSheet
        {
            get {
                object result = _baseObject.DefaultStyleSheet;
                return (result != null) ? CreateWrapper<IJsStyleSheet>(result, _engine) : null;
            }
        }

        public long innerWidth
        {
            get {
                return _baseObject.InnerWidth;
            }
        }

        public long innerHeight
        {
            get {
                return _baseObject.InnerHeight;
            }
        }
    }  

    #endregion
}

