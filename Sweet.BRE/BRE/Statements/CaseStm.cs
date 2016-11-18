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
    public sealed class CaseStm : BaseScopeStm
    {
        private SwitchStm _switchE;
        private List<Statement> _caseList;

        internal CaseStm()
            : base()
        {
            _caseList = new List<Statement>();
        }

        internal CaseStm(Statement caseE)
            : this()
        {
            if ((object)caseE != null)
            {
                For(caseE);
            }
        }

        public Statement[] Case
        {
            get
            {
                return _caseList.ToArray();
            }
        }

        public SwitchStm Switch
        {
            get
            {
                return _switchE;
            }
        }

        internal void SetSwitch(SwitchStm switchE)
        {
            _switchE = switchE;
        }

        public bool SatisfyCase(Statement stm)
        {
            if ((object)stm == null)
            {
                stm = Statement.Null;
            }

            foreach (Statement e in _caseList)
            {
                if (e.Equals(stm))
                    return true;
            }

            return false;
        }

        internal CaseStm For(Statement caseE)
        {
            if (((object)caseE != null) && !SatisfyCase(caseE))
            {
                _caseList.Add(caseE);
            }

            return this;
        }

        public CaseStm Do(params ActionStm[] doActions)
        {
            base.DoAction(doActions);
            return this;
        }

        public static CaseStm As(Statement caseE)
        {
            return new CaseStm(caseE);
        }

        public override object Clone()
        {
            CaseStm ce = new CaseStm();

            foreach (Statement e in _caseList)
            {
                if (!ReferenceEquals(e, null))
                {
                    ce.For((Statement)e.Clone());
                }
            }

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    ce.DoAction((ActionStm)a.Clone());
                }
            }

            return ce;
        }

        public override void Dispose()
        {
            for (int i = _caseList.Count - 1; i > -1; i--)
            {
                Statement e = _caseList[i];
                _caseList.RemoveAt(i);

                e.Dispose();
            }

            base.Dispose();
        }

        internal bool EqualCaseList(Statement[] caseList)
        {
            if ((caseList.Length == _caseList.Count))
            {
                foreach (Statement e in caseList)
                {
                    if (!SatisfyCase(e))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            CaseStm objA = obj as CaseStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                EqualActions(objA.Actions) && EqualCaseList(objA.Case));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (_caseList.Count == 0)
            {
                builder.AppendFormat("{0}: ", RuleConstants.DEFAULT);
                builder.AppendLine();
            }
            else
            {
                foreach (Statement stm in _caseList)
                {
                    builder.AppendFormat("{0} {1}: ", 
                        RuleConstants.CASE, StmCommon.PrepareToString(stm));
                    builder.AppendLine();
                }
            }

            builder.Append(base.ToString());
            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }
    }
}
