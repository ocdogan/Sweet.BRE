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
    public static class RuleCommon
    {
        public static object Convert(object value, DecisionValueType toType)
        {
            return Convert(value, toType, null);
        }

        public static object Convert(object value, DecisionValueType toType, object defValue)
        {
            switch (toType)
            {
                case DecisionValueType.Boolean:
                    bool def1 = StmCommon.ToBoolean(defValue, false);
                    return StmCommon.ToBoolean(value, def1);

                case DecisionValueType.DateTime:
                    DateTime def2 = StmCommon.ToDate(defValue, DateTime.MinValue);
                    return StmCommon.ToDate(value, def2);

                case DecisionValueType.Float:
                    double def3 = StmCommon.ToDouble(defValue, 0);
                    return StmCommon.ToDouble(value, def3);

                case DecisionValueType.String:
                    string def4 = StmCommon.ToString(defValue, null);
                    return StmCommon.ToString(value, def4);

                case DecisionValueType.Integer:
                    long def5 = StmCommon.ToInteger(defValue, 0);
                    return StmCommon.ToInteger(value, def5);

                case DecisionValueType.TimeSpan:
                    TimeSpan def6 = StmCommon.ToTime(defValue, TimeSpan.Zero);
                    return StmCommon.ToTime(value, def6);
            }

            return value;
        }

        public static object[] Prepare(string[] values, DecisionValueType type, DecisionOperation operation)
        {
            int length = (values != null ? values.Length : 0);

            object[] result = new object[0];
            object defValue = RuleCommon.Convert(null, type);

            switch (operation)
            {
                case DecisionOperation.Between:
                    result = new object[] { defValue, defValue };
                    if (length > 0)
                    {
                        result[0] = RuleCommon.Convert(values[0], type);
                        if (length > 1)
                        {
                            result[1] = RuleCommon.Convert(values[1], type);
                        }
                    }
                    break;

                case DecisionOperation.In:
                case DecisionOperation.NotIn:
                    result = new object[length];
                    if (length > 0)
                    {
                        int i = 0;
                        foreach (string value in result)
                        {
                            result[i] = RuleCommon.Convert(values[i], type);
                            i++;
                        }
                    }
                    break;

                default:
                    result = new object[1] { null };
                    if (length > 0)
                    {
                        result[0] = RuleCommon.Convert(values[0], type);
                    }
                    break;
            }

            return result;
        }

        public static bool EvaluateCondition(string leftValue, DecisionOperation operation, 
            string[] rightValues, DecisionValueType valueType)
        {
            object leftObj = RuleCommon.Convert(leftValue, valueType);
            object[] values = Prepare(rightValues, valueType, operation);

            switch (operation)
            {
                case DecisionOperation.Between:
                    return StmCommon.Between(leftObj, values[0], values[1]);

                case DecisionOperation.Equals:
                    return StmCommon.Equals(leftObj, values[0]);

                case DecisionOperation.In:
                    return StmCommon.InSet(leftObj, values);

                case DecisionOperation.GreaterThan:
                    return (StmCommon.Compare(leftObj, values[0]) > 0);

                case DecisionOperation.GreaterThanOrEquals:
                    return (StmCommon.Compare(leftObj, values[0]) >= 0);

                case DecisionOperation.LessThan:
                    return (StmCommon.Compare(leftObj, values[0]) < 0);

                case DecisionOperation.LessThanOrEquals:
                    return (StmCommon.Compare(leftObj, values[0]) <= 0);

                case DecisionOperation.Like:
                    return StmCommon.Like(leftObj, values[0]);

                case DecisionOperation.NotEquals:
                    return !StmCommon.Equals(leftObj, values[0]);

                case DecisionOperation.NotIn:
                    return !StmCommon.InSet(leftObj, values);

                case DecisionOperation.NotLike:
                    return !StmCommon.Like(leftObj, values[0]);
            }

            return false;
        }

        public static object GetDefaultValue(DecisionValueType type)
        {
            switch (type)
            {
                case DecisionValueType.Boolean:
                    return false;

                case DecisionValueType.DateTime:
                    return DateTime.MinValue;

                case DecisionValueType.Float:
                    return (double)0;

                case DecisionValueType.Integer:
                    return (long)0;

                case DecisionValueType.String:
                    return String.Empty;

                case DecisionValueType.TimeSpan:
                    return TimeSpan.Zero;
            }

            return null;
        }

        public static string GetOperatorString(DecisionOperation op)
        {
            switch (op)
            {
                case DecisionOperation.Between:
                    return StmConstants.OP_BETWEEN;

                case DecisionOperation.Equals:
                    return StmConstants.OP_EQUALS;

                case DecisionOperation.GreaterThan:
                    return StmConstants.OP_GREATER_THAN;

                case DecisionOperation.GreaterThanOrEquals:
                    return StmConstants.OP_GREATER_THAN_OR_EQUALS;

                case DecisionOperation.In:
                    return StmConstants.OP_IN;

                case DecisionOperation.LessThan:
                    return StmConstants.OP_LESS_THAN;

                case DecisionOperation.LessThanOrEquals:
                    return StmConstants.OP_LESS_THAN_OR_EQUALS;

                case DecisionOperation.Like:
                    return StmConstants.OP_LIKE;

                case DecisionOperation.NotEquals:
                    return StmConstants.OP_NOT_EQUALS;

                case DecisionOperation.NotIn:
                    return StmConstants.OP_NOT_IN;

                case DecisionOperation.NotLike:
                    return StmConstants.OP_NOT_LIKE;
            }

            return String.Empty;
        }

        public static bool IsNumber(object obj)
        {
            return (obj != null) && ((obj is byte) || (obj is sbyte) || (obj is short) ||
                (obj is int) || (obj is long) || (obj is Single) || (obj is float) ||
                (obj is decimal) || (obj is double) || (obj is ushort) || (obj is uint) ||
                (obj is ulong));
        }

        public static bool IsInteger(object obj)
        {
            return (obj != null) && ((obj is byte) || (obj is sbyte) || (obj is short) ||
                (obj is int) || (obj is long) || (obj is ushort) || (obj is uint) ||
                (obj is ulong));
        }

        public static bool IsFloat(object obj)
        {
            return (obj != null) && ((obj is Single) || (obj is float) || 
                (obj is decimal) || (obj is double));
        }
    }
}
