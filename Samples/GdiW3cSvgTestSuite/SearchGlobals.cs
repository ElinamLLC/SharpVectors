using System;
using System.Drawing;
using System.Diagnostics;

namespace GdiW3cSvgTestSuite
{
    public static class SearchGlobals
    {
        public const string InfoTextNotFound = "The specified text is not found";
        public const string TargetNotFound   = "No occurrances found.";
        public const string TargetFound      = "Replaced {0} occurrances.";

        public const int PopupTimeOut   = 6000;

        public const int Offset         = 6;

        public const int WM_NCHITTEST   = 0x84;
        public const int HT_CLIENT      = 0x1;
        public const int HT_CAPTION     = 0x2;

        public const double FadeOpacity = 0.75;

        public static int InRange(this int x, int lo, int hi)
        {
            Debug.Assert(lo <= hi);
            return x < lo ? lo : (x > hi ? hi : x);
        }

        public static bool IsInRange(this int x, int lo, int hi)
        {
            return x >= lo && x <= hi;
        }

        public static Color HalfMix(this Color one, Color two)
        {
            return Color.FromArgb(
                (one.A + two.A) >> 1,
                (one.R + two.R) >> 1,
                (one.G + two.G) >> 1,
                (one.B + two.B) >> 1);
        }
    }
}
