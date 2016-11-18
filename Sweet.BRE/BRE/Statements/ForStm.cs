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
    public sealed class ForStm : LoopContext
    {
        private StringStm _variable;
        
        private Statement _from;
        private Statement _to;
        private NumericStm _step;

        public ForStm(StringStm variable)
            : this(variable, NumericStm.Zero, new NumericStm(1), new NumericStm(1))
        {
        }

        public ForStm(StringStm variable, Statement to)
            : this(variable, NumericStm.Zero, to, new NumericStm(1))
        {
        }

        public ForStm(StringStm variable, Statement from, Statement to)
            : this(variable, from, to, new NumericStm(1))
        {
        }

        public ForStm(StringStm variable, Statement from, Statement to, NumericStm step)
            : base()
        {
            if (ReferenceEquals(variable, null))
            {
                throw new ArgumentNullException("variable");
            }

            if (ReferenceEquals(from, null))
            {
                from = NumericStm.Zero;
            }

            if (ReferenceEquals(to, null))
            {
                to = NumericStm.As(1);
            }

            if (ReferenceEquals(step, null))
            {
                step = NumericStm.As(1);
            }

            _variable = variable;
            
            _from = from;
            _to = to;
            _step = step;
        }

        public Statement From
        {
            get
            {
                return _from;
            }
        }

        public NumericStm Step
        {
            get
            {
                return _step;
            }
        }

        public Statement To
        {
            get
            {
                return _to;
            }
        }

        public StringStm Variable
        {
            get
            {
                return _variable;
            }
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_variable, null))
            {
                _variable.Dispose();
                _variable = null;
            }

            if (!ReferenceEquals(_from, null))
            {
                _from.Dispose();
                _from = null;
            }

            if (!ReferenceEquals(_to, null))
            {
                _to.Dispose();
                _to = null;
            }

            if (!ReferenceEquals(_step, null))
            {
                _step.Dispose();
                _step = null;
            }

            base.Dispose();
        }

        public static ForStm As(StringStm variable)
        {
            return new ForStm(variable);
        }

        public static ForStm As(StringStm variable, Statement to)
        {
            return new ForStm(variable, to);
        }

        public static ForStm As(StringStm variable, Statement from, Statement to)
        {
            return new ForStm(variable, from, to);
        }

        public static ForStm As(StringStm variable, Statement from, Statement to, NumericStm step)
        {
            return new ForStm(variable, from, to, step);
        }

        public override object Clone()
        {
            StringStm condition = (object)_variable != null ? (StringStm)_variable.Clone() : null;
            Statement from = (object)_from != null ? (Statement)_from.Clone() : null;
            Statement to = (object)_to != null ? (Statement)_to.Clone() : null;
            NumericStm step = (object)_step != null ? (NumericStm)_step.Clone() : null;

            ForStm result = ForStm.As(condition, from, to, step);

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    result.Do((ActionStm)a.Clone());
                }
            }

            return result;
        }

        public ForStm UsingFrom(Statement from)
        {
            if (ReferenceEquals(from, null))
            {
                from = NumericStm.Zero;
            }

            _from = from;
            return this;
        }

        public ForStm UsingTo(Statement to)
        {
            if (ReferenceEquals(to, null))
            {
                to = NumericStm.As(1);
            }

            _to = to;
            return this;
        }

        public ForStm UsingStep(NumericStm step)
        {
            if (ReferenceEquals(step, null))
            {
                step = NumericStm.As(1);
            }

            _step = step;
            return this;
        }

        public ForStm Do(params ActionStm[] doActions)
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
            ForStm objA = obj as ForStm;

            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_variable, objA.Variable) &&
                object.Equals(_from, objA.From) &&
                object.Equals(_to, objA.To) &&
                object.Equals(_step, objA.Step) &&
                base.EqualActions(objA.Actions));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0} ", RuleConstants.FOR);

            string varName = StmCommon.PrepareToString(_variable, false);
            if (String.IsNullOrEmpty(varName))
            {
                varName = "@var";
            }

            builder.AppendFormat("{0} = {1} ",
                varName, StmCommon.PrepareToString(_from));

            builder.AppendFormat("{0} {1} ",
                RuleConstants.TO, StmCommon.PrepareToString(_to));

            if (_step.Value != 1)
            {
                builder.AppendFormat("{0} {1} ",
                    RuleConstants.STEP, StmCommon.PrepareToString(_step));
            }

            builder.AppendLine(RuleConstants.DO + " ");
            builder.Append(base.ToString());

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.InBreak || context.Canceled || 
                context.Halted || context.InReturn);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_variable).Evaluate(context);

            string variableName = (obj != null ? obj.ToString() : String.Empty);
            variableName = (variableName != null ? variableName.Trim() : String.Empty);

            object stepObj = 1;
            if (!ReferenceEquals(_step, null))
            {
                stepObj = ((IStatement)_step).Evaluate(context);
            }

            long step = StmCommon.ToInteger(stepObj, 1);
            step = (step == 0 ? 1 : step);

            long from = StmCommon.ToInteger((!ReferenceEquals(_from, null) ? ((IStatement)_from).Evaluate(context) : 0), 0);
            long to = StmCommon.ToInteger((!ReferenceEquals(_to, null) ? ((IStatement)_to).Evaluate(context) : 1), 1);

            IEvaluationScope scope = context.GetCurrentScope();

            for (long i = from; ((step < 0) ? (i > to) : (i < to)); i = i + step)
            {
                scope.Set(variableName, i);

                base.Evaluate(context);
                if (ExecutionBroken(context))
                    return null;
            }

            return null;
        }

        # region Operators

        public static implicit operator ForStm(string variable)
        {
            return ForStm.As(StringStm.As(variable));
        }

        # endregion
    }
}
