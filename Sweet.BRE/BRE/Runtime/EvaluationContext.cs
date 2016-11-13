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
using System.Globalization;
using System.Text;

namespace Sweet.BRE
{
    public sealed class EvaluationContext : IEvaluationContext, IDisposable
    {
        private bool _breakRequired = false;
        private bool _returnRequired = false;
        private bool _continueRequired = false;
        private bool _stopOnError = true;

        private ExecutionStatus _status = ExecutionStatus.Initializing;

        private IRuleDebugger _debugger;

        private Project _project;
        private ActionStm _actionE;

        private FactList _facts;
        private List<IEvalLog> _logs;

        private StackList<IStatement> _callStack;
        private StackList<Exception> _exceptionStack;
        private StackList<EvaluationScope> _scopeStack;

        internal EvaluationContext(ActionStm actionE)
            : this(actionE, null)
        {
        }

        internal EvaluationContext(ActionStm actionE, IRuleDebugger debugger)
        {
            _debugger = debugger;

            _actionE = actionE;
            if (_actionE is Ruleset)
            {
                _project = ((Ruleset)_actionE).Project;
            }

            _facts = new FactList();

            _logs = new List<IEvalLog>();

            _callStack = new StackList<IStatement>();
            _exceptionStack = new StackList<Exception>();
            
            _scopeStack = new StackList<EvaluationScope>();
            _scopeStack.Push(new EvaluationScope(null));
        }

        public void Dispose()
        {
            if (_status != ExecutionStatus.Disposed)
            {
                _status = ExecutionStatus.Disposed;

                _debugger = null;

                _actionE = null;
                _project = null;

                _facts.Clear();
                _logs.Clear();

                _callStack.Clear();
                _scopeStack.Clear();

                RuleEngineRuntime.ContextDisposed(this);
            }
        }

        IStatement[] IEvaluationContext.CallStack
        {
            get
            {
                return _callStack.ToArray();
            }
        }

        IRuleDebugger IEvaluationContext.Debugger
        {
            get
            {
                return _debugger;
            }
        }

        bool IEvaluationContext.Canceled
        {
            get
            {
                return (_status == ExecutionStatus.Canceled);
            }
        }

        Exception[] IEvaluationContext.ErrorStack
        {
            get
            {
                return _exceptionStack.ToArray();
            }
        }

        IFactList IEvaluationContext.Facts
        {
            get
            {
                return (IFactList)_facts;
            }
        }

        bool IEvaluationContext.Halted
        {
            get
            {
                return (_status == ExecutionStatus.Halted);
            }
        }

        IEvalLog[] IEvaluationContext.Logs
        {
            get
            {
                return _logs.ToArray();
            }
        }

        bool IEvaluationContext.InBreak
        {
            get
            {
                return _breakRequired;
            }
        }

        bool IEvaluationContext.InContinue
        {
            get
            {
                return _continueRequired;
            }
        }

        bool IEvaluationContext.InReturn
        {
            get
            {
                return _returnRequired;
            }
        }

        IEvaluationScope[] IEvaluationContext.ScopeStack
        {
            get
            {
                return _scopeStack.ToArray();
            }
        }

        ExecutionStatus IEvaluationContext.Status
        {
            get
            {
                return _status;
            }
        }

        bool IEvaluationContext.StopOnError
        {
            get
            {
                return _stopOnError;
            }
            set
            {
                _stopOnError = value;
            }
        }

        IVariable[] IEvaluationContext.Variables
        {
            get
            {
                if (_scopeStack.Count > 0)
                {
                    IEvaluationScope scope = _scopeStack.Peek();
                    return scope.ToArray();
                }

                return new IVariable[0];
            }
        }

        void IEvaluationContext.Break()
        {
            _breakRequired = true;
        }

        void IEvaluationContext.Continue()
        {
            _continueRequired = true;
        }

        void IEvaluationContext.Cancel()
        {
            _status = ExecutionStatus.Canceled;
        }

        void IEvaluationContext.Halt()
        {
            _status = ExecutionStatus.Halted;
            throw new HaltException();
        }

        void IEvaluationContext.Return()
        {
            _returnRequired = true;
        }

        void IEvaluationContext.EvaluationStarted(IStatement stm, params object[] args)
        {
            _callStack.Push(stm);

            if (stm is Rule)
            {
                _scopeStack.Push(new EvaluationScope(_scopeStack.Peek()));
            }
            else if (stm is BreakStm)
            {
                _breakRequired = false;
            }
            else if (stm is ContinueStm)
            {
                _continueRequired = false;
            }
            else if (stm is ReturnStm)
            {
                _returnRequired = false;
            }
            else if (stm is HaltStm)
            {
                _status = ExecutionStatus.Executing;
            }

            Debug(stm, DebugStatus.Evaluating, null, args);
        }

        private void UpdateScopes(object obj)
        {
            if (obj is Rule)
            {
                EvaluationScope scope = _scopeStack.Pop();
                scope.SetParent(null);
                scope.Dispose();
            }
        }

        private void UpdateStates(object obj)
        {
            if (obj is Rule)
            {
                _breakRequired = false;
                _continueRequired = false;
                _returnRequired = false;

                return;
            }
            
            if ((_breakRequired || _continueRequired) &&
                ((obj is IfThenStm) || (obj is ForStm) || (obj is WhileStm) ||
                (obj is RepeatUntilStm) || (obj is SwitchStm)))
            {
                _breakRequired = false;
                _continueRequired = false;
            }
        }

        private bool NeedsStateUpdate()
        {
            return (_breakRequired || _continueRequired || _returnRequired);
        }

        IEvaluationScope IEvaluationContext.GetCurrentScope()
        {
            return _scopeStack.Peek();
        }

        Exception IEvaluationContext.GetLastError()
        {
            return _exceptionStack.Peek();
        }

        void IEvaluationContext.HandleError(Exception e, out bool handled)
        {
            handled = false;

            ((IEvaluationContext)this).PushError(e);
            Debug(null, DebugStatus.Error, e, null);
        }

        void IEvaluationContext.EvaluationCompleted(IStatement stm, params object[] args)
        {
            int index = _callStack.IndexOf(stm);
            if (index > -1)
            {
                int count = _callStack.Count - index;

                if (count == 1)
                {
                    _callStack.RemoveAt(index);

                    UpdateScopes(stm);
                    if (NeedsStateUpdate())
                    {
                        UpdateStates(stm);
                    }
                }
                else if (count > 1)
                {
                    GenericList<IStatement> range = _callStack.GetRange(index, count);
                    _callStack.RemoveRange(index, count);

                    if (range != null)
                    {
                        range.Reverse();

                        foreach (IStatement obj in range)
                        {
                            UpdateScopes(obj);
                            if (NeedsStateUpdate())
                            {
                                UpdateStates(obj);
                            }
                        }
                    }
                }
            }

            Debug(stm, DebugStatus.Evaluated, null, args);
        }

        void IEvaluationContext.Evaluate()
        {
            ((IEvaluationContext)this).Evaluate((IFactList)null, (IVariableList)null);
        }

        void IEvaluationContext.Evaluate(IFactList facts)
        {
            ((IEvaluationContext)this).Evaluate(facts, null);
        }

        void IEvaluationContext.Evaluate(IVariableList variables)
        {
            ((IEvaluationContext)this).Evaluate((IFactList)null, variables);
        }

        void IEvaluationContext.Evaluate(IFactList facts, IVariableList variables)
        {
            if (_status == ExecutionStatus.Disposed)
            {
                throw new RuleException(BreResStrings.GetString("ContextIsDisposed"));
            }

            _status = ExecutionStatus.Initializing;
            try
            {
                _breakRequired = false;
                _continueRequired = false;
                _returnRequired = false;

                _facts.Copy(facts);

                EvaluationScope firstScope = _scopeStack[0];
                if (!ReferenceEquals(_project, null))
                {
                    firstScope.Copy(_project.Variables);
                }

                firstScope.Append(variables);

                _status = ExecutionStatus.Executing;

                ((IStatement)_actionE).Evaluate(this);

                if (_status == ExecutionStatus.Executing)
                    _status = ExecutionStatus.Completed;
            }
            catch (HaltException)
            {
                _status = ExecutionStatus.Halted;
            }
            catch (Exception e)
            {
                _status = ExecutionStatus.Failed;

                bool handled = false;
                ((IEvaluationContext)this).HandleError(e, out handled);

                if (!handled && _stopOnError)
                    throw;
            }
        }

        object IEvaluationContext.EvaluateFunction(string function, object[] args)
        {
            if (!ReferenceEquals(_project, null))
            {
                function = _project.ResolveFunctionAlias(function);
            }

            return RuleEngineRuntime.EvaluateFunction(this, function, args);
        }

        void IEvaluationContext.Log(string message)
        {
            _logs.Add(new EvalLog(message, EvalLogType.Info));
        }

        void IEvaluationContext.Log(string message, EvalLogType type)
        {
            _logs.Add(new EvalLog(message, type));
        }

        Exception IEvaluationContext.PopError()
        {
            return _exceptionStack.Pop();
        }

        void IEvaluationContext.PopError(Exception e)
        {
            lock (_exceptionStack)
            {
                int index = _exceptionStack.IndexOf(e);
                if (index > -1)
                {
                    int count = _exceptionStack.Count - index;
                    _exceptionStack.RemoveRange(index, count);
                }
            }
        }

        void IEvaluationContext.PushError(Exception e)
        {
            _exceptionStack.Push(e);
        }

        private void UpdateContinueState(object obj)
        {
            if (_continueRequired && ((obj is ForStm) || (obj is WhileStm) || (obj is RepeatUntilStm) || 
                (obj is IfThenStm) || (obj is SwitchStm) || (obj is Rule)))
            {
                _continueRequired = false;
            }
        }

        private void CheckIteration(IStatement stm)
        {
            if (_continueRequired)
            {
                int index = _callStack.IndexOf(stm);
                if (index > -1)
                {
                    int count = _callStack.Count - index;
                    if (count == 1)
                    {
                        UpdateContinueState(stm);
                        return;
                    }

                    GenericList<IStatement> range = _callStack.GetRange(index, count);

                    foreach (object obj in range)
                    {
                        UpdateContinueState(obj);
                        if (!_continueRequired)
                            break;
                    }
                }
            }
        }

        void IEvaluationContext.IterationStarted(IStatement stm, params object[] args)
        {
            CheckIteration(stm);
            Debug(stm, DebugStatus.Iterating, null, args);
        }

        void IEvaluationContext.IterationCompleted(IStatement stm, params object[] args)
        {
            CheckIteration(stm);
            Debug(stm, DebugStatus.Iterated, null, args);
        }

        private void Debug(IStatement stm, DebugStatus status, Exception e, params object[] args)
        {
            if (_debugger != null)
            {
                DebugEventArgs debugArgs = new DebugEventArgs(this, stm);
                if ((args != null) && (args.Length > 0))
                {
                    debugArgs = new DebugEventArgs(this, stm, args);
                }

                using (debugArgs)
                {
                    debugArgs.SetError(e);
                    debugArgs.SetStatus(status);

                    _debugger.Debug(debugArgs);
                }
            }
            RuleEngineRuntime.Debug(this, stm, status, e, args);
        }
    }
}
