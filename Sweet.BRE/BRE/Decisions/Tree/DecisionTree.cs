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
    public sealed class DecisionTree : DecisionConditionNode, IStatement, IDecision
    {
        internal DecisionTree()
            : base()
        {
        }

        public override DecisionCell Copy()
        {
            DecisionTree result = new DecisionTree();
            result.Initialize(Type, Variable, Operation, Values.ToArray());

            return result;
        }

        object IStatement.Evaluate(IEvaluationContext context, params object[] args)
        {
            bool result = false;
            using (TreeEvaluator e = new TreeEvaluator())
            {
                result = e.Evaluate(context, this);
            }

            return result;
        }

        internal override void SetOwner(IDecision owner)
        {
        }

        public override DecisionConditionCell SetOnMatch(DecisionCell onMatch)
        {
            if (!ReferenceEquals(onMatch, base.OnMatch))
            {
                if (!ReferenceEquals(base.OnMatch, null))
                {
                    base.OnMatch.SetOwner(null);
                }

                base.SetOnMatch(onMatch);
                if (!ReferenceEquals(onMatch, null))
                {
                    onMatch.SetOwner(this);
                }
            }

            return this;
        }

        public override DecisionConditionCell SetElse(DecisionCell elseDo)
        {
            if (!ReferenceEquals(elseDo, base.Else))
            {
                if (!ReferenceEquals(base.Else, null))
                {
                    base.Else.SetOwner(null);
                }

                base.SetElse(elseDo);
                if (!ReferenceEquals(elseDo, null))
                {
                    elseDo.SetOwner(this);
                }
            }

            return this;
        }
    }
}
