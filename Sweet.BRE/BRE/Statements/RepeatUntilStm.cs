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
    public sealed class RepeatUntilStm : LoopContext
    {
        private BooleanStm _condition;

        public RepeatUntilStm(BooleanStm condition)
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

        public static RepeatUntilStm As(BooleanStm condition)
        {
            return new RepeatUntilStm(condition);
        }

        public override object Clone()
        {
            BooleanStm condition = !ReferenceEquals(_condition, null) ? (BooleanStm)_condition.Clone() : null;
            RepeatUntilStm result = RepeatUntilStm.As(condition);

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    result.Do((ActionStm)a.Clone());
                }
            }

            return result;
        }

        public RepeatUntilStm Do(params ActionStm[] doActions)
        {
            if (doActions != null)
            {
                foreach (ActionStm a in doActions)
                {
                    if (!ReferenceEquals(a, null))
                    {
                        base.Actions.Add(a);
                    }
                }
            }
            return this;
        }

        public override bool Equals(object obj)
        {
            RepeatUntilStm objA = obj as RepeatUntilStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_condition, objA.Condition) && 
                base.EqualActions(objA.Actions));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0} ", RuleConstants.REPEAT);
            builder.AppendLine();

            builder.Append(base.ToString());

            builder.AppendFormat("{0} {1} ", StmCommon.PrepareToString(_condition), RuleConstants.UNTIL);

            return builder.ToString();
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.InBreak || context.Canceled || 
                context.Halted || context.InReturn);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            IStatement cond = (IStatement)_condition;

            while (!ExecutionBroken(context))
            {
                base.Evaluate(context);

                if (!ExecutionBroken(context))
                    break;

                object o1 = !ReferenceEquals(cond, null) ? cond.Evaluate(context) : true;
                if (!StmCommon.ToBoolean(o1, true))
                    break;
            }

            return null;
        }
    }
}
