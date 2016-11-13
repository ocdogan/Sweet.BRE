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
    public static class RuleEngineRuntime
    {
        private static List<IRuleDebugger> _debuggers;
        private static List<IEvaluationContext> _instances;

        private static List<Type> _funcHandlerTypes;
        private static List<FunctionInfo> _registeredFunctions;
        private static Dictionary<string, string> _funcAliases;
        private static Dictionary<string, List<FunctionInfoBucket>> _funcInformations;

        private static List<EventHandler<DebugEventArgs>> _debugEventList;
        private static List<EventHandler<FunctionEventArgs>> _evalFuncEventList;

        static RuleEngineRuntime()
        {
            _debuggers = new List<IRuleDebugger>();
            _instances = new List<IEvaluationContext>();

            _debugEventList = new List<EventHandler<DebugEventArgs>>();
            _evalFuncEventList = new List<EventHandler<FunctionEventArgs>>();

            _funcHandlerTypes = new List<Type>();
            _funcAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _funcInformations = new Dictionary<string, List<FunctionInfoBucket>>(StringComparer.OrdinalIgnoreCase);
            _registeredFunctions = new List<FunctionInfo>();

            RegisterFunctions(new IFunctionHandler[] { new ArrayFunctions(), new DateFunctions(), 
                new InternalFunctions(), new MathFunctions(), new StringFunctions() });
        }

        public static event EventHandler<DebugEventArgs> OnDebugEvaluation
        {
            add
            {
                if (value != null)
                {
                    _debugEventList.Add(value);
                }
            }
            remove
            {
                if (_debugEventList.Contains(value))
                {
                    _debugEventList.Remove(value);
                }
            }
        }

        public static event EventHandler<FunctionEventArgs> OnEvaluateFunction
        {
            add
            {
                if (value != null)
                {
                    _evalFuncEventList.Add(value);
                }
            }
            remove
            {
                if (_evalFuncEventList.Contains(value))
                {
                    _evalFuncEventList.Remove(value);
                }
            }
        }

        public static bool Debugging
        {
            get
            {
                return (_debugEventList.Count > 0) || (_debuggers.Count > 0);
            }
        }

        public static IEvaluationContext[] Instances
        {
            get
            {
                lock (_instances)
                {
                    return _instances.ToArray();
                }
            }
        }

        public static FunctionInfo[] RegisteredFunctions
        {
            get
            {
                return _registeredFunctions.ToArray();
            }
        }

        public static void RegisterDebugger(IRuleDebugger debugger)
        {
            if (debugger == null)
            {
                throw new ArgumentNullException("debugger");
            }

            if (!_debuggers.Contains(debugger))
            {
                _debuggers.Add(debugger);
            }
        }

        public static IRuleDebugger NewDebugger()
        {
            IRuleDebugger debugger = new DefaultRuleDebugger(Guid.NewGuid().ToString("N"));
            _debuggers.Add(debugger);
            return debugger;
        }

        public static IRuleDebugger NewDebugger(string name)
        {
            name = name != null ? name.Trim() : null;

            IRuleDebugger debugger = !String.IsNullOrEmpty(name) ? 
                                            new DefaultRuleDebugger(name) : 
                                            new DefaultRuleDebugger(Guid.NewGuid().ToString("N"));
            _debuggers.Add(debugger);
            return debugger;
        }

        public static void RegisterFunctionAlias(string alias, string function)
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
        }

        public static void RegisterFunctions(IFunctionHandler[] handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            foreach (IFunctionHandler handler in handlers)
            {
                RegisterFunctions(handler);
            }
        }

        private static void RegisterFunctionInfo(IFunctionHandler handler, FunctionInfo info)
        {
            List<string> nameList = new List<string>();
            
            nameList.Add(info.Name);
            nameList.AddRange(info.Aliases);

            foreach (string fn in nameList)
            {
                string function = fn;
                function = ((function != null) ? function.Trim() : null);

                if (!String.IsNullOrEmpty(function))
                {
                    List<FunctionInfoBucket> infoList = null;
                    if (_funcInformations.ContainsKey(function))
                    {
                        infoList = _funcInformations[function];
                    }
                    else
                    {
                        infoList = new List<FunctionInfoBucket>();
                        _funcInformations[function] = infoList;
                    }

                    infoList.Add(new FunctionInfoBucket(handler, info));
                }
            }
        }

        public static void RegisterFunctions(IFunctionHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            lock (_funcInformations)
            {
                if (!_funcHandlerTypes.Contains(handler.GetType()))
                {
                    _funcHandlerTypes.Add(handler.GetType());

                    FunctionInfo[] functions = handler.HandledFunctions;
                    foreach (FunctionInfo info in functions)
                    {
                        if (info != null)
                        {
                            _registeredFunctions.Add(info);
                            RegisterFunctionInfo(handler, info);
                        }
                    }
                }
            }
        }

        public static void UnregisterDebugger(IRuleDebugger debugger)
        {
            if (debugger == null)
            {
                throw new ArgumentNullException("debugger");
            }

            if (_debuggers.Contains(debugger))
            {
                _debuggers.Remove(debugger);
            }
        }

        public static void UnregisterFunctionAlias(string alias, string function)
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
        }

        private static void UnregisterFunctionInfo(IFunctionHandler handler, FunctionInfo info)
        {
            List<string> nameList = new List<string>();

            nameList.Add(info.Name);
            nameList.AddRange(info.Aliases);

            foreach (string fn in nameList)
            {
                string function = fn;
                function = ((function != null) ? function.Trim() : null);

                if (!String.IsNullOrEmpty(function) && _funcInformations.ContainsKey(function))
                {
                    List<FunctionInfoBucket> infoList = _funcInformations[function];
                    for (int i = infoList.Count - 1; i > -1; i--)
                    {
                        FunctionInfoBucket bucket = infoList[i];
                        if (handler == bucket.Handler)
                        {
                            infoList.RemoveAt(i);
                        }
                    }

                    if (infoList.Count == 0)
                    {
                        _funcInformations.Remove(function);
                    }
                }
            }
        }

        public static void UnregisterFunctions(IFunctionHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            lock (_funcInformations)
            {
                if (_funcHandlerTypes.Contains(handler.GetType()))
                {
                    _funcHandlerTypes.Remove(handler.GetType());

                    FunctionInfo[] functions = handler.HandledFunctions;

                    foreach (FunctionInfo info in functions)
                    {
                        if (info != null)
                        {
                            _registeredFunctions.Remove(info);
                            UnregisterFunctionInfo(handler, info);
                        }
                    }
                }
            }
        }

        internal static void ContextDisposed(IEvaluationContext context)
        {
            if (context != null)
            {
                lock (_instances)
                {
                    _instances.Remove(context);
                }
            }
        }

        public static IEvaluationContext Initialize(Ruleset ruleset)
        {
            if (ReferenceEquals(ruleset, null))
            {
                throw new ArgumentNullException("ruleset");
            }

            IEvaluationContext result = new EvaluationContext(ruleset);
            lock (_instances)
            {
                _instances.Add(result);
            }

            return result;
        }

        private static void DebugEvaluationByDelegates(DebugEventArgs e)
        {
            if (_debugEventList.Count > 0)
            {
                IEvaluationContext context = e.Context;

                foreach (EventHandler<DebugEventArgs> eventHandler in _debugEventList)
                {
                    if (eventHandler != null)
                    {
                        eventHandler(context, e);
                    }
                }
            }
        }

        internal static void Debug(IEvaluationContext context, IStatement stm, DebugStatus status, Exception e, params object[] args)
        {
            if ((_debuggers.Count > 0) || (_debugEventList.Count > 0))
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                DebugEventArgs debugArgs = new DebugEventArgs(context, stm);
                if ((args != null) && (args.Length > 0))
                {
                    debugArgs = new DebugEventArgs(context, stm, args);
                }

                using (debugArgs)
                {
                    debugArgs.SetError(e);
                    debugArgs.SetStatus(status);

                    if (_debuggers.Count > 0)
                    {
                        foreach (IRuleDebugger debugger in _debuggers)
                        {
                            if (debugger != null)
                            {
                                debugger.Debug(debugArgs);
                            }
                        }
                    }

                    DebugEvaluationByDelegates(debugArgs);
                }
            }
        }

        private static void EvaluateFunctionByHandlers(FunctionEventArgs e)
        {
            string function = e.Name;

            if (_funcInformations.ContainsKey(function))
            {
                e.Handled = false;
                e.Result = double.NaN;

                List<FunctionInfoBucket> bucketList = _funcInformations[function];

                if (bucketList != null)
                {
                    foreach (FunctionInfoBucket bucket in bucketList)
                    {
                        IFunctionHandler handler = bucket.Handler;

                        if (handler != null)
                        {
                            handler.Eval(e);
                            if (e.Handled)
                                break;
                        }
                    }
                }
            }
        }

        private static void EvaluateFunctionByDelegates(FunctionEventArgs e)
        {
            if (_evalFuncEventList.Count > 0)
            {
                e.Handled = false;
                e.Result = double.NaN;

                IEvaluationContext context = e.Context;

                foreach (EventHandler<FunctionEventArgs> eventHandler in _evalFuncEventList)
                {
                    if (eventHandler != null)
                    {
                        e.Handled = false;
                        e.Result = double.NaN;

                        eventHandler(context, e);
                        if (e.Handled)
                            break;
                    }
                }
            }
        }

        internal static object EvaluateFunction(IEvaluationContext context, string function, object[] args)
        {
            function = (function != null ? function.Trim() : null);
            if (String.IsNullOrEmpty(function))
            {
                return new ArgumentNullException("function");
            }

            if (_funcAliases.ContainsKey(function))
            {
                function = _funcAliases[function];
            }

            if (!_funcInformations.ContainsKey(function))
            {
                throw new RuleException(String.Format(BreResStrings.GetString("FunctionCanNotBeHandled"), function));
            }

            FunctionEventArgs eArgs = new FunctionEventArgs(context, function);
            if ((args != null) && (args.Length > 0))
            {
                eArgs = new FunctionEventArgs(context, function, args);
            }

            object result = double.NaN;
            try
            {
                EvaluateFunctionByHandlers(eArgs);
                if (!eArgs.Handled)
                {
                    EvaluateFunctionByDelegates(eArgs);
                }

                if (eArgs.Handled)
                {
                    result = eArgs.Result;
                }

                if (!eArgs.Handled)
                {
                    throw new RuleException(String.Format(BreResStrings.GetString("FunctionCanNotBeHandled"), function));
                }
            }
            finally
            {
                ((IDisposable)eArgs).Dispose();
            }

            return result;
        }
    }
}
