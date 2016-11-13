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
using System.Globalization;
using System.Text;
using System.Threading;

namespace Sweet.BRE
{
    public sealed class ArrayFunctions : IFunctionHandler
    {
        private const string STR_CONTAINS = "CONTAINS";
        private const string STR_COUNT = "COUNT";
        private const string STR_INDEXOF = "INDEXOF";
        private const string STR_LASTINDEXOF = "LASTINDEXOF";
        private const string STR_LENGTH = "LENGTH";

        private List<FunctionInfo> _info;

        public ArrayFunctions()
        {
            _info = new List<FunctionInfo>();
            DefineMethods();
        }

        public FunctionInfo[] HandledFunctions
        {
            get
            {
                return _info.ToArray();
            }
        }

        private void DefineMethods()
        {
            _info.AddRange(
                new FunctionInfo[] { 
                    new FunctionInfo("Contains", 2, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object,
                            ValueType.Integer
                        },
                        ReturnType.Boolean), 
                    new FunctionInfo("Count", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer),
                    new FunctionInfo("IndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object,
                            ValueType.Integer
                        },
                        ReturnType.Integer), 
                    new FunctionInfo("LastIndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object,
                            ValueType.Integer
                        },
                        ReturnType.Integer), 
                    new FunctionInfo("Length", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer)
                }
                );
        }

        public void Eval(FunctionEventArgs e)
        {
            string function = e.Name;

            function = (function != null ? function.Trim() : String.Empty);
            function = function.ToUpperInvariant();

            e.Handled = true;
            object result = e.Result;

            switch (function)
            {
                case STR_CONTAINS:
                    result = Contains(e, e.Args);
                    break;

                case STR_COUNT:
                    result = Count(e, e.Args);
                    break;

                case STR_INDEXOF:
                    result = IndexOf(e, e.Args);
                    break;

                case STR_LASTINDEXOF:
                    result = LastIndexOf(e, e.Args);
                    break;

                case STR_LENGTH:
                    result = Length(e, e.Args);
                    break;

                default:
                    e.Handled = false;
                    break;
            }

            if (e.Handled)
            {
                e.Result = result;
            }
        }

        # region Contains
        private bool ContainsInArray(object[] objs, object item, long start)
        {
            if ((objs != null) && (objs.Length > start))
            {
                long index = 0;
                foreach (object o in objs)
                {
                    if ((index++ >= start) && (o == item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ContainsInICollection(ICollection list, object item, long start)
        {
            if ((list != null) && (list.Count > start))
            {
                long index = 0;
                foreach (object o in list)
                {
                    if ((index++ >= start) && (o == item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ContainsInIDictionary(IDictionary list, object item, long start)
        {
            if (list != null)
            {
                if (start == 0)
                {
                    return list.Contains(item);
                }
                else if (list.Count > start)
                {
                    long index = 0;
                    foreach (object o in list.Keys)
                    {
                        if ((index++ >= start) && (o == item))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool ContainsInIList(IList list, object item, long start)
        {
            if (list != null)
            {
                if (start == 0)
                {
                    return list.Contains(item);
                }
                else if (list.Count > start)
                {
                    long index = 0;
                    foreach (object o in list)
                    {
                        if ((index++ >= start) && (o == item))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Contains(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    object item = args[1];

                    long start = 0;
                    if (args.Length > 2)
                    {
                        string s = StmCommon.ToString(args[2]);
                        if ((s != null) && !long.TryParse(s, out start))
                        {
                            start = 0;
                        }
                    }

                    if ((o1 is Array) || o1.GetType().IsArray)
                    {
                        e.Handled = true;
                        return ContainsInArray((object[])o1, item, start);
                    }

                    if (o1 is IDictionary)
                    {
                        e.Handled = true;
                        return ContainsInIDictionary((IDictionary)o1, item, start);
                    }

                    if (o1 is IList)
                    {
                        e.Handled = true;
                        return ContainsInIList((IList)o1, item, start);
                    }

                    if (o1 is ICollection)
                    {
                        e.Handled = true;
                        return ContainsInICollection((ICollection)o1, item, start);
                    }

                    if ((o1 != null) && (item != null))
                    {
                        string text = o1.ToString();
                        string pattern = item.ToString();

                        if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(pattern))
                        {
                            e.Handled = true;
                            return (text.IndexOf(pattern, (int)start) > -1);
                        }
                    }
                }
            }

            return false;
        }
        # endregion

        # region IndexOf
        private long IndexOfInArray(object[] objs, object item, long start)
        {
            if ((objs != null) && (objs.Length > start))
            {
                long index = 0;
                foreach (object o in objs)
                {
                    if ((index >= start) && (o == item))
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        private long IndexOfInICollection(ICollection list, object item, long start)
        {
            if ((list != null) && (list.Count > start))
            {
                long index = 0;
                foreach (object o in list)
                {
                    if ((index >= start) && (o == item))
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        private long IndexOfInIList(IList list, object item, long start)
        {
            if ((list != null) && (list.Count > start))
            {
                long index = 0;
                foreach (object o in list)
                {
                    if ((index >= start) && (o == item))
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        public long IndexOf(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    object item = args[1];

                    long start = 0;
                    if (args.Length > 2)
                    {
                        string s = StmCommon.ToString(args[2]);
                        if ((s != null) && !long.TryParse(s, out start))
                        {
                            start = 0;
                        }
                    }

                    if ((o1 is Array) || o1.GetType().IsArray)
                    {
                        e.Handled = true;
                        return IndexOfInArray((object[])o1, item, start);
                    }

                    if (o1 is IList)
                    {
                        e.Handled = true;
                        return IndexOfInIList((IList)o1, item, start);
                    }

                    if (o1 is ICollection)
                    {
                        e.Handled = true;
                        return IndexOfInICollection((ICollection)o1, item, start);
                    }

                    if ((o1 != null) && (item != null))
                    {
                        string text = o1.ToString();
                        string pattern = item.ToString();

                        if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(pattern))
                        {
                            e.Handled = true;
                            return text.IndexOf(pattern, (int)start);
                        }
                    }
                }
            }

            return -1;
        }
        # endregion

        # region LastIndexOf
        private long LastIndexOfInArray(object[] objs, object item, long start)
        {
            if ((objs != null) && (objs.Length > start))
            {
                long pos = -1;

                long index = 0;
                foreach (object o in objs)
                {
                    if ((index >= start) && (o == item))
                    {
                        pos = index;
                    }

                    index++;
                }

                return pos;
            }

            return -1;
        }

        private long LastIndexOfInICollection(ICollection list, object item, long start)
        {
            if ((list != null) && (list.Count > start))
            {
                if ((list != null) && (list.Count > start))
                {
                    long pos = -1;

                    long index = 0;
                    foreach (object o in list)
                    {
                        if ((index >= start) && (o == item))
                        {
                            pos = index;
                        }

                        index++;
                    }

                    return pos;
                }

                return -1;
            }

            return -1;
        }

        private long LastIndexOfInIList(IList list, object item, long start)
        {
            if ((list != null) && (list.Count > start))
            {
                if ((list != null) && (list.Count > start))
                {
                    long pos = -1;

                    long index = 0;
                    foreach (object o in list)
                    {
                        if ((index >= start) && (o == item))
                        {
                            pos = index;
                        }

                        index++;
                    }

                    return pos;
                }
            }

            return -1;
        }

        public long LastIndexOf(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    object item = args[1];

                    long start = 0;
                    if (args.Length > 2)
                    {
                        string s = StmCommon.ToString(args[2]);
                        if ((s != null) && !long.TryParse(s, out start))
                        {
                            start = 0;
                        }
                    }

                    if ((o1 is Array) || o1.GetType().IsArray)
                    {
                        e.Handled = true;
                        return LastIndexOfInArray((object[])o1, item, start);
                    }

                    if (o1 is IList)
                    {
                        e.Handled = true;
                        return LastIndexOfInIList((IList)o1, item, start);
                    }

                    if (o1 is ICollection)
                    {
                        e.Handled = true;
                        return LastIndexOfInICollection((ICollection)o1, item, start);
                    }

                    if ((o1 != null) && (item != null))
                    {
                        string text = o1.ToString();
                        string pattern = item.ToString();

                        if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(pattern))
                        {
                            e.Handled = true;
                            return text.LastIndexOf(pattern, (int)start);
                        }
                    }
                }
            }

            return -1;
        }
        # endregion

        public int Count(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    if ((o1 is Array) || o1.GetType().IsArray)
                    {
                        e.Handled = true;
                        return ((object[])o1).Length;
                    }

                    if (o1 is IList)
                    {
                        e.Handled = true;
                        return ((IList)o1).Count;
                    }

                    if (o1 is ICollection)
                    {
                        e.Handled = true;
                        return ((ICollection)o1).Count;
                    }

                    if (o1 is IDictionary)
                    {
                        e.Handled = true;
                        return ((IDictionary)o1).Count;
                    }
                }
            }

            return 0;
        }

        public int Length(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    e.Handled = true;

                    if ((o1 is Array) || o1.GetType().IsArray)
                        return ((object[])o1).Length;

                    if (o1 is IList)
                        return ((IList)o1).Count;

                    if (o1 is ICollection)
                        return ((ICollection)o1).Count;

                    if (o1 is IDictionary)
                        return ((IDictionary)o1).Count;

                    return o1.ToString().Length;
                }
            }

            return 0;
        }
    }
}
