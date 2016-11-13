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
    public class FunctionInfoBucket : IDisposable
    {
        private FunctionInfo info;
        private IFunctionHandler handler;

        internal FunctionInfoBucket(IFunctionHandler handler, FunctionInfo info)
        {
            this.handler = handler;
            this.info = info;
        }

        public IFunctionHandler Handler
        {
            get
            {
                return handler;
            }
        }

        public FunctionInfo Info
        {
            get
            {
                return info;
            }
        }

        public override bool Equals(object obj)
        {
            FunctionInfoBucket objA = obj as FunctionInfoBucket;
            return !ReferenceEquals(objA, null) && ReferenceEquals(info, objA.Info) &&
                ReferenceEquals(handler, objA.Handler);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            handler = null;
            info = null;
        }

        #endregion
    }
}
