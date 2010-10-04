using System;
using System.Xml;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Wpf;  

namespace SharpVectors.Converters
{
    public sealed class LinkVisitor : WpfLinkVisitor
    {
        #region Private Fields

        private bool                _isAggregated;
        private GeometryCollection  _aggregatedGeom;

        private SvgStyleableElement _aggregatedFill;

        private Dictionary<string, bool> _dicLinks;

        #endregion

        #region Constructors and Destructor

        public LinkVisitor()
        {
            _dicLinks = new Dictionary<string, bool>();
        }

        #endregion

        #region Public Properties

        public override bool Aggregates
        {
            get
            {
                return true;
            }
        }

        public override bool IsAggregate
        {
            get
            {
                return _isAggregated;
            }
        }

        public override string AggregatedLayerName
        {
            get
            {
                return SvgObject.LinksLayer;
            }
        }

        #endregion

        #region Public Methods

        public override bool Exists(string linkId)
        {
            if (String.IsNullOrEmpty(linkId))
            {
                return false;
            }

            if (_dicLinks != null && _dicLinks.Count != 0)
            {
                return _dicLinks.ContainsKey(linkId);
            }

            return false;
        }

        public override void Initialize(DrawingGroup linkGroup, WpfDrawingContext context)
        {
            if (linkGroup != null)
            {
                SvgLink.SetKey(linkGroup, SvgObject.LinksLayer);
            }
        }

        public override void Visit(DrawingGroup group, SvgAElement element,
            WpfDrawingContext context, float opacity)
        {
            _isAggregated = false;

            if (group == null || element == null || context == null)
            {
                return;
            }

            AddExtraLinkInformation(group, element);

            //string linkId = element.GetAttribute("id");
            string linkId = GetElementName(element);
            if (String.IsNullOrEmpty(linkId))
            {
                return;
            }
            SvgLink.SetKey(group, linkId);

            if (_dicLinks.ContainsKey(linkId))
            {
                _isAggregated = _dicLinks[linkId];
                return;
            }

            string linkAction = element.GetAttribute("onclick");
            if (String.IsNullOrEmpty(linkAction))
            {
                linkAction = element.GetAttribute("onmouseover");
                if (!String.IsNullOrEmpty(linkAction) && 
                    linkAction.StartsWith("parent.svgMouseOverName", StringComparison.OrdinalIgnoreCase))
                {
                    SvgLink.SetAction(group, SvgLinkAction.LinkTooltip);
                }
                else
                {
                    SvgLink.SetAction(group, SvgLinkAction.LinkNone);
                }
            }
            else
            {
                if (linkAction.StartsWith("parent.svgClick", StringComparison.OrdinalIgnoreCase))
                {
                    SvgLink.SetAction(group, SvgLinkAction.LinkPage);
                }
                else if (linkAction.StartsWith("parent.svgOpenHtml", StringComparison.OrdinalIgnoreCase))
                {
                    SvgLink.SetAction(group, SvgLinkAction.LinkHtml);
                }
                else
                {
                    SvgLink.SetAction(group, SvgLinkAction.LinkNone);
                }
            }

            if (!String.IsNullOrEmpty(linkAction))
            {   
                if (linkAction.IndexOf("'Top'") > 0)
                {
                    SvgLink.SetLocation(group, "Top");
                }
                else if (linkAction.IndexOf("'Bottom'") > 0)
                {
                    SvgLink.SetLocation(group, "Bottom");
                }
                else
                {
                    SvgLink.SetLocation(group, "Top");
                }
            }

            this.AggregateChildren(element, context, opacity);
            if (_isAggregated)
            {
                Geometry drawGeometry = null;
                if (_aggregatedGeom.Count == 1)
                {
                    drawGeometry = _aggregatedGeom[0];
                }
                else
                {
                    GeometryGroup geomGroup = new GeometryGroup();
                    geomGroup.FillRule = FillRule.Nonzero;
                    
                    for (int i = 0; i < _aggregatedGeom.Count; i++)
                    {
                        geomGroup.Children.Add(_aggregatedGeom[i]);
                    }

                    drawGeometry = geomGroup;
                }

                WpfSvgPaint fillPaint = new WpfSvgPaint(context, _aggregatedFill, "fill");
                Brush brush = fillPaint.GetBrush(false);

                brush.SetValue(FrameworkElement.NameProperty, linkId + "_Brush");

                GeometryDrawing drawing = new GeometryDrawing(brush, null, drawGeometry);

                group.Children.Add(drawing);                
            }

            _dicLinks.Add(linkId, _isAggregated);
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
            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(" ", String.Empty);
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        #endregion

        #region Private Methods

        private void AddExtraLinkInformation(DrawingGroup group, SvgElement element)
        {
            string linkColor = element.GetAttribute("color");
            if (!String.IsNullOrEmpty(linkColor))
            {
                SvgLink.SetColor(group, linkColor);
            }
            string linkPartsId = element.GetAttribute("partsid");
            if (!String.IsNullOrEmpty(linkPartsId))
            {
                SvgLink.SetPartsId(group, linkPartsId);
            }
            string linkType = element.GetAttribute("type");
            if (!String.IsNullOrEmpty(linkType))
            {
                SvgLink.SetPartsId(group, linkType);
            }
            string linkNumber = element.GetAttribute("num");
            if (!String.IsNullOrEmpty(linkNumber))
            {
                SvgLink.SetPartsId(group, linkNumber);
            }
            string linkPin = element.GetAttribute("pin");
            if (!String.IsNullOrEmpty(linkPin))
            {
                SvgLink.SetPartsId(group, linkPin);
            }
            string linkLineId = element.GetAttribute("lineid");
            if (!String.IsNullOrEmpty(linkLineId))
            {
                SvgLink.SetPartsId(group, linkLineId);
            }
        }

        private void AggregateChildren(SvgAElement aElement, WpfDrawingContext context, float opacity)
        {
            _isAggregated = false;

            if (aElement == null || aElement.ChildNodes == null)
            {
                return;
            }

            string aggregatedFill = aElement.GetAttribute("fill");
            bool isFillFound = !String.IsNullOrEmpty(aggregatedFill);

            SvgStyleableElement paintElement = null;
            if (isFillFound)
            {
                paintElement = aElement;
            }

            XmlNode targetNode = aElement;
            // Check if the children of the link are wrapped in a Group Element...
            if (aElement.ChildNodes.Count == 1)
            {
                SvgGElement groupElement = aElement.ChildNodes[0] as SvgGElement;
                if (groupElement != null)
                {
                    targetNode = groupElement;
                }
            }

            GeometryCollection geomColl = new GeometryCollection();

            foreach (XmlNode node in targetNode.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                // Handle a case where the clip element has "use" element as a child...
                if (String.Equals(node.LocalName, "use"))
                {
                    SvgUseElement useElement = (SvgUseElement)node;

                    XmlElement refEl = useElement.ReferencedElement;
                    if (refEl != null)
                    {
                        XmlElement refElParent = (XmlElement)refEl.ParentNode;
                        useElement.OwnerDocument.Static = true;
                        useElement.CopyToReferencedElement(refEl);
                        refElParent.RemoveChild(refEl);
                        useElement.AppendChild(refEl);

                        foreach (XmlNode useChild in useElement.ChildNodes)
                        {
                            if (useChild.NodeType != XmlNodeType.Element)
                            {
                                continue;
                            }

                            SvgStyleableElement element = useChild as SvgStyleableElement;
                            if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                            {
                                Geometry childPath = WpfRendering.CreateGeometry(element, context.OptimizePath);

                                if (childPath != null)
                                {
                                    if (isFillFound)
                                    {
                                        string elementFill = element.GetAttribute("fill");
                                        if (!String.IsNullOrEmpty(elementFill) &&
                                            !String.Equals(elementFill, aggregatedFill, StringComparison.OrdinalIgnoreCase))
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        aggregatedFill = element.GetAttribute("fill");
                                        isFillFound = !String.IsNullOrEmpty(aggregatedFill);
                                        if (isFillFound)
                                        {
                                            paintElement = element;
                                        }
                                    }

                                    geomColl.Add(childPath);
                                }
                            }
                        }

                        useElement.RemoveChild(refEl);
                        useElement.RestoreReferencedElement(refEl);
                        refElParent.AppendChild(refEl);
                        useElement.OwnerDocument.Static = false;
                    }
                }
                //else if (String.Equals(node.LocalName, "g"))
                //{   
                //}
                else
                {
                    SvgStyleableElement element = node as SvgStyleableElement;
                    if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                    {
                        Geometry childPath = WpfRendering.CreateGeometry(element, context.OptimizePath);

                        if (childPath != null)
                        {
                            if (isFillFound)
                            {
                                string elementFill = element.GetAttribute("fill");
                                if (!String.IsNullOrEmpty(elementFill) &&
                                    !String.Equals(elementFill, aggregatedFill, StringComparison.OrdinalIgnoreCase))
                                {
                                    return;
                                }
                            }
                            else
                            {
                                aggregatedFill = element.GetAttribute("fill");
                                isFillFound = !String.IsNullOrEmpty(aggregatedFill);
                                if (isFillFound)
                                {
                                    paintElement = element;
                                }
                            }

                            geomColl.Add(childPath);
                        }
                    }
                }
            }

            if (geomColl.Count == 0 || paintElement == null)
            {
                return;
            }

            _aggregatedFill = paintElement;
            _aggregatedGeom = geomColl;

            _isAggregated = true;
        }

        #endregion
    }
}
