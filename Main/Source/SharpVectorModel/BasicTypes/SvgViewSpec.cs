using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// 
    /// TODO:  This class does not yet support custom views
    /// </summary>
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
                return _ownerElement.GetAttribute("preserveAspectRatio");
            }
        }

        public string ViewBoxString
        {
            get
            {
                return _ownerElement.GetAttribute("viewBox");
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
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
