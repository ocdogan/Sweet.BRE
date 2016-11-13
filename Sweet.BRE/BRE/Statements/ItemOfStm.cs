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
using System.Reflection;
using System.Text;

namespace Sweet.BRE
{
    public sealed class ItemOfStm : ActionStm
    {
        private const string StrGetItem = "get_Item";
        private const string StrSetItem = "set_Item";

        private Statement _instance;
        private Statement _index;
        private Statement _valueToSet;

        public ItemOfStm(Statement instance)
            : this(instance, null, null)
        {
        }

        public ItemOfStm(Statement instance, Statement index)
            : this(instance, index, null)
        {
        }

        public ItemOfStm(Statement instance, Statement index, Statement valueToSet)
            : base()
        {
            _instance = (ReferenceEquals(instance, null) ? (Statement)String.Empty : instance);
            _index = (ReferenceEquals(index, null) ? (NumericStm)0 : index);
            _valueToSet = (ReferenceEquals(index, null) ? Statement.Null : valueToSet);
        }

        public Statement Index
        {
            get
            {
                return _index;
            }
        }

        public Statement Instance
        {
            get
            {
                return _instance;
            }
        }

        public Statement SetValue
        {
            get
            {
                return _valueToSet;
            }
        }

        public static ItemOfStm As(string instance, int index)
        {
            return new ItemOfStm(instance, index);
        }

        public static ItemOfStm As(Statement instance, Statement index)
        {
            return new ItemOfStm(instance, index);
        }

        public static ItemOfStm As(Statement instance, Statement index, Statement valueToSet)
        {
            return new ItemOfStm(instance, index, valueToSet);
        }

        public override object Clone()
        {
            return new ItemOfStm((Statement)_instance.Clone(), 
                (Statement)_index.Clone(),
                (Statement)_valueToSet.Clone());
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_instance, null))
            {
                _instance.Dispose();
                _instance = null;
            }

            if (!ReferenceEquals(_index, null))
            {
                _index.Dispose();
                _index = null;
            }

            if (!ReferenceEquals(_valueToSet, null))
            {
                _valueToSet.Dispose();
                _valueToSet = null;
            }

            base.Dispose();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ItemOfStm objA = obj as ItemOfStm;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                object.Equals(_instance, objA.Instance) &&
                object.Equals(_index, objA.Index) &&
                object.Equals(_valueToSet, objA.SetValue));
        }

        private bool HasValue()
        {
            return (!ReferenceEquals(_valueToSet, null) && !object.Equals(_valueToSet, Statement.Null));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}[{1}]", StmCommon.PrepareToString(_instance, false), 
                StmCommon.PrepareToString(_index));

            if (HasValue())
            {
                builder.AppendFormat(" = {0}", StmCommon.PrepareToString(_valueToSet));
            }

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object result = null;
            object instance = ((IStatement)_instance).Evaluate(context);

            if (instance != null)
            {
                object key = ((IStatement)_index).Evaluate(context);

                object value = null;
                if (HasValue())
                {
                    value = ((IStatement)_valueToSet).Evaluate(context);
                }

                Array array = instance as Array;
                if (array != null)
                {
                    int index = (int)StmCommon.ToInteger(key, -1);
                    if ((index > -1) && (index < array.Length))
                    {
                        if (HasValue())
                        {
                            array.SetValue(value, index);
                        }

                        result = array.GetValue(index);
                    }
                }
                else
                {
                    if (RuleCommon.IsNumber(key))
                    {
                        key = (int)StmCommon.ToInteger(key, -1);
                    }
                    else
                    {
                        key = StmCommon.ToString(key);
                    }

                    string member = StrGetItem;
                    object[] memberArgs = new object[] { key };

                    if (HasValue())
                    {
                        member = StrSetItem;
                        memberArgs = new object[] { key, value };
                    }

                    MethodInfo method = ReflectionCommon.GetMethodInfo(instance, member, memberArgs);
                    if (method != null)
                    {
                        BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public |
                           BindingFlags.NonPublic | BindingFlags.Instance |
                           BindingFlags.InvokeMethod;

                        result = method.Invoke(instance, flags, null, memberArgs, null);
                    }
                }
            }

            return result;
        }

        # region Operators

        public static implicit operator ItemOfStm(byte value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(char value)
        {
            return new ItemOfStm(null, value.ToString());
        }

        public static implicit operator ItemOfStm(decimal value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(double value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(short value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(int value)
        {
            return new ItemOfStm(null, value);
        }

        public static implicit operator ItemOfStm(long value)
        {
            return new ItemOfStm(null, value);
        }

        public static implicit operator ItemOfStm(sbyte value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(float value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(string value)
        {
            return new ItemOfStm(value, 0);
        }

        public static implicit operator ItemOfStm(ushort value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(uint value)
        {
            return new ItemOfStm(null, (int)value);
        }

        public static implicit operator ItemOfStm(ulong value)
        {
            return new ItemOfStm(null, (int)value);
        }

        # endregion
    }
}
