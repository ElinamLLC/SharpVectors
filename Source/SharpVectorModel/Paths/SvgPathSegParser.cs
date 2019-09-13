using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegParser
    {
        private static readonly Regex RegexPathCmd    = new Regex(@"(?=[A-DF-Za-df-z])");

        private bool _isClosed;
        private bool _mayHaveCurves;

        public SvgPathSegParser()
        {
            _isClosed      = false;
            _mayHaveCurves = false;
        }

        public bool IsClosed
        {
            get {
                return _isClosed;
            }
        }

        public bool MayHaveCurves
        {
            get {
                return _mayHaveCurves;
            }
        }

        public bool Parse(SvgPathSegList pathList, string pathSegs)
        {
            try
            {
                if (pathList == null || string.IsNullOrWhiteSpace(pathSegs))
                {
                    return false;
                }

                _isClosed = false;
                _mayHaveCurves = false;

                string[] paths = RegexPathCmd.Split(pathSegs);

                return this.Parse(pathList, paths);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.GetType().Name + ": " + ex.Message);
                return false;
            }
        }

        private bool Parse(SvgPathSegList pathList, string[] paths)
        {
            int closedPath = 0;

            SvgPathSeg seg;

            SvgPointF startPoint = new SvgPointF(0, 0);

            foreach (string path in paths)
            {
                string segment = path.Trim();
                if (segment.Length == 0)
                {
                    continue;
                }
                char cmd = segment[0];
                double[] coords = SvgNumber.ParseDoubles(segment.Substring(1));

                int length = coords.Length;
                switch (cmd)
                {
                    // Parse: moveto
                    case 'M':
                        for (int i = 0; i < length; i += 2)
                        {
                            if (length < 2)
                            {
                                return false;
                            }
                            if (i == 0)
                            {
                                seg = new SvgPathSegMovetoAbs(coords[i], coords[i + 1]);

                                startPoint = new SvgPointF(coords[i], coords[i + 1]);
                                SvgPointF endPoint = new SvgPointF(coords[i], coords[i + 1]);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            else
                            {
                                seg = new SvgPathSegLinetoAbs(coords[i], coords[i + 1]);

                                SvgPointF endPoint = new SvgPointF(coords[i], coords[i + 1]);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            pathList.AppendItem(seg);
                        }
                        break;
                    case 'm':
                        for (int i = 0; i < length; i += 2)
                        {
                            if (length < 2)
                            {
                                return false;
                            }
                            if (i == 0)
                            {
                                seg = new SvgPathSegMovetoRel(coords[i], coords[i + 1]);

                                SvgPointF endPoint = new SvgPointF(coords[i] + startPoint.X, coords[i + 1] + startPoint.Y);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            else
                            {
                                seg = new SvgPathSegLinetoRel(coords[i], coords[i + 1]);

                                SvgPointF endPoint = new SvgPointF(coords[i] + startPoint.X, coords[i + 1] + startPoint.Y);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            pathList.AppendItem(seg);
                        }
                        break;
                    // End of: moveto

                    // Parse: lineto
                    case 'L':
                        for (int i = 0; i < length; i += 2)
                        {
                            seg = new SvgPathSegLinetoAbs(coords[i], coords[i + 1]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(coords[i], coords[i + 1]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'l':
                        for (int i = 0; i < length; i += 2)
                        {
                            seg = new SvgPathSegLinetoRel(coords[i], coords[i + 1]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(coords[i] + startPoint.X, coords[i + 1] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'H':
                        for (int i = 0; i < length; i++)
                        {
                            seg = new SvgPathSegLinetoHorizontalAbs(coords[i]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(coords[i], startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'h':
                        for (int i = 0; i < length; i++)
                        {
                            seg = new SvgPathSegLinetoHorizontalRel(coords[i]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(coords[i] + startPoint.X, startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'V':
                        for (int i = 0; i < length; i++)
                        {
                            seg = new SvgPathSegLinetoVerticalAbs(coords[i]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(startPoint.X, coords[i]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'v':
                        for (int i = 0; i < length; i++)
                        {
                            seg = new SvgPathSegLinetoVerticalRel(coords[i]);
                            pathList.AppendItem(seg);

                            SvgPointF endPoint = new SvgPointF(startPoint.X, coords[i] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    // End of: lineto

                    // Parse: beziers
                    case 'C':
                        for (int i = 0; i < length; i += 6)
                        {
                            seg = new SvgPathSegCurvetoCubicAbs(
                                coords[i + 4],
                                coords[i + 5],
                                coords[i],
                                coords[i + 1],
                                coords[i + 2],
                                coords[i + 3]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 4], coords[i + 5]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'c':
                        for (int i = 0; i < length; i += 6)
                        {
                            if ((i + 5) >= length)
                            {
                                break;
                            }
                            seg = new SvgPathSegCurvetoCubicRel(
                                coords[i + 4],
                                coords[i + 5],
                                coords[i],
                                coords[i + 1],
                                coords[i + 2],
                                coords[i + 3]);

                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 4] + startPoint.X, coords[i + 5] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'S':
                        for (int i = 0; i < length; i += 4)
                        {
                            seg = new SvgPathSegCurvetoCubicSmoothAbs(
                                coords[i + 2],
                                coords[i + 3],
                                coords[i],
                                coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 2], coords[i + 3]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 's':
                        for (int i = 0; i < length; i += 4)
                        {
                            seg = new SvgPathSegCurvetoCubicSmoothRel(
                                coords[i + 2],
                                coords[i + 3],
                                coords[i],
                                coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 2] + startPoint.X, coords[i + 3] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'Q':
                        for (int i = 0; i < length; i += 4)
                        {
                            seg = new SvgPathSegCurvetoQuadraticAbs(
                                coords[i + 2],
                                coords[i + 3],
                                coords[i],
                                coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 2], coords[i + 3]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'q':
                        for (int i = 0; i < length; i += 4)
                        {
                            seg = new SvgPathSegCurvetoQuadraticRel(
                                coords[i + 2],
                                coords[i + 3],
                                coords[i],
                                coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i + 2] + startPoint.X, coords[i + 3] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 'T':
                        for (int i = 0; i < length; i += 2)
                        {
                            seg = new SvgPathSegCurvetoQuadraticSmoothAbs(
                                coords[i], coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i], coords[i + 1]);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    case 't':
                        for (int i = 0; i < length; i += 2)
                        {
                            seg = new SvgPathSegCurvetoQuadraticSmoothRel(
                                coords[i], coords[i + 1]);
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;

                            SvgPointF endPoint = new SvgPointF(coords[i] + startPoint.X, coords[i + 1] + startPoint.Y);
                            seg.Limits = new SvgPointF[] { startPoint, endPoint };
                            startPoint = endPoint;
                        }
                        break;
                    // End of: bezier

                    // Parse: arcs
                    case 'A':
                    case 'a':
                        for (int i = 0; i < length; i += 7)
                        {
                            if (cmd == 'A')
                            {
                                seg = new SvgPathSegArcAbs(
                                    coords[i + 5],
                                    coords[i + 6],
                                    coords[i],
                                    coords[i + 1],
                                    coords[i + 2],
                                    (!coords[i + 3].Equals(0)),
                                    (!coords[i + 4].Equals(0)));

                                SvgPointF endPoint = new SvgPointF(coords[i + 5], coords[i + 6]);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            else
                            {
                                seg = new SvgPathSegArcRel(
                                    coords[i + 5],
                                    coords[i + 6],
                                    coords[i],
                                    coords[i + 1],
                                    coords[i + 2],
                                    (!coords[i + 3].Equals(0)),
                                    (!coords[i + 4].Equals(0)));

                                SvgPointF endPoint = new SvgPointF(coords[i + 5] + startPoint.X, coords[i + 6] + startPoint.Y);
                                seg.Limits = new SvgPointF[] { startPoint, endPoint };
                                startPoint = endPoint;
                            }
                            pathList.AppendItem(seg);

                            _mayHaveCurves = true;
                        }
                        break;
                    // End of: arcs

                    // Parse: close
                    case 'z':
                    case 'Z':
                        closedPath++;
                        seg = new SvgPathSegClosePath();
                        pathList.AppendItem(seg);

                        if (pathList.Count >= 2)
                        {
                            SvgPointF endPoint = pathList[0].Limits[0];
                            seg.Limits = new SvgPointF[] { endPoint, startPoint };
                            startPoint = endPoint;
                        }
                        else
                        {
                            seg.Limits = new SvgPointF[] { startPoint, startPoint };
                            startPoint = new SvgPointF(0, 0);
                        }
                        break;
                    // End of: close

                    // Unknown path command
                    default:
                        throw new ApplicationException(string.Format("Unknown path command - ({0})", cmd));
                }
            }

            _isClosed = (closedPath == 1);

            return true;
        }
    }
}
