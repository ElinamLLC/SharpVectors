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
    /// This class represents a parser with support for numbers.
    /// </summary>
    public abstract class SvgNumberParser : SvgParser
    {
        /// <summary>
        /// Array of powers of ten. Using double instead of float gives a tiny bit more precision.
        /// </summary>
        private static readonly double[] _pow10 = new double[128];

        protected SvgNumberParser()
        {
        }

        static SvgNumberParser()
        {
            for (int i = 0; i < _pow10.Length; i++)
            {
                _pow10[i] = Math.Pow(10, i);
            }
        }

        /// <summary>
        /// Parses the content of the buffer and converts it to a float.
        /// </summary>
        protected virtual float ParseFloat()
        {
            int mant      = 0;
            int mantDig   = 0;
            bool mantPos  = true;
            bool mantRead = false;

            int exp       = 0;
            int expDig    = 0;
            int expAdj    = 0;
            bool expPos   = true;

            switch (_current)
            {
                case '-':
                    mantPos = false;
                    // fallthrough
                    goto case '+';
                case '+':
                    _current = _reader.Read();
                    break;
            }

            switch (_current)
            {
                default:
                    ReportUnexpectedCharacterError(_current);
                    return 0.0f;

                case '.':
                    break;

                case '0':
                    mantRead = true;
                    for (;;)
                    {
                        bool lBreak = false;
                        _current = _reader.Read();
                        switch (_current)
                        {
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                lBreak = true;
                                break;
                                //goto lBreak;
                            case '.':
                            case 'e':
                            case 'E':
                                goto m1Break;
                            default:
                                return 0.0f;
                            case '0':
                                break;
                        }
//                        lContinue:;
                        if (lBreak)
                        {
                            break; // break the for loop
                        }
                    }
//                    lBreak:

                    goto case '1';
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    mantRead = true;
                    for (;;)
                    {
                        bool lBreak = false;
                        if (mantDig < 9)
                        {
                            mantDig++;
                            mant = mant * 10 + (_current - '0');
                        }
                        else
                        {
                            expAdj++;
                        }
                        _current = _reader.Read();
                        switch (_current)
                        {
                            default:
                                lBreak = true;
                                break;
                                //goto lBreak2;
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
//                        lContinue:;
                        if (lBreak)
                        {
                            break; // break the for loop
                        }
                    }
//                    lBreak2:;
                    break;
            }
m1Break:

            if (_current == '.')
            {
                _current = _reader.Read();
                switch (_current)
                {
                    default:
                        goto case 'e';
                    case 'e':
                    case 'E':
                        if (!mantRead)
                        {
                            ReportUnexpectedCharacterError(_current);
                            return 0.0f;
                        }
                        break;

                    case '0':
                        if (mantDig == 0)
                        {
                            for (;;)
                            {
                                bool lBreak = false;
                                _current = _reader.Read();
                                expAdj--;
                                switch (_current)
                                {
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        lBreak = true;
                                        break;
                                        //goto lBreak3;
                                    default:
                                        if (!mantRead)
                                        {
                                            return 0.0f;
                                        }
                                        goto m2Break;
                                    case '0':
                                        break;
                                }
//                                lContinue:;
                                if (lBreak)
                                {
                                    break; // break the for loop
                                }
                            }
                            //lBreak3:;
                        }
                        goto case '1';
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        for (;;)
                        {
                            bool lBreak = false;
                            if (mantDig < 9)
                            {
                                mantDig++;
                                mant = mant * 10 + (_current - '0');
                                expAdj--;
                            }
                            _current = _reader.Read();
                            switch (_current)
                            {
                                default:
                                    lBreak = true;
                                    break;
                                    //goto lBreak;
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
//                            lContinue:;
                            if (lBreak)
                            {
                                break; // break the for loop
                            }
                        }
//                        lBreak:;
                        break;
                }
//m2Break:;
            }
m2Break: //TODO-check this well

            switch (_current)
            {
                case 'e':
                case 'E':
                    _current = _reader.Read();
                    switch (_current)
                    {
                        default:
                            ReportUnexpectedCharacterError(_current);
                            return 0f;
                        case '-':
                            expPos = false;
                            goto case '+';
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
                            break; //TODO-check this well!
                        case '+':
                            {
                                _current = _reader.Read();
                                switch (_current)
                                {
                                    default:
                                        ReportUnexpectedCharacterError(_current);
                                        return 0f;
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

                            }
                            break;
                    }

                    switch (_current)
                    {
                        case '0':
                            for (;;)
                            {
                                bool lBreak = false;
                                _current = _reader.Read();
                                switch (_current)
                                {
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        lBreak = true;
                                        break;
                                        //goto lBreak;
                                    default:
                                        goto enBreak;
                                    case '0':
                                        break;
                                }
//                                lContinue:;
                                if (lBreak)
                                {
                                    break; // break the for loop
                                }
                            }
//                            lBreak:

                            goto case '1';
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            for (;;)
                            {
                                bool lBreak = false;
                                if (expDig < 3)
                                {
                                    expDig++;
                                    exp = exp * 10 + (_current - '0');
                                }
                                _current = _reader.Read();
                                switch (_current)
                                {
                                    default:
                                        lBreak = true;
                                        break;
                                        //goto lBreak5;
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
//                                lContinue:;
                                if (lBreak)
                                {
                                    break;
                                }
                            }
//                            lBreak5:;
                            break;
                    }
enBreak:
                    goto default;
                default:
                    break;
            }

            if (!expPos)
            {
                exp = -exp;
            }
            exp += expAdj;
            if (!mantPos)
            {
                mant = -mant;
            }

            return BuildFloat(mant, exp);
        }

        /// <summary>
        /// Computes a float from mantissa and exponent.
        /// </summary>
        public static float BuildFloat(int mant, int exp)
        {
            if (exp < -125 || mant == 0)
            {
                return 0.0f;
            }

            if (exp >= 128)
            {
                return (mant > 0) ? float.PositiveInfinity : float.NegativeInfinity;
            }

            if (exp == 0)
            {
                return mant;
            }

            if (mant >= (1 << 26))
            {
                mant++; // round up trailing bits if they will be dropped.
            }

            return (float)((exp > 0) ? mant * _pow10[exp] : mant / _pow10[-exp]);
        }
    }
}