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
    public sealed class Rule : ActionContext, IRule
    {
        private string _name;
        private string _description;
        
        private RulePriority _priority = RulePriority.Medium;
        private int _subPriority = 50;

        private Ruleset _ruleset;
        private BooleanStm _condition;

        private INamedObjectList _ownerList;

        public Rule()
            : base()
        {
        }

        internal Rule(string name)
            : this()
        {
            SetName(name);
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetDescription(value);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public RulePriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                SetPriority(value);
            }
        }

        public BooleanStm Condition
        {
            get
            {
                return _condition;
            }
        }

        public int Index
        {
            get
            {
                if (!ReferenceEquals(_ownerList, null))
                {
                    return _ownerList.IndexOf(this);
                }
                return -1;
            }
        }

        public IRuleset Ruleset
        {
            get
            {
                return _ruleset;
            }
        }

        public int SubPriority
        {
            get
            {
                return _subPriority;
            }
            set
            {
                SetSubPriority(value);
            }
        }

        INamedObjectList INamedObject.List
        {
            get { return _ownerList; }
        }

        internal void SetOwnerList(INamedObjectList list)
        {
            _ownerList = list;
        }

        public IRule SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public IRule SetPriority(RulePriority priority)
        {
            _priority = priority;
            return this;
        }

        public IRule SetSubPriority(int subPriority)
        {
            if ((subPriority < 0) || (subPriority > 1000))
            {
                throw new RuleException(BreResStrings.GetString("SubPriorityMustBeBetween"));
            }

            _subPriority = subPriority;
            return this;
        }

        internal void SetRuleset(Ruleset project)
        {
            _ruleset = project;
        }

        internal void SetName(string name)
        {
            _name = (name != null ? name.Trim() : null);
        }

        public IRule When(BooleanStm condition)
        {
            _condition = condition;
            return this;
        }

        public IRule Do(params ActionStm[] doActions)
        {
            base.DoAction(doActions);
            return this;
        }

        public static Rule As()
        {
            return new Rule();
        }

        public static Rule As(RulePriority priority, int subPriority)
        {
            Rule result = new Rule();
            
            result.SetPriority(priority);
            result.SetSubPriority(subPriority);

            return result;
        }

        public override object Clone()
        {
            Rule result = Rule.As(_priority, _subPriority);
            result.SetDescription(_description);

            foreach (ActionStm a in base.Actions)
            {
                if (!ReferenceEquals(a, null))
                {
                    result.DoAction((ActionStm)a.Clone());
                }
            }

            return result;
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_ruleset, null))
            {
                _ruleset.RemoveRule(_name);
                _ruleset = null;
            }

            if (!ReferenceEquals(_condition, null))
            {
                _condition.Dispose();
                _condition = null;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            Rule objA = obj as Rule;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                base.Equals(obj));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}(\"{1}\", {2}:{3}) ", RuleConstants.RULE,
                (_name != null ? _name : String.Empty),
                _priority.ToString(), 
                _subPriority.ToString());

            bool hasCondition = !ReferenceEquals(_condition, null);

            if (hasCondition)
            {
                builder.AppendLine();
                builder.AppendFormat("If {0} Then", _condition);
            }

            builder.AppendLine();
            builder.Append(base.ToString());

            if (hasCondition)
            {
                builder.AppendLine(RuleConstants.END + " ");
            }

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.Canceled || context.Halted || context.InReturn);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (!ReferenceEquals(_condition, null))
            {
                object val = ((IStatement)_condition).Evaluate(context, args);
                if (!StmCommon.ToBoolean(val))
                {
                    return double.NaN;
                }
            }

            return base.Evaluate(context, args);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(obj, null)) return -1;

            IRule irule = obj as IRule;
            if (ReferenceEquals(irule, null)) return -1;

            int px = ((10000 * (int)Priority)) + SubPriority;
            int py = ((10000 * (int)irule.Priority)) + irule.SubPriority;

            int result = px.CompareTo(py);
            if (result == 0)
            {
                int ix = Index;
                int iy = irule.Index;

                result = ix.CompareTo(iy);
            }
            return result;
        }
    }
}
