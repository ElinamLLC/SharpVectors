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

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This class represents a <see cref="SvgTextReader"/> which handles strings.
    /// </summary>
    public class SvgStringReader : SvgTextReader
    {
        /// <summary>
        /// The characters.
        /// </summary>
        protected string _inputText;

        /// <summary>
        /// The length of the string.
        /// </summary>
        protected int _textLength;

        /// <summary>
        /// The index of the next character.
        /// </summary>
        protected int _nextIndex;

        /// <summary>
        /// The current line in the stream.
        /// </summary>
        protected int _line = 1;

        /// <summary>
        /// The current column in the stream.
        /// </summary>
        protected int _column;

        /// <summary>
        /// Creates a new StringNormalizingReader. </summary>
        /// <param name="input"> The string to read. </param>
        public SvgStringReader(string input)
        {
            _inputText  = input;
            _textLength = input.Length;
        }

        /// <summary>
        /// Read a single character.  This method will block until a character is available, an I/O error occurs, 
        /// or the end of the stream is reached.
        /// </summary>
        public override int Read()
        {
            int result = (_textLength == _nextIndex) ? -1 : _inputText[_nextIndex++];
            if (result <= 13)
            {
                switch (result)
                {
                    case 13:
                        _column = 0;
                        _line++;
                        int c = (_textLength == _nextIndex) ? -1 : _inputText[_nextIndex];
                        if (c == 10)
                        {
                            _nextIndex++;
                        }
                        return 10;

                    case 10:
                        _column = 0;
                        _line++;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the current line in the stream.
        /// </summary>
        public override int Line
        {
            get {
                return _line;
            }
        }

        /// <summary>
        /// Returns the current column in the stream.
        /// </summary>
        public override int Column
        {
            get {
                return _column;
            }
        }

        /// <summary>
        /// Close the stream.
        /// </summary>
        public virtual void Close()
        {
            _inputText = null;
        }
    }
}