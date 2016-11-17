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
    public class Ruleset : ActionStm, INamedObject, IRuleset
    {
        private string _name;
        private string _description;

        private IProject _project;
        private INamedObjectList _ownerList;

        private NamedObjectList<IRule> _ruleList;

        private readonly object _syncLock = new object();

        public Ruleset()
            : base()
        {
            SetRuleList(new NamedObjectList<IRule>());
        }

        internal Ruleset(string name)
            : this()
        {
            SetName(name);
        }

        public IRule this[string ruleName]
        {
            get
            {
                return GetRule(ruleName);
            }
            set
            {
                _ruleList.Add(ruleName, value);
            }
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

        public IProject Project
        {
            get
            {
                return _project;
            }
        }

        public int RuleCount
        {
            get
            {
                return _ruleList.Count;
            }
        }

        public IRule[] Rules
        {
            get
            {
                return _ruleList.Objects;
            }
        }

        public string[] RuleNames
        {
            get
            {
                return _ruleList.Names;
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

        INamedObjectList INamedObject.List
        {
            get { return _ownerList; }
        }

        internal void SetOwnerList(INamedObjectList list)
        {
            _ownerList = list;
        }

        public IRuleset SetDescription(string description)
        {
            _description = description;
            return this;
        }

        internal void SetProject(IProject project)
        {
            _project = project;
        }

        internal void SetName(string name)
        {
            _name = (name != null ? name.Trim() : null);
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        public static Ruleset As()
        {
            return new Ruleset();
        }

        public IRuleset Clear()
        {
            _ruleList.Clear();
            return this;
        }

        public override object Clone()
        {
            Ruleset result = new Ruleset();
            result.SetDescription(_description);

            NamedObjectList<IRule> list;
            lock (_syncLock)
            {
                list = (NamedObjectList<IRule>)_ruleList.Clone();
            }
            result.SetRuleList(list);
            return result;
        }

        public override void Dispose()
        {
            _ruleList.Dispose();
            base.Dispose();
        }

        public bool IsEqualTo(IRuleset ruleset)
        {
            return _ruleList.Equals(ruleset);
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.Canceled || context.Halted);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (!ReferenceEquals(_ruleList, null) && (_ruleList.Count > 0))
            {
                IRule[] rules = Rules;

                foreach (IStatement e in rules)
                {
                    e.Evaluate(context);
                    if (ExecutionBroken(context))
                        break;
                }
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            Ruleset objA = obj as Ruleset;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                IsEqualTo(objA));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0}({1}) ", RuleConstants.RULESET,
                (_name != null ? _name : String.Empty));

            builder.AppendLine();

            lock (_ruleList.SyncLock)
            {
                int index = 0;
                foreach (Rule rule in _ruleList.Objects)
                {
                    index++;
                    if (index > 1)
                        builder.AppendLine();

                    builder.AppendLine(rule.ToString());
                }
            }

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, this)) return 0;
            if (ReferenceEquals(obj, null)) return -1;

            IRule rule = obj as IRule;
            if (ReferenceEquals(rule, null)) return -1;

            return Name.CompareTo(rule.Name);
        }

        internal void SetRuleList(NamedObjectList<IRule> list)
        {
            INamedObjectList oldList = _ruleList;
            if (!ReferenceEquals(oldList, null))
            {
                oldList.OnAdd -= ruleListOnAdd;
                oldList.OnDispose -= ruleListOnDispose;
                oldList.OnRemove -= ruleListOnRemove;
                oldList.OnValidate -= ruleListOnValidate;
                oldList.OnValidateName -= ruleListOnValidateName;
            }

            _ruleList = list;
            if (!ReferenceEquals(list, null))
            {
                list.OnAdd += ruleListOnAdd;
                list.OnDispose += ruleListOnDispose;
                list.OnRemove += ruleListOnRemove;
                list.OnValidate += ruleListOnValidate;
                list.OnValidateName += ruleListOnValidateName;
            }
        }

        private string ValidateRuleName(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            if (String.IsNullOrEmpty(ruleName))
            {
                throw new ArgumentNullException("ruleName");
            }
            return ruleName;
        }

        private void ruleListOnValidateName(INamedObjectList list, INamedObject obj, string name)
        {
            ValidateRuleName(name);
        }

        private void ruleListOnValidate(INamedObjectList list, INamedObject obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("rule");
            }

            IRule rule = obj as IRule;
            if (ReferenceEquals(rule, null))
            {
                throw new RuleException(BreResStrings.GetString("ExpectingRuleObject"));
            }

            if (ReferenceEquals(rule.Ruleset, this))
            {
                throw new RuleException(BreResStrings.GetString("RuleAlreadyExistsInRuleset"));
            }

            string ruleName = NormalizeName(rule.Name);
            if (ContainsRule(ruleName))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRuleAlreadyExistsInRuleset"), ruleName));
            }
        }

        private void ruleListOnRemove(INamedObjectList list, INamedObject obj)
        {
            Rule rl = obj as Rule;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetRuleset(null);
                rl.SetOwnerList(null);
            }
        }

        private void ruleListOnDispose(INamedObjectList list, INamedObject obj)
        {
            Rule rl = obj as Rule;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetRuleset(null);
                rl.SetOwnerList(null);
                rl.Dispose();
            } 
            else if (obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        private void ruleListOnAdd(INamedObjectList list, INamedObject obj, string name)
        {
            Rule rl = obj as Rule;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetRuleset(this);
                rl.SetName(name);
                rl.SetOwnerList(list);
            }
        }

        public IRuleset AddRule(string ruleName, IRule rule)
        {
            _ruleList.Add(ruleName, rule);
            return this;
        }

        public bool ContainsRule(string ruleName)
        {
            return _ruleList.Contains(ruleName);
        }

        public IRule DefineRule(string ruleName)
        {
            ruleName = ValidateRuleName(ruleName);
            if (ContainsRule(ruleName))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRuleAlreadyExistsInRuleset"), ruleName));
            }

            IRule value = Rule.As();
            _ruleList.Add(ruleName, value);

            return value;
        }

        public IRule GetRule(string ruleName)
        {
            return _ruleList.Get(ruleName);
        }

        public IRuleset RemoveRule(string ruleName)
        {
            _ruleList.Remove(ruleName);
            return this;
        }
    }
}
