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
using System.IO;
using System.Text;

namespace Sweet.BRE
{
    public interface IEvaluationContext : IDisposable
    {
        bool Canceled { get; }
        bool Halted { get; }
        
        bool StopOnError { get; set; }

        ExecutionStatus Status { get; }

        IRuleDebugger Debugger { get; }

        IStatement[] CallStack { get; }
        IEvaluationScope[] ScopeStack { get; }
        Exception[] ErrorStack { get; }

        IFactList Facts { get; }
        IVariable[] Variables { get; }
        
        IEvalLog[] Logs { get; }

        bool InBreak { get; }
        bool InReturn { get; }
        bool InContinue { get; }

        void Halt();
        void Cancel();

        void Break();
        void Return();
        void Continue();

        IEvaluationScope GetCurrentScope();

        void Evaluate();
        void Evaluate(IVariableList variables);
        void Evaluate(IFactList facts, IVariableList variables);

        object EvaluateFunction(string function, object[] args);

        void EvaluationStarted(IStatement stm, params object[] args);
        void EvaluationCompleted(IStatement stm, params object[] args);

        void IterationStarted(IStatement stm, params object[] args);
        void IterationCompleted(IStatement stm, params object[] args);

        void Log(string message);
        void Log(string message, EvalLogType type);

        void PushError(Exception e);
        void PopError(Exception e);
        Exception PopError();

        Exception GetLastError();
        void HandleError(Exception e, out bool handled);
    }
}
