using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// </summary>
    public sealed class SvgPolygonElement : SvgPolyElement, ISvgPolygonElement
    {
        #region Constructors and Destructor

        public SvgPolygonElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISharpMarkerHost Members

        public override SvgPointF[] MarkerPositions
        {
            get {
                SvgPointF[] p1 = base.MarkerPositions;
                SvgPointF[] p2 = new SvgPointF[p1.Length + 1];
                Array.Copy(p1, 0, p2, 0, p1.Length);
                p2[p2.Length - 1] = p1[0];

                return p2;
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion
    }
}