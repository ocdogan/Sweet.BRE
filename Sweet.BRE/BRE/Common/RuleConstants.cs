﻿/*
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
    public static class RuleConstants
    {
        public const string NULL = "NULL";

        public const string RULE = "Rule";
        public const string ENDRULE = "End Rule";

        public const string RULESET = "Ruleset";
        public const string ENDRULESET = "End Ruleset";

        public const string PROJECT = "Project";
        public const string ENDPROJECT = "End Project";

        public const string EVALTREE = "EvalTree";
        public const string EVALTABLE = "EvalTable";

        public const string CALL = "Call";
        public const string DEFINE = "Defin";
        public const string RAISE = "Raise";
        public const string REFLECT = "Reflect";
        public const string FACT = "Fact";

        public const string SWITCH = "Switch";
        public const string CASE = "Case";
        public const string DEFAULT = "Default";
        public const string ENDCASE = "End Case";
        public const string ENDSWITCH = "End Switch";

        public const string SET = "Set";

        public const string TRY = "Try";
        public const string ONERROR = "OnError";
        public const string FINALLY = "Finally";
        public const string ENDTRY = "End Try";
        public const string ENDONERROR = "End OnError";
        public const string ENDFINALLY = "End Finally";

        public const string HALT = "Halt";
        public const string BREAK = "Break";
        public const string CONTINUE = "Continue";
        public const string RETURN = "Return";
        public const string CHECK = "Check";

        public const string BEGIN = "Begin";
        public const string END = "End";

        public const string FOR = "For";
        public const string FROM = "From";
        public const string TO = "To";
        public const string STEP = "Step";
        public const string ENDLOOP = "End Loop";

        public const string DO = "Do";
        public const string WHILE = "While";

        public const string REPEAT = "Repeat";
        public const string UNTIL = "Until";

        public const string IF = "If";
        public const string THEN = "Then";
        public const string ELSE = "Else";
        public const string OTHERWISE = "Otherwise";
        public const string ENDIF = "End If";
    }
}
