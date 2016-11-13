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
    public class OperationArgs : IDisposable
    {
        private char _op;
        private Statement _statement;

        public OperationArgs(char op, Statement stm)
        {
            _op = op;
            _statement = stm;
        }

        public char Operation
        {
            get
            {
                return _op;
            }
        }

        public Statement Statement
        {
            get
            {
                return _statement;
            }
        }

        public void Dispose()
        {
            _statement = null;
        }

        public override int GetHashCode()
        {
            int result = _op.GetHashCode();
            if (!ReferenceEquals(_statement, null))
            {
                result = result ^ _statement.GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            OperationArgs objA = obj as OperationArgs;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                (_op == objA.Operation) && object.Equals(_statement, objA.Statement));
        }

        public override string ToString()
        {
            string result = String.Format("{0}", _op.ToString());
            if (!ReferenceEquals(_statement, null))
            {
                result = String.Format(" {0}", _statement.ToString());
            }

            return result;
        }

        public static bool operator ==(OperationArgs x, OperationArgs y)
        {
            return ReferenceEquals(x, y) || (!ReferenceEquals(x, null) && 
                !ReferenceEquals(y, null) && x.Equals(y));
        }

        public static bool operator !=(OperationArgs x, OperationArgs y)
        {
            return !ReferenceEquals(x, y) && (ReferenceEquals(x, null) ||
                ReferenceEquals(y, null) || !x.Equals(y));
        }
    }
}
