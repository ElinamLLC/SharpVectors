using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;

using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfPathRendering : WpfRendering
    {
        #region Constructors and Destructor

        public WpfPathRendering(SvgElement element)
            : base(element)
		{
		}

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            WpfDrawingContext context = renderer.Context;

            SetQuality(context);
            SetTransform(context);
            SetMask(context);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context = renderer.Context;

            SvgRenderingHint hint = _svgElement.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (String.Equals(_svgElement.ParentNode.LocalName, "clipPath") &&
                !context.RenderingClipRegion)
            {
                return;
            }

            SvgStyleableElement styleElm = (SvgStyleableElement)_svgElement;

            string sVisibility = styleElm.GetPropertyValue("visibility");
            string sDisplay    = styleElm.GetPropertyValue("display");
            if (String.Equals(sVisibility, "hidden") || String.Equals(sDisplay, "none"))
            {
                return;
            }

            DrawingGroup drawGroup = context.Peek();
            Debug.Assert(drawGroup != null);

            Geometry geometry = CreateGeometry(_svgElement, context.OptimizePath);

            if (geometry != null && !geometry.IsEmpty())
            {
                SetClip(context);

                WpfSvgPaint fillPaint = new WpfSvgPaint(context, styleElm, "fill");

                string fileValue = styleElm.GetAttribute("fill");

                Brush brush = fillPaint.GetBrush();

                WpfSvgPaint strokePaint = new WpfSvgPaint(context, styleElm, "stroke");
                Pen pen = strokePaint.GetPen();

                if (brush != null || pen != null)
                {
                    Transform transform = this.Transform;
                    if (transform != null && !transform.Value.IsIdentity)
                    {
                        geometry.Transform = transform;
                        if (brush != null)
                        {
                            Transform brushTransform = brush.Transform;
                            if (brushTransform == null || brushTransform == Transform.Identity)
                            {
                                brush.Transform = transform;
                            }
                        }
                    }
                    else
                    {
                        transform = null; // render any identity transform useless...
                    }

                    GeometryDrawing drawing = new GeometryDrawing(brush, pen, geometry);

                    string elementId = this.GetElementName();
                    if (!String.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
                    {
                        drawing.SetValue(FrameworkElement.NameProperty, elementId);

                        context.RegisterId(elementId);

                        if (context.IncludeRuntime)
                        {
                            SvgObject.SetId(drawing, elementId);
                        }
                    }

                    Brush maskBrush = this.Masking;
                    Geometry clipGeom = this.ClipGeometry;
                    if (clipGeom != null || maskBrush != null)
                    {
                        //Geometry clipped = Geometry.Combine(geometry, clipGeom,
                        //    GeometryCombineMode.Exclude, null);

                        //if (clipped != null && !clipped.IsEmpty())
                        //{
                        //    geometry = clipped;
                        //}
                        DrawingGroup clipMaskGroup = new DrawingGroup();

                        Rect geometryBounds = geometry.Bounds;

                        if (clipGeom != null)
                        {   
                            clipMaskGroup.ClipGeometry = clipGeom;

                            SvgUnitType clipUnits = this.ClipUnits;
                            if (clipUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                Rect drawingBounds = geometryBounds;

                                if (transform != null)
                                {
                                    drawingBounds = transform.TransformBounds(drawingBounds);
                                }

                                TransformGroup transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                transformGroup.Children.Add(
                                    new ScaleTransform(drawingBounds.Width, drawingBounds.Height)); 
                                transformGroup.Children.Add(
                                    new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                                clipGeom.Transform = transformGroup;
                            }
                            else
                            {   
                                if (transform != null)
                                {    
                                    clipGeom.Transform = transform;
                                }
                            }
                        }
                        if (maskBrush != null)
                        {
                            SvgUnitType maskUnits = this.MaskUnits;
                            if (maskUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                Rect drawingBounds = geometryBounds;

                                if (transform != null)
                                {
                                    drawingBounds = transform.TransformBounds(drawingBounds);
                                }

                                TransformGroup transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                transformGroup.Children.Add(
                                    new ScaleTransform(drawingBounds.Width, drawingBounds.Height));
                                transformGroup.Children.Add(
                                    new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                                DrawingGroup maskGroup = ((DrawingBrush)maskBrush).Drawing as DrawingGroup;
                                if (maskGroup != null)
                                {
                                    DrawingCollection maskDrawings = maskGroup.Children;
                                    for (int i = 0; i < maskDrawings.Count; i++)
                                    {
                                        Drawing maskDrawing = maskDrawings[i];
                                        GeometryDrawing maskGeomDraw = maskDrawing as GeometryDrawing;
                                        if (maskGeomDraw != null)
                                        {
                                            if (maskGeomDraw.Brush != null)
                                            {
                                                ConvertColors(maskGeomDraw.Brush);
                                            }
                                            if (maskGeomDraw.Pen != null)
                                            {
                                                ConvertColors(maskGeomDraw.Pen.Brush);
                                            }
                                        }
                                    }
                                }

                                //if (transformGroup != null)
                                //{
                                //    drawingBounds = transformGroup.TransformBounds(drawingBounds);
                                //}

                                //maskBrush.Viewbox = drawingBounds;
                                //maskBrush.ViewboxUnits = BrushMappingMode.Absolute;

                                //maskBrush.Stretch = Stretch.Uniform;

                                //maskBrush.Viewport = drawingBounds;
                                //maskBrush.ViewportUnits = BrushMappingMode.Absolute;

                                maskBrush.Transform = transformGroup; 
                            }
                            else
                            {
                                if (transform != null)
                                {
                                    maskBrush.Transform = transform;
                                }
                            }

                            clipMaskGroup.OpacityMask = maskBrush;
                        }

                        clipMaskGroup.Children.Add(drawing);
                        drawGroup.Children.Add(clipMaskGroup);
                    }
                    else
                    {
                        drawGroup.Children.Add(drawing);
                    }  
                }
            }

            RenderMarkers(renderer, styleElm, context);
        }

        //==========================================================================
        private static float AlphaComposition(Color color)
        {
            float max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
            float min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

            return (min + max) / 2.0f;
        }

        //==========================================================================
        private static float AlphaComposition(Brush brush)
        {
            float alphaValue = 1.0f;

            if (brush != null)
            {  
                if (brush is SolidColorBrush)
                {
                    float nextValue = AlphaComposition((brush as SolidColorBrush).Color);
                    if (nextValue > 0 && nextValue < 1)
                    {
                        alphaValue = nextValue;
                    }
                }
                else if (brush is GradientBrush)
                {
                    foreach (GradientStop gradient_stop in (brush as GradientBrush).GradientStops)
                    {
                        float nextValue = AlphaComposition(gradient_stop.Color);
                        if (nextValue > 0 && nextValue < 1)
                        {
                            alphaValue = nextValue;
                        }
                    }
                }
                //else if (brush is DrawingBrush)
                //{
                //    ConvertColors((brush as DrawingBrush).Drawing);
                //}
                else
                    throw new NotSupportedException();

            }

            return alphaValue;
        }

        //==========================================================================
        private static Color ConvertColor(Color color)
        {
            float max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
            float min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

            return Color.FromScRgb((min + max) / 2.0f, color.ScR, color.ScG, color.ScB);
        }

        //==========================================================================
        private static void ConvertColors(Brush brush)
        {
            if (brush != null)
            {
                SolidColorBrush solidBrush = null;
                GradientBrush gradientBrush = null;

                if (DynamicCast.Cast(brush, out solidBrush))
                {  
                    solidBrush.Color = ConvertColor(solidBrush.Color);
                }
                else if (DynamicCast.Cast(brush, out gradientBrush))
                {
                    GradientStopCollection stopColl = gradientBrush.GradientStops;

                    foreach (GradientStop stop in stopColl)
                    {
                        stop.Color = ConvertColor(stop.Color);
                    }
                }
                //else if (brush is DrawingBrush)
                //{
                //    ConvertColors((brush as DrawingBrush).Drawing);
                //}
                else
                    throw new NotSupportedException();

            }
        }

        #endregion
    }
}
