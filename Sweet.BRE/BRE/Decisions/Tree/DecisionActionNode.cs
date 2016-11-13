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
    public sealed class DecisionActionNode : DecisionActionCell
    {
        private string _variable;

        public DecisionActionNode()
            : base()
        {
        }

        public DecisionActionNode(string variable)
            : base()
        {
            SetVariable(variable);
        }

        public DecisionActionNode(string variable, string value)
            : base(value)
        {
            SetVariable(variable);
        }

        public DecisionActionNode(DecisionValueType type, string variable, string value)
            : base(value)
        {
            SetType(type);
            SetVariable(variable);
        }

        public DecisionActionNode Initialize(DecisionValueType type, string variable, string value)
        {
            SetVariable(variable);
            base.Initialize(type, value);

            return this;
        }

        public override DecisionCell Copy()
        {
            return new DecisionActionNode(Type, Variable, Value);
        }

        protected override string GetVariable()
        {
            return _variable;
        }

        internal override void SetContainer(IDecisionContainer container)
        {
        }

        public DecisionActionNode SetVariable(string variable)
        {
            variable = (variable != null ? variable.Trim() : String.Empty);
            if (String.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException("variable");
            }

            _variable = variable;
            return this;
        }

        public override bool IsEqualTo(DecisionCell cell)
        {
            DecisionActionNode objA = cell as DecisionActionNode;
            return ReferenceEquals(cell, this) || (!ReferenceEquals(objA, null) &&
                (String.Compare(_variable, objA.Variable) == 0) &&
                base.IsEqualTo(cell));
        }
    }
}
