using System;
using System.Globalization;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Stores an ordered pair of floating-point numbers, typically the width 
    /// and height of a rectangle.
    /// </summary>
    [Serializable]
    public struct SvgSizeF : IEquatable<SvgSizeF>
    {
        #region Private Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSizeF"/> class.
        /// </summary>
        public static readonly SvgSizeF Empty = new SvgSizeF();

        private float _width;
        private float _height;

        #endregion

        #region Constructors and Destructor

        /// <summary>Initializes a new instance of the <see cref="SvgSizeF"/> class from the specified dimensions.</summary>
        /// <param name="width">The width component of the new <see cref="SvgSizeF"/>. </param>
        /// <param name="height">The height component of the new <see cref="SvgSizeF"/>. </param>
        public SvgSizeF(float width, float height)
        {
            _width  = width;
            _height = height;
        }

        /// <summary>Initializes a new instance of the <see cref="SvgSizeF"/> class from the specified existing <see cref="SvgSizeF"/>.</summary>
        /// <param name="size">The <see cref="SvgSizeF"/> from which to create the new <see cref="SvgSizeF"/>. </param>
        public SvgSizeF(SvgSizeF size)
        {
            _width  = size._width;
            _height = size._height;
        }

        /// <summary>Initializes a new instance of the <see cref="SvgSizeF"/> class from the specified <see cref="SvgPointF"/>.</summary>
        /// <param name="pt">The <see cref="SvgPointF"/> from which to initialize this <see cref="SvgSizeF"/>. </param>
        public SvgSizeF(SvgPointF pt)
        {
            _width  = pt.X;
            _height = pt.Y;
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Adds the width and height of one <see cref="SvgSizeF"/> structure 
        /// to the width and height of another <see cref="SvgSizeF"/> structure.
        /// </summary>
        /// <param name="sz1">The first <see cref="SvgSizeF"/> to add.</param>
        /// <param name="sz2">The second <see cref="SvgSizeF"/> to add.</param>
        /// <returns>
        /// A <see cref="SvgSizeF"/> structure that is the result of the 
        /// addition operation.
        /// </returns>
        public static SvgSizeF operator +(SvgSizeF sz1, SvgSizeF sz2)
        {
            return SvgSizeF.Add(sz1, sz2);
        }

        /// <summary>
        /// Subtracts the width and height of one <see cref="SvgSizeF"/> 
        /// structure from the width and height of another 
        /// <see cref="SvgSizeF"/> structure.
        /// </summary>
        /// <param name="sz1">
        /// The <see cref="SvgSizeF"/> on the left side of the subtraction 
        /// operator. 
        /// </param>
        /// <param name="sz2">
        /// The <see cref="SvgSizeF"/> on the right side of the subtraction 
        /// operator. 
        /// </param>
        /// <returns>
        /// A <see cref="SvgSizeF"/> that is the result of the subtraction 
        /// operation.
        /// </returns>
        public static SvgSizeF operator -(SvgSizeF sz1, SvgSizeF sz2)
        {
            return SvgSizeF.Subtract(sz1, sz2);
        }

        /// <summary>
        /// Tests whether two <see cref="SvgSizeF"/> structures are equal.
        /// </summary>
        /// <param name="sz1">
        /// The <see cref="SvgSizeF"/> structure on the left side of the 
        /// equality operator. 
        /// </param>
        /// <param name="sz2">
        /// The <see cref="SvgSizeF"/> structure on the right of the equality 
        /// operator. 
        /// </param>
        /// <returns>
        /// This operator returns <see langword="true"/> if <paramref name="sz1"/> 
        /// and <paramref name="sz2"/> have equal width and height; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool operator ==(SvgSizeF sz1, SvgSizeF sz2)
        {
            if (sz1.Width == sz2.Width)
            {
                return (sz1.Height == sz2.Height);
            }

            return false;
        }

        /// <summary>
        /// Tests whether two <see cref="SvgSizeF"/> structures are different.
        /// </summary>
        /// <param name="sz1">
        /// The <see cref="SvgSizeF"/> structure on the left of the inequality 
        /// operator. 
        /// </param>
        /// <param name="sz2">
        /// The <see cref="SvgSizeF"/> structure on the right of the inequality 
        /// operator. 
        /// </param>
        /// <returns>
        /// This operator returns <see langword="true"/> if 
        /// <paramref name="sz1"/> and <paramref name="sz2"/> differ either 
        /// in width or height; <see langword="false"/> if <paramref name="sz1"/> 
        /// and <paramref name="sz2"/> are equal.
        /// </returns>
        public static bool operator !=(SvgSizeF sz1, SvgSizeF sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>
        /// This converts the specified <see cref="SvgSizeF"/> to a 
        /// <see cref="SvgPointF"/>.
        /// </summary>
        /// <param name="size">
        /// The <see cref="SvgSizeF"/> structure to be converted.
        /// </param>
        /// <returns>
        /// The <see cref="SvgPointF"/> structure specifying the result of the
        /// conversion.
        /// </returns>
        public static explicit operator SvgPointF(SvgSizeF size)
        {
            return new SvgPointF(size.Width, size.Height);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="SvgSizeF"/> has zero 
        /// width and height.
        /// </summary>
        /// <value>
        /// This property returns <see langword="true"/> when this 
        /// <see cref="SvgSizeF"/> has both a width and height of zero; 
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (_width == 0.0)
                {
                    return (_height == 0.0);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal component of this <see cref="SvgSizeF"/>.
        /// </summary>
        /// <value>
        /// The horizontal component of this <see cref="SvgSizeF"/>.
        /// </value>
        public float Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical component of this <see cref="SvgSizeF"/>.
        /// </summary>
        /// <value>
        /// The vertical component of this <see cref="SvgSizeF"/>.
        /// </value>
        public float Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This tests whether the specified object is a <see cref="SvgSizeF"/> 
        /// with the same dimensions as this <see cref="SvgSizeF"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to test. </param>
        /// <returns>
        /// This returns <see langword="true"/> if specified object is a 
        /// <see cref="SvgSizeF"/> and has the same width and height as this 
        /// <see cref="SvgSizeF"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SvgSizeF)
            {
                return Equals((SvgSizeF)obj);
            }

            return false;
        }

        /// <summary>
        /// This to see whether the specified <see cref="SvgSizeF"/> is with the 
        /// same dimensions as this <see cref="SvgSizeF"/>.
        /// </summary>
        /// <param name="other">The <see cref="SvgSizeF"/> to test. </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified <see cref="SvgSizeF"/> and has the same width and height as this <see cref="SvgSizeF"/>; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(SvgSizeF other)
        {
            return ((other.Width.Equals(_width)) && 
                (other.Height.Equals(_height)));
        }

        /// <summary>
        /// This returns a hash code for this <see cref="SvgSizeF"/> structure.
        /// </summary>
        /// <returns>
        /// An integer value that specifies a hash value for this 
        /// <see cref="SvgSizeF"/> structure.
        /// </returns>
        public override int GetHashCode()
        {
            return (_width.GetHashCode() ^ _height.GetHashCode());
        }

        /// <summary>
        /// This creates a human-readable string that represents this 
        /// <see cref="SvgSizeF"/>.
        /// </summary>
        /// <returns>A string that represents this <see cref="SvgSizeF"/>.</returns>
        public override string ToString()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            return ("{Width=" + _width.ToString(culture)
                + ", Height=" + _height.ToString(culture) + "}");
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Adds the width and height of one <see cref="SvgSizeF"/> structure to 
        /// the width and height of another <see cref="SvgSizeF"/> structure.
        /// </summary>
        /// <param name="sz1">The first <see cref="SvgSizeF"/> to add.</param>
        /// <param name="sz2">The second <see cref="SvgSizeF"/> to add.</param>
        /// <returns>
        /// A <see cref="SvgSizeF"/> structure that is the result of the addition 
        /// operation.
        /// </returns>
        public static SvgSizeF Add(SvgSizeF sz1, SvgSizeF sz2)
        {
            return new SvgSizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        /// <summary>
        /// Subtracts the width and height of one <see cref="SvgSizeF"/> 
        /// structure from the width and height of another 
        /// <see cref="SvgSizeF"/> structure.
        /// </summary>
        /// <param name="sz1">
        /// The <see cref="SvgSizeF"/> structure on the left side of the 
        /// subtraction operator. 
        /// </param>
        /// <param name="sz2">
        /// The <see cref="SvgSizeF"/> structure on the right side of the 
        /// subtraction operator. 
        /// </param>
        /// <returns>
        /// The <see cref="SvgSizeF"/> that is a result of the subtraction 
        /// operation.
        /// </returns>
        public static SvgSizeF Subtract(SvgSizeF sz1, SvgSizeF sz2)
        {
            return new SvgSizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        #endregion
    }
}
