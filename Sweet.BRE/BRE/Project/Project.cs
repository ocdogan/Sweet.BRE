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
        private Dictionary<string, string> _funcAliases;

        private NamedObjectList<IRuleset> _rulesetList;
        private NamedObjectList<DecisionTree> _decisionTreeList;
        private NamedObjectList<DecisionTable> _decisionTableList;

        private readonly object _syncLock = new object();

        public Project()
            : base()
        {
            _variables = new VariableList();
            _funcAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            SetRulesetList(new NamedObjectList<IRuleset>());
            SetDecisionTreeList(new NamedObjectList<DecisionTree>());
            SetDecisionTableList(new NamedObjectList<DecisionTable>());
        }

        internal Dictionary<string, string> FuncAliases
        {
            get
            {
                return _funcAliases;
            }
        }

        public IVariableList Variables
        {
            get
            {
                return _variables;
            }
        }

        public IRuleset this[string name] 
        {
            get 
            {
                return GetRuleset(name);
            }
            set 
            {
                AddRuleset(name, value);
            }
        }

        private string NormalizeName(string name)
        {
            return (name != null ? name.Trim() : String.Empty);
        }

        public static Project As()
        {
            return new Project();
        }

        public IProject Clear()
        {
            _rulesetList.Clear();
            _decisionTreeList.Clear();
            _decisionTableList.Clear();

            return this;
        }

        public object Clone()
        {
            Project result = new Project();

            NamedObjectList<IRuleset> rulesetList;
            lock (_syncLock)
            {
                rulesetList = (NamedObjectList<IRuleset>)_rulesetList.Clone();
            }
            result.SetRulesetList(rulesetList);

            NamedObjectList<DecisionTable> tableList;
            lock (_syncLock)
            {
                tableList = (NamedObjectList<DecisionTable>)_decisionTableList.Clone();
            }
            result.SetDecisionTableList(tableList);

            NamedObjectList<DecisionTree> treeList;
            lock (_syncLock)
            {
                treeList = (NamedObjectList<DecisionTree>)_decisionTreeList.Clone();
            }
            result.SetDecisionTreeList(treeList);

            return result;
        }

        public void Dispose()
        {
            _variables.Clear();
            _funcAliases.Clear();

            // decision trees
            _rulesetList.Dispose();

            // decision trees
            _decisionTreeList.Dispose();

            // decision tables
            _decisionTableList.Dispose();
        }

        public bool IsEqualTo(IProject iproject)
        {
            if (ReferenceEquals(iproject, this)) return true;
            if (ReferenceEquals(iproject, null)) return false;

            // Variables
            if (_variables.Count != iproject.Variables.Count)
            {
                return false;
            }

            foreach (IVariable key in _variables)
            {
                IVariable vr = iproject.Variables.Get(key.Name);
                if ((vr == null) || !object.Equals(key, vr))
                {
                    return false;
                }
            }

            Project project = iproject as Project;
            if (!ReferenceEquals(project, null))
            {
                // Function Aliases
                int faCount1 = 0;
                if (_funcAliases != null)
                {
                    faCount1 = _funcAliases.Count;
                }

                Dictionary<string, string> _fa = ((Project)iproject)._funcAliases;

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

                // Rulesets
                if (!_rulesetList.Equals(project._rulesetList))
                {
                    return false;
                }

                // Decision trees
                if (!_decisionTreeList.Equals(project._decisionTreeList))
                {
                    return false;
                }

                // Decision tables
                if (!_decisionTableList.Equals(project._decisionTableList))
                {
                    return false;
                }
            }
            else
            {
                // Rulesets
                if (_rulesetList.Count != iproject.RulesetCount)
                {
                    return false;
                }

                foreach (IRuleset dt1 in _rulesetList.Objects)
                {
                    if (!ReferenceEquals(dt1, null))
                    {
                        IRuleset dt2 = iproject.GetRuleset(dt1.Name);
                        if (ReferenceEquals(dt2, null) || !dt1.IsEqualTo(dt2))
                        {
                            return false;
                        }
                    }
                }

                // Decision trees
                if (_decisionTreeList.Count != iproject.DecisionTreeCount)
                {
                    return false;
                }

                foreach (DecisionTree dt1 in _decisionTreeList.Objects)
                {
                    if (!ReferenceEquals(dt1, null))
                    {
                        DecisionTree dt2 = iproject.GetDecisionTree(dt1.Name);
                        if (ReferenceEquals(dt2, null) || !dt1.IsEqualTo(dt2))
                        {
                            return false;
                        }
                    }
                }

                // Decision tables
                if (_decisionTableList.Count != iproject.DecisionTableCount)
                {
                    return false;
                }

                foreach (DecisionTable dt1 in _decisionTableList.Objects)
                {
                    if (!ReferenceEquals(dt1, null))
                    {
                        DecisionTable dt2 = iproject.GetDecisionTable(dt1.Name);
                        if (ReferenceEquals(dt2, null) || !dt1.IsEqualTo(dt2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (obj is IProject) return IsEqualTo((IProject)obj);
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
            alias = ((alias != null) ? alias.Trim().ToUpperInvariant() : null);
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

            string tempFunction;
            lock (_funcAliases)
            {
                _funcAliases.TryGetValue(alias.ToUpperInvariant(), out tempFunction);
            }

            string function = alias;
            if (!String.IsNullOrEmpty(tempFunction))
            {
                function = tempFunction;
            }

            return function;
        }

        public IProject UnregisterFunctionAlias(string alias)
        {
            alias = ((alias != null) ? alias.Trim() : null);

            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            alias = alias.ToUpperInvariant();
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

            IRuleset[] rulesets = _rulesetList.Objects;
            if (!ReferenceEquals(rulesets, null) && (rulesets.Length > 0))
            {
                int index = 0;
                foreach (Ruleset ruleset in rulesets)
                {
                    index++;

                    builder.Append(ruleset.ToString());
                    builder.Append(" ");

                    builder.AppendLine();
                    if (index < rulesets.Length)
                    {
                        builder.AppendLine();
                    }
                }
            }
            return builder.ToString();
        }

        # region DecisionTree

        public int DecisionTreeCount
        {
            get
            {
                return _decisionTreeList.Count;
            }
        }

        public DecisionTree[] DecisionTrees
        {
            get
            {
                return _decisionTreeList.Objects;
            }
        }

        public string[] DecisionTreeNames
        {
            get
            {
                return _decisionTreeList.Names;
            }
        }

        internal void SetDecisionTreeList(NamedObjectList<DecisionTree> list)
        {
            INamedObjectList oldList = _decisionTreeList;
            if (!ReferenceEquals(oldList, null))
            {
                oldList.OnAdd -= decisionTreeListOnAdd;
                oldList.OnDispose -= decisionTreeListOnDispose;
                oldList.OnRemove -= decisionTreeListOnRemove;
                oldList.OnValidate -= decisionTreeListOnValidate;
                oldList.OnValidateName -= decisionTreeListOnValidateName;
            }

            _decisionTreeList = list;
            if (!ReferenceEquals(list, null))
            {
                list.OnAdd += decisionTreeListOnAdd;
                list.OnDispose += decisionTreeListOnDispose;
                list.OnRemove += decisionTreeListOnRemove;
                list.OnValidate += decisionTreeListOnValidate;
                list.OnValidateName += decisionTreeListOnValidateName;
            }
        }

        private string ValidateDecisionTreeName(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return name;
        }

        private void decisionTreeListOnValidateName(INamedObjectList list, INamedObject obj, string name)
        {
            ValidateDecisionTreeName(name);
        }

        private void decisionTreeListOnValidate(INamedObjectList list, INamedObject obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("decisionTree");
            }

            DecisionTree decisionTree = obj as DecisionTree;
            if (ReferenceEquals(decisionTree, null))
            {
                throw new RuleException(BreResStrings.GetString("ExpectingDecisionTreeObject"));
            }

            if (ReferenceEquals(decisionTree.Project, this))
            {
                throw new RuleException(BreResStrings.GetString("DecisionTreeAlreadyExistsInProject"));
            }

            string name = NormalizeName(decisionTree.Name);
            if (ContainsDecisionTree(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedDecisionTreeAlreadyExistsInProject"), name));
            }
        }

        private void decisionTreeListOnRemove(INamedObjectList list, INamedObject obj)
        {
            DecisionTree rl = obj as DecisionTree;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetProject(null);
                rl.SetOwnerList(null);
            }
        }

        private void decisionTreeListOnDispose(INamedObjectList list, INamedObject obj)
        {
            DecisionTree decisionTree = obj as DecisionTree;
            if (!ReferenceEquals(decisionTree, null))
            {
                decisionTree.SetProject(null);
                decisionTree.SetOwnerList(null);
            }
            
            if (obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        private void decisionTreeListOnAdd(INamedObjectList list, INamedObject obj, string name)
        {
            DecisionTree decisionTree = obj as DecisionTree;
            if (!ReferenceEquals(decisionTree, null))
            {
                decisionTree.SetProject(this);
                decisionTree.SetName(name);
                decisionTree.SetOwnerList(list);
            }
        }

        public IProject AddDecisionTree(string name, DecisionTree decisionTree)
        {
            _decisionTreeList.Add(name, decisionTree);
            return this;
        }

        public bool ContainsDecisionTree(string name)
        {
            return _decisionTreeList.Contains(name);
        }

        public DecisionTree DefineDecisionTree(string name)
        {
            name = ValidateDecisionTreeName(name);
            if (ContainsDecisionTree(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedDecisionTreeAlreadyExistsInProject"), name));
            }

            DecisionTree value = new DecisionTree();
            _decisionTreeList.Add(name, value);

            return value;
        }

        public DecisionTree GetDecisionTree(string name)
        {
            return _decisionTreeList.Get(name);
        }

        public IProject RemoveDecisionTree(string name)
        {
            _decisionTreeList.Remove(name);
            return this;
        }

        #endregion DecisionTree

        #region DecisionTable

        public int DecisionTableCount
        {
            get
            {
                return _decisionTableList.Count;
            }
        }

        public DecisionTable[] DecisionTables
        {
            get
            {
                return _decisionTableList.Objects;
            }
        }

        public string[] DecisionTableNames
        {
            get
            {
                return _decisionTableList.Names;
            }
        }

        internal void SetDecisionTableList(NamedObjectList<DecisionTable> list)
        {
            INamedObjectList oldList = _decisionTableList;
            if (!ReferenceEquals(oldList, null))
            {
                oldList.OnAdd -= decisionTableListOnAdd;
                oldList.OnDispose -= decisionTableListOnDispose;
                oldList.OnRemove -= decisionTableListOnRemove;
                oldList.OnValidate -= decisionTableListOnValidate;
                oldList.OnValidateName -= decisionTableListOnValidateName;
            }

            _decisionTableList = list;
            if (!ReferenceEquals(list, null))
            {
                list.OnAdd += decisionTableListOnAdd;
                list.OnDispose += decisionTableListOnDispose;
                list.OnRemove += decisionTableListOnRemove;
                list.OnValidate += decisionTableListOnValidate;
                list.OnValidateName += decisionTableListOnValidateName;
            }
        }

        private string ValidateDecisionTableName(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return name;
        }

        private void decisionTableListOnValidateName(INamedObjectList list, INamedObject obj, string name)
        {
            ValidateDecisionTableName(name);
        }

        private void decisionTableListOnValidate(INamedObjectList list, INamedObject obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("decisionTable");
            }

            DecisionTable decisionTable = obj as DecisionTable;
            if (ReferenceEquals(decisionTable, null))
            {
                throw new RuleException(BreResStrings.GetString("ExpectingDecisionTableObject"));
            }

            if (ReferenceEquals(decisionTable.Project, this))
            {
                throw new RuleException(BreResStrings.GetString("DecisionTableAlreadyExistsInProject"));
            }

            string name = NormalizeName(decisionTable.Name);
            if (ContainsDecisionTable(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedDecisionTableAlreadyExistsInProject"), name));
            }
        }

        private void decisionTableListOnRemove(INamedObjectList list, INamedObject obj)
        {
            DecisionTable rl = obj as DecisionTable;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetProject(null);
                rl.SetOwnerList(null);
            }
        }

        private void decisionTableListOnDispose(INamedObjectList list, INamedObject obj)
        {
            DecisionTable decisionTable = obj as DecisionTable;
            if (!ReferenceEquals(decisionTable, null))
            {
                decisionTable.SetProject(null);
                decisionTable.SetOwnerList(null);
            }
            
            if (obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        private void decisionTableListOnAdd(INamedObjectList list, INamedObject obj, string name)
        {
            DecisionTable decisionTable = obj as DecisionTable;
            if (!ReferenceEquals(decisionTable, null))
            {
                decisionTable.SetProject(this);
                decisionTable.SetName(name);
                decisionTable.SetOwnerList(list);
            }
        }

        public IProject AddDecisionTable(string name, DecisionTable decisionTable)
        {
            _decisionTableList.Add(name, decisionTable);
            return this;
        }

        public bool ContainsDecisionTable(string name)
        {
            return _decisionTableList.Contains(name);
        }

        public DecisionTable DefineDecisionTable(string name)
        {
            name = ValidateDecisionTableName(name);
            if (ContainsDecisionTable(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedDecisionTableAlreadyExistsInProject"), name));
            }

            DecisionTable value = new DecisionTable();
            _decisionTableList.Add(name, value);

            return value;
        }

        public DecisionTable GetDecisionTable(string name)
        {
            return _decisionTableList.Get(name);
        }

        public IProject RemoveDecisionTable(string name)
        {
            _decisionTableList.Remove(name);
            return this;
        }

        #endregion DecisionTable

        #region Ruleset

        public int RulesetCount
        {
            get
            {
                return _rulesetList.Count;
            }
        }

        public IRuleset[] Rulesets
        {
            get
            {
                return _rulesetList.Objects;
            }
        }

        public string[] RulesetNames
        {
            get
            {
                return _rulesetList.Names;
            }
        }

        internal void SetRulesetList(NamedObjectList<IRuleset> list)
        {
            INamedObjectList oldList = _rulesetList;
            if (!ReferenceEquals(oldList, null))
            {
                oldList.OnAdd -= rulesetListOnAdd;
                oldList.OnDispose -= rulesetListOnDispose;
                oldList.OnRemove -= rulesetListOnRemove;
                oldList.OnValidate -= rulesetListOnValidate;
                oldList.OnValidateName -= rulesetListOnValidateName;
            }

            _rulesetList = list;
            if (!ReferenceEquals(list, null))
            {
                list.OnAdd += rulesetListOnAdd;
                list.OnDispose += rulesetListOnDispose;
                list.OnRemove += rulesetListOnRemove;
                list.OnValidate += rulesetListOnValidate;
                list.OnValidateName += rulesetListOnValidateName;
            }
        }

        private string ValidateRulesetName(string name)
        {
            name = NormalizeName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return name;
        }

        private void rulesetListOnValidateName(INamedObjectList list, INamedObject obj, string name)
        {
            ValidateRulesetName(name);
        }

        private void rulesetListOnValidate(INamedObjectList list, INamedObject obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("ruleset");
            }

            Ruleset ruleset = obj as Ruleset;
            if (ReferenceEquals(ruleset, null))
            {
                throw new RuleException(BreResStrings.GetString("ExpectingRulesetObject"));
            }

            if (ReferenceEquals(ruleset.Project, this))
            {
                throw new RuleException(BreResStrings.GetString("RulesetAlreadyExistsInProject"));
            }

            string name = NormalizeName(ruleset.Name);
            if (ContainsRuleset(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRulesetAlreadyExistsInProject"), name));
            }
        }

        private void rulesetListOnRemove(INamedObjectList list, INamedObject obj)
        {
            Ruleset rl = obj as Ruleset;
            if (!ReferenceEquals(rl, null))
            {
                rl.SetProject(null);
                rl.SetOwnerList(null);
            }
        }

        private void rulesetListOnDispose(INamedObjectList list, INamedObject obj)
        {
            Ruleset ruleset = obj as Ruleset;
            if (!ReferenceEquals(ruleset, null))
            {
                ruleset.SetProject(null);
                ruleset.SetOwnerList(null);
            }
            
            if (obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        private void rulesetListOnAdd(INamedObjectList list, INamedObject obj, string name)
        {
            Ruleset ruleset = obj as Ruleset;
            if (!ReferenceEquals(ruleset, null))
            {
                ruleset.SetProject(this);
                ruleset.SetName(name);
                ruleset.SetOwnerList(list);
            }
        }

        public IProject AddRuleset(string name, IRuleset ruleset)
        {
            _rulesetList.Add(name, ruleset);
            return this;
        }

        public bool ContainsRuleset(string name)
        {
            return _rulesetList.Contains(name);
        }

        public IRuleset DefineRuleset(string name)
        {
            name = ValidateRulesetName(name);
            if (ContainsRuleset(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedRulesetAlreadyExistsInProject"), name));
            }

            Ruleset value = new Ruleset();
            _rulesetList.Add(name, value);

            return value;
        }

        public IRuleset GetRuleset(string name)
        {
            return _rulesetList.Get(name);
        }

        public IProject RemoveRuleset(string name)
        {
            _rulesetList.Remove(name);
            return this;
        }

        #endregion Ruleset
    }
}
