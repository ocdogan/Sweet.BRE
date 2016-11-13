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
    public sealed class TableEvaluator : DecisionEvaluator, IDisposable
    {
        private DecisionTable _table;
        private Dictionary<DecisionCell, int> _columnHash;

        public TableEvaluator(DecisionTable table)
        {
            _table = table;
            _columnHash = new Dictionary<DecisionCell, int>();
        }

        public DecisionTable Table
        {
            get
            {
                return _table;
            }
        }

        public void Dispose()
        {
            _table = null;
            _columnHash.Clear();
        }

        protected override bool IsValidCondition(DecisionConditionCell condition)
        {
            int colNo = GetColumnNo(condition);
            return ((colNo > -1) && (colNo < _table.Conditions.Count));
        }

        private int GetColumnNo(DecisionConditionCell condition)
        {
            if (condition != null)
            {
                if (_columnHash.ContainsKey(condition))
                {
                    return _columnHash[condition];
                }

                int result = -1;
                DecisionCell aParent = condition.Parent;

                if ((aParent != null) && _columnHash.ContainsKey(aParent))
                {
                    result = _columnHash[aParent] + 1;

                    if (aParent is DecisionConditionCell)
                    {
                        DecisionConditionCell cond = (DecisionConditionCell)aParent;
                        if (ReferenceEquals(cond.Else, condition))
                            result--;
                    }

                    _columnHash[condition] = result;

                    return result;
                }

                result = _table.GetColumnNo(condition);
                _columnHash[condition] = result;

                return result;
            }

            return -1;
        }

        private int GetColumnNo(DecisionActionCell action)
        {
            return _table.GetColumnNo(action);
        }

        protected override string GetVariable(DecisionCell cell)
        {
            if (cell is DecisionConditionCell)
            {
                int colNo = GetColumnNo((DecisionConditionCell)cell);

                if ((colNo > -1) && (colNo < _table.Conditions.Count))
                {
                    DecisionColumn column = _table.Conditions[colNo];
                    if (column != null)
                    {
                        return column.Name;
                    }
                }
            }
            else
                if (cell is DecisionActionCell)
                {
                    int colNo = GetColumnNo((DecisionActionCell)cell);

                    if ((colNo > -1) && (colNo < _table.Actions.Count))
                    {
                        DecisionColumn column = _table.Actions[colNo];
                        if (column != null)
                        {
                            return column.Name;
                        }
                    }
                }

            return null;
        }

        protected override void EvaluateAction(IEvaluationContext context, DecisionActionCell action)
        {
            if (action != null)
            {
                while (action.Parent is DecisionActionCell)
                {
                    action = (DecisionActionCell)action.Parent;
                }

                int colNo = GetColumnNo(action);
                int colCount = _table.Actions.Count;

                while ((colNo > -1) && (colNo < colCount))
                {
                    HandleAction(context, action);

                    colNo++;
                    action = action.FurtherMore;
                }
            }
        }

        public void Fire(IEvaluationContext context, DecisionActionCell action)
        {
            EvaluateAction(context, action);
        }
    }
}
