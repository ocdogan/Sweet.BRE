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
    public sealed class AddStm : ArithmeticStm
    {
        public AddStm(Statement left, Statement right)
            : base("+", left, right)
        {
        }

        public override object Clone()
        {
            Statement l = ((object)base.Left != null ? (Statement)base.Left.Clone() : null);
            Statement r = ((object)base.Right != null ? (Statement)base.Right.Clone() : null);

            return (new AddStm(l, r));
        }

        public static AddStm As(Statement left, Statement right)
        {
            return (new AddStm(left, right));
        }

        private object AddDate(object o1, object o2)
        {
            DateTime dt1 = StmCommon.ToDate(o1);

            ValueType oType = StmCommon.GetType(o2);
            if ((oType == ValueType.Float) || (oType == ValueType.Integer))
            {
                double d3 = StmCommon.ToDouble(o2);
                return dt1.AddDays(d3);
            }

            if (oType == ValueType.TimeSpan)
            {
                TimeSpan ts1 = StmCommon.ToTime(o2);
                return dt1.Add(ts1);
            }

            DateTime dt2 = StmCommon.ToDate(o2);
            return dt1.AddTicks(dt2.Ticks);
        }

        private object AddString(object o1, object o2)
        {
            string s1 = StmCommon.ToString(o1);
            string s2 = StmCommon.ToString(o2);

            return s1 + s2;
        }

        private object AddTime(object o1, object o2)
        {
            TimeSpan t1 = StmCommon.ToTime(o1);

            ValueType type = StmCommon.GetType(o2);
            if ((type == ValueType.Float) || (type == ValueType.Integer))
            {
                double d3 = StmCommon.ToDouble(o2);
                return t1.Add(TimeSpan.FromDays(d3));
            }

            TimeSpan t2 = StmCommon.ToTime(o2);
            return t1.Add(t2);
        }

        protected override object Evaluate(object o1, object o2)
        {
            if (o1 is string)
            {
                return AddString(o1, o2);
            }

            if (o1 is DateTime)
            {
                return AddDate(o1, o2);
            }

            if (o1 is TimeSpan)
            {
                return AddTime(o1, o2);
            }

            double d1 = StmCommon.ToDouble(o1, double.NaN);
            if (double.IsNaN(d1))
            {
                throw new StatementException(String.Format(ExpEvalResStrings.GetString("CannotAddANonNumericValue"),
                    StmCommon.ToString(o1)));
            }

            double d2 = StmCommon.ToDouble(o2, double.NaN);
            if (double.IsNaN(d2))
            {
                return AddString(o1, o2);
            }

            return d1 + d2;
        }
    }
}
