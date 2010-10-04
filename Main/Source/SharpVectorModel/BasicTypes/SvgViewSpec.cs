using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    // TODO:  This class does not yet support custom views
    public sealed class SvgViewSpec : SvgFitToViewBox, ISvgViewSpec
    {
        #region Constructors and Destructor

        public SvgViewSpec(SvgElement ownerElement)
            : base(ownerElement)
        {
            // only use the base... 
        }

        #endregion

        #region ISvgViewSpec Members

        public string TransformString
        {
            get
            {
                return null;
            }
        }

        public ISvgElement ViewTarget
        {
            get
            {
                return null;
            }
        }

        public string PreserveAspectRatioString
        {
            get
            {
                return ownerElement.GetAttribute("preserveAspectRatio");
            }
        }

        public string ViewBoxString
        {
            get
            {
                return ownerElement.GetAttribute("viewBox");
            }
        }

        public string ViewTargetString
        {
            get
            {
                return null;
            }
        }

        public ISvgTransformList Transform
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region ISvgZoomAndPan Members

        public SharpVectors.Dom.Svg.SvgZoomAndPanType ZoomAndPan
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
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
