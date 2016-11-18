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
    public sealed class TryStm : ScopeStm
    {
        private FinallyStm _finally;
        private List<CatchErrorStm> _handlerList;

        public TryStm()
            : base()
        {
            _finally = new FinallyStm(this);
            _handlerList = new List<CatchErrorStm>();
        }

        public FinallyStm Finally
        {
            get
            {
                return _finally;
            }
        }

        public CatchErrorStm[] Handlers
        {
            get
            {
                return _handlerList.ToArray();
            }
        }

        internal void AddHandler(CatchErrorStm handlerE)
        {
            if ((object)handlerE != null)
            {
                _handlerList.Add(handlerE);
            }
        }

        public TryStm OnError(params ActionStm[] doActions)
        {
            return OnError(null, doActions);
        }

        public TryStm OnError(string onError, params ActionStm[] doActions)
        {
            CatchErrorStm handler = null;

            onError = (onError != null ? onError.Trim() : null);
            foreach (CatchErrorStm hnd in _handlerList)
            {
                if (String.Equals(onError, hnd.OnError))
                {
                    handler = hnd;
                    break;
                }
            }

            if (ReferenceEquals(handler, null))
            {
                handler = new CatchErrorStm(this, onError);
                AddHandler(handler);
            }

            handler.Do(doActions);

            return this;
        }

        public override void Dispose()
        {
            if (!ReferenceEquals(_finally, null))
            {
                _finally.Dispose();
                _finally = null;
            }

            for (int i = _handlerList.Count - 1; i > -1; i--)
            {
                CatchErrorStm ce = _handlerList[i];
                _handlerList.RemoveAt(i);

                ce.SetOwner(null);
                ce.Dispose();
            }

            base.Dispose();
        }

        public TryStm Do(params ActionStm[] doActions)
        {
            if (doActions != null)
            {
                foreach (ActionStm action in doActions)
                {
                    if (!ReferenceEquals(action, null))
                    {
                        base.Actions.Add(action);
                    }
                }
            }

            return this;
        }

        public TryStm FinallyDo(params ActionStm[] doActions)
        {
            if (doActions != null)
            {
                foreach (ActionStm action in doActions)
                {
                    if (!ReferenceEquals(action, null))
                    {
                        _finally.Actions.Add(action);
                    }
                }
            }

            return this;
        }

        public static TryStm As()
        {
            return new TryStm();
        }

        public static TryStm As(string onError)
        {
            TryStm result = new TryStm();
            result.OnError(onError);

            return result;
        }

        public override object Clone()
        {
            TryStm cln = TryStm.As();

            foreach (ActionStm action in base.Actions)
            {
                if (!ReferenceEquals(action, null))
                {
                    cln.Do((ActionStm)action.Clone());
                }
            }

            foreach (CatchErrorStm handler in _handlerList)
            {
                if (!ReferenceEquals(handler, null))
                {
                    cln.AddHandler((CatchErrorStm)handler.Clone());
                }
            }

            return cln;
        }

        private bool EqualHandlers(CatchErrorStm[] ceArr)
        {
            if (ceArr.Length == _handlerList.Count)
            {
                foreach (CatchErrorStm ce1 in ceArr)
                {
                    bool found = false;
                    foreach (CatchErrorStm ce2 in _handlerList)
                    {
                        if (ce2.Equals(ce1))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        return false;
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            TryStm objA = obj as TryStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) && 
                object.Equals(_finally, objA.Finally) && base.EqualActions(objA.Actions) && 
                EqualHandlers(objA.Handlers));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("{0} ", RuleConstants.TRY);
            builder.AppendLine();

            if (base.Actions.Count > 0)
            {
                builder.Append(base.ToString());
            }

            foreach (CatchErrorStm handler in _handlerList)
            {
                builder.AppendLine(handler.ToString());
            }

            if (!ReferenceEquals(_finally, null) && (_finally.Actions.Count > 0))
            {
                builder.AppendFormat("{0} ", _finally.ToString());
                builder.AppendLine();
            }

            builder.Append(RuleConstants.END + " ");

            return builder.ToString();
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            try
            {
                base.Evaluate(context);
            }
            catch (Exception e)
            {
                bool handled = false;
                HandleError(context, e, out handled);

                if (!handled)
                    throw;
            }
            finally
            {
                if (!ReferenceEquals(_finally, null))
                {
                    ((IStatement)_finally).Evaluate(context);
                }
            }

            return null;
        }

        private void HandleError(IEvaluationContext context, Exception e, out bool handled)
        {
            handled = false;

            context.PushError(e);
            try
            {
                string error = e.GetType().FullName;
                foreach (CatchErrorStm handler in _handlerList)
                {
                    string onError = handler.OnError.Value;
                    if (String.IsNullOrEmpty(onError) || CommonHelper.EqualStrings(error, onError, true))
                    {
                        handled = true;
                        ((IStatement)handler).Evaluate(context);

                        break;
                    }
                }
            }
            finally
            {
                context.PopError(e);
            }
        }
    }
}
