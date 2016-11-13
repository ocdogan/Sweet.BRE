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
    public class EvaluationScope : VariableList, IEvaluationScope, IDisposable, ICloneable
    {
        private IEvaluationScope _parent;

        internal EvaluationScope(IEvaluationScope parent)
            : base()
        {
            SetParent(parent);
        }

        public IEvaluationScope Parent
        {
            get
            {
                return _parent;
            }
        }

        internal void SetParent(IEvaluationScope parent)
        {
            _parent = parent;
        }

        public override object Clone()
        {
            EvaluationScope result = new EvaluationScope(null);
            result.Copy(this);

            return (result);
        }

        public override void Dispose()
        {
            _parent = null;
            base.Dispose();
        }

        public override bool Contains(string name)
        {
            ValidateName(name);
            name = NormalizeName(name);

            if (ObjList.ContainsKey(name))
            {
                return true;
            }

            if (_parent != null)
            {
                return _parent.Contains(name);
            }

            return false;
        }

        public override IVariable Get(string name)
        {
            ValidateName(name);
            name = NormalizeName(name);

            if (ObjList.ContainsKey(name))
            {
                return ObjList[name];
            }

            if (_parent != null)
            {
                return _parent.Get(name);
            }

            return null;
        }

        protected override bool GetIsEmpty()
        {
            bool result = base.GetIsEmpty();
            if (!result && (_parent != null))
            {
                result = _parent.IsEmpty;
            }

            return result;
        }

        public override IVariable[] ToArray()
        {
            Dictionary<string, IVariable> list = new Dictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);
            if (_parent != null)
            {
                IVariable[] parentList = _parent.ToArray();
                foreach (IVariable obj in parentList)
                {
                    list[obj.Name] = obj;
                }
            }

            IVariable[] baseList = base.ToArray();
            foreach (IVariable obj in baseList)
            {
                list[obj.Name] = obj;
            }

            IVariable[] result = new IVariable[list.Count];
            list.Values.CopyTo(result, 0);

            return result;
        }
    }
}
