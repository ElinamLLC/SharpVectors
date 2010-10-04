using System;
using System.Text;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAElement : SvgTransformableElement
    {
        #region Constructors and Destructor

        public SvgAElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion
    }
}
