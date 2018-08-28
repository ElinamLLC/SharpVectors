using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgZoomAndPan
    {
        private SvgElement _ownerElement;

        public SvgZoomAndPan(SvgElement ownerElement)
        {
            _ownerElement = ownerElement;
        }

        public SvgZoomAndPanType ZoomAndPan
        {
            get
            {
                if (_ownerElement != null && _ownerElement.HasAttribute("zoomAndPan"))
                {
                    switch (_ownerElement.GetAttribute("zoomAndPan").Trim())
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
