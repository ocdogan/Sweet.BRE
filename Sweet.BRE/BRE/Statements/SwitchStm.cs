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
    public sealed class SwitchStm : BaseScopeStm
    {
        private Statement _switchCase;
        private List<CaseStm> _caseList;

        public SwitchStm(Statement switchCase)
            : base()
        {
            _switchCase = switchCase;
            _caseList = new List<CaseStm>();
        }

        public CaseStm[] CaseList
        {
            get
            {
                return _caseList.ToArray();
            }
        }

        public Statement SwitchCase
        {
            get
            {
                return _switchCase;
            }
        }

        internal void AddCase(CaseStm caseE)
        {
            if ((object)caseE != null)
            {
                _caseList.Add(caseE);
                caseE.SetSwitch(this);
            }
        }

        private CaseStm CaseFor(Statement caseE)
        {
            if ((object)caseE == null)
            {
                caseE = Statement.Null;
            }

            foreach (CaseStm ce in _caseList)
            {
                if (ce.SatisfyCase(caseE))
                    return ce;
            }

            CaseStm result = new CaseStm(caseE);
            AddCase(result);

            return result;
        }

        public SwitchStm Case(Statement caseE, params ActionStm[] doActions)
        {
            CaseStm ce = CaseFor(caseE);
            ce.Do(doActions);

            return this;
        }

        public SwitchStm Case(Statement[] caseE, params ActionStm[] doActions)
        {
            if ((caseE == null) || (caseE.Length == 0))
            {
                Default(doActions);
                return this;
            }

            CaseStm ce = new CaseStm(caseE[0]);
            AddCase(ce);

            ce.Do(doActions);

            for (int i = 1; i < caseE.Length; i++)
            {
                ce.For(caseE[i]);
            }

            return this;
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_switchCase, null))
            {
                _switchCase.Dispose();
                _switchCase = null;
            }

            for (int i = _caseList.Count - 1; i > -1; i--)
            {
                CaseStm ce = _caseList[i];
                _caseList.RemoveAt(i);

                ce.SetSwitch(null);
                ce.Dispose();
            }

            base.Dispose();
        }

        public SwitchStm Default(params ActionStm[] doActions)
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

        public static SwitchStm As(Statement switchCase)
        {
            return new SwitchStm(switchCase);
        }

        public override object Clone()
        {
            Statement swCase = !ReferenceEquals(_switchCase, null) ? (Statement)_switchCase.Clone() : null;

            SwitchStm cln = SwitchStm.As(swCase);

            foreach (ActionStm action in base.Actions)
            {
                if (!ReferenceEquals(action, null))
                {
                    cln.Default((ActionStm)action.Clone());
                }
            }

            foreach (CaseStm caseE in _caseList)
            {
                if (!ReferenceEquals(caseE, null))
                {
                    cln.AddCase((CaseStm)caseE.Clone());
                }
            }

            return cln;
        }

        private bool EqualCases(CaseStm[] ceArr)
        {
            if (ceArr.Length == _caseList.Count)
            {
                foreach (CaseStm ce1 in ceArr)
                {
                    bool found = false;
                    foreach (CaseStm ce2 in _caseList)
                    {
                        if (ce2.Equals(ce1))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            SwitchStm objA = obj as SwitchStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_switchCase, objA.SwitchCase) &&
                base.EqualActions(objA.Actions) && 
                EqualCases(objA.CaseList));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!ReferenceEquals(_switchCase, null))
            {
                builder.AppendFormat("{0} ", RuleConstants.SWITCH);

                string cs = StmCommon.PrepareToString(_switchCase);
                if (cs.StartsWith("("))
                {
                    builder.AppendFormat("{0} ", cs);
                }
                else
                {
                    builder.AppendFormat("({0}) ", cs);
                }

                builder.AppendLine();
            }

            foreach (CaseStm ce in _caseList)
            {
                builder.AppendLine(ce.ToString());
            }

            if (base.Actions.Count > 0)
            {
                builder.AppendFormat("{0}: ", RuleConstants.DEFAULT);
                builder.Append(base.ToString());
            }

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object condLeft = !ReferenceEquals(_switchCase, null) ? ((IStatement)_switchCase).Evaluate(context) : null;

            foreach (CaseStm caseE in _caseList)
            {
                if (ExecutionBroken(context))
                    return null;

                foreach (IStatement condE in caseE.Case)
                {
                    object condRight = !ReferenceEquals(condE, null) ? condE.Evaluate(context) : null;
                    if (StmCommon.Compare(condLeft, condRight) == 0)
                    {
                        ((IStatement)caseE).Evaluate(context);
                        return null;
                    }

                    if (ExecutionBroken(context))
                        return null;
                }
            }

            base.Evaluate(context);

            return null;
        }
    }
}
