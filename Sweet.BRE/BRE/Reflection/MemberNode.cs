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
    public class MemberNode : IDisposable
    {
        private string _value;
        private MemberType _type;

        private MemberNode _prevSibling;
        private MemberNode _nextSibling;

        private GenericList<MemberNode> _arguments;

        internal MemberNode(MemberType type)
            : this(type, null)
        {
        }

        internal MemberNode(MemberType type, string value)
        {
            _arguments = new GenericList<MemberNode>();

            _type = type;
            _value = value;
        }

        public IList<MemberNode> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public MemberNode NextSibling
        {
            get
            {
                return _nextSibling;
            }
        }

        public MemberNode PrevSibling
        {
            get
            {
                return _prevSibling;
            }
        }

        public MemberType Type
        {
            get
            {
                return _type;
            }
        }

        public string Value
        {
            get
            {
                return (_value != null ? _value : String.Empty);
            }
        }

        internal void SetNextSibling(MemberNode value)
        {
            _nextSibling = value;
        }

        internal void SetPrevSibling(MemberNode value)
        {
            _prevSibling = value;
        }

        void IDisposable.Dispose()
        {
            _prevSibling = null;
            _nextSibling = null;

            _arguments.Clear();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}", _type.ToString(), Value);
        }
    }
}
