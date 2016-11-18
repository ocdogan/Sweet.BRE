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
    public sealed class MathFunctions : IFunctionHandler
    {
        private delegate object Fn(FunctionEventArgs e, params object[] args);

        private static readonly List<FunctionInfo> _info = new List<FunctionInfo>();
        private static readonly Dictionary<string, Fn> _functions = new Dictionary<string, Fn>();

        #region Constants

        private const string STR_ABS = "ABS";
        private const string STR_ATAN = "ATAN";
        private const string STR_AVERAGE = "AVERAGE";
        private const string STR_CEILING = "CEILING";
        private const string STR_COS = "COS";
        private const string STR_COSH = "COSH";
        private const string STR_FLOOR = "FLOOR";
        private const string STR_LOG = "LOG";
        private const string STR_LOG10 = "LOG10";
        private const string STR_LOGARITHM = "LOGARITHM";
        private const string STR_LOGARITHM10 = "LOGARITHM10";
        private const string STR_MAX = "MAX";
        private const string STR_MIN = "MIN";
        private const string STR_MEAN = "MEAN";
        private const string STR_MEDIAN = "MEDIAN";
        private const string STR_PI = "PI";
        private const string STR_POW = "POW";
        private const string STR_ROUND = "ROUND";
        private const string STR_SIN = "SIN";
        private const string STR_SUM = "SUM";
        private const string STR_SQRT = "SQRT";
        private const string STR_TAN = "TAN";
        private const string STR_TRUNCATE = "TRUNCATE";

        private const string STR_MAXFLOAT = "MAXFLOAT";
        private const string STR_MINFLOAT = "MINFLOAT";
        private const string STR_MAXINTEGER = "MAXINTEGER";
        private const string STR_MININTEGER = "MININTEGER";

        #endregion Constants

        static MathFunctions()
        {
            CreateInfo();
            RegisterFunctions();
        }

        #region Function Registration

        private static void RegisterFunctions()
        {
            _functions[STR_ABS] = Abs;

            _functions[STR_ATAN] = Atan;

            _functions[STR_AVERAGE] = Mean;

            _functions[STR_CEILING] = Ceiling;

            _functions[STR_COS] = Cos;

            _functions[STR_COSH] = Cosh;

            _functions[STR_FLOOR] = Floor;

            _functions[STR_LOG] = Log;

            _functions[STR_LOGARITHM] = Logarithm;

            _functions[STR_LOG10] = Log10;
            _functions[STR_LOGARITHM10] = Log10;

            _functions[STR_MAX] = Max;

            _functions[STR_MAXFLOAT] = MaxDouble;

            _functions[STR_MAXINTEGER] = MaxLong;

            _functions[STR_MEAN] = Mean;

            _functions[STR_MEDIAN] = Median;

            _functions[STR_MIN] = Min;

            _functions[STR_MINFLOAT] = MinDouble;

            _functions[STR_MININTEGER] = MinLong;

            _functions[STR_PI] = Pi;

            _functions[STR_POW] = Pow;

            _functions[STR_ROUND] = Round;

            _functions[STR_SIN] = Sin;

            _functions[STR_SUM] = Sum;

            _functions[STR_SQRT] = Sqrt;

            _functions[STR_TAN] = Tan;

            _functions[STR_TRUNCATE] = Truncate;
        }

        private static void CreateInfo()
        {
            _info.AddRange(
                new FunctionInfo[] { 
                    new FunctionInfo("Abs", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float),
                    new FunctionInfo("Atan", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Avg", 1, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Ceiling", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float),
                    new FunctionInfo("Cos", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Cosh", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Floor", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float),
                    new FunctionInfo("Log", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Logarithm", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Log10", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Logarithm10", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Mean", 1, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Median", 1, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Max", 2, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Min", 2, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("MaxFloat", 0, 0, null, ReturnType.Float), 
                    new FunctionInfo("MaxInteger", 0, 0, null, ReturnType.Integer), 
                    new FunctionInfo("MinFloat", 0, 0, null, ReturnType.Float), 
                    new FunctionInfo("MinInteger", 0, 0, null, ReturnType.Integer), 
                    new FunctionInfo("Pi", 0, 0, null, ReturnType.Float), 
                    new FunctionInfo("Pow", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Round", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Sin", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Sum", 1, int.MaxValue, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Sqrt", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float), 
                    new FunctionInfo("Tan", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float),
                    new FunctionInfo("Truncate", 1, 1, new ValueType[] 
                        {
                            ValueType.Float
                        },
                        ReturnType.Float)
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

        public static object Abs(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                double d = StmCommon.ToDouble(args[0]);
                return Math.Abs(d);
            }

            return 0;
        }

        public static object Atan(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                double d = StmCommon.ToDouble(args[0]);
                return Math.Atan(d);
            }

            return 0;
        }

        public static object Ceiling(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false; 
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Ceiling(d);
            }

            return 0;
        }

        public static object Cos(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Cos(d);
            }

            return 0;
        }

        public static object Cosh(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Cosh(d);
            }

            return 0;
        }

        public static object Floor(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Floor(d);
            }

            return 0;
        }

        public static object Log(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                object obj = args[0];
                if (obj != null)
                {
                    if (RuleCommon.IsNumber(obj))
                    {
                        e.Handled = true;

                        double d = StmCommon.ToDouble(args[0]);
                        return Math.Log(d);
                    }

                    string s = StmCommon.ToString(obj);
                    if (!String.IsNullOrEmpty(s))
                    {
                        double d;
                        if (double.TryParse(s, out d))
                        {
                            e.Handled = true;
                            return Math.Log(d);
                        }
                    }
                }
            }

            return 0;
        }

        public static object Log10(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                double d = StmCommon.ToDouble(args[0]);
                return Math.Log10(d);
            }

            return 0;
        }

        public static object Logarithm(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                double d = StmCommon.ToDouble(args[0]);
                return Math.Log(d);
            }

            return 0;
        }

        public static object Max(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;
                
                double d1 = StmCommon.ToDouble(args[0]);
                double d2 = StmCommon.ToDouble(args[1]);

                double result = Math.Max(d1, d2);
                if (args.Length > 2)
                {
                    for (int i = 2; i < args.Length; i++)
                    {
                        d1 = StmCommon.ToDouble(args[i]);
                        result = Math.Max(result, d1);
                    }
                }

                return result;
            }

            return 0;
        }

        public static object Mean(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                return (double)Sum(e, args) / args.Length;
            }

            return 0;
        }

        public static object Median(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;

                double[] arrData = new double[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    arrData[i] = StmCommon.ToDouble(args[i]);
                }

                Array.Sort<double>(arrData);

                double mid = 0;
                double midDBLItem = ((arrData.Length + 1) / 2) - 1;
                int midItem = Convert.ToInt32(Math.Floor(midDBLItem));

                if ((arrData.Length % 2) == 0)
                {
                    // there is an even number of items in the array
                    double item1 = arrData[midItem];
                    double item2 = arrData[midItem + 1];

                    mid = (item1 + item2) / 2;
                }
                else
                {
                    // there is an odd number of items in the array.
                    mid = arrData[midItem];
                }

                return mid;
            }

            return 0;
        }

        public static object Min(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;
                
                double d1 = StmCommon.ToDouble(args[0]);
                double d2 = StmCommon.ToDouble(args[1]);

                double result = Math.Min(d1, d2);
                if (args.Length > 2)
                {
                    for (int i = 2; i < args.Length; i++)
                    {
                        d1 = StmCommon.ToDouble(args[i]);
                        result = Math.Min(result, d1);
                    }
                }
            }

            return 0;
        }

        public static object Pow(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 1))
            {
                e.Handled = true;
                
                double d1 = StmCommon.ToDouble(args[0]);
                double d2 = StmCommon.ToDouble(args[1]);

                return Math.Pow(d1, d2);
            }

            return 0;
        }

        public static object Round(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                int len = args.Length;
                double d1 = StmCommon.ToDouble(args[0]);

                long decimals = 0;
                if (len == 2)
                {
                    decimals = StmCommon.ToInteger(args[1]);
                }

                MidpointRounding mode = MidpointRounding.ToEven;
                if (len == 3)
                {
                    string s = StmCommon.ToString(args[2]);
                    s = ((s != null) ? s.Trim() : s);

                    if (!String.IsNullOrEmpty(s))
                    {
                        if (CommonHelper.IsNumber(s))
                        {
                            mode = (MidpointRounding)StmCommon.ToInteger(s);
                        }
                        else
                        {
                            object obj = null;
                            if (CommonHelper.TryParse(typeof(MidpointRounding), s, out obj))
                            {
                                mode = (MidpointRounding)obj;
                            }
                        }
                    }
                }

                return Math.Round(d1, (int)decimals, mode);
            }

            return 0;
        }

        public static object Sin(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Sin(d);
            }

            return 0;
        }

        public static object Sum(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                double result = 0;
                foreach (object o in args)
                {
                    double d = StmCommon.ToDouble(0);
                    if (!double.IsNaN(d))
                    {
                        result += d;
                    }
                }

                return result;
            }

            return 0;
        }

        public static object Sqrt(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Sqrt(d);
            }

            return 0;
        }

        public static object Tan(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Tan(d);
            }

            return 0;
        }
        
        public static object Truncate(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                double d = StmCommon.ToDouble(args[0]);
                return Math.Truncate(d);
            }

            return 0;
        }

        public static object Pi(FunctionEventArgs e, params object[] args)
        {
            return Math.PI;
        }

        public static object MaxDouble(FunctionEventArgs e, params object[] args)
        {
            return double.MaxValue;
        }

        public static object MinDouble(FunctionEventArgs e, params object[] args)
        {
            return double.MinValue;
        }

        public static object MaxLong(FunctionEventArgs e, params object[] args)
        {
            return long.MaxValue;
        }

        public static object MinLong(FunctionEventArgs e, params object[] args)
        {
            return long.MinValue;
        }
    }
}
