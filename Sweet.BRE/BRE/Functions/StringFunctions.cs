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
using System.Globalization;
using System.Text;

namespace Sweet.BRE
{
    public sealed class StringFunctions : IFunctionHandler
    {
        private delegate object Fn(FunctionEventArgs e, params object[] args);

        private static readonly List<FunctionInfo> _info = new List<FunctionInfo>();
        private static readonly Dictionary<string, Fn> _functions = new Dictionary<string, Fn>();

        #region Constants

        private const string STR_CONCAT = "CONCAT";
        private const string STR_STRCONTAINS = "STRCONTAINS";
        private const string STR_ENDSWITH = "ENDSWITH";
        private const string STR_FORMAT = "FORMAT";
        private const string STR_STRINDEX = "STRINDEX";
        private const string STR_STRINDEXOF = "STRINDEXOF";
        private const string STR_INSERT = "INSERT";
        private const string STR_ISNULLOREMPTY = "ISNULLOREMPTY";
        private const string STR_JOIN = "JOIN";
        private const string STR_STRLASTINDEX = "STRLASTINDEX";
        private const string STR_STRLASTINDEXOF = "STRLASTINDEXOF";
        private const string STR_STRLASTPOS = "STRLASTPOS";
        private const string STR_STRLASTPOSITION = "STRLASTPOSITION";
        private const string STR_STRLASTPOSITIONOF = "STRLASTPOSITIONOF";
        private const string STR_PADLEFT = "PADLEFT";
        private const string STR_PADRIGHT = "PADRIGHT";
        private const string STR_STRPOS = "STRPOS";
        private const string STR_STRPOSITION = "STRPOSITION";
        private const string STR_STRPOSITIONOF = "STRPOSITIONOF";
        private const string STR_REMOVE = "REMOVE";
        private const string STR_REPLACE = "REPLACE";
        private const string STR_SPLIT = "SPLIT";
        private const string STR_STARTSWITH = "STARTSWITH";
        private const string STR_SUBSTRING = "SUBSTRING";
        private const string STR_TOLOWER = "TOLOWER";
        private const string STR_TOUPPER = "TOUPPER";
        private const string STR_TRIM = "TRIM";
        private const string STR_TRIMEND = "TRIMEND";
        private const string STR_TRIMSTART = "TRIMSTART";

        #endregion Constants

        static StringFunctions()
        {
            CreateInfo();
            RegisterFunctions();
        }

        #region Function Registration

        private static void RegisterFunctions()
        {
            _functions[STR_CONCAT] = Concat;

            _functions[STR_STRCONTAINS] = Contains;

            _functions[STR_ENDSWITH] = EndsWith;

            _functions[STR_FORMAT] = Format;

            _functions[STR_STRINDEX] = IndexOf;
            _functions[STR_STRINDEXOF] = IndexOf;
            _functions[STR_STRPOS] = IndexOf;
            _functions[STR_STRPOSITION] = IndexOf;
            _functions[STR_STRPOSITIONOF] = IndexOf;

            _functions[STR_INSERT] = Insert;

            _functions[STR_ISNULLOREMPTY] = IsNullOrEmpty;

            _functions[STR_JOIN] = Join;

            _functions[STR_STRLASTINDEX] = LastIndexOf;
            _functions[STR_STRLASTINDEXOF] = LastIndexOf;
            _functions[STR_STRLASTPOS] = LastIndexOf;
            _functions[STR_STRLASTPOSITION] = LastIndexOf;
            _functions[STR_STRLASTPOSITIONOF] = LastIndexOf;

            _functions[STR_PADLEFT] = PadLeft;

            _functions[STR_PADRIGHT] = PadRight;

            _functions[STR_REMOVE] = Remove;

            _functions[STR_REPLACE] = Replace;

            _functions[STR_SPLIT] = Split;

            _functions[STR_STARTSWITH] = StartsWith;

            _functions[STR_SUBSTRING] = Substring;

            _functions[STR_TOLOWER] = ToLower;

            _functions[STR_TOUPPER] = ToUpper;

            _functions[STR_TRIM] = Trim;

            _functions[STR_TRIMEND] = TrimEnd;

            _functions[STR_TRIMSTART] = TrimStart;
        }

        private static void CreateInfo()
        {
            _info.AddRange(
                new FunctionInfo[] { 
                    new FunctionInfo("Concat", 1, int.MaxValue, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String),
                    new FunctionInfo("EndsWith", 2, 2, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("Format", 2, 11, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String),
                    new FunctionInfo("Insert", 2, 3, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String,
                            ValueType.Integer
                        },
                        ReturnType.String), 
                    new FunctionInfo("IsNullOrEmpty", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("Join", 3, int.MaxValue, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("PadLeft", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("PadRight", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("Remove", 2, 3, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.Integer,
                            ValueType.Integer
                        },
                        ReturnType.String), 
                    new FunctionInfo("Replace", 2, 2, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("Split", 2, 2, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("StartsWith", 2, 2, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("StrContains", 2, 2, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String
                        },
                        ReturnType.String),
                    new FunctionInfo("StrIndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String,
                            ValueType.Integer
                        },
                        ReturnType.String)
                            .AddAliases(new string[] { "StrIndex", "StrPos", "StrPosition", "StrPositionOf" }),
                    new FunctionInfo("StrLastIndexOf", 2, 3, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.String,
                            ValueType.Integer
                        },
                        ReturnType.String),
                    new FunctionInfo("Substring", 2, 3, new ValueType[] 
                        {
                            ValueType.String,
                            ValueType.Integer,
                            ValueType.Integer
                        },
                        ReturnType.String), 
                    new FunctionInfo("ToLower", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("ToUpper", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("Trim", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("TrimEnd", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String), 
                    new FunctionInfo("TrimStart", 1, 1, new ValueType[] 
                        {
                            ValueType.String
                        },
                        ReturnType.String) 
                }
                );
        }

        #endregion Function Registration

        public FunctionInfo[] HandledFunctions
        {
            get
            {
                return _info.ToArray();
            }
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

        public static object Concat(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if (args != null)
            {
                foreach (object o1 in args)
                {
                    result += StmCommon.ToString(o1);
                }
            }

            return result;
        }

        public static object Contains(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);
                string s2 = StmCommon.ToString(args[1]);

                if (!String.IsNullOrEmpty(s1) && !String.IsNullOrEmpty(s2))
                {
                    return s1.Contains(s2);
                }
            }

            return false;
        }

        public static object EndsWith(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(s1))
                {
                    string s2 = StmCommon.ToString(args[1]);
                    return s1.EndsWith(s2);
                }
            }

            return false;
        }

        public static object Format(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string format = StmCommon.ToString(args[0]);
                if (args.Length == 1)
                {
                    return format;
                }

                if (!String.IsNullOrEmpty(format))
                {
                    string[] sa = new string[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        sa[i-1] = StmCommon.ToString(args[i]);
                    }

                    return String.Format(format, sa);
                }
            }

            return String.Empty;
        }

        public static object IndexOf(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 1))
            {
                string s1 = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(s1))
                {
                    int startIndex = 0;
                    if (args.Length >= 3)
                    {
                        startIndex = (int)StmCommon.ToInteger(args[2], 0);
                    }

                    string s2 = StmCommon.ToString(args[1]);
                    return s1.IndexOf(s2, startIndex);
                }
            }

            return -1;
        }

        public static object Insert(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 2))
            {
                string s1 = StmCommon.ToString(args[0]);
                string s2 = StmCommon.ToString(args[1]);

                if (String.IsNullOrEmpty(s1))
                {
                    return s2; 
                }

                if (!String.IsNullOrEmpty(s2))
                {
                    string s3 = StmCommon.ToString(args[2]);

                    int startIndex = 0;
                    int.TryParse(s3, out startIndex);

                    return s1.Insert(startIndex, s2);
                }
            }

            return String.Empty;
        }

        public static object IsNullOrEmpty(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 0))
            {
                return String.IsNullOrEmpty(StmCommon.ToString(args[0]));
            }

            return true;
        }

        public static object Join(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 2))
            {
                StringBuilder sb = new StringBuilder();

                string delimeter = StmCommon.ToString(args[0]);

                int len = args.Length;
                for (int i = 1; i < len; i++)
                {
                    sb.Append(StmCommon.ToString(args[i]));
                    if (i < len - 1)
                    {
                        sb.Append(delimeter);
                    }
                }
            }

            return String.Empty;
        }

        public static object LastIndexOf(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;
            if ((args != null) && (args.Length > 1))
            {
                string s1 = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(s1))
                {
                    int startIndex = 0;
                    if (args.Length >= 3)
                    {
                        startIndex = (int)StmCommon.ToInteger(args[2], 0);
                    }

                    string s2 = StmCommon.ToString(args[1]);
                    return s1.LastIndexOf(s2, startIndex);
                }
            }

            return -1;
        }

        public static object PadLeft(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                string s1 = StmCommon.ToString(args[0]);
                string s2 = StmCommon.ToString(args[1]);

                if (!String.IsNullOrEmpty(s2))
                {
                    int totalWidth = 0;
                    if (int.TryParse(s2, out totalWidth))
                    {
                        e.Handled = true;

                        if (args.Length > 2)
                        {
                            string s3 = StmCommon.ToString(args[2]);
                            if (!String.IsNullOrEmpty(s3))
                            {
                                return s1.PadLeft(totalWidth, s3[0]);
                            }
                        }

                        return s1.PadLeft(totalWidth);
                    }
                }
            }

            return String.Empty;
        }

        public static object PadRight(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                string s1 = StmCommon.ToString(args[0]);
                string s2 = StmCommon.ToString(args[1]);

                if (!String.IsNullOrEmpty(s2))
                {
                    int totalWidth = 0;
                    if (int.TryParse(s2, out totalWidth))
                    {
                        e.Handled = true;

                        if (args.Length > 2)
                        {
                            string s3 = StmCommon.ToString(args[2]);
                            if (!String.IsNullOrEmpty(s3))
                            {
                                return s1.PadRight(totalWidth, s3[0]);
                            }
                        }

                        return s1.PadRight(totalWidth);
                    }
                }
            }

            return String.Empty;
        }

        public static object Remove(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);

                if (!String.IsNullOrEmpty(s1))
                {
                    if (args.Length == 1)
                    {
                        return String.Empty;
                    }

                    string s2 = StmCommon.ToString(args[1]);

                    int startIndex = 0;
                    int.TryParse(s2, out startIndex);

                    if (args.Length == 2)
                    {
                        return s1.Remove(startIndex);
                    }

                    string s3 = StmCommon.ToString(args[2]);

                    int count = 0;
                    int.TryParse(s3, out count);

                    return s1.Remove(startIndex, count);
                }
            }

            return String.Empty;
        }

        public static object Replace(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 2))
            {
                string s1 = StmCommon.ToString(args[0]);

                if (!String.IsNullOrEmpty(s1))
                {
                    e.Handled = true;

                    string s2 = StmCommon.ToString(args[1]);
                    string s3 = StmCommon.ToString(args[2]);

                    return s1.Replace(s2, s3);
                }
            }

            return String.Empty;
        }

        public static object Split(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);

                if (!String.IsNullOrEmpty(s1))
                {
                    string s2 = StmCommon.ToString(args[1]);
                    if (String.IsNullOrEmpty(s2))
                    {
                        return (new string[] { s1 });
                    }

                    return s1.Split(s2[0]);
                }
            }

            return (new string[0]);
        }

        public static object StartsWith(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);
                if (!String.IsNullOrEmpty(s1))
                {
                    string s2 = StmCommon.ToString(args[1]);
                    return s1.StartsWith(s2);
                }
            }

            return false;
        }

        public static object Substring(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                string s1 = StmCommon.ToString(args[0]);

                if (!String.IsNullOrEmpty(s1))
                {
                    if (args.Length == 1)
                    {
                        return String.Empty;
                    }

                    string s2 = StmCommon.ToString(args[1]);

                    int startIndex = 0;
                    int.TryParse(s2, out startIndex);

                    if (args.Length == 2)
                    {
                        return s1.Substring(startIndex);
                    }

                    string s3 = StmCommon.ToString(args[2]);

                    int length = 0;
                    int.TryParse(s3, out length);

                    return s1.Substring(startIndex, length);
                }
            }

            return String.Empty;
        }

        public static object ToLower(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if ((args != null) && (args.Length > 0))
            {
                result = StmCommon.ToString(args[0]);
                if (result != null)
                {
                    result = result.ToLowerInvariant();
                }
            }

            return result;
        }

        public static object ToUpper(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if ((args != null) && (args.Length > 0))
            {
                result = StmCommon.ToString(args[0]);
                if (result != null)
                {
                    result = result.ToUpperInvariant();
                }
            }

            return result;
        }

        public static object Trim(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if ((args != null) && (args.Length > 0))
            {
                result = StmCommon.ToString(args[0]);
                if (result != null)
                {
                    result = result.Trim();
                }
            }

            return result;
        }

        public static object TrimEnd(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if ((args != null) && (args.Length > 0))
            {
                result = StmCommon.ToString(args[0]);
                if (result != null)
                {
                    result = result.TrimEnd();
                }
            }

            return result;
        }

        public static object TrimStart(FunctionEventArgs e, params object[] args)
        {
            e.Handled = true;

            string result = String.Empty;
            if ((args != null) && (args.Length > 0))
            {
                result = StmCommon.ToString(args[0]);
                if (result != null)
                {
                    result = result.TrimStart();
                }
            }

            return result;
        }
    }
}
