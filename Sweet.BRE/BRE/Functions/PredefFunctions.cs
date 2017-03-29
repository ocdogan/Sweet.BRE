using System;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public static class Predef
    {
        public static FunctionStm ArrayContains(params Statement[] parameters) { return ((FunctionStm)"Contains").Params(parameters); }

        public static FunctionStm ArrayCount(params Statement[] parameters) { return ((FunctionStm)"Count").Params(parameters); }

        public static FunctionStm ArrayIndexOf(params Statement[] parameters) { return ((FunctionStm)"IndexOf").Params(parameters); }

        public static FunctionStm ArrayLastIndexOf(params Statement[] parameters) { return ((FunctionStm)"LastIndexOf").Params(parameters); }

        public static FunctionStm ArrayLength(params Statement[] parameters) { return ((FunctionStm)"Length").Params(parameters); }

        public static FunctionStm AddDate(params Statement[] parameters) { return ((FunctionStm)"AddDate").Params(parameters); }

        public static FunctionStm AddTime(params Statement[] parameters) { return ((FunctionStm)"AddTime").Params(parameters); }

        public static FunctionStm AddTimeToDate(params Statement[] parameters) { return ((FunctionStm)"AddTimeToDate").Params(parameters); }

        public static FunctionStm SubtractTimeFromDate(params Statement[] parameters) { return ((FunctionStm)"SubTimeFromDate").Params(parameters); }

        public static FunctionStm Date(params Statement[] parameters) { return ((FunctionStm)"Date").Params(parameters); }

        public static FunctionStm MaxDate(params Statement[] parameters) { return ((FunctionStm)"MaxDate").Params(parameters); }

        public static FunctionStm MinDate(params Statement[] parameters) { return ((FunctionStm)"MinDate").Params(parameters); }

        public static FunctionStm MaxTime(params Statement[] parameters) { return ((FunctionStm)"MaxTime").Params(parameters); }

        public static FunctionStm MinTime(params Statement[] parameters) { return ((FunctionStm)"MinTime").Params(parameters); }

        public static FunctionStm Now(params Statement[] parameters) { return ((FunctionStm)"Now").Params(parameters); }

        public static FunctionStm Time(params Statement[] parameters) { return ((FunctionStm)"Time").Params(parameters); }

        public static FunctionStm DayName(params Statement[] parameters) { return ((FunctionStm)"DayName").Params(parameters); }

        public static FunctionStm IsLeapYear(params Statement[] parameters) { return ((FunctionStm)"IsLeapYear").Params(parameters); }

        public static FunctionStm MonthName(params Statement[] parameters) { return ((FunctionStm)"MonthName").Params(parameters); }

        public static FunctionStm YearOf(params Statement[] parameters) { return ((FunctionStm)"Year").Params(parameters); }

        public static FunctionStm MonthOf(params Statement[] parameters) { return ((FunctionStm)"Month").Params(parameters); }

        public static FunctionStm DayOf(params Statement[] parameters) { return ((FunctionStm)"Day").Params(parameters); }

        public static FunctionStm HourOf(params Statement[] parameters) { return ((FunctionStm)"Hour").Params(parameters); }

        public static FunctionStm MinuteOf(params Statement[] parameters) { return ((FunctionStm)"Minute").Params(parameters); }

        public static FunctionStm SecondOf(params Statement[] parameters) { return ((FunctionStm)"Second").Params(parameters); }

        public static FunctionStm MilliSecondOf(params Statement[] parameters) { return ((FunctionStm)"MilliSecond").Params(parameters); }

        public static FunctionStm LocalDate(params Statement[] parameters) { return ((FunctionStm)"LocalDate").Params(parameters); }

        public static FunctionStm UtcDate(params Statement[] parameters) { return ((FunctionStm)"UtcDate").Params(parameters); }

        public static FunctionStm Compare(params Statement[] parameters) { return ((FunctionStm)"Compare").Params(parameters); }

        public static FunctionStm Debug(params Statement[] parameters) { return ((FunctionStm)"Debug").Params(parameters); }

        public static FunctionStm Environment(params Statement[] parameters) { return ((FunctionStm)"Environment").Params(parameters); }

        public static FunctionStm Equals(params Statement[] parameters) { return ((FunctionStm)"Equals").Params(parameters); }

        public static FunctionStm IsNull(params Statement[] parameters) { return ((FunctionStm)"IsNull").Params(parameters); }

        public static FunctionStm IsNumber(params Statement[] parameters) { return ((FunctionStm)"IsNumber").Params(parameters); }

        public static FunctionStm Log(params Statement[] parameters) { return ((FunctionStm)"Log").Params(parameters); }

        public static FunctionStm NewGuid(params Statement[] parameters) { return ((FunctionStm)"NewGuid").Params(parameters); }

        public static FunctionStm Print(params Statement[] parameters) { return ((FunctionStm)"Print").Params(parameters); }

        public static FunctionStm Printf(params Statement[] parameters) { return ((FunctionStm)"Printf").Params(parameters); }

        public static FunctionStm PrintLine(params Statement[] parameters) { return ((FunctionStm)"PrintLine").Params(parameters); }

        public static FunctionStm PrintLn(params Statement[] parameters) { return ((FunctionStm)"PrintLine").Params(parameters); }

        public static FunctionStm Printfln(params Statement[] parameters) { return ((FunctionStm)"Printfln").Params(parameters); }

        public static FunctionStm Write(params Statement[] parameters) { return ((FunctionStm)"Print").Params(parameters); }

        public static FunctionStm Writef(params Statement[] parameters) { return ((FunctionStm)"Printf").Params(parameters); }

        public static FunctionStm WriteLine(params Statement[] parameters) { return ((FunctionStm)"PrintLine").Params(parameters); }

        public static FunctionStm WriteLn(params Statement[] parameters) { return ((FunctionStm)"PrintLine").Params(parameters); }

        public static FunctionStm Writefln(params Statement[] parameters) { return ((FunctionStm)"Printfln").Params(parameters); }

        public static FunctionStm Sleep(params Statement[] parameters) { return ((FunctionStm)"Sleep").Params(parameters); }

        public static FunctionStm ToBoolean(params Statement[] parameters) { return ((FunctionStm)"ToBoolean").Params(parameters); }

        public static FunctionStm ToDate(params Statement[] parameters) { return ((FunctionStm)"ToDate").Params(parameters); }

        public static FunctionStm ToDouble(params Statement[] parameters) { return ((FunctionStm)"ToDouble").Params(parameters); }

        public static FunctionStm ToGuid(params Statement[] parameters) { return ((FunctionStm)"ToGuid").Params(parameters); }

        public static FunctionStm ToInteger(params Statement[] parameters) { return ((FunctionStm)"ToInteger").Params(parameters); }

        public new static FunctionStm ToString(params Statement[] parameters) { return ((FunctionStm)"ToString").Params(parameters); }

        public static FunctionStm ToTime(params Statement[] parameters) { return ((FunctionStm)"ToTime").Params(parameters); }

        public static FunctionStm Trace(params Statement[] parameters) { return ((FunctionStm)"Trace").Params(parameters); }

        public static FunctionStm Abs(params Statement[] parameters) { return ((FunctionStm)"Abs").Params(parameters); }

        public static FunctionStm Atan(params Statement[] parameters) { return ((FunctionStm)"Atan").Params(parameters); }

        public static FunctionStm Avg(params Statement[] parameters) { return ((FunctionStm)"Avg").Params(parameters); }

        public static FunctionStm Ceiling(params Statement[] parameters) { return ((FunctionStm)"Ceiling").Params(parameters); }

        public static FunctionStm Cos(params Statement[] parameters) { return ((FunctionStm)"Cos").Params(parameters); }

        public static FunctionStm Cosh(params Statement[] parameters) { return ((FunctionStm)"Cosh").Params(parameters); }

        public static FunctionStm Floor(params Statement[] parameters) { return ((FunctionStm)"Floor").Params(parameters); }

        public static FunctionStm Logarithm(params Statement[] parameters) { return ((FunctionStm)"Logarithm").Params(parameters); }

        public static FunctionStm Log10(params Statement[] parameters) { return ((FunctionStm)"Log10").Params(parameters); }

        public static FunctionStm Logarithm10(params Statement[] parameters) { return ((FunctionStm)"Logarithm10").Params(parameters); }

        public static FunctionStm Mean(params Statement[] parameters) { return ((FunctionStm)"Mean").Params(parameters); }

        public static FunctionStm Median(params Statement[] parameters) { return ((FunctionStm)"Median").Params(parameters); }

        public static FunctionStm Max(params Statement[] parameters) { return ((FunctionStm)"Max").Params(parameters); }

        public static FunctionStm Min(params Statement[] parameters) { return ((FunctionStm)"Min").Params(parameters); }

        public static FunctionStm MaxFloat(params Statement[] parameters) { return ((FunctionStm)"MaxFloat").Params(parameters); }

        public static FunctionStm MaxInteger(params Statement[] parameters) { return ((FunctionStm)"MaxInteger").Params(parameters); }

        public static FunctionStm MinFloat(params Statement[] parameters) { return ((FunctionStm)"MinFloat").Params(parameters); }

        public static FunctionStm MinInteger(params Statement[] parameters) { return ((FunctionStm)"MinInteger").Params(parameters); }

        public static FunctionStm Pi(params Statement[] parameters) { return ((FunctionStm)"Pi").Params(parameters); }

        public static FunctionStm Pow(params Statement[] parameters) { return ((FunctionStm)"Pow").Params(parameters); }

        public static FunctionStm Round(params Statement[] parameters) { return ((FunctionStm)"Round").Params(parameters); }

        public static FunctionStm Sin(params Statement[] parameters) { return ((FunctionStm)"Sin").Params(parameters); }

        public static FunctionStm Sum(params Statement[] parameters) { return ((FunctionStm)"Sum").Params(parameters); }

        public static FunctionStm Sqrt(params Statement[] parameters) { return ((FunctionStm)"Sqrt").Params(parameters); }

        public static FunctionStm Tan(params Statement[] parameters) { return ((FunctionStm)"Tan").Params(parameters); }

        public static FunctionStm Truncate(params Statement[] parameters) { return ((FunctionStm)"Truncate").Params(parameters); }

        public static FunctionStm Concat(params Statement[] parameters) { return ((FunctionStm)"Concat").Params(parameters); }

        public static FunctionStm EndsWith(params Statement[] parameters) { return ((FunctionStm)"EndsWith").Params(parameters); }

        public static FunctionStm Format(params Statement[] parameters) { return ((FunctionStm)"Format").Params(parameters); }

        public static FunctionStm Insert(params Statement[] parameters) { return ((FunctionStm)"Insert").Params(parameters); }

        public static FunctionStm IsNullOrEmpty(params Statement[] parameters) { return ((FunctionStm)"IsNullOrEmpty").Params(parameters); }

        public static FunctionStm Join(params Statement[] parameters) { return ((FunctionStm)"Join").Params(parameters); }

        public static FunctionStm PadLeft(params Statement[] parameters) { return ((FunctionStm)"PadLeft").Params(parameters); }

        public static FunctionStm PadRight(params Statement[] parameters) { return ((FunctionStm)"PadRight").Params(parameters); }

        public static FunctionStm Remove(params Statement[] parameters) { return ((FunctionStm)"Remove").Params(parameters); }

        public static FunctionStm Replace(params Statement[] parameters) { return ((FunctionStm)"Replace").Params(parameters); }

        public static FunctionStm Split(params Statement[] parameters) { return ((FunctionStm)"Split").Params(parameters); }

        public static FunctionStm StartsWith(params Statement[] parameters) { return ((FunctionStm)"StartsWith").Params(parameters); }

        public static FunctionStm StrContains(params Statement[] parameters) { return ((FunctionStm)"StrContains").Params(parameters); }

        public static FunctionStm StrIndexOf(params Statement[] parameters) { return ((FunctionStm)"StrIndexOf").Params(parameters); }

        public static FunctionStm StrLastIndexOf(params Statement[] parameters) { return ((FunctionStm)"StrLastIndexOf").Params(parameters); }

        public static FunctionStm Substring(params Statement[] parameters) { return ((FunctionStm)"Substring").Params(parameters); }

        public static FunctionStm ToLower(params Statement[] parameters) { return ((FunctionStm)"ToLower").Params(parameters); }

        public static FunctionStm ToUpper(params Statement[] parameters) { return ((FunctionStm)"ToUpper").Params(parameters); }

        public static FunctionStm Trim(params Statement[] parameters) { return ((FunctionStm)"Trim").Params(parameters); }

        public static FunctionStm TrimEnd(params Statement[] parameters) { return ((FunctionStm)"TrimEnd").Params(parameters); }

        public static FunctionStm TrimStart(params Statement[] parameters) { return ((FunctionStm)"TrimStart").Params(parameters); }
    }
}
