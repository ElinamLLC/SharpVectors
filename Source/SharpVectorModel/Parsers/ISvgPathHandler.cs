//---------------------------------------------------------------------------------
// Ported from Apache Batik - Author: Stephane Hillion
//
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements.  See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License.  You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This interface must be implemented and then registred as the handler of a <c>PathParser</c> 
    /// instance in order to be notified of parsing events.
    /// </summary>
    public interface ISvgPathHandler
    {
        /// <summary>
        /// Invoked when the path starts. </summary>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void StartPath();

        /// <summary>
        /// Invoked when the path ends. </summary>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void EndPath();

        /// <summary>
        /// Invoked when a relative moveto command has been parsed.
        /// <para>Command : <b>m</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void MovetoRel(float x, float y);

        /// <summary>
        /// Invoked when an absolute moveto command has been parsed.
        /// <para>Command : <b>M</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void MovetoAbs(float x, float y);

        /// <summary>
        /// Invoked when a closepath has been parsed.
        /// <para>Command : <b>z</b> | <b>Z</b>
        /// </para>
        /// </summary>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void ClosePath();

        /// <summary>
        /// Invoked when a relative line command has been parsed.
        /// <para>Command : <b>l</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the relative x coordinates for the end point </param>
        /// <param name="y"> the relative y coordinates for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoRel(float x, float y);

        /// <summary>
        /// Invoked when an absolute line command has been parsed.
        /// <para>Command : <b>L</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoAbs(float x, float y);

        /// <summary>
        /// Invoked when an horizontal relative line command has been parsed.
        /// <para>Command : <b>h</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the relative X coordinate of the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoHorizontalRel(float x);

        /// <summary>
        /// Invoked when an horizontal absolute line command has been parsed.
        /// <para>Command : <b>H</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the absolute X coordinate of the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoHorizontalAbs(float x);

        /// <summary>
        /// Invoked when a vertical relative line command has been parsed.
        /// <para>Command : <b>v</b>
        /// </para>
        /// </summary>
        /// <param name="y"> the relative Y coordinate of the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoVerticalRel(float y);

        /// <summary>
        /// Invoked when a vertical absolute line command has been parsed.
        /// <para>Command : <b>V</b>
        /// </para>
        /// </summary>
        /// <param name="y"> the absolute Y coordinate of the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void LinetoVerticalAbs(float y);

        /// <summary>
        /// Invoked when a relative cubic bezier curve command has been parsed.
        /// <para>Command : <b>c</b>
        /// </para>
        /// </summary>
        /// <param name="x1"> the relative x coordinate for the first control point </param>
        /// <param name="y1"> the relative y coordinate for the first control point </param>
        /// <param name="x2"> the relative x coordinate for the second control point </param>
        /// <param name="y2"> the relative y coordinate for the second control point </param>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoCubicRel(float x1, float y1, float x2, float y2, float x, float y);

        /// <summary>
        /// Invoked when an absolute cubic bezier curve command has been parsed.
        /// <para>Command : <b>C</b>
        /// </para>
        /// </summary>
        /// <param name="x1"> the absolute x coordinate for the first control point </param>
        /// <param name="y1"> the absolute y coordinate for the first control point </param>
        /// <param name="x2"> the absolute x coordinate for the second control point </param>
        /// <param name="y2"> the absolute y coordinate for the second control point </param>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoCubicAbs(float x1, float y1, float x2, float y2, float x, float y);

        /// <summary>
        /// Invoked when a relative smooth cubic bezier curve command has
        /// been parsed. The first control point is assumed to be the
        /// reflection of the second control point on the previous command
        /// relative to the current point.
        /// <para>Command : <b>s</b>
        /// </para>
        /// </summary>
        /// <param name="x2"> the relative x coordinate for the second control point </param>
        /// <param name="y2"> the relative y coordinate for the second control point </param>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoCubicSmoothRel(float x2, float y2, float x, float y);

        /// <summary>
        /// Invoked when an absolute smooth cubic bezier curve command has
        /// been parsed. The first control point is assumed to be the
        /// reflection of the second control point on the previous command
        /// relative to the current point.
        /// <para>Command : <b>S</b>
        /// </para>
        /// </summary>
        /// <param name="x2"> the absolute x coordinate for the second control point </param>
        /// <param name="y2"> the absolute y coordinate for the second control point </param>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoCubicSmoothAbs(float x2, float y2, float x, float y);

        /// <summary>
        /// Invoked when a relative quadratic bezier curve command has been parsed.
        /// <para>Command : <b>q</b>
        /// </para>
        /// </summary>
        /// <param name="x1"> the relative x coordinate for the control point </param>
        /// <param name="y1"> the relative y coordinate for the control point </param>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative x coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoQuadraticRel(float x1, float y1, float x, float y);

        /// <summary>
        /// Invoked when an absolute quadratic bezier curve command has been parsed.
        /// <para>Command : <b>Q</b>
        /// </para>
        /// </summary>
        /// <param name="x1"> the absolute x coordinate for the control point </param>
        /// <param name="y1"> the absolute y coordinate for the control point </param>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute x coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoQuadraticAbs(float x1, float y1, float x, float y);

        /// <summary>
        /// Invoked when a relative smooth quadratic bezier curve command
        /// has been parsed. The control point is assumed to be the
        /// reflection of the control point on the previous command
        /// relative to the current point.
        /// <para>Command : <b>t</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoQuadraticSmoothRel(float x, float y);

        /// <summary>
        /// Invoked when an absolute smooth quadratic bezier curve command
        /// has been parsed. The control point is assumed to be the
        /// reflection of the control point on the previous command
        /// relative to the current point.
        /// <para>Command : <b>T</b>
        /// </para>
        /// </summary>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void CurvetoQuadraticSmoothAbs(float x, float y);

        /// <summary>
        /// Invoked when a relative elliptical arc command has been parsed. 
        /// <para>Command : <b>a</b>
        /// </para>
        /// </summary>
        /// <param name="rx"> the X axis radius for the ellipse </param>
        /// <param name="ry"> the Y axis radius for the ellipse </param>
        /// <param name="xAxisRotation"> the rotation angle in degrees for the ellipse's
        ///                      X-axis relative to the X-axis </param>
        /// <param name="largeArcFlag"> the value of the large-arc-flag </param>
        /// <param name="sweepFlag"> the value of the sweep-flag </param>
        /// <param name="x"> the relative x coordinate for the end point </param>
        /// <param name="y"> the relative y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void ArcRel(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, float x, float y);

        /// <summary>
        /// Invoked when an absolute elliptical arc command has been parsed.
        /// <para>Command : <b>A</b>
        /// </para>
        /// </summary>
        /// <param name="rx"> the X axis radius for the ellipse </param>
        /// <param name="ry"> the Y axis radius for the ellipse </param>
        /// <param name="xAxisRotation"> the rotation angle in degrees for the ellipse's
        ///                      X-axis relative to the X-axis </param>
        /// <param name="largeArcFlag"> the value of the large-arc-flag </param>
        /// <param name="sweepFlag"> the value of the sweep-flag </param>
        /// <param name="x"> the absolute x coordinate for the end point </param>
        /// <param name="y"> the absolute y coordinate for the end point </param>
        /// <exception cref="FormatException"> if an error occured while processing the path </exception>
        void ArcAbs(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, float x, float y);
    }
}