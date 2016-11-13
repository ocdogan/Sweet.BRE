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
    public class Variable : IVariable
    {
        private string _name;
        private object _value;
        private ValueType _type = ValueType.Null;

        internal Variable(string name, object value)
        {
            _name = name;
            _value = value;
            _type = StmCommon.GetType(value);
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ValueType Type
        {
            get
            {
                return _type;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        internal void Update(object value)
        {
            _value = value;
            _type = StmCommon.GetType(value);
        }

        public void Set(bool value)
        {
            Update(value);
        }

        public void Set(char value)
        {
            Update(value);
        }

        public void Set(DateTime value)
        {
            Update(value);
        }

        public void Set(double value)
        {
            Update(value);
        }

        public void Set(long value)
        {
            Update(value);
        }

        public void Set(string value)
        {
            Update(value);
        }

        public void Set(TimeSpan value)
        {
            Update(value);
        }

        public override bool Equals(object obj)
        {
            IVariable objA = obj as IVariable;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                (String.Compare(_name, objA.Name, true) == 0) && (_type == objA.Type) &&
                object.Equals(_value, objA.Value));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string to = "@var";
            if (!ReferenceEquals(_name, null))
            {
                to = _name;
            }

            string value = StmCommon.PrepareToString(_value);
            builder.AppendFormat("{0} ({1}) = {2}; ", to, _type.ToString(), value);

            return builder.ToString();
        }
    }
}
