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
    public abstract class BooleanStm : ValueStm<bool>
    {
        public static readonly BooleanStm True = new TrueStm();
        public static readonly BooleanStm False = new FalseStm();

        protected BooleanStm(bool value)
            : base(value)
        {
        }

        public static BooleanStm As(bool value)
        {
            return (value ? BooleanStm.True : BooleanStm.False);
        }

        public override object Clone()
        {
            return (base.Value ? BooleanStm.True : BooleanStm.False);
        }

        public BooleanStm And(BooleanStm right)
        {
            return Statement.And(this, right);
        }

        public BooleanStm Not()
        {
            return Statement.Not(this);
        }

        public BooleanStm Or(BooleanStm right)
        {
            return Statement.Or(this, right);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return StmCommon.ToBoolean(base.Value);
        }

        # region Operators

        public static implicit operator BooleanStm(bool value)
        {
            return (value ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(byte value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(char value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(decimal value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(double value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(short value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(int value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(long value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(sbyte value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(float value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(string value)
        {
            bool bValue = false;
            if (!bool.TryParse(value, out bValue))
            {
                double dValue = 0;
                bValue = double.TryParse(value, out dValue) && (dValue == 1);
            }

            return (bValue ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(ushort value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(uint value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator BooleanStm(ulong value)
        {
            return ((value == 1) ? BooleanStm.True : BooleanStm.False);
        }

        public static explicit operator bool(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? false : value.Value);
        }

        public static explicit operator byte(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (byte)0 : (value.Value ? (byte)1 : (byte)0));
        }

        public static explicit operator char(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (char)0 : (value.Value ? (char)1 : (char)0));
        }

        public static explicit operator decimal(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (value.Value ? 1 : 0));
        }

        public static explicit operator double(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (value.Value ? 1 : 0));
        }

        public static explicit operator short(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (short)0 : (value.Value ? (short)1 : (short)0));
        }

        public static explicit operator int(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (value.Value ? 1 : 0));
        }

        public static explicit operator long(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (value.Value ? 1 : 0));
        }

        public static explicit operator sbyte(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (sbyte)0 : (value.Value ? (sbyte)1 : (sbyte)0));
        }

        public static explicit operator float(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? 0 : (value.Value ? 1 : 0));
        }

        public static explicit operator string(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? null : (value.Value ? bool.TrueString : bool.FalseString));
        }

        public static explicit operator ushort(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (ushort)0 : (value.Value ? (ushort)1 : (ushort)0));
        }

        public static explicit operator uint(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (uint)0 : (value.Value ? (uint)1 : (uint)0));
        }

        public static explicit operator ulong(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? (ulong)0 : (value.Value ? (ulong)1 : (ulong)0));
        }

        public static explicit operator NumericStm(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? NumericStm.As(double.NaN) : 
                (value.Value ? NumericStm.One : NumericStm.Zero));
        }

        public static explicit operator StringStm(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? StringStm.As(null) : 
                (value.Value ? StringStm.As(bool.TrueString) : StringStm.As(bool.FalseString)));
        }

        public static explicit operator TimeStm(BooleanStm value)
        {
            return (ReferenceEquals(value, null) ? TimeStm.Zero : (value.Value ? TimeStm.As(1) : TimeStm.Zero));
        }

        /*
        public static BooleanStm operator &(BooleanStm left, BooleanStm right)
        {
            return Statement.And(left, right);
        }

        public static BooleanStm operator |(BooleanStm left, BooleanStm right)
        {
            return Statement.Or(left, right);
        }

        public static UnaryNotStm operator !(BooleanStm target)
        {
            return Statement.Not(target);
        }

        public static BooleanStm operator ==(BooleanStm left, Statement right)
        {
            return Statement.EqualTo(left, right);
        }

        public static BooleanStm operator ==(Statement left, BooleanStm right)
        {
            return Statement.EqualTo(left, right);
        }

        public static BooleanStm operator !=(BooleanStm left, Statement right)
        {
            return Statement.Not(EqualTo(left, right));
        }

        public static BooleanStm operator !=(Statement left, BooleanStm right)
        {
            return Statement.Not(EqualTo(left, right));
        }
        */

        # endregion
    }
}
