using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfPathRendering : WpfRendering
    {
        #region Private Fields

        private bool _setBrushOpacity;
        private bool _isLineSegment;
        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfPathRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            if (renderer == null)
            {
                return;
            }

            _isLineSegment   = false;         
            _setBrushOpacity = true;

            WpfDrawingContext context = renderer.Context;

            //SetQuality(context);
            //SetTransform(context);
            //SetMask(context);

//            _drawGroup = new DrawingGroup();

            SvgStyleableElement styleElm = (SvgStyleableElement)_svgElement;

            float opacityValue = -1;
            bool isStyleOpacity = false;

            string opacity = styleElm.GetAttribute("opacity");
            if (string.IsNullOrWhiteSpace(opacity))
            {
                opacity = styleElm.GetPropertyValue("opacity");
                if (!string.IsNullOrWhiteSpace(opacity))
                {
                    isStyleOpacity = true;
                }
            }
            if (!string.IsNullOrWhiteSpace(opacity))
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
                if (isStyleOpacity && (opacityValue >= 0 && opacityValue < 1))
                {
                    _setBrushOpacity = false;
                    if (styleElm.HasAttribute("fill-opacity") || (styleElm.HasAttribute("style") 
                        && styleElm.GetAttribute("style").Contains("fill-opacity")))
                    {
                        _setBrushOpacity = true;
                    }
                }
            }
            //string eVisibility = _svgElement.GetAttribute("visibility");
            //string eDisplay    = _svgElement.GetAttribute("display");
            //if (string.Equals(eVisibility, "hidden") || string.Equals(eDisplay, "none"))
            //{
            //    opacityValue = 0;
            //}
            //else
            //{
            //    string sVisibility = styleElm.GetPropertyValue("visibility");
            //    string sDisplay = styleElm.GetPropertyValue("display");
            //    if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            //    {
            //        opacityValue = 0;
            //    }
            //}
            string sVisibility = styleElm.GetPropertyValue("visibility");
            if (string.IsNullOrWhiteSpace(sVisibility))
            {
                sVisibility = _svgElement.GetAttribute("visibility");
            }
            string sDisplay = styleElm.GetPropertyValue("display");
            if (string.IsNullOrWhiteSpace(sDisplay))
            {
                sDisplay = _svgElement.GetAttribute("display");
            }
            if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            {
                opacityValue = 0;
            }

            Transform pathTransform = this.Transform;
            if (pathTransform != null && !pathTransform.Value.IsIdentity)
            {
                if (_drawGroup == null)
                {
                    _drawGroup = new DrawingGroup();
                }
                _drawGroup.Transform = pathTransform;
            }
            else
            {
                pathTransform = null; // render any identity transform useless...
            }

            Geometry pathClip = this.ClipGeometry;
            if (pathClip != null && !pathClip.IsEmpty())
            {
                if (_drawGroup == null)
                {
                    _drawGroup = new DrawingGroup();
                }
                _drawGroup.ClipGeometry = pathClip;
            }
            else
            {
                pathClip = null; // render any empty geometry useless...
            }
            Brush pathMask = this.Masking;
            if (pathMask != null)
            {
                if (_drawGroup == null)
                {
                    _drawGroup = new DrawingGroup();
                }
                _drawGroup.OpacityMask = pathMask;
            }

            if (pathTransform != null || pathClip != null || pathMask != null || (opacityValue >= 0 && opacityValue < 1))
            {
                if (_drawGroup == null)
                {
                    _drawGroup = new DrawingGroup();
                }
                if ((opacityValue >= 0 && opacityValue < 1))
                {
                    _drawGroup.Opacity = opacityValue;
                }

                DrawingGroup curGroup = _context.Peek();
                Debug.Assert(curGroup != null);
                if (curGroup != null)
                {
                    curGroup.Children.Add(_drawGroup);
                    context.Push(_drawGroup);
                }
            }
            else
            {
                _drawGroup = null;
            }

            if (_drawGroup != null)
            {
                string elementClass = this.GetElementClass();
                if (!string.IsNullOrWhiteSpace(elementClass) && context.IncludeRuntime)
                {
                    SvgObject.SetClass(_drawGroup, elementClass);
                }

                string elementId = this.GetElementName();
                if (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
                {
                    SvgObject.SetName(_drawGroup, elementId);

                    context.RegisterId(elementId);

                    if (context.IncludeRuntime)
                    {
                        SvgObject.SetId(_drawGroup, elementId);
                    }
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);

            if (_drawGroup != null)
            {
                this.RenderGroup(renderer);
            }
            else
            {
                this.RenderPath(renderer);
            }
        }

        private void RenderGroup(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context = renderer.Context;

            SvgRenderingHint hint = _svgElement.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            var parentNode = _svgElement.ParentNode;
            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (string.Equals(parentNode.LocalName, "clipPath") &&
                !context.RenderingClipRegion)
            {
                return;
            }

            SvgStyleableElement styleElm = (SvgStyleableElement)_svgElement;

            //string sVisibility = styleElm.GetPropertyValue("visibility");
            //string sDisplay    = styleElm.GetPropertyValue("display");
            //if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            //{
            //    return;
            //}

            DrawingGroup drawGroup = context.Peek();
            Debug.Assert(drawGroup != null);

            Geometry geometry = CreateGeometry(_svgElement, context.OptimizePath);

            if (geometry == null || geometry.IsEmpty())
            {
                return;
            }

            var bounds = geometry.Bounds;
            if (string.Equals(_svgElement.LocalName, "line", StringComparison.Ordinal))
            {
                _isLineSegment = true;
            }
            else if (string.Equals(_svgElement.LocalName, "rect", StringComparison.Ordinal))
            {
                _isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
            }
            else if (string.Equals(_svgElement.LocalName, "path", StringComparison.Ordinal))
            {
                _isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
            }

            context.UpdateBounds(bounds);

//                SetClip(context);

            WpfSvgPaint fillPaint = new WpfSvgPaint(context, styleElm, "fill");

//            string fileValue = styleElm.GetAttribute("fill");

            Brush brush = fillPaint.GetBrush(geometry, _setBrushOpacity);
            if (brush == null)
            {
                WpfSvgPaint fallbackPaint = fillPaint.WpfFallback;
                if (fallbackPaint != null)
                {
                    brush = fallbackPaint.GetBrush(geometry, _setBrushOpacity);
                }
            }
            bool isFillTransmable = fillPaint.IsFillTransformable;

            WpfSvgPaint strokePaint = new WpfSvgPaint(context, styleElm, "stroke");
            Pen pen = strokePaint.GetPen(geometry, _setBrushOpacity);

            // By the SVG Specifications:
            // Keyword 'objectBoundingBox' should not be used when the geometry of the applicable 
            // element has no width or no height, such as the case of a horizontal or vertical line, 
            // even when the line has actual thickness when viewed due to having a non-zero stroke 
            // width since stroke width is ignored for bounding box calculations. When the geometry
            // of the applicable element has no width or height and 'objectBoundingBox' is specified, 
            // then the given effect (e.g., a gradient) will be ignored.
            if (pen != null && _isLineSegment && strokePaint.FillType == WpfFillType.Gradient)
            {
                WpfGradientFill gradientFill = (WpfGradientFill)strokePaint.PaintServer;
                if (gradientFill.IsUserSpace == false)
                {
                    bool invalidGrad = false;
                    if (string.Equals(_svgElement.LocalName, "line", StringComparison.Ordinal))
                    {
                        LineGeometry lineGeometry = geometry as LineGeometry;
                        if (lineGeometry != null)
                        {
                            invalidGrad = SvgObject.IsEqual(lineGeometry.EndPoint.X, lineGeometry.StartPoint.X)
                                || SvgObject.IsEqual(lineGeometry.EndPoint.Y, lineGeometry.StartPoint.Y);
                        }
                    }
                    else
                    {
                        invalidGrad = true;
                    }

                    if (invalidGrad)
                    {
                        // Brush is not likely inherited, we need to support fallback too
                        WpfSvgPaint fallbackPaint = strokePaint.WpfFallback;
                        if (fallbackPaint != null)
                        {
                            pen.Brush = fallbackPaint.GetBrush(geometry, _setBrushOpacity);
                        }
                        else
                        {
                            var scopePaint = strokePaint.GetScopeStroke();
                            if (scopePaint != null)
                            {
                                if (scopePaint != strokePaint)
                                {
                                    pen.Brush = scopePaint.GetBrush(geometry, _setBrushOpacity);
                                }
                                else
                                {
                                    pen.Brush = null;
                                }
                            }
                            else
                            {
                                pen.Brush = null;
                            }
                        }
                    }
                }
            }

            if (_paintContext != null)
            {
                _paintContext.Fill   = fillPaint;
                _paintContext.Stroke = strokePaint;
                _paintContext.Tag    = geometry;
            }

            if (brush != null || pen != null)
            {
                Transform transform = this.Transform;

                GeometryDrawing drawing = new GeometryDrawing(brush, pen, geometry);

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
                    //DrawingGroup clipMaskGroup = new DrawingGroup();

                    Rect geometryBounds = geometry.Bounds;

                    if (clipGeom != null)
                    {   
                        //clipMaskGroup.ClipGeometry = clipGeom;

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
                            transformGroup.Children.Add(new ScaleTransform(drawingBounds.Width, drawingBounds.Height)); 
                            transformGroup.Children.Add(new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                            clipGeom.Transform = transformGroup;
                        }
                        else
                        {   
                            if (transform != null)
                            {    
                                clipGeom.Transform = transform;

                                // For element transform, we prefer applying the transform to the
                                // element instead of the group, if the clipping region is also transformed.
                                if (drawGroup == _drawGroup && drawGroup.Transform == transform)
                                {
                                    if (IsNullOrIdentity(geometry.Transform))
                                    {
                                        geometry.Transform = transform;
                                        drawGroup.Transform = null;
                                    }
                                }
                            }
                        }
                    }
                    if (maskBrush != null)
                    {
                        DrawingBrush drawingBrush = (DrawingBrush)maskBrush;

                        SvgUnitType maskUnits = this.MaskUnits;
                        SvgUnitType maskContentUnits = this.MaskContentUnits;
                        if (maskUnits == SvgUnitType.ObjectBoundingBox)
                        {
                            Rect drawingBounds = geometryBounds;

                            if (transform != null)
                            {
                                drawingBounds = transform.TransformBounds(drawingBounds);
                            }
                            DrawingGroup maskGroup = drawingBrush.Drawing as DrawingGroup;
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

                            if (maskContentUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                TransformGroup transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                var scaleTransform = new ScaleTransform(drawingBounds.Width, drawingBounds.Height);
                                transformGroup.Children.Add(scaleTransform);
                                var translateTransform = new TranslateTransform(drawingBounds.X, drawingBounds.Y);
                                transformGroup.Children.Add(translateTransform);

                                Matrix scaleMatrix = new Matrix();
                                Matrix translateMatrix = new Matrix();

                                scaleMatrix.Scale(drawingBounds.Width, drawingBounds.Height);
                                translateMatrix.Translate(drawingBounds.X, drawingBounds.Y);

                                Matrix matrix = Matrix.Multiply(scaleMatrix, translateMatrix);
                                //maskBrush.Transform = transformGroup; 
                                maskBrush.Transform = new MatrixTransform(matrix); 
                            }
                            else
                            {
                                drawingBrush.Viewbox = drawingBounds;
                                drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;

                                drawingBrush.Stretch = Stretch.Uniform;

                                drawingBrush.Viewport = drawingBounds;
                                drawingBrush.ViewportUnits = BrushMappingMode.Absolute;
                            }
                        }
                        else
                        {
                            if (transform != null)
                            {
                                maskBrush.Transform = transform;
                            }
                        }

                        //clipMaskGroup.OpacityMask = maskBrush;
                    }

                    //clipMaskGroup.Children.Add(drawing);
                    //drawGroup.Children.Add(clipMaskGroup);
                    drawGroup.Children.Add(drawing);
                }
                else
                {
                    drawGroup.Children.Add(drawing);
                }  
            }
            // If this is not the child of a "marker", then try rendering a marker...
            if (!string.Equals(parentNode.LocalName, "marker"))
            {
                RenderMarkers(renderer, styleElm, context);
            }

            // Register this drawing with the Drawing-Document...
            this.Rendered(drawGroup);
        }

        private void RenderPath(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context = renderer.Context;

            SvgRenderingHint hint = _svgElement.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping)
            {
                return;
            }
            var parentNode = _svgElement.ParentNode;
            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (string.Equals(parentNode.LocalName, "clipPath") &&
                !context.RenderingClipRegion)
            {
                return;
            }

            SvgStyleableElement styleElm = (SvgStyleableElement)_svgElement;

            //string sVisibility = styleElm.GetPropertyValue("visibility");
            //string sDisplay    = styleElm.GetPropertyValue("display");
            //if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none"))
            //{
            //    return;
            //}

            DrawingGroup drawGroup = context.Peek();
            Debug.Assert(drawGroup != null);

            Geometry geometry = CreateGeometry(_svgElement, context.OptimizePath);

            string elementId = this.GetElementName();
            string elementClass = this.GetElementClass();

            GeometryDrawing drawing = null;

            if (geometry == null || geometry.IsEmpty())
            {
                return;
            }

            var bounds = geometry.Bounds;
            if (string.Equals(_svgElement.LocalName, "line", StringComparison.Ordinal))
            {
                _isLineSegment = true;
            }
            else if (string.Equals(_svgElement.LocalName, "rect", StringComparison.Ordinal))
            {
                _isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
            }
            else if (string.Equals(_svgElement.LocalName, "path", StringComparison.Ordinal))
            {
                _isLineSegment = bounds.Width.Equals(0) || bounds.Height.Equals(0);
            }

            context.UpdateBounds(geometry.Bounds);

//                SetClip(context);

            WpfSvgPaint fillPaint = new WpfSvgPaint(context, styleElm, "fill");

//            string fileValue = styleElm.GetAttribute("fill");

            Brush brush = fillPaint.GetBrush(geometry, _setBrushOpacity);
            if (brush == null)
            {
                WpfSvgPaint fallbackPaint = fillPaint.WpfFallback;
                if (fallbackPaint != null)
                {
                    brush = fallbackPaint.GetBrush(geometry, _setBrushOpacity);
                }
            }
            bool isFillTransmable = fillPaint.IsFillTransformable;

            WpfSvgPaint strokePaint = new WpfSvgPaint(context, styleElm, "stroke");
            Pen pen = strokePaint.GetPen(geometry, _setBrushOpacity);

            // By the SVG Specifications:
            // Keyword 'objectBoundingBox' should not be used when the geometry of the applicable 
            // element has no width or no height, such as the case of a horizontal or vertical line, 
            // even when the line has actual thickness when viewed due to having a non-zero stroke 
            // width since stroke width is ignored for bounding box calculations. When the geometry
            // of the applicable element has no width or height and 'objectBoundingBox' is specified, 
            // then the given effect (e.g., a gradient) will be ignored.
            if (pen != null && _isLineSegment && strokePaint.FillType == WpfFillType.Gradient)
            {
                WpfGradientFill gradientFill = (WpfGradientFill)strokePaint.PaintServer;
                if (gradientFill.IsUserSpace == false)
                {
                    bool invalidGrad = false;
                    if (string.Equals(_svgElement.LocalName, "line", StringComparison.Ordinal))
                    {
                        LineGeometry lineGeometry = geometry as LineGeometry;
                        if (lineGeometry != null)
                        {
                            invalidGrad = SvgObject.IsEqual(lineGeometry.EndPoint.X, lineGeometry.StartPoint.X)
                                || SvgObject.IsEqual(lineGeometry.EndPoint.Y, lineGeometry.StartPoint.Y);
                        }
                    }
                    else
                    {
                        invalidGrad = true;
                    }
                    if (invalidGrad)
                    {
                        // Brush is not likely inherited, we need to support fallback too
                        WpfSvgPaint fallbackPaint = strokePaint.WpfFallback;
                        if (fallbackPaint != null)
                        {
                            pen.Brush = fallbackPaint.GetBrush(geometry, _setBrushOpacity);
                        }
                        else
                        {
                            var scopePaint = strokePaint.GetScopeStroke();
                            if (scopePaint != null)
                            {
                                if (scopePaint != strokePaint)
                                {
                                    pen.Brush = scopePaint.GetBrush(geometry, _setBrushOpacity);
                                }
                                else
                                {
                                    pen.Brush = null;
                                }
                            }
                            else
                            {
                                pen.Brush = null;
                            }
                        }
                    }
                }
            }

            if (_paintContext != null)
            {
                _paintContext.Fill   = fillPaint;
                _paintContext.Stroke = strokePaint;
                _paintContext.Tag    = geometry;
            }

            if (brush != null || pen != null)
            {
                Transform transform = this.Transform;
                if (transform != null && !transform.Value.IsIdentity)
                {
                    geometry.Transform = transform;
                    if (brush != null && isFillTransmable)
                    {
                        Transform brushTransform = brush.Transform;
                        if (brushTransform == null || brushTransform == Transform.Identity)
                        {
                            brush.Transform = transform;
                        }
                        else
                        {
                            TransformGroup groupTransform = new TransformGroup();
                            groupTransform.Children.Add(brushTransform);
                            groupTransform.Children.Add(transform);
                            brush.Transform = groupTransform;
                        }
                    }
                    if (pen != null && pen.Brush != null)
                    {
                        Transform brushTransform = pen.Brush.Transform;
                        if (brushTransform == null || brushTransform == Transform.Identity)
                        {
                            pen.Brush.Transform = transform;
                        }
                        else
                        {
                            TransformGroup groupTransform = new TransformGroup();
                            groupTransform.Children.Add(brushTransform);
                            groupTransform.Children.Add(transform);
                            pen.Brush.Transform = groupTransform;
                        }
                    }
                }
                else
                {
                    transform = null; // render any identity transform useless...
                }

                drawing = new GeometryDrawing(brush, pen, geometry);

                if (!string.IsNullOrWhiteSpace(elementId) && !context.IsRegisteredId(elementId))
                {
                    SvgObject.SetName(drawing, elementId);

                    context.RegisterId(elementId);

                    if (context.IncludeRuntime)
                    {
                        SvgObject.SetId(drawing, elementId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(elementClass) && context.IncludeRuntime)
                {
                    SvgObject.SetClass(drawing, elementClass);
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
                            transformGroup.Children.Add(new ScaleTransform(drawingBounds.Width, drawingBounds.Height)); 
                            transformGroup.Children.Add(new TranslateTransform(drawingBounds.X, drawingBounds.Y));

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
                        DrawingBrush drawingBrush = (DrawingBrush)maskBrush;

                        SvgUnitType maskUnits = this.MaskUnits;
                        SvgUnitType maskContentUnits = this.MaskContentUnits;
                        if (maskUnits == SvgUnitType.ObjectBoundingBox)
                        {
                            Rect drawingBounds = geometryBounds;

                            if (transform != null)
                            {
                                drawingBounds = transform.TransformBounds(drawingBounds);
                            }
                            DrawingGroup maskGroup = drawingBrush.Drawing as DrawingGroup;
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

                            if (maskContentUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                TransformGroup transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                var scaleTransform = new ScaleTransform(drawingBounds.Width, drawingBounds.Height);
                                transformGroup.Children.Add(scaleTransform);
                                var translateTransform = new TranslateTransform(drawingBounds.X, drawingBounds.Y);
                                transformGroup.Children.Add(translateTransform);

                                Matrix scaleMatrix = new Matrix();
                                Matrix translateMatrix = new Matrix();

                                scaleMatrix.Scale(drawingBounds.Width, drawingBounds.Height);
                                translateMatrix.Translate(drawingBounds.X, drawingBounds.Y);

                                Matrix matrix = Matrix.Multiply(scaleMatrix, translateMatrix);
                                //maskBrush.Transform = transformGroup; 
                                maskBrush.Transform = new MatrixTransform(matrix); 
                            }
                            else
                            {
                                drawingBrush.Viewbox = drawingBounds;
                                drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;

                                drawingBrush.Stretch = Stretch.Uniform;

                                drawingBrush.Viewport = drawingBounds;
                                drawingBrush.ViewportUnits = BrushMappingMode.Absolute;
                            }
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

            // If this is not the child of a "marker", then try rendering a marker...
            if (!string.Equals(parentNode.LocalName, "marker"))
            {
                RenderMarkers(renderer, styleElm, context);
            }

            // Register this drawing with the Drawing-Document...
            if (drawing != null)
            {
                this.Rendered(drawing);
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            WpfDrawingContext context = renderer.Context;
            if (_drawGroup != null)
            {
                context.Pop();
            }

            base.AfterRender(renderer);
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
                {
                    throw new NotSupportedException();
                }
            }

            return alphaValue;
        }

        //==========================================================================
        private static Color ConvertColor(Color color)
        {
            if (color != Colors.Transparent)
            {
                return color;
            }

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
                {
                    throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void Initialize(SvgElement element)
        {
            base.Initialize(element);

            _drawGroup       = null;
            _isLineSegment   = false;
            _setBrushOpacity = false;
        }

        #endregion
    }
}
