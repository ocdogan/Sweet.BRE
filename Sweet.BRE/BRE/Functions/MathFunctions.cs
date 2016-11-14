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

        private List<FunctionInfo> _info;

        public MathFunctions()
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

        public void Eval(FunctionEventArgs e)
        {
            string function = e.Name;

            function = (function != null ? function.Trim().ToUpperInvariant() : String.Empty);

            e.Handled = true;
            object result = e.Result;

            switch (function)
            {
                case STR_ABS:
                    result = Abs(e, e.Args);
                    break;

                case STR_ATAN:
                    result = Atan(e, e.Args);
                    break;

                case STR_AVERAGE:
                    result = Mean(e, e.Args);
                    break;

                case STR_CEILING:
                    result = Ceiling(e, e.Args);
                    break;

                case STR_COS:
                    result = Cos(e, e.Args);
                    break;

                case STR_COSH:
                    result = Cosh(e, e.Args);
                    break;

                case STR_FLOOR:
                    result = Floor(e, e.Args);
                    break;

                case STR_LOG:
                    result = Log(e, e.Args);
                    break;

                case STR_LOGARITHM:
                    result = Logarithm(e, e.Args);
                    break;

                case STR_LOG10:
                case STR_LOGARITHM10:
                    result = Log10(e, e.Args);
                    break;

                case STR_MAX:
                    result = Max(e, e.Args);
                    break;

                case STR_MAXFLOAT:
                    result = double.MaxValue;
                    break;

                case STR_MAXINTEGER:
                    result = long.MaxValue;
                    break;

                case STR_MEAN:
                    result = Mean(e, e.Args);
                    break;

                case STR_MEDIAN:
                    result = Median(e, e.Args);
                    break;

                case STR_MIN:
                    result = Min(e, e.Args);
                    break;

                case STR_MINFLOAT:
                    result = double.MinValue;
                    break;

                case STR_MININTEGER:
                    result = long.MinValue;
                    break;

                case STR_PI:
                    result = Math.PI;
                    break;

                case STR_POW:
                    result = Pow(e, e.Args);
                    break;

                case STR_ROUND:
                    result = Round(e, e.Args);
                    break;

                case STR_SIN:
                    result = Sin(e, e.Args);
                    break;

                case STR_SUM:
                    result = Sum(e, e.Args);
                    break;

                case STR_SQRT:
                    result = Sqrt(e, e.Args);
                    break;

                case STR_TAN:
                    result = Tan(e, e.Args);
                    break;

                case STR_TRUNCATE:
                    result = Truncate(e, e.Args);
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

        public double Abs(FunctionEventArgs e, params object[] args)
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

        public double Atan(FunctionEventArgs e, params object[] args)
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

        public double Ceiling(FunctionEventArgs e, params object[] args)
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

        public double Cos(FunctionEventArgs e, params object[] args)
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

        public double Cosh(FunctionEventArgs e, params object[] args)
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

        public double Floor(FunctionEventArgs e, params object[] args)
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

        public double Log(FunctionEventArgs e, params object[] args)
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

        public double Log10(FunctionEventArgs e, params object[] args)
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

        public double Logarithm(FunctionEventArgs e, params object[] args)
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

        public double Max(FunctionEventArgs e, params object[] args)
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

        public double Mean(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                return Sum(e, args) / args.Length;
            }

            return 0;
        }

        public double Median(FunctionEventArgs e, params object[] args)
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

        public double Min(FunctionEventArgs e, params object[] args)
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

        public double Pow(FunctionEventArgs e, params object[] args)
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

        public double Round(FunctionEventArgs e, params object[] args)
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

        public double Sin(FunctionEventArgs e, params object[] args)
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

        public double Sum(FunctionEventArgs e, params object[] args)
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

        public double Sqrt(FunctionEventArgs e, params object[] args)
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

        public double Tan(FunctionEventArgs e, params object[] args)
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

        public double Truncate(FunctionEventArgs e, params object[] args)
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
    }
}
