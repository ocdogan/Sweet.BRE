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
    public sealed class DateFunctions : IFunctionHandler
    {
        public enum DatePart
        {
            Year,
            Month,
            Day,
            Hour,
            Minute,
            Second,
            MilliSecond
        }

        private const string STR_ADDDATE = "ADDDATE";
        private const string STR_ADDTIME = "ADDTIME";
        private const string STR_DATE = "DATE";
        private const string STR_NOW = "NOW";
        private const string STR_TIME = "TIME";

        private const string STR_ASLOCALDATE = "ASLOCALDATE";
        private const string STR_TOLOCALDATE = "TOLOCALDATE";
        private const string STR_LOCALDATE = "LOCALDATE";

        private const string STR_ASUTCDATE = "ASUTCDATE";
        private const string STR_TOUTCDATE = "TOUTCDATE";
        private const string STR_UTCDATE = "UTCDATE";

        private const string STR_MAXDATE = "MAXDATE";
        private const string STR_MINDATE = "MINDATE";
        private const string STR_MAXTIME = "MAXTIME";
        private const string STR_MINTIME = "MINTIME";

        private const string STR_DAYNAME = "DAYNAME";
        private const string STR_MONTHNAME = "MONTHNAME";

        private const string STR_YEAR = "YEAR";
        private const string STR_MONTH = "MONTH";
        private const string STR_DAY = "DAY";
        private const string STR_HOUR = "HOUR";
        private const string STR_MINUTE = "MINUTE";
        private const string STR_SECOND = "SECOND";
        private const string STR_MSECOND = "MSECOND";
        private const string STR_MILLISECOND = "MILLISECOND";

        private List<FunctionInfo> _info;

        public DateFunctions()
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
                    new FunctionInfo("AddDate", 1, 8, new ValueType[] 
                        { 
                            ValueType.DateTime,
                            ValueType.Integer,
                            ValueType.Integer,
                            ValueType.Float,
                            ValueType.Float,
                            ValueType.Float,
                            ValueType.Float,
                            ValueType.Float
                        },
                        ReturnType.DateTime), 
                    new FunctionInfo("AddTime", 1, 5, new ValueType[] 
                        {
                            ValueType.TimeSpan,
                            ValueType.Float,
                            ValueType.Float,
                            ValueType.Float,
                            ValueType.Float
                        },
                        ReturnType.TimeSpan),
                    new FunctionInfo("Date", 0, 0, null, ReturnType.DateTime),
                    new FunctionInfo("MaxDate", 0, 0, null, ReturnType.DateTime),
                    new FunctionInfo("MinDate", 0, 0, null, ReturnType.DateTime),
                    new FunctionInfo("MaxTime", 0, 0, null, ReturnType.TimeSpan),
                    new FunctionInfo("MinTime", 0, 0, null, ReturnType.TimeSpan),
                    new FunctionInfo("Now", 0, 0, null, ReturnType.DateTime),
                    new FunctionInfo("Time", 0, 0, null, ReturnType.TimeSpan),
                    new FunctionInfo("DayName", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.String),
                    new FunctionInfo("MonthName", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.String),
                    new FunctionInfo("Year", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("Month", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("Day", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("Hour", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("Minute", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("Second", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime),
                    new FunctionInfo("MilliSecond", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime)
                            .AddAlias("MSecond"),
                    new FunctionInfo("LocalDate", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime)
                            .AddAliases( new string[] { "AsLocalDate", "ToLocalDate" }),
                    new FunctionInfo("UtcDate", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime)
                            .AddAliases( new string[] { "AsUtcDate", "ToUtcDate" })
                });
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
                case STR_ADDDATE:
                    result = AddDate(e, e.Args);
                    break;

                case STR_ADDTIME:
                    result = AddTime(e, e.Args);
                    break;

                case STR_DATE:
                    result = DateTime.Now.Date;
                    break;

                case STR_DAY:
                    result = GetPart(e, DatePart.Day, e.Args);
                    break;

                case STR_DAYNAME:
                    result = DayName(e, e.Args);
                    break;

                case STR_HOUR:
                    result = GetPart(e, DatePart.Hour, e.Args);
                    break;

                case STR_LOCALDATE:
                case STR_ASLOCALDATE:
                case STR_TOLOCALDATE:
                    result = ToLocal(e, e.Args);
                    break;

                case STR_MAXDATE:
                    result = DateTime.MaxValue;
                    break;

                case STR_MINDATE:
                    result = DateTime.MinValue;
                    break;

                case STR_MAXTIME:
                    result = TimeSpan.MaxValue;
                    break;

                case STR_MILLISECOND:
                    result = GetPart(e, DatePart.MilliSecond, e.Args);
                    break;

                case STR_MINTIME:
                    result = TimeSpan.MinValue;
                    break;

                case STR_MINUTE:
                    result = GetPart(e, DatePart.Minute, e.Args);
                    break;

                case STR_MONTH:
                    result = GetPart(e, DatePart.Month, e.Args);
                    break;

                case STR_MONTHNAME:
                    result = MonthName(e, e.Args);
                    break;

                case STR_NOW:
                    result = DateTime.Now;
                    break;

                case STR_SECOND:
                    result = GetPart(e, DatePart.Second, e.Args);
                    break;

                case STR_TIME:
                    result = DateTime.Now.TimeOfDay;
                    break;

                case STR_UTCDATE:
                case STR_ASUTCDATE:
                case STR_TOUTCDATE:
                    result = ToUtc(e, e.Args);
                    break;

                case STR_YEAR:
                    result = GetPart(e, DatePart.Year, e.Args);
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

        public DateTime AddDate(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                int len = args.Length;
                
                DateTime result = DateTime.MinValue;
                if (args[0] != null)
                {
                    result = StmCommon.ToDate(args[0]);
                }

                if (len >= 8)
                {
                    double milliseconds = StmCommon.ToDouble(args[7], 0);
                    result = result.AddMilliseconds(milliseconds);
                }

                if (len >= 7)
                {
                    double seconds = StmCommon.ToDouble(args[6], 0);
                    result = result.AddSeconds(seconds);
                }

                if (len >= 6)
                {
                    double minutes = StmCommon.ToDouble(args[5], 0);
                    result = result.AddMinutes(minutes);
                }

                if (len >= 5)
                {
                    double hours = StmCommon.ToDouble(args[4], 0);
                    result = result.AddHours(hours);
                }

                if (len >= 4)
                {
                    double days = StmCommon.ToDouble(args[3], 0);
                    result = result.AddDays(days);
                }

                if (len >= 3)
                {
                    int months = (int)StmCommon.ToInteger(args[2], 0);
                    result = result.AddMonths(months);
                }

                if (len >= 2)
                {
                    int years = (int)StmCommon.ToInteger(args[1], 0);
                    result = result.AddYears(years);
                }

                return result;
            }

            return DateTime.MinValue;
        }

        public TimeSpan AddTime(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                int len = args.Length;

                TimeSpan result = TimeSpan.MinValue;
                if (args[0] != null)
                {
                    result = StmCommon.ToTime(args[0]);
                }

                if (len >= 5)
                {
                    double milliseconds = StmCommon.ToDouble(args[4], 0);
                    result = result + TimeSpan.FromMilliseconds(milliseconds);
                }

                if (len >= 4)
                {
                    double seconds = StmCommon.ToDouble(args[3], 0);
                    result = result + TimeSpan.FromSeconds(seconds);
                }

                if (len >= 3)
                {
                    double minutes = StmCommon.ToDouble(args[2], 0);
                    result = result + TimeSpan.FromMinutes(minutes);
                }

                if (len >= 2)
                {
                    double hours = StmCommon.ToDouble(args[1], 0);
                    result = result + TimeSpan.FromHours(hours);
                }

                return result;
            }

            return TimeSpan.Zero;
        }

        public int GetPart(FunctionEventArgs e, DatePart part, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                DateTime d = StmCommon.ToDate(args[0]);
                switch (part)
                {
                    case DatePart.Year:
                        return d.Year;

                    case DatePart.Month:
                        return d.Month;

                    case DatePart.Day:
                        return d.Day;

                    case DatePart.Hour:
                        return d.Hour;

                    case DatePart.Minute:
                        return d.Minute;

                    case DatePart.Second:
                        return d.Second;

                    case DatePart.MilliSecond:
                        return d.Millisecond;
                }
            }

            return 0;
        }

        public DateTime ToLocal(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                DateTime result = StmCommon.ToDate(args[0]);
                result = result.ToLocalTime();

                return result;
            }

            return DateTime.MinValue;
        }

        public DateTime ToUtc(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                DateTime result = StmCommon.ToDate(args[0]);
                result = result.ToUniversalTime();

                return result;
            }

            return DateTime.MinValue;
        }

        private CultureInfo GetCulture(object arg)
        {
            CultureInfo ci = null;
            if (arg != null)
            {
                string cultureName = StmCommon.ToString(arg);
                if (!String.IsNullOrEmpty(cultureName))
                {
                    cultureName = cultureName.Trim();
                    if (!String.IsNullOrEmpty(cultureName))
                    {
                        ci = CultureInfo.CreateSpecificCulture(cultureName);
                    }
                }
            }

            return ((ci != null) ? ci : CultureInfo.InvariantCulture);
        }

        public string DayName(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                DateTime d = StmCommon.ToDate(args[0]);

                CultureInfo ci = CultureInfo.InvariantCulture;
                if (args.Length > 1)
                {
                    ci = GetCulture(args[1]);
                }

                return ci.DateTimeFormat.GetDayName(d.DayOfWeek); 
            }

            return String.Empty;
        }

        public string MonthName(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                DateTime d = StmCommon.ToDate(args[0]);

                CultureInfo ci = CultureInfo.InvariantCulture;
                if (args.Length > 1)
                {
                    ci = GetCulture(args[1]);
                }

                return ci.DateTimeFormat.GetMonthName(d.Month);
            }

            return String.Empty;
        }
    }
}
