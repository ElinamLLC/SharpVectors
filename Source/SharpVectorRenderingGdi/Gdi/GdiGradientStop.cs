using System;
using System.Drawing;

namespace SharpVectors.Renderers.Gdi
{
    /// <summary>
    /// Describes the location and color of a transition point in a gradient.
    /// </summary>
    public sealed class GdiGradientStop : ICloneable
    {
        private Color _color;
        private float _offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="GdiGradientStop"/> class.
        /// </summary>
        public GdiGradientStop()
        {
            _color  = Color.Transparent;
            _offset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GdiGradientStop"/> class with
        /// the specified color and offset.
        /// </summary>
        /// <param name="color">The color value of the gradient stop.</param>
        /// <param name="offset">The location in the gradient where the gradient stop is placed.</param>
        public GdiGradientStop(Color color, float offset)
        {
            _color  = color;
            _offset = offset;
        }

        /// <summary>
        /// Gets or sets the color of the gradient stop.
        /// </summary>
        /// <value>The color of the gradient stop. The default value is <see cref="Color.Transparent"/>.</value>
        public Color Color
        {
            get {
                return _color;
            }
            set {
                _color = value;
            }
        }

        /// <summary>
        /// Gets or set the location of the gradient stop within the gradient vector.
        /// </summary>
        /// <value>
        /// The relative location of this gradient stop along the gradient vector. 
        /// The default value is <c>0.0</c>.
        /// </value>
        public float Offset
        {
            get {
                return _offset;
            }
            set {
                _offset = value;
            }
        }

        /// <summary>
        /// Creates a modifiable clone of this <see cref="]GdiGradientStop"/>, making
        /// deep copies of this object's values.
        /// </summary>
        /// <returns>A modifiable clone of the current object.</returns>
        public GdiGradientStop Clone()
        {
            return new GdiGradientStop(_color, _offset);
        }
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Creates a string representation of this object based on the current culture.
        /// </summary>
        /// <returns>
        /// A string representation of this object that contains its <see cref="GdiGradientStop.Color"/>
        /// and <see cref="GdiGradientStop.Offset"/> values.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", _color, _offset);
        }
    }

}
