using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;

namespace SharpVectors.Converters
{
    /// <summary>
    /// This is an extension of the WPF <see cref="Image"/> control to provide SVG-based image sources.
    /// </summary>
    public sealed class SvgIcon : SvgBitmap
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", 
            typeof(Brush), typeof(SvgIcon), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", 
            typeof(Brush), typeof(SvgIcon), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgIcon"/> class.
        /// </summary>
        public SvgIcon()
        {
        }

        public Brush Fill
        {
            get {
                return (Brush)GetValue(FillProperty);
            }
            set {
                SetValue(FillProperty, value);
            }
        }

        public Brush Stroke
        {
            get {
                return (Brush)GetValue(StrokeProperty);
            }
            set {
                SetValue(StrokeProperty, value);
            }
        }

        #region Protected Methods

        protected override void OnLoadDrawing(DrawingGroup drawing)
        {
            base.OnLoadDrawing(drawing);

            if (drawing != null)
            {
                this.SetBindings(drawing);
            }
        }

        protected override void OnUnloadDiagram()
        {
            if (this.Source != null && this.Source is DrawingImage drawingImage)
            {
                if (drawingImage.Drawing is DrawingGroup drawing)
                {
                    this.ClearBindings(drawing);
                }
            }

            base.OnUnloadDiagram();
        }

        #endregion

        #region Private Methods

        private void SetBindings(DrawingGroup group)
        {
            DrawingCollection drawings = group.Children;
            for (int i = 0; i < drawings.Count; i++)
            {
                Drawing drawing = drawings[i];
                if (drawing is DrawingGroup childGroup)
                {
                    SetBindings(childGroup);
                }
                else if (drawing is GeometryDrawing geometryDrawing)
                {
                    // svg fill color - translated to a geometry.Brush
                    if (geometryDrawing.Brush != null)
                    {
                        var brush = new Binding(nameof(this.Fill))
                        {
                            Source = this,
                            Mode = BindingMode.OneWay
                        };
                        BindingOperations.SetBinding(geometryDrawing, GeometryDrawing.BrushProperty, brush);
                    }

                    // svg stroke color - translated to a geometry.Pen.Brush
                    if (geometryDrawing.Pen != null)
                    {
                        var stroke = new Binding(nameof(this.Stroke))
                        {
                            Source = this,
                            Mode = BindingMode.OneWay
                        };
                        BindingOperations.SetBinding(geometryDrawing.Pen, Pen.BrushProperty, stroke);
                    }
                }
            }
        }

        private void ClearBindings(DrawingGroup group)
        {
            DrawingCollection drawings = group.Children;
            for (int i = 0; i < drawings.Count; i++)
            {
                Drawing drawing = drawings[i];
                if (drawing is DrawingGroup childGroup)
                {
                    ClearBindings(childGroup);
                }
                else if (drawing is GeometryDrawing geometryDrawing)
                {
                    // svg fill color - translated to a geometry.Brush
                    if (geometryDrawing.Brush != null)
                    {
                        BindingOperations.ClearAllBindings(geometryDrawing);
                    }

                    // svg stroke color - translated to a geometry.Pen.Brush
                    if (geometryDrawing.Pen != null)
                    {
                        BindingOperations.ClearAllBindings(geometryDrawing.Pen);
                    }
                }
            }
        }

        #endregion
    }
}
