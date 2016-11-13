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
    public class DecisionConditionCell : DecisionCell
    {
        private DecisionCell _onMatch;
        private DecisionCell _else;

        private DecisionOperation _operation = DecisionOperation.Equals;

        public DecisionConditionCell()
            : base()
        {
        }

        public DecisionConditionCell(string[] values)
            : base(values)
        {
            CheckValues();
        }

        public DecisionConditionCell(DecisionOperation operation, string[] values)
            : base(values)
        {
            SetOperation(operation);
        }

        public override DecisionCellType CellType
        {
            get
            {
                return DecisionCellType.Condition;
            }
        }

        public DecisionCell OnMatch
        {
            get
            {
                return _onMatch;
            }
        }

        public DecisionCell Else
        {
            get
            {
                return _else;
            }
        }

        public override int Level
        {
            get
            {
                int depth = 0;

                DecisionCell parent = base.Parent;
                while (parent != null)
                {
                    depth++;

                    DecisionCell child = parent;
                    parent = parent.Parent;

                    if (parent is DecisionConditionCell)
                    {
                        DecisionConditionCell cond = (DecisionConditionCell)parent;
                        if (ReferenceEquals(cond.Else, child))
                            depth--;
                    }
                }

                return depth;
            }
        }

        public DecisionOperation Operation
        {
            get
            {
                return _operation;
            }
        }

        public string Value
        {
            get
            {
                return (base.Values.Count > 0 ? base.Values[0] : null);
            }
        }

        public override DecisionCell Copy()
        {
            return new DecisionConditionCell(_operation, Values.ToArray());
        }

        public override object Clone()
        {
            DecisionConditionCell result = (DecisionConditionCell)Copy();

            if (_onMatch != null)
            {
                DecisionCell onMatch = (DecisionCell)_onMatch.Clone();
                result.SetOnMatch(onMatch);
            }

            if (_else != null)
            {
                DecisionCell elseDo = (DecisionCell)_else.Clone();
                result.SetElse(elseDo);
            }

            return result;
        }

        protected override int GetMinValueCount()
        {
            if (_operation == DecisionOperation.Between)
            {
                return 2;
            }

            if ((_operation == DecisionOperation.In) ||
                (_operation == DecisionOperation.NotIn))
            {
                return 0;
            }

            return 1;
        }

        protected override int GetMaxValueCount()
        {
            if (_operation == DecisionOperation.Between)
            {
                return 2;
            }

            if ((_operation == DecisionOperation.In) ||
                (_operation == DecisionOperation.NotIn))
            {
                return int.MaxValue;
            }

            return 1;
        }

        public virtual DecisionConditionCell Initialize(DecisionOperation operation, string[] values)
        {
            SetValues(values);
            SetOperation(operation);

            return this;
        }

        public virtual DecisionConditionCell Initialize(DecisionValueType type, DecisionOperation operation, string[] values)
        {
            SetType(type);
            SetValues(values);
            SetOperation(operation);

            return this;
        }

        internal override void SetOwner(IDecision container)
        {
            if (!ReferenceEquals(container, base.Owner))
            {
                base.SetOwner(container);

                if (!ReferenceEquals(_onMatch, null))
                {
                    _onMatch.SetOwner(container);
                }

                if (!ReferenceEquals(_else, null))
                {
                    _else.SetOwner(container);
                }
            }
        }

        internal override void SetParent(DecisionCell parent)
        {
            if (!ReferenceEquals(parent, null) && !(parent is DecisionConditionCell))
            {
                throw new RuleException(BreResStrings.GetString("ConditionCellExpectedAsParent"));
            }

            base.SetParent(parent);
        }

        protected virtual bool SupportedByOnMatch(DecisionCell onMatch)
        {
            return (onMatch.GetType() == this.GetType()) ||
                (onMatch.GetType() == typeof(DecisionActionCell));
        }

        protected virtual bool SupportedByElse(DecisionCell elseDo)
        {
            return (elseDo.GetType() == typeof(DecisionConditionCell));
        }

        private void ValidateOnMatch(DecisionCell onMatch)
        {
            if (!ReferenceEquals(onMatch, null) && !SupportedByOnMatch(onMatch))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("TypeNotSupportedByOnMatch"),
                    onMatch.GetType().Name));
            }
        }

        private void ValidateElse(DecisionCell elseDo)
        {
            if (!ReferenceEquals(elseDo, null) && !SupportedByElse(elseDo))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("TypeNotSupportedByElse"),
                    elseDo.GetType().Name));
            }
        }

        public virtual DecisionConditionCell SetElse(DecisionCell elseDo)
        {
            ValidateElse(elseDo);

            if (!ReferenceEquals(_else, elseDo))
            {
                if (!ReferenceEquals(_else, null))
                {
                    _else.SetContainer(null);
                    _else.SetParent(null);
                    _else.SetOwner(null);
                }

                _else = elseDo;
                if (!ReferenceEquals(_else, null))
                {
                    _else.SetParent(this);
                    _else.SetOwner(Owner);
                    _else.SetContainer(Container);
                }
            }

            return this;
        }

        public virtual DecisionConditionCell SetOnMatch(DecisionCell onMatch)
        {
            ValidateOnMatch(onMatch);

            if (!ReferenceEquals(_onMatch, onMatch))
            {
                if (!ReferenceEquals(_onMatch, null))
                {
                    _onMatch.SetContainer(null);
                    _onMatch.SetParent(null);
                    _onMatch.SetOwner(null);
                }

                _onMatch = onMatch;
                if (!ReferenceEquals(_onMatch, null))
                {
                    _onMatch.SetParent(this);
                    _onMatch.SetOwner(Owner);
                    _onMatch.SetContainer(Container);
                }
            }

            return this;
        }

        public virtual DecisionConditionCell SetOperation(DecisionOperation operation)
        {
            if (_operation != operation)
            {
                _operation = operation;
                CheckValues();
            }

            return this;
        }

        internal override void ValuesChanged()
        {
            CheckValues();
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_else, null))
            {
                _else.Dispose();
                _else = null;
            }

            if (!ReferenceEquals(_onMatch, null))
            {
                _onMatch.Dispose();
                _onMatch = null;
            }

            base.Dispose();
        }

        public override bool IsEqualTo(DecisionCell cell)
        {
            DecisionConditionCell objA = cell as DecisionConditionCell;

            return (this == objA) || ((objA != null) && (_operation == objA.Operation) && 
                base.IsEqualTo((DecisionCell)objA) &&
                ((_onMatch == objA.OnMatch) ||
                ((_onMatch != null) && _onMatch.IsEqualTo(objA.OnMatch))) &&
                ((_else == objA.Else) ||
                ((_else != null) && _else.IsEqualTo(objA.Else))));
        }
    }
}
