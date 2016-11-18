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
    public sealed class IfThenStm : BaseScopeStm
    {
        private BooleanStm _condition;
        private IfThenStm _elseIf;

        public IfThenStm(BooleanStm condition)
            : base()
        {
            _condition = condition;
        }

        public BooleanStm Condition
        {
            get
            {
                return _condition;
            }
        }

        public IfThenStm Else
        {
            get
            {
                return _elseIf;
            }
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_condition, null))
            {
                _condition.Dispose();
                _condition = null;
            }

            if (!ReferenceEquals(_elseIf, null))
            {
                _elseIf.Dispose();
                _elseIf = null;
            }

            base.Dispose();
        }

        public IfThenStm Also(params ActionStm[] doActions)
        {
            return Then(doActions);
        }

        public IfThenStm Then(params ActionStm[] doActions)
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

        private IfThenStm[] GetIfStack()
        {
            List<IfThenStm> list = new List<IfThenStm>();

            IfThenStm ifStm = this;
            while (!ReferenceEquals(ifStm, null))
            {
                list.Add(ifStm);
                ifStm = ifStm.Else;
            }

            return list.ToArray();
        }

        private void SetElseIf(IfThenStm elseIf)
        {
            _elseIf = elseIf;
        }

        public IfThenStm Otherwise(params ActionStm[] doActions)
        {
            if (!ReferenceEquals(doActions, null))
            {
                IfThenStm lastIf = this;
                IfThenStm[] ifStack = GetIfStack();

                if (ifStack.Length > 1)
                {
                    lastIf = ifStack[ifStack.Length - 1];

                    if (!ReferenceEquals(lastIf.Condition, null))
                    {
                        lastIf.SetElseIf(new IfThenStm(null));
                        lastIf = lastIf.Else;
                    }
                }
                else if ((ifStack.Length == 1) && !ReferenceEquals(_condition, null))
                {
                    _elseIf = new IfThenStm(null);
                    lastIf = _elseIf;
                }

                lastIf.Then(doActions);
            }

            return this;
        }

        public IfThenStm ElseIf(IfThenStm elseIf)
        {
            if (!ReferenceEquals(elseIf, null) && ReferenceEquals(elseIf, this))
            {
                IfThenStm[] ifStack = GetIfStack();

                IfThenStm lastIf = ifStack[ifStack.Length - 1];

                if (ifStack.Length == 1)
                {
                    _elseIf = elseIf;
                }
                else if (ReferenceEquals(elseIf.Condition, null) || !ReferenceEquals(lastIf.Condition, null))
                {
                    lastIf.SetElseIf(elseIf);
                }
                else
                {
                    IfThenStm tempIf = ifStack[ifStack.Length - 2];

                    tempIf.SetElseIf(elseIf);
                    elseIf.SetElseIf(lastIf);
                }
            }

            return this;
        }

        public IfThenStm ElseIf(BooleanStm condition, params ActionStm[] doActions)
        {
            IfThenStm[] ifStack = GetIfStack();

            // Search any matching condition
            foreach (IfThenStm ifStm in ifStack)
            {
                if (object.Equals(ifStm.Condition, condition))
                {
                    ifStm.Then(doActions);
                    return this;
                }
            }

            IfThenStm elseIf = IfThenStm.As(condition);
            elseIf.Then(doActions);

            IfThenStm lastIf = ifStack[ifStack.Length - 1];

            if (ifStack.Length == 1)
            {
                _elseIf = elseIf;
            }
            else if (ReferenceEquals(condition, null) || !ReferenceEquals(lastIf.Condition, null))
            {
                lastIf.SetElseIf(elseIf);
            }
            else
            {
                IfThenStm tempIf = ifStack[ifStack.Length - 2];

                tempIf.SetElseIf(elseIf);
                elseIf.SetElseIf(lastIf);
            }

            return this;
        }

        public static IfThenStm As(BooleanStm condition)
        {
            return new IfThenStm(condition);
        }

        public override object Clone()
        {
            BooleanStm condition = null;
            if (!ReferenceEquals(_condition, null))
                condition = (BooleanStm)_condition.Clone();

            IfThenStm result = IfThenStm.As(condition);

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    result.Then((ActionStm)a.Clone());
                }
            }

            IfThenStm elseIf = null;
            if (!ReferenceEquals(_elseIf, null))
                elseIf = (IfThenStm)_elseIf.Clone();

            result.SetElseIf(elseIf);

            return result;
        }

        public override bool Equals(object obj)
        {
            IfThenStm objA = obj as IfThenStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_condition, objA.Condition) &&
                base.EqualActions(objA.Actions) && object.Equals(_elseIf, objA.Else));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!ReferenceEquals(_condition, null))
            {
                builder.AppendFormat("{0} {1} {2} ", RuleConstants.IF,
                    StmCommon.PrepareToString(_condition), RuleConstants.THEN);

                builder.AppendLine();
            }

            builder.Append(base.ToString());
            builder.Append(RuleConstants.END + " ");

            if (!ReferenceEquals(_elseIf, null))
            {
                builder.AppendFormat("{0} {1} ", RuleConstants.ELSE, _elseIf);
            }

            return builder.ToString();
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.Canceled || context.Halted || context.InReturn);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object o1 = !ReferenceEquals(_condition, null) ? ((IStatement)_condition).Evaluate(context) : true;
            if ((o1 == null) || StmCommon.ToBoolean(o1, false))
            {
                return base.Evaluate(context);
            }

            if (!ReferenceEquals(_elseIf, null) && !ExecutionBroken(context))
            {
                context.UpgradeScope();
                try
                {
                    ((IStatement)_elseIf).Evaluate(context);
                }
                finally
                {
                    context.DowngradeScope();
                }
            }

            return null;
        }
    }
}
