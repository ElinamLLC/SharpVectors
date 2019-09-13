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
    /// This class represents a reader which normalizes the line break: <c>\n</c>, <c>\r</c>, <c>\r\n</c> are replaced 
    /// by <c>\n</c>. The methods of this reader are not synchronized. The input is buffered.
    /// </summary>
    public abstract class SvgTextReader
    {
        protected SvgTextReader()
        {
        }

        public abstract int Read();

        /// <summary>
        /// Read characters into a portion of an array. </summary>
        /// <param name="cbuf">  Destination buffer </param>
        /// <param name="off">   Offset at which to start writing characters </param>
        /// <param name="len">   Maximum number of characters to read </param>
        /// <returns> The number of characters read, or -1 if the end of the
        /// stream has been reached </returns>
        public virtual int Read(char[] cbuf, int off, int len)
        {
            if (len == 0)
            {
                return 0;
            }

            int c = this.Read();
            if (c == -1)
            {
                return -1;
            }
            int result = 0;
            do
            {
                cbuf[result + off] = (char)c;
                result++;
                c = this.Read();
            } while (c != -1 && result < len);
            return result;
        }

        /// <summary>
        /// Gets the current line in the stream.
        /// </summary>
        public abstract int Line { get; }

        /// <summary>
        /// Gets the current column in the stream.
        /// </summary>
        public abstract int Column { get; }
    }
}