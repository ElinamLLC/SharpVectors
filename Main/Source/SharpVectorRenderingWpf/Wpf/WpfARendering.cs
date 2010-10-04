using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfARendering : WpfRendering
    {
        #region Private Fields

        private bool                _isLayer;
        private bool                _isAggregated;
        private DrawingGroup        _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfARendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive
        {
            get
            {
                return _isAggregated;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            _isAggregated = false;

            if (_svgElement.FirstChild == _svgElement.LastChild)
            {
                SvgGElement gElement = _svgElement.FirstChild as SvgGElement;
                if (gElement != null)
                {
                    string elementId = gElement.GetAttribute("id");
                    if (!String.IsNullOrEmpty(elementId) &&
                        String.Equals(elementId, "IndicateLayer", StringComparison.OrdinalIgnoreCase))
                    {
                        WpfDrawingContext context = renderer.Context;

                        DrawingGroup animationGroup = context.Links;
                        if (animationGroup != null)
                        {
                            context.Push(animationGroup);
                        }

                        _isLayer = true;
                    }
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            _isAggregated = false;

            if (_isLayer)
            {
                base.Render(renderer);

                return;
            }

            WpfDrawingContext context = renderer.Context;

            Geometry clipGeom   = this.ClipGeometry;
            Transform transform = this.Transform;

            float opacityValue  = -1;

            SvgAElement element = (SvgAElement)_svgElement;
            string opacity = element.GetPropertyValue("opacity");
            if (opacity != null && opacity.Length > 0)
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            WpfLinkVisitor linkVisitor = context.LinkVisitor;

            if (linkVisitor != null || clipGeom != null || transform != null || opacityValue >= 0)
            {
                _drawGroup = new DrawingGroup();

                string elementId = this.GetElementName();
                if (!String.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
                {
                    _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);

                    context.RegisterId(elementId);

                    if (context.IncludeRuntime)
                    {
                        SvgObject.SetId(_drawGroup, elementId);
                    }
                }

                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                if (linkVisitor != null && linkVisitor.Aggregates && context.Links != null)
                {
                    if (!linkVisitor.Exists(elementId))
                    {
                        context.Links.Children.Add(_drawGroup);
                    }
                }
                else
                {
                    currentGroup.Children.Add(_drawGroup);
                }

                context.Push(_drawGroup);

                if (clipGeom != null)
                {
                    _drawGroup.ClipGeometry = clipGeom;
                }

                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }

                if (opacityValue >= 0)
                {
                    _drawGroup.Opacity = opacityValue;
                }

                if (linkVisitor != null)
                {
                    linkVisitor.Visit(_drawGroup, element, context, opacityValue);

                    _isAggregated = linkVisitor.IsAggregate;
                }
            }

            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_isLayer)
            {
                WpfDrawingContext context = renderer.Context;

                context.Pop();

                base.AfterRender(renderer);

                return;
            }

            if (_drawGroup != null)
            {
                WpfDrawingContext context = renderer.Context;

                if (context.IncludeRuntime)
                {
                    // Add the element/object type...
                    SvgObject.SetType(_drawGroup, SvgObjectType.Link);

                    // Add title for tooltips, if any...
                    SvgTitleElement titleElement = this.GetTitleElement();
                    if (titleElement != null)
                    {
                        string titleValue = titleElement.InnerText;
                        if (!String.IsNullOrEmpty(titleValue))
                        {
                            SvgObject.SetTitle(_drawGroup, titleValue);
                        }
                    }
                }

                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null || currentGroup != _drawGroup)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                context.Pop();

                // If not aggregated by a link visitor, we remove it from the links/animation and 
                // add it to the main drawing stack...
                if (!_isAggregated)
                {
                    if (context.Links.Children.Remove(_drawGroup))
                    {
                        currentGroup = context.Peek();

                        currentGroup.Children.Add(_drawGroup);
                    }
                }
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}
