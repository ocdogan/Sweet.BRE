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
    public class DecisionActionCell : DecisionCell
    {
        private DecisionActionCell _furtherMore;

        public DecisionActionCell()
            : base()
        {
        }

        public DecisionActionCell(string value)
            : base(new string[] { value })
        {
        }

        public override DecisionCellType CellType
        {
            get
            {
                return DecisionCellType.Action;
            }
        }

        public DecisionActionCell FurtherMore
        {
            get
            {
                return _furtherMore;
            }
        }

        public override int Level
        {
            get
            {
                int depth = 0;

                DecisionCell parent = base.Parent;
                while (parent is DecisionActionCell)
                {
                    depth++;
                    parent = parent.Parent;
                }

                return depth;
            }
        }

        public string Value
        {
            get
            {
                return (base.Values.Count > 0 ? base.Values[0] : null);
            }
        }

        public DecisionActionCell Initialize(DecisionValueType type, string value)
        {
            SetType(type);
            SetValue(value);

            return this;
        }

        internal override void SetOwner(IDecision container)
        {
            if (!ReferenceEquals(container, base.Owner))
            {
                base.SetOwner(container);
                if (!ReferenceEquals(_furtherMore, null))
                {
                    _furtherMore.SetOwner(container);
                }
            }
        }

        protected virtual bool SupportedByFurtherMore(DecisionCell furtherMore)
        {
            return (furtherMore.GetType() == this.GetType());
        }

        private void ValidateFurtherMore(DecisionCell furtherMore)
        {
            if (!ReferenceEquals(furtherMore, null) && !SupportedByFurtherMore(furtherMore))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("UnsupportedTypeByFurtherMore"),
                    furtherMore.GetType().Name));
            }
        }

        public virtual DecisionActionCell SetFurtherMore(DecisionActionCell furtherMore)
        {
            ValidateFurtherMore(furtherMore);

            if (!ReferenceEquals(_furtherMore, furtherMore))
            {
                if (!ReferenceEquals(_furtherMore, null))
                {
                    _furtherMore.SetContainer(null);
                    _furtherMore.SetParent(null);
                    _furtherMore.SetOwner(null);
                }

                _furtherMore = furtherMore;
                if (!ReferenceEquals(_furtherMore, null))
                {
                    _furtherMore.SetParent(this);
                    _furtherMore.SetOwner(Owner);
                    _furtherMore.SetContainer(Container);
                }
            }

            return this;
        }

        public DecisionActionCell SetValue(string value)
        {
            SetValues(new string[] { value });
            return this;
        }

        public override DecisionCell SetValues(string[] values)
        {
            base.Values.Clear();
            if ((values != null) && (values.Length > 0))
            {
                base.Values.Add(values[0]);
            }

            return this;
        }

        public override DecisionCell Copy()
        {
            return new DecisionActionCell(Value);
        }

        public override object Clone()
        {
            DecisionActionCell result = (DecisionActionCell)Copy();

            if (!ReferenceEquals(_furtherMore, null))
            {
                DecisionActionCell furtherMore = (DecisionActionCell)_furtherMore.Clone();
                result.SetFurtherMore(furtherMore);
            }

            return result;
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_furtherMore, null))
            {
                _furtherMore.Dispose();
                _furtherMore = null;
            }

            base.Dispose();
        }

        public override bool IsEqualTo(DecisionCell cell)
        {
            DecisionActionCell objA = cell as DecisionActionCell;
            
            return (this == objA) || ((objA != null) && base.IsEqualTo((DecisionCell)objA) && 
                ((_furtherMore == objA.FurtherMore) || 
                ((_furtherMore != null) && _furtherMore.IsEqualTo(objA.FurtherMore))));
        }
    }
}
