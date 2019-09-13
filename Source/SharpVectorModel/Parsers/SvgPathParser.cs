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
using System.Diagnostics;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This class implements an event-based parser for the SVG path's <c>d</c> attribute values.
    /// </summary>
    public class SvgPathParser : SvgNumberParser
    {
        /// <summary>
        /// The path handler used to report parse events.
        /// </summary>
        protected ISvgPathHandler _pathHandler;

        /// <summary>
        /// Creates a new PathParser.
        /// </summary>
        public SvgPathParser(ISvgPathHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "The handler cannot be null or Nothing.");
            }

            _pathHandler = handler;
        }

        /// <summary>
        /// Allows an application to register a path handler.
        /// <para>
        /// If the application does not register a handler, all events reported by the parser will be silently ignored.
        /// </para>
        /// <para>
        /// Applications may register a new or different handler in the middle of a parse, and the parser must begin 
        /// using the new handler immediately.
        /// </para> 
        /// </summary>
        /// <value> The transform list handler. </value>
        public virtual ISvgPathHandler Handler
        {
            get {                
                return _pathHandler;
            }
            set {
                if (value != null)
                {
                    _pathHandler = value;
                }
            }
        }

        protected override void DoParse()
        {
            _pathHandler.StartPath();

            _current = _reader.Read();
            for (;;)
            {
                bool loopBreak = false;
                try
                {
                    switch (_current)
                    {
                        case 0xD:
                        case 0xA:
                        case 0x20:
                        case 0x9:
                            _current = _reader.Read();
                            break;
                        case 'z':
                        case 'Z':
                            _current = _reader.Read();
                            _pathHandler.ClosePath();
                            break;
                        case 'm':
                            Parsem();
                            break;
                        case 'M':
                            ParseM();
                            break;
                        case 'l':
                            Parsel();
                            break;
                        case 'L':
                            ParseL();
                            break;
                        case 'h':
                            Parseh();
                            break;
                        case 'H':
                            ParseH();
                            break;
                        case 'v':
                            Parsev();
                            break;
                        case 'V':
                            ParseV();
                            break;
                        case 'c':
                            Parsec();
                            break;
                        case 'C':
                            ParseC();
                            break;
                        case 'q':
                            Parseq();
                            break;
                        case 'Q':
                            ParseQ();
                            break;
                        case 's':
                            Parses();
                            break;
                        case 'S':
                            ParseS();
                            break;
                        case 't':
                            Parset();
                            break;
                        case 'T':
                            ParseT();
                            break;
                        case 'a':
                            Parsea();
                            break;
                        case 'A':
                            ParseA();
                            break;
                        case -1:
                            loopBreak = true;
                            break;
                            //goto loopBreak;
                        default:
                            ReportUnexpected(_current);
                            break;
                    }
                }
                catch (FormatException ex)
                {
                    Trace.TraceError(ex.GetType().Name + ": " + ex.Message);

                    SkipSubPath();
                }
//                loopContinue:;
                if (loopBreak)
                {
                    break;
                }
            }
 //           loopBreak:

            SkipSpaces();
            if (_current != -1)
            {
                ReportError("end.of.stream.expected", new object[] { _current });
            }

            _pathHandler.EndPath();
        }

        /// <summary>
        /// Parses a 'm' command.
        /// </summary>
        protected internal virtual void Parsem()
        {
            _current = _reader.Read();
            SkipSpaces();

            float x = ParseFloat();
            SkipCommaSpaces();
            float y = ParseFloat();
            _pathHandler.MovetoRel(x, y);

            bool expectNumber = SkipCommaSpaces2();
            _parsel(expectNumber);
        }

        /// <summary>
        /// Parses a 'M' command.
        /// </summary>
        protected internal virtual void ParseM()
        {
            _current = _reader.Read();
            SkipSpaces();

            float x = ParseFloat();
            SkipCommaSpaces();
            float y = ParseFloat();
            _pathHandler.MovetoAbs(x, y);

            bool expectNumber = SkipCommaSpaces2();
            _parseL(expectNumber);
        }

        /// <summary>
        /// Parses a 'l' command.
        /// </summary>
        protected internal virtual void Parsel()
        {
            _current = _reader.Read();
            SkipSpaces();
            _parsel(true);
        }

        protected internal virtual void _parsel(bool expectNumber)
        {
            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;
                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.LinetoRel(x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'L' command.
        /// </summary>
        protected internal virtual void ParseL()
        {
            _current = _reader.Read();
            SkipSpaces();
            _parseL(true);
        }

        protected internal virtual void _parseL(bool expectNumber)
        {
            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;
                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.LinetoAbs(x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'h' command.
        /// </summary>
        protected internal virtual void Parseh()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;
                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();
                _pathHandler.LinetoHorizontalRel(x);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'H' command.
        /// </summary>
        protected internal virtual void ParseH()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();
                _pathHandler.LinetoHorizontalAbs(x);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'v' command.
        /// </summary>
        protected internal virtual void Parsev()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();

                _pathHandler.LinetoVerticalRel(x);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'V' command.
        /// </summary>
        protected internal virtual void ParseV()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }
                float x = ParseFloat();

                _pathHandler.LinetoVerticalAbs(x);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'c' command.
        /// </summary>
        protected internal virtual void Parsec()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x1 = ParseFloat();
                SkipCommaSpaces();
                float y1 = ParseFloat();
                SkipCommaSpaces();
                float x2 = ParseFloat();
                SkipCommaSpaces();
                float y2 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoCubicRel(x1, y1, x2, y2, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'C' command.
        /// </summary>
        protected internal virtual void ParseC()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x1 = ParseFloat();
                SkipCommaSpaces();
                float y1 = ParseFloat();
                SkipCommaSpaces();
                float x2 = ParseFloat();
                SkipCommaSpaces();
                float y2 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoCubicAbs(x1, y1, x2, y2, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'q' command.
        /// </summary>
        protected internal virtual void Parseq()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x1 = ParseFloat();
                SkipCommaSpaces();
                float y1 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoQuadraticRel(x1, y1, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'Q' command.
        /// </summary>
        protected internal virtual void ParseQ()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x1 = ParseFloat();
                SkipCommaSpaces();
                float y1 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoQuadraticAbs(x1, y1, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 's' command.
        /// </summary>
        protected internal virtual void Parses()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x2 = ParseFloat();
                SkipCommaSpaces();
                float y2 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoCubicSmoothRel(x2, y2, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'S' command.
        /// </summary>
        protected internal virtual void ParseS()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x2 = ParseFloat();
                SkipCommaSpaces();
                float y2 = ParseFloat();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoCubicSmoothAbs(x2, y2, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 't' command.
        /// </summary>
        protected internal virtual void Parset()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoQuadraticSmoothRel(x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'T' command.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void parseT() throws FormatException, java.io.IOException
        protected internal virtual void ParseT()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.CurvetoQuadraticSmoothAbs(x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'a' command.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void parsea() throws FormatException, java.io.IOException
        protected internal virtual void Parsea()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float rx = ParseFloat();
                SkipCommaSpaces();
                float ry = ParseFloat();
                SkipCommaSpaces();
                float ax = ParseFloat();
                SkipCommaSpaces();

                bool laf;
                switch (_current)
                {
                    default:
                        ReportUnexpected(_current);
                        return;
                    case '0':
                        laf = false;
                        break;
                    case '1':
                        laf = true;
                        break;
                }

                _current = _reader.Read();
                SkipCommaSpaces();

                bool sf;
                switch (_current)
                {
                    default:
                        ReportUnexpected(_current);
                        return;
                    case '0':
                        sf = false;
                        break;
                    case '1':
                        sf = true;
                        break;
                }

                _current = _reader.Read();
                SkipCommaSpaces();

                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.ArcRel(rx, ry, ax, laf, sf, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Parses a 'A' command.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void parseA() throws FormatException, java.io.IOException
        protected internal virtual void ParseA()
        {
            _current = _reader.Read();
            SkipSpaces();
            bool expectNumber = true;

            for (;;)
            {
                switch (_current)
                {
                    default:
                        if (expectNumber)
                        {
                            ReportUnexpected(_current);
                        }
                        return;

                    case '+':
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                }

                float rx = ParseFloat();
                SkipCommaSpaces();
                float ry = ParseFloat();
                SkipCommaSpaces();
                float ax = ParseFloat();
                SkipCommaSpaces();

                bool laf;
                switch (_current)
                {
                    default:
                        ReportUnexpected(_current);
                        return;
                    case '0':
                        laf = false;
                        break;
                    case '1':
                        laf = true;
                        break;
                }

                _current = _reader.Read();
                SkipCommaSpaces();

                bool sf;
                switch (_current)
                {
                    default:
                        ReportUnexpected(_current);
                        return;
                    case '0':
                        sf = false;
                        break;
                    case '1':
                        sf = true;
                        break;
                }

                _current = _reader.Read();
                SkipCommaSpaces();
                float x = ParseFloat();
                SkipCommaSpaces();
                float y = ParseFloat();

                _pathHandler.ArcAbs(rx, ry, ax, laf, sf, x, y);
                expectNumber = SkipCommaSpaces2();
            }
        }

        /// <summary>
        /// Skips a sub-path.
        /// </summary>
        protected internal virtual void SkipSubPath()
        {
            for (;;)
            {
                switch (_current)
                {
                    case -1:
                    case 'm':
                    case 'M':
                        return;
                    default:
                        break;
                }
                _current = _reader.Read();
            }
        }

        protected internal virtual void ReportUnexpected(int ch)
        {
            ReportUnexpectedCharacterError(_current);
            SkipSubPath();
        }

        /// <summary>
        /// Skips the whitespaces and an optional comma. </summary>
        /// <returns> true if comma was skipped. </returns>
        protected internal virtual bool SkipCommaSpaces2()
        {
            bool wspBreak = false;
            for (;;)
            {
                switch (_current)
                {
                    default:
                        wspBreak = true;
                        break;
                    case 0x20:
                    case 0x9:
                    case 0xD:
                    case 0xA:
                        break;
                }
                if (wspBreak)
                {
                    break;
                }
                _current = _reader.Read();
//                wsp1Continue:;
            }
//            wsp1Break:

            if (_current != ',')
            {
                return false; // no comma.
            }

            wspBreak = false;

            for (;;)
            {
                switch (_current = _reader.Read())
                {
                    default:
                        wspBreak = true;
                        break;
                    case 0x20:
                    case 0x9:
                    case 0xD:
                    case 0xA:
                        break;
                }
                if (wspBreak)
                {
                    break;
                }
//                wsp2Continue:;
            }
//            wsp2Break:
            return true; // had comma
        }
    }

}