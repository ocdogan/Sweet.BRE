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
    public static class StmConstants
    {
        public const string NULL = "NULL";

        public const string TRUE = "TRUE";
        public const string FALSE = "FALSE";

        public const string OP_ASSIGN = "=";
        public const string OP_EQUALS = "==";
        public const string OP_NOT_EQUALS = "!=";
        public const string OP_GREATER_THAN = ">";
        public const string OP_GREATER_THAN_OR_EQUALS = ">=";
        public const string OP_LESS_THAN = "<";
        public const string OP_LESS_THAN_OR_EQUALS = "<=";

        public const string OP_AND = "AND";
        public const string OP_BETWEEN = "BETWEEN";
        public const string OP_IN = "IN";
        public const string OP_IS = "IS";
        public const string OP_IS_NOT = "IS NOT";
        public const string OP_LIKE = "LIKE";
        public const string OP_NOT = "NOT";
        public const string OP_NOT_IN = "NOT IN";
        public const string OP_NOT_LIKE = "NOT LIKE";
        public const string OP_OR = "OR";

        public const string CMD_IF = "IF";
        public const string CMD_THEN = "THEN";
        public const string CMD_ELSE = "ELSE";
        public const string CMD_NOP = "NOP()";
    }
}
