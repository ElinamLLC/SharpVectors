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

            SvgPatternElement svgElm = (SvgPatternElement)_svgElement;

            double x      = Math.Round(svgElm.X.AnimVal.Value, 4);
            double y      = Math.Round(svgElm.Y.AnimVal.Value, 4);
            double width  = Math.Round(svgElm.Width.AnimVal.Value, 4);
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
    }
}
