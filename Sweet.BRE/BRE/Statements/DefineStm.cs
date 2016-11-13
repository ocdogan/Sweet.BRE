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
using System.Globalization;
using System.Text;

namespace Sweet.BRE
{
    public sealed class DefineStm : ActionStm
    {
        private StringStm _name;

        public DefineStm(StringStm name)
            : base()
        {
            _name = (ReferenceEquals(name, null) ? (StringStm)null : name);
        }

        public StringStm Name
        {
            get
            {
                return _name;
            }
        }

        public static DefineStm As(string name)
        {
            return new DefineStm(name);
        }

        public static DefineStm As(StringStm name)
        {
            return new DefineStm(name);
        }

        public override object Clone()
        {
            return DefineStm.As((StringStm)_name.Value);
        }

        public override bool Equals(object obj)
        {
            DefineStm objA = obj as DefineStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_name, objA.Name));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}); ", RuleConstants.DEFINE, 
                StmCommon.PrepareToString(_name));

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_name).Evaluate(context);

            string name = ((obj != null) ? obj.ToString() : null);
            name = ((name != null) ? name.Trim() : String.Empty);

            IEvaluationScope topScope = context.ScopeStack[0];
            if (!topScope.Contains(name))
            {
                topScope.Set(name, (string)null);
            }

            return null;
        }

        # region Operators

        public static implicit operator DefineStm(string name)
        {
            return new DefineStm(name);
        }

        # endregion
    }
}
