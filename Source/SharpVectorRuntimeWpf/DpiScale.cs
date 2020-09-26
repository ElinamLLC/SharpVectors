// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Source: https://github.com/microsoft/Snip-Insights

using System;

namespace SharpVectors.Runtime
{
    /// <summary>
    /// Stores DPI information from which a <see cref="System.Windows.Media.Visual"/> 
    /// or <see cref="System.Windows.UIElement"/> is rendered.
    /// </summary>
    [Serializable]
    public class DpiScale : IEquatable<DpiScale>, ICloneable
    {
        private readonly double _dpiScaleX;
        private readonly double _dpiScaleY;

        /// <summary>
        /// Initializes a new instance of the DpiScale class.
        /// </summary>
        public DpiScale()
            : this(1, 1)
        {

        }

        /// <summary>
        /// Initializes a new instance of the DpiScale class.
        /// </summary>
        public DpiScale(double value)
            : this(value, value)
        {

        }

        /// <summary>
        /// Initializes a new instance of the DpiScale class.
        /// </summary>
        /// <param name="dpiScaleX">DPI scale on X-axis</param>
        /// <param name="dpiScaleY">DPI scale on Y-axis</param>         
        public DpiScale(double dpiScaleX, double dpiScaleY)
        {
            if (double.IsNaN(dpiScaleX) || dpiScaleX <= 0)
                throw new ArgumentOutOfRangeException("dpiScaleX");

            if (double.IsNaN(dpiScaleY) || dpiScaleY <= 0)
                throw new ArgumentOutOfRangeException("dpiScaleY");

            _dpiScaleX = dpiScaleX;
            _dpiScaleY = dpiScaleY;
        }

        /// <summary>
        /// Gets the DPI scale on the X axis.When DPI is 96, <see cref="DpiScaleX"/> is 1. 
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="DpiScaleY"/>
        /// </remarks>
        public double DpiScaleX
        {
            get { return _dpiScaleX; }
        }

        /// <summary>
        /// Gets the DPI scale on the Y axis. When DPI is 96, <see cref="DpiScaleY"/> is 1. 
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="DpiScaleX"/>
        /// </remarks>
        public double DpiScaleY
        {
            get { return _dpiScaleY; }
        }

        /// <summary>
        /// Get or sets the PixelsPerDip at which the text should be rendered.
        /// </summary>
        public double PixelsPerDip
        {
            get { return _dpiScaleY; }
        }

        /// <summary>
        /// Gets the PPI along X axis.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="PixelsPerInchY"/>
        /// </remarks>
        public double PixelsPerInchX
        {
            get { return DpiUtilities.DefaultPixelsPerInch * _dpiScaleX; }
        }

        /// <summary>
        /// Gets the PPI along Y axis.
        /// </summary>
        /// <remarks>
        /// On Windows Desktop, this value is the same as <see cref="PixelsPerInchX"/>
        /// </remarks>
        public double PixelsPerInchY
        {
            get { return DpiUtilities.DefaultPixelsPerInch * _dpiScaleY; }
        }

        public override string ToString()
        {
            return "X=" + _dpiScaleX.ToString("0.###") + ", Y=" + _dpiScaleY.ToString("0.###");
        }

        #region Equatable Members

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="obj">The object being compared against</param>
        /// <returns>True if the objects are equal, False otherwise</returns>
        public sealed override bool Equals(object obj)
        {
            if (obj is DpiScale)
            {
                return Equals((DpiScale)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code of the current object
        /// </summary>
        /// <returns>The hash code of the object</returns>
        public override int GetHashCode()
        {
            return _dpiScaleX.GetHashCode() + _dpiScaleY.GetHashCode() << 2;
        }

        /// <summary>
        /// Equality test against a <see cref="DpiScale"/> object.
        /// </summary>
        /// <param name="dpiScale2">The object being compared against</param>
        /// <returns>True if the objects are equal, False otherwise</returns>
        /// <remarks>
        /// Two DPI scale values are equal if they are equal after rounding up
        /// to hundredths place.
        ///
        /// Common PPI values in use are:
        /// <list type="table">
        /// <listheader><term>PPI</term><term>DPI(%)</term><term>DPI(Ratio)</term></listheader>
        /// <item><term>96</term><term>100%</term><term>1.00</term></item>
        /// <item><term>120</term><term>125%</term><term>1.25</term></item>
        /// <item><term>144</term><term>150%</term><term>1.50</term></item>
        /// <item><term>192</term><term>200%</term><term>2.00</term></item>
        /// </list>
        /// </remarks>
        public bool Equals(DpiScale other)
        {
            if (other is null)
            {
                return false;
            }

            return _dpiScaleX.Equals(other._dpiScaleX) && _dpiScaleY.Equals(other._dpiScaleY);
        }

        /// <summary>
        /// Checks to inequality between two <see cref="DpiScale"/> instances.
        /// </summary>
        /// <param name="dpiScaleA">The first object being compared</param>
        /// <param name="dpiScaleB">The second object being compared</param>
        /// <returns>True if the objects are not equal, otherwise False</returns>
        public static bool operator !=(DpiScale dpiScaleA, DpiScale dpiScaleB)
        {
            if ((object.ReferenceEquals(dpiScaleA, null) && !object.ReferenceEquals(dpiScaleB, null)) ||
                (!object.ReferenceEquals(dpiScaleA, null) && object.ReferenceEquals(dpiScaleB, null)))
            {
                return true;
            }

            return !dpiScaleA.Equals(dpiScaleB);
        }

        /// <summary>
        /// Checks for equality between two <see cref="DpiScale"/> instances.
        /// </summary>
        /// <param name="dpiScaleA">The first object being compared</param>
        /// <param name="dpiScaleB">The second object being compared</param>
        /// <returns>True if the two objects are equal, otherwise False</returns>
        public static bool operator ==(DpiScale dpiScaleA, DpiScale dpiScaleB)
        {
            if (object.ReferenceEquals(dpiScaleA, null) &&
                object.ReferenceEquals(dpiScaleB, null))
            {
                return true;
            }

            return dpiScaleA.Equals(dpiScaleB);
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public DpiScale Clone()
        {
            return new DpiScale(_dpiScaleX, _dpiScaleY);
        }

        #endregion
    }
}
