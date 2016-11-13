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
    public class DecisionValueList : GenericList<string> 
    {
        private DecisionCell _owner;

        internal DecisionValueList(DecisionCell owner)
            : base(false)
        {
            _owner = owner;
        }

        public DecisionCell Owner
        {
            get
            {
                return _owner;
            }
        }

        public override void Dispose()
        {
            _owner = null;
            lock (base.SyncRoot)
            {
                base.Dispose();
            }
        }

        private DecisionValueType GetValueType()
        {
            return (!ReferenceEquals(_owner, null) ? _owner.Type : DecisionValueType.String);
        }

        protected override bool InsertItem(int index, string item)
        {
            bool result = base.InsertItem(index, item);
            if (result && (_owner != null))
            {
                _owner.ValuesChanged();
            }

            return result;
        }

        public bool EqualList(IList<string> list)
        {
            lock (base.SyncRoot)
            {
                if ((list != null) && (list.Count == base.Count))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string item1 = base[i];
                        string item2 = list[i];

                        if (!String.Equals(item1, item2))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            IList<string> objA = obj as IList<string>;
            return (objA == this) || ((objA != null) && EqualList(objA));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            int index = 0;
            DecisionValueType type = GetValueType();

            foreach (string value in this)
            {
                index++;

                string str = value;
                if (value == null)
                {
                    str = StmConstants.NULL;
                }
                else if (type == DecisionValueType.String)
                {
                    str = StmCommon.PrepareToString(str);
                }

                builder.Append(str);
                if (index < base.Count)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }
    }
}
