using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedNumberList.
    /// </summary>
    public sealed class SvgAnimatedNumberList : ISvgAnimatedNumberList
    {
        #region Private Fields

        public readonly static SvgAnimatedNumberList Empty = new SvgAnimatedNumberList();

        private SvgNumberList _baseVal;
        private SvgNumberList _animVal;

        #endregion

        #region Constructor

        public SvgAnimatedNumberList()
        {
            _baseVal = new SvgNumberList();
            _animVal = _baseVal;
        }

        public SvgAnimatedNumberList(string str)
        {
            _baseVal = new SvgNumberList(str);
            _animVal = _baseVal;
        }

        #endregion

        #region ISvgAnimatedNumberList Interface

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ISvgAnimatedNumberList"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ISvgAnimatedNumberList"/>.</value>
        public int Count
        {
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
        public ISvgAnimatedNumber this[uint index]
        {
            get {
                if (_animVal != null && _baseVal != null && _animVal.NumberOfItems == _baseVal.NumberOfItems)
                {
                    return new SvgAnimatedNumber(_baseVal.GetItem(index).Value, _animVal.GetItem(index).Value);
                }
                return null;
            }
        }

        /// <summary>
        /// The base value of the given attribute before applying any animations
        /// </summary>
        public ISvgNumberList BaseVal
        {
            get {
                return _baseVal;
            }
        }

        /// <summary>
        /// If the given attribute or property is being animated, then this attribute contains the current animated
        /// value of the attribute or property, and both the object itself and its contents are readonly. If the given 
        /// attribute or property is not currently being animated, then this attribute contains the same value as 'BaseVal'.
        /// </summary>
        public ISvgNumberList AnimVal
        {
            get {
                return _animVal;
            }
        }

        #endregion
    }
}