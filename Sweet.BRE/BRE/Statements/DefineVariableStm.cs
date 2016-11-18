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
    public sealed class DefineVariableStm : ActionStm
    {
        private Statement _value;
        private Statement _name;

        public DefineVariableStm(Statement to)
            : this(to, null)
        {
        }

        public DefineVariableStm(Statement name, Statement value)
            : base()
        {
            if (ReferenceEquals(name, null))
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
            _value = (ReferenceEquals(value, null) ? Statement.Null : value);
        }

        public Statement Name
        {
            get
            {
                return _name;
            }
        }

        public Statement Value
        {
            get
            {
                return _value;
            }
        }

        public DefineVariableStm Init(Statement value)
        {
            _value = value;
            return this;
        }

        public static DefineVariableStm As(string to)
        {
            return As(to, null);
        }

        public static DefineVariableStm As(string to, Statement value)
        {
            return As(to, value);
        }

        public static DefineVariableStm As(Statement to, Statement value)
        {
            return new DefineVariableStm(to, value);
        }

        public override object Clone()
        {
            Statement to = !ReferenceEquals(_name, null) ? (Statement)_name.Clone() : null;
            Statement value = !ReferenceEquals(_value, null) ? (Statement)_value.Clone() : null;

            return new DefineVariableStm(to, value);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_name, null))
            {
                _name.Dispose();
                _name = null;
            }

            if (!ReferenceEquals(_value, null))
            {
                _value.Dispose();
                _value = null;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            DefineVariableStm objA = obj as DefineVariableStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_name, objA.Name) &&
                object.Equals(_value, objA.Value));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string to = "@var";
            if (!ReferenceEquals(_name, null))
            {
                to = StmCommon.PrepareToString(_name, false);
            }

            string value = StmCommon.PrepareToString(_value);
            builder.AppendFormat("{0} = {1}; ", to, value);

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_name).Evaluate(context);

            string name = (obj != null ? obj.ToString() : String.Empty);
            name = (name != null ? name.Trim() : String.Empty);

            if (!String.IsNullOrEmpty(name))
            {
                IEvaluationScope scope = context.GetCurrentScope();
                if (scope != null)
                {
                    object value = ((IStatement)_value).Evaluate(context, args);

                    if (value == null)
                    {
                        scope.Set(name, true, (string)value);
                        return null;
                    }

                    TypeCode typeCode = Type.GetTypeCode(value.GetType());

                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            scope.Set(name, true, (bool)value);
                            break;

                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            scope.Set(name, true, StmCommon.ToInteger(value));
                            break;

                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            scope.Set(name, true, StmCommon.ToDouble(value));
                            break;

                        case TypeCode.DateTime:
                            scope.Set(name, true, (DateTime)value);
                            break;

                        case TypeCode.Char:
                        case TypeCode.String:
                            scope.Set(name, true, StmCommon.ToString(value));
                            break;

                        default:
                            {
                                if (value is TimeSpan)
                                {
                                    scope.Set(name, true, (TimeSpan)value);
                                }
                                else
                                {
                                    scope.Set(name, true, StmCommon.ToString(value));
                                }
                                break;
                            }
                    }
                }
            }

            return null;
        }

        # region Operators

        public static implicit operator DefineVariableStm(string name)
        {
            return new DefineVariableStm(name);
        }

        # endregion
    }
}
