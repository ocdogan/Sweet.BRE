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
    public sealed class ModuloStm : ArithmeticStm
    {
        public ModuloStm(Statement left, Statement right)
            : base("%", left, right)
        {
        }

        public override object Clone()
        {
            Statement l = ((object)base.Left != null ? (Statement)base.Left.Clone() : null);
            Statement r = ((object)base.Right != null ? (Statement)base.Right.Clone() : null);

            return (new ModuloStm(l, r));
        }

        public static ModuloStm As(Statement left, Statement right)
        {
            return (new ModuloStm(left, right));
        }

        protected override object Evaluate(object o1, object o2)
        {
            double d1 = StmCommon.ToDouble(o1, double.NaN);
            double d2 = StmCommon.ToDouble(o2, double.NaN);

            if (double.IsNaN(d1))
            {
                throw new StatementException(String.Format(ExpEvalResStrings.GetString("CannotModuloANonNumericValue"),
                    StmCommon.ToString(o1)));
            }

            if (double.IsNaN(d2))
            {
                throw new StatementException(String.Format(ExpEvalResStrings.GetString("CannotModuloValueByANonNumericValue"),
                    StmCommon.ToString(o2)));
            }

            return d1 % d2;
        }
    }
}
