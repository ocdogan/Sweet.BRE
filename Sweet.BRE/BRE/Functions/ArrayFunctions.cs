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

        private delegate object Fn(FunctionEventArgs e, params object[] args);

        private static readonly Dictionary<string, Fn> _functions = new Dictionary<string, Fn>();

        #region Constants

        private const string STR_CONTAINS = "CONTAINS";
        private const string STR_COUNT = "COUNT";
        private const string STR_COUNTOF = "COUNTOF";
        private const string STR_HAS = "HAS";
        private const string STR_INDEX = "INDEX";
        private const string STR_INDEXOF = "INDEXOF";
        private const string STR_LASTINDEX = "LASTINDEX";
        private const string STR_LASTINDEXOF = "LASTINDEXOF";
        private const string STR_LASTPOS = "LASTPOS";
        private const string STR_LASTPOSOF = "LASTPOSOF";
        private const string STR_LASTPOSITION = "LASTPOSITION";
        private const string STR_LASTPOSITIONOF = "LASTPOSITIONOF";
        private const string STR_LEN = "LEN";
        private const string STR_LENOF = "LENOF";
        private const string STR_LENGTH = "LENGTH";
        private const string STR_LENGTHOF = "LENGTHOF";
        private const string STR_POS = "POS";
        private const string STR_POSOF = "POSOF";
        private const string STR_POSITION = "POSITION";
        private const string STR_POSITIONOF = "POSITIONOF";

        #endregion Constants

        private List<FunctionInfo> _info;

        static ArrayFunctions()
        {
            _functions[STR_CONTAINS] = Contains;
            _functions[STR_HAS] = Contains;

            _functions[STR_COUNT] = Count;
            _functions[STR_COUNTOF] = Count;

            _functions[STR_INDEX] = IndexOf;
            _functions[STR_INDEXOF] = IndexOf;
            _functions[STR_POS] = IndexOf;
            _functions[STR_POSOF] = IndexOf;
            _functions[STR_POSITION] = IndexOf;
            _functions[STR_POSITIONOF] = IndexOf;

            _functions[STR_LASTINDEX] = LastIndexOf;
            _functions[STR_LASTINDEXOF] = LastIndexOf;
            _functions[STR_LASTPOS] = LastIndexOf;
            _functions[STR_LASTPOSOF] = LastIndexOf;
            _functions[STR_LASTPOSITION] = LastIndexOf;
            _functions[STR_LASTPOSITIONOF] = LastIndexOf;

            _functions[STR_LEN] = Length;
            _functions[STR_LENGTH] = Length;
            _functions[STR_LENOF] = Length;
            _functions[STR_LENGTHOF] = Length;         }

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
                        ReturnType.Boolean)
                            .AddAlias("Has"), 
                    new FunctionInfo("Count", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer)
                            .AddAlias("CountOf"),
                    new FunctionInfo("IndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object,
                            ValueType.Integer
                        },
                        ReturnType.Integer)
                            .AddAliases(new string[] { "Index", "Pos", "PosOf", "Position", "PositionOf" }),
                    new FunctionInfo("LastIndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object,
                            ValueType.Integer
                        },
                        ReturnType.Integer)
                            .AddAliases(new string[] { "LastIndex", "LastPos", "LastPosOf", "LastPosition", "LastPositionOf" }),
                    new FunctionInfo("Length", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer)
                            .AddAliases(new string[] { "Len", "LenOf", "LengthOf" })
                }
                );
        }


        public void Eval(FunctionEventArgs e)
        {
            string function = e.Name;
            function = (function != null ? function.Trim().ToUpperInvariant() : null);

            e.Handled = false;
            e.Result = null;

            if (!String.IsNullOrEmpty(function))
            {
                Fn f;
                if (_functions.TryGetValue(function, out f) && (f != null))
                {
                    e.Result = f(e, e.Args);
                    e.Handled = true;
                }
            }
        }

        # region Contains

        private static bool ContainsInArray(Array objs, object item, long start)
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

        private static bool ContainsInICollection(ICollection list, object item, long start)
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

        private static bool ContainsInIDictionary(IDictionary list, object item, long start)
        {
            if (list != null)
            {
                if (start == 0)
                {
                    return list.Contains(item);
                }

                if (list.Count > start)
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

        private static bool ContainsInIList(IList list, object item, long start)
        {
            if (list != null)
            {
                if (start == 0)
                {
                    return list.Contains(item);
                }

                if (list.Count > start)
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

        public static object Contains(FunctionEventArgs e, params object[] args)
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
                        return ContainsInArray((Array)o1, item, start);
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

        private static long IndexOfInArray(Array objs, object item, long start)
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

        private static long IndexOfInICollection(ICollection list, object item, long start)
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

        private static long IndexOfInIList(IList list, object item, long start)
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

        public static object IndexOf(FunctionEventArgs e, params object[] args)
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
                        return IndexOfInArray((Array)o1, item, start);
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

        private static long LastIndexOfInArray(Array objs, object item, long start)
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

        private static long LastIndexOfInICollection(ICollection list, object item, long start)
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

        private static long LastIndexOfInIList(IList list, object item, long start)
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

        public static object LastIndexOf(FunctionEventArgs e, params object[] args)
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
                        return LastIndexOfInArray((Array)o1, item, start);
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

        public static object Count(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                object o1 = args[0];
                if (o1 != null)
                {
                    e.Handled = true;

                    if ((o1 is Array) || o1.GetType().IsArray)
                    {
                        return ((Array)o1).Length;
                    }

                    if (o1 is IList)
                    {
                        return ((IList)o1).Count;
                    }

                    if (o1 is ICollection)
                    {
                        return ((ICollection)o1).Count;
                    }

                    if (o1 is IDictionary)
                    {
                        return ((IDictionary)o1).Count;
                    }

                    string str = o1.ToString();
                    if (str != null)
                    {
                        return str.Length;
                    }

                    return 0;
                }
            }

            return 0;
        }

        public static object Length(FunctionEventArgs e, params object[] args)
        {
            return Count(e, args);
        }
    }
}
