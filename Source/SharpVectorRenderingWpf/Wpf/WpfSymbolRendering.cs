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
    public sealed class WpfSymbolRendering : WpfRendering
    {
        #region Private Fields

        private bool _isRecursive;
        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfSymbolRendering(SvgElement element)
            : base(element)
        {
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

            SvgSymbolElement svgElm = (SvgSymbolElement)_svgElement;

            double x      = Math.Round(svgElm.X.AnimVal.Value, 4);
            double y      = Math.Round(svgElm.Y.AnimVal.Value, 4);
            double width  = Math.Round(svgElm.Width.AnimVal.Value, 4);
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

            FitToViewbox(context, elmRect);

            Transform transform = this.Transform;
            if (transform != null)
            {
                _drawGroup.Transform = transform;
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
            // Register this drawing with the Drawing-Document...
            if (_drawGroup != null)
            {
                this.Rendered(_drawGroup);
            }
            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            this.OnAfterRender(renderer);

            base.AfterRender(renderer);
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _isRecursive = false;
            _drawGroup   = null;
        }

        #endregion

        #region Private Methods

        private void OnAfterRender(WpfDrawingRenderer renderer)
        {
            Debug.Assert(_drawGroup != null);

            WpfDrawingContext context = renderer.Context;

            DrawingGroup currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
            {
                throw new InvalidOperationException("An existing group is expected.");
            }

            context.Pop();

            DrawingGroup drawGroup = CreateOuterGroup();
            if (drawGroup == null)
            {
                return;
            }

            SvgSymbolElement symbolElement = (SvgSymbolElement)_svgElement;

            var transform = _drawGroup.Transform;

            Transform useTransform = null;

            XmlNode childNode = _svgElement.FirstChild;
            string childName = childNode.Name;
            try
            {
                // If none of the following attribute exists, an exception is thrown...
                double x      = symbolElement.X.AnimVal.Value;
                double y      = symbolElement.Y.AnimVal.Value;
                double width  = symbolElement.Width.AnimVal.Value;
                double height = symbolElement.Height.AnimVal.Value;
                if (width > 0 && height > 0)
                {
                    Rect elementBounds = new Rect(x, y, width, height);

                    useTransform = this.UseFitToViewbox(symbolElement, elementBounds);
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            if (useTransform != null)
            {
                if (transform == null)
                {
                    transform = useTransform;
                }
                else // if (transform != null)
                {
                    transform = Combine(useTransform, transform, true);
                }
            }

            _drawGroup.Transform = transform;

            currentGroup = context.Peek();
            if (currentGroup == null || currentGroup.Children.Remove(_drawGroup) == false)
            {
                return;
            }

            drawGroup.Children.Add(_drawGroup);
            currentGroup.Children.Add(drawGroup);
        }

        private Transform UseFitToViewbox(SvgElement svgElement, Rect elementBounds)
        {
            if (svgElement == null)
            {
                return null;
            }
            ISvgFitToViewBox fitToView = svgElement as ISvgFitToViewBox;
            if (fitToView == null)
            {
                return null;
            }

            SvgPreserveAspectRatio spar = (SvgPreserveAspectRatio)fitToView.PreserveAspectRatio.AnimVal;

            SvgRect viewBox = (SvgRect)fitToView.ViewBox.AnimVal;
            SvgRect rectToFit = new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height);

            double[] transformArray = spar.FitToViewBox(viewBox, rectToFit);

            double translateX = Math.Round(transformArray[0], 4);
            double translateY = Math.Round(transformArray[1], 4);
            double scaleX     = Math.Round(transformArray[2], 4);
            double scaleY     = Math.Round(transformArray[3], 4);

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            if (!translateX.Equals(0.0) || !translateY.Equals(0.0))
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if (!scaleX.Equals(1.0) || !scaleY.Equals(1.0))
            {
                scaleMatrix = new ScaleTransform(scaleX, scaleY);
            }

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                if (translateMatrix.Value.IsIdentity && scaleMatrix.Value.IsIdentity)
                {
                    return null;
                }
                if (translateMatrix.Value.IsIdentity)
                {
                    return scaleMatrix;
                }
                if (scaleMatrix.Value.IsIdentity)
                {
                    return translateMatrix;
                }
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                return transformGroup;
            }
            if (translateMatrix != null)
            {
                return translateMatrix;
            }
            if (scaleMatrix != null)
            {
                return scaleMatrix;
            }
            return null;
        }

        private DrawingGroup CreateOuterGroup()
        {
            DrawingGroup drawGroup = null;

            SvgSymbolElement svgElm = (SvgSymbolElement)_svgElement;

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
