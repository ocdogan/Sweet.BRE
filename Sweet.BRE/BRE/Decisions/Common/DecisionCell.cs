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
    public abstract class DecisionCell : IDisposable, ICloneable
    {
        private IDecision _owner;
        private IDecisionContainer _container;

        private DecisionValueList _values;
        private DecisionValueType _type = DecisionValueType.String;

        private DecisionCell _parent;

        protected DecisionCell()
        {
            _values = new DecisionValueList(this);
        }

        protected DecisionCell(string[] values)
            : this()
        {
            SetValues(values);
        }

        public virtual DecisionCellType CellType
        {
            get
            {
                return DecisionCellType.Undefined;
            }
        }

        public IDecisionContainer Container
        {
            get
            {
                return _container;
            }
        }

        public virtual int Level
        {
            get
            {
                return 0;
            }
        }

        public int MinValueCount
        {
            get
            {
                int result = GetMinValueCount();
                result = (result < 0 ? 0 : result);

                return result;
            }
        }

        public int MaxValueCount
        {
            get
            {
                int result = GetMaxValueCount();
                result = (result < 0 ? 0 : result);

                return result;
            }
        }

        public IDecision Owner
        {
            get
            {
                return _owner;
            }
        }

        public DecisionCell Parent
        {
            get
            {
                return _parent;
            }
        }

        public DecisionValueType Type
        {
            get
            {
                return GetValueType();
            }
        }

        public DecisionValueList Values
        {
            get
            {
                return _values;
            }
        }

        public string Variable
        {
            get
            {
                return GetVariable();
            }
        }

        public virtual void Dispose()
        {
            SetParent(null);
            SetOwner(null);
            SetContainer(null);

            _values.Clear();
        }

        protected virtual void CheckValues()
        {
            int maxCount = MaxValueCount;
            if (_values.Count > maxCount)
            {
                _values.RemoveRange(maxCount, _values.Count - maxCount);
            }
        }

        public virtual DecisionCell Copy()
        {
            return null;
        }

        public virtual object Clone()
        {
            return Copy();
        }

        protected virtual int GetMinValueCount()
        {
            return 1;
        }

        protected virtual int GetMaxValueCount()
        {
            return 1;
        }

        protected virtual DecisionValueType GetValueType()
        {
            return _type;
        }

        protected virtual string GetVariable()
        {
            return null;
        }

        internal virtual void SetContainer(IDecisionContainer container)
        {
            _container = container;
        }

        internal virtual void SetOwner(IDecision owner)
        {
            _owner = owner;
        }

        internal virtual void SetParent(DecisionCell parent)
        {
            if (!ReferenceEquals(parent, null))
            {
                if (ReferenceEquals(parent, this))
                {
                    throw new RuleException(BreResStrings.GetString("CannotSetItselfAsParent"));
                }

                DecisionCell aParent = parent.Parent;
                while (aParent != null)
                {
                    if (ReferenceEquals(aParent, this))
                    {
                        throw new RuleException(BreResStrings.GetString("CannotSetChildCellAsParent"));
                    }

                    aParent = aParent.Parent;
                }
            }

            _parent = parent;
        }

        public virtual DecisionCell SetType(DecisionValueType type)
        {
             _type = type;
             return this;
        }

        public virtual DecisionCell SetValues(string[] values)
        {
            _values.Clear();
            if (values != null)
            {
                _values.AddRange(values);
            }

            return this;
        }

        internal virtual void ValuesChanged()
        {
        }

        public virtual bool IsEqualTo(DecisionCell cell)
        {
            return ReferenceEquals(cell, this) || (!ReferenceEquals(cell, null) &&
                ReferenceEquals(GetType(), cell.GetType()) &&
                (_type == cell.Type) && object.Equals(_values, cell.Values));
        }
    }
}
