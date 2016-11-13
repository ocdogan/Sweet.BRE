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
using System.Collections.ObjectModel;
using System.Text;

namespace Sweet.BRE
{
    public class GenericList<T> : IDisposable, IList, IList<T>,
        ICollection, ICollection<T>, IEnumerable, IEnumerable<T>
        where T : class
    {
        private List<T> _list;
        private bool _disposeList = true;
        private bool _allowMultiInstance = true;
        private bool _isReadOnly = false;

        public GenericList()
            : this(null, true, 0, false)
        {
        }

        public GenericList(int size)
            : this(null, true, size, false)
        {
        }

        public GenericList(bool allowMultiInstance)
            : this(null, allowMultiInstance, 0, false)
        {
        }

        public GenericList(bool allowMultiInstance, bool isReadonly)
            : this(null, allowMultiInstance, 0, isReadonly)
        {
        }

        public GenericList(bool allowMultiInstance, int size, bool isReadonly)
            : this(null, allowMultiInstance, 0, isReadonly)
        {
        }

        private GenericList(List<T> list, bool allowMultiInstance, int size, bool isReadonly)
            : base()
        {
            _isReadOnly = isReadonly;
            _allowMultiInstance = allowMultiInstance;

            _disposeList = (list == null);
            _list = (list != null ? list : new List<T>(size));
        }

        public bool AllowMultiInstance
        {
            get
            {
                return _allowMultiInstance;
            }
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
                return _isReadOnly;
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
                if (_allowMultiInstance || !ContainsItem(item))
                {
                    SetItem(index, item);
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_list).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_list).SyncRoot;
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
                if (_allowMultiInstance || !ContainsItem(item))
                {
                    SetItem(index, item);
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

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)_list).SyncRoot;
            }
        }

        protected virtual T Cast(object item)
        {
            return (item as T);
        }

        protected virtual void SetItem(int index, T value)
        {
            ((IList)_list)[index] = value;
        }

        protected void CheckReadOnly()
        {
            if (_isReadOnly)
            {
                throw new NotSupportedException(CommonResStrings.GetString("ListIsReadonly"));
            }
        }

        int IList.Add(object value)
        {
            int index = _list.Count;
            return (InsertItem(index, Cast(value)) ? index : -1);
        }

        public void Add(T item)
        {
            InsertItem(_list.Count, Cast(item));
        }

        public void AddRange(params T[] items)
        {
            CheckReadOnly();

            if (items != null)
            {
                foreach (T item in items)
                {
                    T item2 = Cast(item);
                    if (_allowMultiInstance || !ContainsItem(item2))
                    {
                        InsertItem(_list.Count, item2);
                    }
                }
            }
        }

        public GenericList<T> AsReadOnly()
        {
            return new GenericList<T>(_list, _allowMultiInstance, 0, _isReadOnly);
        }

        public int BinarySearch(T item)
        {
            return _list.BinarySearch(item);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(index, 0, item, comparer);
        }

        public virtual void Clear()
        {
            RemoveRange(0, _list.Count);
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

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return _list.ConvertAll(converter);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public void CopyTo(T[] array)
        {
            _list.CopyTo(array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            _list.CopyTo(0, array, arrayIndex, count);
        }

        public void CopyTo(int startAt, T[] array, int arrayIndex, int count)
        {
            _list.CopyTo(startAt, array, arrayIndex, count);
        }

        public virtual void Dispose()
        {
            if (_disposeList && (_list != null))
            {
                T[] items = this.ToArray();
                _list.Clear();

                foreach (T item in items)
                {
                    if (!ReferenceEquals(item, null) && (item is IDisposable))
                    {
                        ((IDisposable)item).Dispose();
                    }
                }
            }
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public bool Exists(Predicate<T> match)
        {
            return _list.Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return _list.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, count, match);
        }

        public T FindLast(Predicate<T> match)
        {
            return _list.FindLast(match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return _list.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, count, match);
        }

        public void ForEach(Action<T> action)
        {
            _list.ForEach(action);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        public GenericList<T> GetRange(int index, int count)
        {
            GenericList<T> list = new GenericList<T>();
            list._list.AddRange(_list.GetRange(index, count));

            return list;
        }

        protected virtual int IndexOfItem(T item, int index, int count)
        {
            return _list.IndexOf(Cast(item), index, count);
        }

        protected virtual int LastIndexOfItem(T item, int index, int count)
        {
            return _list.LastIndexOf(Cast(item), index, count);
        }

        int IList.IndexOf(object value)
        {
            return IndexOfItem(Cast(value), 0, _list.Count);
        }

        public int IndexOf(T item)
        {
            return IndexOfItem(Cast(item), 0, _list.Count);
        }

        public int IndexOf(T item, int index)
        {
            return IndexOfItem(Cast(item), index, _list.Count);
        }

        public int IndexOf(T item, int index, int count)
        {
            return IndexOfItem(Cast(item), index, count);
        }

        protected virtual bool InsertItem(int index, T item)
        {
            CheckReadOnly();

            item = Cast(item);
            if (_allowMultiInstance || !ContainsItem(item))
            {
                ((IList)_list).Insert(index, item);
                return true;
            }

            return false;
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, Cast(value));
        }

        public void Insert(int index, T item)
        {
            InsertItem(index, Cast(item));
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            CheckReadOnly();

            lock (this.SyncRoot)
            {
                foreach (T item in collection)
                {
                    InsertItem(index++, item);
                }
            }
        }

        public int LastIndexOf(T item)
        {
            return LastIndexOfItem(Cast(item), 0, _list.Count);
        }

        public int LastIndexOf(T item, int index)
        {
            return LastIndexOfItem(Cast(item), index, _list.Count);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return LastIndexOfItem(Cast(item), index, count);
        }

        void IList.Remove(object value)
        {
            RemoveItem(Cast(value));
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        protected virtual bool RemoveItem(T item)
        {
            CheckReadOnly();
            return ((IList<T>)_list).Remove(Cast(item));
        }

        public void RemoveAt(int index)
        {
            RemoveItem(_list[index]);
        }

        public bool Remove(T item)
        {
            return RemoveItem(Cast(item));
        }

        public int RemoveAll(Predicate<T> match)
        {
            CheckReadOnly();

            int count = 0;
            lock (this.SyncRoot)
            {
                for (int i = _list.Count - 1; i > -1; i--)
                {
                    if (match(_list[i]))
                    {
                        RemoveItem(_list[i]);
                        count++;
                    }
                }
            }

            return count;
        }

        public void RemoveRange(int index, int count)
        {
            CheckReadOnly();

            lock (this.SyncRoot)
            {
                while (count > 0)
                {
                    RemoveItem(_list[index]);
                    count--;
                }
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            CheckReadOnly();
            _list.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            CheckReadOnly();
            _list.Sort(comparison);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            CheckReadOnly();
            _list.Sort(index, count, comparer);
        }

        public void Reverse()
        {
            CheckReadOnly();
            _list.Reverse();
        }

        public void Reverse(int index, int count)
        {
            CheckReadOnly();
            _list.Reverse(index, count);
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[_list.Count];
            Array.Copy(_list.ToArray(), 0, destinationArray, 0, _list.Count);

            return destinationArray;
        }

        public bool TrueForAll(Predicate<T> match)
        {
            return _list.TrueForAll(match);
        }
    }
}
