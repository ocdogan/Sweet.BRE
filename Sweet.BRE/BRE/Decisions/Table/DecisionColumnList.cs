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
    public class DecisionColumnList : GenericList<DecisionColumn> 
    {
        private DecisionTable _owner;
        private Dictionary<string, DecisionColumn> _items;

        internal DecisionColumnList(DecisionTable owner)
            : base(false)
        {
            _owner = owner;
            _items = new Dictionary<string, DecisionColumn>(StringComparer.OrdinalIgnoreCase);
        }

        public DecisionColumn this[string name]
        {
            get
            {
                return _items[NormalizeName(name)];
            }
        }

        public DecisionTable Owner
        {
            get
            {
                return _owner;
            }
        }

        public DecisionColumn Add(string name)
        {
            return Add(name, null, DecisionValueType.String);
        }

        public DecisionColumn Add(string name, DecisionValueType type)
        {
            return Add(name, null, type);
        }

        public DecisionColumn Add(string name, string description, DecisionValueType type)
        {
            name = (name != null ? name.Trim() : String.Empty);
            if (_items.ContainsKey(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnAlreadyExists"), name));
            }

            DecisionColumn result = new DecisionColumn();

            result.SetName(name);
            result.SetDescription(description);
            result.SetType(type);

            base.Add(result);

            return result;
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        public override void Clear()
        {
            lock (base.SyncRoot)
            {
                DecisionColumn[] items = base.ToArray();
                base.Clear();

                foreach (DecisionColumn item in items)
                {
                    if (!ReferenceEquals(item, null))
                    {
                        ((IDisposable)item).Dispose();
                    }
                }
            }
        }

        public override void Dispose()
        {
            lock (base.SyncRoot)
            {
                _owner = null;
                _items.Clear();

                base.Dispose();
            }
        }

        public void ChangeName(string currName, string newName)
        {
            currName = NormalizeName(currName);
            newName = NormalizeName(newName);

            if (currName != newName)
            {
                if (!_items.ContainsKey(currName))
                {
                    throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnDoesNotExists"), currName));
                }

                if (_items.ContainsKey(newName))
                {
                    throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnAlreadyExists"), newName));
                }

                DecisionColumn item = _items[currName];
                if (!ReferenceEquals(item, null))
                {
                    _items.Remove(currName);
                    item.SetName(newName);

                    _items[newName] = item;
                }
            }
        }

        public void ChangeType(string name, DecisionValueType newType)
        {
            name = NormalizeName(name);

            if (!_items.ContainsKey(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnDoesNotExists"), name));
            }

            DecisionColumn item = _items[name];
            if (!ReferenceEquals(item, null))
            {
                item.SetType(newType);
            }
        }

        public int IndexOf(string name)
        {
            name = NormalizeName(name);
            if (_items.ContainsKey(name))
            {
                DecisionColumn item = _items[name];
                return base.IndexOf(item);
            }

            return -1;
        }

        public virtual bool Remove(string name)
        {
            name = NormalizeName(name);
            if (_items.ContainsKey(name))
            {
                DecisionColumn item = _items[name];
                return RemoveItem(item);
            }

            return false;
        }

        protected virtual void AfterRemove(DecisionColumn item)
        {
        }

        protected virtual void BeforeRemove(DecisionColumn item)
        {
        }

        protected override bool RemoveItem(DecisionColumn item)
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }

            bool result = false;
            lock (base.SyncRoot)
            {
                BeforeRemove(item);
                try
                {
                    result = base.RemoveItem(item);
                    item.SetOwner(null);

                    string name = item.Name;
                    if (_items.ContainsKey(name))
                    {
                        _items.Remove(name);
                    }
                }
                finally
                {
                    if (result)
                    {
                        AfterRemove(item);
                    }
                }
            }

            return result;
        }

        protected virtual void AfterInsert(DecisionColumn item)
        {
        }

        protected virtual void BeforeInsert(DecisionColumn item)
        {
        }

        protected override bool InsertItem(int index, DecisionColumn item)
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }

            bool result = false;
            lock (base.SyncRoot)
            {
                string name = item.Name;
                if (_items.ContainsKey(name))
                {
                    throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnAlreadyExists"), name));
                }

                BeforeInsert(item);
                try
                {
                    result = base.InsertItem(index, item);
                    item.SetOwner(_owner);

                    _items[name] = item;
                }
                finally
                {
                    if (result)
                    {
                        AfterInsert(item);
                    }
                }
            }

            return result;
        }

        public DecisionColumn Get(string name)
        {
            if (_items.ContainsKey(name))
            {
                return _items[name];
            }

            return null;
        }

        public bool Contains(string name)
        {
            return _items.ContainsKey(name);
        }

        protected override bool ContainsItem(DecisionColumn item)
        {
            lock (base.SyncRoot)
            {
                if (ReferenceEquals(item, null))
                {
                    for (int j = 0; j < base.Count; j++)
                    {
                        if (ReferenceEquals(base[j], null))
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

        protected bool EqualList(DecisionColumnList list)
        {
            lock (base.SyncRoot)
            {
                if ((list != null) && (list.Count == base.Count))
                {
                    foreach (DecisionColumn item1 in list)
                    {
                        DecisionColumn item2 = this[item1.Name];
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
            DecisionColumnList objA = obj as DecisionColumnList;
            return ReferenceEquals(objA, this) || (!ReferenceEquals(objA, null) && EqualList(objA));
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
