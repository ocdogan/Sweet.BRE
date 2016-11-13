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
    public sealed class DebugEventArgs : EventArgs, IDisposable
    {
        private static readonly object[] EmptyArgs = new object[0];

        private DebugStatus _status;
        private Exception _error;

        private object[] _args;
        private IStatement _statement;
        private IEvaluationContext _context;

        internal DebugEventArgs(IEvaluationContext context, IStatement statement)
            : this(context, statement, null)
        {
        }

        internal DebugEventArgs(IEvaluationContext context, IStatement statement, params object[] args)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            _args = args ?? EmptyArgs;
            _context = context;
            _statement = statement;
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

        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        public IStatement Statement
        {
            get
            {
                return _statement;
            }
        }

        public DebugStatus Status
        {
            get
            {
                return _status;
            }
        }

        void IDisposable.Dispose()
        {
            _args = null;
            _context = null;
            _statement = null;
            _error = null;
        }

        internal void SetError(Exception exception)
        {
            _error = exception;
        }

        internal void SetStatus(DebugStatus status)
        {
            _status = status;
        }
    }
}
