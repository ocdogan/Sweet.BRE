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
    public sealed class OrStm : LogicalStm
    {
        public OrStm(BooleanStm left, BooleanStm right)
            : base(StmConstants.OP_OR, left, right)
        {
        }

        public override object Clone()
        {
            BooleanStm l = ((object)base.Left != null ? (BooleanStm)base.Left.Clone() : null);
            BooleanStm r = ((object)base.Right != null ? (BooleanStm)base.Right.Clone() : null);

            return (new OrStm(l, r));
        }

        public static OrStm As(BooleanStm left, BooleanStm right)
        {
            return (new OrStm(left, right));
        }

        protected override bool Evaluate(object o1, object o2)
        {
            bool b1 = StmCommon.ToBoolean(o1, false);
            bool b2 = StmCommon.ToBoolean(o2, false);

            return b1 || b2;
        }
    }
}
