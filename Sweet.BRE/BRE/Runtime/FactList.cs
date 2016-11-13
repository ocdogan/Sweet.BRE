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
    public class FactList : IFactList, IDisposable, ICloneable
    {
        private IDictionary<string, object> _list;

        public FactList()
        {
            _list = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string[] Names
        {
            get
            {
                string[] result = new string[_list.Count];
                _list.Keys.CopyTo(result, 0);

                return result;
            }
        }

        public object this[string name]
        {
            get
            {
                return Get(name);
            }
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(string name)
        {
            return _list.ContainsKey(NormalizeName(name));
        }

        public void Copy(IFactList facts)
        {
            _list.Clear();

            if (facts != null)
            {
                foreach (string key in facts.Names)
                {
                    string name = NormalizeName(key);
                    _list[name] = facts[key];
                }
            }
        }

        private void Add(string key, object value)
        {
            string name = NormalizeName(key);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            _list[name] = value;
        }

        public object Get(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (_list.ContainsKey(name))
            {
                return _list[name];
            }

            return null;
        }

        public void Set(string name, object value)
        {
            Add(name, value);
        }

        public object Clone()
        {
            FactList result = new FactList();
            result.Copy(this);

            return (result);
        }

        public void Dispose()
        {
            _list.Clear();
        }
    }
}
