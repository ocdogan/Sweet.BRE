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
    public sealed class FunctionInfo
    {
        private string _name;
        private int _minParams;
        private int _maxParams;
        private ReturnType _returnType;
        private ValueType[] _paramTypes;
        private Dictionary<string, object> _aliasList;

        public FunctionInfo(string name, int minParams, int maxParams, ValueType[] paramTypes, ReturnType returnType)
        {
            _name = name;
            _minParams = minParams;
            _maxParams = maxParams;
            _returnType = returnType;
            _paramTypes = ((paramTypes != null) ? paramTypes : new ValueType[0]);
            _aliasList = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string[] Aliases
        {
            get
            {
                string[] result = new string[_aliasList.Count];
                if (_aliasList.Count > 0)
                {
                    _aliasList.Keys.CopyTo(result, 0);
                }

                return result;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
        
        public int MinParams
        {
            get
            {
                return _minParams;
            }
        }

        public int MaxParams
        {
            get
            {
                return _maxParams;
            }
        }

        public ValueType[] ParamTypes
        {
            get
            {
                return _paramTypes;
            }
        }

        public ReturnType ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        public FunctionInfo AddAlias(string aliasName)
        {
            if (!String.IsNullOrEmpty(aliasName))
            {
                _aliasList[aliasName] = null;
            }

            return this;
        }

        public FunctionInfo AddAliases(string[] aliasNames)
        {
            if ((aliasNames != null) && (aliasNames.Length > 0))
            {
                foreach (string aliasName in aliasNames)
                {
                    if (!String.IsNullOrEmpty(aliasName))
                    {
                        _aliasList[aliasName] = null;
                    }
                }
            }

            return this;
        }

        public FunctionInfo RemoveAlias(string aliasName)
        {
            if (!String.IsNullOrEmpty(aliasName) && _aliasList.ContainsKey(aliasName))
            {
                _aliasList.Remove(aliasName);
            }

            return this;
        }

        public FunctionInfo RemoveAlias(string[] aliasNames)
        {
            if ((aliasNames != null) && (aliasNames.Length > 0))
            {
                foreach (string aliasName in aliasNames)
                {
                    if (!String.IsNullOrEmpty(aliasName) && _aliasList.ContainsKey(aliasName))
                    {
                        _aliasList[aliasName] = null;
                    }
                }
            }

            return this;
        }

        public FunctionInfo ClearAliases()
        {
            _aliasList.Clear();
            return this;
        }

        public override bool Equals(object obj)
        {
            bool result = ReferenceEquals(this, obj);

            if (!result)
            {
                result = false;
                FunctionInfo objA = obj as FunctionInfo;

                if (!ReferenceEquals(objA, null))
                {
                    result = (_minParams == objA.MinParams) &&
                        (_maxParams == objA.MaxParams) && (_returnType == objA.ReturnType) &&
                        ReferenceEquals(_paramTypes, objA.ParamTypes);

                    if (result)
                    {
                        result = (String.Compare(_name, objA.Name) == 0);
                        if (!result)
                        {
                            string[] aliases = objA.Aliases;
                            if (aliases.Length == _aliasList.Count)
                            {
                                foreach (string alias in aliases)
                                {
                                    result = _aliasList.ContainsKey(alias);
                                    if (result)
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
