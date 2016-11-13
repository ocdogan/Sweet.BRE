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
    public sealed class EvaluateTreeStm : ActionStm
    {
        private Statement _tree;

        public EvaluateTreeStm(Statement tree)
            : base()
        {
            _tree = (ReferenceEquals(tree, null) ? Statement.Null : tree);
        }

        public Statement Tree
        {
            get
            {
                return _tree;
            }
        }

        public static EvaluateTreeStm As(string tree)
        {
            return new EvaluateTreeStm(tree);
        }

        public static EvaluateTreeStm As(Statement tree)
        {
            return new EvaluateTreeStm(tree);
        }

        public override object Clone()
        {
            return EvaluateTreeStm.As((Statement)_tree.Clone());
        }

        public override bool Equals(object obj)
        {
            EvaluateTreeStm objA = obj as EvaluateTreeStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_tree, objA.Tree));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}); ", RuleConstants.EVALTREE, 
                StmCommon.PrepareToString(_tree));

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
            object obj = ((IStatement)_tree).Evaluate(context);

            string tree = (obj != null ? obj.ToString() : null);
            tree = (tree != null ? tree.Trim() : String.Empty);

            if (String.IsNullOrEmpty(tree))
            {
                if (context.LoggingEnabled)
                {
                   context.Log("Cannot evaluate decision tree. Name parameter is blank.", EvalLogType.Error);
                }
                return null;
            }

            if (String.IsNullOrEmpty(tree))
            {
                throw new RuleException(BreResStrings.GetString("CannotCallABlankTreeName"));
            }

            Project project = FindProject(context);
            if (ReferenceEquals(project, null))
            {
                throw new RuleException(BreResStrings.GetString("CannotFindAProjectInExecutionStack"));
            }

            IStatement treeE = project.GetTree(tree);
            if (ReferenceEquals(treeE, null))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("CannotFindANamedTree"), tree));
            }

            treeE.Evaluate(context);

            return null;
        }

        # region Operators

        public static implicit operator EvaluateTreeStm(string value)
        {
            return new EvaluateTreeStm(value);
        }

        # endregion
    }
}
