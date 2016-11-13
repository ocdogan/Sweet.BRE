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
    public interface IVariableList : IEnumerable, IEnumerable<IVariable>
    {
        int Count { get; }
        bool IsEmpty { get; }
        IVariable this[string name] { get; }

        IVariable Get(string name);
        bool Contains(string name);

        void Append(IVariable[] objects);
        void Append(IVariableList context);

        void Copy(IVariable[] objects);
        void Copy(IVariableList context);

        void Set(string name, bool value);
        void Set(string name, char value);
        void Set(string name, DateTime value);
        void Set(string name, double value);
        void Set(string name, long value);
        void Set(string name, string value);
        void Set(string name, TimeSpan value);

        IVariable[] ToArray();
    }
}
