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

namespace Sweet.BRE
{
    public class BinHexEncoder
    {
        private const string SHexDigits = "0123456789ABCDEF";

        protected BinHexEncoder( )
        {
        }

        private static int Encode( byte[] inArray, int offsetIn, int count, char[] outArray )
        {
            int num = 0;
            int num2 = 0;
            
            int length = outArray.Length;
            
            for (int i = 0; i < count; i++)
            {
                byte num3 = inArray[offsetIn++];
                outArray[num++] = SHexDigits[num3 >> 4];
                
                if (num == length)
                {
                    break;
                }

                outArray[num++] = SHexDigits[num3 & 15];
                if (num == length)
                {
                    break;
                }
            }

            return (num - num2);
        }

        public static string Encode( byte[] inArray )
        {
            if (inArray == null)
            {
                throw new ArgumentNullException( "inArray" );
            }

            return Encode( inArray, 0, inArray.Length );
        }

        public static string Encode( byte[] inArray, int offsetIn, int count )
        {
            if (inArray == null)
            {
                throw new ArgumentNullException( "inArray" );
            }

            if (0 > offsetIn)
            {
                throw new ArgumentOutOfRangeException( "offsetIn" );
            }

            if (0 > count)
            {
                throw new ArgumentOutOfRangeException( "count" );
            }

            if (count > (inArray.Length - offsetIn))
            {
                throw new ArgumentOutOfRangeException( "count" );
            }

            char[] chArray = new char[2 * count];
            return new string( chArray, 0, Encode( inArray, offsetIn, count, chArray ) );
        }
    }
}

