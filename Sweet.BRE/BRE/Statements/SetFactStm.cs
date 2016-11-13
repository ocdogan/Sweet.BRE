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
    public sealed class SetFactStm : ActionStm
    {
        private Statement _value;
        private Statement _factName;

        public SetFactStm(Statement factName) 
            : this(factName, null)
        {
        }

        public SetFactStm(Statement factName, Statement value) 
            : base()
        {
            _factName = (ReferenceEquals(factName, null) ? Statement.Null : factName);
            _value = (ReferenceEquals(value, null) ? Statement.Null : value);
        }

        public Statement Fact
        {
            get
            {
                return _factName;
            }
        }

        public Statement Value
        {
            get
            {
                return _value;
            }
        }

        public SetFactStm Set(Statement value)
        {
            _value = value;
            return this;
        }

        public static SetFactStm As(string fact)
        {
            return new SetFactStm(fact);
        }

        public static SetFactStm As(string fact, Statement value)
        {
            return new SetFactStm(fact, value);
        }

        public static SetFactStm As(Statement fact)
        {
            return new SetFactStm(fact);
        }

        public static SetFactStm As(Statement fact, Statement value)
        {
            return new SetFactStm(fact, value);
        }

        public override object Clone()
        {
            return new SetFactStm((Statement)_factName.Clone(), (Statement)_value.Clone());
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_factName, null))
            {
                _factName.Dispose();
                _factName = null;
            }

            if (!ReferenceEquals(_value, null))
            {
                _value.Dispose();
                _value = null;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            SetFactStm objA = obj as SetFactStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_factName, objA.Fact) && 
                object.Equals(_value, objA.Value));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}, {2}); ", RuleConstants.FACT, 
                StmCommon.PrepareToString(_factName),
                StmCommon.PrepareToString(_value));

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_factName).Evaluate(context);

            string factName = (obj != null ? obj.ToString() : String.Empty);
            factName = (factName != null ? factName.Trim() : String.Empty);

            if (!String.Equals(factName, null))
            {
                object value = ((IStatement)_value).Evaluate(context, args);
                context.Facts.Set(factName, value);
            }

            return null;
        }

        # region Operators

        public static implicit operator SetFactStm(string fact)
        {
            return new SetFactStm(fact);
        }

        # endregion
    }
}
