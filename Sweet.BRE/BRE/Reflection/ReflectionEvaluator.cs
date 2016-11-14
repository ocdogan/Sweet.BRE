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
using System.Reflection;
using System.Text;

namespace Sweet.BRE
{
    public class ReflectionEvaluator
    {
        private object _thisObj;
        private object[] _args;

        private bool _break = false;

        private MemberNode[] _nodes;
        private Stack<Stack<object>> _instFrames;
        private Stack<Stack<MemberNode>> _memberFrames;

        private static Dictionary<string, MethodInfo> _methodCache;

        static ReflectionEvaluator()
        {
            _methodCache = new Dictionary<string, MethodInfo>();
        }

        private ReflectionEvaluator(object obj, MemberNode[] nodes, params object[] args)
        {
            _args = args;
            _thisObj = obj;
            _nodes = nodes;

            _instFrames = new Stack<Stack<object>>();
            _memberFrames = new Stack<Stack<MemberNode>>();
        }

        private Stack<object> GetInstanceFrame()
        {
            return ((_instFrames.Count > 0) ? _instFrames.Peek() : null); 
        }

        private object PeekInstance()
        {
            Stack<object> frame = GetInstanceFrame();
            return (((frame != null) && (frame.Count > 0)) ? frame.Peek() : null);
        }

        private void PushInstance(object value)
        {
            Stack<object> frame = GetInstanceFrame();
            if (frame != null)
            {
                frame.Push(value);
            }
        }

        private void BeginFrame()
        {
            _memberFrames.Push(new Stack<MemberNode>());
            _instFrames.Push(new Stack<object>());

            PushInstance(_thisObj);
        }

        private void EndFrame()
        {
            _instFrames.Pop().Clear();
            _memberFrames.Pop().Clear();
        }

        private Stack<MemberNode> GetMemberFrame()
        {
            return ((_memberFrames.Count > 0) ? _memberFrames.Peek() : null);
        }

        private MemberNode PeekMember()
        {
            Stack<MemberNode> frame = GetMemberFrame();
            return (((frame != null) && (frame.Count > 0)) ? frame.Peek() : null);
        }

        private void PushMember(MemberNode value)
        {
            Stack<MemberNode> frame = GetMemberFrame();
            if (frame != null)
            {
                frame.Push(value);
            }
        }

        private int GetArgsHashCode(object[] args)
        {
            int result = 0;
            if ((args != null) && (args.Length > 0))
            {
                foreach (object obj in args)
                {
                    int hash = ((obj != null) ? obj.GetType().GetHashCode() : 0);
                    result = (result ^ hash);
                }
            }

            return result;
        }

        private MethodInfo GetMethodInfo(object obj, string member, object[] args)
        {
            MethodInfo result = null;

            if (obj != null)
            {
                member = ((member != null) ? member.Trim() : String.Empty);

                if (!String.IsNullOrEmpty(member))
                {
                    string key = obj.GetType().ToString().ToLowerInvariant();

                    key += member;
                    key += GetArgsHashCode(args).ToString();

                    lock (_methodCache)
                    {
                        if (_methodCache.ContainsKey(key))
                        {
                            result = _methodCache[key];
                        }
                        else
                        {
                            result = ReflectionCommon.GetMethodInfo(obj, member, args);
                            _methodCache[key] = result;
                        }
                    }
                }
            }

            return result;
        }

        private object EvaluateMethod(MemberNode node)
        {
            object instance = PeekInstance();
            if (instance == null)
            {
                _break = true;
                return null;
            }

            object[] args = null;
            string member = node.Value;

            bool useArguments = false;

            MemberNode sibling = node.NextSibling;

            bool isMethod = ((sibling != null) && (sibling.Type == MemberType.Parenthesis));
            bool isIndexer = ((sibling != null) && (sibling.Type == MemberType.Indexer) &&
                (sibling.Arguments.Count > 0));

            if (isMethod)
            {
                useArguments = true;
            }
            else 
            {
                member = "get_" + node.Value;
                if (isIndexer && (node.Value == "Item"))
                {
                    useArguments = true;
                }
            }

            if (useArguments)
            {
                PushMember(sibling);

                if ((sibling != null) && (sibling.Arguments.Count > 0))
                {
                    object obj = EvaluateArgs(sibling);

                    args = new object[] { obj };
                    if ((obj != null) && obj.GetType().IsArray)
                    {
                        Array arr = (Array)obj;
                        if (arr.Length == 0)
                        {
                            args = new object[0];
                        }
                        else
                        {
                            object[] objArr = new object[arr.Length];
                            Array.Copy(arr, objArr, arr.Length);

                            args = objArr;
                        }
                    }
                }
            }

            object result = null;

            MethodInfo method = GetMethodInfo(instance, member, args);
            if (method != null)
            {
                BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public |
                   BindingFlags.NonPublic | BindingFlags.Instance |
                   BindingFlags.InvokeMethod;

                result = method.Invoke(instance, flags, null, args, null);
            }

            return result;
        }

        private object[] EvaluateArgs(MemberNode node)
        {
            object[] args = new object[node.Arguments.Count];

            BeginFrame();
            try
            {
                int i = 0;
                foreach (MemberNode arg in node.Arguments)
                {
                    args[i++] = Evaluate(arg);
                    if (_break)
                    {
                        args = null;
                        break;
                    }
                }
            }
            finally
            {
                EndFrame();
            }

            return args;
        }

        private object EvaluateArgument(MemberNode node)
        {
            int index = int.Parse(node.Value);
            if ((index > -1) && (index < _args.Length))
            {
                return _args[index];
            }

            return null;
        }

        private object EvaluateIndex(MemberNode node)
        {
            object instance = PeekInstance();
            if (instance == null)
            {
                _break = true;
                return null;
            }

            object result = null;
            object[] args = EvaluateArgs(node);

            if ((args != null) && (args.Length > 0))
            {
                Array array = instance as Array;
                if (array != null)
                {
                    int index = (int)StmCommon.ToInteger(args[0], -1);
                    if ((index > -1) && (index < array.Length))
                    {
                        result = array.GetValue(index);
                    }
                }
                else
                {
                    object key = args[0];
                    if (RuleCommon.IsNumber(key))
                    {
                        args[0] = (int)StmCommon.ToInteger(key, -1);
                    }
                    else
                    {
                        args[0] = StmCommon.ToString(key);
                    }

                    string member = "get_Item";

                    MethodInfo method = GetMethodInfo(instance, member, args);
                    if (method != null)
                    {
                        BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public |
                           BindingFlags.NonPublic | BindingFlags.Instance |
                           BindingFlags.InvokeMethod;

                        result = method.Invoke(instance, flags, null, args, null);
                    }
                }
            }

            return result;
        }

        private object Evaluate(MemberNode node)
        {
            if (_break)
            {
                return null;
            }

            PushMember(node);

            bool pushInst = true;
            object result = null;

            switch (node.Type)
            {
                case MemberType.Decimal:
                    result = decimal.Parse(node.Value);
                    break;

                case MemberType.Date:
                    result = DateTime.Parse(node.Value);
                    break;

                case MemberType.Float:
                    result = float.Parse(node.Value);
                    break;

                case MemberType.Numeric:
                    result = int.Parse(node.Value);
                    break;

                case MemberType.String:
                    result = node.Value;
                    break;

                case MemberType.Argument:
                    result = EvaluateArgument(node);
                    break;

                case MemberType.Method:
                    result = EvaluateMethod(node);
                    break;

                case MemberType.Parenthesis:
                    result = EvaluateArgs(node);
                    break;

                case MemberType.Indexer:
                    result = EvaluateIndex(node);
                    break;

                default:
                    pushInst = false;
                    break;
            }

            if (pushInst)
            {
                PushInstance(result);
            }

            node = PeekMember();
            if (node != null)
            {
                MemberNode sibling = node.NextSibling;
                if (sibling != null)
                {
                    result = Evaluate(sibling);
                }
            }

            return result;
        }

        private void StartEvaluate()
        {
            _instFrames.Clear();
            _memberFrames.Clear();
        }

        private object Evaluate()
        {
            object result = _thisObj;

            if ((_nodes != null) && (_nodes.Length > 0))
            {
                StartEvaluate();

                object[] arr = new object[_nodes.Length];

                int i = 0;
                foreach (MemberNode node in _nodes)
                {
                    BeginFrame();
                    try
                    {
                        arr[i++] = Evaluate(_nodes[0]);
                    }
                    finally
                    {
                        EndFrame();
                    }
                }

                return ((arr.Length == 1) ? arr[0] : arr);
            }

            return result;
        }

        public static object Evaluate(object obj, string path)
        {
            return Evaluate(obj, path, null);
        }

        public static object Evaluate(object obj, string path, object[] args)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            path = (path != null ? path.Trim() : null);
            if (String.IsNullOrEmpty(path))
            {
                return obj;
            }

            args = ((args != null) ? args : (new object[0]));

            return (new ReflectionEvaluator(obj, ReflectionParser.Parse(path), args)).Evaluate();
        }
    }
}
