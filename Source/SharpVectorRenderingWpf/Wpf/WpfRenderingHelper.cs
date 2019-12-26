using System;
using System.Xml;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Renderers.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Rename: WpfDocumentRenderer
    /// </remarks>
    public sealed class WpfRenderingHelper : DependencyObject
    {
        #region Private Fields

        private string _currentLang;
        private string _currentLangName;

        private WpfDrawingRenderer _renderer;

        private IDictionary<string, WpfRenderingBase> _rendererMap;

        // A simple way to prevent use element circular references.
        private ISet<string> _useIdElements;
        private ISet<int> _useElements;

        #endregion

        #region Constructors and Destructor

        public WpfRenderingHelper(WpfDrawingRenderer renderer)
        {
            var cultureInfo = CultureInfo.CurrentCulture;

            _currentLang     = cultureInfo.TwoLetterISOLanguageName;
            _currentLangName = cultureInfo.Name;
            _renderer        = renderer;
            _rendererMap     = new Dictionary<string, WpfRenderingBase>(StringComparer.OrdinalIgnoreCase);
            _useElements     = new HashSet<int>();
            _useIdElements   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

            if (string.Equals(elementName, "use"))
            {
                RenderUseElement(svgElement);
            }
            else
            {
                RenderElement(svgElement);
            }
        }

        public void RenderAs(SvgElement svgElement)
        {
            if (svgElement == null)
            {
                return;
            }

            RenderElementAs(svgElement);
        }

        public void RenderChildren(ISvgElement svgElement)
        {
            if (svgElement == null)
            {
                return;
            }

            string elementName = svgElement.LocalName;

            if (string.Equals(elementName, "switch"))
            {
                RenderSwitchChildren(svgElement);
            }
            else
            {
                RenderElementChildren(svgElement);
            }
        }

        public void RenderMask(ISvgElement svgElement)
        {
            if (svgElement == null)
            {
                return;
            }

            string elementName = svgElement.LocalName;

            if (string.Equals(elementName, "switch"))
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

        private bool BeginUseElement(SvgUseElement element, out int hashCode)
        {
            hashCode = -1;

            string useId = element.Id;
            if (string.IsNullOrWhiteSpace(useId))
            {
                hashCode = element.OuterXml.GetHashCode();
                if (_useElements.Contains(hashCode))
                {
                    return false;
                }

                _useElements.Add(hashCode);
            }
            else
            {
                if (_useIdElements.Contains(useId))
                {
                    return false;
                }
                _useIdElements.Add(useId);
            }

            return true;
        } 

        private bool EndUseElement(SvgUseElement element, int hashCode)
        {
            bool isRemoved = _useElements.Remove(hashCode);
            string useId = element.Id;
            if (string.IsNullOrWhiteSpace(useId))
            {
                //int hashCode = element.OuterXml.GetHashCode();

                return isRemoved;
            }
            return _useIdElements.Remove(useId);
        }

        private void RenderElement(ISvgElement svgElement)
        {
            bool isNotRenderable = !svgElement.IsRenderable;

            if (string.Equals(svgElement.LocalName, "a"))
            {   
            }
            if (isNotRenderable)
            {
                return;
            }

            WpfRenderingBase renderingNode = WpfRendering.Create(svgElement);
            if (renderingNode == null)
            {
                return;
            }

            if (!renderingNode.NeedRender(_renderer))
            {
                //renderingNode.Dispose();
                //renderingNode = null;
                return;
            }

            SvgElement currentElement = (SvgElement)svgElement;

            if (_rendererMap.ContainsKey(currentElement.UniqueId))
            {
                // Might be circular rendering...
                System.Diagnostics.Debug.WriteLine("Circular Object: " + currentElement.LocalName);
                return;
            }

            _rendererMap[currentElement.UniqueId] = renderingNode;
            renderingNode.BeforeRender(_renderer);

            renderingNode.Render(_renderer);

            if (!renderingNode.IsRecursive && svgElement.HasChildNodes)
            {
                RenderChildren(svgElement);
            }

            renderingNode = _rendererMap[currentElement.UniqueId];
            renderingNode.AfterRender(_renderer);

            _rendererMap.Remove(currentElement.UniqueId);

            //renderingNode.Dispose();
            //renderingNode = null;
        }

        private void RenderElementAs(SvgElement svgElement)
        {
            WpfRenderingBase renderingNode = WpfRendering.Create(svgElement);
            if (renderingNode == null)
            {
                return;
            }

            if (!renderingNode.NeedRender(_renderer))
            {
                //renderingNode.Dispose();
                //renderingNode = null;
                return;
            }

            _rendererMap[svgElement.UniqueId] = renderingNode;
            renderingNode.BeforeRender(_renderer);

            renderingNode.Render(_renderer);

            if (!renderingNode.IsRecursive && svgElement.HasChildNodes)
            {
                RenderChildren(svgElement);
            }

            renderingNode = _rendererMap[svgElement.UniqueId];
            renderingNode.AfterRender(_renderer);

            _rendererMap.Remove(svgElement.UniqueId);

            //renderingNode.Dispose();
            //renderingNode = null;
        }

        private void RenderUseElement(ISvgElement svgElement)
        {
            SvgUseElement useElement = (SvgUseElement)svgElement;

            int hashCode = 0; // useElement.OuterXml.GetHashCode();

            if (!this.BeginUseElement(useElement, out hashCode))
            {
                return;
            }

            SvgDocument document = useElement.OwnerDocument;

            XmlElement refEl = useElement.ReferencedElement;
            if (refEl == null)
            {
                this.EndUseElement(useElement, hashCode);
                return;
            }
            XmlElement refElParent = refEl.ParentNode as XmlElement;
            var siblingNode = refEl.PreviousSibling;
            if (siblingNode != null && siblingNode.NodeType == XmlNodeType.Whitespace)
            {
                siblingNode = siblingNode.PreviousSibling;
            }

            // For the external node, the documents are different, and we may not be
            // able to insert this node, so we first import it...
            if (useElement.OwnerDocument != refEl.OwnerDocument)
            {
                var importedNode = useElement.OwnerDocument.ImportNode(refEl, true) as XmlElement;

                if (importedNode != null)
                {
                    var importedSvgElement = importedNode as SvgElement;
                    if (importedSvgElement != null)
                    {
                        importedSvgElement.Imported       = true;
                        importedSvgElement.ImportNode     = refEl as SvgElement;
                        importedSvgElement.ImportDocument = refEl.OwnerDocument as SvgDocument;
                    }

                    refEl = importedNode;
                }
            }
            else
            {
                // For elements/nodes within the same document, clone it.
                refEl = (XmlElement)refEl.CloneNode(true);
            }
            // Reset any ID on the cloned/copied element to avoid duplication of IDs.
            //           refEl.SetAttribute("id", "");

            useElement.OwnerDocument.Static = true;
            useElement.CopyToReferencedElement(refEl);

            XmlElement refSiblingEl = null;
            string useId = null;

            // Compensate for the parent's class and sibling css loss from cloning...
            if (refElParent != null && refElParent.HasAttribute("class"))
            {
                var parentClass = refElParent.GetAttribute("class");
                if (!string.IsNullOrWhiteSpace(parentClass))
                {
                    var parentEl = document.CreateElement(refElParent.LocalName);
                    parentEl.SetAttribute("class", parentClass);

                    parentEl.AppendChild(refEl);

                    refEl = parentEl;
                }
            }
            else if (refElParent != null && siblingNode != null)
            {
                var siblingEl = siblingNode as XmlElement;
                if (siblingEl != null && siblingEl.HasAttribute("class"))
                {
                    var siblingClass = siblingEl.GetAttribute("class");
                    if (!string.IsNullOrWhiteSpace(siblingClass))
                    {
                        refSiblingEl = (XmlElement)siblingEl.CloneNode(true);

                        useElement.AppendChild(refSiblingEl);
                    }
                }
            }
            else
            {
                //useId = useElement.Id;
                //useElement.SetAttribute("id", "");
            }

            useElement.AppendChild(refEl);

            // Now, render the use element...
            this.RenderElement(svgElement);

            if (refSiblingEl != null)
            {
                useElement.RemoveChild(refSiblingEl);
            }
            useElement.RemoveChild(refEl);
            useElement.RestoreReferencedElement(refEl);

            if (useId != null)
            {
                useElement.SetAttribute("id", useId);
            }

            this.EndUseElement(useElement, hashCode);
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
                    if (!ownerDocument.Supports(req, string.Empty))
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
                    if (!ownerDocument.Supports(req, string.Empty))
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
                    if (string.Equals(req, _currentLang, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(req, _currentLangName, StringComparison.OrdinalIgnoreCase))
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
