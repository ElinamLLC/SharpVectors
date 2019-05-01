using System;
using System.Xml;
using System.Diagnostics;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfSvgRendering : WpfRendering
    {
        #region Private Fields

        private bool _isRoot;
        private bool _isRecursive;
        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfSvgRendering(SvgElement element)
            : base(element)
        {
            _isRoot      = false;
            _isRecursive = false;

            var svgRootElm = element as SvgSvgElement;
            if (svgRootElm != null)
            {
                _isRoot = svgRootElm.IsOuterMost;
            }
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive
        {
            get {
                return _isRecursive;
            }
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            WpfDrawingContext context = renderer.Context;
            _drawGroup = new DrawingGroup();

            if (context.Count == 0)
            {
                context.Push(_drawGroup);
                context.Root = _drawGroup;
            }
            else if (context.Count == 1)
            {        
                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }
                if (currentGroup == context.Root && !context.IsFragment)
                {
                    SvgObject.SetName(_drawGroup, SvgObject.DrawLayer);
                    if (context.IncludeRuntime)
                    {
                        SvgLink.SetKey(_drawGroup, SvgObject.DrawLayer);
                    }
                }

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);
            }
            else
            {
                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);
            }

            SvgSvgElement svgElm = (SvgSvgElement)_svgElement;

            double x      = Math.Round(svgElm.X.AnimVal.Value,      4);
            double y      = Math.Round(svgElm.Y.AnimVal.Value,      4);
            double width  = Math.Round(svgElm.Width.AnimVal.Value,  4);
            double height = Math.Round(svgElm.Height.AnimVal.Value, 4);

            Rect elmRect  = new Rect(x, y, width, height);

            XmlNode parentNode = _svgElement.ParentNode;

            ISvgFitToViewBox fitToView = svgElm as ISvgFitToViewBox;
            if (fitToView != null)
            {
                ISvgAnimatedRect animRect = fitToView.ViewBox;
                if (animRect != null)
                {
                    ISvgRect viewRect = animRect.AnimVal;
                    if (viewRect != null)
                    {
                        Rect wpfViewRect = WpfConvert.ToRect(viewRect);
                        if (!wpfViewRect.IsEmpty && wpfViewRect.Width > 0 && wpfViewRect.Height > 0)
                        {
                            elmRect = wpfViewRect;
                        }
                    }
                }
            } 

            Transform transform = null;
            if (parentNode.NodeType != XmlNodeType.Document)
            {
                FitToViewbox(context, elmRect);

                transform = this.Transform;
                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }
            }

            if (!elmRect.IsEmpty && !elmRect.Width.Equals(0) && !elmRect.Height.Equals(0))
            {   
                // Elements such as "pattern" are also rendered by this renderer, so we make sure we are
                // dealing with the root SVG element...
                if (parentNode != null && parentNode.NodeType == XmlNodeType.Document)
                {
                    _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                }
                else
                {
                    if (transform != null)
                    {
                        // We have already applied the transform, which will translate to the start point...
                        if (transform is TranslateTransform)
                        {
                            _drawGroup.ClipGeometry = new RectangleGeometry(
                                new Rect(0, 0, elmRect.Width, elmRect.Height));
                        }
                        else
                        {
                            _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                        }
                    }
                    else
                    {
                        _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                    }
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);

            var settings = _context.Settings;

            if (_isRoot && _drawGroup.ClipGeometry != null)
            {
                if (settings.IgnoreRootViewbox)
                {
                    _drawGroup.ClipGeometry = null;
                }
                else if (settings.EnsureViewboxSize)
                {
                    var bounds = _drawGroup.ClipGeometry.Bounds;
                    if (!bounds.IsEmpty)
                    {
                        using (var ctx = _drawGroup.Open())
                        {
                            ctx.DrawRectangle(null, new Pen(Brushes.Transparent, 1), bounds);
                        }
                    }
                }
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);

            Debug.Assert(_drawGroup != null);

            WpfDrawingContext context = renderer.Context;

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            context.Pop();

            if (_isRoot && !context.IsFragment)
            {
                this.AdjustViewbox();
            }

            if (_isRoot || context.IsFragment)
            {
                return;
            }

            DrawingGroup drawGroup = CreateOuterGroup();
            if (drawGroup == null)
            {
                return;
            }

            currentGroup = context.Peek();
            if (currentGroup == null || currentGroup.Children.Remove(_drawGroup) == false)
            {
                return;
            }

            drawGroup.Children.Add(_drawGroup);
            currentGroup.Children.Add(drawGroup);
        }

        private void AdjustViewbox()
        {
            if (!_isRoot || _drawGroup == null)
            {
                return;
            }

            Rect bounds = _context.Bounds;

            bounds.Union(_drawGroup.Bounds);

            if (_drawGroup.ClipGeometry != null)
            {
                bounds.Union(_drawGroup.ClipGeometry.Bounds);
            }

            if (bounds.IsEmpty || _context.Settings == null)
            {
                return;
            }

            if (_context.Settings.EnsureViewboxPosition)
            {
                Point ptTopLeft = bounds.TopLeft;

                if (!Point.Equals(ptTopLeft, new Point(0, 0)))
                {
                    TranslateTransform translate = new TranslateTransform(-ptTopLeft.X, -ptTopLeft.Y);

                    Transform transform = _drawGroup.Transform;
                    if (transform != null && !transform.Value.IsIdentity)
                    {
                        TransformGroup groupTransform = new TransformGroup();
                        groupTransform.Children.Add(transform);
                        groupTransform.Children.Add(translate);

                        _drawGroup.Transform = groupTransform;
                    }
                    else
                    {
                        _drawGroup.Transform = translate;
                    }
                }
            }
        }

        private DrawingGroup CreateOuterGroup()
        {
            DrawingGroup drawGroup = null;

            SvgSvgElement svgElm = (SvgSvgElement)_svgElement;

            ISvgAnimatedPreserveAspectRatio animatedAspectRatio = svgElm.PreserveAspectRatio;
            if (animatedAspectRatio == null || animatedAspectRatio.AnimVal == null)
            {
                return drawGroup;
            }

            double x      = svgElm.X.AnimVal.Value;
            double y      = svgElm.Y.AnimVal.Value;
            double width  = svgElm.Width.AnimVal.Value;
            double height = svgElm.Height.AnimVal.Value;

            Rect clipRect = new Rect(x, y, width, height);

            if (!clipRect.IsEmpty)
            {
                drawGroup = new DrawingGroup();
                drawGroup.ClipGeometry = new RectangleGeometry(clipRect);
            }

            return drawGroup;
        }

        #endregion
    }
}
