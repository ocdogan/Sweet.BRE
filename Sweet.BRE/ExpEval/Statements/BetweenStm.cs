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
    public class BetweenStm : BooleanStm
    {
        private Statement _max = Statement.Null;
        private Statement _min = Statement.Null;
        private Statement _target = Statement.Null;

        public BetweenStm(Statement target, Statement min, Statement max) 
            : base(true)
        {
            if (ReferenceEquals(target, null))
            {
                throw new ArgumentNullException("target");
            }

            if (ReferenceEquals(min, null))
            {
                throw new ArgumentNullException("min");
            }

            if (ReferenceEquals(max, null))
            {
                throw new ArgumentNullException("max");
            }
            
            _min = min;
            _max = max;
            _target = target;
        }

        public Statement Max
        {
            get
            {
                return _max;
            }
        }

        public Statement Min
        {
            get
            {
                return _min;
            }
        }

        public Statement Target
        {
            get
            {
                return _target;
            }
        }

        public static BetweenStm As(Statement target, Statement min, Statement max)
        {
            return (new BetweenStm(target, min, max));
        }

        public override object Clone()
        {
            Statement min = (ReferenceEquals(_min, null) ? null : (Statement)_min.Clone());
            Statement max = (ReferenceEquals(_max, null) ? null : (Statement)_max.Clone());
            Statement target = (ReferenceEquals(_target, null) ? null : (Statement)_target.Clone());

            return BetweenStm.As(target, min, max);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_min, null))
            {
                _min.Dispose();
                _min = null;
            }

            if (!ReferenceEquals(_max, null))
            {
                _max.Dispose();
                _max = null;
            }

            if (!ReferenceEquals(_target, null))
            {
                _target.Dispose();
                _target = null;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            BetweenStm objA = obj as BetweenStm;
            return ReferenceEquals(this, objA) || (!ReferenceEquals(objA, null) && 
                object.Equals(_max, objA.Max) && 
                object.Equals(_min, objA.Min) && 
                object.Equals(_target, objA.Target));
        }

        public override int GetHashCode()
        {
            int minHash = (!ReferenceEquals(_min, null) ? _min.GetHashCode() : 0);
            int maxHash = (!ReferenceEquals(_max, null) ? _max.GetHashCode() : 0);
            int targetHash = (!ReferenceEquals(_target, null) ? _target.GetHashCode() : 0);

            return ((targetHash ^ maxHash) ^ minHash);
        }

        public override string ToString()
        {
            return String.Format("{0} BETWEEN {1} AND {2}", _target.ToString(), _min.ToString(), _max.ToString());
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object min = !ReferenceEquals(_target, null) ? ((IStatement)_min).Evaluate(context) : null;
            object max = !ReferenceEquals(_target, null) ? ((IStatement)_max).Evaluate(context) : null;
            object value = !ReferenceEquals(_target, null) ? ((IStatement)_target).Evaluate(context) : null;

            return StmCommon.Between(value, min, max);
        }
    }
}
