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
    /// This class implements an event-based parser for the SVG points
    /// attribute values (used with polyline and polygon elements).
    /// </summary>
    public class SvgPointsParser : SvgNumberParser
    {
        /// <summary>
        /// The points handler used to report parse events.
        /// </summary>
        protected ISvgPointsHandler _pointsHandler;

        /// <summary>
        /// Whether the last character was a 'e' or 'E'.
        /// </summary>
        protected bool _isERead;

        /// <summary>
        /// Creates a new PointsParser.
        /// </summary>
        public SvgPointsParser(ISvgPointsHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "The handler cannot be null or Nothing.");
            }

            _pointsHandler = handler;
        }

        /// <summary>
        /// Gets or sets an application <see cref="ISvgPointsHandler"/> implementation to register a points handler.
        /// </summary>
        /// <value>The transform list handler.</value>
        /// <remarks>
        /// <para>
        /// If the application does not register a handler, all events reported by the parser will be silently ignored.
        /// </para>
        /// <para>Applications may register a new or different handler in the middle of a parse, and the parser must 
        /// begin using the new handler immediately.</para> 
        /// </remarks>
        public virtual ISvgPointsHandler Handler
        {
            get {
                return _pointsHandler;
            }
            set {
                if (value != null)
                {
                    _pointsHandler = value;
                }
            }
        }

        /// <summary>
        /// Parses the current stream.
        /// </summary>
        protected override void DoParse()
        {
            _pointsHandler.StartPoints();

            _current = _reader.Read();
            this.SkipSpaces();

            for (;;)
            {
                if (_current == -1)
                {
                    break;
                }
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pointsHandler.Point(x, y);
                SkipCommaSpaces();
            }

            _pointsHandler.EndPoints();
        }
    }
}