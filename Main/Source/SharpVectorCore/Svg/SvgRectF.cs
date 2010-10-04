using System;
using System.Globalization;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This stores a set of four float precision numbers that represent the 
    /// location and size of a rectangle.
    /// </summary>
    [Serializable]
    public struct SvgRectF : IEquatable<SvgRectF>
    {
        #region Private Fields

        /// <summary>
        /// Represents an instance of the <see cref="SvgRectF"/> structure 
        /// with its members uninitialized.
        /// </summary>
        public static readonly SvgRectF Empty = new SvgRectF();
                    
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgRectF"/> 
        /// structure with the specified location and size.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the upper-left corner of the rectangle. 
        /// </param>
        /// <param name="y">
        /// The y-coordinate of the upper-left corner of the rectangle. 
        /// </param>
        /// <param name="width">The width of the rectangle. </param>
        /// <param name="height">The height of the rectangle. </param>
        public SvgRectF(float x, float y, float width, float height)
        {
            _x      = x;
            _y      = y;
            _width  = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgRectF"/> structure 
        /// with the specified location and size.
        /// </summary>
        /// <param name="size">
        /// A <see cref="SvgSizeF"/> that represents the width and height of the 
        /// rectangular region.
        /// </param>
        /// <param name="location">
        /// A <see cref="SvgPointF"/> that represents the upper-left corner 
        /// of the rectangular region. 
        /// </param>
        public SvgRectF(SvgPointF location, SvgSizeF size)
        {
            _x      = location.X;
            _y      = location.Y;
            _width  = size.Width;
            _height = size.Height;
        }

        /// <summary>
        /// Creates a <see cref="SvgRectF"/> structure with upper-left corner 
        /// and lower-right corner at the specified locations.
        /// </summary>
        /// <param name="left">
        /// The x-coordinate of the upper-left corner of the rectangular region. 
        /// </param>
        /// <param name="top">
        /// The y-coordinate of the upper-left corner of the rectangular region.
        /// </param>
        /// <param name="right">
        /// The x-coordinate of the lower-right corner of the rectangular region. 
        /// </param>
        /// <param name="bottom">
        /// The y-coordinate of the lower-right corner of the rectangular region.
        /// </param>
        /// <returns>
        /// The new <see cref="SvgRectF"/> that this method creates.
        /// </returns>
        public static SvgRectF Create(float left, float top, 
            float right, float bottom)
        {
            return new SvgRectF(left, top, right - left, bottom - top);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <value>
        /// A <see cref="SvgPointF"/> that represents the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure.
        /// </value>
        public SvgPointF Location
        {
            get
            {
                return new SvgPointF(_x, _y);
            }
            set
            {
                _x = value.X;
                _y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the size of this <see cref="SvgRectF"/>.
        /// </summary>
        /// <value>
        /// A <see cref="SvgSizeF"/> that represents the width and height of this 
        /// <see cref="SvgRectF"/> structure.
        /// </value>
        public SvgSizeF Size
        {
            get
            {
                return new SvgSizeF(_width, _height);
            }
            set
            {
                _width  = value.Width;
                _height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <value>
        /// The x-coordinate of the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure.
        /// </value>
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <value>
        /// The y-coordinate of the upper-left corner of this 
        /// <see cref="SvgRectF"/> structure. 
        /// </value>
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of this <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <value>
        /// The width of this <see cref="SvgRectF"/> structure.
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
        /// Gets or sets the height of this <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <value>
        /// The height of this <see cref="SvgRectF"/> structure.
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

        /// <summary>
        /// Gets the x-coordinate of the left edge of this <see cref="SvgRectF"/> 
        /// structure.
        /// </summary>
        /// <value>
        /// The x-coordinate of the left edge of this <see cref="SvgRectF"/> 
        /// structure.
        /// </value>
        public float Left
        {
            get
            {
                return _x;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this <see cref="SvgRectF"/> 
        /// structure.
        /// </summary>
        /// <value>
        /// The y-coordinate of the top edge of this <see cref="SvgRectF"/> 
        /// structure.
        /// </value>
        public float Top
        {
            get
            {
                return _y;
            }
        }

        /// <summary>
        /// Gets the x-coordinate that is the sum of <see cref="SvgRectF.X"/> 
        /// and <see cref="SvgRectF.Width"/> of this <see cref="SvgRectF"/> 
        /// structure.
        /// </summary>
        /// <value>
        /// The x-coordinate that is the sum of <see cref="SvgRectF.X"/> and 
        /// <see cref="SvgRectF.Width"/> of this <see cref="SvgRectF"/> 
        /// structure.
        /// </value>
        public float Right
        {
            get
            {
                return (_x + _width);
            }
        }

        /// <summary>
        /// Gets the y-coordinate that is the sum of <see cref="SvgRectF.Y"/> 
        /// and <see cref="SvgRectF.Height"/> of this <see cref="SvgRectF"/> 
        /// structure.
        /// </summary>
        /// <value>
        /// The y-coordinate that is the sum of <see cref="SvgRectF.Y"/> and 
        /// <see cref="SvgRectF.Height"/> of this <see cref="SvgRectF"/> 
        /// structure.
        /// </value>
        public float Bottom
        {
            get
            {
                return (_y + _height);
            }
        }

        /// <summary>
        /// Tests whether the <see cref="SvgRectF.Width"/> or 
        /// <see cref="SvgRectF.Height"/> property of this 
        /// <see cref="SvgRectF"/> has a value of zero.
        /// </summary>
        /// <value>
        /// This property returns true if the <see cref="SvgRectF.Width"/> or 
        /// <see cref="SvgRectF.Height"/> property of this 
        /// <see cref="SvgRectF"/> has a value of zero; otherwise, false.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (_width > 0f)
                {
                    return (_height <= 0f);
                }

                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This tests whether an object is a <see cref="SvgRectF"/> with the 
        /// same location and size of this <see cref="SvgRectF"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to test. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified object is a 
        /// <see cref="SvgRectF"/> and its <see cref="X"/>, <see cref="Y"/>, 
        /// <see cref="Width"/>, and <see cref="Height"/> properties are equal 
        /// to the corresponding properties of this <see cref="SvgRectF"/>; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SvgRectF)
            {
                return Equals((SvgRectF)obj);
            }

            return false;
        }

        /// <summary>
        /// This tests whether the specified <see cref="SvgRectF"/> is with 
        /// the same location and size of this <see cref="SvgRectF"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="SvgRectF"/> to test. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if specified <see cref="SvgRectF"/> 
        /// has its <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/>, and 
        /// <see cref="Height"/> properties are equal to the corresponding 
        /// properties of this <see cref="SvgRectF"/>; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(SvgRectF other)
        {
            if (((other.X == _x) && (other.Y == _y)) && (other.Width == _width))
            {
                return (other.Height == _height);
            }

            return false;
        }

        /// <overloads>
        /// This determines if the specified point or rectangle is contained 
        /// within this <see cref="SvgRectF"/> structure.
        /// </overloads>
        /// <summary>
        /// This determines if the specified point is contained within this 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test. </param>
        /// <param name="y">The y-coordinate of the point to test. </param>
        /// <returns>
        /// This method returns true if the point defined by x and y is 
        /// contained within this <see cref="SvgRectF"/> structure; otherwise 
        /// false.
        /// </returns>
        public bool Contains(float x, float y)
        {
            if (((_x <= x) && (x < (_x + _width))) && (_y <= y))
            {
                return (y < (_y + _height));
            }

            return false;
        }

        /// <summary>
        /// This determines if the specified point is contained within this 
        /// <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="pt">The <see cref="SvgPointF"/> to test. </param>
        /// <returns>
        /// This method returns true if the point represented by the pt 
        /// parameter is contained within this <see cref="SvgRectF"/> 
        /// structure; otherwise false.
        /// </returns>
        public bool Contains(SvgPointF pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        /// <summary>
        /// This determines if the rectangular region represented by rect is 
        /// entirely contained within this <see cref="SvgRectF"/> structure.
        /// </summary>
        /// <param name="rect">The <see cref="SvgRectF"/> to test. </param>
        /// <returns>
        /// This method returns true if the rectangular region represented by 
        /// rect is entirely contained within the rectangular region represented 
        /// by this <see cref="SvgRectF"/>; otherwise false.
        /// </returns>
        public bool Contains(SvgRectF rect)
        {
            if (((_x <= rect.X) && ((rect.X + rect.Width) <= 
                (_x + _width))) && (_y <= rect.Y))
            {
                return ((rect.Y + rect.Height) <= (_y + _height));
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code for this <see cref="SvgRectF"/> structure. 
        /// For information about the use of hash codes, see Object.GetHashCode.
        /// </summary>
        /// <returns>The hash code for this <see cref="SvgRectF"/>.</returns>
        public override int GetHashCode()
        {
            return (_x.GetHashCode() ^ _y.GetHashCode() ^
                _width.GetHashCode() ^ _height.GetHashCode());
        }

        /// <overloads>
        /// Inflates this <see cref="SvgRectF"/> structure by the specified 
        /// amount.
        /// </overloads>
        /// <summary>
        /// Inflates this <see cref="SvgRectF"/> structure by the specified 
        /// amount.
        /// </summary>
        /// <param name="x">The amount to inflate this <see cref="SvgRectF"/> structure horizontally. </param>
        /// <param name="y">The amount to inflate this <see cref="SvgRectF"/> structure vertically. </param>
        /// <returns>This method does not return a value.</returns>
        public void Inflate(float x, float y)
        {
            _x      -= x;
            _y      -= y;
            _width  += 2f * x;
            _height += 2f * y;
        }

        /// <summary>
        /// Inflates this <see cref="SvgRectF"/> by the specified amount.
        /// </summary>
        /// <param name="size">The amount to inflate this rectangle. </param>
        /// <returns>This method does not return a value.</returns>
        public void Inflate(SvgSizeF size)
        {
            this.Inflate(size.Width, size.Height);
        }

        public void Intersect(SvgRectF rect)
        {
            SvgRectF ef = Intersection(rect, this);
            _x      = ef.X;
            _y      = ef.Y;
            _width  = ef.Width;
            _height = ef.Height;
        }

        /// <summary>
        /// This replaces this <see cref="SvgRectF"/> structure with the 
        /// intersection of itself and the specified <see cref="SvgRectF"/> 
        /// structure.
        /// </summary>
        /// <param name="rect">The rectangle to intersect. </param>
        /// <returns>This method does not return a value.</returns>
        public void Intersection(SvgRectF rect)
        {
            SvgRectF result = SvgRectF.Intersection(rect, this);
            _x      = result.X;
            _y      = result.Y;
            _width  = result.Width;
            _height = result.Height;
        }

        /// <summary>
        /// This determines if this rectangle intersects with rect.
        /// </summary>
        /// <param name="rect">The rectangle to test. </param>
        /// <returns>
        /// This method returns <see langword="true"/> if there is any 
        /// intersection; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Intersects(SvgRectF rect)
        {
            if (((rect.X < (_x + _width)) && (_x < (rect.X + rect.Width))) && 
                (rect.Y < (_y + _height)))
            {
                return (_y < (rect.Y + rect.Height));
            }

            return false;
        }

        /// <overloads>
        /// Adjusts the location of this rectangle by the specified amount.
        /// </overloads>
        /// <summary>
        /// Adjusts the location of this rectangle by the specified amount.
        /// </summary>
        /// <param name="pos">The amount to offset the location. </param>
        /// <returns>This method does not return a value.</returns>
        public void Offset(SvgPointF pos)
        {
            this.Offset(pos.X, pos.Y);
        }

        /// <summary>
        /// Adjusts the location of this rectangle by the specified amount.
        /// </summary>
        /// <param name="y">The amount to offset the location vertically. </param>
        /// <param name="x">The amount to offset the location horizontally. </param>
        /// <returns>This method does not return a value.</returns>
        public void Offset(float x, float y)
        {
            _x += x;
            _y += y;
        }

        /// <summary>
        /// Converts the Location and <see cref="Size"/> of this 
        /// <see cref="SvgRectF"/> to a human-readable string.
        /// </summary>
        /// <returns>
        /// A string that contains the position, width, and height of this 
        /// <see cref="SvgRectF"/> structure; for example, 
        /// "{X=20, Y=20, Width=100, Height=50}".
        /// </returns>
        public override string ToString()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            return ("{X=" + _x.ToString(culture) + ",Y=" + _y.ToString(culture)
                + ",Width=" + _width.ToString(culture) 
                + ",Height=" + _height.ToString(culture) + "}");
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// This tests whether two <see cref="SvgRectF"/> structures have equal 
        /// location and size.
        /// </summary>
        /// <param name="left">
        /// The <see cref="SvgRectF"/> structure that is to the left of the 
        /// equality operator. 
        /// </param>
        /// <param name="right">
        /// The <see cref="SvgRectF"/> structure that is to the right of the 
        /// equality operator. 
        /// </param>
        /// <returns>
        /// This operator returns true if the two specified 
        /// <see cref="SvgRectF"/> structures have equal 
        /// <see cref="SvgRectF.X"/>, <see cref="SvgRectF.Y"/>, 
        /// <see cref="SvgRectF.Width"/>, and <see cref="SvgRectF.Height"/>
        /// properties.
        /// </returns>
        public static bool operator ==(SvgRectF left, SvgRectF right)
        {
            if (((left.X == right.X) && (left.Y == right.Y)) && 
                (left.Width == right.Width))
            {
                return (left.Height == right.Height);
            }

            return false;
        }

        /// <summary>
        /// This tests whether two <see cref="SvgRectF"/> structures differ in 
        /// location or size.</summary>
        /// <param name="left">
        /// The <see cref="SvgRectF"/> structure that is to the left of the 
        /// inequality operator. 
        /// </param>
        /// <param name="right">
        /// The <see cref="SvgRectF"/> structure that is to the right of the 
        /// inequality operator. 
        /// </param>
        /// <returns>
        /// This operator returns true if any of the <see cref="SvgRectF.X"/>, 
        /// <see cref="SvgRectF.Y"/>, <see cref="SvgRectF.Width"/>, or 
        /// <see cref="SvgRectF.Height"/> properties of the two 
        /// <see cref="SvgRectF"/> structures are unequal; otherwise false.
        /// </returns>
        public static bool operator !=(SvgRectF left, SvgRectF right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates and returns an inflated copy of the specified 
        /// <see cref="SvgRectF"/> structure. The copy is inflated by the 
        /// specified amount. The original rectangle remains unmodified.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="SvgRectF"/> to be copied. This rectangle is not 
        /// modified. 
        /// </param>
        /// <param name="x">
        /// The amount to inflate the copy of the rectangle horizontally. 
        /// </param>
        /// <param name="y">
        /// The amount to inflate the copy of the rectangle vertically. 
        /// </param>
        /// <returns>The inflated <see cref="SvgRectF"/>.</returns>
        public static SvgRectF Inflate(SvgRectF rect, float x, float y)
        {
            SvgRectF result = rect;
            result.Inflate(x, y);

            return result;
        }

        /// <summary>
        /// Returns a <see cref="SvgRectF"/> structure that represents the 
        /// intersection of two rectangles. If there is no intersection, and 
        /// empty <see cref="SvgRectF"/> is returned.
        /// </summary>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <returns>
        /// A third <see cref="SvgRectF"/> structure the size of which 
        /// represents the overlapped area of the two specified rectangles.
        /// </returns>
        public static SvgRectF Intersection(SvgRectF a, SvgRectF b)
        {
            float dLeft   = Math.Max(a.X, b.X);
            float dRight  = Math.Min(a.X + a.Width, b.X + b.Width);
            float dTop    = Math.Max(a.Y, b.Y);
            float dBottom = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if ((dRight >= dLeft) && (dBottom >= dTop))
            {
                return new SvgRectF(dLeft, dTop, dRight - dLeft,
                    dBottom - dTop);
            }

            return SvgRectF.Empty;
        }

        /// <summary>
        /// Creates the smallest possible third rectangle that can contain both 
        /// of two rectangles that form a union.
        /// </summary>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <returns>
        /// A third <see cref="SvgRectF"/> structure that contains both of 
        /// the two rectangles that form the union.
        /// </returns>
        public static SvgRectF Union(SvgRectF a, SvgRectF b)
        {
            float dLeft   = Math.Min(a.X, b.X);
            float dRight  = Math.Max(a.X + a.Width, b.X + b.Width);
            float dTop    = Math.Min(a.Y, b.Y);
            float dBottom = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new SvgRectF(dLeft, dTop, dRight - dLeft, dBottom - dTop);
        }

        #endregion
    }
}
