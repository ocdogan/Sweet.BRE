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
    public sealed class TimeStm : ValueStm<TimeSpan>
    {
        public static readonly TimeStm Zero = new TimeStm(TimeSpan.Zero);
        public static readonly TimeStm MinValue = new TimeStm(TimeSpan.MinValue);
        public static readonly TimeStm MaxValue = new TimeStm(TimeSpan.MaxValue);

        public TimeStm(double value)
            : this(TimeSpan.FromMilliseconds(value))
        {
        }

        public TimeStm(TimeSpan value) 
            : base(value)
        {
        }

        public static TimeStm As(double value)
        {
            return new TimeStm(value);
        }

        public static TimeStm As(string value)
        {
            TimeSpan val = TimeSpan.Zero;
            if (!TimeSpan.TryParse(value, out val))
            {
                val = TimeSpan.Zero;
            }

            return new TimeStm(val);
        }

        public static TimeStm As(TimeSpan value)
        {
            return new TimeStm(value);
        }

        public override object Clone()
        {
            return new TimeStm(base.Value);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return StmCommon.ToTime(base.Value);
        }

        # region Operators

        public static implicit operator TimeStm(byte value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(DateTime value)
        {
            return new TimeStm(value.TimeOfDay);
        }

        public static implicit operator TimeStm(decimal value)
        {
            return new TimeStm((double)value);
        }

        public static implicit operator TimeStm(double value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(short value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(int value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(long value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(sbyte value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(float value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(string value)
        {
            return TimeStm.As(value);
        }

        public static implicit operator TimeStm(TimeSpan value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(ushort value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(uint value)
        {
            return new TimeStm(value);
        }

        public static implicit operator TimeStm(ulong value)
        {
            return new TimeStm(value);
        }

        public static explicit operator bool(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? false : (value.Value.TotalMilliseconds == 1));
        }

        public static explicit operator byte(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (byte)0 : (byte)value.Value.TotalMilliseconds);
        }

        public static explicit operator char(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (char)0 : (char)value.Value.TotalMilliseconds);
        }

        public static explicit operator decimal(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (decimal)value.Value.TotalMilliseconds);
        }

        public static explicit operator double(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : value.Value.TotalMilliseconds);
        }

        public static explicit operator short(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (short)0 : (short)value.Value.TotalMilliseconds);
        }

        public static explicit operator int(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (int)value.Value.TotalMilliseconds);
        }

        public static explicit operator long(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (long)value.Value.TotalMilliseconds);
        }

        public static explicit operator sbyte(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (sbyte)0 : (sbyte)value.Value.TotalMilliseconds);
        }

        public static explicit operator float(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (float)value.Value.TotalMilliseconds);
        }

        public static explicit operator string(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? null : value.Value.ToString());
        }

        public static explicit operator ushort(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (ushort)0 : (ushort)value.Value.TotalMilliseconds);
        }

        public static explicit operator uint(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (uint)0 : (uint)value.Value.TotalMilliseconds);
        }

        public static explicit operator ulong(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? (ulong)0 : (ulong)value.Value.TotalMilliseconds);
        }

        public static explicit operator NumericStm(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? NumericStm.As(double.NaN) : NumericStm.As(value.Value.TotalMilliseconds));
        }

        public static explicit operator StringStm(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? StringStm.As(null) : StringStm.As(value.Value.TotalMilliseconds));
        }

        public static explicit operator BooleanStm(TimeStm value)
        {
            return (ReferenceEquals(value, null) ? BooleanStm.False : 
                (value.Value.TotalMilliseconds == 1 ? BooleanStm.True : BooleanStm.False));
        }

        # endregion
    }
}
