using System;
using System.Globalization;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This represents an ordered pair of float precision x- and y-coordinates 
    /// that defines a point in a two-dimensional plane.
    /// </summary>
    [Serializable]
    public struct SvgPointF : IEquatable<SvgPointF>
    {
        #region Private Fields

        /// <summary>
        /// Represents a new instance of the <see cref="SvgPointF"/> structure 
        /// with member data left uninitialized.
        /// </summary>
        public static readonly SvgPointF Empty = new SvgPointF();

        private double _x;
        private double _y;
        private bool   _notEmpty;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgPointF"/> structure 
        /// with the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the point. </param>
        /// <param name="y">The y-coordinate of the point. </param>
        public SvgPointF(float x, float y)
        {
            _x        = x;
            _y        = y;
            _notEmpty = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgPointF"/> structure 
        /// with the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the point. </param>
        /// <param name="y">The y-coordinate of the point. </param>
        public SvgPointF(double x, double y)
        {
            _x        = x;
            _y        = y;
            _notEmpty = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="SvgPointF"/> is empty.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if both <see cref="SvgPointF.X"/> and 
        /// <see cref="SvgPointF.Y"/> are 0; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                return !_notEmpty;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of this <see cref="SvgPointF"/>.
        /// </summary>
        /// <value>
        /// The x-coordinate of this <see cref="SvgPointF"/>.
        /// </value>
        public float X
        {
            get
            {
                return (float)_x;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of this <see cref="SvgPointF"/>.
        /// </summary>
        /// <value>
        /// The y-coordinate of this <see cref="SvgPointF"/>.
        /// </value>
        public float Y
        {
            get
            {
                return (float)_y;
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of this <see cref="SvgPointF"/>.
        /// </summary>
        /// <value>
        /// The x-coordinate of this <see cref="SvgPointF"/>.
        /// </value>
        public double ValueX
        {
            get
            {
                return _x;
            }
            set
            {
                _x        = value;
                _notEmpty = true;
            }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of this <see cref="SvgPointF"/>.
        /// </summary>
        /// <value>
        /// The y-coordinate of this <see cref="SvgPointF"/>.
        /// </value>
        public double ValueY
        {
            get
            {
                return _y;
            }
            set
            {
                _y        = value;
                _notEmpty = true;
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// This translates the <see cref="SvgPointF"/> by the specified 
        /// <see cref="SvgSizeF"/>.
        /// </summary>
        /// <param name="sz">
        /// The <see cref="SvgSizeF"/> that specifies the numbers to add to the 
        /// x- and y-coordinates of the <see cref="SvgPointF"/>.
        /// </param>
        /// <param name="pt">The <see cref="SvgPointF"/> to translate.</param>
        /// <returns>The translated <see cref="SvgPointF"/>.</returns>
        public static SvgPointF operator +(SvgPointF pt, SvgSizeF sz)
        {
            return SvgPointF.Add(pt, sz);
        }

        /// <summary>
        /// This translates a <see cref="SvgPointF"/> by the negative of a specified 
        /// <see cref="SvgSizeF"/>. 
        /// </summary>
        /// <param name="sz">
        /// The <see cref="SvgSizeF"/> that specifies the numbers to subtract from 
        /// the coordinates of pt.
        /// </param>
        /// <param name="pt">The <see cref="SvgPointF"/> to translate.</param>
        /// <returns>The translated <see cref="SvgPointF"/>.</returns>
        public static SvgPointF operator -(SvgPointF pt, SvgSizeF sz)
        {
            return SvgPointF.Subtract(pt, sz);
        }

        /// <summary>
        /// This compares two <see cref="SvgPointF"/> structures. The result 
        /// specifies whether the values of the <see cref="SvgPointF.X"/> and 
        /// <see cref="SvgPointF.Y"/> properties of the two <see cref="SvgPointF"/> 
        /// structures are equal.
        /// </summary>
        /// <param name="right">A <see cref="SvgPointF"/> to compare. </param>
        /// <param name="left">A <see cref="SvgPointF"/> to compare. </param>
        /// <returns>
        /// This is <see langword="true"/> if the <see cref="SvgPointF.X"/> and 
        /// <see cref="SvgPointF.Y"/> values of the left and right 
        /// <see cref="SvgPointF"/> structures are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(SvgPointF left, SvgPointF right)
        {
            if (left.X.Equals(right.X))
            {
                return (left.Y.Equals(right.Y));
            }

            return false;
        }

        /// <summary>
        /// This determines whether the coordinates of the specified points are 
        /// not equal.
        /// </summary>
        /// <param name="left">A <see cref="SvgPointF"/> to compare.</param>
        /// <param name="right">A <see cref="SvgPointF"/> to compare.</param>
        /// <returns>
        /// This <see langword="true"/> to indicate the <see cref="SvgPointF.X"/> 
        /// and <see cref="SvgPointF.Y"/> values of left and right are not equal; 
        /// otherwise, <see langword="false"/>. 
        /// </returns>
        public static bool operator !=(SvgPointF left, SvgPointF right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This computes the distance between this <see cref="SvgPointF"/>
        /// and the specified <see cref="SvgPointF"/>.
        /// </summary>
        /// <param name="point">
        /// A <see cref="SvgPointF"/> object specifying the other point from
        /// which to determine the distance.
        /// </param>
        /// <returns>
        /// The distance between this point and the specified point.
        /// </returns>
        public double Distance(SvgPointF point)
        {
            return Math.Sqrt((point.X - this.X) * (point.X - this.X)
                + (point.Y - this.Y) * (point.Y - this.Y));
        }

        /// <summary>
        /// This determines whether this <see cref="SvgPointF"/> contains the same 
        /// coordinates as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to test. 
        /// </param>
        /// <returns>
        /// This method returns <see langword="true"/> if the specified object 
        /// is a <see cref="SvgPointF"/> and has the same coordinates as this 
        /// <see cref="SvgPointF"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SvgPointF)
            {
                return Equals((SvgPointF)obj);
            }

            return false;
        }

        /// <summary>
        /// This determines whether this <see cref="SvgPointF"/> contains the same 
        /// coordinates as the specified <see cref="SvgPointF"/>.
        /// </summary>
        /// <param name="other">The <see cref="SvgPointF"/> to test.</param>
        /// <returns>
        /// This method returns <see langword="true"/> if the specified 
        /// <see cref="SvgPointF"/> has the same coordinates as this 
        /// <see cref="SvgPointF"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(SvgPointF other)
        {
            if ((other.X == this.X) && (other.Y == this.Y))
            {
                return other.GetType().Equals(base.GetType());
            }

            return false;
        }

        /// <summary>
        /// This returns a hash code for this <see cref="SvgPointF"/> structure.
        /// </summary>
        /// <returns>
        /// An integer value that specifies a hash value for this 
        /// <see cref="SvgPointF"/> structure.
        /// </returns>
        public override int GetHashCode()
        {
            return (_x.GetHashCode() ^ _y.GetHashCode());
        }

        /// <summary>
        /// This converts this <see cref="SvgPointF"/> to a human readable string.
        /// </summary>
        /// <returns>
        /// A string that represents this <see cref="SvgPointF"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, 
                "{{X={0}, Y={1}}",  _x, _y);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// This translates a given <see cref="SvgPointF"/> by a specified 
        /// <see cref="SvgSizeF"/>.
        /// </summary>
        /// <param name="pt">The <see cref="SvgPointF"/> to translate.</param>
        /// <param name="sz">
        /// The <see cref="SvgSizeF"/> that specifies the numbers to add to the 
        /// coordinates of pt.
        /// </param>
        /// <returns>The translated <see cref="SvgPointF"/>.</returns>
        public static SvgPointF Add(SvgPointF pt, SvgSizeF sz)
        {
            return new SvgPointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <summary>
        /// This translates a <see cref="SvgPointF"/> by the negative of a 
        /// specified size.
        /// </summary>
        /// <param name="pt">The <see cref="SvgPointF"/> to translate.</param>
        /// <param name="sz">
        /// The <see cref="SvgSizeF"/> that specifies the numbers to subtract from 
        /// the coordinates of pt.
        /// </param>
        /// <returns>The translated <see cref="SvgPointF"/>.</returns>
        public static SvgPointF Subtract(SvgPointF pt, SvgSizeF sz)
        {
            return new SvgPointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        #endregion
    }
}
