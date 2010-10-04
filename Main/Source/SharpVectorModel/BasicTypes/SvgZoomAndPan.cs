using System;
using System.Xml;

using SharpVectors.Dom.Css;


namespace SharpVectors.Dom.Svg
{
    public sealed class SvgZoomAndPan
    {
        public SvgZoomAndPan(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
        }

        #region Private fields
        private SvgElement ownerElement;
        #endregion

        public SvgZoomAndPanType ZoomAndPan
        {
            get
            {
                if (ownerElement != null && ownerElement.HasAttribute("zoomAndPan"))
                {
                    switch (ownerElement.GetAttribute("zoomAndPan").Trim())
                    {
                        case "magnify": return SvgZoomAndPanType.Magnify;
                        case "disable": return SvgZoomAndPanType.Disable;
                    }
                }
                return SvgZoomAndPanType.Unknown;
            }
        }
    }
}
