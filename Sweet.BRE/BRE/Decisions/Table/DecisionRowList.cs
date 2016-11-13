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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public class DecisionRowList : GenericList<DecisionRow>
    {
        private DecisionTable _owner;

        internal DecisionRowList(DecisionTable owner)
            : base(false)
        {
            _owner = owner;
        }

        public DecisionTable Owner
        {
            get
            {
                return _owner;
            }
        }

        public override void Dispose()
        {
            lock (base.SyncRoot)
            {
                _owner = null;
                base.Dispose();
            }
        }

        public DecisionRow Add(bool addCells)
        {
            if (!addCells)
            {
                DecisionRow result = new DecisionRow();
                base.Add(result);

                return result;
            }

            return Add(null, null);
        }

        private void UpdateRow(DecisionRow result, string[] conditions, string[] actions)
        {
            conditions = ((conditions != null) ? conditions : new string[0]);
            actions = ((actions != null) ? actions : new string[0]);

            if (_owner != null)
            {
                DecisionConditionCell lastCond = null;

                int i = 0;
                IEnumerator<DecisionColumn> en1 = _owner.Conditions.GetEnumerator();

                while (en1.MoveNext())
                {
                    string value = (i < conditions.Length) ? conditions[i++] : null;

                    DecisionConditionCell condition = new DecisionConditionCell();
                    condition.SetValues(new string[] { value });

                    if (lastCond == null)
                    {
                        result.SetRoot(condition);
                    }
                    else
                    {
                        lastCond.SetOnMatch(condition);
                    }

                    lastCond = condition;
                }

                DecisionActionCell lastAction = null;

                i = 0;
                IEnumerator<DecisionColumn> en2 = _owner.Actions.GetEnumerator();

                while (en2.MoveNext())
                {
                    string value = (i < actions.Length) ? actions[i++] : null;

                    DecisionActionCell action = new DecisionActionCell();
                    action.SetValue(value);

                    if (lastAction != null)
                    {
                        lastAction.SetFurtherMore(action);
                    }
                    else
                        if (lastCond == null)
                        {
                            result.SetRoot(action);
                        }
                        else
                        {
                            lastCond.SetOnMatch(action);
                        }

                    lastAction = action;
                }
            }
        }

        public DecisionRow Add(string[] conditions, string[] actions)
        {
            DecisionRow result = new DecisionRow();
            base.Add(result);

            UpdateRow(result, conditions, actions);

            return result;
        }

        protected override bool RemoveItem(DecisionRow item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            bool result = false;
            lock (base.SyncRoot)
            {
                result = base.RemoveItem(item);

                item.SetOwner(null);
                item.SetRoot(null);
            }

            return result;
        }

        protected override bool InsertItem(int index, DecisionRow item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            bool result = false;
            lock (base.SyncRoot)
            {
                if (!ReferenceEquals(item.Owner, null))
                {
                    item.Owner.Rows.Remove(item);
                }

                result = base.InsertItem(index, item);
                item.SetOwner(_owner);
            }

            return result;
        }

        protected override bool ContainsItem(DecisionRow item)
        {
            lock (base.SyncRoot)
            {
                if (item == null)
                {
                    for (int j = 0; j < base.Count; j++)
                    {
                        if (base[j] == null)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                for (int i = 0; i < base.Count; i++)
                {
                    if (ReferenceEquals(base[i], item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        protected bool EqualList(DecisionRowList list)
        {
            lock (base.SyncRoot)
            {
                if ((list != null) && (list.Count == base.Count))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DecisionRow item1 = base[i];
                        DecisionRow item2 = list[i];

                        if (!object.Equals(item1, item2))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            DecisionRowList objA = obj as DecisionRowList;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) && EqualList(objA));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
