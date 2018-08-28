using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Specifies the order for matrix transform operations.
    /// </summary>
    [Serializable]
    public enum SvgTransformOrder
    {
        /// <summary>
        /// The new operation is applied after the old operation.
        /// </summary>
	    Prepend = 0,
        /// <summary>
        /// The new operation is applied before the old operation.
        /// </summary>
	    Append  = 1,
    }
}
