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
    public sealed class RaiseErrorStm : ActionStm
    {
        private Statement _message;
        private BooleanStm _condition;

        public RaiseErrorStm(Statement message)
            : this(message, null)
        {
        }

        public RaiseErrorStm(Statement message, BooleanStm condition)
            : base()
        {
            _message = (ReferenceEquals(message, null) ? Statement.Null : message);
            _condition = !ReferenceEquals(condition, null) ? condition : (BooleanStm)true;
        }

        public BooleanStm Condition
        {
            get
            {
                return _condition;
            }
        }

        public Statement Message
        {
            get
            {
                return _message;
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

        public static RaiseErrorStm As(string message)
        {
            return new RaiseErrorStm(message);
        }

        public static RaiseErrorStm As(string message, BooleanStm condition)
        {
            return new RaiseErrorStm(message, condition);
        }

        public static RaiseErrorStm As(Statement message)
        {
            return new RaiseErrorStm(message);
        }

        public static RaiseErrorStm As(Statement message, BooleanStm condition)
        {
            return new RaiseErrorStm(message, condition);
        }

        public override object Clone()
        {
            BooleanStm condition = !ReferenceEquals(_condition, null) ? (BooleanStm)_condition.Clone() : null;

            return RaiseErrorStm.As((Statement)_message.Clone(), condition);
        }

        public override bool Equals(object obj)
        {
            RaiseErrorStm objA = obj as RaiseErrorStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_condition, objA.Condition) &&
                object.Equals(_message, objA.Message));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}, {2}); ", RuleConstants.RAISE, 
                StmCommon.PrepareToString(_message), 
                StmCommon.PrepareToString(_condition));

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object o1 = !ReferenceEquals(_condition, null) ? ((IStatement)_condition).Evaluate(context) : true;
            if (StmCommon.ToBoolean(o1, true))
            {
                object obj = ((IStatement)_message).Evaluate(context);
                string message = (obj != null ? obj.ToString() : null);

                throw new RuleException(message);
            }

            return null;
        }
    }
}
