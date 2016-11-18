using System;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public static class Predef
    {
        public static FunctionStm ArrayContains() { return "Contains"; }

        public static FunctionStm ArrayCount() { return "Count"; }

        public static FunctionStm ArrayIndexOf() { return "IndexOf"; }

        public static FunctionStm ArrayLastIndexOf() { return "LastIndexOf"; }

        public static FunctionStm ArrayLength() { return "Length"; }

        public static FunctionStm AddDate() { return "AddDate"; }

        public static FunctionStm AddTime() { return "AddTime"; }

        public static FunctionStm AddTimeToDate() { return "AddTimeToDate"; }

        public static FunctionStm SubtractTimeFromDate() { return "SubTimeFromDate"; }

        public static FunctionStm Date() { return "Date"; }

        public static FunctionStm MaxDate() { return "MaxDate"; }

        public static FunctionStm MinDate() { return "MinDate"; }

        public static FunctionStm MaxTime() { return "MaxTime"; }

        public static FunctionStm MinTime() { return "MinTime"; }

        public static FunctionStm Now() { return "Now"; }

        public static FunctionStm Time() { return "Time"; }

        public static FunctionStm DayName() { return "DayName"; }

        public static FunctionStm IsLeapYear() { return "IsLeapYear"; }

        public static FunctionStm MonthName() { return "MonthName"; }

        public static FunctionStm YearOf() { return "Year"; }

        public static FunctionStm MonthOf() { return "Month"; }

        public static FunctionStm DayOf() { return "Day"; }

        public static FunctionStm HourOf() { return "Hour"; }

        public static FunctionStm MinuteOf() { return "Minute"; }

        public static FunctionStm SecondOf() { return "Second"; }

        public static FunctionStm MilliSecondOf() { return "MilliSecond"; }

        public static FunctionStm LocalDate() { return "LocalDate"; }

        public static FunctionStm UtcDate() { return "UtcDate"; }

        public static FunctionStm Compare() { return "Compare"; }

        public static FunctionStm Debug() { return "Debug"; }

        public static FunctionStm Environment() { return "Environment"; }

        public static FunctionStm Equals() { return "Equals"; }

        public static FunctionStm IsNull() { return "IsNull"; }

        public static FunctionStm IsNumber() { return "IsNumber"; }

        public static FunctionStm Log() { return "Log"; }

        public static FunctionStm NewGuid() { return "NewGuid"; }

        public static FunctionStm Print() { return "Print"; }

        public static FunctionStm Printf() { return "Printf"; }

        public static FunctionStm PrintLine() { return "PrintLine"; }

        public static FunctionStm PrintLn() { return "PrintLine"; }

        public static FunctionStm Printfln() { return "Printfln"; }

        public static FunctionStm Write() { return "Print"; }

        public static FunctionStm Writef() { return "Printf"; }

        public static FunctionStm WriteLine() { return "PrintLine"; }

        public static FunctionStm WriteLn() { return "PrintLine"; }

        public static FunctionStm Writefln() { return "Printfln"; }

        public static FunctionStm Sleep() { return "Sleep"; }

        public static FunctionStm ToBoolean() { return "ToBoolean"; }

        public static FunctionStm ToDate() { return "ToDate"; }

        public static FunctionStm ToDouble() { return "ToDouble"; }

        public static FunctionStm ToGuid() { return "ToGuid"; }

        public static FunctionStm ToInteger() { return "ToInteger"; }

        public static FunctionStm ToString() { return "ToString"; }

        public static FunctionStm ToTime() { return "ToTime"; }

        public static FunctionStm Trace() { return "Trace"; }

        public static FunctionStm Abs() { return "Abs"; }

        public static FunctionStm Atan() { return "Atan"; }

        public static FunctionStm Avg() { return "Avg"; }

        public static FunctionStm Ceiling() { return "Ceiling"; }

        public static FunctionStm Cos() { return "Cos"; }

        public static FunctionStm Cosh() { return "Cosh"; }

        public static FunctionStm Floor() { return "Floor"; }

        public static FunctionStm Logarithm() { return "Logarithm"; }

        public static FunctionStm Log10() { return "Log10"; }

        public static FunctionStm Logarithm10() { return "Logarithm10"; }

        public static FunctionStm Mean() { return "Mean"; }

        public static FunctionStm Median() { return "Median"; }

        public static FunctionStm Max() { return "Max"; }

        public static FunctionStm Min() { return "Min"; }

        public static FunctionStm MaxFloat() { return "MaxFloat"; }

        public static FunctionStm MaxInteger() { return "MaxInteger"; }

        public static FunctionStm MinFloat() { return "MinFloat"; }

        public static FunctionStm MinInteger() { return "MinInteger"; }

        public static FunctionStm Pi() { return "Pi"; }

        public static FunctionStm Pow() { return "Pow"; }

        public static FunctionStm Round() { return "Round"; }

        public static FunctionStm Sin() { return "Sin"; }

        public static FunctionStm Sum() { return "Sum"; }

        public static FunctionStm Sqrt() { return "Sqrt"; }

        public static FunctionStm Tan() { return "Tan"; }

        public static FunctionStm Truncate() { return "Truncate"; }

        public static FunctionStm Concat() { return "Concat"; }

        public static FunctionStm EndsWith() { return "EndsWith"; }

        public static FunctionStm Format() { return "Format"; }

        public static FunctionStm Insert() { return "Insert"; }

        public static FunctionStm IsNullOrEmpty() { return "IsNullOrEmpty"; }

        public static FunctionStm Join() { return "Join"; }

        public static FunctionStm PadLeft() { return "PadLeft"; }

        public static FunctionStm PadRight() { return "PadRight"; }

        public static FunctionStm Remove() { return "Remove"; }

        public static FunctionStm Replace() { return "Replace"; }

        public static FunctionStm Split() { return "Split"; }

        public static FunctionStm StartsWith() { return "StartsWith"; }

        public static FunctionStm StrContains() { return "StrContains"; }

        public static FunctionStm StrIndexOf() { return "StrIndexOf"; }

        public static FunctionStm StrLastIndexOf() { return "StrLastIndexOf"; }

        public static FunctionStm Substring() { return "Substring"; }

        public static FunctionStm ToLower() { return "ToLower"; }

        public static FunctionStm ToUpper() { return "ToUpper"; }

        public static FunctionStm Trim() { return "Trim"; }

        public static FunctionStm TrimEnd() { return "TrimEnd"; }

        public static FunctionStm TrimStart() { return "TrimStart"; }
    }
}
