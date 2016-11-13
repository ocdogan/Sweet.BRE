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
    public class DecisionConditionNode : DecisionConditionCell
    {
        private string _variable;

        public DecisionConditionNode()
            : base()
        {
        }

        public DecisionConditionNode(string variable)
            : base()
        {
            SetVariable(variable);
            CheckValues();
        }

        public DecisionConditionNode(string variable, string[] values)
            : base(values)
        {
            SetVariable(variable);
            CheckValues();
        }

        public DecisionConditionNode(DecisionValueType type, string variable, string[] values)
            : base(values)
        {
            SetType(type);
            SetVariable(variable);
            CheckValues();
        }

        public DecisionConditionNode(string variable, DecisionOperation operation, string[] values)
            : base(operation, values)
        {
            SetVariable(variable);
            CheckValues();
        }

        public DecisionConditionNode(DecisionValueType type, string variable, DecisionOperation operation, string[] values)
            : base(operation, values)
        {
            SetType(type);
            SetVariable(variable);
            CheckValues();
        }

        public override DecisionCell Copy()
        {
            return new DecisionConditionNode(Type, Variable, Operation, Values.ToArray());
        }

        public virtual DecisionConditionNode Initialize(string variable, DecisionOperation operation, string[] values)
        {
            SetVariable(variable);
            base.Initialize(operation, values);

            return this;
        }

        public virtual DecisionConditionNode Initialize(DecisionValueType type, string variable, DecisionOperation operation, string[] values)
        {
            SetVariable(variable);
            base.Initialize(type, operation, values);

            return this;
        }

        protected override string GetVariable()
        {
            return _variable;
        }

        internal override void SetContainer(IDecisionContainer container)
        {
        }

        public DecisionConditionNode SetVariable(string variable)
        {
            variable = (variable != null ? variable.Trim() : String.Empty);
            if (String.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException("variable");
            }

            _variable = variable;
            return this;
        }

        protected override bool SupportedByElse(DecisionCell elseDo)
        {
            return (elseDo.GetType() == typeof(DecisionActionNode)) ||
                (elseDo.GetType() == typeof(DecisionConditionNode));
        }

        protected override bool SupportedByOnMatch(DecisionCell onMatch)
        {
            return (onMatch.GetType() == typeof(DecisionActionNode)) ||
                (onMatch.GetType() == typeof(DecisionConditionNode));
        }

        internal override void ValuesChanged()
        {
            CheckValues();
        }

        public override bool IsEqualTo(DecisionCell cell)
        {
            DecisionConditionNode objA = cell as DecisionConditionNode;
            return ReferenceEquals(cell, this) || (!ReferenceEquals(objA, null) && 
                (String.Compare(_variable, objA.Variable) == 0) && 
                base.IsEqualTo(cell));
        }
    }
}
