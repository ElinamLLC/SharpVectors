using System;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Gdi
{
    public sealed class GdiRenderingHelper
    {
        #region Private Fields

        private string _currentLang;
        private GdiGraphicsRenderer _renderer;
        //private Dictionary<ISvgElement, GdiRenderingBase> _rendererMap;
        private Stack<GdiRenderingBase> _rendererMap;

        #endregion

        #region Constructors and Destructor

        public GdiRenderingHelper(GdiGraphicsRenderer renderer)
        {
            _currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            _renderer    = renderer;
            //_rendererMap = new Dictionary<ISvgElement, GdiRenderingBase>();
            _rendererMap = new Stack<GdiRenderingBase>();
        }

        #endregion

        #region Public Methods

        public void Render(ISvgDocument docElement)
        {
            SvgSvgElement root = docElement.RootElement as SvgSvgElement;

            if (root != null)
            {
                this.Render(root);
            }
        }

        public void Render(SvgDocument docElement)
        {
            SvgSvgElement root = docElement.RootElement as SvgSvgElement;

            if (root != null)
            {
                this.Render(root);
            }
        }

        public void Render(ISvgElement svgElement)
        {
            if (svgElement == null)
            {
                return;
            }
            
            string elementName = svgElement.LocalName;

            if (String.Equals(elementName, "use"))
            {
                RenderUseElement(svgElement);
            }
            else
            {
                RenderElement(svgElement);
            }
        }

        public void RenderChildren(ISvgElement svgElement)
        {
            if (svgElement == null)
            {
                return;
            }

            string elementName = svgElement.LocalName;

            if (String.Equals(elementName, "switch"))
            {
                RenderSwitchChildren(svgElement);
            }
            else
            {
                RenderElementChildren(svgElement);
            }
        }

        #endregion

        #region Private Methods

        private void RenderElement(ISvgElement svgElement)
        {
            bool isNotRenderable = !svgElement.IsRenderable || String.Equals(svgElement.LocalName, "a");

            if (isNotRenderable)
            {
                return;
            }

            GdiRenderingBase renderingNode = GdiRendering.Create(svgElement);
            if (renderingNode == null)
            {
                return;
            }

            if (!renderingNode.NeedRender(_renderer))
            {
                renderingNode.Dispose();
                renderingNode = null;
                return;
            }

            //_rendererMap[svgElement] = renderingNode;
            _rendererMap.Push(renderingNode);
            renderingNode.BeforeRender(_renderer);

            renderingNode.Render(_renderer);

            if (!renderingNode.IsRecursive && svgElement.HasChildNodes)
            {
                RenderChildren(svgElement);
            }

            //renderingNode = _rendererMap[svgElement];
            renderingNode = _rendererMap.Pop();
            Debug.Assert(renderingNode.Element == svgElement);
            renderingNode.AfterRender(_renderer);

            //_rendererMap.Remove(svgElement);

            renderingNode.Dispose();
            renderingNode = null;
        }

        private void RenderUseElement(ISvgElement svgElement)
        {
            SvgUseElement useElement = (SvgUseElement)svgElement;

            XmlElement refEl = useElement.ReferencedElement;
            if (refEl == null)
                return;
            XmlElement refElParent = (XmlElement)refEl.ParentNode;
            useElement.OwnerDocument.Static = true;
            useElement.CopyToReferencedElement(refEl);
            refElParent.RemoveChild(refEl);
            useElement.AppendChild(refEl);

            this.RenderElement(svgElement);
            
            useElement.RemoveChild(refEl);
            useElement.RestoreReferencedElement(refEl);
            refElParent.AppendChild(refEl);
            useElement.OwnerDocument.Static = false;
        }

        private void RenderElementChildren(ISvgElement svgElement)
        {
            foreach (XmlNode node in svgElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                SvgElement element = node as SvgElement;
                if (element != null)
                {
                    this.Render(element);
                }
            }
        }

        private void RenderSwitchChildren(ISvgElement svgElement)
        {
            // search through all child elements and find one that passes all tests
            foreach (XmlNode node in svgElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                SvgElement element = node as SvgElement;
                ISvgTests testsElement = node as ISvgTests;
                if (element != null && testsElement != null && PassesSwitchAllTest(testsElement))
                {
                    this.Render(element);

                    // make sure we only render the first element that passes
                    break;
                }
            }
        }

        private bool PassesSwitchAllTest(ISvgTests element)
        {
            SvgDocument ownerDocument = ((SvgElement)element).OwnerDocument;

            bool requiredFeatures = true;
            if (element.RequiredFeatures.NumberOfItems > 0)
            {
                foreach (string req in element.RequiredFeatures)
                {
                    if (!ownerDocument.Supports(req, String.Empty))
                    {
                        requiredFeatures = false;
                        break;
                    }
                }
            }
            if (!requiredFeatures)
            {
                return false;
            }

            bool requiredExtensions = true;
            if (element.RequiredExtensions.NumberOfItems > 0)
            {
                foreach (string req in element.RequiredExtensions)
                {
                    if (!ownerDocument.Supports(req, String.Empty))
                    {
                        requiredExtensions = false;
                        break;
                    }
                }
            }
            if (!requiredExtensions)
            {
                return false;
            }

            bool systemLanguage = true;
            if (element.SystemLanguage.NumberOfItems > 0)
            {
                systemLanguage = false; 
                // TODO: or if one of the languages indicated by user preferences exactly 
                // equals a prefix of one of the languages given in the value of this 
                // parameter such that the first tag character following the prefix is "-".

                foreach (string req in element.SystemLanguage)
                {
                    if (String.Equals(req, _currentLang, StringComparison.OrdinalIgnoreCase))
                    {
                        systemLanguage = true;
                    }
                }
            }

            return systemLanguage;
        }

        #endregion
    }
}
