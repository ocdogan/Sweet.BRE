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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public sealed class Project : IProject, ICloneable, IDisposable
    {
        private VariableList _variables;
        private Dictionary<string, IRuleset> _rulesets;
        private Dictionary<string, DecisionTree> _decisionTrees;
        private Dictionary<string, DecisionTable> _decisionTables;
        private Dictionary<string, string> _funcAliases;

        public Project()
            : base()
        {
            _variables = new VariableList();
            _funcAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _rulesets = new Dictionary<string, IRuleset>(StringComparer.OrdinalIgnoreCase);
            _decisionTrees = new Dictionary<string, DecisionTree>(StringComparer.OrdinalIgnoreCase);
            _decisionTables = new Dictionary<string, DecisionTable>(StringComparer.OrdinalIgnoreCase);
        }

        public int DecisionTableCount
        {
            get
            {
                return _decisionTables.Count;
            }
        }

        public DecisionTable[] DecisionTables
        {
            get
            {
                DecisionTable[] array = new DecisionTable[_decisionTables.Count];
                _decisionTables.Values.CopyTo(array, 0);

                return array;
            }
        }

        public int DecisionTreeCount
        {
            get
            {
                return _decisionTrees.Count;
            }
        }

        public DecisionTree[] DecisionTrees
        {
            get
            {
                DecisionTree[] array = new DecisionTree[_decisionTrees.Count];
                _decisionTrees.Values.CopyTo(array, 0);

                return array;
            }
        }

        internal Dictionary<string, string> FuncAliases
        {
            get
            {
                return _funcAliases;
            }
        }

        public int RulesetCount
        {
            get
            {
                return _rulesets.Count;
            }
        }

        public IRuleset[] Rulesets
        {
            get
            {
                IRuleset[] array = new IRuleset[_rulesets.Count];
                _rulesets.Values.CopyTo(array, 0);

                return array;
            }
        }

        public string[] RulesetNames
        {
            get
            {
                string[] array = new string[_rulesets.Count];
                _rulesets.Keys.CopyTo(array, 0);

                return array;
            }
        }

        public string[] DecisionTableNames
        {
            get
            {
                string[] array = new string[_decisionTables.Count];
                _decisionTables.Keys.CopyTo(array, 0);

                return array;
            }
        }

        public string[] DecisionTreeNames
        {
            get
            {
                string[] array = new string[_decisionTrees.Count];
                _decisionTrees.Keys.CopyTo(array, 0);

                return array;
            }
        }

        public IVariableList Variables
        {
            get
            {
                return _variables;
            }
        }

        public IRuleset this[string rulesetName] 
        {
            get 
            {
                return GetRuleset(rulesetName);
            }
            set 
            {
                rulesetName = NormalizeName(rulesetName);

                Validate(rulesetName);
                Validate(value);

                if (!ReferenceEquals(value.Project, null))
                {
                    value = (Ruleset)value.Clone();
                }

                _rulesets[rulesetName] = value;

                Ruleset rs = value as Ruleset;
                if (!ReferenceEquals(rs, null))
                {
                    rs.SetProject(this);
                    rs.SetName(rulesetName);
                }
            }
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        public IRuleset GetRuleset(string rulesetName)
        {
            return _rulesets[NormalizeName(rulesetName)];
        }

        public DecisionTable GetTable(string tableName)
        {
            return _decisionTables[NormalizeName(tableName)];
        }

        public DecisionTree GetTree(string treeName)
        {
            return _decisionTrees[NormalizeName(treeName)];
        }

        private void Validate(IRuleset ruleset)
        {
            if ((object)ruleset == null)
            {
                throw new ArgumentNullException("ruleset");
            }

            if (ReferenceEquals(ruleset.Project, this))
            {
                throw new RuleException(BreResStrings.GetString("RulesetAlreadyExistsInProject"));
            }

            string name = ruleset.Name;
            name = (name != null ? name.ToUpperInvariant() : String.Empty);

            if (ContainsRuleset(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRulesetAlreadyExistsInProject"), name));
            }
        }

        private void Validate(string rulesetName)
        {
            rulesetName = NormalizeName(rulesetName);
            if (String.IsNullOrEmpty(rulesetName))
            {
                throw new ArgumentNullException("rulesetName");
            }
        }

        public IRuleset DefineRuleset(string rulesetName)
        {
            rulesetName = NormalizeName(rulesetName);

            Validate(rulesetName);

            Ruleset value = Ruleset.As();
            
            value.SetProject(this);
            value.SetName(rulesetName);

            _rulesets.Add(rulesetName, value);

            return value;
        }

        public DecisionTable DefineTable(string tableName)
        {
            tableName = NormalizeName(tableName);
            if (String.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (_decisionTables.ContainsKey(tableName))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedTableAlreadyExistsInProject"), tableName));
            }

            DecisionTable value = new DecisionTable();
            _decisionTables.Add(tableName, value);

            return value;
        }

        public DecisionTree DefineTree(string treeName)
        {
            treeName = NormalizeName(treeName);

            if (String.IsNullOrEmpty(treeName))
            {
                throw new ArgumentNullException("treeName");
            }

            if (_decisionTrees.ContainsKey(treeName))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedTreeAlreadyExistsInProject"), treeName));
            }

            DecisionTree value = new DecisionTree();
            _decisionTrees.Add(treeName, value);

            return value;
        }

        public IProject AddRuleset(string rulesetName, IRuleset ruleset)
        {
            rulesetName = NormalizeName(rulesetName);

            Validate(rulesetName);
            Validate(ruleset);

            if (!ReferenceEquals(ruleset.Project, null))
            {
                ruleset = (Ruleset)ruleset.Clone();
            }

            Ruleset rs = ruleset as Ruleset;
            if (!ReferenceEquals(rs, null))
            {
                rs.SetProject(this);
                rs.SetName(rulesetName);
            }

            _rulesets.Add(rulesetName, ruleset);

            return this;
        }

        public static Project As()
        {
            return new Project();
        }

        public IProject Clear()
        {
            _decisionTables.Clear();
            _decisionTrees.Clear();

            Ruleset[] rulesets = new Ruleset[_rulesets.Count];
            _rulesets.Values.CopyTo(rulesets, 0);

            _rulesets.Clear();

            foreach (Ruleset me in rulesets)
            {
                me.SetProject(null);
            }

            return this;
        }

        public object Clone()
        {
            Project result = new Project();
            foreach (Ruleset me in _rulesets.Values)
            {
                result.AddRuleset(me.Name, (Ruleset)me.Clone());
            }

            foreach (string key in _decisionTables.Keys)
            {
                DecisionTable table = _decisionTables[key];
                result._decisionTables.Add(key, (DecisionTable)((ICloneable)table).Clone());
            }

            foreach (string key in _decisionTrees.Keys)
            {
                DecisionTree tree = _decisionTrees[key];
                result._decisionTrees.Add(key, (DecisionTree)((ICloneable)tree).Clone());
            }

            return result;
        }

        public bool ContainsRuleset(string rulesetName)
        {
            return _rulesets.ContainsKey(NormalizeName(rulesetName));
        }

        public bool ContainsTable(string tableName)
        {
            return _decisionTables.ContainsKey(NormalizeName(tableName));
        }

        public bool ContainsTree(string treeName)
        {
            return _decisionTrees.ContainsKey(NormalizeName(treeName));
        }

        public void Dispose()
        {
            _funcAliases.Clear();

            // rulesets
            Ruleset[] rulesets = new Ruleset[_rulesets.Count];
            _rulesets.Values.CopyTo(rulesets, 0);

            _rulesets.Clear();
            
            foreach (Ruleset me in rulesets)
            {
                me.SetProject(null);
                me.Dispose();
            }

            // decision tables
            DecisionTable[] tables = new DecisionTable[_decisionTables.Count];
            _decisionTables.Values.CopyTo(tables, 0);

            _decisionTables.Clear();

            foreach (DecisionTable table in tables)
            {
                ((IDisposable)table).Dispose();
            }

            // decision trees
            DecisionTree[] trees = new DecisionTree[_decisionTrees.Count];
            _decisionTrees.Values.CopyTo(trees, 0);

            _decisionTrees.Clear();

            foreach (DecisionTree tree in trees)
            {
                ((IDisposable)tree).Dispose();
            }
        }

        public bool IsEqualTo(IProject prj)
        {
            if (ReferenceEquals(prj, this))
            {
                return true;
            }

            if (ReferenceEquals(prj, null))
            {
                return false;
            }

            // Variables
            if (_variables.Count != prj.Variables.Count)
            {
                return false;
            }

            foreach (IVariable key in _variables)
            {
                IVariable vr = prj.Variables.Get(key.Name);
                if ((vr == null) || !object.Equals(key, vr))
                {
                    return false;
                }
            }

            // Function Aliases
            if (prj is Project)
            {
                int faCount1 = 0;
                if (_funcAliases != null)
                {
                    faCount1 = _funcAliases.Count;
                }

                Dictionary<string, string> _fa = ((Project)prj)._funcAliases;

                int faCount2 = 0;
                if (_fa != null)
                {
                    faCount2 = _fa.Count;
                }

                if (faCount1 != faCount2)
                {
                    return false;
                }

                if (faCount1 > 0)
                {
                    foreach (string alias in _funcAliases.Keys)
                    {
                        if (!_fa.ContainsKey(alias))
                        {
                            return false;
                        }

                        string func1 = _funcAliases[alias];
                        string func2 = _fa[alias];

                        if (func1 != func2)
                        {
                            return false;
                        }
                    }
                }
            }

            // Decision tables
            if (_decisionTables.Count != prj.DecisionTableCount)
            {
                return false;
            }

            foreach (string key in _decisionTables.Keys)
            {
                DecisionTable dct = prj.GetTable(key);
                if ((dct == null) || !dct.IsEqualTo(_decisionTables[key]))
                {
                    return false;
                }
            }

            // Decision trees
            if (_decisionTrees.Count != prj.DecisionTreeCount)
            {
                return false;
            }

            foreach (string key in _decisionTrees.Keys)
            {
                DecisionTree dct = prj.GetTree(key);
                if ((dct == null) || !dct.IsEqualTo(_decisionTrees[key]))
                {
                    return false;
                }
            }

            // Rulesets
            if (_rulesets.Count != prj.RulesetCount)
            {
                return false;
            }

            foreach (string key in _rulesets.Keys)
            {
                IRuleset rs = prj.GetRuleset(key);
                if (ReferenceEquals(rs, null) || !rs.IsEqualTo(_rulesets[key]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string[] GetFunctionAliases()
        {
            lock (_funcAliases)
            {
                string[] result = new string[_funcAliases.Count];
                _funcAliases.Keys.CopyTo(result, 0);

                return result;
            }
        }

        public IProject RegisterFunctionAlias(string alias, string function)
        {
            alias = ((alias != null) ? alias.Trim() : null);
            function = ((function != null) ? function.Trim() : null);

            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (String.IsNullOrEmpty(function))
            {
                throw new ArgumentNullException("function");
            }

            lock (_funcAliases)
            {
                _funcAliases[alias] = function;
            }
            return this;
        }

        public string ResolveFunctionAlias(string alias)
        {
            alias = ((alias != null) ? alias.Trim() : null);

            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            string function = alias;

            lock (_funcAliases)
            {
                if (_funcAliases.ContainsKey(alias))
                {
                    string tempFunction = _funcAliases[alias];
                    if (!String.IsNullOrEmpty(tempFunction))
                    {
                        function = tempFunction;
                    }
                }
            }

            return function;
        }

        public IProject RemoveRuleset(string rulesetName)
        {
            rulesetName = NormalizeName(rulesetName);
            if (ContainsRuleset(rulesetName))
            {
                IRuleset ruleset = _rulesets[rulesetName];
                _rulesets.Remove(rulesetName);

                Ruleset rs = ruleset as Ruleset;
                if (!ReferenceEquals(rs, null))
                {
                    rs.SetProject(null);
                }
            }

            return this;
        }

        public IProject RemoveTable(string tableName)
        {
            tableName = NormalizeName(tableName);
            if (ContainsTable(tableName))
            {
                _decisionTables.Remove(tableName);
            }

            return this;
        }

        public IProject RemoveTree(string treeName)
        {
            treeName = NormalizeName(treeName);
            if (ContainsTable(treeName))
            {
                _decisionTrees.Remove(treeName);
            }

            return this;
        }

        public IProject UnregisterFunctionAlias(string alias, string function)
        {
            alias = ((alias != null) ? alias.Trim() : null);
            function = ((function != null) ? function.Trim() : null);

            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (String.IsNullOrEmpty(function))
            {
                throw new ArgumentNullException("function");
            }

            lock (_funcAliases)
            {
                if (_funcAliases.ContainsKey(alias))
                {
                    _funcAliases.Remove(alias);
                }
            }
            return this;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            int index = 0;
            foreach (Ruleset me in _rulesets.Values)
            {
                index++;

                builder.Append(me.ToString());
                builder.Append(" ");

                builder.AppendLine();
                if (index < _rulesets.Count)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }
    }
}
