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
using System.Globalization;
using System.Text;

namespace Sweet.BRE
{
    public sealed class DateStm : ValueStm<DateTime>
    {
        public static readonly DateStm MinValue = new DateStm(DateTime.MinValue);
        public static readonly DateStm MaxValue = new DateStm(DateTime.MaxValue);

        public DateStm(DateTime value) 
            : base(value)
        {
        }

        public static DateStm Now
        {
            get
            {
                return new DateStm(DateTime.Now);
            }
        }

        public static DateStm Today
        {
            get
            {
                return new DateStm(DateTime.Today);
            }
        }

        public static DateStm UtcNow
        {
            get
            {
                return new DateStm(DateTime.UtcNow);
            }
        }

        public static DateStm As(DateTime value)
        {
            return new DateStm(value);
        }

        public static DateStm As(string value)
        {
            DateTime val = DateTime.MinValue;
            if (!DateTime.TryParse(value, out val))
            {
                val = DateTime.MinValue;
            }

            return new DateStm(val);
        }

        public override object Clone()
        {
            return new DateStm(base.Value);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return StmCommon.ToDate(base.Value);
        }

        # region Operators

        public static implicit operator DateStm(string value)
        {
            DateTime dValue = DateTime.MinValue;
            DateTime.TryParse(value, out dValue);

            return new DateStm(dValue);
        }

        public static implicit operator DateStm(DateTime value)
        {
            return new DateStm(value);
        }

        # endregion
    }
}
