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
    /// This interface must be implemented and then registred as the handler of a <c>PointsParser</c> 
    /// instance in order to be notified of parsing events.
    /// </summary>
    public interface ISvgPointsHandler
    {
        /// <summary>
        /// Invoked when the points attribute starts. </summary>
        /// <exception cref="FormatException"> if an error occured while processing the points </exception>
        void StartPoints();

        /// <summary>
        /// Invoked when a point has been parsed. </summary>
        /// <param name="x"> the x coordinate of the point </param>
        /// <param name="y"> the y coordinate of the point </param>
        /// <exception cref="FormatException"> if an error occured while processing the points </exception>
        void Point(float x, float y);

        /// <summary>
        /// Invoked when the points attribute ends. </summary>
        /// <exception cref="FormatException"> if an error occured while processing the points </exception>
        void EndPoints();
    }
}