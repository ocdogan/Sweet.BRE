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
    public sealed class DecisionTable : IStatement, INamedObject, IDecision, IDisposable, ICloneable
    {
        private string _name;
        private string _description;

        private IProject _project;
        private INamedObjectList _ownerList;

        private DecisionColumnList _actions;
        private DecisionColumnList _conditions;
        private DecisionRowList _rows;

        private bool _evaluateAllRows = false;

        public DecisionTable()
            : base()
        {
            _actions = new DecisionColumnList(this);
            _conditions = new DecisionColumnList(this);
            _rows = new DecisionRowList(this);
        }

        internal DecisionTable(string name)
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

        public IProject Project
        {
            get { return _project; }
        }

        INamedObjectList INamedObject.List
        {
            get { return _ownerList; }
        }

        internal void SetOwnerList(INamedObjectList list)
        {
            _ownerList = list;
        }

        public DecisionTable SetDescription(string description)
        {
            _description = description;
            return this;
        }

        internal void SetName(string name)
        {
            _name = (name != null ? name.Trim() : null);
        }

        internal void SetProject(IProject project)
        {
            _project = project;
        }

        public DecisionColumnList Actions
        {
            get
            {
                return _actions;
            }
        }

        public DecisionColumnList Conditions
        {
            get
            {
                return _conditions;
            }
        }

        public bool EvaluateAllRows
        {
            get
            {
                return _evaluateAllRows;
            }
            set
            {
                _evaluateAllRows = value;
            }
        }

        public DecisionRowList Rows
        {
            get
            {
                return _rows;
            }
        }

        public object Clone()
        {
            DecisionTable cln = new DecisionTable();
            foreach (DecisionColumn col in _actions)
            {
                object obj = ((ICloneable)col).Clone();
                cln.Actions.Add((DecisionColumn)obj);
            }

            foreach (DecisionColumn col in _conditions)
            {
                object obj = ((ICloneable)col).Clone();
                cln.Conditions.Add((DecisionColumn)obj);
            }

            foreach (DecisionRow row in _rows)
            {
                object obj = ((ICloneable)row).Clone();
                cln.Rows.Add((DecisionRow)obj);
            }

            return cln;
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, this)) return 0;
            if (ReferenceEquals(obj, null)) return -1;

            DecisionTable dt = obj as DecisionTable;
            if (ReferenceEquals(dt, null)) return -1;

            return Name.CompareTo(dt.Name);
        }

        void IDisposable.Dispose()
        {
            _rows.Dispose();
            _actions.Dispose();
            _conditions.Dispose();
        }

        # region Editing functions

        public void Flatten()
        {
            DecisionTableEditor.Flatten(this);
        }

        public void MergeCells()
        {
            DecisionTableEditor.MergeCells(this);
        }

        public void AddAction(int index, string name, string description, DecisionValueType type)
        {
            DecisionTableEditor.AddAction(this, index, name, description, type);
        }

        public void AddCondition(int index, string name, string description, DecisionValueType type)
        {
            DecisionTableEditor.AddCondition(this, index, name, description, type);
        }

        public void RemoveAction(int index)
        {
            DecisionTableEditor.RemoveAction(this, index);
        }

        public void RemoveAction(string name)
        {
            DecisionTableEditor.RemoveAction(this, name);
        }

        public void RemoveCondition(int index)
        {
            DecisionTableEditor.RemoveCondition(this, index);
        }

        public void RemoveCondition(string name)
        {
            DecisionTableEditor.RemoveCondition(this, name);
        }

        public void ReorderAction(int currIndex, int newIndex)
        {
            DecisionTableEditor.ReorderAction(this, currIndex, newIndex);
        }

        public void ReorderCondition(int currIndex, int newIndex)
        {
            DecisionTableEditor.ReorderCondition(this, currIndex, newIndex);
        }

        # endregion

        public int GetColumnNo(DecisionConditionCell condition)
        {
            if ((condition != null) && ReferenceEquals(this, condition.Owner))
            {
                int result = -1;
                DecisionCell parent = condition;

                while (parent != null)
                {
                    result++;

                    DecisionCell child = parent;
                    parent = parent.Parent;

                    if (parent is DecisionConditionCell)
                    {
                        DecisionConditionCell cond = (DecisionConditionCell)parent;
                        if (ReferenceEquals(cond.Else, child))
                            result--;
                    }
                }

                return (result < 0) ? 0 : result;
            }

            return -1;
        }

        public int GetColumnNo(DecisionActionCell action)
        {
            if ((action != null) && ReferenceEquals(this, action.Owner))
            {
                int result = 0;
                DecisionCell parent = action.Parent;

                while (parent is DecisionActionCell)
                {
                    result++;
                    parent = parent.Parent;
                }

                return result;
            }

            return -1;
        }

        private bool EqualActions(DecisionColumnList actions)
        {
            if (actions.Count == _actions.Count)
            {
                foreach (DecisionColumn col in actions)
                {
                    DecisionColumn da = _actions.Get(col.Name);
                    if ((da == null) || !object.Equals(da, col))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool EqualConditions(DecisionColumnList conditions)
        {
            if (conditions.Count == _conditions.Count)
            {
                int i = 0;
                foreach (DecisionColumn col in conditions)
                {
                    DecisionColumn dc = _conditions[i++];
                    if ((dc == null) || !object.Equals(dc, col))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool EqualRows(DecisionRowList rows)
        {
            if (rows.Count == _rows.Count)
            {
                int i = 0;
                foreach (DecisionRow row in rows)
                {
                    DecisionRow dr = _rows[i++];
                    if ((dr == null) || !dr.IsEqualTo(row))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool IsEqualTo(DecisionTable table)
        {
            return ReferenceEquals(table, this) || (!ReferenceEquals(table, null) &&
                EqualActions(table.Actions) &&
                EqualConditions(table.Conditions) &&
                EqualRows(table.Rows));
        }

        public object Evaluate(IEvaluationContext context, params object[] args)
        {
            bool result = false;

            if (_rows.Count > 0)
            {
                using (TableEvaluator e = new TableEvaluator(this))
                {
                    foreach (DecisionRow row in _rows)
                    {
                        DecisionCell cell = row.Root;

                        if (cell is DecisionActionCell)
                        {
                            result = true;
                            e.Fire(context, (DecisionActionCell)cell);
                        }
                        else if (cell is DecisionConditionCell)
                        {
                            result = e.Evaluate(context, (DecisionConditionCell)cell) || result;
                        }

                        if (result && !_evaluateAllRows)
                            break;
                    }
                }
            }

            return result;
        }
    }
}
