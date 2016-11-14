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
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Sweet.BRE
{
    public sealed class InternalFunctions : IFunctionHandler
    {
        private delegate object Fn(FunctionEventArgs e, params object[] args);

        private static readonly Dictionary<string, Fn> _functions = new Dictionary<string, Fn>();

        #region Constants

        private const string STR_ASBOOLEAN = "ASBOOLEAN";
        private const string STR_ASDATE = "ASDATE";
        private const string STR_ASDOUBLE = "ASDOUBLE";
        private const string STR_ASGUID = "ASGUID";
        private const string STR_ASINTEGER = "ASINTEGER";
        private const string STR_ASSTRING = "ASSTRING";
        private const string STR_ASTIME = "ASTIME";
        private const string STR_ASUUID = "ASUUID";
        private const string STR_BOOLEAN = "BOOLEAN";
        private const string STR_COMPARE = "COMPARE";
        private const string STR_DATE = "DATE";
        private const string STR_DEBUG = "DEBUG";
        private const string STR_DOUBLE = "DOUBLE";
        private const string STR_ENVIRONMENT = "ENVIRONMENT";
        private const string STR_GETENVIRONMENT = "GETENVIRONMENT";
        private const string STR_GETENVIRONMENTVARIABLE = "GETENVIRONMENTVARIABLE";
        private const string STR_EQUALS = "EQUALS";
        private const string STR_GUID = "GUID";
        private const string STR_INTEGER = "INTEGER";
        private const string STR_ISNULL = "ISNULL";
        private const string STR_ISNUMBER = "ISNUMBER";
        private const string STR_LOG = "LOG";
        private const string STR_NEWGUID = "NEWGUID";
        private const string STR_NEWUUID = "NEWUUID";
        private const string STR_PRINT = "PRINT";
        private const string STR_PRINTLINE = "PRINTLINE";
        private const string STR_PRINTLN = "PRINTLN";
        private const string STR_PRINTF = "PRINTF";
        private const string STR_PRINTFORMAT = "PRINTFORMAT";
        private const string STR_PRINTFORMATLINE = "PRINTFORMATLINE";
        private const string STR_PRINTFORMATLN = "PRINTFORMATLN";
        private const string STR_PRINTFLN = "PRINTFLN";
        private const string STR_SLEEP = "SLEEP";
        private const string STR_STRING = "STRING";
        private const string STR_TIME = "TIME";
        private const string STR_TOBOOLEAN = "TOBOOLEAN";
        private const string STR_TODATE = "TODATE";
        private const string STR_TODOUBLE = "TODOUBLE";
        private const string STR_TOGUID = "TOGUID";
        private const string STR_TOINTEGER = "TOINTEGER";
        private const string STR_TOSTRING = "TOSTRING";
        private const string STR_TOTIME = "TOTIME";
        private const string STR_TOUUID = "TOUUID";
        private const string STR_TRACE = "TRACE";
        private const string STR_TRACERT = "TRACERT";
        private const string STR_UUID = "UUID";
        private const string STR_WRITE = "WRITE";
        private const string STR_WRITELINE = "WRITELINE";
        private const string STR_WRITELN = "WRITELN";
        private const string STR_WRITEF = "WRITEF";
        private const string STR_WRITEFORMAT = "WRITEFORMAT";
        private const string STR_WRITEFORMATLINE = "WRITEFORMATLINE";
        private const string STR_WRITEFORMATLN = "WRITEFORMATLN";
        private const string STR_WRITEFLN = "WRITEFLN";

        #endregion Constants

        private List<FunctionInfo> _info;

        static InternalFunctions()
        {
            _functions[STR_COMPARE] = Compare;

            _functions[STR_DEBUG] = WriteToDebug;

            _functions[STR_ENVIRONMENT] = EnvironmentVariable;
            _functions[STR_GETENVIRONMENT] = EnvironmentVariable;
            _functions[STR_GETENVIRONMENTVARIABLE] = EnvironmentVariable;

            _functions[STR_EQUALS] = Equals;

            _functions[STR_ISNULL] = IsNull;

            _functions[STR_ISNUMBER] = IsNumber;

            _functions[STR_NEWGUID] = NewGuid;
            _functions[STR_NEWUUID] = NewGuid;

            _functions[STR_PRINT] = Print;
            _functions[STR_WRITE] = Print;

            _functions[STR_PRINTF] = Printf;
            _functions[STR_PRINTFORMAT] = Printf;
            _functions[STR_WRITEF] = Printf;
            _functions[STR_WRITEFORMAT] = Printf;

            _functions[STR_PRINTLN] = PrintLine;
            _functions[STR_PRINTLINE] = PrintLine;
            _functions[STR_WRITELN] = PrintLine;
            _functions[STR_WRITELINE] = PrintLine;

            _functions[STR_PRINTFLN] = Printfln;
            _functions[STR_PRINTFORMATLN] = Printfln;
            _functions[STR_PRINTFORMATLINE] = Printfln;
            _functions[STR_WRITEFLN] = Printfln;
            _functions[STR_WRITEFORMATLN] = Printfln;
            _functions[STR_WRITEFORMATLINE] = Printfln;

            _functions[STR_SLEEP] = Sleep;

            _functions[STR_ASBOOLEAN] = ToBoolean;
            _functions[STR_BOOLEAN] = ToBoolean;
            _functions[STR_TOBOOLEAN] = ToBoolean;

            _functions[STR_ASDATE] = ToDate;
            _functions[STR_DATE] = ToDate;
            _functions[STR_TODATE] = ToDate;

            _functions[STR_ASDOUBLE] = ToDouble;
            _functions[STR_DOUBLE] = ToDouble;
            _functions[STR_TODOUBLE] = ToDouble;

            _functions[STR_ASGUID] = ToGuid;
            _functions[STR_ASUUID] = ToGuid;
            _functions[STR_GUID] = ToGuid;
            _functions[STR_TOGUID] = ToGuid;
            _functions[STR_UUID] = ToGuid;
            _functions[STR_TOUUID] = ToGuid;

            _functions[STR_ASINTEGER] = ToInteger;
            _functions[STR_INTEGER] = ToInteger;
            _functions[STR_TOINTEGER] = ToInteger;

            _functions[STR_ASSTRING] = ToString;
            _functions[STR_STRING] = ToString;
            _functions[STR_TOSTRING] = ToString;

            _functions[STR_ASTIME] = ToTime;
            _functions[STR_TIME] = ToTime;
            _functions[STR_TOTIME] = ToTime;

            _functions[STR_TRACE] = WriteToTrace;
            _functions[STR_TRACERT] = WriteToTrace;
        }

        public InternalFunctions()
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
                    new FunctionInfo("Compare", 2, 2, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object
                        },
                        ReturnType.Integer),
                    new FunctionInfo("Debug", 1, 3, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.String,
                            ValueType.Integer
                        },
                        ReturnType.Void), 
                    new FunctionInfo("Environment", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.String)
                            .AddAliases(new string[] { "EnvironmentVariable", "GetEnvironment", "GetEnvironmentVariable" }), 
                    new FunctionInfo("Equals", 2, 2, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object
                        },
                        ReturnType.Boolean), 
                    new FunctionInfo("IsNull", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Boolean), 
                    new FunctionInfo("IsNumber", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Boolean), 
                    new FunctionInfo("Log", 1, 2, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.Object
                        },
                        ReturnType.Void),
                    new FunctionInfo("NewGuid", 0, 0, null, ReturnType.String)
                            .AddAlias("NewUuid"),
                    new FunctionInfo("Print", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Void)
                            .AddAlias("Write"),
                    new FunctionInfo("Printf", 0, 1, new ValueType[]
                        {
                            ValueType.Object
                        },
                        ReturnType.Void)
                            .AddAliases(new string[] { "PrintFormat", "Writef", "WriteFormat" }),
                    new FunctionInfo("PrintLine", 0, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Void)
                            .AddAliases(new string[] { "PrintLn", "WriteLn", "WriteLine" }),
                    new FunctionInfo("Printfln", 0, 1, new ValueType[]
                        {
                            ValueType.Object
                        },
                        ReturnType.Void)
                            .AddAliases(new string[] { "PrintFormatLine", "PrintFormatLn", "Writefln", "WriteFormatLine", "WriteFormatLn" }),
                    new FunctionInfo("Sleep", 1, 1, new ValueType[] 
                        {
                            ValueType.Integer
                        },
                        ReturnType.Void),
                    new FunctionInfo("ToBoolean", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Boolean)
                            .AddAliases(new string[] { "AsBoolean", "Boolean" }), 
                    new FunctionInfo("ToDate", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.DateTime)
                            .AddAliases(new string[] { "AsDate", "Date" }), 
                    new FunctionInfo("ToDouble", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Float)
                            .AddAliases(new string[] { "AsDouble", "Double" }),
                    new FunctionInfo("ToGuid", 1, 1, new ValueType[]
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer)
                            .AddAliases(new string[] { "AsGuid", "Guid", "AsUuid", "Uuid", "ToUuid" }),
                    new FunctionInfo("ToInteger", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.Integer)
                            .AddAliases(new string[] { "AsInteger", "Integer" }), 
                    new FunctionInfo("ToString", 1, 2, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.String
                        },
                        ReturnType.String)
                            .AddAliases(new string[] { "AsString", "String" }), 
                    new FunctionInfo("ToTime", 1, 1, new ValueType[] 
                        {
                            ValueType.Object
                        },
                        ReturnType.TimeSpan)
                            .AddAliases(new string[] { "AsTime", "Time" }),
                    new FunctionInfo("Trace", 1, 2, new ValueType[] 
                        {
                            ValueType.Object,
                            ValueType.String
                        },
                        ReturnType.Void)
                            .AddAlias("Tracert")
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

        public static object Compare(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;
                return StmCommon.Compare(args[0], args[1]);
            }

            return 0;
        }

        public static object WriteToDebug(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                string category = String.Empty;
                string msg = StmCommon.ToString(args[0]);

                if (args.Length > 1)
                {
                    category = StmCommon.ToString(args[1]);
                }

                Debug.Write(category, msg);
            }

            return double.NaN;
        }

        public static object EnvironmentVariable(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string varName = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(varName))
                {
                    return Environment.GetEnvironmentVariable(varName);
                }
            }

            return String.Empty;
        }

        public static object Equals(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                return StmCommon.Compare(args[0], (args.Length > 1) ? args[1] : null) == 0;
            }

            return false;
        }

        public static object IsNull(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                foreach (object o1 in args)
                {
                    if (args[0] != null)
                        return false;
                }
            }

            return true;
        }

        public static object NewGuid(FunctionEventArgs e, params object[] args)
        {
            return Guid.NewGuid();
        }

        public static object IsNumber(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                object o1 = args[0];
                if (o1 != null)
                {
                    if (RuleCommon.IsNumber(o1))
                        return true;

                    if ((o1 is string) && CommonHelper.IsNumber((string)o1))
                        return true;
                }
            }

            return false;
        }

        public static object Print(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                foreach (object o1 in args)
                {
                    Console.Write(StmCommon.ToString(o1));
                }
            }

            return null;
        }

        public static object Printf(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string format = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(format))
                {
                    string[] sa = new string[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        sa[i - 1] = StmCommon.ToString(args[i]);
                    }

                    Console.Write(String.Format(format, sa));
                }
            }

            return null;
        }

        public static object Printfln(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string format = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(format))
                {
                    string[] sa = new string[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        sa[i - 1] = StmCommon.ToString(args[i]);
                    }

                    Console.WriteLine(String.Format(format, sa));
                }
            }

            return null;
        }

        public static object PrintLine(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if (args != null)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    foreach (object o1 in args)
                    {
                        Console.Write(StmCommon.ToString(o1));
                    }
                    Console.WriteLine();
                }
            }

            return null;
        }

        public static object Sleep(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                long interval = StmCommon.ToInteger(args[0]);
                Thread.Sleep((int)interval);
            }

            return null;
        }

        public static object ToBoolean(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                return StmCommon.ToBoolean(args[0]);
            }

            return false;
        }

        public static object ToDate(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                return StmCommon.ToDate(args[0]);
            }

            return DateTime.MinValue;
        }

        public static object ToDouble(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            double result = 0;

            if ((args != null) && (args.Length > 0))
            {
                string s = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(s) && !double.TryParse(s, out result))
                {
                    result = double.NaN;
                }
            }

            return result;
        }

        public static object ToGuid(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                return StmCommon.ToGuid(args[0]);
            }

            return Guid.Empty;
        }

        public static object ToInteger(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            long result = 0;

            if ((args != null) && (args.Length > 0))
            {
                string s = StmCommon.ToString(args[0]);
                if ((s != null) && !long.TryParse(s, out result))
                {
                    result = 0;
                }
            }

            return result;
        }

        public static object ToString(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                object obj1 = args[0];
                
                if (obj1 != null)
                {
                    if (args.Length > 1)
                    {
                        string format = StmCommon.ToString(args[1]);

                        if (!String.IsNullOrEmpty(format))
                        {
                            format = format.Trim();

                            if (!String.IsNullOrEmpty(format))
                            {
                                MethodInfo info = obj1.GetType().GetMethod("ToString", new Type[] { typeof(string) });
                                if (info != null)
                                {
                                    ParameterInfo[] prms = info.GetParameters();
                                    if ((prms != null) && prms[0].Name.StartsWith("format", StringComparison.OrdinalIgnoreCase))
                                    {
                                        object result = info.Invoke(obj1, new object[] { format });
                                        return ((result != null) ? result.ToString() : String.Empty);
                                    }
                                }
                            }
                        }
                    }

                    return StmCommon.ToString(obj1);
                }
            }

            return String.Empty;
        }

        public static object ToTime(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            TimeSpan result = TimeSpan.Zero;

            if ((args != null) && (args.Length > 0))
            {
                string s = StmCommon.ToString(args[0]);
                if ((s != null) && !TimeSpan.TryParse(s, out result))
                {
                    result = TimeSpan.Zero;
                }
            }

            return result;
        }

        public static object WriteToTrace(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string category = String.Empty;
                string msg = StmCommon.ToString(args[0]);

                if (args.Length > 1)
                {
                    category = StmCommon.ToString(args[1]);
                }

                Trace.Write(msg, category);
                Trace.Flush();
            }

            return double.NaN;
        }
    }
}
