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

        private delegate object Fn(FunctionEventArgs e, params object[] args);

        private static readonly List<FunctionInfo> _info = new List<FunctionInfo>();
        private static readonly Dictionary<string, Fn> _functions = new Dictionary<string, Fn>();

        #region Constants

        private const string STR_ADDDATE = "ADDDATE";
        private const string STR_ADDTIME = "ADDTIME";
        private const string STR_ADDTIMETODATE = "ADDTIMETODATE";
        private const string STR_SUBTIMEFROMDATE = "SUBTIMEFROMDATE";

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

        private const string STR_ISLEAPYEAR = "ISLEAPYEAR";
        
        private const string STR_YEAR = "YEAR";
        private const string STR_MONTH = "MONTH";
        private const string STR_DAY = "DAY";
        private const string STR_HOUR = "HOUR";
        private const string STR_MINUTE = "MINUTE";
        private const string STR_SECOND = "SECOND";
        private const string STR_MSECOND = "MSECOND";
        private const string STR_MILLISECOND = "MILLISECOND";

        #endregion Constants

        static DateFunctions()
        {
            CreateInfo();
            RegisterFunctions();
        }

        #region Function Registration

        private static void RegisterFunctions()
        {
            _functions[STR_ADDDATE] = AddDate;
            _functions[STR_ADDTIME] = AddTime;
            _functions[STR_ADDTIMETODATE] = AddTimeToDate;
            _functions[STR_SUBTIMEFROMDATE] = SubTimeFromDate;

            _functions[STR_DATE] = Date;

            _functions[STR_DAY] = GetDay;

            _functions[STR_DAYNAME] = DayName;

            _functions[STR_HOUR] = GetHour;

            _functions[STR_LOCALDATE] = ToLocal;
            _functions[STR_ASLOCALDATE] = ToLocal;
            _functions[STR_TOLOCALDATE] = ToLocal;

            _functions[STR_MAXDATE] = MaxDate;

            _functions[STR_MINDATE] = MinDate;

            _functions[STR_MAXTIME] = MaxTime;

            _functions[STR_MILLISECOND] = GetMilliSecond;

            _functions[STR_MINTIME] = MinTime;

            _functions[STR_MINUTE] = GetMinute;

            _functions[STR_MONTH] = GetMonth;

            _functions[STR_MONTHNAME] = MonthName;

            _functions[STR_NOW] = Now;

            _functions[STR_SECOND] = GetSecond;

            _functions[STR_TIME] = TimeOfDay;

            _functions[STR_UTCDATE] = ToUtc;
            _functions[STR_ASUTCDATE] = ToUtc;
            _functions[STR_TOUTCDATE] = ToUtc;

            _functions[STR_YEAR] = GetYear;
            _functions[STR_ISLEAPYEAR] = IsLeapYear;
        }

        private static void CreateInfo()
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
                    new FunctionInfo("AddTimeToDate", 1, 2, new ValueType[] 
                        { 
                            ValueType.DateTime,
                            ValueType.TimeSpan
                        },
                        ReturnType.DateTime), 
                    new FunctionInfo("SubTimeFromDate", 1, 2, new ValueType[] 
                        { 
                            ValueType.DateTime,
                            ValueType.TimeSpan
                        },
                        ReturnType.DateTime), 
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
                    new FunctionInfo("IsLeapYear", 1, 1, new ValueType[] 
                        { 
                            ValueType.Integer
                        }, 
                        ReturnType.String)
                            .AddAlias("IsLeap"),
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
                            .AddAliases( new string[] { "AsLocalDate", "ToLocalDate" } ),
                    new FunctionInfo("UtcDate", 1, 1, new ValueType[] 
                        { 
                            ValueType.DateTime 
                        }, 
                        ReturnType.DateTime)
                            .AddAliases( new string[] { "AsUtcDate", "ToUtcDate" } )
                } );
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

        public static object AddDate(FunctionEventArgs e, params object[] args)
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

        public static object AddTime(FunctionEventArgs e, params object[] args)
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

        public static object AddTimeToDate(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;
                
                DateTime result = DateTime.MinValue;
                if (args[0] != null)
                {
                    result = StmCommon.ToDate(args[0]);
                }

                if (args.Length >= 2)
                {
                    result += StmCommon.ToTime(args[1]);
                }

                return result;
            }

            return DateTime.MinValue;
        }

        public static object SubTimeFromDate(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                DateTime result = DateTime.MinValue;
                if (args[0] != null)
                {
                    result = StmCommon.ToDate(args[0]);
                }

                if (args.Length >= 2)
                {
                    result -= StmCommon.ToTime(args[1]);
                }

                return result;
            }

            return DateTime.MinValue;
        }

        public static object IsLeapYear(FunctionEventArgs e, params object[] args)
        {
            e.Handled = false;
            if ((args != null) && (args.Length > 0))
            {
                e.Handled = true;

                if (args[0] != null)
                {
                    long year = StmCommon.ToInteger(args[0], -1);
                    if (year > 0)
                    {
                        return DateTime.IsLeapYear((int)year);
                    }

                    DateTime dt = StmCommon.ToDate(args[0]);
                    return DateTime.IsLeapYear(dt.Year);
                }
            }

            return false;
        }
       
        public static object GetYear(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Year, args);
        }
                       
        public static object GetMonth(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Month, args);
        }

        public static object GetDay(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Day, args);
        }

        public static object GetHour(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Hour, args);
        }

        public static object GetMinute(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Minute, args);
        }

        public static object GetSecond(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.Second, args);
        }

        public static object GetMilliSecond(FunctionEventArgs e, params object[] args)
        {
            return GetDatePart(e, DatePart.MilliSecond, args);
        }

        private static int GetDatePart(FunctionEventArgs e, DatePart part, params object[] args)
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

        public static object ToLocal(FunctionEventArgs e, params object[] args)
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

        public static object ToUtc(FunctionEventArgs e, params object[] args)
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

        private static CultureInfo GetCulture(object arg)
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

        public static object DayName(FunctionEventArgs e, params object[] args)
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

        public static object MonthName(FunctionEventArgs e, params object[] args)
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

        public static object Now(FunctionEventArgs e, params object[] args)
        {
            return DateTime.Now;
        }

        public static object Date(FunctionEventArgs e, params object[] args)
        {
            return DateTime.Now.Date;
        }

        public static object MaxDate(FunctionEventArgs e, params object[] args)
        {
            return DateTime.MaxValue;
        }

        public static object MinDate(FunctionEventArgs e, params object[] args)
        {
            return DateTime.MinValue;
        }

        public static object MaxTime(FunctionEventArgs e, params object[] args)
        {
            return TimeSpan.MaxValue;
        }

        public static object MinTime(FunctionEventArgs e, params object[] args)
        {
            return TimeSpan.MinValue;
        }

        public static object TimeOfDay(FunctionEventArgs e, params object[] args)
        {
            return DateTime.Now.TimeOfDay;
        }
    }
}

