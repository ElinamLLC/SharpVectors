using System;
using System.Xml;
using System.Collections.Generic;

using System.Windows.Media;

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
            if (string.IsNullOrWhiteSpace(linkId))
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
            if (string.IsNullOrWhiteSpace(linkId))
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
            if (string.IsNullOrWhiteSpace(linkAction))
            {
                linkAction = element.GetAttribute("onmouseover");
                if (!string.IsNullOrWhiteSpace(linkAction) && 
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

            if (!string.IsNullOrWhiteSpace(linkAction))
            {   
                if (linkAction.IndexOf("'Top'", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    SvgLink.SetLocation(group, "Top");
                }
                else if (linkAction.IndexOf("'Bottom'", StringComparison.OrdinalIgnoreCase) > 0)
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

                SvgObject.SetName(brush, linkId + "_Brush");

                GeometryDrawing drawing = new GeometryDrawing(brush, null, drawGeometry);

                group.Children.Add(drawing);                
            }

            _dicLinks.Add(linkId, _isAggregated);
        }

        public static string GetElementName(SvgElement element)
        {
            if (element == null)
            {
                return string.Empty;
            }
            string elementId = element.Id;
            if (elementId != null)
            {
                elementId = elementId.Trim();
            }
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return string.Empty;
            }
            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(" ", string.Empty);
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        #endregion

        #region Private Methods

        private void AddExtraLinkInformation(DrawingGroup group, SvgElement element)
        {
            string linkColor = element.GetAttribute("color");
            if (!string.IsNullOrWhiteSpace(linkColor))
            {
                SvgLink.SetColor(group, linkColor);
            }
            string linkPartsId = element.GetAttribute("partsid");
            if (!string.IsNullOrWhiteSpace(linkPartsId))
            {
                SvgLink.SetPartsId(group, linkPartsId);
            }
            string linkType = element.GetAttribute("type");
            if (!string.IsNullOrWhiteSpace(linkType))
            {
                SvgLink.SetPartsId(group, linkType);
            }
            string linkNumber = element.GetAttribute("num");
            if (!string.IsNullOrWhiteSpace(linkNumber))
            {
                SvgLink.SetPartsId(group, linkNumber);
            }
            string linkPin = element.GetAttribute("pin");
            if (!string.IsNullOrWhiteSpace(linkPin))
            {
                SvgLink.SetPartsId(group, linkPin);
            }
            string linkLineId = element.GetAttribute("lineid");
            if (!string.IsNullOrWhiteSpace(linkLineId))
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
            bool isFillFound = !string.IsNullOrWhiteSpace(aggregatedFill);

            SvgStyleableElement paintElement = null;
            if (isFillFound)
            {
                paintElement = aElement;
            }

            XmlNode targetNode = aElement;
            // Check if the children of the link are wrapped in a Group Element...
            if (aElement.ChildNodes.Count == 1)
            {
                var groupElement = aElement.ChildNodes[0] as SvgGElement;
                if (groupElement != null)
                {
                    targetNode = groupElement;
                }
            }

            WpfDrawingSettings settings = context.Settings;

            GeometryCollection geomColl = new GeometryCollection();

            foreach (XmlNode node in targetNode.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                // Handle a case where the clip element has "use" element as a child...
                if (string.Equals(node.LocalName, "use"))
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

                            var element = useChild as SvgStyleableElement;
                            if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                            {
                                Geometry childPath = CreateGeometry(element, settings.OptimizePath);

                                if (childPath != null)
                                {
                                    if (isFillFound)
                                    {
                                        string elementFill = element.GetAttribute("fill");
                                        if (!string.IsNullOrWhiteSpace(elementFill) &&
                                            !string.Equals(elementFill, aggregatedFill, StringComparison.OrdinalIgnoreCase))
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        aggregatedFill = element.GetAttribute("fill");
                                        isFillFound = !string.IsNullOrWhiteSpace(aggregatedFill);
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
                //else if (string.Equals(node.LocalName, "g"))
                //{   
                //}
                else
                {
                    var element = node as SvgStyleableElement;
                    if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                    {
                        Geometry childPath = CreateGeometry(element, settings.OptimizePath);

                        if (childPath != null)
                        {
                            if (isFillFound)
                            {
                                string elementFill = element.GetAttribute("fill");
                                if (!string.IsNullOrWhiteSpace(elementFill) &&
                                    !string.Equals(elementFill, aggregatedFill, StringComparison.OrdinalIgnoreCase))
                                {
                                    return;
                                }
                            }
                            else
                            {
                                aggregatedFill = element.GetAttribute("fill");
                                isFillFound = !string.IsNullOrWhiteSpace(aggregatedFill);
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
