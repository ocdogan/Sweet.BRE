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
    public sealed class ReflectionStm : ActionStm
    {
        private Statement _instance;
        private Statement _method;

        private List<Statement> _parameters;

        public ReflectionStm(Statement instance, Statement methodName, params Statement[] parameters) 
            : base()
        {
            _parameters = new List<Statement>();

            _instance = ReferenceEquals(instance, null) ? Statement.Null : instance;
            _method = ReferenceEquals(methodName, null) ? Statement.Null : methodName;

            Params(parameters);
        }

        public Statement Instance
        {
            get
            {
                return _instance;
            }
        }

        public Statement Method
        {
            get
            {
                return _method;
            }
        }

        public Statement[] Parameters
        {
            get
            {
                return _parameters.ToArray();
            }
        }

        public static ReflectionStm As(string instance, params Statement[] parameters)
        {
            return new ReflectionStm(instance, null, parameters);
        }

        public static ReflectionStm As(string instance, string methodName, params Statement[] parameters)
        {
            return new ReflectionStm(instance, methodName, parameters);
        }

        public static ReflectionStm As(Statement instance, params Statement[] parameters)
        {
            return new ReflectionStm(instance, null, parameters);
        }

        public static ReflectionStm As(Statement instance, Statement methodName, params Statement[] parameters)
        {
            return new ReflectionStm(instance, methodName, parameters);
        }

        public ReflectionStm Params(params Statement[] parameters)
        {
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Statement param = parameters[i];
                    if (ReferenceEquals(param, null))
                    {
                        param = Statement.Null;
                    }

                    _parameters.Add(param);
                }
            }

            return this;
        }

        public override object Clone()
        {
            Statement[] args = new Statement[_parameters.Count];
            for (int i = 0; i < _parameters.Count; i++)
            {
                Statement item = _parameters[i];
                if (!ReferenceEquals(item, null))
                {
                    args[i] = (Statement)item.Clone();
                }
            }

            return ReflectionStm.As((Statement)_instance.Clone(), 
                (Statement)_method.Clone(), args);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_instance, null))
            {
                _instance.Dispose();
                _instance = null;
            }

            if (!ReferenceEquals(_method, null))
            {
                _method.Dispose();
                _method = null;
            }

            if (_parameters != null)
            {
                for (int i = _parameters.Count - 1; i > -1; i--)
                {
                    Statement param = _parameters[i];
                    _parameters.RemoveAt(i);

                    param.Dispose();
                }
            }

            base.Dispose();
        }

        private bool EqualParameters(Statement[] parameters)
        {
            if (parameters.Length == _parameters.Count)
            {
                int index = 0;
                foreach (Statement param1 in parameters)
                {
                    Statement param2 = _parameters[index++];
                    if (!param1.Equals(param2))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            ReflectionStm objA = obj as ReflectionStm;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                object.Equals(_instance, objA.Instance) &&
                object.Equals(_method, objA.Method) &&
                EqualParameters(objA.Parameters));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}, {2}", RuleConstants.REFLECT, 
                StmCommon.PrepareToString(_instance), 
                StmCommon.PrepareToString(_method));

            if ((_parameters != null) && (_parameters.Count > 0))
            {
                builder.Append(", ");

                for (int i = 0; i < _parameters.Count; i++)
                {
                    Statement param = _parameters[i];
                    if (ReferenceEquals(param, null))
                    {
                        param = Statement.Null;
                    }

                    builder.Append(StmCommon.PrepareToString(param));
                    if (i < _parameters.Count - 1)
                    {
                        builder.Append(", ");
                    }
                }
            }

            builder.Append(")");

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object instance = null;
            if (ReferenceEquals(_instance, null) || ReferenceEquals(_instance, Statement.Null))
            {
                instance = context;
            }
            else
            {
                instance = ((IStatement)_instance).Evaluate(context);
            }

            if (instance == null)
            {
                if (context.LoggingEnabled)
                {
                    context.Log("Cannot evaluate ReflectionStm. Instance parameter evaluated to null.",
                       EvalLogType.Warning);
                }
                return null;
            }

            object obj = ((IStatement)_method).Evaluate(context);

            string method = ((obj != null) ? obj.ToString() : String.Empty);
            method = ((method != null) ? method.Trim() : String.Empty);

            if (String.IsNullOrEmpty(method))
            {
                if (context.LoggingEnabled)
                {
                    context.Log("Cannot evaluate ReflectionStm. Method parameter evaluated as blank.",
                       EvalLogType.Warning);
                }
                return instance;
            }

            object[] prms = null;
            if (_parameters != null)
            {
                prms = new object[_parameters.Count];

                int i = 0;
                foreach (Statement stm in _parameters)
                {
                    prms[i++] = ((IStatement)stm).Evaluate(context);
                }
            }

            return ReflectionEvaluator.Evaluate(instance, method, prms);
        }

        # region Operators

        public static BooleanStm operator ==(ReflectionStm left, ReflectionStm right)
        {
            return Statement.EqualTo(left, right);
        }

        public static BooleanStm operator >(ReflectionStm left, ReflectionStm right)
        {
            return Statement.GreaterThan(left, right);
        }

        public static BooleanStm operator >=(ReflectionStm left, ReflectionStm right)
        {
            return Statement.GreaterThanOrEquals(left, right);
        }

        public static BooleanStm operator !=(ReflectionStm left, ReflectionStm right)
        {
            return Statement.NotEqualTo(left, right);
        }

        public static BooleanStm operator <(ReflectionStm left, ReflectionStm right)
        {
            return Statement.LessThan(left, right);
        }

        public static BooleanStm operator <=(ReflectionStm left, ReflectionStm right)
        {
            return Statement.LessThanOrEquals(left, right);
        }

        # endregion
    }
}
