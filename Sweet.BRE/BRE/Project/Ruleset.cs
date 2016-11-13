﻿/*
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
    public class Ruleset : ActionStm
    {
        private string _name;
        private string _description;

        private Project _project;

        private List<Rule> _ruleList;
        private Dictionary<string, Rule> _rules;

        public Ruleset()
            : base()
        {
            _ruleList = new List<Rule>();
            _rules = new Dictionary<string, Rule>();
        }

        internal Ruleset(string name)
            : this()
        {
            SetName(name);
        }

        public Rule this[string ruleName]
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
                    value = (Rule)value.Clone();
                }

                _rules[ruleName] = value;

                value.SetRuleset(this);
                value.SetName(ruleName);
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

        public Project Project
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
                return _rules.Count;
            }
        }

        public Rule[] Rules
        {
            get
            {
                Rule[] result = _ruleList.ToArray();
                if (result.Length > 1)
                {
                    Array.Sort<Rule>(result, new Comparison<Rule>(CompareRules));
                }

                return result;
            }
        }

        public string[] RuleNames
        {
            get
            {
                string[] array = new string[_rules.Count];
                _rules.Keys.CopyTo(array, 0);

                return array;
            }
        }

        public Ruleset AddRule(string ruleName, Rule rule)
        {
            ruleName = NormalizeName(ruleName);

            Validate(ruleName);
            Validate(rule);

            if (!ReferenceEquals(rule.Ruleset, null))
            {
                rule = (Rule)rule.Clone();
            }

            rule.SetRuleset(this);
            rule.SetName(ruleName);

            _ruleList.Add(rule);
            _rules.Add(ruleName, rule);

            return this;
        }

        public Ruleset SetDescription(string description)
        {
            _description = description;
            return this;
        }

        internal void SetProject(Project project)
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

        public Ruleset Clear()
        {
            Rule[] rules = _ruleList.ToArray();

            _rules.Clear();
            _ruleList.Clear();

            foreach (Rule me in rules)
            {
                me.SetRuleset(null);
            }

            return this;
        }

        public override object Clone()
        {
            Ruleset result = new Ruleset();
            result.SetDescription(_description);

            foreach (Rule me in _rules.Values)
            {
                result.AddRule(me.Name, (Rule)me.Clone());
            }

            return result;
        }

        public bool ContainsRule(string ruleName)
        {
            return _rules.ContainsKey(NormalizeName(ruleName));
        }

        public Rule DefineRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);

            Validate(ruleName);

            Rule value = Rule.As();

            value.SetRuleset(this);
            value.SetName(ruleName);

            _ruleList.Add(value);
            _rules.Add(ruleName, value);

            return value;
        }

        public override void Dispose()
        {
            // rules
            Rule[] rules = _ruleList.ToArray();

            _rules.Clear();
            _ruleList.Clear();

            foreach (Rule me in rules)
            {
                me.SetRuleset(null);
                me.Dispose();
            }

            base.Dispose();
        }

        public Rule GetRule(string ruleName)
        {
            return _rules[NormalizeName(ruleName)];
        }

        public bool IsEqualTo(Ruleset ruleset)
        {
            if (!ReferenceEquals(ruleset, null))
            {
                if (!ReferenceEquals(ruleset, this) && (_rules.Count == ruleset.RuleCount))
                {
                    foreach (string key in _rules.Keys)
                    {
                        Rule rule = ruleset.GetRule(key);
                        if (ReferenceEquals(rule, null) || !object.Equals(rule, _rules[key]))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            
            return false;
        }

        public Ruleset RemoveRule(string ruleName)
        {
            ruleName = NormalizeName(ruleName);
            if (ContainsRule(ruleName))
            {
                Rule rule = _rules[ruleName];
                
                _rules.Remove(ruleName);
                _ruleList.Remove(rule);

                rule.SetRuleset(null);
            }

            return this;
        }

        private void Validate(Rule rule)
        {
            if (ReferenceEquals(rule, null))
            {
                throw new ArgumentNullException("rule");
            }

            if (ReferenceEquals(rule.Ruleset, this))
            {
                throw new RuleException(BreResStrings.GetString("RuleAlreadyExistsInRuleset"));
            }

            string name = rule.Name;
            name = (name != null ? name.ToUpperInvariant() : String.Empty);

            if (ContainsRule(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRuleAlreadyExistsInRuleset"), name));
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

        private int CompareRules(Rule rule1, Rule rule2)
        {
            int priority1 = (!ReferenceEquals(rule1, null) ? (((10000 * (int)rule1.Priority)) + rule1.SubPriority) : 0);
            int priority2 = (!ReferenceEquals(rule2, null) ? (((10000 * (int)rule2.Priority)) + rule2.SubPriority) : 0);

            int result = priority1.CompareTo(priority2);
            if (result == 0)
            {
                int index1 = (!ReferenceEquals(rule1, null) ? _ruleList.IndexOf(rule1) : -1);
                int index2 = (!ReferenceEquals(rule2, null) ? _ruleList.IndexOf(rule2) : -1);

                result = index1.CompareTo(index2);
            }

            return result;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (!ReferenceEquals(_rules, null) && (_rules.Count > 0))
            {
                Rule[] rules = _ruleList.ToArray();
                if (rules.Length > 1)
                {
                    Array.Sort<Rule>(rules, new Comparison<Rule>(CompareRules));
                }
                
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
            foreach (Rule rule in _rules.Values)
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
