using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpVectors.Woffs
{
    /// <summary>
    /// A class defining the Triplet Encoding used for simple glyph encoding in Compact Table Format (CTF).
    /// </summary>
    /// <remarks>
    /// <para>A simple glyph defines all the contours and points that are used to create the glyph outline.</para>
    /// <para>Each point is presented in a {dx, dy, on/off-curve} triplet that is stored with the variable 
    /// length encoding consuming 2 to 5 bytes per triplet.</para>
    /// <para>
    /// This class represents the data formats of <c>flags[]</c>, <c>(xCoordinate, yCoordinates)[]</c> of the simple glyph data.
    /// </para>
    /// <para>The coordinate values of simple glyph outline points are calculated as follows:</para>
    /// <code>
    ///     Xcoord[i] = (short)((Xsign)*(xCoordinate[i] + DeltaX[index]);
    ///     Ycoord[i] = (short)((Ysign)*(xCoordinate[i] + DeltaY[index]);
    /// </code>
    /// <para>Here is some pseudo-code illustrating how to read in the data:</para>
    /// <code>
    /// <![CDATA[
    /// for i = 0 to (glyph.numberOfPoints - 1)
    /// {
    ///     bitflags = flags[i]
    ///     isOnCurvePoint = ((bitflags & 0x80) == 0)
    ///     index = (bitflags & 0x7F)
    ///     xIsNegative = coordEncoding[index].xIsNegative
    ///     yIsNegative = coordEncoding[index].yIsNegative;
    /// 
    ///     // subtract one from byteCount since one byte is for the flags
    ///     byteCount = coordEncoding[index].byteCount - 1
    /// 
    ///     data = 0
    ///     for j = 0 to (byteCount - 1)
    ///     {
    ///         data <<= 8
    ///         ultmp = glyfData.getNextUInt8()
    ///         data |= ultmp
    ///     }
    /// 
    ///     ultmp = data >> ((byteCount * 8) - coordEncoding[index].xBits)
    ///     ultmp &= ((1L << coordEncoding[index].xBits) - 1)
    ///     dx = ultmp
    /// 
    ///     ultmp = data >> ((byteCount * 8) - coordEncoding[index].xBits - coordEncoding[index].yBits)
    ///     ultmp &= ((1L << coordEncoding[index].yBits) - 1)
    ///     dy = ultmp
    /// 
    ///     dx += coordEncoding[index].deltaX
    ///     dy += coordEncoding[index].deltaY
    /// 
    ///     if (xIsNegative)
    ///         dx = -dx
    /// 
    ///     if (yIsNegative)
    ///         dy = -dy
    /// 
    ///     x = (x + dx)
    ///     y = (y + dy)
    /// }
    /// ]]>
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/WOFF2/#glyf_table_format">5.1. Transformed glyf table format</seealso>.
    /// <seealso href="https://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CTF">5.6. The 'glyf' Table Translation</seealso>.
    public static class WoffTriplet
    {
        public static bool Decode(WoffReader flags, WoffReader glyphs, uint nPoints, IList<WoffPoint> points)
        {
            int y = 0;
            int x = 0;

            for (uint i = 0; i < nPoints; i++)
            {
                int dx = 0, dy = 0;
                byte flag = flags.ReadByte();
                bool onCurve = (flag >> 7) == 0;
                flag &= 0x7f;

                if (flag < 10)
                {
                    dx = 0;
                    dy = WithSign(flag, ((flag & 14) << 7) + glyphs.ReadByte());

                }
                else if (flag < 20)
                {
                    dx = WithSign(flag, (((flag - 10) & 14) << 7) + glyphs.ReadByte());
                    dy = 0;

                }
                else if (flag < 84)
                {
                    int b0 = flag - 20;
                    int b1 = glyphs.ReadByte();
                    dx = WithSign(flag, 1 + (b0 & 0x30) + (b1 >> 4));
                    dy = WithSign(flag >> 1, 1 + ((b0 & 0x0c) << 2) + (b1 & 0x0f));

                }
                else if (flag < 120)
                {
                    int b0 = flag - 84;
                    dx = WithSign(flag, 1 + ((b0 / 12) << 8) + glyphs.ReadByte());
                    dy = WithSign(flag >> 1, 1 + (((b0 % 12) >> 2) << 8) + glyphs.ReadByte());

                }
                else if (flag < 124)
                {
                    int b1 = glyphs.ReadByte();
                    int b2 = glyphs.ReadByte();
                    dx = WithSign(flag, (b1 << 4) + (b2 >> 4));
                    dy = WithSign(flag >> 1, ((b2 & 0x0f) << 8) + glyphs.ReadByte());

                }
                else
                {
                    dx = WithSign(flag, glyphs.ReadUInt16());
                    dy = WithSign(flag >> 1, glyphs.ReadUInt16());
                }

                x += dx;
                y += dy;
                points.Add(new WoffPoint(x, y, onCurve));
            }

            return true;
        }

        private static int WithSign(int flag, int baseval)
        {
            // Precondition: 0 <= baseval < 65536 (to avoid integer overflow)
            return (flag & 1) != 0 ? baseval : -baseval;
        }
    }
}
