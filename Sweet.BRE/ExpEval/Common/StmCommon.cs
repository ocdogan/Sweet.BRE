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
    public static class StmCommon
    {
        public static bool Between(object value, object min, object max)
        {
            return (Compare(value, min) >= 0) && (Compare(value, max) <= 0);
        }

        public static int Compare(object o1, object o2)
        {
            int i1 = (o1 != null ? 1 : 0);
            int i2 = (o2 != null ? 1 : 0);

            if ((i1 == 0) || (i2 == 0))
            {
                return i1.CompareTo(i2);
            }

            ValueType t1 = GetType(o1);
            switch (t1)
            {
                case ValueType.Boolean:
                    int b1 = ((bool)o1 ? 1 : 0);
                    int b2 = ((bool)o2 ? 1 : 0);

                    return b1.CompareTo(b2);

                case ValueType.DateTime:
                    DateTime dt1 = ToDate(o1);
                    DateTime dt2 = ToDate(o2);

                    return dt1.CompareTo(dt2);

                case ValueType.Integer:
                    long l1 = ToInteger(o1);
                    long l2 = ToInteger(o2);

                    return l1.CompareTo(l2);

                case ValueType.Float:
                    double d1 = ToDouble(o1);
                    double d2 = ToDouble(o2);

                    return d1.CompareTo(d2);

                case ValueType.Null:
                    int n1 = (o1 != null ? 0 : 1);
                    int n2 = (o2 != null ? 0 : 1);

                    return n1.CompareTo(n2);

                case ValueType.String:
                    string s1 = ToString(o1);
                    string s2 = ToString(o2);

                    return String.Compare(s1, s2);
            }

            if (o1 is IComparable)
            {
                return ((IComparable)o1).CompareTo(o2);
            }

            if (o2 is IComparable)
            {
                return -(((IComparable)o2).CompareTo(o1));
            }

            return 0;
        }

        public static object Convert(object value, ValueType toType)
        {
            return Convert(value, toType, null);
        }

        public static object Convert(object value, ValueType toType, object defValue)
        {
            switch (toType)
            {
                case ValueType.Null:
                    return null;

                case ValueType.Boolean:
                    bool def1 = StmCommon.ToBoolean(defValue, false);
                    return StmCommon.ToBoolean(value, def1);

                case ValueType.DateTime:
                    DateTime def2 = StmCommon.ToDate(defValue, DateTime.MinValue);
                    return StmCommon.ToDate(value, def2);

                case ValueType.Float:
                    double def3 = StmCommon.ToDouble(defValue, 0);
                    return StmCommon.ToDouble(value, def3);

                case ValueType.String:
                    string def4 = StmCommon.ToString(defValue, null);
                    return StmCommon.ToString(value, def4);

                case ValueType.Integer:
                    long def5 = StmCommon.ToInteger(defValue, 0);
                    return StmCommon.ToInteger(value, def5);

                case ValueType.TimeSpan:
                    TimeSpan def6 = StmCommon.ToTime(defValue, TimeSpan.Zero);
                    return StmCommon.ToTime(value, def6);

                case ValueType.Char:
                    char def7 = StmCommon.ToChar(defValue, '\0');
                    return StmCommon.ToChar(value, def7);
            }

            return value;
        }

        public static ValueType GetType(object obj)
        {
            if (obj == null)
            {
                return ValueType.Null;
            }

            if (obj is TimeSpan)
            {
                return ValueType.TimeSpan;
            }

            TypeCode typeCode = Type.GetTypeCode(obj.GetType());
            
            switch (typeCode)
            {
                case TypeCode.Char:
                    return ValueType.Char;

                case TypeCode.String:
                    return ValueType.String;
             
                case TypeCode.Boolean:
                    return ValueType.Boolean;

                case TypeCode.DateTime:
                    return ValueType.DateTime;

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return ValueType.Float;

                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return ValueType.Integer;
            }

            return ValueType.Object;
        }

        public static string GetOperatorString(ArithmeticOperation op)
        {
            switch (op)
            {
                case ArithmeticOperation.Add:
                    return "+";

                case ArithmeticOperation.Subtract:
                    return "-";

                case ArithmeticOperation.Multiply:
                    return "*";

                case ArithmeticOperation.Divide:
                    return "/";

                case ArithmeticOperation.Modulo:
                    return "%";
            }

            return String.Empty;
        }

        public static string GetOperatorString(LogicalOperation op)
        {
            switch (op)
            {
                case LogicalOperation.And:
                    return StmConstants.OP_AND;

                case LogicalOperation.Or:
                    return StmConstants.OP_OR;
            }

            return String.Empty;
        }

        public static bool InSet(object o1, object o2)
        {
            int i1 = (o1 != null ? 1 : 0);
            int i2 = (o2 != null ? 1 : 0);

            // if one of o1 or o2 is null
            if ((i1 == 0) || (i2 == 0))
            {
                // if o1 is null
                if (i1 == 0)
                {
                    // if o2 is null
                    if (i2 == 0)
                        return true;

                    // if o2 is array
                    if (o2.GetType().IsArray)
                    {
                        Array a2 = (Array)o2;

                        foreach (object o3 in a2)
                        {
                            // if any item in array is null
                            if (object.Equals(o3, o1))
                                return true;
                        }
                    }
                }

                return false;
            }

            // if o2 is array
            if (o2.GetType().IsArray)
            {
                Array a2 = (Array)o2;

                // if o1 is not an array
                if (!o1.GetType().IsArray)
                {
                    foreach (object o3 in a2)
                    {
                        if (Compare(o1, o3) == 0)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                // if both o1 and o2 is array
                Array a1 = (Array)o1;

                foreach (object o3 in a1)
                {
                    bool found = false;
                    foreach (object o4 in a2)
                    {
                        if (Compare(o3, o4) == 0)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        return false;
                }

                return true;
            }

            // if o1 is not array
            if (!o1.GetType().IsArray)
            {
                return (Compare(o1, o2) == 0);
            }

            return false;
        }

        public static bool IsNullStm(Statement stm)
        {
            if (ReferenceEquals(stm, null) || object.Equals(stm, Statement.Null))
            {
                return true;
            }

            ValueStm objB = (stm as ValueStm);
            return (!ReferenceEquals(objB, null) && ((objB.Value is NullStm) ||
                ReferenceEquals(objB.Value, null)));
        }

        public static bool Like(object o1, object o2)
        {
            string s1 = StmCommon.ToString(o1, String.Empty);

            string pattern = StmCommon.ToString(o2, String.Empty);
            pattern = pattern.Replace("%", "*");

            Wildcard wildcard = new Wildcard(pattern);
            return wildcard.IsMatch(s1);
        }

        public static string PrepareToString(object value)
        {
            return PrepareToString(value, true);
        }

        public static string PrepareToString(object value, bool useQuotation)
        {
            string result = (ReferenceEquals(value, null) ? StmConstants.NULL : value.ToString());
            result = (result != null ? result.ToString() : null);

            if (useQuotation && (result != null) && (value is StringStm))
            {
                result = "\"" + result.Replace("\"", "\\\"") + "\"";
            }

            return (result != null ? result : String.Empty);
        }

        public static bool ToBoolean(object value)
        {
            return ToBoolean(value, false);
        }

        public static bool ToBoolean(object value, bool defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            if (value is string)
            {
                string val = (string)value;
                if (val == String.Empty)
                {
                    return defValue;
                }
            }

            string s = ToString(value);
            ValueType type = GetType(value);

            switch (type)
            {
                case ValueType.Float:
                case ValueType.Integer:
                    {
                        double d;
                        if (double.TryParse(s, out d))
                        {
                            return ((long)d == 1);
                        }

                        return false;
                    }
            }

            bool result = defValue;

            if (!bool.TryParse(s, out result))
            {
                if (CommonHelper.EqualStrings(s, "yes", true))
                {
                    return true;
                }

                if (CommonHelper.EqualStrings(s, "no", true))
                {
                    return false;
                }

                if (CommonHelper.EqualStrings(s, "y", true))
                {
                    return true;
                }

                if (CommonHelper.EqualStrings(s, "n", true))
                {
                    return false;
                }

                if (CommonHelper.EqualStrings(s, "t", true))
                {
                    return true;
                }

                if (CommonHelper.EqualStrings(s, "f", true))
                {
                    return false;
                }

                double d;
                result = double.TryParse(s, out d) && ((long)d != 0);
            }

            return result;
        }

        public static DateTime ToDate(object value)
        {
            return ToDate(value, DateTime.MinValue);
        }

        public static DateTime ToDate(object value, DateTime defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is DateTime)
            {
                return (DateTime)value;
            }

            if (value is TimeSpan)
            {
                TimeSpan ts = (TimeSpan)value;
                return DateTime.MinValue.Add(ts);
            }

            DateTime result = defValue;
            string s = ToString(value);

            if (!DateTime.TryParse(s, out result))
            {
                s = s.Replace(" ", "T");
                if (!DateTime.TryParse(s, out result))
                {
                    result = defValue;
                }
            }

            return result;
        }

        public static double ToDouble(object value)
        {
            return ToDouble(value, double.NaN);
        }

        public static double ToDouble(object value, double defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is int)
            {
                return (int)value;
            }

            if (value is double)
            {
                return (double)value;
            }

            if (value is long)
            {
                return (long)value;
            }

            if (value is short)
            {
                return (short)value;
            }

            if (value is bool)
            {
                return ((bool)value ? 1 : 0);
            }

            string s = ToString(value);

            double result = defValue;
            if (!double.TryParse(s, out result))
            {
                result = defValue;
            }

            return result;
        }

        public static Guid ToGuid(object value)
        {
            return ToGuid(value, Guid.Empty);
        }

        public static Guid ToGuid(object value, Guid defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is Guid)
            {
                return (Guid)value;
            }

            string s = ToString(value);

            Guid result = defValue;
            if (!Guid.TryParse(s, out result))
            {
                result = defValue;
            }

            return result;
        }

        public static long ToInteger(object value)
        {
            return ToInteger(value, 0);
        }

        public static long ToInteger(object value, long defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is int)
            {
                return (int)value;
            }

            if (value is long)
            {
                return (long)value;
            }

            if (value is short)
            {
                return (short)value;
            }

            if (value is bool)
            {
                return ((bool)value ? 1 : 0);
            }

            if (value is float)
            {
                double d = (double)value;
                value = Math.Truncate(d);
            }

            string s = ToString(value);

            long result = defValue;
            if (!long.TryParse(s, out result))
            {
                result = defValue;
            }

            return result;
        }

        public static char ToChar(object value)
        {
            return ToChar(value, '\0');
        }

        public static char ToChar(object value, char defValue)
        {
            string str = ToString(value, defValue.ToString());
            if (String.IsNullOrEmpty(str))
            {
                return defValue;
            }

            return str[0];
        }

        public static string ToString(object value)
        {
            return ToString(value, null);
        }

        public static string ToString(object value, string defValue)
        {
            if (value is string)
            {                
                return (string)value;
            }

            if (value is double)
            {
                if (double.IsNaN((double)value))
                {
                    return String.Empty;
                }

                return ((double)value).ToString("R", CultureInfo.InvariantCulture);
            }

            if (value is decimal)
            {
                return ((decimal)value).ToString("R", CultureInfo.InvariantCulture);
            }

            if (value is float)
            {
                return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToString("s").Replace('T', ' ');
            }

            string result = (value != null ? value.ToString() : defValue);
            if (result == null)
            {
                result = defValue;
            }

            return result;
        }

        public static TimeSpan ToTime(object value)
        {
            return ToTime(value, TimeSpan.Zero);
        }

        public static TimeSpan ToTime(object value, TimeSpan defValue)
        {
            if (value == null)
            {
                return defValue;
            }

            if (value is TimeSpan)
            {
                return (TimeSpan)value;
            }

            if (value is DateTime)
            {
                return ((DateTime)value).TimeOfDay;
            }

            if (value is double)
            {
                return TimeSpan.FromMilliseconds((double)value);
            }

            TimeSpan result = defValue;
            string s = ToString(value);

            if (!TimeSpan.TryParse(s, out result))
            {
                double d = 0;
                if (double.TryParse(s, out d))
                {
                    return TimeSpan.FromMilliseconds(d);
                }

                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParse(s, out dt))
                {
                    return dt.TimeOfDay;
                }

                result = defValue;
            }

            return result;
        }
    }
}
