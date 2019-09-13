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
using System.IO;
using System.Diagnostics;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This class is the superclass of all parsers. It provides localization and error handling methods.
    /// </summary>
    public abstract class SvgParser
    {
        private bool _isSuccessful;

        /// <summary>
        /// The normalizing reader.
        /// </summary>
        protected SvgTextReader _reader;

        /// <summary>
        /// The current character.
        /// </summary>
        protected int _current;

        protected SvgParser()
        {
        }

        /// <summary>
        /// Returns the current character value.
        /// </summary>
        public virtual int Current
        {
            get {
                return _current;
            }
        }

        /// <summary>
        /// </summary>
        public virtual string FormatMessage(string key, object[] args)
        {
            return string.Empty;
        }

        /// <summary>
        /// Parses the given string.
        /// </summary>
        public virtual bool Parse(string input)
        {
            _isSuccessful = true;
            try
            {
                _reader = new SvgStringReader(input);
                this.DoParse();
            }
            catch (IOException ex)
            {
                _isSuccessful = false;
                Trace.TraceError(ex.GetType().Name + ": " + ex.Message);
            }

            return _isSuccessful;
        }

        /// <summary>
        /// Method responsible for actually parsing data after AbstractParser
        /// has initialized itself.
        /// </summary>
        protected abstract void DoParse();

        /// <summary>
        /// Signals an error to the error handler. </summary>
        /// <param name="key"> The message key in the resource bundle. </param>
        /// <param name="args"> The message arguments. </param>
        protected virtual void ReportError(string key, object[] args)
        {
            _isSuccessful = false;

            Console.WriteLine("--------------ReportError-------------");
            Console.WriteLine(key);
            foreach (var arg in args)
            {
                if (arg is char)
                {
                    Console.WriteLine(Convert.ToChar(arg));
                }
                else if (arg is int)
                {
                    Console.WriteLine(Convert.ToChar(arg));
                }
                else
                {
                    Console.WriteLine(arg);
                }
            }
            Console.WriteLine("--------------ReportError-------------");
        }

        /// <summary>
        /// simple api to call often reported error.
        /// Just a wrapper for reportError().
        /// </summary>
        /// <param name="expectedChar"> what caller expected </param>
        /// <param name="currentChar"> what caller found </param>
        protected virtual void ReportCharacterExpectedError(char expectedChar, int currentChar)
        {
            ReportError("character.expected", new object[] { expectedChar, currentChar });
        }

        /// <summary>
        /// simple api to call often reported error.
        /// Just a wrapper for reportError().
        /// </summary>
        /// <param name="currentChar"> what the caller found and didnt expect </param>
        protected virtual void ReportUnexpectedCharacterError(int currentChar)
        {
            ReportError("character.unexpected", new object[] { currentChar });
        }

        /// <summary>
        /// Returns a localized error message. </summary>
        /// <param name="key"> The message key in the resource bundle. </param>
        /// <param name="args"> The message arguments. </param>
        protected virtual string CreateErrorMessage(string key, object[] args)
        {
            try
            {
                return FormatMessage(key, args);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.GetType().Name + ": " + ex.Message);
                return key;
            }
        }

        /// <summary>
        /// Skips the whitespaces in the current reader.
        /// </summary>
        protected virtual void SkipSpaces()
        {
            for (;;)
            {
                switch (_current)
                {
                    default:
                        return;
                    case 0x20:
                    case 0x09:
                    case 0x0D:
                    case 0x0A:
                        break;
                }
                _current = _reader.Read();
            }
        }

        /// <summary>
        /// Skips the whitespaces and an optional comma.
        /// </summary>
        protected virtual void SkipCommaSpaces()
        {
            bool wspBreak = false;
            for (;;)
            {
                switch (_current)
                {
                    default:
                        wspBreak = true;
                        break;
                        //goto wspBreak;
                    case 0x20:
                    case 0x9:
                    case 0xD:
                    case 0xA:
                        break;
                }
                if (wspBreak)
                {
                    break; // break the for-loop;
                }
                _current = _reader.Read();
            }
//            wsp1Break:

            wspBreak = false;

            if (_current == ',')
            {
                for (;;)
                {
                    switch (_current = _reader.Read())
                    {
                        default:
                            wspBreak = true;
                            break;
                            //goto wsp2Break;
                        case 0x20:
                        case 0x9:
                        case 0xD:
                        case 0xA:
                            break;
                    }
                    if (wspBreak)
                    {
                        break; // break the for-loop
                    }
                }
//                wsp2Break:;
            }
        }
    }
}