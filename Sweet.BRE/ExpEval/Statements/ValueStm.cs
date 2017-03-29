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
    public abstract class ValueStm : Statement
    {
        private object _value;

        protected ValueStm(object value)
            : base()
        {
            _value = value;
        }

        public object Value
        {
            get
            {
                return GetValue();
            }
        }

        protected virtual object GetValue()
        {
            return _value;
        }

        protected void SetValue(object value)
        {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            ValueStm objA = obj as ValueStm;
            return ReferenceEquals(this, obj) || object.Equals(obj, _value) ||
                (!ReferenceEquals(objA, null) && object.Equals(_value, objA.Value));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            if (_value == null)
            {
                return StmConstants.NULL;
            }

            return StmCommon.ToString(_value);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            return _value;
        }

        # region Operators

        public static implicit operator ValueStm(bool value)
        {
            return (value ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator ValueStm(byte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(char value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator ValueStm(decimal value)
        {
            return new NumericStm((double)value);
        }

        public static implicit operator ValueStm(double value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(short value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(int value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(long value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(sbyte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(float value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(string value)
        {
            return new StringStm(value);
        }

        public static implicit operator ValueStm(ushort value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(uint value)
        {
            return new NumericStm(value);
        }

        public static implicit operator ValueStm(ulong value)
        {
            return new NumericStm(value);
        }

        # endregion
    }
}
