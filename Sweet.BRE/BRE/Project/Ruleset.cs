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
    public class Ruleset : ActionStm, IRuleset
    {
        private string _name;
        private string _description;

        private IProject _project;

        private IRule[] _ruleArray;
        private string[] _ruleNames;
        private List<IRule> _ruleList;
        private Dictionary<string, IRule> _ruleIndex;

        private readonly object _syncLock = new object();

        public Ruleset()
            : base()
        {
            _ruleList = new List<IRule>();
            _ruleIndex = new Dictionary<string, IRule>();
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
                ruleName = NormalizeName(ruleName);

                Validate(ruleName);
                Validate(value);

                if (!ReferenceEquals(value.Ruleset, null))
                {
                    value = (IRule)value.Clone();
                }

                Rule rule;
                lock (_syncLock)
                {
                    _ruleArray = null;
                    _ruleNames = null;

                    IRule old;
                    _ruleIndex.TryGetValue(ruleName, out old);

                    if (!ReferenceEquals(old, value))
                    {
                        _ruleList.Remove(old);

                        rule = old as Rule;
                        if (!ReferenceEquals(rule, null))
                        {
                            rule.SetRuleset(null);
                        }
                    }

                    rule = value as Rule;
                    if (!ReferenceEquals(rule, null))
                    {
                        rule.SetRuleset(this);
                        rule.SetName(ruleName);
                    }

                    _ruleIndex[ruleName] = value;
                    _ruleList.Add(value);
                }
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
                return _ruleIndex.Count;
            }
        }

        public IRule[] Rules
        {
            get
            {
                if (_ruleArray == null)
                {
                    lock (_syncLock)
                    {
                        if (_ruleArray == null)
                        {
                            _ruleList.Sort(delegate(IRule x, IRule y)
                            {
                                if (ReferenceEquals(x, y)) return 0;
                                if (ReferenceEquals(x, null)) return 1;
                                if (ReferenceEquals(y, null)) return -1;

                                int px = ((10000 * (int)x.Priority)) + x.SubPriority;
                                int py = ((10000 * (int)y.Priority)) + y.SubPriority;

                                int result = px.CompareTo(py);
                                if (result == 0)
                                {
                                    int ix = _ruleList.IndexOf(x);
                                    int iy = _ruleList.IndexOf(y);

                                    result = ix.CompareTo(iy);
                                }
                                return result;
                            });

                            _ruleArray = _ruleList.ToArray();
                        }
                    }
                }
                return _ruleArray;
            }
        }

        public string[] RuleNames
        {
            get
            {
                if (_ruleNames == null)
                {
                    lock (_syncLock)
                    {
                        if (_ruleNames == null)
                        {
                            string[] names = new string[_ruleIndex.Count];
                            _ruleIndex.Keys.CopyTo(names, 0);

                            _ruleNames = names;
                        }
                    }
                }
                return _ruleNames;
            }
        }

        public IRuleset AddRule(string ruleName, IRule rule)
        {
            ruleName = NormalizeName(ruleName);

            Validate(ruleName);
            Validate(rule);

            if (!ReferenceEquals(rule.Ruleset, null))
            {
                rule = (IRule)rule.Clone();
            }

            Rule rl = rule as Rule;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetRuleset(this);
                rl.SetName(ruleName);
            }

            lock (_syncLock)
            {
                _ruleArray = null;
                _ruleNames = null;

                _ruleIndex.Add(ruleName, rule);
                _ruleList.Add(rule);
            }

            return this;
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
            lock (_syncLock)
            {
                IRule[] rules = _ruleList.ToArray();

                _ruleArray = null;
                _ruleNames = null;

                _ruleIndex.Clear();

                if (rules.Length > 0)
                {
                    Rule rule;
                    foreach (IRule irule in rules)
                    {
                        rule = irule as Rule;
                        if (!ReferenceEquals(rule, null))
                        {
                            rule.SetRuleset(null);
                        }
                    }
                }
            }
            return this;
        }

        public override object Clone()
        {
            Ruleset result = new Ruleset();
            result.SetDescription(_description);

            lock (_syncLock)
            {
                foreach (IRule rule in _ruleIndex.Values)
                {
                    result.AddRule(rule.Name, (IRule)rule.Clone());
                }
            }

            return result;
        }

        public bool ContainsRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            lock (_syncLock)
            {
                return _ruleIndex.ContainsKey(ruleName);
            }
        }

        public IRule DefineRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);

            Validate(ruleName);

            Rule value = Rule.As();

            value.SetRuleset(this);
            value.SetName(ruleName);

            lock (_syncLock)
            {
                _ruleArray = null;
                _ruleNames = null;

                _ruleIndex.Add(ruleName, value);
                _ruleList.Add(value);
            }

            return value;
        }

        public override void Dispose()
        {
            lock (_syncLock)
            {
                // rules
                IRule[] rules = _ruleList.ToArray();

                _ruleArray = null;
                _ruleNames = null;

                _ruleIndex.Clear();
                _ruleList.Clear();

                if (rules.Length > 0)
                {
                    Rule rl;
                    foreach (IRule rule in rules)
                    {
                        rl = rule as Rule;
                        if (!ReferenceEquals(rl, null))
                        {
                            rl.SetRuleset(null);
                            rl.Dispose();
                        }
                    }
                }
            }

            base.Dispose();
        }

        public IRule GetRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            lock (_syncLock)
            {
                return _ruleIndex[ruleName];
            }
        }

        public bool IsEqualTo(IRuleset ruleset)
        {
            if (!ReferenceEquals(ruleset, null))
            {
                lock (_syncLock)
                {
                    if (!ReferenceEquals(ruleset, this) && (_ruleIndex.Count == ruleset.RuleCount))
                    {
                        foreach (string key in _ruleIndex.Keys)
                        {
                            IRule rule = ruleset.GetRule(key);
                            if (ReferenceEquals(rule, null) || !object.Equals(rule, _ruleIndex[key]))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }           
            return false;
        }

        public IRuleset RemoveRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            lock (_syncLock)
            {
                IRule irule;
                if (_ruleIndex.TryGetValue(ruleName, out irule))
                {
                    _ruleArray = null;
                    _ruleNames = null;

                    _ruleIndex.Remove(ruleName);
                    _ruleList.Remove(irule);

                    Rule rule = irule as Rule;
                    if (!ReferenceEquals(rule, null))
                    {
                        rule.SetRuleset(null);
                    }
                }
            }
            return this;
        }

        private void Validate(IRule rule)
        {
            if (ReferenceEquals(rule, null))
            {
                throw new ArgumentNullException("rule");
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

        private void Validate(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            if (String.IsNullOrEmpty(ruleName))
            {
                throw new ArgumentNullException("ruleName");
            }
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.Canceled || context.Halted);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (!ReferenceEquals(_ruleIndex, null) && (_ruleIndex.Count > 0))
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

            int index = 0;
            foreach (Rule rule in _ruleIndex.Values)
            {
                index++;
                if (index > 1)
                    builder.AppendLine();

                builder.AppendLine(rule.ToString());
            }

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }
    }
}
