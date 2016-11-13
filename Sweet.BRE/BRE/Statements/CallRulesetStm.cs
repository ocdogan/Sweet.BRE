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
    public sealed class CallRulesetStm : ActionStm
    {
        private Statement _ruleset;

        public CallRulesetStm(Statement ruleset)
            : base()
        {
            _ruleset = (ReferenceEquals(ruleset, null) ? Statement.Null : ruleset);
        }

        public Statement Ruleset
        {
            get
            {
                return _ruleset;
            }
        }

        public static CallRulesetStm As(string ruleset)
        {
            return new CallRulesetStm(ruleset);
        }

        public static CallRulesetStm As(Statement ruleset)
        {
            return new CallRulesetStm(ruleset);
        }

        public override object Clone()
        {
            return CallRulesetStm.As((Statement)_ruleset.Clone());
        }

        public override bool Equals(object obj)
        {
            CallRulesetStm objA = obj as CallRulesetStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_ruleset, objA.Ruleset));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}); ", RuleConstants.CALL, 
                StmCommon.PrepareToString(_ruleset));

            return builder.ToString();
        }

        private Project FindProject(IEvaluationContext context)
        {
            IStatement[] stack = context.CallStack;
            if (!ReferenceEquals(stack, null))
            {
                foreach (IStatement e in stack)
                {
                    if (e is Ruleset)
                    {
                        return ((Ruleset)e).Project;
                    }
                }
            }

            return null;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (ReferenceEquals(_ruleset, null) || _ruleset.Equals(Statement.Null))
            {
                throw new RuleException(BreResStrings.GetString("CannotCallABlankRulesetName"));
            }

            object obj = ((IStatement)_ruleset).Evaluate(context);
            
            string ruleset = StmCommon.ToString(obj);
            ruleset = (ruleset != null ? ruleset.Trim() : String.Empty);

            if (String.IsNullOrEmpty(ruleset))
            {
                if (context.LoggingEnabled)
                {
                    context.Log("Cannot call ruleset. Name parameter is blank.", EvalLogType.Error);
                }
                return null;
            }

            Project project = FindProject(context);
            if (ReferenceEquals(project, null))
            {
                throw new RuleException(BreResStrings.GetString("CannotFindAProjectInExecutionStack"));
            }

            IStatement rulesetE = project[ruleset];
            if (ReferenceEquals(rulesetE, null))
            {
                throw new RuleException(BreResStrings.GetString("CannotFindANamedRuleset"), ruleset);
            }

            rulesetE.Evaluate(context);

            return null;
        }

        # region Operators

        public static implicit operator CallRulesetStm(string value)
        {
            return new CallRulesetStm(value);
        }

        # endregion
    }
}
