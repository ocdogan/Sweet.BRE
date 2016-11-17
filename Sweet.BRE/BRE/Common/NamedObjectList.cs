using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sweet.BRE
{
    public class NamedObjectList<T> : INamedObjectList, ICloneable, IDisposable
        where T : class, INamedObject
    {
        private string[] _objectNames;
        private T[] _objectArray;
        private List<T> _objectList;
        private Dictionary<string, T> _objectIndex;

        private readonly object _syncLock = new object();

        public event ObjectEvent OnRemove;
        public event ObjectEvent OnDispose;
        public event ObjectNamedEvent OnAdd;
        public event ObjectEvent OnValidate;
        public event ObjectNamedEvent OnValidateName;

        public NamedObjectList()
        {
            _objectList = new List<T>();
            _objectIndex = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        internal object SyncLock
        {
            get { return _syncLock; }
        }

        public int Count
        {
            get 
            {
                lock (_syncLock)
                {
                    return _objectIndex.Count;
                }
            }
        }

        public string[] Names
        {
            get
            {
                if (_objectNames == null)
                {
                    lock (_syncLock)
                    {
                        if (_objectNames == null)
                        {
                            string[] names = new string[_objectIndex.Count];
                            _objectIndex.Keys.CopyTo(names, 0);

                            _objectNames = names;
                        }
                    }
                }
                return _objectNames;
            }
        }

        public T[] Objects
        {
            get
            {
                if (_objectArray == null)
                {
                    lock (_syncLock)
                    {
                        if (_objectArray == null)
                        {
                            _objectList.Sort(delegate(T x, T y)
                            {
                                if (ReferenceEquals(x, y)) return 0;
                                if (ReferenceEquals(x, null)) return 1;
                                if (ReferenceEquals(y, null)) return -1;

                                return x.CompareTo(y);
                            });

                            _objectArray = _objectList.ToArray();
                        }
                    }
                }
                return _objectArray;
            }
        }

        public T this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Add(name, value);
            }
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        private string ValidateName(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return name;
        }

        private void ValidateName(T obj, string name)
        {
            name = ValidateName(name);
            if (OnValidateName != null)
            {
                OnValidateName(this, obj, name);
            }
        }

        private void Validate(T obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("obj");
            }

            if (OnValidate != null)
            {
                OnValidate(this, obj);
            }

            if (ReferenceEquals(obj.List, this))
            {
                throw new RuleException(BreResStrings.GetString("ObjectAlreadyExistsInList"));
            }

            string name = NormalizeName(obj.Name);
            if (((INamedObjectList)this).Contains(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("ObjectWithSameNameAlreadyExistsInList"), name));
            }
        }

        public object Clone()
        {
            NamedObjectList<T> result = Activator.CreateInstance(this.GetType()) as NamedObjectList<T>;
            if (result != null)
            {
                lock (_syncLock)
                {
                    foreach (T obj in _objectIndex.Values)
                    {
                        if (obj is ICloneable)
                        {
                            ((INamedObjectList)result).Add(obj.Name, (T)((ICloneable)obj).Clone());
                        }
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            lock (_syncLock)
            {
                // rules
                T[] objects = _objectList.ToArray();

                _objectArray = null;
                _objectNames = null;

                _objectIndex.Clear();
                _objectList.Clear();

                if (objects.Length > 0)
                {
                    foreach (T obj in objects)
                    {
                        if (!ReferenceEquals(obj, null) && (OnDispose != null))
                        {
                            OnDispose(this, obj);
                        }
                    }
                }
            }
        }

        public void Add(string name, T obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("obj");
            }

            Validate(obj);

            name = NormalizeName(name);
            ValidateName(obj, name);

            if (!ReferenceEquals(obj.List, null) && (obj is ICloneable))
            {
                obj = (T)((ICloneable)obj).Clone();
            }

            lock (_syncLock)
            {
                _objectArray = null;
                _objectNames = null;

                T old;
                _objectIndex.TryGetValue(name, out old);

                if (!ReferenceEquals(old, null) && !ReferenceEquals(old, obj))
                {
                    _objectList.Remove(old);
                    if (OnRemove != null)
                    {
                        OnRemove(this, old);
                    }
                }

                _objectIndex[name] = obj;
                _objectList.Add(obj);

                if (OnAdd != null)
                {
                    OnAdd(this, obj, name);
                }
            }
        }

        public void Clear()
        {
            lock (_syncLock)
            {
                // rules
                T[] objects = _objectList.ToArray();

                _objectArray = null;
                _objectNames = null;

                _objectIndex.Clear();
                _objectList.Clear();

                if (objects.Length > 0)
                {
                    foreach (T obj in objects)
                    {
                        if (!ReferenceEquals(obj, null) && (OnRemove != null))
                        {
                            OnRemove(this, obj);
                        }
                    }
                }
            }
        }

        public bool Contains(string name)
        {
            name = NormalizeName(name);
            if (!String.IsNullOrEmpty(name))
            {
                lock (_syncLock)
                {
                    return _objectIndex.ContainsKey(name);
                }
            }
            return false;
        }

        public int IndexOf(T obj)
        {
            if (!ReferenceEquals(obj, null) && ReferenceEquals(this, obj.List))
            {
                lock (_syncLock)
                {
                    return _objectList.IndexOf(obj);
                }
            }
            return -1;
        }

        public T Get(string name)
        {
            name = NormalizeName(name);
            ValidateName(name);

            lock (_syncLock)
            {
                T result;
                _objectIndex.TryGetValue(name, out result);
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;

            INamedObjectList list = obj as INamedObjectList;
            if (!ReferenceEquals(list, null))
            {
                lock (_syncLock)
                {
                    if (!ReferenceEquals(list, this) && (_objectIndex.Count == list.Count))
                    {
                        foreach (string key in _objectIndex.Keys)
                        {
                            INamedObject nob = list.Get(key);
                            if (ReferenceEquals(nob, null) || !object.Equals(nob, _objectIndex[key]))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void UpdateName(string name, string newName)
        {
            name = NormalizeName(name);
            ValidateName(name);

            newName = NormalizeName(newName);
            ValidateName(newName);

            if (name != newName)
            {
                lock (_syncLock)
                {
                    T obj = Remove(name);
                    if (!ReferenceEquals(obj, null))
                    {
                        Add(newName, obj);
                    }
                }
            }
        }

        public T Remove(string name)
        {
            name = NormalizeName(name);
            ValidateName(name);

            T obj = default(T);
            
            lock (_syncLock)
            {
                if (_objectIndex.TryGetValue(name, out obj))
                {
                    _objectArray = null;
                    _objectNames = null;

                    _objectIndex.Remove(name);
                    _objectList.Remove(obj);

                    if (OnRemove != null)
                    {
                        OnRemove(this, obj);
                    }
                }
            }
            return obj;
        }

        event ObjectEvent INamedObjectList.OnRemove
        {
            add { this.OnRemove += value; }
            remove { this.OnRemove -= value; }
        }

        event ObjectEvent INamedObjectList.OnDispose
        {
            add { this.OnDispose += value; }
            remove { this.OnDispose -= value; }
        }

        event ObjectEvent INamedObjectList.OnValidate
        {
            add { this.OnValidate += value; }
            remove { this.OnValidate -= value; }
        }

        event ObjectNamedEvent INamedObjectList.OnAdd
        {
            add { this.OnAdd += value; }
            remove { this.OnAdd -= value; }
        }

        event ObjectNamedEvent INamedObjectList.OnValidateName
        {
            add { this.OnValidateName += value; }
            remove { this.OnValidateName -= value; }
        }

        int INamedObjectList.Count
        {
            get { return this.Count; }
        }

        string[] INamedObjectList.Names
        {
            get { return this.Names; }
        }

        INamedObject[] INamedObjectList.Objects
        {
            get { return this.Objects; }
        }

        INamedObject INamedObjectList.this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                this.Add(name, (T)value);
            }
        }

        void INamedObjectList.Clear()
        {
            this.Clear();
        }

        bool INamedObjectList.Contains(string name)
        {
            return this.Contains(name);
        }

        int INamedObjectList.IndexOf(INamedObject obj)
        {
            return this.IndexOf((T)obj);
        }

        INamedObject INamedObjectList.Get(string name)
        {
            return this.Get(name);
        }

        INamedObject INamedObjectList.Remove(string name)
        {
            return this.Remove(name);
        }

        void INamedObjectList.Add(string name, INamedObject obj)
        {
            this.Add(name, (T)obj);
        }

        void INamedObjectList.UpdateName(string name, string newName)
        {
            this.UpdateName(name, newName);
        }
    }
}
