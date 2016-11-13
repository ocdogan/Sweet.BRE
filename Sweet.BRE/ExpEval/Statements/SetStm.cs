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
    public sealed class SetStm : StatementList<Statement>
    {
        public SetStm(params Statement[] items) 
            : base(false)
        {
            Add(items);
        }

        public void Add(params Statement[] items)
        {
            base.AddRange(items);
        }

        public static SetStm As(params Statement[] items)
        {
            return new SetStm(items);
        }

        public override object Clone()
        {
            Statement[] items = new Statement[base.Count];
            for (int i = 0; i < base.Count; i++)
            {
                Statement item = base[i];
                if (!ReferenceEquals(item, null))
                {
                    items[i] = (Statement)item.Clone();
                }
            }

            return SetStm.As(items);
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj)
            {
                return true;
            }

            SetStm objA = obj as SetStm;
            if (!ReferenceEquals(objA, null) && (objA.Count == this.Count))
            {
                foreach (Statement e in objA)
                {
                    if (!this.Contains(e))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = (Statement.Null).GetHashCode();
            foreach (Statement e in this)
            {
                int hash = (!ReferenceEquals(e, null) ? e.GetHashCode() : 0);
                result = (result ^ hash);
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');

            int index = 0;
            foreach (Statement stm in this)
            {
                index++;
                if (index > 1)
                {
                    sb.Append(", ");
                }
                sb.AppendFormat("({0})", StmCommon.PrepareToString(stm));
            }

            sb.Append(')');

            return sb.ToString();
        }

        private object[] SolveArray(object obj)
        {
            if ((obj != null) && obj.GetType().IsArray)
            {
                List<object> result = new List<object>();
                foreach (object obj2 in (Array)obj)
                {
                    result.AddRange(SolveArray(obj2));
                }

                return result.ToArray();
            }

            return new object[] { obj };
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            List<object> result = new List<object>();
            foreach (IStatement e in this)
            {
                object obj = !ReferenceEquals(e, null) ? e.Evaluate(context) : null;
                result.AddRange(SolveArray(obj));
            }

            return result.ToArray();
        }
    }
}
