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
    public class FunctionEventArgs : EventArgs, IDisposable
    {
        public new static readonly FunctionEventArgs Empty = new FunctionEventArgs(null);

        private object[] _args;
        private bool _handled = false;
        private string _name;
        private object _result;
        private IEvaluationContext _context;

        protected FunctionEventArgs(IEvaluationContext context)
            : this(context, null)
        {
        }

        public FunctionEventArgs(IEvaluationContext context, string name, params object[] args)
        {
            _args = args;
            _context = context;
            _name = (name != null ? name.Trim() : null);
        }

        public object[] Args
        {
            get
            {
                return _args;
            }
        }

        public IEvaluationContext Context
        {
            get
            {
                return _context;
            }
        }

        public bool Handled
        {
            get
            {
                return _handled;
            }
            set
            {
                _handled = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public object Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        void IDisposable.Dispose()
        {
            _args = null;
            _context = null;
        }
    }
}
