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
    public sealed class ConditionTestStm : ActionStm
    {
        private BooleanStm _condition;

        private Statement _ifTrue;
        private Statement _ifFalse;

        public ConditionTestStm(BooleanStm condition, Statement ifTrue, Statement ifFalse)
            : base()
        {
            _condition = !ReferenceEquals(condition, null) ? condition : (BooleanStm)true;
            _ifTrue = !ReferenceEquals(ifTrue, null) ? ifTrue : Statement.Null;
            _ifFalse = !ReferenceEquals(ifFalse, null) ? ifFalse : Statement.Null;
        }

        public BooleanStm Condition
        {
            get
            {
                return _condition;
            }
        }

        public Statement IfTrue
        {
            get
            {
                return _ifTrue;
            }
        }

        public Statement IfFalse
        {
            get
            {
                return _ifFalse;
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

        public static ConditionTestStm As(BooleanStm condition, Statement ifTrue, Statement ifFalse)
        {
            return new ConditionTestStm(condition, ifTrue, ifFalse);
        }

        public override object Clone()
        {
            BooleanStm condition = !ReferenceEquals(_condition, null) ? (BooleanStm)_condition.Clone() : null;

            Statement ifTrue = !ReferenceEquals(_ifTrue, null) ? (Statement)_ifTrue.Clone() : null;
            Statement ifFalse = !ReferenceEquals(_ifFalse, null) ? (Statement)_ifFalse.Clone() : null;

            return ConditionTestStm.As(condition, ifTrue, ifFalse);
        }

        public override bool Equals(object obj)
        {
            ConditionTestStm objA = obj as ConditionTestStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_condition, objA.Condition) &&
                object.Equals(_ifTrue, objA.IfTrue) &&
                object.Equals(_ifFalse, objA.IfFalse));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(RuleConstants.CHECK);
            builder.Append("(");
            builder.Append(StmCommon.PrepareToString(_condition));
            builder.Append(" ? ");
            builder.Append(StmCommon.PrepareToString(_ifTrue));
            builder.Append(" : ");
            builder.Append(StmCommon.PrepareToString(_ifFalse));
            builder.Append(") ");

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object o1 = !ReferenceEquals(_condition, null) ? ((IStatement)_condition).Evaluate(context) : true;

            bool test = StmCommon.ToBoolean(o1, false);
            object result = (test ? ((IStatement)_ifTrue).Evaluate(context) : ((IStatement)_ifFalse).Evaluate(context));

            return result;
        }
    }
}
