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
    public sealed class StringStm : ValueStm<string>
    {
        public StringStm(string value) 
            : base(value)
        {
        }

        public static StringStm As(object value)
        {
            return new StringStm(value != null ? value.ToString() : null);
        }

        public override object Clone()
        {
            return new StringStm(base.Value);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return StmCommon.ToString(base.Value);
        }

        public override bool Equals(object obj)
        {
            StringStm objA = obj as StringStm;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                CommonHelper.EqualStrings(base.Value, objA.Value, true));
        }

        public override int GetHashCode()
        {
            string value = ToString();
            value = (value != null ? value : String.Empty);

            return value.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        # region Operators
        
        public static implicit operator StringStm(bool value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(byte value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(char value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(DateTime value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(decimal value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(double value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(Enum value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(short value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(int value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(long value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(sbyte value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(float value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(string value)
        {
            return new StringStm(value);
        }

        public static implicit operator StringStm(TimeSpan value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(ushort value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(uint value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator StringStm(ulong value)
        {
            return new StringStm(value.ToString());
        }

        public static explicit operator bool(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return false;
            }

            bool result = false;
            if (!bool.TryParse(value.Value, out result))
            {
                result = false;
            }

            return result;
        }

        public static explicit operator byte(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            byte result = 0;
            if (!byte.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator char(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return (char)0;
            }

            char result = (char)0;
            if (!char.TryParse(value.Value, out result))
            {
                result = (char)0;
            }

            return result;
        }

        public static explicit operator DateTime(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return DateTime.MinValue;
            }

            DateTime result = DateTime.MinValue;
            if (!DateTime.TryParse(value.Value, out result))
            {
                result = DateTime.MinValue;
            }

            return result;
        }

        public static explicit operator decimal(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            decimal result = 0;
            if (!decimal.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator double(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            double result = double.NaN;
            if (!double.TryParse(value.Value, out result))
            {
                result = double.NaN;
            }

            return result;
        }

        public static explicit operator short(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            short result = 0;
            if (!short.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator int(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            int result = 0;
            if (!int.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator long(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            long result = 0;
            if (!long.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator sbyte(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            sbyte result = 0;
            if (!sbyte.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator float(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            float result = float.NaN;
            if (!float.TryParse(value.Value, out result))
            {
                result = float.NaN;
            }

            return result;
        }

        public static explicit operator string(StringStm value)
        {
            return (ReferenceEquals(value, null) ? null : value.Value);
        }

        public static explicit operator ushort(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            ushort result = 0;
            if (!ushort.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator uint(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            uint result = 0;
            if (!uint.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator ulong(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return 0;
            }

            ulong result = 0;
            if (!ulong.TryParse(value.Value, out result))
            {
                result = 0;
            }

            return result;
        }

        public static explicit operator BooleanStm(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return BooleanStm.False;
            }

            bool b = false;
            if (!bool.TryParse(value.Value, out b))
            {
                b = false;
            }

            return (b ? BooleanStm.True : BooleanStm.False);
        }

        public static explicit operator DateStm(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return DateStm.MinValue;
            }

            DateTime d = DateTime.MinValue;
            if (!DateTime.TryParse(value.Value, out d))
            {
                d = DateTime.MinValue;
            }

            return new DateStm(d);
        }

        public static explicit operator NumericStm(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return NumericStm.NaN;
            }

            double d = double.NaN;
            if (!double.TryParse(value.Value, out d))
            {
                d = double.NaN;
            }

            return new NumericStm(d);
        }

        public static explicit operator TimeStm(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return TimeStm.Zero;
            }

            TimeSpan ts = TimeSpan.Zero;
            if (!TimeSpan.TryParse(value.Value, out ts))
            {
                double d = 0;
                if (double.TryParse(value.Value, out d))
                {
                    ts = TimeSpan.FromMilliseconds(d);
                }
            }

            return new TimeStm(ts);
        }

        public static explicit operator TimeSpan(StringStm value)
        {
            if (ReferenceEquals(value, null))
            {
                return TimeSpan.Zero;
            }

            TimeSpan d = TimeSpan.Zero;
            if (!TimeSpan.TryParse(value.Value, out d))
            {
                d = TimeSpan.Zero;
            }

            return d;
        }

        # endregion
    }
}
