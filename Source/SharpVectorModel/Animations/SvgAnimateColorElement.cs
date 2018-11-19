using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimateColorElement : SvgAnimateBaseElement, ISvgAnimateColorElement
    {
        #region Constructors and Destructors

        public SvgAnimateColorElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion
    }
}
