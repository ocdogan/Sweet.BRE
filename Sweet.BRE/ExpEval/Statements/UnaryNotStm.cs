﻿/*
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
    public sealed class UnaryNotStm : BooleanStm
    {
        private Statement _target = Statement.Null;

        public UnaryNotStm(BooleanStm target)
            : base(true)
        {
            _target = target;
            if (ReferenceEquals(_target, null))
            {
                _target = BooleanStm.True;
            }
        }

        public Statement Target
        {
            get
            {
                if (ReferenceEquals(_target, null))
                {
                    _target = BooleanStm.True;
                }

                return _target;
            }
        }

        public static UnaryNotStm As(BooleanStm target)
        {
            return new UnaryNotStm(target);
        }

        public override object Clone()
        {
            object target = (_target is ICloneable) ? ((ICloneable)_target).Clone() : _target;
            return UnaryNotStm.As(target as BooleanStm);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_target, null))
            {
                _target.Dispose();
                _target = null;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            UnaryNotStm objA = obj as UnaryNotStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_target, objA.Target));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("NOT {0}",
                (ReferenceEquals(_target, null) ? String.Empty : _target.ToString()));
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            IStatement t = _target;
            object o1 = !ReferenceEquals(t, null) ? t.Evaluate(context) : null;

            return !StmCommon.ToBoolean(o1, false);
        }
    }
}
