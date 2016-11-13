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
    public sealed class EvaluateTableStm : ActionStm
    {
        private Statement _table;

        public EvaluateTableStm(Statement table)
            : base()
        {
            _table = (ReferenceEquals(table, null) ? Statement.Null : table);
        }

        public Statement Table
        {
            get
            {
                return _table;
            }
        }

        public static EvaluateTableStm As(string table)
        {
            return new EvaluateTableStm(table);
        }

        public static EvaluateTableStm As(Statement table)
        {
            return new EvaluateTableStm(table);
        }

        public override object Clone()
        {
            return EvaluateTableStm.As((Statement)_table.Clone());
        }

        public override bool Equals(object obj)
        {
            EvaluateTableStm objA = obj as EvaluateTableStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_table, objA.Table));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}); ", RuleConstants.EVALTABLE, 
                StmCommon.PrepareToString(_table));

            return builder.ToString();
        }

        private IProject FindProject(IEvaluationContext context)
        {
            IStatement[] stack = context.CallStack;
            if (!ReferenceEquals(stack, null))
            {
                foreach (IStatement e in stack)
                {
                    if (e is IRuleset)
                    {
                        return ((IRuleset)e).Project;
                    }
                }
            }

            return null;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            object obj = ((IStatement)_table).Evaluate(context);

            string table = ((obj != null) ? obj.ToString() : null);
            table = ((table != null) ? table.Trim() : String.Empty);

            if (String.IsNullOrEmpty(table))
            {
                if (context.LoggingEnabled)
                {
                    context.Log("Cannot evaluate decision table. Name parameter is blank.", EvalLogType.Error);
                }
                return null;
            }

            IProject project = FindProject(context);
            if (ReferenceEquals(project, null))
            {
                throw new RuleException(BreResStrings.GetString("CannotFindAProjectInExecutionStack"));
            }

            IStatement tableE = project.GetTable(table);
            if (ReferenceEquals(tableE, null))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("CannotFindANamedTable"), table));
            }

            tableE.Evaluate(context);

            return null;
        }

        # region Operators

        public static implicit operator EvaluateTableStm(string value)
        {
            return new EvaluateTableStm(value);
        }

        # endregion
    }
}
