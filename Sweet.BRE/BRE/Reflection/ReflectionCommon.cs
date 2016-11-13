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
    public static class ReflectionCommon
    {
        public static MethodInfo GetMethodInfo(object obj, string member, object[] args)
        {
            if ((obj == null) || String.IsNullOrEmpty(member))
            {
                return null;
            }

            Type[] argTypes = new Type[0];
            if ((args != null) && (args.Length > 0))
            {
                argTypes = new Type[args.Length];

                int i = 0;
                foreach (object arg in args)
                {
                    argTypes[i++] = ((arg != null) ? arg.GetType() : typeof(object));
                }
            }

            BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.InvokeMethod;

            try
            {
                MethodInfo method = obj.GetType().GetMethod(member, flags, null, argTypes, null);
                if (method != null)
                {
                    return method;
                }

            }
            catch (ArgumentException) { }
            catch (AmbiguousMatchException)
            { }

            Type[] interfaces = obj.GetType().GetInterfaces();
            if (interfaces != null)
            {
                foreach (Type iType in interfaces)
                {
                    try
                    {
                        MethodInfo method = iType.GetMethod(member, flags, null, argTypes, null);
                        if (method != null)
                        {
                            return method;
                        }
                    }
                    catch (ArgumentException) { }
                    catch (AmbiguousMatchException)
                    { }
                }
            }

            if ((args != null) && (args.Length > 0))
            {
                return GetMethodInfo(obj, member, null);
            }

            return null;
        }

        public static MemberType TokenToType(MemberToken token)
        {
            switch (token)
            {
                case MemberToken.None:
                    return MemberType.Root;

                case MemberToken.Name:
                    return MemberType.Method;

                case MemberToken.Numeric:
                    return MemberType.Numeric;

                case MemberToken.Decimal:
                    return MemberType.Decimal;

                case MemberToken.Float:
                    return MemberType.Float;

                case MemberToken.StringConst:
                    return MemberType.String;

                case MemberToken.Date:
                    return MemberType.Date;

                case MemberToken.Argument:
                    return MemberType.Argument;

                case MemberToken.LeftParen:
                    return MemberType.Parenthesis;

                case MemberToken.LeftIndex:
                    return MemberType.Indexer;

                case MemberToken.Dot:
                    return MemberType.Dot;
            }

            return MemberType.Undefined;
        }

        public static MemberToken TypeToToken(MemberType type)
        {
            switch (type)
            {
                case MemberType.Method:
                    return MemberToken.Name;

                case MemberType.Numeric:
                    return MemberToken.Numeric;

                case MemberType.Decimal:
                    return MemberToken.Decimal;

                case MemberType.Float:
                    return MemberToken.Float;

                case MemberType.String:
                    return MemberToken.StringConst;

                case MemberType.Date:
                    return MemberToken.Date;

                case MemberType.Argument:
                    return MemberToken.Argument;

                case MemberType.Parenthesis:
                    return MemberToken.LeftParen;

                case MemberType.Indexer:
                    return MemberToken.LeftIndex;

                case MemberType.Dot:
                    return MemberToken.Dot;
            }

            return MemberToken.None;
        }
    }
}
