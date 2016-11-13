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
    public sealed class FactStm : BooleanStm
    {
        private StringStm _name;

        public FactStm(StringStm name)
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

        public static FactStm As(string name)
        {
            return new FactStm(name);
        }

        public static FactStm As(StringStm name)
        {
            return new FactStm(name);
        }

        public override object Clone()
        {
            return FactStm.As(_name);
        }

        public override bool Equals(object obj)
        {
            FactStm objA = obj as FactStm;
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

            name = (name != null ? name.Trim() : String.Empty);
            name = (String.IsNullOrEmpty(name) ? "fact" : name);
            
            name = "@" + name;

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
                    context.Log("Cannot resolve blank fact name.", EvalLogType.Warning);
                }
                return null;
            }

            IFactList facts = context.Facts;
            if (!facts.Contains(name))
            {
                if (context.LoggingEnabled)
                {
                    context.Log(String.Format("Cannot find fact '{0}'.", name),
                        EvalLogType.Warning);
                }
                return null;
            }

            return facts[name];
        }

        # region Operators

        public static implicit operator FactStm(string name)
        {
            return new FactStm(name);
        }

        public static BooleanStm operator ==(FactStm left, FactStm right)
        {
            return StringStm.EqualTo(left, right);
        }

        public static BooleanStm operator >(FactStm left, FactStm right)
        {
            return StringStm.GreaterThan(left, right);
        }

        public static BooleanStm operator >=(FactStm left, FactStm right)
        {
            return StringStm.GreaterThanOrEquals(left, right);
        }

        public static BooleanStm operator !=(FactStm left, FactStm right)
        {
            return StringStm.NotEqualTo(left, right);
        }

        public static BooleanStm operator <(FactStm left, FactStm right)
        {
            return StringStm.LessThan(left, right);
        }

        public static BooleanStm operator <=(FactStm left, FactStm right)
        {
            return StringStm.LessThanOrEquals(left, right);
        }

        # endregion
    }
}
