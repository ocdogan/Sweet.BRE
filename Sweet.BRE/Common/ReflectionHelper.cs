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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Sweet.BRE
{
    public sealed class ReflectionHelper
    {
        public static MethodInfo FindMethod(Type type, string methodName, Type[] argTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (String.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullException("methodName");
            }

            BindingFlags flags = BindingFlags.InvokeMethod |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance;

            Type[] arguments = (argTypes != null ? argTypes : (new Type[0]));

            try
            {
                return type.GetMethod(methodName, flags, null, arguments, null);
            }
            catch (AmbiguousMatchException) { }
            catch (ArgumentException) 
            { }

            return null;
        }

        public static object CreateNonPublicInstance(Type type)
        {
            return CreateNonPublicInstance(type, null);
        }

        public static object CreateNonPublicInstance(Type type, object[] args)
        {
            BindingFlags flags = BindingFlags.CreateInstance | BindingFlags.NonPublic |
                BindingFlags.Public | BindingFlags.Instance;

            return Activator.CreateInstance(type, flags, null, args, null);
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            return CreateInstance(type, BindingFlags.CreateInstance, args);
        }

        public static object CreateInstance(Type type, BindingFlags flags, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            flags |= BindingFlags.CreateInstance |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance;

            if (args != null && args.Length == 0)
                args = null;

            ConstructorInfo[] constructors = type.GetConstructors(flags);
            if (constructors != null)
            {
                int paramCount;
                int argCount = args == null ? 0 : args.Length;

                object[] arguments = new object[argCount];
                if (argCount > 0)
                {
                    Array.Copy(args, arguments, argCount);
                }

                ConstructorInfo thisCtor = null;
                foreach (ConstructorInfo ctor in constructors)
                {
                    ParameterInfo[] cParams = ctor.GetParameters();
                    paramCount = cParams == null ? 0 : cParams.Length;

                    if (argCount == paramCount)
                    {
                        int index = 0;
                        if (argCount > 0)
                        {
                            object arg;
                            foreach (ParameterInfo pInfo in cParams)
                            {
                                arg = arguments[index++];
                                if (arg != null && !pInfo.ParameterType.IsAssignableFrom(arg.GetType()))
                                {
                                    index = -1;
                                    break;
                                }
                            }
                        }

                        if (index >= 0)
                        {
                            thisCtor = ctor;
                            break;
                        }
                    }
                }

                if (thisCtor != null)
                {
                    return thisCtor.Invoke(arguments);
                }
            }

            return null;
        }

        public static object InvokeMemberStatic(Type type, string memberName,
            MemberTypes memberTypes, BindingFlags flags, bool validateMethod,
            CultureInfo culture, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (String.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException("memberName");
            }

            flags |= BindingFlags.Static;
            if ((flags & BindingFlags.Instance) != 0)
            {
                flags ^= BindingFlags.Instance;
            }

            if (validateMethod)
            {
                MemberInfo[] methods = type.GetMember(memberName, memberTypes, flags);
                if (methods == null || methods.Length == 0)
                {
                    return null;
                }
            }

            if ((args != null) && (args.Length == 0))
                args = null;

            return type.InvokeMember(memberName, flags, null, null, args, culture);
        }

        public static object InvokeMember(Type type, object instance,
            string memberName, MemberTypes memberTypes, BindingFlags flags,
            bool validateMethod, CultureInfo culture, params object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (String.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException("memberName");
            }

            if (type == null)
            {
                type = instance.GetType();
            }

            flags |= BindingFlags.Instance;
            if ((flags & BindingFlags.Static) != 0)
            {
                flags ^= BindingFlags.Static;
            }

            if (validateMethod)
            {
                MemberInfo[] methods = type.GetMember(memberName, memberTypes, flags);
                if ((methods == null) || (methods.Length == 0))
                {
                    return null;
                }
            }

            if ((args != null) && (args.Length == 0))
                args = null;

            return type.InvokeMember(memberName, flags, null, instance, args, culture);
        }

        public static object InvokeMethod(object instance, string methodName,
            bool validateMethod, params object[] args)
        {
            return InvokeMethod(null, instance, methodName, validateMethod, null, args);
        }

        public static object InvokeMethod(System.Type type, object instance, string methodName,
            bool validateMethod, params object[] args)
        {
            return InvokeMethod(type, instance, methodName, validateMethod, null, args);
        }

        public static object InvokeMethod(System.Type type, object instance, string methodName,
            bool validateMethod, CultureInfo culture, params object[] args)
        {
            return InvokeMember(type, instance, methodName, MemberTypes.Method,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.InvokeMethod,
                validateMethod, culture, args);
        }

        public static object InvokeMethodStatic(Type type, string methodName,
            bool validateMethod, params object[] args)
        {
            return InvokeMethodStatic(type, methodName, validateMethod, null, args);
        }

        public static object InvokeMethodStatic(Type type, string methodName,
            bool validateMethod, CultureInfo culture, params object[] args)
        {
            return InvokeMemberStatic(type, methodName, MemberTypes.Method,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.InvokeMethod,
                validateMethod, culture, args);
        }

        public static object InvokeField(object instance, string fieldName,
            bool validateMethod, params object[] args)
        {
            return InvokeField(null, instance, fieldName, validateMethod, null, args);
        }

        public static object InvokeField(System.Type type, object instance, string fieldName,
            bool validateMethod, params object[] args)
        {
            return InvokeField(type, instance, fieldName, validateMethod, null, args);
        }

        public static object InvokeField(System.Type type, object instance, string fieldName,
            bool validateMethod, CultureInfo culture, params object[] args)
        {
            return InvokeMember(type, instance, fieldName, MemberTypes.Field,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.GetField,
                validateMethod, culture, args);
        }

        public static object InvokeFieldStatic(Type type, string fieldName,
            bool validateMethod, params object[] args)
        {
            return InvokeFieldStatic(type, fieldName, validateMethod, null, args);
        }

        public static object InvokeFieldStatic(System.Type type, string fieldName,
            bool validateMethod, CultureInfo culture, params object[] args)
        {
            return InvokeMemberStatic(type, fieldName, MemberTypes.Field,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.GetField,
                validateMethod, culture, args);
        }

        public static Attribute[] GetAttributes(MemberInfo member, bool inherit)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            Attribute[] array = new Attribute[0];
            object[] customAttributes = member.GetCustomAttributes(typeof(Attribute), inherit);

            if (customAttributes != null)
            {
                array = new Attribute[customAttributes.Length];
                customAttributes.CopyTo(array, 0);
            }

            return array;
        }

        public static string GetEnvironmentResourceString(string key)
        {
            return InvokeMethodStatic(typeof(System.Environment), "GetResourceString", false,
                new object[] { key }) as string;
        }

        public static string GetEnvironmentResourceString(string key, params object[] values)
        {
            object[] valueArgs = new object[0];
            if (values != null)
            {
                valueArgs = values;
            }

            return InvokeMethodStatic(typeof(System.Environment), "GetResourceString", false,
                new object[] { key, valueArgs }) as string;
        }

        public static string GetEnvironmentResourceStringLocal(string key)
        {
            return InvokeMethodStatic(typeof(System.Environment), "GetResourceStringLocal", false,
                new object[] { key }) as string;
        }
    }
}
