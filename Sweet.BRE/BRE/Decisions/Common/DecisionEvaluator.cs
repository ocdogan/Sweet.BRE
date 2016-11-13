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
    public abstract class DecisionEvaluator
    {
        protected virtual bool IsValidCondition(DecisionConditionCell condition)
        {
            return true;
        }

        protected virtual string GetVariable(DecisionCell cell)
        {
            return null;
        }

        protected virtual void HandleAction(IEvaluationContext context, DecisionActionCell action)
        {
            string variable = GetVariable(action);

            if (!String.IsNullOrEmpty(variable))
            {
                string value = action.Value;

                if (value != null)
                {
                    IEvaluationScope scope = context.GetCurrentScope();

                    if (scope != null)
                    {
                        switch (action.Type)
                        {
                            case DecisionValueType.Boolean:
                                scope.Set(variable, StmCommon.ToBoolean(value));
                                break;

                            case DecisionValueType.DateTime:
                                scope.Set(variable, StmCommon.ToDate(value));
                                break;

                            case DecisionValueType.Float:
                                scope.Set(variable, StmCommon.ToDouble(value));
                                break;

                            case DecisionValueType.String:
                                scope.Set(variable, (value != null ? value.ToString() : null));
                                break;

                            case DecisionValueType.Integer:
                                scope.Set(variable, StmCommon.ToInteger(value));
                                break;

                            case DecisionValueType.TimeSpan:
                                scope.Set(variable, StmCommon.ToTime(value));
                                break;
                        }
                    }
                }
            }
        }

        protected virtual bool HandleCondition(IEvaluationContext context, DecisionConditionCell condition)
        {
            string variable = GetVariable(condition);

            if (!String.IsNullOrEmpty(variable))
            {
                IEvaluationScope scope = context.GetCurrentScope();
                if (scope != null)
                {
                    IVariable varObj = scope.Get(variable);

                    object obj = ((varObj != null) ? varObj.Value : null);
                    string value = ((obj != null) ? obj.ToString() : null);

                    return RuleCommon.EvaluateCondition(value, condition.Operation,
                        condition.Values.ToArray(), condition.Type);
                }

                return false;
            }

            return true;
        }

        protected virtual bool EvaluateSibling(IEvaluationContext context, DecisionCell sibling)
        {
            bool result = false;

            if (sibling is DecisionActionCell)
            {
                result = true;
                EvaluateAction(context, (DecisionActionCell)sibling);
            }
            else
                if (sibling is DecisionConditionCell)
                {
                    result = Evaluate(context, (DecisionConditionCell)sibling);
                }

            return result;
        }

        protected virtual void EvaluateAction(IEvaluationContext context, DecisionActionCell action)
        {
            while (action != null)
            {
                HandleAction(context, action);
                action = action.FurtherMore;
            }
        }

        public virtual bool Evaluate(IEvaluationContext context, DecisionConditionCell condition)
        {
            if ((condition != null) && IsValidCondition(condition))
            {
                bool result = true;

                // Handle condition
                if (condition.Values.Count > 0)
                {
                    result = HandleCondition(context, condition);
                }

                // Handle sibling
                if (result)
                {
                    result = EvaluateSibling(context, condition.OnMatch);
                }
                
                if (!result)
                {
                    result = EvaluateSibling(context, condition.Else);
                }

                return result;
            }

            return false;
        }
    }
}
