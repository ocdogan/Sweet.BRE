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
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public abstract class ArithmeticStm : NumericBinaryStm
    {
        protected ArithmeticStm(string op, Statement left, Statement right)
            : base(left, right)
        {
            if ((object)left == null)
            {
                throw new ArgumentNullException("left");
            }

            if ((object)right == null)
            {
                throw new ArgumentNullException("right");
            }

            base.SetOperator(op != null ? op.Trim() : String.Empty);
        }

        public string Operator
        {
            get
            {
                return base.GetOperator();
            }
        }

        public static ArithmeticStm As(char op, Statement left, Statement right)
        {
            switch (op)
            {
                case '*':
                    return MultiplyStm.As(left, right);
                case '/':
                    return DivideStm.As(left, right);
                case '%':
                    return ModuloStm.As(left, right);
                case '-':
                    return SubtractStm.As(left, right);
                default:
                    return AddStm.As(left, right);
            }
        }

        # region Operators

        public static AddStm operator +(ArithmeticStm left, ArithmeticStm right)
        {
            return Statement.Add(left, right);
        }

        public static SubtractStm operator -(ArithmeticStm left, ArithmeticStm right)
        {
            return Statement.Subtract(left, right);
        }

        public static DivideStm operator /(ArithmeticStm left, ArithmeticStm right)
        {
            return Statement.Divide(left, right);
        }

        public static MultiplyStm operator *(ArithmeticStm left, ArithmeticStm right)
        {
            return Statement.Multiply(left, right);
        }

        public static ModuloStm operator %(ArithmeticStm left, ArithmeticStm right)
        {
            return Statement.Mod(left, right);
        }

        # endregion
    }
}
