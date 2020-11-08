using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgStopElement.
    /// </summary>
    public sealed class SvgStopElement : SvgStyleableElement, ISvgStopElement
    {
        public SvgStopElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get {
                return false;
            }
        }

        public ISvgAnimatedNumber Offset
        {
            get {
                string attr = GetAttribute("offset").Trim();
                if (string.IsNullOrWhiteSpace(attr))
                {
                    return null;
                }
                if (attr.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                {
                    attr = attr.TrimEnd(new char[1] { '%' });
                    double tmp = SvgNumber.ParseNumber(attr);

                    if (tmp > 100)
                    {
                        attr = "100";
                    }
                    else if (tmp < 0)
                    {
                        attr = SvgConstants.ValZero;
                    }
                }
                else
                {
                    double tmp = SvgNumber.ParseNumber(attr) * 100;
                    if (tmp > 100)
                    {
                        attr = "100";
                    }
                    else if (tmp < 0)
                    {
                        attr = SvgConstants.ValZero;
                    }
                    else
                    {
                        attr = tmp.ToString(SvgNumber.Format);
                    }
                }

                return new SvgAnimatedNumber(attr);
            }
        }
    }
}
