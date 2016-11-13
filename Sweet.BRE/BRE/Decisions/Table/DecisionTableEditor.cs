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
    public sealed class DecisionTableEditor : IDisposable
    {
        # region TableRow

        private class TableRow : IDisposable
        {
            private int _mergedOn = -1;

            private List<DecisionActionCell> _actions;
            private List<DecisionConditionCell> _conditions;

            public TableRow()
            {
                _actions = new List<DecisionActionCell>();
                _conditions = new List<DecisionConditionCell>();
            }

            public List<DecisionActionCell> Actions
            {
                get
                {
                    return _actions;
                }
            }

            public List<DecisionConditionCell> Conditions
            {
                get
                {
                    return _conditions;
                }
            }

            public int MergedOn
            {
                get
                {
                    return _mergedOn;
                }
            }

            public void Dispose()
            {
                _actions.Clear();
                _conditions.Clear();
            }

            internal void SetMergedOn(int mergedOn)
            {
                mergedOn = (mergedOn < 0) ? -1 : mergedOn;
                if ((_mergedOn < 0) || (mergedOn < _mergedOn))
                {
                    _mergedOn = mergedOn;
                }
            }

            internal void Add(DecisionActionCell action)
            {
                _actions.Add(action);
            }

            internal void Add(DecisionActionCell[] actions)
            {
                if (actions != null)
                {
                    _actions.AddRange(actions);
                }
            }

            internal void Add(DecisionConditionCell condition)
            {
                _conditions.Add(condition);
            }

            internal void Add(DecisionConditionCell[] conditions)
            {
                if (conditions != null)
                {
                    _conditions.AddRange(conditions);
                }
            }
        }

        # endregion

        # region Instance fields

        private DecisionTable _table;

        private List<TableRow> _rows;
        private Stack<DecisionConditionCell> _stack;

        private DecisionTableEditor(DecisionTable table)
        {
            _table = table;

            _rows = new List<TableRow>();
            _stack = new Stack<DecisionConditionCell>();
        }

        public void Dispose()
        {
            _table = null;

            TableRow[] rows = _rows.ToArray();
            _rows.Clear();

            foreach (TableRow row in rows)
            {
                row.Dispose();
            }
        }

        # endregion

        # region Flatten

        private TableRow ProcessStack()
        {
            TableRow result = new TableRow();
            _rows.Add(result);

            if (_stack.Count > 0)
            {
                DecisionConditionCell[] conditions = _stack.ToArray();
                Array.Reverse(conditions);

                DecisionConditionCell last = null;
                foreach (DecisionConditionCell item in conditions)
                {
                    DecisionConditionCell condition = ((item != null) ? (DecisionConditionCell)item.Copy() : null);
                    result.Add(condition);

                    if (last != null)
                    {
                        last.SetOnMatch(condition);
                    }
                    last = condition;
                }
            }

            return result;
        }

        private void ProcessActions(TableRow row, DecisionActionCell rootAction)
        {
            if ((row != null) && (rootAction != null))
            {
                DecisionActionCell last = null;
                DecisionActionCell item = rootAction;

                while (item != null)
                {
                    DecisionActionCell action = ((item != null) ? (DecisionActionCell)item.Copy() : null);
                    row.Add(action);

                    item = item.FurtherMore;

                    if (last != null)
                    {
                        last.SetFurtherMore(action);
                    }
                    last = action;
                }

                if (row.Conditions.Count > 0)
                {
                    DecisionConditionCell lastCondition = row.Conditions[row.Conditions.Count - 1];
                    lastCondition.SetOnMatch(row.Actions[0]);
                }
            }
        }

        private void ProcessCondition(DecisionConditionCell condition)
        {
            _stack.Push(condition);
            try
            {
                DecisionCell onMatch = condition.OnMatch;
                if (onMatch is DecisionConditionCell)
                {
                    ProcessCondition((DecisionConditionCell)onMatch);
                }
                else
                {
                    TableRow row = ProcessStack();

                    DecisionActionCell action = (onMatch as DecisionActionCell);
                    if (action != null)
                    {
                        ProcessActions(row, action);
                    }
                }
            }
            finally
            {
                _stack.Pop();
            }

            DecisionConditionCell elseMatch = condition.Else as DecisionConditionCell;
            if (elseMatch != null)
            {
                ProcessCondition(elseMatch);
            }
        }

        private void FlattenTable()
        {
            if (_table.Rows.Count > 0)
            {
                foreach (DecisionRow row in _table.Rows)
                {
                    DecisionCell root = row.Root;

                    if (root is DecisionConditionCell)
                    {
                        ProcessCondition((DecisionConditionCell)root);
                    }
                    else
                        if (root is DecisionActionCell)
                        {
                            ProcessActions(ProcessStack(), (DecisionActionCell)root);
                        }
                }

                FlattenCells();
            }
        }

        private void TrimActions(TableRow row)
        {
            int count = row.Actions.Count;
            int aCount = _table.Actions.Count;

            if (count > aCount)
            {
                row.Actions.RemoveRange(aCount, count - aCount);
                if (aCount > 0)
                {
                    DecisionActionCell last = row.Actions[aCount - 1];
                    last.SetFurtherMore(null);
                }
            }
        }

        private void TrimConditions(TableRow row)
        {
            int count = row.Conditions.Count;
            int cCount = _table.Conditions.Count;

            if (count > cCount)
            {
                row.Conditions.RemoveRange(cCount, count - cCount);
                if (cCount > 0)
                {
                    DecisionConditionCell last = row.Conditions[cCount - 1];
                    last.SetOnMatch(null);

                    if (row.Actions.Count > 0)
                    {
                        last.SetOnMatch(row.Actions[0]);
                    }
                }
            }
        }

        private void CompleteMissingConditions(TableRow row)
        {
            int count = row.Conditions.Count;
            int cCount = _table.Conditions.Count;

            if (count < cCount)
            {
                int len = cCount - count;
                DecisionConditionCell last = row.Conditions[count - 1];

                for (int i = 0; i < len; i++)
                {
                    DecisionConditionCell cell = new DecisionConditionCell();
                    last.SetOnMatch(cell);

                    last = cell;
                    row.Conditions.Add(cell);
                }

                last = row.Conditions[cCount - 1];
                last.SetOnMatch(null);

                if (row.Actions.Count > 0)
                {
                    last.SetOnMatch(row.Actions[0]);
                }
            }
        }

        private void ClearTableRows()
        {
            foreach (DecisionRow row in _table.Rows)
            {
                row.SetRoot(null);
            }

            _table.Rows.Clear();
        }

        private void FlattenCells()
        {
            ClearTableRows();

            int aCount = _table.Actions.Count;
            int cCount = _table.Conditions.Count;

            foreach (TableRow row in _rows)
            {
                // Reset Else cells
                foreach (DecisionConditionCell condition in row.Conditions)
                {
                    condition.SetElse(null);
                }

                // Trim actions
                TrimActions(row);

                // Trim conditions
                TrimConditions(row);

                // Complete missing conditions
                CompleteMissingConditions(row);

                // Recreate row
                DecisionRow dRow = _table.Rows.Add(false);
                if (row.Conditions.Count > 0)
                {
                    dRow.SetRoot(row.Conditions[0]);
                }
                else if (row.Actions.Count > 0)
                {
                    dRow.SetRoot(row.Actions[0]);
                }
            }
        }
        
        # endregion

        # region Merge

        private bool EqualConditions(DecisionConditionCell cell1, DecisionConditionCell cell2)
        {
            return ReferenceEquals(cell1, cell2) ||
                (!(ReferenceEquals(cell1, null) || ReferenceEquals(cell2, null)) &&
                (cell1.Operation == cell2.Operation) && 
                object.Equals(cell1.Values, cell2.Values));
        }

        private int GetMatchingCells(TableRow currRow, TableRow preRow)
        {
            int result = -1;
            int cCount = currRow.Conditions.Count;

            for (int j = 0; j < cCount; j++)
            {
                DecisionConditionCell preCell = preRow.Conditions[j];
                DecisionConditionCell currCell = currRow.Conditions[j];

                if (!EqualConditions(currCell, preCell))
                    break;

                result = j;
            }

            return result;
        }

        private DecisionCell[,] GetCellMatrix()
        {
            int rowCount = _rows.Count;
            int condCount = _table.Conditions.Count;
            int actCount = _table.Actions.Count;

            DecisionCell[,] matrix = new DecisionCell[rowCount, condCount + actCount];

            if (rowCount > 0)
            {
                // Fill cells
                for (int i = 0; i < rowCount; i++)
                {
                    TableRow row = _rows[i];

                    for (int j = 0; j < condCount; j++)
                    {
                        matrix[i, j] = row.Conditions[j];
                    }

                    for (int j = 0; j < actCount; j++)
                    {
                        matrix[i, j + condCount] = row.Actions[j];
                    }
                }

                // Validate cells
                for (int j = 0; j < condCount; j++)
                {
                    for (int i = rowCount - 1; i > 0; i--)
                    {
                        DecisionConditionCell cell = (DecisionConditionCell)matrix[i, j];
                        DecisionConditionCell up = (DecisionConditionCell)matrix[i - 1, j];

                        if (EqualConditions(cell, up))
                        {
                            if (j == 0)
                            {
                                matrix[i, j] = null;

                                cell.SetElse(null);
                                if (j < condCount - 1)
                                {
                                    cell.SetOnMatch(null);
                                }

                                continue;
                            }

                            DecisionConditionCell left = (DecisionConditionCell)matrix[i, j - 1];
                            if (left == null)
                            {
                                matrix[i, j] = null;

                                cell.SetElse(null);
                                if (j < condCount - 1)
                                {
                                    cell.SetOnMatch(null);
                                }
                            }
                        }
                    }
                }

                // Set else cells
                for (int i = rowCount - 1; i > 0; i--)
                {
                    for (int j = 1; j < condCount; j++)
                    {
                        DecisionConditionCell cell = (DecisionConditionCell)matrix[i, j];
                        DecisionConditionCell left = (DecisionConditionCell)matrix[i, j - 1];

                        if ((left == null) && (cell != null))
                        {
                            DecisionConditionCell up = null;

                            for (int k = i - 1; k > -1; k--)
                            {
                                up = (DecisionConditionCell)matrix[k, j];
                                if (up != null)
                                    break;
                            }

                            if (up != null)
                            {
                                up.SetElse(cell);
                            }
                        }
                    }
                }
            }

            return matrix;
        }

        private void MergeConditions()
        {
            FlattenTable();

            int rowCount = _rows.Count;
            int condCount = _table.Conditions.Count;

            if ((rowCount > 0) && (condCount > 1))
            {
                DecisionCell[,] matrix = GetCellMatrix();

                ClearTableRows();

                for (int i = 0; i < rowCount; i++)
                {
                    DecisionCell cell = matrix[i, 0];

                    if (cell != null)
                    {
                        DecisionRow row = _table.Rows.Add(false);
                        row.SetRoot(cell);
                    }
                }
            }
        }

        # endregion

        # region Validation

        private void ValidateName(string name, DecisionColumnList list)
        {
            name = (name != null ? name.Trim() : String.Empty);
            if (list.Contains(name))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("NamedColumnAlreadyExists"), name));
            }
        }

        private void ValidateIndex(int index, DecisionColumnList list)
        {
            if ((index < 0) || (index > list.Count))
            {
                throw new IndexOutOfRangeException(String.Format(BreResStrings.GetString("IndexOutOfRangeForActions."),
                    index.ToString()));
            }
        }

        # endregion

        # region Add column

        private void AddActionColumn(int index, string name, string description, DecisionValueType type)
        {
            ValidateName(name, _table.Actions);
            ValidateIndex(index, _table.Actions);

            FlattenTable();

            DecisionColumn item = new DecisionColumn();

            item.SetName(name);
            item.SetDescription(description);
            item.SetType(type);

            _table.Actions.Insert(index, item);

            if (_rows.Count == 0)
                return;

            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];

                DecisionActionCell cell = new DecisionActionCell();
                row.Actions.Insert(index, cell);

                if (index > 0)
                {
                    DecisionActionCell left = row.Actions[index - 1];
                    left.SetFurtherMore(cell);
                }
                else
                    if (index == 0)
                    {
                        int count = _table.Conditions.Count;
                        if (count == 0)
                        {
                            dRow.SetRoot(cell);
                        }
                        else
                        {
                            DecisionConditionCell left = row.Conditions[count - 1];
                            left.SetOnMatch(cell);
                        }
                    }

                if (index < row.Actions.Count - 1)
                {
                    DecisionActionCell right = row.Actions[index + 1];
                    cell.SetFurtherMore(right);
                }
            }

            MergeConditions();
        }

        private void AddConditionColumn(int index, string name, string description, DecisionValueType type)
        {
            ValidateName(name, _table.Conditions);
            ValidateIndex(index, _table.Conditions);

            FlattenTable();

            DecisionColumn item = new DecisionColumn();

            item.SetName(name);
            item.SetDescription(description);
            item.SetType(type);

            _table.Conditions.Insert(index, item);

            if (_rows.Count == 0)
                return;

            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];

                DecisionConditionCell cell = new DecisionConditionCell();
                row.Conditions.Insert(index, cell);

                if (index == 0)
                {
                    dRow.SetRoot(cell);
                }

                if (index > 0)
                {
                    DecisionConditionCell left = row.Conditions[index - 1];
                    left.SetOnMatch(cell);
                }

                if (index < row.Conditions.Count - 1)
                {
                    DecisionConditionCell right = row.Conditions[index + 1];
                    cell.SetOnMatch(right);
                }
                else
                    if ((index == row.Conditions.Count - 1) && (row.Actions.Count > 0))
                    {
                        DecisionActionCell right = row.Actions[0];
                        cell.SetOnMatch(right);
                    }
            }

            MergeConditions();
        }

        # endregion

        # region Remove column

        private void RemoveActionColumn(string name)
        {
            ValidateName(name, _table.Actions);

            int index = _table.Actions.IndexOf(name);
            RemoveActionColumn(index);
        }

        private void RemoveActionColumn(int index)
        {
            ValidateIndex(index, _table.Actions);

            FlattenTable();

            _table.Actions.RemoveAt(index);

            if (_rows.Count == 0)
                return;

            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];
                DecisionActionCell cell = row.Actions[index];

                DecisionCell left = cell.Parent;
                DecisionActionCell right = cell.FurtherMore;

                cell.SetFurtherMore(null);
                row.Actions.RemoveAt(index);

                if (left is DecisionActionCell)
                {
                    ((DecisionActionCell)left).SetFurtherMore(right);
                }
                else
                    if (left is DecisionConditionCell)
                    {
                        ((DecisionConditionCell)left).SetOnMatch(right);
                    }
                    else
                        if ((index == 0) && (_table.Conditions.Count == 0))
                        {
                            dRow.SetRoot(right);
                        }
            }

            MergeConditions();
        }

        private void RemoveConditionColumn(string name)
        {
            ValidateName(name, _table.Conditions);

            int index = _table.Conditions.IndexOf(name);
            RemoveConditionColumn(index);
        }

        private void RemoveConditionColumn(int index)
        {
            ValidateIndex(index, _table.Conditions);

            FlattenTable();

            _table.Conditions.RemoveAt(index);

            if (_rows.Count == 0)
                return;

            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];
                DecisionConditionCell cell = row.Conditions[index];

                DecisionCell right = cell.OnMatch;
                DecisionConditionCell left = null;

                if (cell.Parent is DecisionConditionCell)
                {
                    left = (DecisionConditionCell)cell.Parent;
                }

                cell.SetOnMatch(null);
                row.Conditions.RemoveAt(index);

                if (index == 0)
                {
                    dRow.SetRoot(right);
                }
                else
                    if (left != null)
                    {
                        left.SetOnMatch(right);
                    }
            }

            MergeConditions();
        }

        # endregion

        # region Reorder column

        private bool CanReorderColumn(int currIndex, int newIndex, int columnCount)
        {
            return (currIndex != newIndex) && (columnCount > 1) && (currIndex > -1) && 
                (currIndex < columnCount) && (newIndex > -1) && (newIndex < columnCount);
        }

        private void ReorderActionColumn(int currIndex, int newIndex)
        {
            if (!CanReorderColumn(currIndex, newIndex, _table.Actions.Count))
                return;

            FlattenTable();

            // Swap column
            DecisionColumn column = _table.Actions[currIndex];

            _table.Actions.Insert(newIndex, column);
            if (newIndex > currIndex)
            {
                _table.Actions.RemoveAt(currIndex);
            }
            else
                if (newIndex < currIndex)
                {
                    _table.Actions.RemoveAt(currIndex + 1);
                }

            // Order cells
            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];
                DecisionActionCell cell = row.Actions[currIndex];

                // Swap cell
                row.Actions.Insert(newIndex, cell);
                if (newIndex > currIndex)
                {
                    row.Actions.RemoveAt(currIndex);
                }
                else
                    if (newIndex < currIndex)
                    {
                        row.Actions.RemoveAt(currIndex + 1);
                    }

                int condCount = row.Conditions.Count;

                // Change row root
                if (condCount == 0)
                {
                    dRow.SetRoot(row.Actions[0]);
                }
                else
                {
                    // Update last condition
                    DecisionConditionCell left = row.Conditions[condCount - 1];
                    left.SetOnMatch(row.Actions[0]);
                }

                // Update left action
                currIndex = row.Actions.IndexOf(cell);
                if (currIndex > 0)
                {
                    DecisionActionCell left = row.Actions[currIndex - 1];
                    left.SetFurtherMore(cell);
                }

                // Update right action
                if (currIndex < row.Actions.Count - 1)
                {
                    DecisionActionCell right = row.Actions[currIndex + 1];
                    cell.SetFurtherMore(right);
                }
            }

            MergeConditions();
        }

        private void ReorderConditionColumn(int currIndex, int newIndex)
        {
            if (!CanReorderColumn(currIndex, newIndex, _table.Conditions.Count))
                return;

            FlattenTable();

            // Swap column
            DecisionColumn column = _table.Conditions[currIndex];

            _table.Conditions.Insert(newIndex, column);
            if (newIndex > currIndex)
            {
                _table.Conditions.RemoveAt(currIndex);
            }
            else
                if (newIndex < currIndex)
                {
                    _table.Conditions.RemoveAt(currIndex + 1);
                }

            // Oder cells
            int i = 0;
            foreach (TableRow row in _rows)
            {
                DecisionRow dRow = _table.Rows[i++];
                DecisionConditionCell cell = row.Conditions[currIndex];

                // Swap cell
                row.Conditions.Insert(newIndex, cell);
                if (newIndex > currIndex)
                {
                    row.Conditions.RemoveAt(currIndex);
                }
                else
                    if (newIndex < currIndex)
                    {
                        row.Conditions.RemoveAt(currIndex + 1);
                    }

                // Change row root
                currIndex = row.Conditions.IndexOf(cell);

                if (currIndex == 0)
                {
                    dRow.SetRoot(cell);
                }
                else
                {
                    // Update left
                    DecisionConditionCell left = row.Conditions[currIndex - 1];
                    left.SetOnMatch(cell);
                }

                // Update right
                int count = row.Conditions.Count;
                if (currIndex < count - 1)
                {
                    DecisionCell right = row.Conditions[currIndex + 1];
                    cell.SetOnMatch(right);
                }
                else
                    if ((currIndex == count - 1) && (row.Actions.Count > 0))
                    {
                        DecisionCell right = row.Actions[0];
                        cell.SetOnMatch(right);
                    }
            }

            MergeConditions();
        }

        # endregion

        # region Static methods

        public static void Flatten(DecisionTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.FlattenTable();
            }
        }

        public static void MergeCells(DecisionTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.MergeConditions();
            }
        }

        public static void AddAction(DecisionTable table, int index, string name,
            string description, DecisionValueType type)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.AddActionColumn(index, name, description, type);
            }
        }

        public static void AddCondition(DecisionTable table, int index, string name, 
            string description, DecisionValueType type)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.AddConditionColumn(index, name, description, type);
            }
        }

        public static void RemoveAction(DecisionTable table, int index)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.RemoveActionColumn(index);
            }
        }

        public static void RemoveAction(DecisionTable table, string name)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.RemoveActionColumn(name);
            }
        }

        public static void RemoveCondition(DecisionTable table, int index)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.RemoveConditionColumn(index);
            }
        }

        public static void RemoveCondition(DecisionTable table, string name)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.RemoveConditionColumn(name);
            }
        }

        public static void ReorderAction(DecisionTable table, int currIndex, int newIndex)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.ReorderActionColumn(currIndex, newIndex);
            }
        }

        public static void ReorderCondition(DecisionTable table, int currIndex, int newIndex)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            using (DecisionTableEditor nTable = new DecisionTableEditor(table))
            {
                nTable.ReorderConditionColumn(currIndex, newIndex);
            }
        }

        # endregion
    }
}
