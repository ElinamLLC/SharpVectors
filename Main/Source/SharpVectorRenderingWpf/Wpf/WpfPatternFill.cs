using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Css;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfPatternFill : WpfFill
    {
        #region Private Fields

        private XmlElement oldParent;
        private SvgPatternElement _patternElement;

        #endregion

        #region Constructors and Destructor

        public WpfPatternFill(SvgPatternElement patternElement)
		{
			_patternElement = patternElement;
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context)
        {
            Rect bounds = new Rect(0, 0, 1, 1);
            Drawing image = GetImage(context);
            Rect destRect = GetDestRect(bounds);            

            DrawingBrush tb  = new DrawingBrush(image);
            //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
            //tb.Viewport = new Rect(0, 0, destRect.Width, destRect.Height);
            tb.Viewbox       = destRect;
            tb.Viewport      = destRect;
            tb.ViewboxUnits  = BrushMappingMode.Absolute;
            tb.ViewportUnits = BrushMappingMode.Absolute;
            tb.TileMode      = TileMode.Tile;

            MatrixTransform transform = GetTransformMatrix(image.Bounds);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                tb.Transform = transform;
            }

            return tb;
        }

        #endregion

        #region Private Methods

        private SvgSvgElement MoveIntoSvgElement()
        {
            SvgDocument doc = _patternElement.OwnerDocument;
            SvgSvgElement svgElm = doc.CreateElement("", "svg", SvgDocument.SvgNamespace) as SvgSvgElement;

            XmlNodeList children = _patternElement.Children;
            if (children.Count > 0)
            {
                oldParent = children[0].ParentNode as XmlElement;
            }

            for (int i = 0; i < children.Count; i++)
            {
                svgElm.AppendChild(children[i]);
            }

            if (_patternElement.HasAttribute("viewBox"))
            {
                svgElm.SetAttribute("viewBox", _patternElement.GetAttribute("viewBox"));
            }
            //svgElm.SetAttribute("x", "0");
            //svgElm.SetAttribute("y", "0");
            svgElm.SetAttribute("x",      _patternElement.GetAttribute("x"));
            svgElm.SetAttribute("y",      _patternElement.GetAttribute("y"));
            svgElm.SetAttribute("width",  _patternElement.GetAttribute("width"));
            svgElm.SetAttribute("height", _patternElement.GetAttribute("height"));

            if (_patternElement.PatternContentUnits.AnimVal.Equals(SvgUnitType.ObjectBoundingBox))
            {
                svgElm.SetAttribute("viewBox", "0 0 1 1");
            }

            _patternElement.AppendChild(svgElm);

            return svgElm;
        }

        private void MoveOutOfSvgElement(SvgSvgElement svgElm)
        {
            while (svgElm.ChildNodes.Count > 0)
            {
                oldParent.AppendChild(svgElm.ChildNodes[0]);
            }

            _patternElement.RemoveChild(svgElm);
        }

        private Drawing GetImage(WpfDrawingContext context)
        {
            WpfDrawingRenderer renderer = new WpfDrawingRenderer();
            renderer.Window = _patternElement.OwnerDocument.Window as SvgWindow;

            WpfDrawingSettings settings = context.Settings.Clone();
            settings.TextAsGeometry = true;
            WpfDrawingContext patternContext = new WpfDrawingContext(true,
                settings);

            patternContext.Initialize(null, context.FontFamilyVisitor, null);

            SvgSvgElement elm = MoveIntoSvgElement();

            renderer.Render((SvgElement)elm, patternContext);
            Drawing img = renderer.Drawing;

            MoveOutOfSvgElement(elm);

            return img;
        }

        private double CalcPatternUnit(SvgLength length, SvgLengthDirection dir, Rect bounds)
        {
            if (_patternElement.PatternUnits.AnimVal.Equals(SvgUnitType.UserSpaceOnUse))
            {
                return length.Value;
            }
            else
            {
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

        private MatrixTransform GetTransformMatrix(Rect bounds)
        {
            SvgMatrix svgMatrix = 
                ((SvgTransformList)_patternElement.PatternTransform.AnimVal).TotalMatrix;

            MatrixTransform transformMatrix = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);

            double translateX = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            double translateY = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            transformMatrix.Matrix.TranslatePrepend(translateX, translateY);

            return transformMatrix;
        }

        #endregion
    }
}
