using System.Xml;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Utils;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfPatternFill : WpfFill
    {
        #region Private Fields

        private bool _isUserSpace;
//        private XmlElement oldParent;
        private SvgPatternElement _patternElement;
        private SvgPatternElement _renderedElement;

        #endregion

        #region Constructors and Destructor

        public WpfPatternFill(SvgPatternElement patternElement)
        {
            _isUserSpace    = false;
            _patternElement = patternElement;
        }

        #endregion

        #region Public Properties

        public override bool IsUserSpace
        {
            get {
                return _isUserSpace;
            }
        }

        public override WpfFillType FillType
        {
            get {
                return WpfFillType.Pattern;
            }
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context, Transform viewTransform)
        {
            DrawingGroup image = GetImage(context);
            if (image == null || image.Bounds.Width.Equals(0) || image.Bounds.Height.Equals(0))
            {
                return null;
            }
            bool isUserSpace = true;
            Rect bounds = elementBounds;
            if (_renderedElement.PatternContentUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
            {
                bounds = new Rect(0, 0, 1, 1);
                isUserSpace = false;
            }
            Rect destRect = GetDestRect(bounds);  
            // Check for validity of the brush...
            if (destRect.Width.Equals(0) || destRect.Height.Equals(0) || destRect.IsEmpty)
            {
                return null;
            }

            // Apply a scale if needed
            if (isUserSpace && image.Transform != null)
            {
                ISvgFitToViewBox fitToView = _renderedElement as ISvgFitToViewBox;
                if (fitToView != null && fitToView.ViewBox != null)
                {
                    ISvgAnimatedRect animRect = fitToView.ViewBox;
                    ISvgRect viewRect = animRect.AnimVal;
                    if (viewRect != null)
                    {
                        Rect wpfViewRect = WpfConvert.ToRect(viewRect);
                        if (!wpfViewRect.IsEmpty && wpfViewRect.Width > 0 && wpfViewRect.Height > 0)
                        {
                            var scaleX = elementBounds.Width > 0 ? destRect.Width / wpfViewRect.Width : 1;
                            var scaleY = elementBounds.Height > 0 ? destRect.Height / wpfViewRect.Height : 1;

                            if (!scaleX.Equals(1) || !scaleY.Equals(1))
                            {
                                var currentTransform = image.Transform as ScaleTransform;
                                if (currentTransform != null)
                                {
                                    image.Transform = new ScaleTransform(scaleX, scaleY);
                                }
                            }
                        }
                    }
                }
            }

            DrawingBrush tb  = new DrawingBrush(image);
            tb.Viewbox       = destRect;
            tb.Viewport      = destRect;
            //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
            //tb.Viewport = new Rect(0, 0, bounds.Width, bounds.Height);
            tb.ViewboxUnits  = BrushMappingMode.Absolute;
            tb.ViewportUnits = isUserSpace ? BrushMappingMode.Absolute : BrushMappingMode.RelativeToBoundingBox;
            tb.TileMode      = TileMode.Tile;
//            tb.Stretch       = isUserSpace ? Stretch.Fill : Stretch.Uniform;

            if (isUserSpace)
            {
                MatrixTransform transform = GetTransformMatrix(image.Bounds, isUserSpace);
                if (transform != null && !transform.Matrix.IsIdentity)
                {
                    tb.Transform = transform;
                }
            }
            else
            {
                MatrixTransform transform = GetTransformMatrix(bounds, isUserSpace);
                if (transform != null && !transform.Matrix.IsIdentity)
                {
                    tb.RelativeTransform = transform;
                }
            }

            return tb;
        }

        #endregion

        #region Private Methods

//        private SvgSvgElement MoveIntoSvgElement()
//        {
//            SvgDocument doc = _patternElement.OwnerDocument;
//            SvgSvgElement svgElm = doc.CreateElement("", "svg", SvgDocument.SvgNamespace) as SvgSvgElement;

//            XmlNodeList children = _patternElement.Children;
//            if (children.Count > 0)
//            {
//                oldParent = children[0].ParentNode as XmlElement;
//            }

//            for (int i = 0; i < children.Count; i++)
//            {
//                svgElm.AppendChild(children[i]);
//            }

//            if (_patternElement.HasAttribute("viewBox"))
//            {
//                svgElm.SetAttribute("viewBox", _patternElement.GetAttribute("viewBox"));
//            }
//            //svgElm.SetAttribute("x", "0");
//            //svgElm.SetAttribute("y", "0");
//            svgElm.SetAttribute("x",      _patternElement.GetAttribute("x"));
//            svgElm.SetAttribute("y",      _patternElement.GetAttribute("y"));
//            svgElm.SetAttribute("width",  _patternElement.GetAttribute("width"));
//            svgElm.SetAttribute("height", _patternElement.GetAttribute("height"));

//            if (_patternElement.PatternContentUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
//            {
////                svgElm.SetAttribute("viewBox", "0 0 1 1");
//            }
//            else
//            {
//                _isUserSpace = true;
//            }

//            _patternElement.AppendChild(svgElm);

//            return svgElm;
//        }

        //private void MoveOutOfSvgElement(SvgSvgElement svgElm)
        //{
        //    while (svgElm.ChildNodes.Count > 0)
        //    {
        //        oldParent.AppendChild(svgElm.ChildNodes[0]);
        //    }

        //    _patternElement.RemoveChild(svgElm);
        //}

        private void PrepareTargetPattern()
        {
            var renderedElement = _patternElement;

            if (renderedElement.ReferencedElement != null)
            {
                SvgPatternElement svgElm = _patternElement.CloneNode(true) as SvgPatternElement;

                renderedElement = _patternElement.ReferencedElement;
                if (svgElm.IsEmpty)
                {
                    XmlNodeList children = renderedElement.Children;
                    if (children.Count > 0)
                    {
                        foreach (SvgElement element in children)
                        {
                            svgElm.AppendChild(element.Clone());
                        }
                    }
                }

                var inheritedAttributes = renderedElement.Attributes;
                foreach (XmlAttribute attr in inheritedAttributes)
                {
                    if (!svgElm.HasAttribute(attr.Name))
                    {
                        svgElm.SetAttribute(attr.Name, attr.Value);
                    }
                }

                _patternElement.AppendChild(svgElm);

//                renderedElement = svgElm;

                while (renderedElement.ReferencedElement != null)
                {
                    renderedElement = renderedElement.ReferencedElement;
                    inheritedAttributes = renderedElement.Attributes;
                    foreach (XmlAttribute attr in inheritedAttributes)
                    {
                        if (!svgElm.HasAttribute(attr.Name))
                        {
                            svgElm.SetAttribute(attr.Name, attr.Value);
                        }
                    }
                }

                svgElm.Id = "";
                renderedElement = svgElm;
            }

            _renderedElement = renderedElement;
        }

        private DrawingGroup GetImage(WpfDrawingContext context)
        {
            PrepareTargetPattern();

            WpfDrawingRenderer renderer = new WpfDrawingRenderer();
            renderer.Window = _renderedElement.OwnerDocument.Window as SvgWindow;

//            WpfDrawingSettings settings = context.Settings.Clone();
            WpfDrawingSettings settings = context.Settings;
            bool isTextAsGeometry = settings.TextAsGeometry;
            settings.TextAsGeometry = true;
            WpfDrawingContext patternContext = new WpfDrawingContext(true, settings);
            patternContext.Name = "Pattern";

            patternContext.Initialize(null, context.FontFamilyVisitor, null);

            if (_renderedElement.PatternContentUnits.AnimVal.Equals((ushort)SvgUnitType.ObjectBoundingBox))
            {
                //                svgElm.SetAttribute("viewBox", "0 0 1 1");
            }
            else
            {
                _isUserSpace = true;
            }

            //SvgSvgElement elm = MoveIntoSvgElement();
            //renderer.Render(elm, patternContext);

            //SvgPatternElement patternElement = _patternElement.ReferencedElement;
            //if (patternElement != null)
            //{
            //    if (patternElement.ReferencedElement != null)
            //    {
            //        _renderedElement = patternElement.ReferencedElement;
            //        renderer.RenderAs(patternElement.ReferencedElement, patternContext);
            //    }
            //    else
            //    {
            //        _renderedElement = patternElement;
            //        renderer.RenderAs(patternElement, patternContext);
            //    }
            //}
            //else
            //{
            //    _renderedElement = _patternElement;
            //    renderer.RenderAs(_patternElement, patternContext);
            //}
            renderer.RenderAs(_renderedElement, patternContext);
            DrawingGroup rootGroup = renderer.Drawing;

            //MoveOutOfSvgElement(elm);

            if (_renderedElement != null && _renderedElement != _patternElement)
            {
                _patternElement.RemoveChild(_renderedElement);
            }

            settings.TextAsGeometry = isTextAsGeometry;

            if (rootGroup.Children.Count == 1)
            {
                var childGroup = rootGroup.Children[0] as DrawingGroup;
                if (childGroup != null)
                {
                    return childGroup;
                }
            }

            return rootGroup;
        }

        private double CalcPatternUnit(SvgLength length, SvgLengthDirection dir, Rect bounds)
        {
            if (_patternElement.PatternUnits.AnimVal.Equals((ushort)SvgUnitType.UserSpaceOnUse))
            {
                _isUserSpace = true;
                return length.Value;
            }
            double calcValue = length.ValueInSpecifiedUnits;
            if (dir == SvgLengthDirection.Horizontal)
            {
                calcValue *= bounds.Width;
            }
            else
            {
                calcValue *= bounds.Height;
            }
            if (length.UnitType == SvgLengthType.Percentage)
            {
                calcValue /= 100F;
            }

            return calcValue;
        }

        private Rect GetDestRect(Rect bounds)
        {
            Rect result = new Rect(0, 0, 0, 0);

            result.X = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Y = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            result.Width = CalcPatternUnit(_patternElement.Width.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Height = CalcPatternUnit(_patternElement.Height.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            return result;
        }

        private MatrixTransform GetTransformMatrix(Rect bounds, bool isUserSpace)
        {
            SvgMatrix svgMatrix = ((SvgTransformList)_patternElement.PatternTransform.AnimVal).TotalMatrix;

            MatrixTransform transformMatrix = new MatrixTransform((float)svgMatrix.A, (float)svgMatrix.B, (float)svgMatrix.C,
                (float)svgMatrix.D, (float)svgMatrix.E, (float)svgMatrix.F);

            //Matrix transformMatrix = new Matrix(svgMatrix.A, svgMatrix.B, svgMatrix.C,
            //    svgMatrix.D, svgMatrix.E, svgMatrix.F);

            //double translateX = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength,
            //    SvgLengthDirection.Horizontal, bounds);
            //double translateY = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength,
            //    SvgLengthDirection.Vertical, bounds);

            //transformMatrix.TranslatePrepend(translateX, translateY);
            ////transformMatrix.Value.TranslatePrepend(translateX, translateY);

            //return new MatrixTransform(transformMatrix);

            return transformMatrix;
        }

        #endregion
    }
}
