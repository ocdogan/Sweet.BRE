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
using System.Data;
using System.Text;

namespace Sweet.BRE
{
    public abstract class ActionContext : ActionStm
    {
        private ActionList _actions;

        protected ActionContext()
            : base()
        {
            _actions = new ActionList();
        }

        public ActionList Actions
        {
            get
            {
                return _actions;
            }
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_actions, null))
            {
                for (int i = _actions.Count-1; i > -1; i--)
                {
                    ActionStm action = _actions[i];
                    _actions.RemoveAt(i);

                    action.Dispose();
                }
            }

            base.Dispose();
        }

        protected virtual ActionContext DoAction(params ActionStm[] doActions)
        {
            if (doActions != null)
            {
                foreach (ActionStm action in doActions)
                {
                    if (!ReferenceEquals(action, null))
                    {
                        _actions.Add(action);
                    }
                }
            }
            return this;
        }

        private bool ActionExists(ActionStm action)
        {
            foreach (ActionStm a in _actions)
            {
                if (object.Equals(a, action))
                    return true;
            }

            return false;
        }

        protected bool EqualActions(ActionList actions)
        {
            if (ReferenceEquals(actions, null))
            {
                return (_actions.Count == 0);
            }

            if ((actions.Count == _actions.Count))
            {
                foreach (ActionStm a in actions)
                {
                    if (!ActionExists(a))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            ActionContext objA = obj as ActionContext;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                EqualActions(objA.Actions));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ActionStm a in _actions)
            {
                builder.Append(a.ToString());
                if (a is FunctionStm)
                    builder.Append("; ");

                builder.AppendLine();
            }

            return builder.ToString();
        }

        protected override bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.InBreak || context.Canceled || 
                context.Halted || context.InReturn);
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            if (!ReferenceEquals(_actions, null) && !ExecutionBroken(context))
            {
                try
                {
                    context.IterationStarted(this);

                    foreach (IStatement e in _actions)
                    {
                        e.Evaluate(context);
                        if (context.InContinue || ExecutionBroken(context))
                            break;
                    }
                }
                finally
                {
                    context.IterationCompleted(this);
                }
            }

            return null;
        }
    }
}
