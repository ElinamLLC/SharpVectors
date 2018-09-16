using System;
using System.Windows;
using System.Text.RegularExpressions;

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

        protected WpfDrawingContext _context;

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
        public virtual void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }
            _context = renderer.Context;
        }

        public virtual void Render(WpfDrawingRenderer renderer) { }
        public virtual void AfterRender(WpfDrawingRenderer renderer) { }

        public string GetElementName()
        {
            return GetElementName(_svgElement, _context);
        }

        public static string GetElementName(SvgElement element, WpfDrawingContext context = null)
        {
            if (element == null)
            {
                return string.Empty;
            }
            if (context != null && context.IDVisitor != null)
            {
                return context.IDVisitor.Visit(element, context);
            }
            string elementId = element.Id;
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return string.Empty;
            }
            elementId = elementId.Trim();
            if (IsValidIdentifier(elementId))
            {
                return elementId;
            }

            return Regex.Replace(elementId, @"[^[0-9a-zA-Z]]*", "_");
        }

        public string GetElementClass()
        {
            return GetElementClassName(_svgElement, _context);
        }

        public static string GetElementClassName(SvgElement element, WpfDrawingContext context = null)
        {
            if (element == null)
            {
                return string.Empty;
            }
            if (context != null && context.ClassVisitor != null)
            {
                return context.ClassVisitor.Visit(element, context);
            }

            string className = (element as ISvgStylable)?.ClassName?.BaseVal?.Trim();
            return string.IsNullOrWhiteSpace(className) ? string.Empty : className;
        }

        public static bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return false;
            }

            if (!IsIdentifierStart(identifier[0]))
            {
                return false;
            }

            for (int i = 1; i < identifier.Length; i++)
            {
                if (!IsIdentifierPart(identifier[i]))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsIdentifierStart(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || char.IsLetter(c);
        }

        public static bool IsIdentifierPart(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                || c == '_' || (c >= '0' && c <= '9') || char.IsLetter(c);
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
