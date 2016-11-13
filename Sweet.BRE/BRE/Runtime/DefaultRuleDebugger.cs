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
    public class DefaultRuleDebugger : IRuleDebugger
    {
        private string _name;
        private Action<object, DebugEventArgs> _handler;

        public DefaultRuleDebugger()
            : this(null, null)
        {
        }

        public DefaultRuleDebugger(string name)
            : this(name, null)
        {
            _name = name;
        }

        public DefaultRuleDebugger(Action<object, DebugEventArgs> handler)
            : this(null, handler)
        {
        }

        public DefaultRuleDebugger(string name, Action<object, DebugEventArgs> handler)
        {
            _name = name;
            _handler = handler;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public void Debug(DebugEventArgs args)
        {
            if (_handler != null)
            {
                _handler(this, args);
            }
        }
    }
}
