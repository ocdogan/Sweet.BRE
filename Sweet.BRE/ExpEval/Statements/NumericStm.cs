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
    public class NumericStm : ValueStm<double>
    {
        public static readonly NumericStm NaN = new NumericStm(double.NaN);
        public static readonly NumericStm Zero = new NumericStm(0);
        public static readonly NumericStm MinValue = new NumericStm(double.MinValue);
        public static readonly NumericStm MaxValue = new NumericStm(double.MaxValue);
        public static readonly NumericStm One = new NumericStm(1);
        public static readonly NumericStm MinusOne = new NumericStm(-1);

        public NumericStm(string value)
            : base(0)
        {
            double d = 0;
            double.TryParse(value, out d);

            SetValue(d);
        }

        public NumericStm(double value) 
            : base(value)
        {
        }

        public static NumericStm As(double value)
        {
            return new NumericStm(value);
        }

        public override object Clone()
        {
            return new NumericStm(base.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return StmCommon.ToDouble(base.Value);
        }

        # region Operators

        public static AddStm operator +(NumericStm left, NumericStm right)
        {
            return Statement.Add(left, right);
        }

        public static SubtractStm operator -(NumericStm left, NumericStm right)
        {
            return Statement.Subtract(left, right);
        }

        public static DivideStm operator /(NumericStm left, NumericStm right)
        {
            return Statement.Divide(left, right);
        }

        public static MultiplyStm operator *(NumericStm left, NumericStm right)
        {
            return Statement.Multiply(left, right);
        }

        public static ModuloStm operator %(NumericStm left, NumericStm right)
        {
            return Statement.Mod(left, right);
        }

        public static implicit operator NumericStm(bool value)
        {
            return new NumericStm(value ? 1 : 0);
        }

        public static implicit operator NumericStm(byte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(char value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(decimal value)
        {
            return new NumericStm((double)value);
        }

        public static implicit operator NumericStm(double value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(short value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(int value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(long value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(sbyte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(float value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(string value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(ushort value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(uint value)
        {
            return new NumericStm(value);
        }

        public static implicit operator NumericStm(ulong value)
        {
            return new NumericStm(value);
        }

        public static explicit operator bool(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (value.Value == 1) : false;
        }

        public static explicit operator byte(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (byte)value.Value : (byte)0;
        }

        public static explicit operator char(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (char)value.Value : (char)0;
        }

        public static explicit operator decimal(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (decimal)value.Value : 0;
        }

        public static explicit operator double(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? value.Value : double.NaN;
        }

        public static explicit operator float(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (float)value.Value : float.NaN;
        }

        public static explicit operator int(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (int)value.Value : 0;
        }

        public static explicit operator long(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (long)value.Value : 0;
        }

        public static explicit operator short(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (short)value.Value : (short)0;
        }

        public static explicit operator string(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? value.Value.ToString("R", CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator uint(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (uint)value.Value : 0;
        }

        public static explicit operator ulong(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (ulong)value.Value : 0;
        }

        public static explicit operator ushort(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? (ushort)value.Value : (ushort)0;
        }

        public static explicit operator BooleanStm(NumericStm value)
        {
            if (!ReferenceEquals(value, null))
            {
                return BooleanStm.False;
            }

            return (value.Value == 1 ? BooleanStm.True : BooleanStm.False);
        }

        public static explicit operator StringStm(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? StringStm.As(value.Value.ToString("R", CultureInfo.InvariantCulture)) : StringStm.As(null);
        }

        public static explicit operator TimeStm(NumericStm value)
        {
            return !ReferenceEquals(value, null) ? TimeStm.As(value.Value) : TimeStm.As(0);
        }

        # endregion
    }
}
