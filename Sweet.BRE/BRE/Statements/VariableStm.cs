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
    public sealed class VariableStm : BooleanStm
    {
        private StringStm _name;

        public VariableStm(StringStm name)
            : base(true)
        {
            _name = (ReferenceEquals(name, null) ? (StringStm)String.Empty : name);
        }

        public StringStm Name
        {
            get
            {
                return _name;
            }
        }

        public static VariableStm As(string name)
        {
            return new VariableStm(name);
        }

        public static VariableStm As(StringStm name)
        {
            return new VariableStm(name);
        }

        public override object Clone()
        {
            return VariableStm.As(_name);
        }

        public override bool Equals(object obj)
        {
            VariableStm objA = obj as VariableStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_name, objA.Name));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string name = StmCommon.PrepareToString(_name, false);

            name = ((name != null) ? name.Trim() : String.Empty);
            name = (String.IsNullOrEmpty(name) ? "@var" : name);

            builder.Append(name);

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_name).Evaluate(context);

            string name = ((obj != null) ? obj.ToString() : String.Empty);
            name = ((name != null) ? name.Trim() : String.Empty);

            if (String.IsNullOrEmpty(name))
            {
                if (context.LoggingEnabled)
                {
                    context.Log("Cannot resolve blank variable name.", EvalLogType.Warning);
                }
                return null;
            }

            IEvaluationScope scope = context.GetCurrentScope();

            IVariable vr = scope.Get(name);
            if (vr == null)
            {
                if (context.LoggingEnabled)
                {
                    context.Log(String.Format("Cannot find variable '{0}'.", name),
                        EvalLogType.Warning);
                }
                return null;
            }

            return ((vr != null) ? vr.Value : null);
        }

        # region Operators

        public static implicit operator VariableStm(string name)
        {
            return new VariableStm(name);
        }

        public static implicit operator VariableStm(bool value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(byte value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(char value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(DateTime value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(decimal value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(double value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(short value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(int value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(long value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(sbyte value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(float value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(TimeSpan value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(ushort value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(uint value)
        {
            return new VariableStm(value.ToString());
        }

        public static implicit operator VariableStm(ulong value)
        {
            return new VariableStm(value.ToString());
        }

        /*
        public static BooleanStm operator ==(VariableStm left, Statement right)
        {
            return StringStm.EqualTo(left, right);
        }

        public static BooleanStm operator ==(Statement left, VariableStm right)
        {
            return StringStm.EqualTo(left, right);
        }

        public static BooleanStm operator !=(VariableStm left, Statement right)
        {
            return StringStm.NotEqualTo(left, right);
        }

        public static BooleanStm operator !=(Statement left, VariableStm right)
        {
            return StringStm.NotEqualTo(left, right);
        }

        public static BooleanStm operator >(VariableStm left, VariableStm right)
        {
            return StringStm.GreaterThan(left, right);
        }

        public static BooleanStm operator >=(VariableStm left, VariableStm right)
        {
            return StringStm.GreaterThanOrEquals(left, right);
        }

        public static BooleanStm operator <(VariableStm left, VariableStm right)
        {
            return StringStm.LessThan(left, right);
        }

        public static BooleanStm operator <=(VariableStm left, VariableStm right)
        {
            return StringStm.LessThanOrEquals(left, right);
        }
        */

        # endregion
    }
}
