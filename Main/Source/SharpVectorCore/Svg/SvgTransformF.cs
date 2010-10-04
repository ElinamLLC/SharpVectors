using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This is an implementation of the 3-by-3 affine matrix that represents 
    /// a geometric transform.
    /// </summary>
    public class SvgTransformF : ICloneable
    {
        #region Private Fields

        private float m11;
        private float m12;
        private float m21;
        private float m22;
        private float dx;
        private float dy;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class
        /// as the identity transform or matrix.
        /// </summary>
        public SvgTransformF()
        {
            m11 = 1.0f;
            m12 = 0.0f;
            m21 = 0.0f;
            m22 = 1.0f;
            dx  = 0.0f;
            dy  = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class 
        /// to the geometric transform defined by the specified rectangle and 
        /// array of points.
        /// </summary>
        /// <param name="rect">
        /// A <see cref="SvgRectF"/> structure that represents the rectangle 
        /// to be transformed.
        /// </param>
        /// <param name="plgpts">
        /// An array of three <see cref="SvgPointF"/> structures that represents the 
        /// points of a parallelogram to which the upper-left, upper-right, and 
        /// lower-left corners of the rectangle is to be transformed. The 
        /// lower-right corner of the parallelogram is implied by the first three 
        /// corners.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="plgpts"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the length of the <paramref name="plgpts"/> array is not equal
        /// to 3.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the width or height of the <paramref name="rect"/> is zero.
        /// </exception>
        public SvgTransformF(SvgRectF rect, SvgPointF[] plgpts)
        {
            if (plgpts == null)
            {
                throw new ArgumentNullException("plgpts");
            }
            if (plgpts.Length != 3)
            {
                throw new ArgumentException("plgpts");
            }

            if ((rect.Width == 0) || (rect.Height == 0))
            {
                throw new ArgumentOutOfRangeException("rect");
            }

            MapRectToRect(rect, plgpts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class 
        /// with the specified elements.
        /// </summary>
        /// <param name="elements">
        /// An array of six items defining the transform.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="elements"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the length of the <paramref name="elements"/> array is not equal
        /// to 6.
        /// </exception>
        public SvgTransformF(float[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }
            if (elements.Length != 6)
            {
                throw new ArgumentException("elements");
            }

            this.m11 = elements[0];
            this.m12 = elements[1];
            this.m21 = elements[2];
            this.m22 = elements[3];
            this.dx  = elements[4];
            this.dy  = elements[5];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class 
        /// with the specified elements.
        /// </summary>
        /// <param name="m11">
        /// The value in the first row and first column of the new <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="m12">
        /// The value in the first row and second column of the new <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="m21">
        /// The value in the second row and first column of the new <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="m22">
        /// The value in the second row and second column of the new <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="dx">
        /// The value in the third row and first column of the new <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="dy">
        /// The value in the third row and second column of the new <see cref="SvgTransformF"/>.
        /// </param>
        public SvgTransformF(float m11, float m12, float m21, float m22,
                      float dx, float dy)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.dx = dx;
            this.dy = dy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgTransformF"/> class
        /// with parameters copied from the specified parameter, a copy 
        /// constructor.
        /// </summary>
        /// <param name="source">
        /// The <see cref="SvgTransformF"/> instance from which the parameters
        /// are to be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public SvgTransformF(SvgTransformF source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.m11 = source.m11;
            this.m12 = source.m12;
            this.m21 = source.m21;
            this.m22 = source.m22;
            this.dx  = source.dx;
            this.dy  = source.dy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an array of floating-point values that represents the elements 
        /// of this <see cref="SvgTransformF"/>.
        /// </summary>
        /// <value>
        /// An array of floating-point values that represents the elements 
        /// of this <see cref="SvgTransformF"/>.
        /// </value>
        public float[] Elements
        {
            get
            {
				return new float[] {m11, m12, m21, m22, dx, dy};
            }
        }

        /// <summary>               
        /// Gets a value indicating whether this <see cref="SvgTransformF"/> is the 
        /// identity matrix.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this 
        /// <see cref="SvgTransformF"/> is identity; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsIdentity
        {
            get
            {
                return (m11 == 1.0f && m12 == 0.0f &&
                        m21 == 0.0f && m22 == 1.0f &&
                        dx == 0.0f && dy == 0.0f);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SvgTransformF"/> is 
        /// invertible.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this 
        /// <see cref="SvgTransformF"/> is invertible; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsInvertible
        {
            get
            {
                return ((m11 * m22 - m21 * m11) != 0.0f);
            }
        }

        /// <summary>
        /// Gets the <c>x</c> translation value (the <c>dx</c> value, or the 
        /// element in the third row and first column) of this <see cref="SvgTransformF"/>.
        /// </summary>
        /// <value>
        /// The <c>x</c> translation value of this <see cref="SvgTransformF"/>.
        /// </value>
        public float OffsetX
        {
            get
            {
                return dx;
            }
        }

        /// <summary>
        /// Gets the <c>y</c> translation value (the <c>dy</c> value, or the 
        /// element in the third row and second column) of this <see cref="SvgTransformF"/>.
        /// </summary>
        /// <value>
        /// The <c>y</c> translation value of this <see cref="SvgTransformF"/>.
        /// </value>
        public float OffsetY
        {
            get
            {
                return dy;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine whether the specified object is a <see cref="SvgTransformF"/> 
        /// and is identical to this <see cref="SvgTransformF"/>.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>
        /// This method returns <see langword="true"/> if obj is the specified 
        /// <see cref="SvgTransformF"/> identical to this 
        /// <see cref="SvgTransformF"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(Object obj)
        {
            SvgTransformF other = (obj as SvgTransformF);
            if (other != null)
            {
                return (other.m11 == m11 && other.m12 == m12 &&
                        other.m21 == m21 && other.m22 == m22 &&
                        other.dx  == dx  && other.dy  == dy);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code.
        /// </summary>
        /// <returns>The hash code for this <see cref="SvgTransformF"/>.</returns>
        public override int GetHashCode()
        {
            return (int)(m11 + m12 + m21 + m22 + dx + dy);
        }

        /// <summary>
        /// Inverts this <see cref="SvgTransformF"/>, if it is invertible.
        /// </summary>
        public void Invert()
        {
            float determinant = this.m11 * this.m22 - this.m21 * this.m11;
            if (determinant != 0.0f)
            {
                float nm11 = this.m22 / determinant;
                float nm12 = -(this.m12 / determinant);
                float nm21 = -(this.m21 / determinant);
                float nm22 = this.m11 / determinant;
                float ndx  = (this.m12 * this.dy - this.m22 * this.dx) / determinant;
                float ndy  = (this.m21 * this.dx - this.m11 * this.dy) / determinant;

                this.m11 = nm11;
                this.m12 = nm12;
                this.m21 = nm21;
                this.m22 = nm22;
                this.dx  = ndx;
                this.dy  = ndy;
            }
        }

        /// <overloads>
        /// Multiplies this <see cref="SvgTransformF"/> by the specified 
        /// <see cref="SvgTransformF"/> by appending or prepending the specified 
        /// <see cref="SvgTransformF"/>.
        /// </overloads>
        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the specified 
        /// <see cref="SvgTransformF"/> by prepending the specified 
        /// <see cref="SvgTransformF"/>.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        public void Multiply(SvgTransformF matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }
            Multiply((SvgTransformF)matrix, this);
        }

        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the matrix specified in 
        /// the matrix parameter, and in the order specified in the order parameter.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        /// <param name="order">
        /// The <see cref="TransformOrder"/> that represents the order of the 
        /// multiplication.
        /// </param>
        public void Multiply(SvgTransformF matrix, SvgTransformOrder order)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }
            if (order == SvgTransformOrder.Prepend)
            {
                Multiply((SvgTransformF)matrix, this);
            }
            else
            {
                Multiply(this, (SvgTransformF)matrix);
            }
        }

        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the specified 
        /// <see cref="SvgTransformF"/> by prepending the specified 
        /// <see cref="SvgTransformF"/>.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        //public void Multiply(SvgTransformF matrix)
        //{
        //    if (matrix == null)
        //    {
        //        throw new ArgumentNullException("matrix");
        //    }
        //    Multiply(matrix, this);
        //}

        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the matrix specified in 
        /// the matrix parameter, and in the order specified in the order parameter.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        /// <param name="order">
        /// The <see cref="TransformOrder"/> that represents the order of the 
        /// multiplication.
        /// </param>
        //public void Multiply(SvgTransformF matrix, TransformOrder order)
        //{
        //    if (matrix == null)
        //    {
        //        throw new ArgumentNullException("matrix");
        //    }
        //    if (order == TransformOrder.Prepend)
        //    {
        //        Multiply(matrix, this);
        //    }
        //    else
        //    {
        //        Multiply(this, matrix);
        //    }
        //}

        /// <summary>
        /// Resets this <see cref="SvgTransformF"/> to have the elements of the 
        /// identity matrix.
        /// </summary>
        public void Reset()
        {
            m11 = 1.0f;
            m12 = 0.0f;
            m21 = 0.0f;
            m22 = 0.1f;
            dx  = 0.0f;
            dy  = 0.0f;
        }

        /// <overloads>
        /// Applies a clockwise rotation of the specified angle about the 
        /// origin to this <see cref="SvgTransformF"/>. 
        /// </overloads>
        /// <summary>
        /// Applies a clockwise rotation of the specified angle about the 
        /// origin to this <see cref="SvgTransformF"/>. 
        /// </summary>
        /// <param name="angle">
        /// The angle (extent) of the rotation, in degrees.
        /// </param>
        public void Rotate(float angle)
        {
			double radians = (angle * (Math.PI / 180.0));
			float cos = (float)(Math.Cos(radians));
			float sin = (float)(Math.Sin(radians));

			float nm11 = cos * this.m11 + sin * this.m21;
			float nm12 = cos * this.m12 + sin * this.m22;
			float nm21 = cos * this.m21 - sin * this.m11;
			float nm22 = cos * this.m22 - sin * this.m12;

			this.m11 = nm11;
			this.m12 = nm12;
			this.m21 = nm21;
			this.m22 = nm22;
        }

        /// <summary>
        /// Applies a clockwise rotation of the specified angle about the 
        /// origin to this <see cref="SvgTransformF"/>, and in the order specified 
        /// in the order parameter. 
        /// </summary>
        /// <param name="angle">
        /// The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="order">
        /// A <see cref="TransformOrder"/> that specifies the order (append or 
        /// prepend) in which the rotation is applied to this 
        /// <see cref="SvgTransformF"/>.
        /// </param>
        public void Rotate(float angle, SvgTransformOrder order)
        {
			double radians = (angle * (Math.PI / 180.0));
			float cos = (float)(Math.Cos(radians));
			float sin = (float)(Math.Sin(radians));

			if (order == SvgTransformOrder.Prepend)
			{
				float nm11 = cos * this.m11 + sin * this.m21;
				float nm12 = cos * this.m12 + sin * this.m22;
				float nm21 = cos * this.m21 - sin * this.m11;
				float nm22 = cos * this.m22 - sin * this.m12;

				this.m11 = nm11;
				this.m12 = nm12;
				this.m21 = nm21;
				this.m22 = nm22;
			}
			else
			{
				float nm11 = this.m11 * cos - this.m12 * sin;
				float nm12 = this.m11 * sin + this.m12 * cos;
				float nm21 = this.m21 * cos - this.m22 * sin;
				float nm22 = this.m21 * sin + this.m22 * cos;
				float ndx  = this.dx  * cos - this.dy  * sin;
				float ndy  = this.dx  * sin + this.dy  * cos;

				this.m11 = nm11;
				this.m12 = nm12;
				this.m21 = nm21;
				this.m22 = nm22;
				this.dx  = ndx;
				this.dy  = ndy;
			}
        }

        /// <overloads>
        /// Applies a clockwise rotation about the specified point to this 
        /// <see cref="SvgTransformF"/> by appending or prepending the rotation.
        /// </overloads>
        /// <summary>
        /// Applies a clockwise rotation about the specified point to this 
        /// <see cref="SvgTransformF"/> by prepending the rotation.
        /// </summary>
        /// <param name="angle">
        /// The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="point">
        /// A <see cref="SvgPointF"/> that represents the center of the rotation.
        /// </param>
        public void RotateAt(float angle, SvgPointF point)
        {
            Translate(point.X, point.Y);
            Rotate(angle);
            Translate(-point.X, -point.Y);
        }

        /// <summary>
        /// Applies a clockwise rotation about the specified point to this 
        /// <see cref="SvgTransformF"/> in the specified order. 
        /// </summary>
        /// <param name="angle">
        /// The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="point">
        /// A <see cref="SvgPointF"/> that represents the center of the rotation.
        /// </param>
        /// <param name="order">
        /// A <see cref="TransformOrder"/> that specifies the order (append or 
        /// prepend) in which the rotation is applied.
        /// </param>
        public void RotateAt(float angle, SvgPointF point, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                Translate(point.X, point.Y);
                Rotate(angle);
                Translate(-point.X, -point.Y);
            }
            else
            {
                Translate(-point.X, -point.Y);
                Rotate(angle, SvgTransformOrder.Append);
                Translate(point.X, point.Y);
            }
        }

        /// <overloads>
        /// Applies the specified scale vector to this <see cref="SvgTransformF"/> 
        /// by appending or prepending the scale vector.
        /// </overloads>
        /// <summary>
        /// Applies the specified scale vector to this <see cref="SvgTransformF"/> 
        /// by prepending the scale vector.
        /// </summary>
        /// <param name="scaleX">
        /// The value by which to scale this <see cref="SvgTransformF"/> in the 
        /// x-axis direction.
        /// </param>
        /// <param name="scaleY">
        /// The value by which to scale this <see cref="SvgTransformF"/> in the 
        /// y-axis direction.
        /// </param>
        public void Scale(float scaleX, float scaleY)
        {
            m11 *= scaleX;
            m12 *= scaleX;
            m21 *= scaleY;
            m22 *= scaleY;
        }

        /// <summary>
        /// Applies the specified scale vector to this <see cref="SvgTransformF"/> 
        /// using the specified order.
        /// </summary>
        /// <param name="scaleX">
        /// The value by which to scale this <see cref="SvgTransformF"/> in the 
        /// x-axis direction.
        /// </param>
        /// <param name="scaleY">
        /// The value by which to scale this <see cref="SvgTransformF"/> in the 
        /// y-axis direction.
        /// </param>
        /// <param name="order">
        /// A <see cref="TransformOrder"/> that specifies the order (append or 
        /// prepend) in which the scale vector is applied to this 
        /// <see cref="SvgTransformF"/>.
        /// </param>
        public void Scale(float scaleX, float scaleY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                m11 *= scaleX;
                m12 *= scaleX;
                m21 *= scaleY;
                m22 *= scaleY;
            }
            else
            {
                m11 *= scaleX;
                m12 *= scaleY;
                m21 *= scaleX;
                m22 *= scaleY;
                dx *= scaleX;
                dy *= scaleY;
            }
        }

        /// <overloads>
        /// Applies the specified shear vector to this <see cref="SvgTransformF"/> by 
        /// appending or prepending the shear vector.
        /// </overloads>
        /// <summary>
        /// Applies the specified shear vector to this <see cref="SvgTransformF"/> by 
        /// prepending the shear vector.
        /// </summary>
        /// <param name="shearX">
        /// The horizontal shear factor.
        /// </param>
        /// <param name="shearY">
        /// The vertical shear factor.
        /// </param>
        public void Shear(float shearX, float shearY)
        {
            float nm11 = this.m11 + this.m21 * shearY;
            float nm12 = this.m12 + this.m22 * shearY;
            float nm21 = this.m11 * shearX + this.m21;
            float nm22 = this.m12 * shearX + this.m22;

            this.m11 = nm11;
            this.m12 = nm12;
            this.m21 = nm21;
            this.m22 = nm22;
        }

        /// <summary>
        /// Applies the specified shear vector to this <see cref="SvgTransformF"/> in 
        /// the specified order.
        /// </summary>
        /// <param name="shearX">
        /// The horizontal shear factor.
        /// </param>
        /// <param name="shearY">
        /// The vertical shear factor.
        /// </param>
        /// <param name="order">
        /// A <see cref="TransformOrder"/> that specifies the order (append or 
        /// prepend) in which the shear is applied.
        /// </param>
        public void Shear(float shearX, float shearY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                float nm11 = this.m11 + this.m21 * shearY;
                float nm12 = this.m12 + this.m22 * shearY;
                float nm21 = this.m11 * shearX + this.m21;
                float nm22 = this.m12 * shearX + this.m22;

                this.m11 = nm11;
                this.m12 = nm12;
                this.m21 = nm21;
                this.m22 = nm22;
            }
            else
            {
                float nm11 = this.m11 + this.m12 * shearX;
                float nm12 = this.m11 * shearY + this.m12;
                float nm21 = this.m21 + this.m22 * shearX;
                float nm22 = this.m21 * shearY + this.m22;
                float ndx = this.dx + this.dy * shearX;
                float ndy = this.dx * shearY + this.dy;

                this.m11 = nm11;
                this.m12 = nm12;
                this.m21 = nm21;
                this.m22 = nm22;
                this.dx  = ndx;
                this.dy  = ndy;
            }
        }

        /// <overloads>
        /// Applies the specified translation vector to this <see cref="SvgTransformF"/> 
        /// by appending or prepending the translation vector.
        /// </overloads>
        /// <summary>
        /// Applies the specified translation vector to this <see cref="SvgTransformF"/> 
        /// by prepending the translation vector.
        /// </summary>
        /// <param name="offsetX">
        /// The <c>x</c> value by which to translate this <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="offsetY">
        /// The <c>y</c> value by which to translate this <see cref="SvgTransformF"/>.
        /// </param>
        public void Translate(float offsetX, float offsetY)
        {
            dx += offsetX * m11 + offsetY * m21;
            dy += offsetX * m12 + offsetY * m22;
        }

        /// <summary>
        /// Applies the specified translation vector to this <see cref="SvgTransformF"/> 
        /// in the specified order.
        /// </summary>
        /// <param name="offsetX">
        /// The <c>x</c> value by which to translate this <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="offsetY">
        /// The <c>y</c> value by which to translate this <see cref="SvgTransformF"/>.
        /// </param>
        /// <param name="order">
        /// A <see cref="TransformOrder"/> that specifies the order (append or 
        /// prepend) in which the translation is applied to this <see cref="SvgTransformF"/>.
        /// </param>
        public void Translate(float offsetX, float offsetY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                dx += offsetX * m11 + offsetY * m21;
                dy += offsetX * m12 + offsetY * m22;
            }
            else
            {
                dx += offsetX;
                dy += offsetY;
            }
        }

        /// <summary>
        /// Applies the geometric transform represented by this 
        /// <see cref="SvgTransformF"/> to a specified point. 
        /// </summary>
        /// <param name="x">The input <c>x</c> value of the point.</param>
        /// <param name="y">The input <c>y</c> value of the point.</param>
        /// <param name="ox">The transformed <c>x</c> value of the point.</param>
        /// <param name="oy">The transformed <c>y</c> value of the point.</param>
        public void Transform(float x, float y, out float ox, out float oy)
        {
            ox = x * m11 + y * m21 + dx;
            oy = x * m12 + y * m22 + dy;
        }

        /// <summary>
        /// Applies the reverse geometric transform represented by this 
        /// <see cref="SvgTransformF"/> to a specified point. 
        /// </summary>
        /// <param name="x">The input <c>x</c> value of the point.</param>
        /// <param name="y">The input <c>y</c> value of the point.</param>
        /// <param name="ox">The transformed <c>x</c> value of the point.</param>
        /// <param name="oy">The transformed <c>y</c> value of the point.</param>
        public void ReverseTransform(float x, float y, out float ox, out float oy)
        {
            float determinant = this.m11 * this.m22 - this.m21 * this.m11;
            if (determinant != 0.0f)
            {
                float nm11 =   this.m22 / determinant;
                float nm12 = -(this.m12 / determinant);
                float nm21 = -(this.m21 / determinant);
                float nm22 =   this.m11 / determinant;

                ox = x * nm11 + y * nm21;
                oy = x * nm12 + y * nm22;
            }
            else
            {
                ox = x;
                oy = y;
            }
        }

        /// <summary>
        /// Applies the geometric transform represented by this 
        /// <see cref="SvgTransformF"/> to a specified array of points. 
        /// </summary>
        /// <param name="pts">
        /// An array of <see cref="SvgPointF"/> structures that represents the points 
        /// to transform.
        /// </param>
        public void TransformPoints(SvgPointF[] pts)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            int nLength = pts.Length;

            for (int i = nLength - 1; i >= 0; --i)
            {
                float x  = pts[i].X;
                float y  = pts[i].Y;
                pts[i].ValueX = x * m11 + y * m21 + dx;
                pts[i].ValueY = x * m12 + y * m22 + dy;
            }
        }

        /// <summary>
        /// Multiplies each vector in an array by the matrix. The translation 
        /// elements of this matrix (third row) are ignored.
        /// </summary>
        /// <param name="pts">
        /// An array of <see cref="SvgPointF"/> structures that represents the points 
        /// to transform.
        /// </param>
        public void TransformVectors(SvgPointF[] pts)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            int nLength = pts.Length;

            for (int i = nLength - 1; i >= 0; --i)
            {
                float x  = pts[i].X;
                float y  = pts[i].Y;
                pts[i].ValueX = x * m11 + y * m21;
                pts[i].ValueY = x * m12 + y * m22;
            }
        }

        #endregion

        #region Private Methods

        private void Multiply(SvgTransformF a, SvgTransformF b)
        {
            float nm11 = a.m11 * b.m11 + a.m12 * b.m21;
            float nm12 = a.m11 * b.m12 + a.m12 * b.m22;
            float nm21 = a.m21 * b.m11 + a.m22 * b.m21;
            float nm22 = a.m21 * b.m12 + a.m22 * b.m22;
            float ndx  = a.dx  * b.m11 + a.dy  * b.m21 + b.dx;
            float ndy  = a.dx  * b.m12 + a.dy  * b.m22 + b.dy;

            this.m11 = nm11;
            this.m12 = nm12;
            this.m21 = nm21;
            this.m22 = nm22;
            this.dx  = ndx;
            this.dy  = ndy;
        }

        private void MapRectToRect(SvgRectF rect, SvgPointF[] plgpts)
        {
            SvgPointF pt1 = new SvgPointF(plgpts[1].X - plgpts[0].X,
                            plgpts[1].Y - plgpts[0].Y);
            SvgPointF pt2 = new SvgPointF(plgpts[2].X - plgpts[0].X,
                            plgpts[2].Y - plgpts[0].Y);

            this.m11 = pt1.X / rect.Width;
            this.m12 = pt1.Y / rect.Width;
            this.m21 = pt2.X / rect.Height;
            this.m22 = pt2.Y / rect.Height;
            this.dx = plgpts[0].X - rect.X / rect.Width * pt1.X - rect.Y / rect.Height * pt2.X;
            this.dy = plgpts[0].Y - rect.X / rect.Width * pt1.Y - rect.Y / rect.Height * pt2.Y;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new <see cref="SvgTransformF"/> that is a deep 
        /// copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public SvgTransformF Clone()
        {
            return new SvgTransformF(this.m11, this.m12,
                              this.m21, this.m22,
                              this.dx, this.dy);
        }

        /// <summary>
        /// This creates a new <see cref="SvgTransformF"/> that is a deep 
        /// copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        //ITransform ITransform.Clone()
        //{
        //    return this.Clone();
        //}

        /// <summary>
        /// This creates a new <see cref="SvgTransformF"/> that is a deep 
        /// copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}