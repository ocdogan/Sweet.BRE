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
    public class NumericBinaryStm : NumericStm
    {
        private string _op;

        private Statement _left = Statement.Null;
        private Statement _right = Statement.Null;

        public NumericBinaryStm(Statement left, Statement right)
            : base(0d)
        {
            _left = left;
            _right = right;
        }

        public Statement Left
        {
            get
            {
                if (ReferenceEquals(_left, null))
                {
                    _left = Statement.Null;
                }

                return _left;
            }
        }

        public Statement Right
        {
            get
            {
                if (ReferenceEquals(_right, null))
                {
                    _right = Statement.Null;
                }

                return _right;
            }
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_left, null))
            {
                _left.Dispose();
                _left = null;
            }

            if (!ReferenceEquals(_right, null))
            {
                _right.Dispose();
                _right = null;
            }
        }

        public override bool Equals(object obj)
        {
            NumericBinaryStm objA = obj as NumericBinaryStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_left, objA.Left) &&
                object.Equals(_right, objA.Right));
        }

        public override int GetHashCode()
        {
            int leftHash = (!ReferenceEquals(_left, null) ? _left.GetHashCode() : 0);
            int rightHash = (!ReferenceEquals(_right, null) ? _right.GetHashCode() : 0);

            return (leftHash ^ rightHash);
        }

        public virtual string GetOperator()
        {
            return _op;
        }

        protected void SetOperator(string value)
        {
            _op = (value != null ? value.Trim() : null);
        }

        public override string ToString()
        {
            string op = GetOperator();
            op = (op != null ? op.Trim() : String.Empty);

            string left = StmCommon.PrepareToString(_left);
            string right = StmCommon.PrepareToString(_right);

            return String.Format("({0} {1} {2})", left, op, right);
        }

        protected virtual object Evaluate(object o1, object o2)
        {
            return double.NaN;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            IStatement l = _left;
            IStatement r = _right;

            object o1 = !ReferenceEquals(l, null) ? l.Evaluate(context) : null;
            object o2 = !ReferenceEquals(r, null) ? r.Evaluate(context) : null;

            return Evaluate(o1, o2);
        }
    }
}
