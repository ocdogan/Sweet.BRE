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
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Sweet.BRE
{
    public static class CommonHelper
    {
        private class EnumCache
        {
            public object Default;
            public Dictionary<string, object> Values;
        }

        private static string _basePath = null;

        private static CultureInfo enCulture = new CultureInfo("en");
        private static CultureInfo enUsCulture = new CultureInfo("en-us");

        private static Dictionary<string, string> _config;

        private const int MaxMessageLen = 9900;
        private const string DefaultEventlogSource = "Unified Technology Platform";

        private static object _syncObject;
        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private static Dictionary<Type, EnumCache> _enumCache = new Dictionary<Type, EnumCache>();

        public static int SignedHiword(int n)
        {
            return (short)((n >> 0x10) & 0xffff);
        }

        public static int SignedLoword(int n)
        {
            return (short)(n & 0xffff);
        }

        public static bool Failed(int hr)
        {
            return (hr < 0);
        }

        public static bool Succeeded(int hr)
        {
            return (hr >= 0);
        }

        public static int ThrowOnFailure(int hr)
        {
            return ThrowOnFailure(hr, null);
        }

        public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
        {
            if (Failed(hr) && ((expectedHRFailure == null) || (Array.IndexOf<int>(expectedHRFailure, hr) < 0)))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return hr;
        }

        public static bool IsCriticalException(Exception e)
        {
            return (e is NullReferenceException) || (e is StackOverflowException) ||
                (e is OutOfMemoryException) || (e is ThreadAbortException) || (e is ExecutionEngineException) ||
                (e is IndexOutOfRangeException) || (e is AccessViolationException);
        }

        public static string ComposeContentType(string contentType, Encoding encoding, string action)
        {
            if ((encoding == null) && (action == null))
            {
                return contentType;
            }

            StringBuilder builder = new StringBuilder(contentType);
            if (encoding != null)
            {
                builder.Append("; charset=");
                builder.Append(encoding.WebName);
            }

            if (action != null)
            {
                builder.Append("; action=\"");
                builder.Append(action);
                builder.Append("\"");
            }

            return builder.ToString();
        }

        public static MemberInfo FindMatchingMember(string name, Type ownerType, bool ignoreCase)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy 
                                    | BindingFlags.NonPublic 
                                    | BindingFlags.Public 
                                    | BindingFlags.Static 
                                    | BindingFlags.Instance;

            foreach (MemberInfo info2 in ownerType.GetMembers(flags))
            {
                if (info2.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    return info2;
                }
            }

            return null;
        }

        public static string GetClassName(string fullQualifiedName)
        {
            if (fullQualifiedName == null)
            {
                return null;
            }
            string text = fullQualifiedName;
            int num = fullQualifiedName.LastIndexOf('.');
            if (num != -1)
            {
                text = fullQualifiedName.Substring(num + 1);
            }
            return text;
        }

        public static Type GetMemberType(MemberInfo memberInfo)
        {
            FieldInfo info = memberInfo as FieldInfo;
            if (info != null)
            {
                return info.FieldType;
            }

            PropertyInfo info2 = memberInfo as PropertyInfo;
            if (info2 != null)
            {
                if (info2.PropertyType != null)
                {
                    return info2.PropertyType;
                }
                return info2.GetGetMethod().ReturnType;
            }

            EventInfo info4 = memberInfo as EventInfo;
            if (info4 != null)
            {
                return info4.EventHandlerType;
            }

            return null;
        }

        public static bool IsTypePrimitive(Type type)
        {
            if (!type.IsPrimitive && !type.IsEnum && (type != typeof(Guid)) &&
                (type != typeof(IntPtr)) && (type != typeof(string)) && (type != typeof(DateTime)))
            {
                return (type == typeof(TimeSpan));
            }

            return true;
        }

        public static void GetNamespaceAndClassName(string fullQualifiedName, out string namespaceName, out string className)
        {
            namespaceName = String.Empty;
            className = String.Empty;

            if (fullQualifiedName != null)
            {
                int length = fullQualifiedName.LastIndexOf('.');
                if (length != -1)
                {
                    namespaceName = fullQualifiedName.Substring(0, length);
                    className = fullQualifiedName.Substring(length + 1);
                }
                else
                {
                    className = fullQualifiedName;
                }
            }
        }

        public static PropertyDescriptor GetRealPropertyDescriptor(PropertyDescriptor descriptor)
        {
            PropertyDescriptor result = descriptor;
            if (descriptor != null)
            {
                object obj = ReflectionHelper.InvokeMethod(descriptor, "get_RealPropertyDescriptor", true);
                if (obj is PropertyDescriptor)
                {
                    result = (PropertyDescriptor)obj;
                }
            }

            return result;
        }

        public static bool IsEventPropertyDescriptor(PropertyDescriptor descriptor)
        {
            if (descriptor != null)
            {
                System.Type type = descriptor.GetType();
                while (type != null)
                {
                    if (String.Equals(type.FullName, "System.ComponentModel.PropertyDescriptor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (String.Equals(type.Name, "EventPropertyDescriptor",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    type = type.BaseType;
                }

                PropertyDescriptor realDescriptor = GetRealPropertyDescriptor(descriptor);
                if ((realDescriptor != null) && (realDescriptor != descriptor))
                {
                    return IsEventPropertyDescriptor(realDescriptor);
                }
            }

            return false;
        }

        public static System.Type ConvertToType(string propTypeName)
        {
            propTypeName = TrimString(propTypeName);
            if (String.IsNullOrEmpty(propTypeName))
            {
                return null;
            }

            propTypeName = ToLowerEng(propTypeName);

            switch (propTypeName)
            {
                case "string":
                case "system.string":
                    return typeof(String);
                case "byte":
                case "system.byte":
                    return typeof(byte);
                case "int16":
                case "system.int16":
                    return typeof(Int16);
                case "short":
                    return typeof(short);
                case "bool":
                case "boolean":
                case "system.boolean":
                    return typeof(bool);
                case "datetime":
                case "system.datetime":
                    return typeof(DateTime);
                case "decimal":
                case "system.decimal":
                    return typeof(decimal);
                case "double":
                case "system.double":
                    return typeof(double);
                case "float":
                case "single":
                case "system.single":
                    return typeof(float);
                case "int32":
                case "system.int32":
                    return typeof(Int32);
                case "int":
                case "integer":
                    return typeof(int);
                case "int64":
                case "system.int64":
                    return typeof(Int64);
                case "long":
                    return typeof(long);
                case "dataset":
                case "system.data.dataset":
                    return typeof(System.Data.DataSet);
                case "xmldocument":
                case "system.xml.xmldocument":
                    return typeof(System.Xml.XmlDocument);
            }

            return null;
        }

        public static MethodInfo GetMethodExactMatch(Type type, string name, BindingFlags bindingAttr, Type[] types)
        {
            foreach (MethodInfo info2 in type.GetMethods(bindingAttr))
            {
                if (((bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.IgnoreCase) ?
                    (String.Compare(info2.Name, name, StringComparison.OrdinalIgnoreCase) == 0) :
                    (String.Compare(info2.Name, name, StringComparison.Ordinal) == 0))
                {
                    bool flag2 = false;
                    if (types != null)
                    {
                        ParameterInfo[] parameters = info2.GetParameters();
                        if (parameters.GetLength(0) == types.Length)
                        {
                            for (int i = 0; !flag2 && (i < parameters.Length); i++)
                            {
                                flag2 = (parameters[i].ParameterType == null) ||
                                    !parameters[i].ParameterType.IsAssignableFrom(types[i]);
                            }
                        }
                        else
                        {
                            flag2 = true;
                        }
                    }

                    if (!flag2)
                    {
                        return info2;
                    }
                }
            }

            return null;
        }

        public static XmlWriter CreateXmlWriter(object output)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.UTF8;

            if (output is string)
            {
                return XmlWriter.Create(output as string, settings);
            }
            else if (output is StringBuilder)
            {
                return XmlWriter.Create(output as StringBuilder, settings);
            }
            else if (output is TextWriter)
            {
                settings.CloseOutput = true;
                return XmlWriter.Create(output as TextWriter, settings);
            }
            else if (output is Stream)
            {
                settings.CloseOutput = !(output is MemoryStream);
                return XmlWriter.Create(output as Stream, settings);
            }
            else if (output is XmlWriter)
            {
                return XmlWriter.Create(output as XmlWriter, settings);
            }

            return null;
        }

        public static XmlReader CreateXmlReader(object input)
        {
            if (input is string)
            {
                return XmlReader.Create(new StringReader(input as String));
            }
            else if (input is StringBuilder)
            {
                return XmlReader.Create(new StringReader((input as StringBuilder).ToString()));
            }
            else if (input is TextReader)
            {
                return XmlReader.Create(input as TextReader);
            }
            else if (input is Stream)
            {
                return XmlReader.Create(input as Stream);
            }
            else if (input is XmlDocument)
            {
                return XmlReader.Create(new StringReader((input as XmlDocument).OuterXml));
            }
            else if (input is XmlReader)
            {
                return (input as XmlReader);
            }
            else if (input != null)
            {
                return XmlReader.Create(new StringReader(input.ToString()));
            }

            return null;
        }

        public static Type GetType(string typeName)
        {
            return GetType(typeName, false, false);
        }

        public static Type GetType(string typeName, bool throwOnError)
        {
            return GetType(typeName, throwOnError, false);
        }

        public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            Type result = Type.GetType(typeName, throwOnError, ignoreCase);

            if (result == null)
            {
                if (_typeCache.ContainsKey(typeName))
                {
                    result = _typeCache[typeName];
                }

                if (result == null)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly asm in assemblies)
                    {
                        result = asm.GetType(typeName, false, ignoreCase);
                        if (result != null)
                        {
                            _typeCache[typeName] = result;
                            break;
                        }
                    }
                }
            }

            if ((result == null) && throwOnError)
            {
                throw new TypeLoadException(ReflectionHelper.GetEnvironmentResourceString("Arg_TypeLoadNullStr"));
            }

            return result;
        }

        public static string TrimString(string value)
        {
            return (String.IsNullOrEmpty(value) ? String.Empty : value.Trim());
        }

        public static string TrimStart(string value)
        {
            return (String.IsNullOrEmpty(value) ? String.Empty : value.TrimStart());
        }

        public static string TrimEnd(string value)
        {
            return (String.IsNullOrEmpty(value) ? String.Empty : value.TrimEnd());
        }

        public static string ToLowerEng(string value)
        {
            return (String.IsNullOrEmpty(value) ? value : value.ToLower(enCulture));
        }

        public static string ToUpperEng(string value)
        {
            return (String.IsNullOrEmpty(value) ? value : value.ToUpper(enCulture));
        }

        public static string Capitalize(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = ToLowerEng(value.TrimStart());
                if (value.Length > 0)
                {
                    string firstChar = ToUpperEng(value[0].ToString());
                    value = firstChar + value.Substring(1);
                }
            }

            return value;
        }

        public static string AddHttpPrefix(string url)
        {
            url = TrimString(url);
            if (!String.IsNullOrEmpty(url) && !url.ToLower().StartsWith("http://"))
            {
                url = "http://" + url;
            }

            return url;
        }

        public static string AddUsdlExtension(string url)
        {
            url = TrimString(url);
            if (!String.IsNullOrEmpty(url) && url.ToLower().StartsWith("http://") &&
                !url.ToLower().EndsWith("?usdl"))
            {
                url += "?usdl";
            }

            return url;
        }

        public static string RemoveUsdlExtension(string url)
        {
            url = TrimString(url);
            if (!String.IsNullOrEmpty(url) && url.ToLower().EndsWith("?usdl"))
            {
                url = url.Remove(url.Length - ("?usdl").Length);
            }

            return url;
        }

        public static byte[] BinarySerialize(Object obj)
        {
            byte[] serializedObject = new byte[0];
            MemoryStream ms = new MemoryStream();
            try
            {
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(ms, obj);

                ms.Seek(0, 0);
                serializedObject = ms.ToArray();
            }
            finally
            {
                ms.Close();
            }

            return serializedObject;
        }

        public static object BinaryDeserialize(byte[] serializedObject)
        {
            Object obj = null;
            MemoryStream ms = new MemoryStream();
            try
            {
                ms.Write(serializedObject, 0, serializedObject.Length);
                ms.Seek(0, 0);

                BinaryFormatter b = new BinaryFormatter();
                obj = b.Deserialize(ms);
            }
            finally
            {
                ms.Close();
            }

            return obj;
        }

        public static T BinaryDeserialize<T>(byte[] serializedObject)
        {
            return (T)BinaryDeserialize(serializedObject);
        }

        public static string SerializeToXml(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            XmlSerializer xsr = new XmlSerializer(obj.GetType());
            xsr.Serialize(sw, obj);

            return sb.ToString();
        }

        public static string SerializeObject(object obj, out string typeName, out string serialization)
        {
            typeName = null;
            serialization = "Null";

            if (obj != null)
            {
                Type objType = obj.GetType();
                typeName = objType.FullName;

                serialization = "Simple";

                if (obj is System.Type)
                {
                    return ((System.Type)obj).FullName;
                }

                if (obj is XmlDocument)
                {
                    return ((XmlDocument)obj).OuterXml;
                }

                if (IsTypePrimitive(objType))
                {
                    return ObjectToString(obj);
                }

                serialization = "Xml";

                try
                {
                    return SerializeToXml(obj);
                }
                catch (InvalidOperationException)
                {
                }

                serialization = "Binary";

                byte[] ba = BinarySerialize(obj);
                return BinHexEncoder.Encode(ba);
            }

            return null;
        }

        public static string ObjectToString(object obj)
        {
            string result = null;

            if (obj != null)
            {
                result = obj.ToString();
                Type objType = obj.GetType();

                if (IsTypePrimitive(objType))
                {
                    CultureInfo ci = new CultureInfo("en-us");

                    if (obj is DateTime)
                    {
                        DateTime utcDate = ((DateTime)obj).ToUniversalTime();
                        result = utcDate.ToString("u");
                    }
                    else if (objType == typeof(Single))
                    {
                        result = ((Single)obj).ToString("G", ci);
                    }
                    else if (objType == typeof(Double))
                    {
                        result = ((Double)obj).ToString("G", ci);
                    }
                    else if (objType == typeof(Decimal))
                    {
                        result = ((Decimal)obj).ToString("G", ci);
                    }
                }
            }

            return result;
        }

        private static object DeserializeBinaryObject(string data)
        {
            byte[] buffer = BinHexDecoder.Decode(data.ToCharArray());

            MemoryStream stream = new MemoryStream();
            stream.Write(buffer, 0, buffer.Length);

            stream.Flush();
            if (stream.Length > 0)
            {
                try
                {
                    return BinaryDeserialize(stream.ToArray());
                }
                catch (SerializationException)
                { }
            }

            return null;
        }

        private static object DeserializeSimpleObject(string data, Type objType)
        {
            if (typeof(System.Type).IsAssignableFrom(objType))
            {
                return GetType(data);
            }

            if (typeof(XmlDocument).IsAssignableFrom(objType))
            {
                ConstructorInfo[] constructors = objType.GetConstructors(BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.Public);

                object value = null;
                if (constructors != null)
                {
                    foreach (ConstructorInfo cInfo in constructors)
                    {
                        ParameterInfo[] parameters = cInfo.GetParameters();
                        if ((parameters == null) || (parameters.Length == 0))
                        {
                            value = cInfo.Invoke(new object[0]);
                            break;
                        }
                    }
                }

                if (value == null)
                {
                    value = new XmlDocument();
                }

                (value as XmlDocument).LoadXml(data);

                return value;
            }

            if (objType == typeof(Single))
            {
                return Single.Parse(data, enUsCulture);
            }

            if (objType == typeof(Double))
            {
                return Double.Parse(data, enUsCulture);
            }

            if (objType == typeof(Decimal))
            {
                return Decimal.Parse(data, enUsCulture);
            }

            object obj = ConvertToType(data, objType);
            if (typeof(DateTime).IsAssignableFrom(objType))
            {
                obj = DateTime.SpecifyKind((DateTime)obj, DateTimeKind.Local);
            }

            return obj;
        }

        public static object DeserializeObject(string data, string typeName, string serialization)
        {
            serialization = ToLowerEng(TrimString(serialization));
            if (serialization == "null")
            {
                return null;
            }

            data = TrimString(data);
            typeName = TrimString(typeName);

            if (String.IsNullOrEmpty(data) || String.IsNullOrEmpty(typeName))
            {
                return null;
            }

            Type objType = GetType(typeName);

            if (objType == null)
            {
                return null;
            }

            if (serialization == "simple")
            {
                return DeserializeSimpleObject(data, objType);
            }

            if (serialization == "xml")
            {
                StringReader sr = new StringReader(data);

                XmlSerializer xsr = new XmlSerializer(objType);
                return xsr.Deserialize(sr);
            }

            if (serialization == "binary")
            {
                return DeserializeBinaryObject(data);
            }

            return null;
        }

        public static object ConvertToType(object value, System.Type conversionType)
        {
            return ConvertToType(value, conversionType, false, true);
        }

        public static object ConvertToType(object value, System.Type conversionType,
            bool throwOnError)
        {
            return ConvertToType(value, conversionType, false, throwOnError);
        }

        public static object ConvertToType(object value, Type conversionType, bool forceDeserialization,
            bool throwOnError)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType = value.GetType();

            if (conversionType.IsAssignableFrom(valueType))
            {
                return value;
            }

            object result = null;
            try
            {
                result = Convert.ChangeType(value, conversionType);
            }
            catch (InvalidCastException)
            {
                try
                {
                    if (forceDeserialization && (value is String))
                    {
                        StringReader sr = new StringReader(value as string);
                        XmlSerializer xs = new XmlSerializer(conversionType);

                        value = xs.Deserialize(sr);
                        return Convert.ChangeType(value, conversionType);
                    }

                    if (forceDeserialization && conversionType.IsSerializable && (value is byte[]))
                    {
                        value = BinaryDeserialize((byte[])value);
                        return Convert.ChangeType(value, conversionType);
                    }

                    if (typeof(String).IsAssignableFrom(conversionType))
                    {
                        if (!IsTypePrimitive(value.GetType()))
                        {
                            return SerializeToXml(value);
                        }

                        string typeName = null;
                        string serialization = "null";

                        return SerializeObject(value, out typeName, out serialization);
                    }
                }
                catch (Exception)
                { }

                result = null;
                if (throwOnError)
                    throw;
            }

            return result;
        }

        public static T ConvertToType<T>(object value, bool throwOnError)
        {
            return (T)ConvertToType(value, typeof(T), false, throwOnError);
        }

        public static T GetDefaultValueOfType<T>()
        {
            return (T)GetDefaultValueOfType(typeof(T));
        }

        public static object GetDefaultValueOfType(Type type)
        {
            if ((type != null) && IsTypePrimitive(type))
            {
                if (typeof(DateTime).IsAssignableFrom(type))
                    return DateTime.MinValue;

                if (typeof(TimeSpan).IsAssignableFrom(type))
                    return TimeSpan.MinValue;

                if (typeof(bool).IsAssignableFrom(type))
                    return false;

                if (typeof(decimal).IsAssignableFrom(type))
                    return (decimal)0;

                if (typeof(double).IsAssignableFrom(type))
                    return (double)0;

                if (typeof(float).IsAssignableFrom(type))
                    return (float)0;

                if (typeof(byte).IsAssignableFrom(type))
                    return (byte)0;

                if (typeof(short).IsAssignableFrom(type))
                    return (short)0;

                if (typeof(int).IsAssignableFrom(type))
                    return (int)0;

                if (typeof(long).IsAssignableFrom(type))
                    return (long)0;

                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Instance);

                ParameterInfo[] parameters;
                foreach (ConstructorInfo info in constructors)
                {
                    parameters = info.GetParameters();
                    if ((parameters == null) || (parameters.Length == 0))
                    {
                        return info.Invoke(new object[0]);
                    }
                }
            }

            return null;
        }

        public static void ThrowException(string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            throw new Exception(message);
        }

        public static void ThrowException(string message, Type exceptionType)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            if (exceptionType == null || !typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new Exception(message);
            }

            ConstructorInfo defConstructor = null;
            ConstructorInfo[] constructors = exceptionType.GetConstructors(BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);

            foreach (ConstructorInfo constructor in constructors)
            {
                if (constructor != null)
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    if ((parameters != null) && (parameters.Length == 1) &&
                        (parameters[0].ParameterType == typeof(string)) &&
                        (parameters[0].Name == "message"))
                    {
                        defConstructor = constructor;
                        break;
                    }
                }
            }

            if (defConstructor == null)
            {
                throw new MissingMethodException(exceptionType.FullName,
                    ".ctor(string message)");
            }

            throw (Exception)defConstructor.Invoke(new object[] { message });
        }

        public static string AppendDirectorySeparator(string path)
        {
            string result = TrimString(path);

            if (!String.IsNullOrEmpty(result))
            {
                if (result.IndexOf("://") > -1)
                {
                    if (result.EndsWith(@"\"))
                        result = result.Remove(result.Length - 1, 1);

                    if (!result.EndsWith("/"))
                        result += "/";
                }
                else if (result.IndexOf(@":\\") > -1)
                {
                    if (result.EndsWith("/"))
                        result = result.Remove(result.Length - 1, 1);

                    if (!result.EndsWith(@"\"))
                        result += @"\";
                }
                else
                {
                    int index1 = result.IndexOf(@"\");
                    int index2 = result.IndexOf(@"/");

                    if ((index1 > -1) || (index2 > -1))
                    {
                        string dirSep = System.IO.Path.DirectorySeparatorChar.ToString();
                        if ((dirSep != "/") && (index2 > -1))
                        {
                            result = result.Replace("/", dirSep);
                        }

                        if (!result.EndsWith(dirSep))
                        {
                            result += dirSep;
                        }
                    }
                }
            }

            return result;
        }

        public static bool IsLocalPath(string path)
        {
            path = TrimString(path);
            if (!String.IsNullOrEmpty(path))
            {
                int volSepPos = path.IndexOf(System.IO.Path.VolumeSeparatorChar);
                if ((volSepPos == -1) || (path.IndexOf("://") == -1))
                    return true;

                string s = TrimString(path.Substring(0, volSepPos));
                if (String.IsNullOrEmpty(s))
                    return false;

                return (s.Length == 1);
            }

            return true;
        }

        public static bool IsFloating(System.Type objType)
        {
            return (objType != null) &&
                (
                (objType == typeof(Single)) ||
                (objType == typeof(Decimal)) ||
                (objType == typeof(Double))
                );
        }

        public static bool IsNumber(string value)
        {
            value = TrimString(value);

            if (!String.IsNullOrEmpty(value))
            {
                int sepCnt = 0;
                foreach (char c in value.ToCharArray())
                {
                    if (!char.IsNumber(c))
                    {
                        if ((sepCnt == 0) && (c == '.'))
                        {
                            sepCnt++;
                            continue;
                        }

                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool IsNumeric(System.Type objType)
        {
            return (objType != null) &&
                (
                (objType == typeof(Byte)) ||
                (objType == typeof(SByte)) ||
                (objType == typeof(Single)) ||
                (objType == typeof(Int16)) ||
                (objType == typeof(Int32)) ||
                (objType == typeof(Int64)) ||
                (objType == typeof(Decimal)) ||
                (objType == typeof(Double)) ||
                (objType == typeof(UInt16)) ||
                (objType == typeof(UInt32)) ||
                (objType == typeof(UInt64))
                );
        }

        public static bool HasWhiteSpace(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                char[] cArr = text.ToCharArray();
                foreach (char ch in cArr)
                {
                    if (Char.IsWhiteSpace(ch))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetConfiguration(string name)
        {
            return GetConfiguration(name, String.Empty);
        }

        public static string GetConfiguration(string name, string defaultValue)
        {
            lock (InternalSyncObject)
            {
                if (_config == null)
                {
                    string configName;

                    Dictionary<string, string> pairList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    string[] keys = ConfigurationManager.AppSettings.AllKeys;

                    foreach (string key in keys)
                    {
                        configName = TrimString(key);
                        if (!String.IsNullOrEmpty(configName))
                        {
                            pairList[configName] = ConfigurationManager.AppSettings[key];
                        }
                    }

                    _config = pairList;
                }
            }

            name = TrimString(name);
            if (!String.IsNullOrEmpty(name) && _config.ContainsKey(name))
            {
                string result = TrimString(_config[name]);
                if (result == null)
                {
                    result = String.Empty;
                }

                return result;
            }

            return defaultValue;
        }

        public static bool EqualStrings(string s1, string s2, bool ignoreCase)
        {
            StringComparison comparison = StringComparison.CurrentCulture;
            if (ignoreCase)
            {
                comparison = StringComparison.OrdinalIgnoreCase;
            }

            return String.Equals(s1, s2, comparison);
        }

        public static void EventLogService(String log)
        {
            EventLog elog = new EventLog();
            elog.Log = "Application";
            elog.Source = DefaultEventlogSource;
            elog.WriteEntry(log);
            elog.Close();
        }

        private static object InternalSyncObject
        {
            get
            {
                if (_syncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref _syncObject, obj2, null);
                }

                return _syncObject;
            }
        }

        public static bool TryParse(Type enumType, string str, out object value)
        {
            value = null;
            if ((enumType != null) && enumType.IsEnum)
            {
                EnumCache enums;
                if (!_enumCache.TryGetValue(enumType, out enums))
                {
                    enums = new EnumCache();
                    enums.Values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                    Array values = Enum.GetValues(enumType);
                    if ((values != null) && (values.Length > 0))
                    {
                        enums.Default = values.GetValue(0);

                        string[] names = Enum.GetNames(enumType);

                        int len = values.Length;
                        for (int i = 0; i < len; i++)
                        {
                            enums.Values.Add(names[i], values.GetValue(i));
                        }
                    }

                    _enumCache[enumType] = enums;
                }

                value = enums.Default;

                str = ((str != null) ? str.Trim() : str);
                if (!String.IsNullOrEmpty(str))
                {
                    object enumVal;
                    if (enums.Values.TryGetValue(str, out enumVal))
                    {
                        value = enumVal;
                        return true;
                    }
                }
            }

            return false;
        }

        public static string ReadCharacterSet(string contentType)
        {
            string characterSet = String.Empty;

            if (!String.IsNullOrEmpty(contentType))
            {
                string lconType = contentType.ToLower(CultureInfo.InvariantCulture);
                if (lconType.Trim().StartsWith("text/"))
                {
                    characterSet = "ISO-8859-1";
                }

                int pos = lconType.IndexOf(";");
                if (pos > 0)
                {
                    while ((pos = lconType.IndexOf("charset", pos)) >= 0)
                    {
                        pos += 7;
                        if ((lconType[pos - 8] == ';') || (lconType[pos - 8] == ' '))
                        {
                            while ((pos < lconType.Length) && (lconType[pos] == ' '))
                            {
                                pos++;
                            }

                            if ((pos < (lconType.Length - 1)) && (lconType[pos] == '='))
                            {
                                pos++;
                                int num2 = lconType.IndexOf(';', pos);

                                if (num2 > pos)
                                {
                                    characterSet = contentType.Substring(pos, num2 - pos).Trim();
                                }
                                else
                                {
                                    characterSet = contentType.Substring(pos).Trim();
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return (characterSet == null ? String.Empty : characterSet);
        }

        public static Encoding GetResponseEncoding(WebResponse response)
        {
            Encoding contentEnc = null;
            if (response != null)
            {
                HttpWebResponse httpResp = response as HttpWebResponse;
                if (httpResp != null)
                {
                    if (!String.IsNullOrEmpty(httpResp.ContentEncoding))
                    {
                        try
                        {
                            contentEnc = Encoding.GetEncoding(httpResp.ContentEncoding);
                        }
                        catch (ArgumentException)
                        { }
                    }
                }

                if (contentEnc == null)
                {
                    try
                    {
                        contentEnc = Encoding.GetEncoding(ReadCharacterSet(response.ContentType));
                    }
                    catch (ArgumentException)
                    { }
                }
            }

            return contentEnc;
        }

        public static string GetMD5HashBinHex(string toBeHashed)
        {
            MD5 hash = MD5.Create();
            byte[] result = hash.ComputeHash(Encoding.ASCII.GetBytes(toBeHashed));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string ApplicationBaseDirectory
        {
            get
            {
                if (_basePath == null)
                {
                    AppDomain myDomain = AppDomain.CurrentDomain;

                    string dirChar = Path.DirectorySeparatorChar.ToString();
                    string configFile = myDomain.SetupInformation.ConfigurationFile;

                    if (!String.IsNullOrEmpty(configFile))
                    {
                        string configDir = Path.GetDirectoryName(configFile);
                        if (!String.IsNullOrEmpty(configDir))
                        {
                            if (configDir[configDir.Length-1] != Path.DirectorySeparatorChar)
                            {
                                configDir += dirChar;
                            }

                            if (configDir.Equals("Web.config", StringComparison.OrdinalIgnoreCase))
                            {
                                configDir += "bin" + dirChar;
                            }

                            _basePath = configDir;
                        }
                    }

                    if (String.IsNullOrEmpty(_basePath))
                    {
                        string result = myDomain.BaseDirectory;

                        if (!String.IsNullOrEmpty(result) && 
                            (result[result.Length-1] != Path.DirectorySeparatorChar))
                        {
                            result += dirChar;
                        }

                        _basePath = result;
                    }
                }

                return _basePath;
            }
        }

        public static string ConvertToFullPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            string localPath = "";

            string applicationBaseDir = ApplicationBaseDirectory;
            if (!String.IsNullOrEmpty(applicationBaseDir))
            {
                try
                {
                    Uri uri = new Uri(applicationBaseDir);
                    if (uri.IsFile)
                    {
                        localPath = uri.LocalPath;
                    }
                }
                catch (ArgumentException) { }
                catch (UriFormatException)
                { }
            }

            if (!String.IsNullOrEmpty(localPath))
            {
                return Path.GetFullPath(Path.Combine(localPath, path));
            }

            return Path.GetFullPath(path);
        }
    }
}