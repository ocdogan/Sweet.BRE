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
    public sealed class DecisionColumn : IDisposable, ICloneable
    {
        private DecisionTable _owner;

        private string _name;
        private string _description;

        private DecisionValueType _type = DecisionValueType.String;

        internal DecisionColumn()
        {
        }

        internal DecisionColumn(DecisionTable owner)
        {
            SetOwner(owner);
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public DecisionTable Owner
        {
            get
            {
                return _owner;
            }
        }

        public DecisionValueType Type
        {
            get
            {
                return _type;
            }
        }

        public object Clone()
        {
            DecisionColumn result = new DecisionColumn();

            result.SetName(_name);
            result.SetDescription(_description);
            result.SetType(_type);

            return result;
        }

        public void Dispose()
        {
            _owner = null;
        }

        public DecisionTable SetDescription(string description)
        {
            _description = (description != null ? description : String.Empty);
            return _owner;
        }

        internal DecisionTable SetName(string name)
        {
            _name = (name != null ? name.Trim() : String.Empty);
            return _owner;
        }

        internal void SetOwner(DecisionTable owner)
        {
            _owner = owner;
        }

        public DecisionTable SetType(DecisionValueType type)
        {
            _type = type;
            return _owner;
        }

        public override bool Equals(object obj)
        {
            DecisionColumn objA = obj as DecisionColumn;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) && 
                (_name == objA.Name) && (_description == objA.Description) && 
                (_type == objA.Type));
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
