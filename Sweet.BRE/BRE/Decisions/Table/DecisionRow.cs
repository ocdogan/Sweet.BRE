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
    public sealed class DecisionRow : IDisposable, ICloneable, IDecisionContainer
    {
        private DecisionTable _owner;
        private DecisionCell _root;

        internal DecisionRow()
            : base()
        {
        }

        public int Index
        {
            get
            {
                if (!ReferenceEquals(_owner, null))
                {
                    return _owner.Rows.IndexOf(this);
                }

                return -1;
            }
        }

        public DecisionTable Owner
        {
            get
            {
                return _owner;
            }
        }

        public DecisionCell Root
        {
            get
            {
                return _root;
            }
        }

        public object Clone()
        {
            DecisionRow row = new DecisionRow();
            if (!ReferenceEquals(_root, null))
            {
                DecisionCell root = (DecisionCell)_root.Clone();
                row.SetRoot(root);
            }

            return row;
        }

        public void Dispose()
        {
            SetOwner(null);
            if (!ReferenceEquals(_root, null))
            {
                _root.Dispose();
                SetRoot(null);
            }
        }

        internal void SetOwner(DecisionTable owner)
        {
            if (!ReferenceEquals(_owner, owner))
            {
                _owner = owner;
                if (!ReferenceEquals(_root, null))
                {
                    _root.SetOwner(owner);
                }
            }
        }

        private bool SupportedByRoot(DecisionCell root)
        {
            return (root.GetType() == typeof(DecisionConditionCell)) ||
                (root.GetType() == typeof(DecisionActionCell));
        }

        private void ValidateRoot(DecisionCell root)
        {
            if (!ReferenceEquals(root, null) && !SupportedByRoot(root))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("TypeNotSupportedByRoot"),
                    root.GetType().Name));
            }
        }

        public void SetRoot(DecisionCell root)
        {
            ValidateRoot(root);

            if (!ReferenceEquals(_root, root))
            {
                if (!ReferenceEquals(_root, null))
                {
                    _root.SetParent(null);
                    _root.SetOwner(null);
                    _root.SetContainer(null);
                }

                _root = root;
                if (!ReferenceEquals(_root, null))
                {
                    _root.SetParent(null);
                    _root.SetOwner(_owner);
                    _root.SetContainer(this);
                }
            }
        }

        internal bool EvaluateNode(IEvaluationContext context, params object[] args)
        {
            return false;
        }

        public bool IsEqualTo(object obj)
        {
            DecisionRow objA = obj as DecisionRow;
            return ReferenceEquals(obj, this) || ((objA != null) &&
                ((_root == objA.Root) || ((_root != null) && _root.IsEqualTo(objA.Root))));
        }
    }
}
