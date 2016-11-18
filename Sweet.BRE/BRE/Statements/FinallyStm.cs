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
    public sealed class FinallyStm : ScopeStm
    {
        private TryStm _try;

        internal FinallyStm(TryStm tryE)
            : base()
        {
            SetTry(tryE);
        }

        public TryStm Try
        {
            get
            {
                return _try;
            }
        }

        internal void SetTry(TryStm tryE)
        {
            _try = tryE;
        }

        public FinallyStm Do(params ActionStm[] doActions)
        {
            base.DoAction(doActions);
            return this;
        }

        public override object Clone()
        {
            FinallyStm cln = new FinallyStm(null);

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    cln.DoAction((ActionStm)a.Clone());
                }
            }

            return cln;
        }

        public override bool Equals(object obj)
        {
            FinallyStm objA = obj as FinallyStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                EqualActions(objA.Actions));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}: ", RuleConstants.FINALLY);
            builder.AppendLine();

            builder.Append(base.ToString());
            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }
    }
}
