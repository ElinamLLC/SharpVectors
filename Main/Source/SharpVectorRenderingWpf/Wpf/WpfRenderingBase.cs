using System;
using System.Windows;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>      
    /// Defines the interface required for a rendering node to interact with the renderer and the SVG DOM
    /// </summary>
    public abstract class WpfRenderingBase : DependencyObject, IDisposable
    {
        #region Private Fields

        protected SvgElement _svgElement;

        #endregion

        #region Constructors and Destructor

        protected WpfRenderingBase(SvgElement element)
        {
            this._svgElement = element;
        }

        ~WpfRenderingBase()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public SvgElement Element
        {
            get { return _svgElement; }
        }

        public virtual bool IsRecursive
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool NeedRender(WpfDrawingRenderer renderer)
        {
            if (_svgElement.GetAttribute("display") == "none")
            {
                return false;
            }

            return true;
        }

        // define empty handlers by default
        public virtual void BeforeRender(WpfDrawingRenderer renderer) { }
        public virtual void Render(WpfDrawingRenderer renderer) { }
        public virtual void AfterRender(WpfDrawingRenderer renderer) { }

        public string GetElementName()
        {
            if (_svgElement == null)
            {
                return String.Empty;
            }
            string elementId = _svgElement.Id;
            if (elementId != null)
            {
                elementId = elementId.Trim();
            }
            if (String.IsNullOrEmpty(elementId))
            {
                return String.Empty;
            }
            if (elementId.Contains("&#x"))
            {
                elementId = HttpUtility.HtmlDecode(elementId);
            }
            if (elementId.Contains("レイヤー"))
            {
                elementId = elementId.Replace("レイヤー", "Layer");
            }
            else if (elementId.Equals("台紙"))
            {
                elementId = "Mount";
            }
            else if (elementId.Equals("キャプション"))
            {
                elementId = "Caption";
            }
            int numberId = 0;
            if (Int32.TryParse(elementId, out numberId))
            {
                return String.Empty;
            }

            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(' ', '_');
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        public static string GetElementName(SvgElement element)
        {
            if (element == null)
            {
                return String.Empty;
            }
            string elementId = element.Id;
            if (elementId != null)
            {
                elementId = elementId.Trim();
            }
            if (String.IsNullOrEmpty(elementId))
            {
                return String.Empty;
            }
            if (elementId.Contains("&#x"))
            {
                elementId = HttpUtility.HtmlDecode(elementId);
            }
            if (elementId.Contains("レイヤー"))
            {
                elementId = elementId.Replace("レイヤー", "Layer");
            }
            else if (elementId.Equals("イラスト"))
            {
                elementId = "Illustration";
            }
            else if (elementId.Equals("台紙"))
            {
                elementId = "Mount";
            }
            else if (elementId.Equals("キャプション"))
            {
                elementId = "Caption";
            }
            else if (elementId.Equals("細線"))
            {
                elementId = "ThinLine";
            }
            int numberId = 0;
            if (Int32.TryParse(elementId, out numberId))
            {
                return String.Empty;
            }
      
            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(' ', '_');
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
