using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPreserveAspectRatio : ISvgPreserveAspectRatio
    {
        #region Static members

        private static Regex parCheck = new Regex("^(?<align>[A-Za-z]+)\\s*(?<meet>[A-Za-z]*)$");

        #endregion

        #region Private Fields

        private bool _isDefaultAlign;

        private SvgElement _ownerElement;

        private SvgMeetOrSlice _meetOrSlice;
        private SvgPreserveAspectRatioType _alignment;

        #endregion

        #region Constructors

        public SvgPreserveAspectRatio(string attr, SvgElement ownerElement)
        {
            _ownerElement = ownerElement;

            Match match = parCheck.Match(attr.Trim());
            if (match.Groups["align"].Success)
            {
                switch (match.Groups["align"].Value)
                {
                    case CssConstants.ValNone:
                        _alignment = SvgPreserveAspectRatioType.None;
                        break;
                    case "xMinYMin":
                        _alignment = SvgPreserveAspectRatioType.XMinYMin;
                        break;
                    case "xMidYMin":
                        _alignment = SvgPreserveAspectRatioType.XMidYMin;
                        break;
                    case "xMaxYMin":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMin;
                        break;
                    case "xMinYMid":
                        _alignment = SvgPreserveAspectRatioType.XMinYMid;
                        break;
                    case "xMaxYMid":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMid;
                        break;
                    case "xMinYMax":
                        _alignment = SvgPreserveAspectRatioType.XMinYMax;
                        break;
                    case "xMidYMax":
                        _alignment = SvgPreserveAspectRatioType.XMidYMax;
                        break;
                    case "xMaxYMax":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMax;
                        break;
                    default:
                        _alignment = SvgPreserveAspectRatioType.XMidYMid;
                        _isDefaultAlign = true;
                        break;
                }
            }
            else
            {
                _alignment = SvgPreserveAspectRatioType.XMidYMid;
                _isDefaultAlign = true;
            }

            if (match.Groups["meet"].Success)
            {
                _isDefaultAlign = false;
                switch (match.Groups["meet"].Value)
                {
                    case "slice":
                        _meetOrSlice = SvgMeetOrSlice.Slice;
                        break;
                    case "meet":
                        _meetOrSlice = SvgMeetOrSlice.Meet;
                        break;
                    case "":
                        _meetOrSlice = SvgMeetOrSlice.Meet;
                        break;
                    default:
                        _meetOrSlice = SvgMeetOrSlice.Unknown;
                        break;
                }
            }
            else
            {
                _meetOrSlice = SvgMeetOrSlice.Meet;
            }
        }
        #endregion

        #region Public Properties

        public bool IsDefaultAlign
        {
            get {
                return _isDefaultAlign;
            }
        }

        #endregion

        #region Public Methods

        public double[] FitToViewBox(SvgRect viewBox, SvgRect rectToFit)
        {
            var localName = _ownerElement.LocalName;
            if (string.Equals(localName, "svg", StringComparison.Ordinal)) // (_ownerElement is SvgSvgElement)
            {
                ISvgMatrix mat = ((SvgSvgElement)_ownerElement).ViewBoxTransform;
                return new double[] { mat.E, mat.F, mat.A, mat.D };
            }
            if (string.Equals(localName, "pattern", StringComparison.Ordinal)) // (_ownerElement is SvgPatternElement)
            {
                ISvgMatrix mat = ((SvgPatternElement)_ownerElement).ViewBoxTransform;
                return new double[] { mat.E, mat.F, mat.A, mat.D };
            }

            double translateX = 0;
            double translateY = 0;
            double scaleX = 1;
            double scaleY = 1;

            if (!viewBox.IsEmpty && !rectToFit.IsEmpty)
            {
                // calculate scale values for non-uniform scaling
                scaleX = rectToFit.Width / viewBox.Width;
                scaleY = rectToFit.Height / viewBox.Height;

                if (_alignment != SvgPreserveAspectRatioType.None)
                {
                    // uniform scaling
                    if (_meetOrSlice == SvgMeetOrSlice.Meet)
                        scaleX = Math.Min(scaleX, scaleY);
                    else
                        scaleX = Math.Max(scaleX, scaleY);

                    scaleY = scaleX;

                    if (_alignment == SvgPreserveAspectRatioType.XMidYMax ||
                      _alignment == SvgPreserveAspectRatioType.XMidYMid ||
                      _alignment == SvgPreserveAspectRatioType.XMidYMin)
                    {
                        // align to the Middle X
                        translateX = (rectToFit.X + rectToFit.Width / 2) - scaleX * (viewBox.X + viewBox.Width / 2);
                    }
                    else if (_alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                      _alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                      _alignment == SvgPreserveAspectRatioType.XMaxYMin)
                    {
                        // align to the right X
                        translateX = (rectToFit.Width - viewBox.Width * scaleX);
                    }

                    if (_alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                      _alignment == SvgPreserveAspectRatioType.XMidYMid ||
                      _alignment == SvgPreserveAspectRatioType.XMinYMid)
                    {
                        // align to the Middle Y
                        translateY = (rectToFit.Y + rectToFit.Height / 2) - scaleY * (viewBox.Y + viewBox.Height / 2);
                    }
                    else if (_alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                      _alignment == SvgPreserveAspectRatioType.XMidYMax ||
                      _alignment == SvgPreserveAspectRatioType.XMinYMax)
                    {
                        // align to the bottom Y
                        translateY = (rectToFit.Height - viewBox.Height * scaleY);
                    }
                }
                else
                {
                    translateX = -viewBox.X * scaleX;
                    translateY = -viewBox.Y * scaleY;
                }
            }

            if (!SvgNumber.IsValid(translateX))
            {
                translateX = 0;
            }
            if (!SvgNumber.IsValid(translateY))
            {
                translateY = 0;
            }
            if (!SvgNumber.IsValid(scaleX))
            {
                scaleX = 1;
            }
            if (!SvgNumber.IsValid(scaleY))
            {
                scaleY = 1;
            }

            return new double[] { translateX, translateY, scaleX, scaleY };
        }

        #endregion

        #region ISvgPreserveAspectRatio Members

        public SvgPreserveAspectRatioType Align
        {
            get {
                return _alignment;
            }
            set {
                throw new NotImplementedException();
            }
        }

        public SvgMeetOrSlice MeetOrSlice
        {
            get {
                return _meetOrSlice;
            }
            set {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
