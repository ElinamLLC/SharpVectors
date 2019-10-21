using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedLengthList : ISvgAnimatedLengthList
    {
        #region Private Fields

        private SvgLengthList _baseVal;
        private SvgLengthList _animVal;

        #endregion

        #region Constructors

        public SvgAnimatedLengthList(string propertyName, string str, SvgElement ownerElement, SvgLengthDirection direction)
        {
            _baseVal = new SvgLengthList(propertyName, str, ownerElement, direction);
            _animVal = _baseVal;
        }

        #endregion

        #region ISvgAnimatedLengthList Interface

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ISvgAnimatedLengthList"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ISvgAnimatedLengthList"/>.</value>
        public int Count {
            get {
                if (_animVal != null && _baseVal != null && _animVal.NumberOfItems == _baseVal.NumberOfItems)
                {
                    return (int)_animVal.NumberOfItems;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public ISvgAnimatedLength this[uint index]
        {
            get {
                if (_animVal != null && _baseVal != null && _animVal.NumberOfItems == _baseVal.NumberOfItems)
                {
                    return new SvgAnimatedLength(_baseVal.GetItem(index), _animVal.GetItem(index));
                }
                return null;
            }
        }

        public ISvgLengthList BaseVal
        {
            get {
                return _baseVal;
            }
        }

        public ISvgLengthList AnimVal
        {
            get {
                return _animVal;
            }
        }

        #endregion
    }
}