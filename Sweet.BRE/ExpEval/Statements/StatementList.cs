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
    public abstract class StatementList<T> : Statement, IDisposable, IList, IList<T>,
        ICollection, ICollection<T>, IEnumerable, IEnumerable<T>
        where T : Statement
    {
        private bool _multiInstance = true;
        private List<T> _list = new List<T>();

        protected StatementList(bool multiInstance)
            : base()
        {
            _multiInstance = multiInstance;
        }

        bool IList.IsFixedSize
        {
            get
            {
                return ((IList)_list).IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return ((IList)_list).IsReadOnly;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return ((IList)_list).IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)_list)[index];
            }
            set
            {
                T item = Cast(value);
                if (_multiInstance || !ContainsItem(item))
                {
                    ((IList)_list)[index] = item;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                return _list[index] as T;
            }
            set
            {
                T item = Cast(value);
                if (_multiInstance || !ContainsItem(item))
                {
                    ((IList)_list)[index] = item;
                }
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_list).IsSynchronized;
            }
        }

        public bool MultiInstance
        {
            get
            {
                return _multiInstance;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_list).SyncRoot;
            }
        }

        protected virtual T Cast(object item)
        {
            if ((item == null) && (typeof(T) == typeof(Statement)))
            {
                item = Statement.Null;
            }

            return (item as T);
        }

        protected virtual int AddItem(T item)
        {
            item = Cast(item);
            if (_multiInstance || !ContainsItem(item))
            {
                return ((IList)_list).Add(item);
            }

            return -1;
        }

        int IList.Add(object value)
        {
            return AddItem(Cast(value));
        }

        public void Add(T item)
        {
            AddItem(Cast(item));
        }

        public void AddRange(params T[] items)
        {
            if (items != null)
            {
                foreach (T item in items)
                {
                    T item2 = Cast(item);
                    if (_multiInstance || !ContainsItem(item2))
                    {
                        AddItem(item2);
                    }
                }
            }
        }

        public virtual void Clear()
        {
            _list.Clear();
        }

        protected virtual bool ContainsItem(T item)
        {
            return ((IList)_list).Contains(Cast(item));
        }

        bool IList.Contains(object value)
        {
            return ContainsItem(Cast(value));
        }

        public bool Contains(T item)
        {
            return ContainsItem(Cast(item));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public override void Dispose()
        {
            if (_list != null)
            {
                T[] items = this.ToArray();
                _list.Clear();

                foreach (T item in items)
                {
                    if (!ReferenceEquals(item, null))
                    {
                        item.Dispose();
                    }
                }
            }

            base.Dispose();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        protected virtual int IndexOfItem(T item)
        {
            return ((IList)_list).IndexOf(Cast(item));
        }

        int IList.IndexOf(object value)
        {
            return IndexOfItem(Cast(value));
        }

        public int IndexOf(T item)
        {
            return IndexOfItem(Cast(item));
        }

        protected virtual void InsertItem(int index, T item)
        {
            item = Cast(item);
            if (_multiInstance || !ContainsItem(item))
            {
                ((IList)_list).Insert(index, item);
            }
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, Cast(value));
        }

        public void Insert(int index, T item)
        {
            InsertItem(index, Cast(item));
        }

        protected virtual bool RemoveItem(T item)
        {
            return ((IList<T>)_list).Remove(Cast(item));
        }

        void IList.Remove(object value)
        {
            RemoveItem(Cast(value));
        }

        public virtual bool Remove(T item)
        {
            return RemoveItem(Cast(item));
        }

        public virtual void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[_list.Count];
            Array.Copy(_list.ToArray(), 0, destinationArray, 0, _list.Count);

            return destinationArray;
        }
    }
}
