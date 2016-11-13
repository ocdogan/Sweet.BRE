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
    public sealed class SetVariableStm : ActionStm
    {
        private Statement _value;
        private Statement _to;

        public SetVariableStm(Statement to) 
            : this(to, null)
        {
        }

        public SetVariableStm(Statement to, Statement value) 
            : base()
        {
            if (ReferenceEquals(to, null))
            {
                throw new ArgumentNullException("to");
            }

            _to = to;
            _value = (ReferenceEquals(value, null) ? Statement.Null : value);
        }

        public Statement To
        {
            get
            {
                return _to;
            }
        }

        public Statement Value
        {
            get
            {
                return _value;
            }
        }

        public SetVariableStm Set(Statement value)
        {
            _value = value;
            return this;
        }

        public static SetVariableStm As(string to)
        {
            return As(to, null);
        }

        public static SetVariableStm As(string to, Statement value)
        {
            return As(to, value);
        }

        public static SetVariableStm As(Statement to, Statement value)
        {
            return new SetVariableStm(to, value);
        }

        public override object Clone()
        {
            Statement to = !ReferenceEquals(_to, null) ? (Statement)_to.Clone() : null;
            Statement value = !ReferenceEquals(_value, null) ? (Statement)_value.Clone() : null;

            return new SetVariableStm(to, value);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_to, null))
            {
                _to.Dispose();
                _to = null;
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
            SetVariableStm objA = obj as SetVariableStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_to, objA.To) && 
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
            if (!ReferenceEquals(_to, null))
            {
                to = StmCommon.PrepareToString(_to, false);
            }

            string value = StmCommon.PrepareToString(_value);
            builder.AppendFormat("{0} = {1}; ", to, value);

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_to).Evaluate(context);

            string to = (obj != null ? obj.ToString() : String.Empty);
            to = (to != null ? to.Trim() : String.Empty);

            if (!String.IsNullOrEmpty(to))
            {
                IEvaluationScope scope = context.GetCurrentScope();
                if (scope != null)
                {
                    object value = ((IStatement)_value).Evaluate(context, args);

                    if (value == null)
                    {
                        scope.Set(to, (string)value);
                        return null;
                    }

                    TypeCode typeCode = Type.GetTypeCode(value.GetType());

                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            scope.Set(to, (bool)value);
                            break;

                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            scope.Set(to, StmCommon.ToInteger(value));
                            break;

                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            scope.Set(to, StmCommon.ToDouble(value));
                            break;

                        case TypeCode.DateTime:
                            scope.Set(to, (DateTime)value);
                            break;

                        case TypeCode.Char:
                        case TypeCode.String:
                            scope.Set(to, StmCommon.ToString(value));
                            break;

                        default:
                            {
                                if (value is TimeSpan)
                                {
                                    scope.Set(to, (TimeSpan)value);
                                }
                                else
                                {
                                    scope.Set(to, StmCommon.ToString(value));
                                }
                                break;
                            }
                    }
                }
            }

            return null;
        }

        # region Operators

        public static implicit operator SetVariableStm(string to)
        {
            return new SetVariableStm(to);
        }

        # endregion
    }
}
