using System;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfPatternRendering : WpfRendering
    {
        #region Private Fields

        //private bool _isRoot;
        private bool _isRecursive;
        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfPatternRendering(SvgElement element)
            : base(element)
        {
            //_isRoot = false;
            _isRecursive = false;
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

            //if (context.Count == 0)
            //{
            //    _drawGroup = new DrawingGroup();
            //    context.Push(_drawGroup);
            //    context.Root = _drawGroup;
            //}
            //else if (context.Count == 1)
            {
                DrawingGroup currentGroup = context.Peek();

                if (currentGroup == null)
                {
                    throw new InvalidOperationException("An existing group is expected.");
                }

                if (currentGroup == context.Root)
                {
                    if (context.IsFragment)
                    {
                        // Do not add extra layer to fragments...
                        _drawGroup = currentGroup;
                    }
                    else
                    {
                        _drawGroup = new DrawingGroup();
                        SvgObject.SetName(_drawGroup, SvgObject.DrawLayer);
                        if (context.IncludeRuntime)
                        {
                            SvgLink.SetKey(_drawGroup, SvgObject.DrawLayer);
                        }

                        currentGroup.Children.Add(_drawGroup);
                        context.Push(_drawGroup);
                    }
                }
                else
                {
                    _drawGroup = new DrawingGroup();
                    currentGroup.Children.Add(_drawGroup);
                    context.Push(_drawGroup);
                }
            }
            //else
            //{
            //    _drawGroup = new DrawingGroup();
            //    DrawingGroup currentGroup = context.Peek();

            //    if (currentGroup == null)
            //    {
            //        throw new InvalidOperationException("An existing group is expected.");
            //    }

            //    currentGroup.Children.Add(_drawGroup);
            //    context.Push(_drawGroup);
            //}

            SvgPatternElement svgElm = (SvgPatternElement)_svgElement;

            double x = Math.Round(svgElm.X.AnimVal.Value, 4);
            double y = Math.Round(svgElm.Y.AnimVal.Value, 4);
            double width = Math.Round(svgElm.Width.AnimVal.Value, 4);
            double height = Math.Round(svgElm.Height.AnimVal.Value, 4);

            if (width < 0 || height < 0)
            {
                // For invalid dimension, prevent the drawing of the children...
                _isRecursive = true;
                return;
            }

            Rect elmRect = new Rect(x, y, width, height);

//            XmlNode parentNode = _svgElement.ParentNode;

            ISvgFitToViewBox fitToView = svgElm as ISvgFitToViewBox;
            ISvgAnimatedPreserveAspectRatio preserveAspectRatio = null;
            if (fitToView != null && fitToView.PreserveAspectRatio != null)
            {
                preserveAspectRatio = fitToView.PreserveAspectRatio;
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
            var aspectRatio = (preserveAspectRatio != null) ? preserveAspectRatio.AnimVal : null;
            if (aspectRatio != null/* && aspectRatio.Align == SvgPreserveAspectRatioType.None*/)
            {
                FitToViewbox(context, elmRect);

                transform = this.Transform;
                if (transform != null)
                {
                    _drawGroup.Transform = transform;
                }
            }

            //if (!elmRect.IsEmpty && !elmRect.Width.Equals(0) && !elmRect.Height.Equals(0))
            //{
            //    //// Elements such as "pattern" are also rendered by this renderer, so we make sure we are
            //    //// dealing with the root SVG element...
            //    //if (parentNode != null && parentNode.NodeType == XmlNodeType.Document)
            //    //{
            //    //    _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
            //    //}
            //    //else
            //    {
            //        if (transform != null)
            //        {
            //            // We have already applied the transform, which will translate to the start point...
            //            if (transform is TranslateTransform)
            //            {
            //                //_drawGroup.ClipGeometry = new RectangleGeometry(
            //                //    new Rect(0, 0, elmRect.Width, elmRect.Height));
            //            }
            //            else
            //            {
            //                _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
            //            }
            //        }
            //        else
            //        {
            //            _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
            //        }
            //    }
            //}
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            //_isRoot = false;
            _isRecursive = false;

            _drawGroup = null;
        }

        #endregion

        #region Private Methods

        //private void OnAfterRender(WpfDrawingRenderer renderer)
        //{
        //    Debug.Assert(_drawGroup != null);

        //    // Support for Tiny 1.2 viewport-fill property...
        //    if (_svgElement.HasAttribute("viewport-fill"))
        //    {
        //        var viewportFill = _svgElement.GetAttribute("viewport-fill");
        //        if (!string.IsNullOrWhiteSpace(viewportFill))
        //        {
        //            SvgPatternElement svgElm = (SvgPatternElement)_svgElement;

        //            var brush = WpfFill.CreateViewportBrush(svgElm);
        //            if (brush != null)
        //            {
        //                var bounds = new RectangleGeometry(_drawGroup.Bounds);
        //                var drawing = new GeometryDrawing(brush, null, bounds);

        //                _drawGroup.Children.Insert(0, drawing);
        //            }
        //        }
        //    }

        //    WpfDrawingContext context = renderer.Context;

        //    DrawingGroup currentGroup = context.Peek();

        //    if (currentGroup == null || currentGroup != _drawGroup)
        //    {
        //        throw new InvalidOperationException("An existing group is expected.");
        //    }

        //    context.Pop();

        //    ////if (_isRoot && !context.IsFragment)
        //    ////{
        //    ////    this.AdjustViewbox();
        //    ////}
        //    //if (_isRoot || context.IsFragment)
        //    //{
        //    //    return;
        //    //}

        //    //DrawingGroup drawGroup = CreateOuterGroup();
        //    //if (drawGroup == null)
        //    //{
        //    //    return;
        //    //}

        //    //currentGroup = context.Peek();
        //    //if (currentGroup == null || currentGroup.Children.Remove(_drawGroup) == false)
        //    //{
        //    //    return;
        //    //}

        //    //drawGroup.Children.Add(_drawGroup);
        //    //currentGroup.Children.Add(drawGroup);
        //}

        //private DrawingGroup CreateOuterGroup()
        //{
        //    DrawingGroup drawGroup = null;

        //    SvgPatternElement svgElm = (SvgPatternElement)_svgElement;

        //    ISvgAnimatedPreserveAspectRatio animatedAspectRatio = svgElm.PreserveAspectRatio;
        //    if (animatedAspectRatio == null || animatedAspectRatio.AnimVal == null)
        //    {
        //        return drawGroup;
        //    }

        //    double x = svgElm.X.AnimVal.Value;
        //    double y = svgElm.Y.AnimVal.Value;
        //    double width = svgElm.Width.AnimVal.Value;
        //    double height = svgElm.Height.AnimVal.Value;

        //    Rect clipRect = new Rect(x, y, width, height);

        //    if (!clipRect.IsEmpty)
        //    {
        //        drawGroup = new DrawingGroup();
        //        drawGroup.ClipGeometry = new RectangleGeometry(clipRect);
        //    }

        //    return drawGroup;
        //}

        #endregion
    }
}
