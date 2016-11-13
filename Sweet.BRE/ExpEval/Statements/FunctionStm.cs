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
    public sealed class FunctionStm : ActionStm
    {
        private StringStm _name;
        private List<Statement> _parameters;

        public FunctionStm(StringStm name, params Statement[] parameters) 
            : base()
        {
            _parameters = new List<Statement>();
            _name = (ReferenceEquals(name, null) ? (StringStm)String.Empty : name);

            Params(parameters);
        }

        public Statement[] Parameters
        {
            get
            {
                return _parameters.ToArray();
            }
        }

        public StringStm Name
        {
            get
            {
                return _name;
            }
        }

        public static FunctionStm As(string name, params Statement[] parameters)
        {
            return new FunctionStm(name, parameters);
        }

        public static FunctionStm As(StringStm name, params Statement[] parameters)
        {
            return new FunctionStm(name, parameters);
        }

        public FunctionStm Params(params Statement[] parameters)
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
            Statement[] items = new Statement[_parameters.Count];
            for (int i = 0; i < _parameters.Count; i++)
            {
                Statement item = _parameters[i];
                if (!ReferenceEquals(item, null))
                {
                    items[i] = (Statement)item.Clone();
                }
            }

            return FunctionStm.As((StringStm)_name.Clone(), items);
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_name, null))
            {
                _name.Dispose();
                _name = null;
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
            FunctionStm objA = obj as FunctionStm;
            return ReferenceEquals(obj, this) || (!ReferenceEquals(objA, null) &&
                CommonHelper.EqualStrings(_name.Value, objA.Name.Value, true) &&
                EqualParameters(objA.Parameters));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            string name = _name.ToString();
            name = (name != null ? name.Trim() : String.Empty);

            int pos = name.IndexOf("(");
            if (pos > -1)
            {
                name = name.Substring(0, pos - 1);
                name = (name != null ? name.Trim() : String.Empty);
            }

            builder.Append(name);

            if ((_parameters == null) || (_parameters.Count == 0))
            {
                builder.Append("()");
            }
            else
            {
                builder.Append('(');

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

                builder.Append(')');
            }

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_name).Evaluate(context);

            string function = (obj != null ? obj.ToString() : null);
            function = (function != null ? function.Trim() : String.Empty);

            int pos = function.IndexOf("(");
            if (pos > -1)
            {
                function = function.Substring(0, pos - 1);
                function = (function != null ? function.Trim() : String.Empty);
            }

            if (!String.IsNullOrEmpty(function))
            {
                object[] arguments = null;

                if (_parameters.Count > 0)
                {
                    arguments = new object[_parameters.Count];
                    for (int i = 0; i < _parameters.Count; i++)
                    {
                        IStatement arg = _parameters[i];
                        arguments[i] = !ReferenceEquals(arg, null) ? arg.Evaluate(context) : null;
                    }
                }

                return context.EvaluateFunction(function, arguments);
            }

            return null;
        }

        # region Operators

        public static implicit operator FunctionStm(string value)
        {
            return new FunctionStm(value);
        }

        # endregion
    }
}
