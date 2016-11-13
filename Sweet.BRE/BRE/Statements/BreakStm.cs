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
using System.Data;
using System.Text;

namespace Sweet.BRE
{
    public sealed class BreakStm : ActionStm
    {
        private BooleanStm _condition;

        public BreakStm()
            : this(null)
        {
        }

        public BreakStm(BooleanStm condition)
            : base()
        {
            _condition = !ReferenceEquals(condition, null) ? condition : (BooleanStm)true;
        }

        public BooleanStm Condition
        {
            get
            {
                return _condition;
            }
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_condition, null))
            {
                _condition.Dispose();
                _condition = null;
            }

            base.Dispose();
        }

        public static BreakStm As()
        {
            return new BreakStm();
        }

        public static BreakStm As(BooleanStm condition)
        {
            return new BreakStm(condition);
        }

        public override object Clone()
        {
            BooleanStm condition = !ReferenceEquals(_condition, null) ? (BooleanStm)_condition.Clone() : null;
            return BreakStm.As(condition);
        }

        public override bool Equals(object obj)
        {
            BreakStm objA = obj as BreakStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_condition, objA.Condition));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}); ", RuleConstants.BREAK,
                StmCommon.PrepareToString(_condition));

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object o1 = !ReferenceEquals(_condition, null) ? ((IStatement)_condition).Evaluate(context) : true;
            if (StmCommon.ToBoolean(o1, true))
            {
                context.Break();
            }

            return null;
        }
    }
}
