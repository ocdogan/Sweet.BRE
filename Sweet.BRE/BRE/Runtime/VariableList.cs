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
    public class VariableList : IVariableList, IDisposable, ICloneable,
        IEnumerable, IEnumerable<IVariable>
    {
        private List<IVariable> _list;
        private IDictionary<string, IVariable> _objList;

        public VariableList()
        {
            _list = new List<IVariable>();
            _objList = new Dictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);
        }

        protected IList<IVariable> List
        {
            get
            {
                return _list;
            }
        }

        protected IDictionary<string, IVariable> ObjList
        {
            get
            {
                return _objList;
            }
        }

        public int Count
        {
            get
            {
                return _objList.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return GetIsEmpty();
            }
        }

        public IVariable this[string name]
        {
            get
            {
                return Get(name);
            }
        }

        protected string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        protected string ValidateName(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return name;
        }

        public IVariableList Clear()
        {
            _objList.Clear();
            return this;
        }

        public virtual bool Contains(string name)
        {
            return _objList.ContainsKey(NormalizeName(name));
        }

        public IVariableList Append(IVariableList context)
        {
            _list.Clear();
            _objList.Clear();

            if (context != null)
            {
                Append(context.ToArray());
            }
            return this;
        }

        public IVariableList Append(IVariable[] objects)
        {
            if (objects != null)
            {
                foreach (IVariable obj in objects)
                {
                    Update(obj.Name, obj.Value);
                }
            }
            return this;
        }

        public IVariableList Copy(IVariableList context)
        {
            _list.Clear();
            _objList.Clear();

            if (context != null)
            {
                Append(context.ToArray());
            }
            return this;
        }

        public IVariableList Copy(IVariable[] objects)
        {
            _list.Clear();
            _objList.Clear();

            if (objects != null)
            {
                Append(objects);
            }
            return this;
        }

        public virtual IVariable Get(string name)
        {
            ValidateName(name);

            name = NormalizeName(name);
            if (_objList.ContainsKey(name))
            {
                return _objList[name];
            }

            return null;
        }

        protected virtual bool GetIsEmpty()
        {
            return (_list.Count == 0);
        }

        public virtual IVariable[] ToArray()
        {
            return _list.ToArray();
        }

        protected virtual void Update(string name, object value)
        {
            name = NormalizeName(name);

            IVariable obj;
            if (_objList.TryGetValue(name, out obj))
            {
                Variable v = obj as Variable;
                if (!ReferenceEquals(v, null))
                {
                    v.Update(value);
                    return;
                }
            }

            obj = new Variable(name, value);

            _list.Add(obj);
            _objList[name] = obj;
        }

        public virtual IVariableList Set(string name, bool value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, char value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, DateTime value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, double value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, long value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, string value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual IVariableList Set(string name, TimeSpan value)
        {
            IVariable vr = Get(name);
            if (vr != null)
            {
                vr.Set(value);
                return this;
            }

            Update(name, value);
            return this;
        }

        public virtual object Clone()
        {
            VariableList result = new VariableList();
            result.Copy(this);

            return (result);
        }

        public virtual void Dispose()
        {
            _list.Clear();
            _objList.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return ((IEnumerable<IVariable>)_list).GetEnumerator();
        }
    }
}
