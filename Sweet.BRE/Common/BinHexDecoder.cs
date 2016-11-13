/*
The MIT License(MIT)
=====================

Copyright(c) 2008, Cagatay Dogan

Permission is hereby granted, free of charge, to any person obtaining a cop
of this software and associated documentation files (the "Software"), to deal  
in the Software without restriction, including without limitation the right
to use, copy, modify, merge, publish, distribute, sublicense, and/or sel
copies of the Software, and to permit persons to whom the Software is  
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in  
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS O
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,  
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL TH
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHE
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS I
THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sweet.BRE
{
    public class BinHexDecoder
    {
        protected BinHexDecoder( )
        {
        }

        private static int HextToBase10( char c )
        {
            int num = 0;
            if ((c >= 'a') && (c <= 'f'))
            {
                num = (int)((c - 'a') + 10);
            }
            else if ((c >= 'A') && (c <= 'F'))
            {
                num = (int)((c - 'A') + 10);
            }
            else if ((c >= '0') && (c <= '9'))
            {
                num = (int)(c - '0');
            }
            else
            {
                throw new Exception(CommonResStrings.GetString("InvalidCharacter"));
            }

            return num;
        }

        public static byte[] Decode( char[] chars )
        {
            if (chars == null)
            {
                throw new ArgumentNullException( "chars" );
            }

            int length = chars.Length;
            if (length == 0)
            {
                return new byte[0];
            }

            if (length % 2 != 0)
            {
                throw new Exception(CommonResStrings.GetString("ArrayLengthMustBeEven"));
            }

            int c1;
            int c2;
            int index = 0;

            List<byte> list = new List<byte>( );
            while (index < length)
            {
                c1 = HextToBase10(chars[index]);
                c2 = HextToBase10(chars[index + 1]);

                index = index + 2;
                list.Add((byte)(16*c1 + c2));
            }

            return list.ToArray( );
        }
    }
}

