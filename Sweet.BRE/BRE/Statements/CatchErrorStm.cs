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
    public sealed class CatchErrorStm : BaseScopeStm
    {
        private TryStm _owner;
        private StringStm _onError;

        internal CatchErrorStm(TryStm tryE)
            : this(tryE, null)
        {
        }

        internal CatchErrorStm(TryStm tryE, StringStm onError)
            : base()
        {
            SetOwner(tryE);
            _onError = (ReferenceEquals(onError, null) ? (StringStm)null : onError);
        }

        public StringStm OnError
        {
            get
            {
                return _onError;
            }
        }

        public TryStm Owner
        {
            get
            {
                return _owner;
            }
        }

        internal void SetOwner(TryStm owner)
        {
            _owner = owner;
        }

        public CatchErrorStm Do(params ActionStm[] doActions)
        {
            base.DoAction(doActions);
            return this;
        }

        public override object Clone()
        {
            CatchErrorStm cln = new CatchErrorStm(null, 
                (StringStm)_onError.Clone());

            foreach (ActionStm action in base.Actions)
            {
                if (!ReferenceEquals(action, null))
                {
                    cln.DoAction((ActionStm)action.Clone());
                }
            }

            return cln;
        }

        public override bool Equals(object obj)
        {
            CatchErrorStm objA = obj as CatchErrorStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                CommonHelper.EqualStrings(_onError.Value, objA.OnError.Value, true) && 
                EqualActions(objA.Actions));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string strOnError = null;
            if (_onError.Value != null)
            {
                strOnError = StmCommon.PrepareToString(_onError);
            }

            if (String.IsNullOrEmpty(strOnError))
            {
                builder.Append(RuleConstants.ONERROR);
            }
            else
            {
                builder.AppendFormat("{0} '{1}'", RuleConstants.ONERROR, strOnError);
            }

            CommonHelper.LineFeedIfNeeded(builder);
            builder.Append(base.ToString());

            return builder.ToString();
        }
    }
}
