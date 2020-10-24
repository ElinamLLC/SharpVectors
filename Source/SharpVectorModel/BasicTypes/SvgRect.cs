using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Rectangles are defined as consisting of a (x,y) coordinate pair identifying a minimum X value, 
    /// a minimum Y value, and a width and height, which are usually constrained to be non-negative. 
    /// </summary>
    public sealed class SvgRect : ISvgRect, IEquatable<SvgRect>
    {
        #region Static Fields

        public static readonly SvgRect Empty = new SvgRect(0, 0, 0, 0);

        #endregion

        #region Private Fields

        private double _x;
        private double _y;
        private double _width;
        private double _height;

        #endregion

        #region Constructors

        public SvgRect(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public SvgRect(string str)
        {
            string replacedStr = Regex.Replace(str, @"(\s|,)+", ",");
            string[] tokens    = replacedStr.Split(new char[] { ',' });
            if (tokens.Length == 2)
            {
                _x      = 0;
                _y      = 0;
                _width  = SvgNumber.ParseNumber(tokens[0]);
                _height = SvgNumber.ParseNumber(tokens[1]);
            }
            else if (tokens.Length == 4)
            {
                _x      = SvgNumber.ParseNumber(tokens[0]);
                _y      = SvgNumber.ParseNumber(tokens[1]);
                _width  = SvgNumber.ParseNumber(tokens[2]);
                _height = SvgNumber.ParseNumber(tokens[3]);
            }
            else
            {
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr,
                    "Invalid SvgRect value: " + str);
            }
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get {
                return (_width <= 0 || _height <= 0);
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// This tests whether two <see cref="SvgRect"/> structures have equal 
        /// location and size.
        /// </summary>
        /// <param name="left">
        /// The <see cref="SvgRect"/> structure that is to the left of the 
        /// equality operator. 
        /// </param>
        /// <param name="right">
        /// The <see cref="SvgRect"/> structure that is to the right of the 
        /// equality operator. 
        /// </param>
        /// <returns>
        /// This operator returns true if the two specified 
        /// <see cref="SvgRect"/> structures have equal 
        /// <see cref="SvgRect.X"/>, <see cref="SvgRect.Y"/>, 
        /// <see cref="SvgRect.Width"/>, and <see cref="SvgRect.Height"/>
        /// properties.
        /// </returns>
        public static bool operator ==(SvgRect left, SvgRect right)
        {
            if (left is null && right is null)
            {
                return true;
            }
            else if (left is null)
            {
                return false;
            }
            else if (right is null)
            {
                return false;
            }

            if ((left.X.Equals(right.X) && left.Y.Equals(right.Y)) &&
                left.Width.Equals(right.Width))
            {
                return left.Height.Equals(right.Height);
            }

            return false;
        }

        /// <summary>
        /// This tests whether two <see cref="SvgRect"/> structures differ in 
        /// location or size.</summary>
        /// <param name="left">
        /// The <see cref="SvgRect"/> structure that is to the left of the 
        /// inequality operator. 
        /// </param>
        /// <param name="right">
        /// The <see cref="SvgRect"/> structure that is to the right of the 
        /// inequality operator. 
        /// </param>
        /// <returns>
        /// This operator returns true if any of the <see cref="SvgRect.X"/>, 
        /// <see cref="SvgRect.Y"/>, <see cref="SvgRect.Width"/>, or 
        /// <see cref="SvgRect.Height"/> properties of the two 
        /// <see cref="SvgRect"/> structures are unequal; otherwise false.
        /// </returns>
        public static bool operator !=(SvgRect left, SvgRect right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This tests whether an object is a <see cref="SvgRect"/> with the 
        /// same location and size of this <see cref="SvgRect"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to test. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified object is a 
        /// <see cref="SvgRect"/> and its <see cref="X"/>, <see cref="Y"/>, 
        /// <see cref="Width"/>, and <see cref="Height"/> properties are equal 
        /// to the corresponding properties of this <see cref="SvgRect"/>; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SvgRect)
            {
                return this.Equals((SvgRect)obj);
            }

            return false;
        }

        /// <summary>
        /// This tests whether the specified <see cref="SvgRect"/> is with 
        /// the same location and size of this <see cref="SvgRect"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="SvgRect"/> to test. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if specified <see cref="SvgRect"/> 
        /// has its <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/>, and 
        /// <see cref="Height"/> properties are equal to the corresponding 
        /// properties of this <see cref="SvgRect"/>; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(SvgRect other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.X.Equals(_x) && other.Y.Equals(_y) && other.Width.Equals(_width))
            {
                return other.Height.Equals(_height);
            }
            return false;
        }

        /// <summary>
        /// This tests whether the specified <see cref="SvgRect"/> is with 
        /// the same location and size of this <see cref="SvgRect"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="SvgRect"/> to test. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if specified <see cref="SvgRect"/> 
        /// has its <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/>, and 
        /// <see cref="Height"/> properties are equal to the corresponding 
        /// properties of this <see cref="SvgRect"/>; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(ISvgRect other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.X.Equals(_x) && other.Y.Equals(_y) && other.Width.Equals(_width))
            {
                return other.Height.Equals(_height);
            }
            return false;
        }

        /// <overloads>
        /// This determines if the specified point or rectangle is contained 
        /// within this <see cref="SvgRect"/> structure.
        /// </overloads>
        /// <summary>
        /// This determines if the specified point is contained within this 
        /// <see cref="SvgRect"/> structure.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test. </param>
        /// <param name="y">The y-coordinate of the point to test. </param>
        /// <returns>
        /// This method returns true if the point defined by x and y is 
        /// contained within this <see cref="SvgRect"/> structure; otherwise 
        /// false.
        /// </returns>
        public bool Contains(double x, double y)
        {
            if (((_x <= x) && (x < (_x + _width))) && (_y <= y))
            {
                return (y < (_y + _height));
            }

            return false;
        }

        /// <summary>
        /// This determines if the specified point is contained within this 
        /// <see cref="SvgRect"/> structure.
        /// </summary>
        /// <param name="pt">The <see cref="SvgPoint"/> to test. </param>
        /// <returns>
        /// This method returns true if the point represented by the pt 
        /// parameter is contained within this <see cref="SvgRect"/> 
        /// structure; otherwise false.
        /// </returns>
        public bool Contains(SvgPoint pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        /// <summary>
        /// This determines if the rectangular region represented by rect is 
        /// entirely contained within this <see cref="SvgRect"/> structure.
        /// </summary>
        /// <param name="rect">The <see cref="SvgRect"/> to test. </param>
        /// <returns>
        /// This method returns true if the rectangular region represented by 
        /// rect is entirely contained within the rectangular region represented 
        /// by this <see cref="SvgRect"/>; otherwise false.
        /// </returns>
        public bool Contains(SvgRect rect)
        {
            if (((_x <= rect.X) && ((rect.X + rect.Width) <=
                (_x + _width))) && (_y <= rect.Y))
            {
                return ((rect.Y + rect.Height) <= (_y + _height));
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code for this <see cref="SvgRect"/> structure. 
        /// For information about the use of hash codes, see Object.GetHashCode.
        /// </summary>
        /// <returns>The hash code for this <see cref="SvgRect"/>.</returns>
        public override int GetHashCode()
        {
            double[] values = this.GetHashValues();
            return (values[0].GetHashCode() ^ values[1].GetHashCode() ^
                values[2].GetHashCode() ^ values[3].GetHashCode());
        }

        /// <overloads>
        /// Inflates this <see cref="SvgRect"/> structure by the specified 
        /// amount.
        /// </overloads>
        /// <summary>
        /// Inflates this <see cref="SvgRect"/> structure by the specified 
        /// amount.
        /// </summary>
        /// <param name="x">The amount to inflate this <see cref="SvgRect"/> structure horizontally. </param>
        /// <param name="y">The amount to inflate this <see cref="SvgRect"/> structure vertically. </param>
        /// <returns>This method does not return a value.</returns>
        public void Inflate(double x, double y)
        {
            _x      -= x;
            _y      -= y;
            _width  += 2f * x;
            _height += 2f * y;
        }

        public void Intersect(SvgRect rect)
        {
            if (rect == null)
            {
                return;
            }

            SvgRect ef = Intersection(rect, this);
            _x      = ef.X;
            _y      = ef.Y;
            _width  = ef.Width;
            _height = ef.Height;
        }

        /// <summary>
        /// This replaces this <see cref="SvgRect"/> structure with the 
        /// intersection of itself and the specified <see cref="SvgRect"/> 
        /// structure.
        /// </summary>
        /// <param name="rect">The rectangle to intersect. </param>
        /// <returns>This method does not return a value.</returns>
        public void Intersection(SvgRect rect)
        {
            if (rect == null)
            {
                return;
            }

            SvgRect result = SvgRect.Intersection(rect, this);
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
        public bool Intersects(SvgRect rect)
        {
            if (((rect.X < (_x + _width)) && (_x < (rect.X + rect.Width))) &&
                (rect.Y < (_y + _height)))
            {
                return (_y < (rect.Y + rect.Height));
            }

            return false;
        }

        /// <summary>
        /// Adjusts the location of this rectangle by the specified amount.
        /// </summary>
        /// <param name="y">The amount to offset the location vertically. </param>
        /// <param name="x">The amount to offset the location horizontally. </param>
        /// <returns>This method does not return a value.</returns>
        public void Offset(double x, double y)
        {
            _x += x;
            _y += y;
        }

        public override string ToString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return ("{X=" + _x.ToString(culture) + ",Y=" + _y.ToString(culture)
                + ",Width=" + _width.ToString(culture) + ",Height=" + _height.ToString(culture) + "}");
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates and returns an inflated copy of the specified 
        /// <see cref="SvgRect"/> structure. The copy is inflated by the 
        /// specified amount. The original rectangle remains unmodified.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="SvgRect"/> to be copied. This rectangle is not 
        /// modified. 
        /// </param>
        /// <param name="x">
        /// The amount to inflate the copy of the rectangle horizontally. 
        /// </param>
        /// <param name="y">
        /// The amount to inflate the copy of the rectangle vertically. 
        /// </param>
        /// <returns>The inflated <see cref="SvgRect"/>.</returns>
        public static SvgRect Inflate(SvgRect rect, float x, float y)
        {
            SvgRect result = rect;
            result.Inflate(x, y);

            return result;
        }

        /// <summary>
        /// Returns a <see cref="SvgRect"/> structure that represents the 
        /// intersection of two rectangles. If there is no intersection, and 
        /// empty <see cref="SvgRect"/> is returned.
        /// </summary>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <returns>
        /// A third <see cref="SvgRect"/> structure the size of which 
        /// represents the overlapped area of the two specified rectangles.
        /// </returns>
        public static SvgRect Intersection(SvgRect a, SvgRect b)
        {
            double dLeft   = Math.Max(a.X, b.X);
            double dRight  = Math.Min(a.X + a.Width, b.X + b.Width);
            double dTop    = Math.Max(a.Y, b.Y);
            double dBottom = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if ((dRight >= dLeft) && (dBottom >= dTop))
            {
                return new SvgRect(dLeft, dTop, dRight - dLeft,
                    dBottom - dTop);
            }

            return SvgRect.Empty;
        }

        /// <summary>
        /// Creates the smallest possible third rectangle that can contain both 
        /// of two rectangles that form a union.
        /// </summary>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <returns>
        /// A third <see cref="SvgRect"/> structure that contains both of 
        /// the two rectangles that form the union.
        /// </returns>
        public static SvgRect Union(SvgRect a, SvgRect b)
        {
            double dLeft   = Math.Min(a.X, b.X);
            double dRight  = Math.Max(a.X + a.Width, b.X + b.Width);
            double dTop    = Math.Min(a.Y, b.Y);
            double dBottom = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new SvgRect(dLeft, dTop, dRight - dLeft, dBottom - dTop);
        }

        #endregion

        #region ISvgRect Interface

        public double X
        {
            get {
                return _x;
            }
            set {
                _x = value;
            }
        }

        public double Y
        {
            get {
                return _y;
            }
            set {
                _y = value;
            }
        }

        public double Width
        {
            get {
                return _width;
            }
            set {
                _width = value;
            }
        }

        public double Height
        {
            get {
                return _height;
            }
            set {
                _height = value;
            }
        }

        #endregion

        #region Private Methods

        private double[] GetHashValues()
        {
            return new double[] { _x, _y, _width, _height };
        }

        #endregion
    }
}
